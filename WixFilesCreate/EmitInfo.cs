using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

namespace WixFilesCreate
{
    public class EmitInfo
    {
        private readonly XElement _root;

        private int _counter = 0;

        public EmitInfo(XElement root)
        {
            _root = root;
        }

        public void AddNodes(Node node, string parentFolder)
        {
            var cleanName = FixName(node.DirectoryInfo.Name);

            var cg = new XElement("ComponentGroup", new XAttribute("Id", cleanName + "Group"), new XAttribute("Directory", cleanName + "Dir"));
            var f = new XElement("Fragment", cg);
            _root.Add(f);

            foreach (var file in node.Files)
            {
                var id = file.Replace("-", "_").Replace(" ", "_") + "." + _counter++;

                if (!(char.IsLetter(id[0]) || id[0] == '_'))
                {
                    id = "_" + id;
                }

                var xFile = new XElement("File", new XAttribute("Id", id), new XAttribute("Name", file), new XAttribute("Source", @"$(var.SOURCEDIR)" + parentFolder + @"\" + file));

                var component = new XElement("Component", new XAttribute("Id", id), new XAttribute("Guid", Guid.NewGuid().ToString("D")), xFile);

                cg.Add(component);
            }

            foreach (var n in node.SubFolders)
            {
                AddNodes(n, parentFolder + @"\" + n.DirectoryInfo.Name);
            }
        }

        private string FixName(string name)
        {
            var words = name.Split(" ");

            for (var i = 0; i < words.Length; i++)
            {
                if (char.IsLower(words[i][0]))
                {
                    var sb = new StringBuilder();

                    sb.Append(char.ToUpperInvariant(words[i][0]));

                    sb.Append(words[i].Substring(1));

                    words[i] = sb.ToString();
                }
            }

            return string.Join("", words);
        }
    }
}
