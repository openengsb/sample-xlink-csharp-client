using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsharpSampleClient
{
    class Facility_Output
    {
        public long fo_Id { get; set; }    
        public string itemName { get; set; }    
        public int outputAmount { get; set; }    
        public DateTime outputDate { get; set; }    
        public long productionFacility_Id { get; set; }    
    }
}
