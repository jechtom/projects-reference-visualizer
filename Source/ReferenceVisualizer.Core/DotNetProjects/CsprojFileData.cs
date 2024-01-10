using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class CsprojFileData
    {
        public string FileName { get; set; }

        public ICollection<string> References { get; set; }
    }
}
