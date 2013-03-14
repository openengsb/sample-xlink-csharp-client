using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

using Newtonsoft.Json;

using OOSourceCodeDomain;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;

using Org.Openengsb.XLinkCSharpClient.Model;

namespace Org.Openengsb.XLinkCSharpClient.XLink
{
    /// <summary>
    /// Manages the connect/disconnect to the OpenEngSB and the registration/deregistration to XLink.
    /// </summary>
    class OpenEngSBConnectionManager
    {
        /*Supplied program arguments*/
        private String xlinkServerURL;
        private String domainId;
        private String programname;
        private String openengsbContext;

        /// <summary>
        /// Only possible OpenEngSBConnectionManager instance
        /// </summary>
        private static OpenEngSBConnectionManager instance = null;

        /// <summary>
        /// Flag indicating the connection status
        /// </summary>
        private bool connected = false;

        /// <summary>
        /// HostIP of the local system, used to identify the Host during an XLink call
        /// </summary>
        private String hostIp;

        /// <summary>
        /// Id of the registered OpenEngSB-Connector
        /// </summary>
        private String connectorUUID;

        /// <summary>
        /// XLinkUrlBlueprint received during XLinkRegistration
        /// </summary>
        private XLinkUrlBlueprint blueprint;

        /// <summary>
        /// Qualified Class Name to identify the used DomainModel at the OpenEngSB
        /// </summary>
        private string classNameOfOpenEngSBModel;

        /// <summary>
        /// List of other locally installed tools
        /// </summary>
        private OOSourceCodeDomain.XLinkConnector[] currentlyInstalledTools;

        /*XLink variables*/
        private static IOOSourceCodeDomainSoap11Binding ooSourceConnector;
        private static IDomainFactory factory;

        private OpenEngSBConnectionManager(String xlinkServerURL, String domainId, String programname, String hostIp, string classNameOfOpenEngSBModel, String openengsbContext)
        {
            this.xlinkServerURL = xlinkServerURL;
            this.domainId = domainId;
            this.programname = programname;
            this.connected = false;
            this.hostIp = hostIp;
            this.classNameOfOpenEngSBModel = classNameOfOpenEngSBModel;
            this.openengsbContext = openengsbContext;
        }	

        /// <summary>
        /// Initializes the Connectors only instance.
        /// </summary>
        /// <param name="xlinkBaseUrl">Link to OpenEngSB server</param>
        /// <param name="hostIp">IP of the local host</param>
        public static void initInstance(String xlinkBaseUrl,
                String domainId, String programname,
                String hostIp, string classNameOfOpenEngSBModel,
                String openengsbContext)
        {
            instance = new OpenEngSBConnectionManager(xlinkBaseUrl, domainId,
                    programname, hostIp, classNameOfOpenEngSBModel, openengsbContext);
        }

        /// <summary>
        /// Returns the Connectors only instance.
        /// </summary>
        /// <returns>Connectors only instance</returns>
        public static OpenEngSBConnectionManager getInstance()
        {
            if (instance == null)
            {
                Console.WriteLine("getInstance():OpenEngSBConnectionManager was not initialized.");
            }
            return instance;
        }

        /// <summary>
        /// Creates/Registers the connector at the OpenEngSB and registers the connector to XLink
        /// </summary>
        public void connectToOpenEngSbWithXLink() 
        {
            outputLine("Trying to connect to OpenEngSB and XLink...");
            ooSourceConnector = new OOSourceCodeConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", xlinkServerURL, ooSourceConnector, new ForwardDefaultExceptionHandler());
            connectorUUID = factory.CreateDomainService(domainId);
            factory.RegisterConnector(connectorUUID, domainId);
            blueprint = factory.ConnectToXLink(connectorUUID, hostIp, programname, initModelViewRelation());
            setCurrentlyInstalledTools(blueprint.registeredTools);
            connected = true;
            outputLine("Connecting done.");
        }

        /// <summary>
        /// Unregisters the connector from XLink and removes it from the OpenEngSB
        /// </summary>
        public void disconnect()
        {
            outputLine("Disconnecting from OpenEngSB and XLink...");
            factory.DisconnectFromXLink(connectorUUID, hostIp);
            factory.UnRegisterConnector(connectorUUID);
            factory.DeleteDomainService(connectorUUID);
            factory.StopConnection(connectorUUID);
            outputLine("Disconnected.");
        }

        private void outputLine(string line)
        {
            Console.WriteLine(line);
        }

