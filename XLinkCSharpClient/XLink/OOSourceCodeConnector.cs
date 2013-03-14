using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using OOSourceCodeDomain;

using Org.Openengsb.XLinkCSharpClient.Model;

namespace Org.Openengsb.XLinkCSharpClient.XLink
{

    /// <summary>
    /// ConnectorImplementaiton of the OOSourecCode Domain, defined at the OpenEngSB. 
    /// Since the Domain defines XLink methods, the XLink functionality is implemented here.
    /// </summary>
    class OOSourceCodeConnector : IOOSourceCodeDomainSoap11Binding
    {

        public OOSourceCodeConnector()
        {
        }

        /// <summary>
        /// Updates the list with local registered software tools.
        /// </summary>
        public void onRegisteredToolsChanged(XLinkConnector[] currentlyInstalledTools)
        {         
            Program.openengsbConnectionManager.setCurrentlyInstalledTools(currentlyInstalledTools);
        }

        /// <summary>
        /// Triggers a search for WorkingdirectoryFiles corresponding to incomming matching OOClass instances
        /// </summary>
        public void openXLinks(object[] matchingObjects, string viewId)
        {
            if (!Program.viewId.Equals(viewId))
            {
                outputLine("An XLink matching was triggerd with an unknown viewId.");
            }

            //Start matching in thread to avoid timeout at the server
            SearchForMatchesThread searchThread = new SearchForMatchesThread(matchingObjects);
            Thread xlinkMatchingThread = new Thread(searchThread.searchForMatchesThread);
            xlinkMatchingThread.Start();
        }

        /// <summary>
        /// Domain Method of OOSourceCodeDomain, not used during example
        /// </summary>
        public void updateClass(OOClass args0)
        {
            outputLine("'updateClass' was triggered from the OpenEngSB");
            //Implement in real Program
        }

        public void getAliveState(out orgopenengsbcoreapiAliveState? @return, out bool returnSpecified)
        {
            @return = orgopenengsbcoreapiAliveState.ONLINE;
            returnSpecified = true;
        }

        public string getInstanceId()
        {
            return null;
        }

        private void outputLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
