using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.AtomicDataModels
{
    public class BinType
    {
        public int binTypeId { get; set; }

        public string binTypeDesc { get; set; }

        public double capacity { get; set; }

        public double binTrashDisposalArea { get; set; }
    }
}
