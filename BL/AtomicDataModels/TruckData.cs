using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BL.AtomicDataModels
{
    public class TruckData
    {
        public int Id { get; set; }

        // ATM i let this property be, dunno if we'll need it later.
        public int TruckTypeId { get; set; }

        public double CurrentCapacity { get; set;}

        public double Capacity { get; set; }

        public string TruckTypeDesc { get; set; }

    }
}