using BL.AtomicDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasteManagerWebApi.ViewDataModels
{
    public class BinManagementViewModel
    {
        public List<BinData> bins { get; set; }
        public List<LutItem> areas { get; set; }
        public List<BinType> binTypes { get; set;}
        public List<BuildingData> buildings { get; set; }
    }
}