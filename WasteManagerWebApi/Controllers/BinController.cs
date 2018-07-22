using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL.AtomicDataModels;
using DAL;

namespace WasteManagerWebApi.Controllers
{
    public class BinController : BaseController
    {
        [HttpGet]
        [Route(@"api/Bin/GetAllBins")]
        public List<BinData> GetAllBins()
        {
            List<BinData> allBins = new List<BinData>();

            using (BL.BinBusinessLogic bl = new BL.BinBusinessLogic())
            {
                allBins = bl.GetAllBinsData();
            }

            return allBins;
        }

        [HttpGet]
        [Route(@"api/Bin/GetBin/{id}")]
        public BinData GetBinById(int id)
        {
            BinData bin;

            using (BL.BinBusinessLogic bl = new BL.BinBusinessLogic())
            {
                 bin = bl.GetBinData(id);
            }

            return bin;
        }

        [HttpPost]
        [Route("api/Bin/UpdateBin")]
        public IHttpActionResult UpdateBin([FromBody]BinData i_Bin)
        {
            IHttpActionResult result = Ok();

            if (i_Bin.binId <= 0)
            {
                result = BadRequest();
            }

            else
            {
                using (BL.BinBusinessLogic bl = new BL.BinBusinessLogic())
                {
                    bl.UpdateBinData((i_Bin));
                }
            }

            return result;
        }

        [HttpPost]
        [Route(@"api/Bin/AddBin")]
        public IHttpActionResult AddBin([FromBody]BinData i_Bin)
        {
            IHttpActionResult result = Ok();

            if (i_Bin == null)
            {
                result = BadRequest();
            }


            using (BL.BinBusinessLogic bl = new BL.BinBusinessLogic())
            {
                bl.AddNewBinData(i_Bin);
            }

            return result;
        }

        [HttpDelete]
        [Route("api/Bin/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            IHttpActionResult result = BadRequest();

            if (id <= 0)
            {
                result = BadRequest("Not a valid bin id");
            }

            else
            {
                using (BL.BinBusinessLogic bl = new BL.BinBusinessLogic())
                {
                    bl.DeleteBin(id);
                }

                result = Ok();
            }

            return result;
        }
    }
}
