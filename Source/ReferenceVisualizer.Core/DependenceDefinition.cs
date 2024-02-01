using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReferenceVisualizer.Core
{
    public class DependenceDefinition
    {
        public string DependentNodeId { get; set; }
        public string DependenceNodeId { get; set; }
        public string Type { get; set; }
    }
}
