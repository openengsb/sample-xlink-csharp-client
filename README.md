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
- a working directory to be searched by the program
- the OpenEngSB context to be used for creating XLinks