        public bool isConnected()
        {
            return connected;
        }

        /// <summary>
        /// Redefines the array with currently installed local software tools. 
        /// Since the Domain Connector uses OOSourceCodeDomain.XLinkConnector but the blueprint uses OpenEngSBCore.XLinkConnector, the setter must 
        /// set values manually.
        /// </summary>
        public void setCurrentlyInstalledTools(OpenEngSBCore.XLinkConnector[] newArryOfInstalledTools)
        {
            currentlyInstalledTools = convertBetweenDLLTypes(newArryOfInstalledTools);
        }

        /// <summary>
        /// TODO TBW
        /// unterschiedliche DLLS deshalb andere typen
        /// </summary>
        private OOSourceCodeDomain.XLinkConnector[] convertBetweenDLLTypes(OpenEngSBCore.XLinkConnector[] newArryOfInstalledTools)
        {
            OOSourceCodeDomain.XLinkConnector[] convertedArray = new OOSourceCodeDomain.XLinkConnector[newArryOfInstalledTools.Length];
            for (int i = 0; i < newArryOfInstalledTools.Length; i++)
            {
                convertedArray[i].id = newArryOfInstalledTools[i].id;
                convertedArray[i].toolName = newArryOfInstalledTools[i].toolName;
                convertedArray[i].availableViews = new OOSourceCodeDomain.XLinkConnectorView[newArryOfInstalledTools[i].availableViews.Length];
                for (int e = 0; e < newArryOfInstalledTools[i].availableViews.Length; e++)
                {
                    convertedArray[i].availableViews[e].viewId = newArryOfInstalledTools[i].availableViews[e].viewId;
                    convertedArray[i].availableViews[e].name = newArryOfInstalledTools[i].availableViews[e].name;
                    // TODO set available views map - unterschiedliche DLLS deshalb andere typen
                    //convertedArray[i].availableViews[e].descriptions = newArryOfInstalledTools[i].availableViews[e].descriptions;
                }
            }
            return convertedArray;
        }

        /// <summary>
        /// TODO TBW
        /// </summary>
        public void setCurrentlyInstalledTools(OOSourceCodeDomain.XLinkConnector[] newArryOfInstalledTools)
        {
            currentlyInstalledTools = newArryOfInstalledTools;
        }

        /// <summary>
        /// Creates the Array of Model/View relations, offered by the Tool, for XLink
        /// </summary>
        private ModelToViewsTuple[] initModelViewRelation()
        {
            ModelToViewsTuple[] modelsToViews
                = new ModelToViewsTuple[1];
            Dictionary<String, String> descriptions = new Dictionary<String, String>();
            descriptions.Add("en", "This view opens the values in a C# SourceCode viewer.");
            descriptions.Add("de", "Dieses Tool öffnet die Werte in einem C# SourceCode viewer.");

            OpenEngSBCore.XLinkConnectorView[] views = new OpenEngSBCore.XLinkConnectorView[1];
            views[0] = (new OpenEngSBCore.XLinkConnectorView() { name = "C# SourceCode View", viewId = Program.viewId, descriptions = descriptions.ConvertMap<entry3>() });
            modelsToViews[0] =
                    new ModelToViewsTuple()
                    {
                        description = new ModelDescription() { modelClassName = classNameOfOpenEngSBModel, versionString = "3.0.0.SNAPSHOT" },
                        views = views
                    };
            return modelsToViews;
        }

        /// <summary>
        /// Creates a XLink to the given WorkingDirectoryFile and copies it to the clipboard. 
        /// Aborts the creation if no connection to the OpenEngSB is established.
        /// </summary>
        /// <param name="file"></param>
        public string createXLink(WorkingDirectoryFile file)
        {
            if (!connected)
            {
                outputLine("Error while creating XLink. No connection to OpenEngSB.");
                return null;
            }
            ModelDescription modelInformation = blueprint.viewToModels.ConvertMap<String, ModelDescription>()[Program.viewId];

    	    /*Note that only the target class SQLCreate is allowed */
            if (!modelInformation.modelClassName.Equals(classNameOfOpenEngSBModel))
            {
                outputLine("Error: Defined ModelClass '"+ classNameOfOpenEngSBModel + "' for view, from OpenEngSB, is not supported by this software program.");
                return null;
            }

            String completeUrl = blueprint.baseUrl;
            completeUrl += "&" + blueprint.keyNames.modelClassKeyName + "=" + HttpUtility.UrlEncode(modelInformation.modelClassName);
            completeUrl += "&" + blueprint.keyNames.modelVersionKeyName + "=" + HttpUtility.UrlEncode(modelInformation.versionString);
            completeUrl += "&" + blueprint.keyNames.contextIdKeyName + "=" + HttpUtility.UrlEncode(openengsbContext);      

            string objectString = convertWorkingDirectoryFileToJSON(file);
            completeUrl += "&" + blueprint.keyNames.identifierKeyName + "=" + HttpUtility.UrlEncode(objectString);

            return completeUrl;
        }

