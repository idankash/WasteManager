using BL;
using BL.AtomicDataModels;
using BL.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WasteManagerWebApi.ViewDataModels;

namespace WasteManagerWebApi.Controllers
{
    public class BinsController : BaseController
    {

        [HttpGet]
        public BinManagementViewModel GetBinManagementViewModel()
        {
            try
            {
                BinManagementViewModel viewModel = new BinManagementViewModel();

                using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic())
                {
                    viewModel.bins = binBusinessLogic.GetAllBinsData();
                    viewModel.binTypes = binBusinessLogic.GetAllBinTypes();
                }

                using (LutLogic lutLogic = new LutLogic())
                {
                    viewModel.areas = lutLogic.GetLutArea();
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpGet]
        //public List<BinData> GetAllBins()
        //{
        //    throw new NotImplementedException();
        //}

    }
}
