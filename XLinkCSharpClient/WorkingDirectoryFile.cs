using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XLinkCSharpClient.Model
{
    class WorkingDirectoryFile
    {
        public string fileName;
        public string directoryOfFile;
        public string wholePath;
        public string content;

        public WorkingDirectoryFile(String input_wholePath)
        {
            FileInfo fi = new FileInfo(input_wholePath);
            fileName = fi.Name;
            directoryOfFile = fi.DirectoryName;
            wholePath = input_wholePath;
        }
    }
}
