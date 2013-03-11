using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OOSourceCodeDomain;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;

using Org.Openengsb.XLinkCSharpClient.Model;

namespace Org.Openengsb.XLinkCSharpClient.XLink
{
    /// <summary>
    /// TODO TBW
    /// </summary>
    class OpenEngSBConnectionManager
    {
        /*Supplied program arguments*/
        private String xlinkServerURL;
        private String domainId;
        private String programname;

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

        /*XLink variables*/
        private static IOOSourceCodeDomainSoap11Binding ooSourceConnector;
        private static IDomainFactory factory;

        private OpenEngSBConnectionManager(String xlinkServerURL, String domainId, String programname, String hostIp)
        {
            this.xlinkServerURL = xlinkServerURL;
            this.domainId = domainId;
            this.programname = programname;
            this.connected = false;
            this.hostIp = hostIp;
        }	

        /// <summary>
        /// Initializes the Connectors only instance.
        /// </summary>
        /// <param name="xlinkBaseUrl">Link to OpenEngSB server</param>
        /// <param name="hostIp">IP of the local host</param>
        public static void initInstance(String xlinkBaseUrl,
                String domainId, String programname,
                String hostIp)
        {
            instance = new OpenEngSBConnectionManager(xlinkBaseUrl, domainId,
                    programname, hostIp);
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
            factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", xlinkServerURL, ooSourceConnector, new RetryDefaultExceptionHandler());
            connectorUUID = factory.CreateDomainService(domainId); 
            factory.RegisterConnector(connectorUUID, domainId);
            blueprint = factory.ConnectToXLink(connectorUUID, hostIp, programname, initModelViewRelation());
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
        /// Creates the Array of Model/View relations, offered by the Tool, for XLink
        /// </summary>
        private static ModelToViewsTuple[] initModelViewRelation()
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
                    { //TODO replace model string
                        description = new ModelDescription() { modelClassName = "org.openengsb.domain.SQLCode.model.SQLCreate", versionString = "3.0.0.SNAPSHOT" },
                        views = views
                    };
            return modelsToViews;
        }

                /*TODO transfere to real connector*/
        public void createXLink(WorkingDirectoryFile file)
        {
            if (!connected)
            {
                outputLine("Error while creating XLink. No connection to OpenEngSB.");
                return;
            }
            String completeUrl = "dummyUrl"; //connector.getTemplate().getBaseUrl();
            completeUrl += "&" + ""; //connector.getTemplate().getModelClassKey() + "=" + urlEncodeParameter(modelInformation.getModelClassName());
            completeUrl += "&" + ""; //connector.getTemplate().getModelVersionKey() + "=" + urlEncodeParameter(modelInformation.getModelVersionString());
            completeUrl += "&" + ""; //connector.getTemplate().getContextIdKeyName() + "=" + urlEncodeParameter(openEngSBContext);   
            String classNameKey = "className";
            completeUrl += "&" + classNameKey + "=" + ""; //selectedStmt.getTableName();
            Clipboard.SetText(completeUrl);
        }
    }
}
