How to configure & build
==========================
- Install & Configure Java JDK 1.6 or higher

- Install & Configure Maven 3.0 or higher

- Clone and Build (mvn clean install) the [OpenEngSB](https://github.com/openengsb/openengsb-framework) with XLink and the Domains 'SQLCreate' and 'OOSoureCode' 

- Start the OpenEngSB server and make sure that the JMS-Port bundle is installed<br/>
Note that, if server and client are not running on the same machine, the xlink base-URL (e.g. contains also the server URL) must
be configured in $OPENENGSB_HOME$/etc/org.openengsb.core.services.internal.connectormanager.cfg

- Clone this project

- Open XLinkCSharp.sln with a C# IDE that has *nuget support* and load the referenced nuget packages

Tutorial for building with MS Visual Studio 2012
==========================

- When opening XLinkCSharp.sln, an error states that the project was not correctly loaded

- Ignore this error

- Right-Click on the solution and choose 'Enable Nuget Package Restore' and confirm

- Reload the project

- Right-Click on the project and choose 'Manage Nuget Packages' and select 'Restore'

- After the missing packages have been loaded, the project is ready to build

How to start
==========================
The programm requires two parameters to start

- A path to a directory, this will be the working directory to search for files

- The OpenEngSB context to be used for creating XLinks

- When running the program with MS Visual Studio 2012 in debug mode, an user-execution is marked. This exception is handled by the code, you may ignore it.

Structure of accepted C# files
==========================
The program searches the working directory for C# files and parses their content. 
Examples of these C# files can be found in the directory 'XLinkCSharpClient/Resources'.
These files are also the valid test data for the demonstration.

Implemented Functionality
==========================
- Client automatically connects to the OpenEngSB and registers for XLink
- .cs Files in the WorkingDirectory can be viewed in a List and opened
- Via the 'xlink <filename>' command, a valid XLink-URL is copied into the clipboard
- Incoming potential Matches are searched in the WorkingDirectory and, if found, the most likely match is displayed.
- Incoming updates about other local tools that support XLink are processed. 

Not implemented Functionality
==========================
- Creation of new C# classes
