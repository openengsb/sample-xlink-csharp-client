Project information
==========================
This project contains a sample client in C# which uses the XLink framework, built with the IDE SharpDevelop.
It manages the Java and C# class files in it's given working directory, they can be listed and openend. 
For a single file, a XLink can be generated.

Current status of the project.
==========================
The C# bridge and the used DomainClasses have not yet been included. Currently it is not possible to use this client with the OpenEngSB (and XLink) framework.
(TODO explain how to intall the C# bridge)


How to configure & build
==========================
- Install & Configure Java JDK 1.6 or higher

- Install & Configure Maven 3.0 or higher

- Clone and Build (mvn clean install) the [OpenEngSB](https://github.com/openengsb/openengsb-framework) with XLink and the Domains 'SQLCreate' and 'OOSoureCode' 

- Clone this project

- Start the OpenEngSB server and make sure that the JMS-Port bundle is installed<br/>
Note that, if server and client are not running on the same machine, the xlink base-URL (e.g. contains also the server URL) must
be configured in $OPENENGSB_HOME$/etc/org.openengsb.core.services.internal.connectormanager.cfg

- Run the project with a C# IDE
(TODO change the project to a maven project)


How to start
==========================
The programm requires two parameters to start
- a working directory to be searched by the program
- the OpenEngSB context to be used for creating XLinks
