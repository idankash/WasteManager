using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.AtomicDataModels
{
    public class AreaData
    {
        public LutItem area { get; set; }
        public int numOfBuildings { get; set; }
        public double capacity { get; set; }
        public double maxCapacity { get; set; }
        public int truckId { get; set; }
        public int numOfCleanups { get; set; }
    }
}
