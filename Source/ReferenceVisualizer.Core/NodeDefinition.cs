using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core
{
    public class NodeDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public NodeState State { get; set; }
    }
}
