using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixFilesCreate
{
    public class Node
    {
        public DirectoryInfo DirectoryInfo;
        public List<Node> SubFolders;
        public List<string> Files;

        public Node(DirectoryInfo dir)
        {
            DirectoryInfo = dir;
            SubFolders = new List<Node>();
            Files = new List<string>();
        }
    }
}
