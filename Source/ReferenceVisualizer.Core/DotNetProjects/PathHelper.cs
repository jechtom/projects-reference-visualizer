using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public static class PathHelper
    {
        public static bool IsDescendant(string parent, string descendant)
        {
            parent = Path.GetFullPath(parent);
            descendant = Path.GetFullPath(descendant);
            bool result = descendant.IndexOf(parent, 0, StringComparison.OrdinalIgnoreCase) == 0;
            return result;
        }
    }
}
