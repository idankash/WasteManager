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

                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    viewModel.buildings = buildingLogic.GetBuildings();
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public Bin AddNewBin(BinData newBin)
        {
            try
            {
                using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic())
                {
                    return binBusinessLogic.AddNewBin(newBin);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public List<BinData> GetAllBins()
        {
            try
            {
                using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic())
                {
                    return binBusinessLogic.GetAllBins();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public void DeleteBin(int binId)
        {
            try
            {
                using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic())
                {
                    binBusinessLogic.DeleteBin(binId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public void UpdateBin(BinData updatedBin)
        {
            try
            {
                using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic())
                {
                    binBusinessLogic.UpdateBin(updatedBin , DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
