using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Org.Openengsb.XLinkCSharpClient.XLink;
using Org.Openengsb.XLinkCSharpClient.SearchLogic;

namespace Org.Openengsb.XLinkCSharpClient
{
    /// <summary>
    /// Programm entry point. Controlls the user interaction.
    /// </summary>
    class Program
    {
        /*program exit codes - constant*/
        private static readonly int EXIT_SUCCESS = 0;
        private static readonly int EXIT_FAILURE = 1;

        /*Programname for usage()*/
        private static readonly string programname = "XLinkCSharpClient";

        /*Program language*/
        public static readonly string locale = "en";

        /*Logic to browse the WorkingDirectory*/
        public static DirectoryBrowser directoryBrowser;

        /*Context to be used at the OpenEngSB*/
        private static String openengsbContext = "foo";

        /*ConnectionManager to the OpenEngSB, also creates XLinks*/
        public static OpenEngSBConnectionManager openengsbConnectionManager;

        /*XLink Properties*/
        private static string domainId = "oosourcecode";
        private static string xlinkServerURL = "tcp://localhost.:6549";
        private static string hostIp = "127.0.0.1";
        public static string viewId = "C#SourceCode";
        private static string classNameOfOpenEngSBModel = "org.openengsb.domain.OOSourceCode.model.OOClass";


        [STAThread]
        static int Main(string[] args)
        {
            OpenEngSBConnectionManager.initInstance(xlinkServerURL, domainId, programname, hostIp, classNameOfOpenEngSBModel, openengsbContext);
            openengsbConnectionManager = OpenEngSBConnectionManager.getInstance();
            directoryBrowser = new DirectoryBrowser();

            if(args.Length < 1){
                exitProgramWithError("Missing Parameter\nUsage: " + programname + " {workingDirectory}");
            }
            if (!directoryBrowser.setWorkingDirectory(args[0]))
            {
                exitProgramWithError("Supplied Path \"" + args[0] + "\" is not a directory.");
            }
            openengsbContext = args[1];
            printWelcomeInformation();
           
            connectToOpenEngSBAndRegisterForXLink();

            outputLine("Insert your command:");

            String line;
            while ((line = Console.ReadLine()) != null)
            {
                if (line.StartsWith("changeWD"))
                {
                    string[] lineParams = line.Split(' ');
                    if (lineParams.Length < 2)
                    {
                        outputLine("changeWD <directoryPath> - directoryPath is missing");
                        continue;
                    }
                    string param = lineParams[1];
                    if (directoryBrowser.setWorkingDirectory(param))
                    {
                        outputLine("WorkingDirectory changed.");
                        directoryBrowser.reloadListOfWorkingDirectoryFiles();
                    }           
                }
                else if (line.Equals("displayWD"))
                {
                    directoryBrowser.displayWorkingDirectory();
                }
                else if (line.Equals("help"))
                {
                    processHelp();
                }
                else if (line.StartsWith("changeFT"))
                {
                    string[] lineParams = line.Split(' ');
                    if (lineParams.Length < 2)
                    {
                        outputLine("changeFT <filetype> - filetype is missing");
                        continue;
                    }
                    directoryBrowser.changeFileType(lineParams[1]);
                }
                else if (line.Equals("displayFT"))
                {
                    directoryBrowser.displaySupportedFileTypes();
                }
                else if (line.Equals("displaySupportedFT"))
                {
                    directoryBrowser.displaySupportedFileTypes();
                }
                else if (line.Equals("list"))
                {
                    directoryBrowser.displayListOfWorkingDirectoryFiles();
                }
                else if (line.Equals("reload"))
                {
                    directoryBrowser.reloadListOfWorkingDirectoryFiles();
                }
                else if (line.StartsWith("open"))
                {
                    string[] lineParams = line.Split(' ');
                    if (lineParams.Length < 2)
                    {
                        outputLine("open <filename> - filename is missing");
                        continue;
                    }
                    directoryBrowser.openFile(lineParams[1]);
                }
                else if (line.StartsWith("exit"))
                {
                    break;
                }
                else if (line.StartsWith("xlink"))
                {
                    if(!openengsbConnectionManager.isConnected())
                    {
                        outputLine("No connection to the OpenEngSB.");
                        continue;
                    }
                    string[] lineParams = line.Split(' ');
                    if (lineParams.Length < 2)
                    {
                        outputLine("xlink <filename> - filename is missing");
                        continue;
                    }
                    directoryBrowser.createXLinkFromFileString(lineParams[1]);
                }
                else if (line.Equals("listLocalSwitch"))
                {
                    if (!openengsbConnectionManager.isConnected())
                    {
                        outputLine("No connection to the OpenEngSB.");
                        continue;
                    }
                    openengsbConnectionManager.listOtherLocalInstalledSoftwareTools();
                }
                else if (line.StartsWith("localSwitch"))
                {
                    if (!openengsbConnectionManager.isConnected())
                    {
                        outputLine("No connection to the OpenEngSB.");
                        continue;
                    }
                    string[] lineParams = line.Split(' ');
                    if (lineParams.Length < 4)
                    {
                        outputLine("missin parameters. Insert 'help' for usage.");
                        continue;
                    }
                    openengsbConnectionManager.triggerLocalSwitch(lineParams[1], lineParams[2], lineParams[3]);
                }
                else
                {
                    outputLine("Unknown command \"" + line + "\".\nType \"help\" to list all commands.");
                }
                outputLine("\nInsert your command:");
            }
			exitProgramWithSucces();
			return 0;
        }

