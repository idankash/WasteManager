using BL.AtomicDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasteManagerWebApi.ViewDataModels
{
    public class BinInformationViewModel
    {
        public List<BinData> Bins { get; set; }
        //public List<LutBintype> binTypes...
        //  ...
    }
}