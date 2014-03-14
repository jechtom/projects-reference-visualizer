using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReferenceVisualizer.Core.DotNetProjects
{
    public class CsprojFileDataReader
    {
        public CsprojFileData ReadFromFile(string filename)
        {
            var result = new CsprojFileData()
            {
                FileName = filename,
                References = new List<string>()
            };

            // read file and its references
            var document = new XmlDocument();
            document.Load(filename);

            var nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("c", "http://schemas.microsoft.com/developer/msbuild/2003");

            foreach (var node in document.SelectNodes("/c:Project/c:ItemGroup/c:ProjectReference/@Include", nsmgr).Cast<XmlNode>())
            {
                result.References.Add(node.Value);
            }

            return result;
        }
    }
}
