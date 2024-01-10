using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReferenceVisualizer.Core
{
    public enum NodeState
    {
        /// <summary>
        /// Regular node.
        /// </summary>
        Normal,

        /// <summary>
        /// Reference found to this node but node itself was not found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Node is out of searched scope.
        /// </summary>
        OutOfContext
    }
}
