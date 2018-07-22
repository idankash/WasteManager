using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL;
using BL.AtomicDataModels;
using DAL;

namespace WasteManagerWebApi.Controllers
{
    public class TruckController : BaseController
    {
        [HttpGet]
        [Route(@"api/Truck/GetAllTrucks")]
        public List<TruckData> GetAllTrucks()
        {
            List<Truck> allTrucks;

            List<TruckData> allTrucksModels = new List<TruckData>();

            using (BL.TruckBusinessLogic bl = new BL.TruckBusinessLogic())
            {             
                allTrucks = bl.GetAllTrucks();

                foreach (Truck truck in allTrucks)
                {
                    allTrucksModels.Add(transformLogicInfoToUIInfo(truck));
                }
            }

            return allTrucksModels;
        }

        [HttpGet]
        [Route(@"api/Truck/GetTruck/{id}")]
        public TruckData GetTruck(int id)
        {
            using (BL.TruckBusinessLogic bl = new BL.TruckBusinessLogic())
            {
                Truck truck = bl.GetTruck(id);
                return transformLogicInfoToUIInfo(truck);
            }
        }

        // There is a big question about the adding / updating truck to the DB.
        // the question is how we will let the client choose the new truck...
        // should it be in from a known list of types?
        // can he make a new brand truck from scratch?

        [HttpPost]
        [Route("api/Truck/UpdateTruck")]
        public IHttpActionResult UpdateTruck([FromBody]TruckData updatedTruck)
        {
            IHttpActionResult result = Ok();

            if (updatedTruck.Id <= 0)
            {
                result = BadRequest();
            }

            else
            {
                using (BL.TruckBusinessLogic bl = new BL.TruckBusinessLogic())
                {
                    bl.UpdateTruck(transformUIInfoToLogicInfo(updatedTruck));
                }
            }

            return result;
        }

        [HttpPost]
        [Route(@"api/Truck/NewTruck")]
        public IHttpActionResult AddTruck([FromBody]TruckData i_Truck)
        {
            IHttpActionResult result = Ok();

            if (i_Truck == null)
            {
                result = BadRequest();
            }

            Truck truck = new Truck();

            truck.TruckTypeId = i_Truck.TruckTypeId;

            

            using (BL.TruckBusinessLogic bl = new BL.TruckBusinessLogic())
            {
                
                bl.AddNewTruck(transformUIInfoToLogicInfo(i_Truck));
            }

            return result;
        }

        [HttpDelete]
        [Route("api/Truck/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            IHttpActionResult result = BadRequest();

            if (id <= 0)
            {
                result = BadRequest("Not a valid truck id");
            }

            else
            {
                using (BL.TruckBusinessLogic bl = new BL.TruckBusinessLogic())
                {
                    bl.DeleteTruck(id);
                }

                result = Ok();
            }

            return result;
        }


        // Had to add those 2 methods to convert from/to UI to/from Logic Truck's info.
        // Dunno if i should put it here or somewhere else..at least it works fine :P

        private TruckData transformLogicInfoToUIInfo(Truck i_Truck)
        {
            TruckData truck = new TruckData();

            truck.Capacity = i_Truck.LUT_TruckType.Capacity;
            truck.CurrentCapacity = i_Truck.CurrentCapacity;
            truck.Id = i_Truck.TruckId;
            truck.TruckTypeId = i_Truck.LUT_TruckType.TruckTypeId;
            truck.TruckTypeDesc = i_Truck.LUT_TruckType.TruckTypeDesc;

            return truck;
        }

        private Truck transformUIInfoToLogicInfo(TruckData i_Truck)
        {
            Truck truck = new Truck();

            truck.CurrentCapacity = i_Truck.CurrentCapacity;
            truck.TruckId = i_Truck.Id;
            truck.TruckTypeId = i_Truck.TruckTypeId;
            truck.LUT_TruckType.TruckTypeDesc = i_Truck.TruckTypeDesc;
            truck.LUT_TruckType.Capacity = i_Truck.Capacity;

            return truck;
        }
    }
}
