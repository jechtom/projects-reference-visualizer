using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class DotNetProjectGraphDiscoveryService : IGraphDiscoveryService
    {
        private CsprojFileDataReader csprojFileReader = new CsprojFileDataReader();
        private SolutionFileDataReader slnFileReader = new SolutionFileDataReader();
        private IDotNetProjectGraphBuilder builder;

        public string FolderPath { get; set; }

        public string FolderPathFullPath
        {
            get
            {
                if (FolderPath == null)
                    return null;
                return Path.GetFullPath(FolderPath);
            }
        }

        public DotNetProjectGraphDiscoveryService(IDotNetProjectGraphBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");

            this.builder = builder;
        }

        public Task<GraphData> Resolve(System.Threading.CancellationToken cancellationToken, IProgress<DiscoveryProgress> progress)
        {
            return Task.Factory.StartNew<GraphData>(() => {
                
                // read *.csproj projects
                var projects = FindCsprojFiles(cancellationToken, progress);
                cancellationToken.ThrowIfCancellationRequested();
                
                // read *.sln files
                var solutions = FindSlnFiles(cancellationToken, progress);
                cancellationToken.ThrowIfCancellationRequested();
                
                // build result
                var result = builder.Build(this, projects, solutions);
                return result;
            });
        }

        private ICollection<CsprojFileData> FindCsprojFiles(System.Threading.CancellationToken cancellationToken, IProgress<DiscoveryProgress> progress)
        {
            var files = new List<CsprojFileData>();

            // find csproj files
            foreach (var fileName in System.IO.Directory.EnumerateFiles(FolderPath, "*.csproj", System.IO.SearchOption.AllDirectories))
            {
                cancellationToken.ThrowIfCancellationRequested();

                progress.Report(new DiscoveryProgress() { CurrentItem = fileName });

                CsprojFileData csprojData = null;
                try
                {
                    csprojData = csprojFileReader.ReadFromFile(fileName);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error reading CSPROJ: " + e.Message);
                }

                if (csprojData != null)
                    files.Add(csprojData);
            }

            return files;
        }

        private ICollection<SolutionFileData> FindSlnFiles(System.Threading.CancellationToken cancellationToken, IProgress<DiscoveryProgress> progress)
        {
            var files = new List<SolutionFileData>();

            // find csproj files
            foreach (var fileName in System.IO.Directory.EnumerateFiles(FolderPath, "*.sln", System.IO.SearchOption.AllDirectories))
            {
                cancellationToken.ThrowIfCancellationRequested();

                progress.Report(new DiscoveryProgress() { CurrentItem = fileName });

                SolutionFileData slnData = null;
                try
                {
                    slnData = slnFileReader.ReadFromFile(fileName);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error reading SLN: " + e.Message);
                }

                if (slnData != null)
                    files.Add(slnData);
            }

            return files;
        }
    }
}
