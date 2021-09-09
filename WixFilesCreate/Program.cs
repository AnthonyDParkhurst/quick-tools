using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace WixFilesCreate
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootDir = new DirectoryInfo(args[0]);

            var rootNode = GetNodes(rootDir);

            var r = new XElement("root");
            var x = new XDocument(r);

            var e = new EmitInfo(r);

            e.AddNodes(rootNode, string.Empty);

            // write it
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(sb, xws))
            {
                x.Save(xw);
            }

            File.WriteAllText("files.wxs", sb.ToString());
        }

        private static Node GetNodes(DirectoryInfo dir)
        {
            var node = new Node(dir);

            node.Files.AddRange(dir.GetFiles().Select(fi => fi.Name));

            foreach (var subdir in dir.GetDirectories())
            {
                node.SubFolders.Add(GetNodes(subdir));
            }

            return node;
        }
    }
}
