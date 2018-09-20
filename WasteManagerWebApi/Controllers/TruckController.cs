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
    public class TrucksController : BaseController
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
                return viewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public TruckData AddNewTruck(TruckData newTruck)
        {
            try
            {
                using(TruckBusinessLogic truckBusinessLogic = new TruckBusinessLogic())
                {
                    return truckBusinessLogic.AddNewTruck(newTruck);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public List<TruckData> GetAllTrucks()
        {
            try
            {
                using(TruckBusinessLogic truckBusinessLogic = new TruckBusinessLogic())
                {
                    return truckBusinessLogic.GetAllTrucks();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete]
        public void DeleteTruck(int truckId)
        {
            try
            {
                using(TruckBusinessLogic truckBusinessLogic = new TruckBusinessLogic())
                {
                    truckBusinessLogic.DeleteTruck(truckId);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public void UpdateTruck(TruckData updatedTruck)
        {
            try
            {
                using (TruckBusinessLogic truckBusinessLogic = new TruckBusinessLogic())
                {
                    truckBusinessLogic.UpdateTruck(updatedTruck);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}