using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public class BinBusinessLogic : BaseBusinessLogic
    {
        public BinBusinessLogic()
        {
            this.db = new WasteManagerEntities();
        }

        //TODO - Implement Methods of GET/SET/UPDATE/DELETE things from the database

        public List<Bin> GetAllBins()
        {
            return this.db.Bins.ToList();
        }

        public Bin GetBin(int id)
        {
            return this.db.Bins.Where(x => x.BinId == id).SingleOrDefault();
        }

        public Bin AddNewBin(Bin newBin)
        {
            this.db.Bins.Add(newBin);
            this.db.SaveChanges();
            //TODO - check if newBin got an id from db 
            return newBin;
        }

        public void UpdateBin(Bin updatedBin, DateTime dt)
        {
            Bin oldBin = this.db.Bins.Where(x => x.BinId == updatedBin.BinId).SingleOrDefault();
            if (oldBin != null)
            {
                //oldBin.CurrentCapacity = updatedBin.CurrentCapacity;
                oldBin = updatedBin;
                //you can update more properties here.... (in the same way)
                InsertInfoToBinLog(updatedBin, dt);
            }
            this.db.SaveChanges();

        }

        // TODO - Refactor method name
        private void InsertInfoToBinLog(Bin updatedBin, DateTime dt)
        {
            // TODO: Think about moving the code below into a class ... :S

            BinLog bLog = new BinLog();
            bLog.BinId = updatedBin.BinId;
            bLog.CurrentCapacity = updatedBin.CurrentCapacity;
            bLog.UpdateDate = dt;

            db.BinLogs.Add(bLog);

            db.SaveChanges();
        }

        public void DeleteBin(int id)
        {
            Bin binToRemove = db.Bins.Where(x => x.BinId == id).SingleOrDefault();
            db.Bins.Remove(binToRemove);
            db.SaveChanges();
        }





        // TRUCK - CRUD 

        public List<Truck> GetAllTrucks()
        {
            return db.Trucks.ToList();
        }

        public Truck GetTruck(int id)
        {
            return db.Trucks.Where(x => x.TruckId == id).SingleOrDefault();
        }

        public Truck AddNewTruck(Truck newTruck)
        {
            db.Trucks.Add(newTruck);
            db.SaveChanges();

            return newTruck;
        }

        public void UpdateTruck(Truck updatedTruck)
        {
            Truck truck = db.Trucks.Where(x => x.TruckId == updatedTruck.TruckId).SingleOrDefault();

            if (truck != null)
            {
                truck = updatedTruck;
            }

            db.SaveChanges();
        }

        public void DeleteTruck(int id)
        {
            Truck truck = db.Trucks.Where(x => x.TruckId == id).SingleOrDefault();

            db.Trucks.Remove(truck);
            db.SaveChanges();
        }

        public double GetMaxCapacityByBinType(int binTypeId)
        {
            LUT_BinType bt = db.LUT_BinType.Where(x => x.BinTypeId == binTypeId).SingleOrDefault();

            if (bt == null)
            {
                throw new Exception();
            }

            return bt.Capacity;
        }
    }
}