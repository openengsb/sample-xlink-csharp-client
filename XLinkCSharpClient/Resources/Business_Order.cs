using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsharpSampleClient
{
    class Business_Order
    {
        public long bo_Id { get; set; }
        public string organisation { get; set; }
        public double orderTotal { get; set; }
        public long business_policy_id { get; set; }
        public long customer_id { get; set; }
        public DateTime creationDate  { get; set; }
    }
}
