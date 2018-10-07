using BL.AtomicDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasteManagerWebApi.ViewDataModels
{
    public class EffectiveSchedulingViewModel
    {
        public List<AreaData> areas { get; set; }
        public List<TruckData> trucks { get; set; }
    }
}