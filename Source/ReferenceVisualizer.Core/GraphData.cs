using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReferenceVisualizer.Core
{
    public class GraphData
    {
        public ICollection<NodeDefinition> Nodes { get; set; }
        public ICollection<ReferenceDefinition> References { get; set; }
    }
}
