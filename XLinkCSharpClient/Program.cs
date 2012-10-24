using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using XLinkCSharpClient.Model;

namespace XLinkCSharpClient
{
    class Program
    {
        /**program exit codes - constant*/
        private static readonly int EXIT_SUCCESS = 0;
        /**program exit codes - constant*/
        private static readonly int EXIT_FAILURE = 1;
        /**Programname for usage()*/
        private static readonly string programname = "XLinkCSharpClient";

        private static readonly string[] supportedFileTypes = { "java", "cs" };
        private static string workingDirectory;
        private static string filetype = "java";
        private static List<WorkingDirectoryFile> wdFiles;
        private static String openengsbContext;

        [STAThread]
        static int Main(string[] args)
        {
            if(args.Length < 2){
                outputLine("Missing Parameter");
                outputLine("Usage: " + programname + " {workingDirectory} {openengsb.context}");
                return EXIT_FAILURE;
            }
            if (!setWorkingDirectory(args[0]))
            {
                outputLine("Supplied Path \"" + args[0] + "\" is not a directory.");
                return EXIT_FAILURE;
            }
            setOpenEngSBContext(args[1]);

            outputLine("Modelclass Browser is running.");
            processDisplayWD();
            processDisplayFT();
            displaySupportedFT();
            processReload();
            Console.Write("Insert your command:");

            String line;
            while ((line = Console.ReadLine()) != null)
            {
                if (line.StartsWith("changeWD"))
                {
                    processChangeWD(line);
                }
                else if (line.Equals("displayWD"))
                {
                    processDisplayWD();
                }
                else if (line.Equals("help"))
                {
                    processHelp();
                }
                else if (line.StartsWith("changeFT"))
                {
                    processChangeFT(line);
                }
                else if (line.Equals("displayFT"))
                {
                    processDisplayFT();
                }
                else if (line.Equals("displaySupportedFT"))
                {
                    processDisplayFT();
                }
                else if (line.Equals("list"))
                {
                    processList();
                }
                else if (line.Equals("reload"))
                {
                    processReload();
                }
                else if (line.StartsWith("open"))
                {
                    processOpen(line);
                }
                else if (line.StartsWith("exit"))
                {
                    break;
                }
                else if (line.StartsWith("xlink"))
                {
                    processXLink(line);
                }
                else
                {
                    outputLine("Unknown command \""+line+"\".\nType \"help\" to list all commands.");
                }
                Console.Write("Insert your command:");
            }

            outputLine("Program is closing.");
            return EXIT_SUCCESS;
        }

        private static void processHelp()
        {
            outputLine("Available Commands:");
            outputLine("\tchangeFT <filetype> - changes the current Filetype, must be a supported filetype");
            outputLine("\n\tchangeWD <directoryPath> - changes the current WorkingDirectory, must be a correct directoryPath");
            outputLine("\n\tdisplayFT - displays the current filetype");
            outputLine("\n\tdisplaySupportedFT - displays all supported filetypes");
            outputLine("\n\tdisplayWD - displays the current WorkingDirectory");
            outputLine("\n\texit - closes the program");
            outputLine("\n\thelp - displays this help");
            outputLine("\n\tlist - lists all files of the defined filetype in the workingDirectory");
            outputLine("\n\topen <filename> - opens the given file in the standart editor");
            outputLine("\n\treload - reloads filelist from the workingDirectory");
            outputLine("\n\txlink <filename> - copies the xlink of the specified file to the clipboard"); 
        }

        private static void processChangeWD(String line)
        {
            string[] lineParams = line.Split(' ');
            if (lineParams.Length < 2)
            {
                outputLine("changeWD <directoryPath> - directoryPath is missing");
                return;
            }
            string param = lineParams[1];
            if (!setWorkingDirectory(param))
            {
                outputLine("Supplied Path \"" + param + "\" is not a directory.");
            }
            outputLine("WorkingDirectory changed.");
            processReload();
        }

        private static void processChangeFT(String line)
        {
            string[] lineParams = line.Split(' ');
            if (lineParams.Length < 2)
            {
                outputLine("changeFT <filetype> - filetype is missing");
                return;
            }
            string param = lineParams[1];
            if (!setFileType(param))
            {
                outputLine("Supplied Filetype \"" + param + "\" is not supported.");
            }
            outputLine("FileType changed.");
            processReload();
        }

