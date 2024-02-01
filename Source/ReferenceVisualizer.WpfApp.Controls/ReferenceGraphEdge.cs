using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReferenceVisualizer.WpfApp.Controls
{
    public class ReferenceGraphEdge : IEdge<Core.NodeDefinition>
    {
        public ReferenceGraphEdge(Core.NodeDefinition source, Core.NodeDefinition target)
        {
            Source = source;
            Target = target;
        }

        public Core.NodeDefinition Source { get; set; }

        public Core.NodeDefinition Target { get; set; }
    }
}
