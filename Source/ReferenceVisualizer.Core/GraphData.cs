using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReferenceVisualizer.Core
{
    public class GraphData
    {
        public IDictionary<string, NodeDefinition> Nodes { get; set; }
        public ICollection<DependenceDefinition> References { get; set; }
    }
}
