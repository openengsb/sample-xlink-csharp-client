using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using OOSourceCodeDomain;

using Org.Openengsb.XLinkCSharpClient.Model;

namespace Org.Openengsb.XLinkCSharpClient.XLink
{
    /// <summary>
    /// Utils class providing a conversion method to connect the interal model to the defined OpenEngSBModel
    /// </summary>
    class LinkingUtils
    {
        private LinkingUtils() { }

        /// <summary>
        /// Convert the interal model "WorkingDirectoryFile" to the defined OpenEngSBModel "OOClass".
        /// Returns null if resulting instance lacks classname or packagename.
        /// </summary>
        public static OOClass convertWorkingDirectoryFileToOpenEngSBModel(WorkingDirectoryFile wdf)
        {
            //Regex to fetch Data
            Regex classRegex = new Regex(@".*class ([a-zA-Z0-9_]+).*");
            Regex packageRegex = new Regex(@".*namespace ([a-zA-Z0-9_\.]+).*");
            Regex varRegex = new Regex(@" *(public|protected|private) (int|double|float|string|DateTime|bool|long) ([a-zA-Z0-9_]+) {.*");

            OOClass resultingInstance = new OOClass();
            List<OOVariable> variables = new List<OOVariable>();
            //not used in example: set this with dummy values
            resultingInstance.methods = "";

            string[] contentLines = wdf.content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < contentLines.Length; i++)
            {
                if (resultingInstance.packageName == null)
                {
                    Match match = packageRegex.Match(contentLines[i]);
                    if (match.Success)
                    {
                        resultingInstance.packageName = match.Groups[1].Value;
                    }
                }
                else if (resultingInstance.className == null)
                {
                    Match match = classRegex.Match(contentLines[i]);
                    if (match.Success)
                    {
                        resultingInstance.className = match.Groups[1].Value;
                    }
                }
                else
                {
                    Match match = varRegex.Match(contentLines[i]);
                    if (match.Success)
                    {
                        OOVariable newVar = new OOVariable();
                        newVar.type = match.Groups[2].Value;
                        newVar.name = match.Groups[3].Value;
                        //not used in example: set this with dummy values
                        newVar.isFinal = false;
                        newVar.isFinalSpecified = true;
                        newVar.isStatic = false;
                        newVar.isStaticSpecified = true;
                        variables.Add(newVar);
                    }
                }

            }
            if (resultingInstance.packageName == null || resultingInstance.className == null)
            {
                //Instance not correctly set
                return null;
            }
            resultingInstance.attributes = new OOVariable[variables.Count];
            for (int i = 0; i < variables.Count; i++)
            {
                resultingInstance.attributes[i] = variables[i];
            }
            return resultingInstance;
        }
    }
}
