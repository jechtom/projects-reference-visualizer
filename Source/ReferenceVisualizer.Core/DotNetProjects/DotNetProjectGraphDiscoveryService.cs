using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class DotNetProjectGraphDiscoveryService : IGraphDiscoveryService
    {
        private CsprojFileDataReader csprojFileReader = new CsprojFileDataReader();

        public string FolderPath { get; set; }

        public string FolderPathFullPath
        {
            get
            {
                if (FolderPath == null)
                    return null;
                return Path.GetFullPath(FolderPath);
            }
        }

        public Task<GraphData> Resolve(System.Threading.CancellationToken cancellationToken, IProgress<DiscoveryProgress> progress)
        {
            return Task.Factory.StartNew<GraphData>(() => {
                var files = new List<CsprojFileData>();

                // find csproj files
                foreach (var fileName in System.IO.Directory.EnumerateFiles(FolderPath, "*.csproj", System.IO.SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    progress.Report(new DiscoveryProgress() { CurrentItem = fileName });

                    CsprojFileData csprojData = null;
                    try
                    {
                        csprojData = csprojFileReader.ReadFromFile(fileName);
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine("Error reading CSPROJ: " + e.Message);
                    }

                    if(csprojData != null)
                        files.Add(csprojData);
                }

                cancellationToken.ThrowIfCancellationRequested();

                // build result
                var result = new GraphData();
                ApplyToGraphData(result, files);
                return result;
            });
        }

        private void ApplyToGraphData(GraphData graphData, ICollection<CsprojFileData> fileData)
        {
            // crete nodes from csproj data
            var data = fileData.Select(fd => new
            {
                FileData = fd,
                Node = CreateNodeFromCsprojFileData(fd, NodeState.Normal)
            }).ToList();

            // references
            graphData.References = new List<ReferenceDefinition>();
            foreach (var item in data.ToArray())
            {
                var directory = Path.GetDirectoryName(item.FileData.FileName);

                foreach (var reference in item.FileData.References)
                {
                    // resolve path and find csproj file
                    string referencePath = Path.GetFullPath(Path.Combine(directory, reference));
                    var referencedNode = data.FirstOrDefault(i => string.Equals(i.FileData.FileName, referencePath, StringComparison.OrdinalIgnoreCase));

                    if (referencedNode == null)
                    {
                        // not found - create "not found" node
                        bool outOfBound = referencePath.IndexOf(this.FolderPathFullPath, StringComparison.OrdinalIgnoreCase) == -1; 
                        var missingReferenceData = new CsprojFileData() { FileName = referencePath };
                        referencedNode = new
                        {
                            FileData = missingReferenceData,
                            Node = CreateNodeFromCsprojFileData(missingReferenceData, outOfBound ? NodeState.OutOfBound : NodeState.NotFound)
                        };
                        data.Add(referencedNode);
                    }

                    // create reference
                    graphData.References.Add(new ReferenceDefinition()
                        {
                            NodeFromId = item.Node.Id,
                            NodeToId = referencedNode.Node.Id
                        });
                }
            }

            // nodes
            graphData.Nodes = data.Select(d => d.Node).ToList();
        }

        private NodeDefinition CreateNodeFromCsprojFileData(CsprojFileData data, NodeState state)
        {
            return new NodeDefinition()
                {
                    Id = "csproj." + data.FileName,
                    Name = Path.GetFileNameWithoutExtension(data.FileName),
                    Path = data.FileName,
                    State = state
                };
        }
    }
}
