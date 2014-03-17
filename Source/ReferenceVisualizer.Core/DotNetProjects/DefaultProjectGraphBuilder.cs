using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class DefaultProjectGraphBuilder : IDotNetProjectGraphBuilder
    {
        public GraphData Build(DotNetProjectGraphDiscoveryService service, ICollection<CsprojFileData> projects, ICollection<SolutionFileData> solutions)
        {
            var graphData = new GraphData();

            // crete nodes from csproj data
            var data = projects.Select(fd => new
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
                        bool outOfBound = referencePath.IndexOf(service.FolderPathFullPath, StringComparison.OrdinalIgnoreCase) == -1; 
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
            graphData.Nodes = data.Select(d => d.Node).ToDictionary(d => d.Id);
            return graphData;
        }

        private NodeDefinition CreateNodeFromCsprojFileData(CsprojFileData data, NodeState state)
        {
            return new NodeDefinition()
                {
                    Id = "project.csproj." + data.FileName,
                    Name = Path.GetFileNameWithoutExtension(data.FileName),
                    Path = data.FileName,
                    State = state,
                    Type = "project.csproj"
                };
        }
    }
}
