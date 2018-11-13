using BL.AtomicDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasteManagerWebApi.ViewDataModels
{
    public class BuildingAreaManagementViewModel
    {
        public List<LutItem> areas { get; set; }
        public List<DbBuilding> buildings { get; set; }
    }
}