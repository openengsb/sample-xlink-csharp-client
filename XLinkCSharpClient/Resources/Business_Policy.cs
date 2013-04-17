using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsharpSampleClient
{
    class Business_Policy
    {
        public long bp_Id { get; set; }
        public string policyNumber { get; set; }
        public string policyDescription { get; set; }
        public string author { get; set; }
        public DateTime lastUpdated { get; set; }
    }
}
