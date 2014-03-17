using ReferenceVisualizer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.WpfApp.Controls
{
    public class GraphDataConvertor
    {
        public object ConvertToGraph(GraphData data)
        {
            var result = new QuickGraph.BidirectionalGraph<NodeDefinition, ReferenceGraphEdge>();

            // add nodes
            result.AddVertexRange(data.Nodes.Values);

            foreach (var reference in data.References)
            {
                NodeDefinition from = data.Nodes[reference.NodeFromId];
                NodeDefinition to = data.Nodes[reference.NodeToId];

                result.AddEdge(new ReferenceGraphEdge(from, to));
            }

            return result;
        }
    }
}
