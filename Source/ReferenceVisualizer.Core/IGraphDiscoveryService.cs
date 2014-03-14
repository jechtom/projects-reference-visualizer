using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReferenceVisualizer.Core
{
    public interface IGraphDiscoveryService
    {
        Task<GraphData> Resolve(CancellationToken cancellationToken, IProgress<DiscoveryProgress> progress);
    }
}
