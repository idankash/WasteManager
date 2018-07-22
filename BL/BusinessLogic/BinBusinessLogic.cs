using BL.AtomicDataModels;
using DAL;
using FND;
using System;
using System.Collections;
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

        public BinBusinessLogic(WasteManagerEntities db) : base(db)
        {

        }

        public List<BinData> GetAllBinsData()
        {
            List<Bin> dbBins = null;
            List<BinData> bins = new List<BinData>();

            try
            {
                dbBins = this.db.Bins.ToList();

                foreach (Bin dbBin in dbBins)
                {
                    bins.Add(transformToBinData(dbBin));
                }

                return bins;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public List<Bin> GetAllBins()
        {
            try
            {
                return this.db.Bins.ToList();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public Bin GetBin(int id)
        {
            try
            {
                return this.db.Bins.Where(x => x.BinId == id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public BinData GetBinData(int id)
        {
            Bin dbBin = GetBin(id);

            return transformToBinData(dbBin);
        }

        public void AddNewBinData(BinData i_BinData)
        {
            AddNewBin(transformBinDataToBin(i_BinData));
        }

        public Bin AddNewBin(Bin newBin)
        {
            try
            {
                this.db.Bins.Add(newBin);
                this.db.SaveChanges();      //TODO - check if newBin got an id from db 
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }

            return newBin;
        }

        public void UpdateBinData(BinData i_BinData)
        {
            UpdateBin(transformBinDataToBin(i_BinData), DateTime.Now);
        }

        public void UpdateBin(Bin updatedBin, DateTime dt)
        {
            try
            {
                Bin oldBin = this.db.Bins.Where(x => x.BinId == updatedBin.BinId).SingleOrDefault();

                if (oldBin != null)
                {
                    //oldBin.CurrentCapacity = updatedBin.CurrentCapacity;
                    oldBin = updatedBin;
                    //you can update more properties here.... (in the same way)
                    updateBinLogForEachBinAction(updatedBin, dt);
                }

                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public void DeleteBin(int id)
        {
            try
            {
                Bin binToRemove = this.db.Bins.Where(x => x.BinId == id).SingleOrDefault();
                this.db.Bins.Remove(binToRemove);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public double GetMaxCapacityByBinType(int binTypeId)
        {
            LUT_BinType bt;

            try
            {
                bt = this.db.LUT_BinType.Where(x => x.BinTypeId == binTypeId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }

            if (bt == null)
            {
                throw ErrorHandler.Handle(new Exception("There isn't id like this in the system."), this);
            }

            return bt.Capacity;
        }

        private BinData transformToBinData(Bin i_Bin)
        {
            BinData binData = new BinData();

            binData.binId = i_Bin.BinId;
            binData.binTypeId = i_Bin.BinTypeId;
            binData.binTypeDesc = i_Bin.LUT_BinType.BinTypeDesc;
            binData.cityAddress = i_Bin.CityAddress;
            binData.currentCapacity = i_Bin.CurrentCapacity;
            binData.isInUser = i_Bin.IsInUse;
            binData.maxCapacity = i_Bin.LUT_BinType.Capacity;
            binData.streetAddress = i_Bin.StreetAddress;
            binData.streetNumber = i_Bin.StreetNumber;

            return binData;
        }

        private Bin transformBinDataToBin(BinData i_BinData)
        {
            Bin bin = new Bin();

            bin.CityAddress = i_BinData.cityAddress;
            bin.IsInUse = i_BinData.isInUser;
            bin.StreetAddress = i_BinData.streetAddress;
            bin.CurrentCapacity = i_BinData.currentCapacity;
            bin.BinTypeId = i_BinData.binTypeId;
            bin.StreetNumber = i_BinData.streetNumber;
            bin.LUT_BinType.Capacity = i_BinData.maxCapacity;
            bin.LUT_BinType.BinTypeDesc = i_BinData.binTypeDesc;

            return bin;
        }

        private void updateBinLogForEachBinAction(Bin updatedBin, DateTime dt)
        {
            BinLog bLog = new BinLog();

            bLog.BinId = updatedBin.BinId;
            bLog.CurrentCapacity = updatedBin.CurrentCapacity;
            bLog.UpdateDate = dt;

            try
            {
                this.db.BinLogs.Add(bLog);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }
    }
}
