using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsharpSampleClient
{
    class Address
    {
        public long a_Id { get; set; }
        public string street { get; set; }
        public int number { get; set; }
        public string region { get; set; }
        public int postalCode { get; set; }
        public string country { get; set; }
        public int countryCode { get; set; }
    }
}
