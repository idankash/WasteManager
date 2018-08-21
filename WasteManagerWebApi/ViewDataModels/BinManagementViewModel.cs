using BL.AtomicDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasteManagerWebApi.ViewDataModels
{
    public class BinManagementViewModel
    {
        List<BinData> bins { get; set; }
        List<Area> areas { get; set; }
        List<BinType> binTypes { get; set;}

    }
}