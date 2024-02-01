﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class SolutionProjectGraphBuilder
    {
        public GraphData Build(string scopePath, DotNetProjectsData projectData)
        {
            var graphData = new GraphData();

            // crete nodes from csproj data
            var projectsWithNode = projectData.CsprojFiles.Select(fd => new
            {
                FileData = fd,
                Node = CreateNodeFromCsprojFileData(fd, NodeState.Normal)
            }).ToList();

            var solutionsWithNode = projectData.SolutionFiles.Select(fd => new
            {
                FileData = fd,
                Node = CreateNodeFromSolutionFileData(fd, NodeState.Normal)
            }).ToList();

            // references
            graphData.References = new List<DependenceDefinition>();
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
                        bool outOfBound = PathHelper.IsDescendant(scopePath, reference.FullPath);
                        var missingReferenceData = new CsprojFileData() { FileName = reference.FullPath };
                        referencedNode = new
                        {
                            FileData = missingReferenceData,
                            Node = CreateNodeFromCsprojFileData(missingReferenceData, outOfBound ? NodeState.OutOfContext : NodeState.NotFound)
                        };
                        projectsWithNode.Add(referencedNode);
                    }

                    // create reference
                    graphData.References.Add(new DependenceDefinition()
                        {
                            DependentNodeId = item.Node.Id,
                            DependenceNodeId = referencedNode.Node.Id,
                            Type = "project"
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
                    Id = "dotnet.csproj." + data.FileName,
                    Name = Path.GetFileNameWithoutExtension(data.FileName),
                    Path = data.FileName,
                    State = state,
                    Type = "dotnet.csproj"
                };
        }

        private NodeDefinition CreateNodeFromSolutionFileData(SolutionFileData data, NodeState state)
        {
            return new NodeDefinition()
            {
                Id = "dotnet.sln." + data.FileName,
                Name = Path.GetFileNameWithoutExtension(data.FileName),
                Path = data.FileName,
                State = state,
                Type = "dotnet.sln"
            };
        }
    }
}
