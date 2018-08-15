using BL;
using DAL;
using FND;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL.AtomicDataModels;
using System.Web.Http.Cors;

namespace WasteManagerWebApi.Controllers
{
    [EnableCors(origins:"*", headers:"*",methods:"*")]    
    public class TestController : BaseController
    {
        [HttpGet]
        public bool test()  
        {
            Logger.Instance.WriteInfo("test controller", this);
            return true;
        }

        [HttpGet]
        public List<BinData> GetAllBins()
        {
            List<BinData> bins = null;
            try
            {
                using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic())
                {
                    bins = binBusinessLogic.GetAllBinsData();
                }

                return bins;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, this);
                return null;
            }
        }
    }
}
