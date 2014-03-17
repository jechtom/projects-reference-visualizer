using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class SolutionFileLineParser
    {
        Regex projectLineBeginRegex = new Regex(@"^Project\(""\{[a-z0-9-]+\}""\) = ", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Common", "Common\Common.csproj", "{2D31F23C-121D-4309-95D8-F0D50924B24B}"
        public SolutionProjectData TryParse(string line)
        {
            // >>>Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = <<<
            var beginLine = projectLineBeginRegex.Match(line);
            if (!beginLine.Success)
                return null;

            // find values ("Common", "Common\Common.csproj", "{2D31F23C-121D-4309-95D8-F0D50924B24B}")
            // values: project_name; project_path; project_ID
            string[] values = ParseValues(line, beginLine.Index + beginLine.Length);

            if (values.Length != 3)
                throw new InvalidOperationException("Unexpected line format: " + line);

            return new SolutionProjectData()
            {
                Name = values[0],
                RelativePath = values[1]
            };
        }

        private string[] ParseValues(string line, int startIndex)
        {
            var values = new List<string>(3);
            int i = startIndex - 1;
            bool valueStarted = false;
            var currentValue = new StringBuilder();

            while ((++i) < line.Length)
            {
                bool isLast = i == (line.Length - 1);
                bool isDoubleQuote = line[i] == '"';
                bool isTwoDoubleQuotes = isDoubleQuote && !isLast && line[i + 1] == '"';

                // start or end value
                if(isDoubleQuote && !isTwoDoubleQuotes)
                {
                    if(valueStarted)
                    {
                        values.Add(currentValue.ToString());
                        currentValue.Clear();
                        valueStarted = false;
                    }
                    else
                    {
                        valueStarted = true;
                    }

                    continue;
                }

                // append char to current value
                if (valueStarted)
                {
                    if (isTwoDoubleQuotes)
                        i++;
                    
                    currentValue.Append(line[i]);
                }
            }

            if (valueStarted)
                throw new InvalidOperationException("Unexpected line ending: " + line);

            return values.ToArray();
        }
    }
}
