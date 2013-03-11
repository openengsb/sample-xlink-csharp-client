using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        public void onRegisteredToolsChanged(XLinkConnector[] currentlyInstalledTools)
        {
            Console.WriteLine("onRegisteredToolsChanged " + currentlyInstalledTools.Length);
        }

        public void openXLinks(object[] matchingObjects, string viewId)
        {
            if (!Program.viewId.Equals(viewId))
            {
                outputLine("An XLink matching was triggerd with an unknown viewId.");
            } 
            
            Console.WriteLine("matchingObjects " + matchingObjects.Length);


            //TODO convertMatchingObjects

            String className = "test";

            WorkingDirectoryFile searchedFile = null;
            foreach (WorkingDirectoryFile wdf in Program.directoryBrowser.getFilesFromWorkingDirectory())
            {
                if (wdf.fileName.Equals(className))
                {
                    searchedFile = wdf;
                    break;
                }
            }

            //TODO write sophisticated search algorithm

            if (searchedFile != null)
            {
                outputLine("opening file due to xlink call...");
                System.Diagnostics.Process.Start(searchedFile.wholePath);
            }
            else
            {
                outputLine("openXLink was called with " + className + " but no match was found.");
            }
        }

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
