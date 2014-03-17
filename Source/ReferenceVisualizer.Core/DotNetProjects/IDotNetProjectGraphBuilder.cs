using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public interface IDotNetProjectGraphBuilder
    {
        GraphData Build(DotNetProjectGraphDiscoveryService service, ICollection<CsprojFileData> projects, ICollection<SolutionFileData> solutions);
    }
}
