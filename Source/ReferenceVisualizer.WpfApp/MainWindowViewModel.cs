using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ReferenceVisualizer.WpfApp
{
    public class MainWindowViewModel : DependencyObject
    {
        public MainWindowViewModel()
        {
            LoadCommand = new FuncCommand(Load);
            BrowseCommand = new FuncCommand(Browse);
#if DEBUG
            this.Path = @"d:\Avast.InternalSystem";
#endif
        }

        public FuncCommand LoadCommand { get; private set; }
        public FuncCommand BrowseCommand { get; private set; }

        public async void Load()
        {
            Core.IGraphDiscoveryService discoveryService;
            discoveryService = new Core.DotNetProjects.DotNetProjectGraphDiscoveryService()
            {
                FolderPath = this.Path
            };

            try
            {
                LoadCommand.ChangeCanExecuteChanged(false);
                BrowseCommand.ChangeCanExecuteChanged(false);

                var result = await discoveryService.Resolve(new CancellationToken(), new Progress<Core.DiscoveryProgress>());
                this.Graph = CreateUIGraphFromData(result);
            }
            finally
            {
                LoadCommand.ChangeCanExecuteChanged(true);
                BrowseCommand.ChangeCanExecuteChanged(true);
            }
        }

        public void Browse()
        {
            using(var dialog = new FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                dialog.SelectedPath = this.Path;
                dialog.ShowNewFolderButton = false;
                var result = dialog.ShowDialog();
                if(result == DialogResult.OK)
                {
                    this.Path = dialog.SelectedPath;
                }
            }
            
        }

        public IBidirectionalGraph<object, QuickGraph.IEdge<object>> Graph
        {
            get { return (IBidirectionalGraph<object, QuickGraph.IEdge<object>>)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof(IBidirectionalGraph<object, QuickGraph.IEdge<object>>), typeof(MainWindowViewModel), new PropertyMetadata(null));

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(""));


        private IBidirectionalGraph<object, IEdge<object>> CreateUIGraphFromData(Core.GraphData data)
        {
            var result = new QuickGraph.BidirectionalGraph<object, QuickGraph.IEdge<object>>();

            // build key map
            Dictionary<string, object> keyMap = data.Nodes.ToDictionary(n => n.Id, n => (object)n);

            // add nodes
            result.AddVertexRange(data.Nodes);

            foreach (var reference in data.References)
            {
                object from = keyMap[reference.NodeFromId];
                object to = keyMap[reference.NodeToId];

                result.AddEdge(new QuickGraph.Edge<object>(from, to));
            }

            return result;
        }
    }
}
