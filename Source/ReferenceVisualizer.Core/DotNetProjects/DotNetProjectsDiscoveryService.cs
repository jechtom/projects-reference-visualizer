using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class DotNetProjectsDiscoveryService
    {
        private CsprojFileDataReader csprojFileReader = new CsprojFileDataReader();
        private SolutionFileDataReader slnFileReader = new SolutionFileDataReader();

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

        public DotNetProjectsData Discover(CancellationToken cancellationToken, IProgress<DiscoveryProgress> progress)
        {
            var csprojFiles = new List<CsprojFileData>();
            var slnFiles = new List<SolutionFileData>();

            // read *.sln and *.csproj
            foreach (var item in DiscoverAndParseFiles(cancellationToken, progress))
            {
                switch (item)
                {
                    case CsprojFileData csprojFileData:
                        csprojFiles.Add(csprojFileData);
                        break;
                    case SolutionFileData slnFileData:
                        slnFiles.Add(slnFileData);
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid item type: {item?.GetType().Name ?? "null"}");
                }
            }

            // build result
            var data = new DotNetProjectsData()
            {
                CsprojFiles = csprojFiles,
                SolutionFiles = slnFiles
            };

            return data;
        }

        private IEnumerable<object> DiscoverAndParseFiles(CancellationToken cancellationToken, IProgress<DiscoveryProgress> progress)
        {
            Stopwatch sw = Stopwatch.StartNew();

            foreach(var path in Directory.EnumerateFileSystemEntries(FolderPath, "*", SearchOption.AllDirectories))
            {
                if(sw.ElapsedMilliseconds > 100)
                {
                    progress.Report(new DiscoveryProgress() { CurrentItem = path });
                    sw.Restart();
                }

                cancellationToken.ThrowIfCancellationRequested();

                if(path.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase))
                {
                    CsprojFileData csprojData = null;

                    try
                    {
                        csprojData = csprojFileReader.ReadFromFile(path, progress);
                    }
                    catch (Exception e)
                    {
                        progress.Report(new DiscoveryProgress()
                        {
                            CurrentItem = path,
                            ErrorMessage = $"Can't load file \"{path}\". Error: {e.Message}"
                        });
                    }

                    yield return csprojData;

                } 
                else if(path.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
                {
                    SolutionFileData slnData = null;
                    try
                    {
                        slnData = slnFileReader.ReadFromFile(path, progress);
                    }
                    catch (Exception e)
                    {
                        progress.Report(new DiscoveryProgress()
                        {
                            CurrentItem = path,
                            ErrorMessage = $"Can't load file \"{path}\". Error: {e.Message}"
                        });
                    }

                    yield return slnData;
                }
            }
        }
    }
}
