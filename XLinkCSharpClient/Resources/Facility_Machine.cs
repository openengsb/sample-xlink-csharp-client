using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsharpSampleClient
{
    class Facility_Machine
    {
        public long fm_Id { get; set; }    
        public int machineSerialId { get; set; } 
        public int averageOutput { get; set; } 
        public DateTime purchaseDate { get; set; } 
        public long productionFacility_Id { get; set; } 
    }
}
