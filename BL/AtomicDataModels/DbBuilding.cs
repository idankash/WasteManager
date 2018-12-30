using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.AtomicDataModels
{
    public class DbBuilding
    {
        public string streetName { get; set; }
        public string streetNumber { get; set; }
        public int buildingId { get; set; }
        public int areaId { get; set; }
        public double trashDisposalArea { get; set; }
    }
}
