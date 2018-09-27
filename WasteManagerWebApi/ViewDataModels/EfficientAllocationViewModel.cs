using BL.AtomicDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasteManagerWebApi.ViewDataModels
{
    public class EfficientAllocationViewModel
    {
        public List<BuildingData> buildings { get; set; }
    }
}