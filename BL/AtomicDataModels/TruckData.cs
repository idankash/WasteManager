using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL.AtomicDataModels
{
    public class TruckData
    {
        public int truckId { get; set; }
        public int truckTypeId { get; set; }
        public string truckTypeDesc { get; set; }
        public int? areaId { get; set; }
        public string areaDesc { get; set; }
        public double currentCapacity { get; set; }
        public double maxCapacity { get; set; }

    }
}