        private static void processOpen(String line)
        {
            string[] lineParams = line.Split(' ');
            if (lineParams.Length < 2)
            {
                outputLine("open <filename> - filename is missing");
                return;
            }
            string param = lineParams[1];
            WorkingDirectoryFile searchedFile = null;
            foreach (WorkingDirectoryFile wdf in wdFiles)
            {
                if (wdf.fileName.Equals(param))
                {
                    searchedFile = wdf;
                    break;
                }
            }

            if (searchedFile != null)
            {
                outputLine("opening file...");
                System.Diagnostics.Process.Start(searchedFile.wholePath);
            }
            else
            {
                outputLine("file was not found");
            }
            
        }

        private static void processXLink(String line)
        {
            string[] lineParams = line.Split(' ');
            if (lineParams.Length < 2)
            {
                outputLine("xlink <filename> - filename is missing");
                return;
            }
            string param = lineParams[1];
            WorkingDirectoryFile searchedFile = null;
            foreach (WorkingDirectoryFile wdf in wdFiles)
            {
                if (wdf.fileName.Equals(param))
                {
                    searchedFile = wdf;
                    break;
                }
            }

            if (searchedFile != null)
            {
                createXLink(searchedFile);
                outputLine("xlink was copied to clipboard...");
            }
            else
            {
                outputLine("file was not found");
            }

        }

        private static void processList()
        {
            if (wdFiles.Count != 0)
            {
                outputLine("Files of the working directory");
            }
            else
            {
                outputLine("There are no Files of the filetype in the working directory.");
            }
            foreach(WorkingDirectoryFile wdf in wdFiles){
                outputLine(wdf.fileName + " (" + wdf.directoryOfFile + ")");
            }
        }

        private static void processReload()
        {
            wdFiles = new List<WorkingDirectoryFile>();
            string[] filePaths = Directory.GetFiles(workingDirectory, "*."+filetype, SearchOption.AllDirectories);
            foreach (String file in filePaths)
            {
                wdFiles.Add(new WorkingDirectoryFile(file));
            }
            outputLine("FileList reloaded");
        }

        private static void processDisplayWD()
        {
            outputLine("The current WorkingDirectory is \"" + workingDirectory + "\"");
        }

        private static void processDisplayFT()
        {
            outputLine("The current filetype is \"" + filetype+"\"");
        }

        private static void displaySupportedFT()
        {
            string ft = supportedFileTypes[0];
            for (int i = 1; i < supportedFileTypes.Length;i++)
            {
                ft += ", " +supportedFileTypes[i];
            }
            outputLine("Supported filetypes are {" + ft + "}");
        }

        private static bool setWorkingDirectory(String newWorkingDirectory)
        {
            if (Directory.Exists(newWorkingDirectory))
            {
                workingDirectory = newWorkingDirectory;
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void setOpenEngSBContext(String context)
        {
            openengsbContext = context;
        }

        private static bool setFileType(String newFileType)
        {
            if (supportedFileTypes.Contains(newFileType))
            {
                filetype = newFileType;
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void outputLine(string line)
        {
            Console.WriteLine(line);
        }

        /*transfere to real connector*/

        private static void createXLink(WorkingDirectoryFile file)
        {
            String completeUrl = "dummyUrl"; //connector.getTemplate().getBaseUrl();
            completeUrl += "&" + ""; //connector.getTemplate().getModelClassKey() + "=" + urlEncodeParameter(modelInformation.getModelClassName());
            completeUrl += "&" + ""; //connector.getTemplate().getModelVersionKey() + "=" + urlEncodeParameter(modelInformation.getModelVersionString());
            completeUrl += "&" + ""; //connector.getTemplate().getContextIdKeyName() + "=" + urlEncodeParameter(openEngSBContext);   
            String classNameKey = "className";
            completeUrl += "&" + classNameKey + "=" + ""; //selectedStmt.getTableName();
            Clipboard.SetText(completeUrl);
        }

        private static void openXlink(String className)
        {
            processReload();
            WorkingDirectoryFile searchedFile = null;
            foreach (WorkingDirectoryFile wdf in wdFiles)
            {
                if (wdf.fileName.Equals(className))
                {
                    searchedFile = wdf;
                    break;
                }
            }

            if (searchedFile != null)
            {
                outputLine("opening file due to xlink call...");
                System.Diagnostics.Process.Start(searchedFile.wholePath);
            }
            else
            {
                outputLine("openXLink was called with "+className+" but no match was found.");
            }
        }
    }
}
