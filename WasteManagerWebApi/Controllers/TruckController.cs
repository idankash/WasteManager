using BL;
using BL.AtomicDataModels;
using BL.BusinessLogic;
using DAL;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WasteManagerWebApi.ViewDataModels;

namespace WasteManagerWebApi.Controllers
{
    public class TruckController : BaseController
    {
        [HttpGet]
        public TruckManagementViewModel GetTruckManagementViewModel()
        {
            try
            {
                TruckManagementViewModel viewModel = new TruckManagementViewModel();
                using (TruckBusinessLogic truckBusinessLogic = new TruckBusinessLogic())
                {
                    viewModel.trucks = truckBusinessLogic.GetAllTrucks();
                    viewModel.truckTypes = truckBusinessLogic.GetAllTypes();
                }

                using (LutLogic lutLogic = new LutLogic())
                {
                    viewModel.areas = lutLogic.GetLutArea();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        public List<LutItem> areas { get; set; }
    }
}