        /// <summary>
        /// Converts a WorkingDirectoryFile instance to a OOCLass instance and serializes it to String, with JSON
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string convertWorkingDirectoryFileToJSON(WorkingDirectoryFile file)
        {
            OOClass ooClassOfFile = LinkingUtils.convertWorkingDirectoryFileToOpenEngSBModel(file);
            string output = JsonConvert.SerializeObject(ooClassOfFile);
            //HACK: remove isSpecified fields from JSON String
            output = output.Replace(",\"isStaticSpecified\":true","");
            output = output.Replace(",\"isFinalSpecified\":true", "");
            return output;
        }

        /// <summary>
        /// List the other local software tools, currently using xlink.
        /// </summary>
        public void listOtherLocalInstalledSoftwareTools()
        {
            if (currentlyInstalledTools == null || currentlyInstalledTools.Length == 0)
            {
                outputLine("The list of other local installed tools is currently not available or empty.");
                return;
            }
            outputLine("List of other local installed tools using xlink:");
            for (int i = 0; i < currentlyInstalledTools.Length; i++)
            {
                outputLine("\n"+currentlyInstalledTools[i].toolName);
                for (int e = 0; e < currentlyInstalledTools[i].availableViews.Length; e++)
                {
                    string localizedDescription = "";
                    //test if the description is available in the defined language
                    if (currentlyInstalledTools[i].availableViews[e].descriptions.ConvertMap<String, String>()[Program.locale] != null)
                    {
                        localizedDescription = currentlyInstalledTools[i].availableViews[e].descriptions.ConvertMap<String, String>()[Program.locale];
                    }
                    else
                    {
                        //if not set arbitary value of map
                        //ps: dummy access to an arbitary value of the map, sure there are better ways.
                        foreach (String dummyLoop in currentlyInstalledTools[i].availableViews[e].descriptions.ConvertMap<String, String>().Values)
                        {
                            localizedDescription = dummyLoop;
                            break;
                        }                                        
                    }
                    outputLine("\t"+currentlyInstalledTools[i].availableViews[e].viewId + " - " + localizedDescription);
                }
            }
        }

        /// <summary>
        /// TODO TBW
        /// </summary>
        public void triggerLocalSwitch(String programname, String viewId, String filename)
        {
            OOSourceCodeDomain.XLinkConnector otherLocalTool = findCurrentlyInstalledToolToName(programname);
            if (otherLocalTool == null)
            {
                outputLine("Supplied programname '"+programname+"' unknown.");
                return;
            }
            OOSourceCodeDomain.XLinkConnectorView otherLocalView = null;
            for(int i=0;i<otherLocalTool.availableViews.Length;i++)
            {
                if (otherLocalTool.availableViews[i].viewId.Equals(viewId))
                {
                    otherLocalView = otherLocalTool.availableViews[i];
                    break;
                }
            }
            if (otherLocalView == null)
            {
                outputLine("Supplied viewId '" + viewId + "' unknown to program '" + programname + "'.");
                return;
            }

            WorkingDirectoryFile searchedFile = Program.directoryBrowser.searchForFile(filename);
            if (searchedFile == null)
            {
                outputLine("Specified file was not found.");
                return;
            }
            else
            {
                String xlink = createXLink(searchedFile);
                xlink += "&" + blueprint.connectorId + "&" + blueprint.keyNames.viewIdKeyName + "=" + HttpUtility.UrlEncode(viewId);
                WebRequest webRequest = WebRequest.Create(xlink);
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    outputLine("Local switch was triggered.");
                }
                else
                {
                    outputLine("Local switch was not successfull triggered, returned status code was " + (int)response.StatusCode);
                }
            }
        }

        private OOSourceCodeDomain.XLinkConnector findCurrentlyInstalledToolToName(String programname)
        {
            return null;
        }
    }
}
