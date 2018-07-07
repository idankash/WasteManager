using BL;
using DAL;
using FND;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WasteManagerWebApi.Controllers
{
    public class TestController : ApiController
    {
        //[HttpGet]
        //public bool test()
        //{
        //    Logger.Instance.WriteInfo("test controller", this);
        //    return true;
        //}

        [HttpGet]
        public List<Bin> GetAllBins()
        {
            List<Bin> bins = null;
            try
            {
                using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic())
                {
                    bins = binBusinessLogic.GetAllBins();
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
