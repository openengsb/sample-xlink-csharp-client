﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OOSourceCodeDomain;

using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;

namespace Org.Openengsb.XLinkCSharpClient.XLink
{
    /// <summary>
    /// Threadclass to trigger the search for XLink matches
    /// </summary>
    class SearchForMatchesThread
    {
        private object[] matchingObjects;

        public SearchForMatchesThread(object[] matchingObjects)
        {
            this.matchingObjects = matchingObjects;
        }

        /// <summary>
        /// Searches for Matches to incomming XLinks, in the local WorkingDirectory
        /// </summary>
        public void searchForMatchesThread()
        {
            for (int i = 0; i < matchingObjects.Length; i++)
            {
                IMarshaller marseller = new JsonMarshaller();
                OOClass currentPotentialMatch = marseller.UnmarshallObject<OOClass>(matchingObjects[i].ToString());
                Program.directoryBrowser.searchForXLinkMatches(currentPotentialMatch);
            }
        }
    }
}
