using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class SolutionFileDataReader
    {
        public SolutionFileData ReadFromFile(string filename, IProgress<DiscoveryProgress> progress)
        {
            var result = new SolutionFileData()
            {
                FileName = filename,
                Projects = new List<SolutionProjectData>()
            };

            string filenameDirectory = Path.GetDirectoryName(filename);

            // read file and its project references
            var parser = new SolutionFileLineParser();
            using (var textReader = new StreamReader(filename))
            {
                while (!textReader.EndOfStream)
                {
                    var line = textReader.ReadLine();
                    var projectReference = parser.TryParse(line);
                    if (projectReference == null)
                        continue;

                    // ignore non-csproj files
                    if (!projectReference.RelativePath.EndsWith(".csproj"))
                        continue;

                    // resolve full path and add to collection
                    try
                    {
                        projectReference.FullPath = Path.GetFullPath(Path.Combine(filenameDirectory, projectReference.RelativePath));
                        result.Projects.Add(projectReference);
                    }
                    catch(NotSupportedException)
                    {
                        // invoked on invalid path
                        progress.Report(new DiscoveryProgress()
                        {
                            CurrentItem = filename,
                            ErrorMessage = $"Not supported path in solution file: {projectReference.RelativePath}"
                        });
                    }
                }
            }

            return result;
        }
    }
}
