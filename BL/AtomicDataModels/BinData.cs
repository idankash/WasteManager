using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BL.AtomicDataModels
{
    public class BinData
    {
        public int binId { get; set; }
        public int binTypeId { get; set; }
        public string binTypeDesc { get; set; }
        public string cityAddress { get; set; }
        public string streetAddress { get; set; }
        public string streetNumber { get; set; }
        public double currentCapacity { get; set; }
        public double maxCapacity { get; set; }
        public bool isInUser { get; set; }
    }
}