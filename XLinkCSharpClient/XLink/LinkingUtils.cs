using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OOSourceCodeDomain;

using Org.Openengsb.XLinkCSharpClient.Model;

namespace Org.Openengsb.XLinkCSharpClient.XLink
{
    /// <summary>
    /// TODO TBW
    /// </summary>
    class LinkingUtils
    {
        private LinkingUtils() { }

                    

	        /* TODO
             * Regex _regex = new Regex(@".*class ([a-zA-Z0-9_]+).*");
             * Match match = _regex.Match(line);
             * return match.Groups[1].Value;
             */
                    
        public static OOClass convertWorkingDirectoryFileToOpenEngSBModel(WorkingDirectoryFile wdf)
        {
            //TODO convert WorkingDirectoryFile to model
            OOClass test = new OOClass();
            test.className = "testClass";
            test.packageName = "testpackage";
            test.methods = "bla";
            OOVariable testVar = new OOVariable();
            testVar.name = "varName";
            testVar.type = "varType";
            test.attributes = new OOVariable[1];
            test.attributes[0] = testVar;
            return test;
        }
    }
}
