using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core
{
    public class NodeGroupingProcessor
    {
        public NodeGroupingProcessor(Func<NodeDefinition, string> groupFunc)
        {
            GroupFunc = groupFunc;
        }

        public Func<NodeDefinition, string> GroupFunc { get; }

        public GraphData Group(GraphData data)
        {
            var result = new GraphData()
            { 
                Nodes = new Dictionary<string, NodeDefinition>(),
                References = new List<DependenceDefinition>()
            };

            var groups = new Dictionary<string, List<NodeDefinition>>();
            var groupsByContent = new Dictionary<string, string>();
            foreach (var n in data.Nodes)
            {
                string groupName = GroupFunc(n.Value);
                if(groupName == null)
                {
                    result.Nodes.Add(n);
                    continue;
                }

                if(!groups.TryGetValue(groupName, out List<NodeDefinition> nodes))
                {
                    groups.Add(groupName, nodes = new List<NodeDefinition>());
                }

                nodes.Add(n.Value);
                groupsByContent.Add(n.Value.Id, groupName);
            }

            // process groups
            StringBuilder sb = new StringBuilder();
            foreach (var group in groups)
            {
                sb.Clear();
                sb.AppendLine(group.Key);
                sb.AppendLine();
                foreach (var item in group.Value)
                {
                    sb.AppendLine(item.Path);
                }



                var node = new NodeDefinition()
                {
                    Id = $"group.{group.Key}",
                    Name = $"{group.Key} ({group.Value.Count} projects)",
                    State = NodeState.Normal,
                    Note = sb.ToString(),
                    Type = "group",
                    Path = string.Empty
                };
                result.Nodes.Add(group.Key, node);
            }

            foreach (var rf in data.References)
            {
                bool fromInGroup = groupsByContent.TryGetValue(rf.DependentNodeId, out string fromGroup);
                bool toInGroup = groupsByContent.TryGetValue(rf.DependenceNodeId, out string toGroup);
                if(!fromInGroup && !toInGroup)
                {
                    result.References.Add(rf);
                    continue;
                }

                if(fromInGroup && toInGroup && fromGroup == toGroup)
                {
                    // link between inside the group
                    continue;
                }

                result.References.Add(new DependenceDefinition()
                {
                    DependentNodeId = fromInGroup ? fromGroup : rf.DependentNodeId,
                    DependenceNodeId = toInGroup ? toGroup : rf.DependenceNodeId
                });
            }

            return result;
        }
    }
}
