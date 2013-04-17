using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsharpSampleClient
{
    class Customer
    {
        public long co_Id { get; set; }
        public int numberOfOrders { get; set; }
        public int creditcardNumber { get; set; }
        public int bankAccountNumber { get; set; }
        public int bankNumber { get; set; }
        public float balance { get; set; }    
    }
}
