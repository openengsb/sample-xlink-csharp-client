using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Org.Openengsb.XLinkCSharpClient.Model;

namespace Org.Openengsb.XLinkCSharpClient.SearchLogic
{
    /// <summary>
    /// TODO TBW
    /// </summary>
    class DirectoryBrowser
    {
        private static readonly string[] supportedFileTypes = { "cs" };
        private static string currentFileType = supportedFileTypes[0];
        private static string workingDirectory;
        private static List<WorkingDirectoryFile> wdFiles;

        /// <summary>
        /// Displays the set of supported FileTypes
        /// </summary>
        public void displaySupportedFileTypes()
        {
            string ft = supportedFileTypes[0];
            for (int i = 1; i < supportedFileTypes.Length; i++)
            {
                ft += ", " + supportedFileTypes[i];
            }
            outputLine("Supported filetypes are {" + ft + "}");
        }

        /// <summary>
        /// Sets the new WorkingDirectory to browse with the given Directory
        /// </summary>
        /// <param name="newWorkingDirectory"></param>
        /// <returns>False if the supplied directory does not exist</returns>
        public bool setWorkingDirectory(String newWorkingDirectory)
        {
            if (Directory.Exists(newWorkingDirectory))
            {
                workingDirectory = newWorkingDirectory;
                return true;
            }
            else
            {
                outputLine("Supplied Path \"" + newWorkingDirectory + "\" is not a directory.");
                return false;
            }
        }

        /// <summary>
        /// Prints the list of currently loaded Directory Files
        /// </summary>
        public void displayListOfWorkingDirectoryFiles()
        {
            if (wdFiles.Count != 0)
            {
                outputLine("Files of the working directory");
            }
            else
            {
                outputLine("There are no Files of the filetype in the working directory.");
            }
            foreach (WorkingDirectoryFile wdf in wdFiles)
            {
                outputLine(wdf.fileName + " (" + wdf.directoryOfFile + ")");
            }
        }

        /// <summary>
        /// Reloads List of Directory Files
        /// </summary>
        public void reloadListOfWorkingDirectoryFiles()
        {
            wdFiles = new List<WorkingDirectoryFile>();
            string[] filePaths = Directory.GetFiles(workingDirectory, "*." + currentFileType, SearchOption.AllDirectories);
            foreach (String file in filePaths)
            {
                wdFiles.Add(new WorkingDirectoryFile(file));
            }
            outputLine("Loaded " + filePaths.Length+ " Files from WorkingDirectory");
        }

        /// <summary>
        /// Displays the current Working Directory
        /// </summary>
        public void displayWorkingDirectory()
        {
            outputLine("The current WorkingDirectory is \"" + workingDirectory + "\"");
        }

        /// <summary>
        /// Displays the current File Type
        /// </summary>
        public void displaycurrentFileType()
        {
            outputLine("The current filetype is \"" + currentFileType + "\"");
        }

        /// <summary>
        /// Reloads the Files from the WorkingDirectory and Returns them.
        /// </summary>
        /// <returns></returns>
        public List<WorkingDirectoryFile> getFilesFromWorkingDirectory()
        {
            reloadListOfWorkingDirectoryFiles();
            return wdFiles;
        }

        /// <summary>
        /// Opens the File with the given name
        /// </summary>
        public void openFile(string fileName)
        {
            WorkingDirectoryFile searchedFile = searchForFile(fileName);
            if (searchedFile != null)
            {
                outputLine("Opening file '" + fileName + "'...");
                System.Diagnostics.Process.Start(searchedFile.wholePath);
            }
            else
            {
                outputLine("File '" + fileName + "' was not found in WorkingDirectory.");
            }
        }

        /// <summary>
        /// Creates the XLink to the given File with the given name and copies it to the Clipborad.
        /// </summary>
        public void createXLinkFromFileString(string fileName)
        {
            WorkingDirectoryFile searchedFile = searchForFile(fileName);
            if (searchedFile != null)
            {
                Program.openengsbConnectionManager.createXLink(searchedFile);
                outputLine("Xlink was copied to clipboard...");
            }
            else
            {
                outputLine("File was not found.");
            }
        }

        /// <summary>
        /// Returns the FileEntry with the given Name or null
        /// </summary>
        public WorkingDirectoryFile searchForFile(String fileName)
        {
            WorkingDirectoryFile searchedFile = null;
            foreach (WorkingDirectoryFile wdf in wdFiles)
            {
                if (wdf.fileName.Equals(fileName))
                {
                    searchedFile = wdf;
                    break;
                }
            }
            return searchedFile;
        }

        /// <summary>
        /// Switches the fileType to the supplied one.
        /// Reloads the Files of the Working Directory if the switch was successfull.
        /// </summary>
        public void changeFileType(String newFileType) 
        {
            if (!setFileType(newFileType))
            {
                outputLine("Supplied Filetype \"" + newFileType + "\" is not supported.");
                return;
            }
            outputLine("FileType changed.");
            reloadListOfWorkingDirectoryFiles();
        }

        /// <summary>
        /// Switches the fileType to the supplied one.
        /// Returns false if the supplied fileType is not supported.
        /// </summary>
        private bool setFileType(String newFileType)
        {
            if (supportedFileTypes.Contains(newFileType))
            {
                currentFileType = newFileType;
                return true;
            }
            else
            {
                return false;
            }
        }


        private void outputLine(string line)
        {
            Console.WriteLine(line);
        }

    }
}
