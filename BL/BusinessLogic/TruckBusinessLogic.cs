using DAL;
using FND;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class TruckBusinessLogic : BaseBusinessLogic
    {
        public TruckBusinessLogic()
        {
            this.db = new WasteManagerEntities();
        }

        public TruckBusinessLogic(WasteManagerEntities db) : base(db)
        {

        }

        // TRUCK - CRUD 

        public List<Truck> GetAllTrucks()
        {
            try
            {
                 return this.db.Trucks.ToList();
                // instead of return, move to a local var
                // transform(localvar);
                // return list of TruckData;
            }
            catch (Exception ex)
            {
                
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public Truck GetTruck(int id)
        {
            try
            {
                return this.db.Trucks.Where(x => x.TruckId == id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public Truck AddNewTruck(Truck newTruck)
        {
            try
            {
                this.db.Trucks.Add(newTruck);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }

            return newTruck;
        }

        public void UpdateTruck(Truck updatedTruck)
        {
            try
            {
                Truck truck = this.db.Trucks.Where(x => x.TruckId == updatedTruck.TruckId).SingleOrDefault();
                if (truck != null)
                {
                    truck.TruckId = updatedTruck.TruckId;
                    truck.TruckTypeId = updatedTruck.TruckTypeId;
                    truck.CurrentCapacity = updatedTruck.CurrentCapacity;
                }
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public void DeleteTruck(int id)
        {
            try
            {
                Truck truck = this.db.Trucks.Where(x => x.TruckId == id).SingleOrDefault();
                this.db.Trucks.Remove(truck);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public void ClearingBins(List<Bin> binList, int truckId, DateTime currentDateTime)  //Go over all the bins and unloading them.
        {
            Truck truck = GetTruck(truckId);

            foreach (Bin bin in binList)
            {
                truck.CurrentCapacity += bin.CurrentCapacity;
                this.UpdateTruck(truck);

                bin.CurrentCapacity = 0;
                using (BinBusinessLogic binBl = new BinBusinessLogic(this.db))
                {
                    binBl.UpdateBin(bin, currentDateTime);

                }

            }
        }

        //public void AddNewTruckType(int capacity, string truckTypeDesc)
        //{
        //    LUT_TruckType newTruckType = new LUT_TruckType();
        //    newTruckType.Capacity = 50000;
        //    newTruckType.TruckTypeDesc = "CoolTruck";

        //    this.db.LUT_TruckType.Add(newTruckType);

        //}
    }
}
