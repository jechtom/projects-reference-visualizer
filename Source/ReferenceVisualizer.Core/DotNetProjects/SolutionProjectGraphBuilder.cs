using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class SolutionProjectGraphBuilder : IDotNetProjectGraphBuilder
    {
        public GraphData Build(DotNetProjectGraphDiscoveryService service, ICollection<CsprojFileData> projects, ICollection<SolutionFileData> solutions)
        {
            var graphData = new GraphData();

            // crete nodes from csproj data
            var projectsWithNode = projects.Select(fd => new
            {
                FileData = fd,
                Node = CreateNodeFromCsprojFileData(fd, NodeState.Normal)
            }).ToList();

            var solutionsWithNode = solutions.Select(fd => new
            {
                FileData = fd,
                Node = CreateNodeFromSolutionFileData(fd, NodeState.Normal)
            }).ToList();

            // references
            graphData.References = new List<ReferenceDefinition>();
            foreach (var item in solutionsWithNode)
            {
                var directory = Path.GetDirectoryName(item.FileData.FileName);

                foreach (var reference in item.FileData.Projects)
                {
                    // resolve path and find csproj file
                    var referencedNode = projectsWithNode.FirstOrDefault(i => i.FileData != null && string.Equals(i.FileData.FileName, reference.FullPath, StringComparison.OrdinalIgnoreCase));

                    if (referencedNode == null)
                    {
                        // not found - create "not found" node
                        bool outOfBound = PathHelper.IsDescendant(service.FolderPathFullPath, reference.FullPath);
                        var missingReferenceData = new CsprojFileData() { FileName = reference.FullPath };
                        referencedNode = new
                        {
                            FileData = missingReferenceData,
                            Node = CreateNodeFromCsprojFileData(missingReferenceData, outOfBound ? NodeState.OutOfBound : NodeState.NotFound)
                        };
                        projectsWithNode.Add(referencedNode);
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
            graphData.Nodes = 
                projectsWithNode.Select(d => d.Node)
                .Union(solutionsWithNode.Select(d => d.Node))
                .ToDictionary(d => d.Id);

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

        private NodeDefinition CreateNodeFromSolutionFileData(SolutionFileData data, NodeState state)
        {
            return new NodeDefinition()
            {
                Id = "project.solution." + data.FileName,
                Name = Path.GetFileNameWithoutExtension(data.FileName),
                Path = data.FileName,
                State = state,
                Type = "project.solution"
            };
        }
    }
}
