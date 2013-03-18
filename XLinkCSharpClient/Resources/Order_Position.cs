using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsharpSampleClient
{
    class Order_Position
    {
        public long op_Id { get; set; }
        public string name { get; set; }
        public int size { get; set; }   
        public double cost { get; set; }  
        public int amount { get; set; }  
        public long clientContract_Id { get; set; } 
        public long productStorage_Id { get; set; } 
    }
}
