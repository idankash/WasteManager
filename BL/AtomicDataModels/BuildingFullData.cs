using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.AtomicDataModels
{
    public class BuildingFullData
    {
        public string streetName { get; set; }
        public string streetNumber { get; set; }
        public int buildingId { get; set; }
        public int areaId { get; set; }
        public int numOfDays { get; set; }
        public double binsArea { get; set; }
        public double avgCapacity { get; set; }
    }
}
