using BL;
using BL.AtomicDataModels;
using BL.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WasteManagerWebApi.ViewDataModels;

namespace WasteManagerWebApi.Controllers
{
    public class AreaController : BaseController
    {
        [HttpGet]
        public EffectiveSchedulingViewModel GetEffectiveSchedulingViewModel()
        {
            try
            {
                EffectiveSchedulingViewModel viewModel = new EffectiveSchedulingViewModel();

                using(LutLogic lutLogic = new LutLogic())
                {
                    viewModel.areas = lutLogic.GetAreaData();
                }
                

                using (TruckBusinessLogic truckBusinessLogic = new TruckBusinessLogic())
                {
                    viewModel.trucks = truckBusinessLogic.GetAllTrucks();
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public AreaData GetTotalStats()
        {
            try
            {
                using (LutLogic luLogic = new LutLogic())
                {
                    return luLogic.GetTotalStats();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public int WorkSchedule() //1 = success 0 = failure
        {
            try
            {
                using (TruckBusinessLogic truckBusinessLogic = new TruckBusinessLogic())
                {
                    return truckBusinessLogic.WorkSchedule();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public List<AreaData> GetAreaData()
        {
            try
            {
                using (LutLogic luLogic = new LutLogic())
                {
                    return luLogic.GetAreaData();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public int GetNumOfCleanups(int truckId, int areaId)
        {
            try
            {
                using (LutLogic luLogic = new LutLogic())
                {
                    return luLogic.GetNumOfCleanups(truckId, areaId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public int ManuallyWorkSchedule([FromBody] UpdatedArea updatedArea) //better way to do this?? without new class?? 
        {
            try
            {
                using (TruckBusinessLogic truckBusinessLogic = new TruckBusinessLogic())
                {
                    return truckBusinessLogic.ManuallyWorkSchedule(updatedArea.updatedArea);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
