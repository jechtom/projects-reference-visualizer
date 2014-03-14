using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReferenceVisualizer.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();
            layoutAlgorithms.DataContext = new GraphSharp.Algorithms.Layout.StandardLayoutAlgorithmFactory<object, IEdge<object>, IBidirectionalGraph<object,IEdge<object>>>().AlgorithmTypes;
        }

        private void ApplyLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var algorihmType = (string)fe.DataContext;
            graphLayout.LayoutAlgorithmType = algorihmType;
            zoomControl.ZoomToFill();
        }
    }
}
