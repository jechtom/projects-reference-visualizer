using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class DotNetProjectsData
    {
        public ICollection<CsprojFileData> CsprojFiles { get; set; }
        public ICollection<SolutionFileData> SolutionFiles { get; set; }
    }
}