        /// <summary>
        /// Prints general program Information and loads the Files from the Working Directory
        /// </summary>
        private static void printWelcomeInformation()
        {
            outputLine("C# Model Browser is running.");
            directoryBrowser.displayWorkingDirectory();
            directoryBrowser.displaySupportedFileTypes();
            directoryBrowser.displaycurrentFileType();
            directoryBrowser.reloadListOfWorkingDirectoryFiles();
        }

        /// <summary>
        /// Tries to connect to the OpenEngSB. Catches possible Exceptions.
        /// </summary>
        private static void connectToOpenEngSBAndRegisterForXLink()
        {
            try
            {
                openengsbConnectionManager.connectToOpenEngSbWithXLink();
            }
            catch
            {
                outputLine("Connect failed.");
            }
        }

        /// <summary>
        /// Tries to disconnect from the OpenEngSB. Catches possible Exceptions.
        /// </summary>
        private static void disconnectFromXLinkAndOpenEngSB()
        {
            if (!openengsbConnectionManager.isConnected())
            {
                return;
            }
            try
            {
                openengsbConnectionManager.disconnect();
            }
            catch
            {
                outputLine("Disconnect failed.");
            }
        }

        /// <summary>
        /// Prints the given ErrorMsg. Disconnects from the OpenEngSB and exits.
        /// </summary>
        private static void exitProgramWithSucces(){
            outputLine("Program is closing.");
            disconnectFromXLinkAndOpenEngSB();
            outputLine("Press any key to close...");
            Console.ReadKey();
            System.Environment.Exit(EXIT_SUCCESS);      	
        }

        /// <summary>
        /// Prints the given ErrorMsg. Disconnects from the OpenEngSB and exits with an error code.
        /// </summary>
        private static void exitProgramWithError(String errorMsg){
            outputLine(errorMsg);
            disconnectFromXLinkAndOpenEngSB();
            outputLine("Press any key to close...");
            Console.ReadKey();
            System.Environment.Exit(EXIT_FAILURE);    	
        }        

        /// <summary>
        /// Prints the list of available commands.
        /// </summary>
        private static void processHelp()
        {
            outputLine("\nAvailable Commands:");

            /*Commented information about commands since they are not needed in controlled experiment*/

            //outputLine("\nchangeFT <filetype> - changes the current Filetype, must be a supported filetype");
            //outputLine("\nchangeWD <directoryPath> - changes the current WorkingDirectory, must be a correct directoryPath");
            //outputLine("\ndisplayFT - displays the current filetype");
            //outputLine("\ndisplaySupportedFT - displays all supported filetypes");
            //outputLine("\ndisplayWD - displays the current WorkingDirectory");
            outputLine("\nexit - closes the program");
            outputLine("\nhelp - displays this help");
            outputLine("\nlist - lists all files of the defined filetype in the workingDirectory");
            outputLine("\nopen <filename> - opens the given file in the standard editor");
            //outputLine("\nreload - reloads filelist from the workingDirectory");
            outputLine("\nxlink <filename> - copies the xlink of the specified file to the clipboard");
            outputLine("\nlistLocalSwitch - lists the name and views of all other localy installed software tools with xLink.");
            outputLine("\nlocalSwitch <programname> <viewId> <filename> - triggers an immediate switch to the given software tools with the xlink of the given file.");
            outputLine("");
        }

        private static void outputLine(string line)
        {
            Console.WriteLine(line);
        }

    }
}
