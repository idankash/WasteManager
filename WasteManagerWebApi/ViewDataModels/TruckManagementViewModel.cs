using BL.AtomicDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasteManagerWebApi.ViewDataModels
{
    public class TruckManagementViewModel
    {
        public List<TruckData> trucks { get; set; }
        public List<LutItem> areas { get; set; }
        public List<TruckType> truckTypes { get; set; }
    }
}