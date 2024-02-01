using GraphSharp.Controls;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.WpfApp.Controls
{
    public class ReferenceGraphLayout : GraphLayout<
        Core.NodeDefinition, 
        ReferenceGraphEdge, 
        BidirectionalGraph<Core.NodeDefinition, ReferenceGraphEdge>
    >
    {
        public ReferenceGraphLayout()
        {
        }
    }


}
