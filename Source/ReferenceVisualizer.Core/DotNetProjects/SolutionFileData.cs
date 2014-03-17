using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class SolutionFileData
    {
        public ICollection<SolutionProjectData> Projects { get; set; }

        public string FileName { get; set; }
    }
}
