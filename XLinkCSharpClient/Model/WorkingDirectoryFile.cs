using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;


namespace Org.Openengsb.XLinkCSharpClient.Model
{
    /// <summary>
    /// Internal Modelclass to hold Content and Metadata of loaded Class Files
    /// </summary>
    class WorkingDirectoryFile
    {
        public string fileName;
        public string directoryOfFile;
        public string wholePath;
        public string content;
        
        public WorkingDirectoryFile(String input_wholePath)
        {
            FileInfo fi = new FileInfo(input_wholePath);
            this.fileName = fi.Name;
            this.directoryOfFile = fi.DirectoryName;
            this.wholePath = input_wholePath;
            this.content = "";

            StreamReader reader = new StreamReader(wholePath);
            string line = string.Empty;
            bool classOpen = false;
            int bracketCount = 0;

            Regex _regex = new Regex(@".*namespace [a-zA-Z0-9_\.]+.*");

            while ((line = reader.ReadLine()) != null)
            {
                if (!classOpen)
                {
                    Match match = _regex.Match(line);
                    if (match.Success)
                    {
                        classOpen = true;
                        content += line + "\n";
                        content += reader.ReadLine() + "\n"; // eat first open bracket
                        bracketCount = 1;
                    }
                }
                else 
                {
                    content += line + "\n";
                    if (line.Contains("{"))
                    {
                        bracketCount++;
                    }
                    else if (line.Contains("}"))
                    {
                        bracketCount--;
                    }
                    if (bracketCount == 0)
                    {
                        break;
                    }
                }
            }
            reader.Close();
        }
    }
}
