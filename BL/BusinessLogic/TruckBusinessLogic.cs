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

        // TRUCK - CRUD 

        public List<Truck> GetAllTrucks()
        {
            try
            {
                return this.db.Trucks.ToList();
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
                    truck = updatedTruck;
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
    }
}
