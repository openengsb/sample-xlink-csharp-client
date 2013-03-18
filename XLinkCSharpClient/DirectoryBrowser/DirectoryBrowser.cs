using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Org.Openengsb.XLinkCSharpClient.Model;

using OOSourceCodeDomain;
using Org.Openengsb.XLinkCSharpClient.XLink;

namespace Org.Openengsb.XLinkCSharpClient.SearchLogic
{
    /// <summary>
    /// Specifies the file-browse functionality, in the defined working directory.
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
                outputLine("Files of the working directory:\n");
            }
            else
            {
                outputLine("There are no Files of the filetype in the working directory.");
            }
            foreach (WorkingDirectoryFile wdf in wdFiles)
            {
                outputLine("- " + wdf.fileName + " (" + wdf.directoryOfFile + ")");
                /*OOClass test = LinkingUtils.convertWorkingDirectoryFileToOpenEngSBModel(wdf);
                outputLine("package "+test.packageName);
                outputLine("class " + test.className);
                for (int i = 0; i < test.attributes.Length;i++) {
                    outputLine("var " + i + " type " + test.attributes[i].type);
                    outputLine("var "+ i +" name "+test.attributes[i].name);                   
                }*/
            }
            outputLine("");
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
                String xlink = Program.openengsbConnectionManager.createXLink(searchedFile);
                if (xlink != null)
                {
                    Clipboard.SetText(xlink);
                    outputLine("Xlink was copied to clipboard...");
                }               
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
            if (searchedFile == null)
            {
                foreach (WorkingDirectoryFile wdf in wdFiles)
                {
                    if (wdf.fileName.Contains(fileName))
                    {
                        searchedFile = wdf;
                        break;
                    }
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

        /// <summary>
        /// Searches through the Files for the most potential Match.
        /// First searches for matching filesnames.
        /// If no files where found or if more than one file was found, search for matching variables.
        /// </summary>
        public void searchForXLinkMatches(OOClass potentialMatch)
        {
            if (wdFiles.Count == 0)
            {
                outputLine("An XLink match was triggered, but the List of loaded files is empty.");
                return;
            }

            //first search for matching filesnames
            List<WorkingDirectoryFile> correspondingFiles = new List<WorkingDirectoryFile>();
            foreach (WorkingDirectoryFile wdf in wdFiles)
            {
                OOClass classModelRepresentation = LinkingUtils.convertWorkingDirectoryFileToOpenEngSBModel(wdf);
                if (classModelRepresentation.className.ToLower().Contains(potentialMatch.className.ToLower()))
                {
                    correspondingFiles.Add(wdf);
                }
            }

            WorkingDirectoryFile foundMatch = null;

            //if no files where found or if more than one file was found, compare the matching variables
            if (correspondingFiles.Count != 1)
            {
                if (correspondingFiles.Count == 0)
                {
                    correspondingFiles = wdFiles;
                }

                WorkingDirectoryFile mostPotentialLocalMatch = correspondingFiles[0];
                int maxMatchValue = matchValueOfClass(mostPotentialLocalMatch, potentialMatch);
                foreach (WorkingDirectoryFile localWDF in correspondingFiles)
                {
                    int currentMatchValue = matchValueOfClass(localWDF, potentialMatch);
                    if (currentMatchValue > maxMatchValue)
                    {
                        maxMatchValue = currentMatchValue;
                        mostPotentialLocalMatch = localWDF;
                    }
                }       
                if (matchValueOfClass(mostPotentialLocalMatch, potentialMatch) != 0)
                {
                    foundMatch = mostPotentialLocalMatch;
                }
            }
            else {
                foundMatch = correspondingFiles[0];
            }

            if (foundMatch != null)
            {
                outputLine("An XLink match was triggered and a local match was found.");
                System.Diagnostics.Process.Start(foundMatch.wholePath);
            }
            else
            {
                outputLine("An XLink match was triggered, but no local match was found.");
            }
        }

        /// <summary>
        /// Returns an int value which indicates, how many many SQLFiels are alike between the two given statements.
        /// </summary>
        private int matchValueOfClass(WorkingDirectoryFile wdFileToCheck, OOClass potentialMatch) 
        {
            int matchValue = 0;
            for (int i = 0; i < potentialMatch.attributes.Length; i++)
            {
                String attributeRepresentation = potentialMatch.attributes[i].type + " " + potentialMatch.attributes[i].name;
                if (wdFileToCheck.content.ToLower().Contains(attributeRepresentation.ToLower()))
                {
                    matchValue++;
                }
            }
            return matchValue;
        }


        private void outputLine(string line)
        {
            Console.WriteLine(line);
        }

    }
}
