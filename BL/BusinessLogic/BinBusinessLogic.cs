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
        //TODO - Implement Methods of GET/SET/UPDATE/DELETE things from the database

        public List<BinData> GetAllBinsData()
        {
            List<spBin_GetBinListFullDetails_Result> dbBins = null;
            List<BinData> bins = new List<BinData>();

            try
            {
                dbBins = this.db.spBin_GetBinListFullDetails().ToList();

                foreach (spBin_GetBinListFullDetails_Result dbBin in dbBins)
                {
                    BinData binData = new BinData()
                    {
                        binId = dbBin.BinId,
                        binTypeId = dbBin.BinTypeId,
                        binTypeDesc = dbBin.BinTypeDesc,
                        cityAddress = dbBin.AreaDesc,
                        currentCapacity = dbBin.CurrentCapacity,
                        maxCapacity = dbBin.Capacity,
                        streetAddress = dbBin.BuildingAddress,
                        binTrashDisposalArea = dbBin.BinTrashDisposalArea,
                    };

                    bins.Add(binData);
                }
                return bins;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public List<BinType> GetAllBinTypes()
        {
            try
            {
                List<LUT_BinType> dbBinTypes = this.db.LUT_BinType.ToList();

                List<BinType> binTypes = new List<BinType>();

                foreach (LUT_BinType dbBin in dbBinTypes)
                {
                    BinType binType = new BinType()
                    {
                        binTypeId = dbBin.BinTypeId,
                        binTypeDesc = dbBin.BinTypeDesc,
                        capacity = dbBin.Capacity,
                        binTrashDisposalArea = dbBin.BinTrashDisposalArea
                    };

                    binTypes.Add(binType);
                }

                return binTypes;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public List<BinData> GetAllBins()
        {
            try
            {
                List<spBin_GetBinListFullDetails_Result> dbBins = this.db.spBin_GetBinListFullDetails().ToList();
                List<BinData> binDataList = new List<BinData>();

                foreach (var dbBin in dbBins)
                {
                    binDataList.Add(DbBinToBinData(dbBin));
                }
                return binDataList;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public Bin GetBin(int binId)
        {
            try
            {
                return this.db.Bins.Where(x => x.BinId == binId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public Bin AddNewBin(BinData newBin)
        {
            Bin dbBin;
            try
            {
                dbBin = new Bin();
                BinDataToDbBin(dbBin, newBin);
                this.db.Bins.Add(dbBin);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }

            return dbBin;
        }

        public void UpdateBin(BinData updatedBin, DateTime dt)
        {
            try
            {
                Bin oldBin = this.db.Bins.Where(x => x.BinId == updatedBin.binId).SingleOrDefault();
                if (oldBin == null)
                {
                    throw new Exception("Bin not found.");
                }

                BinDataToDbBin(oldBin, updatedBin);

                AddNewBinLog(oldBin, dt);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        private void AddNewBinLog(Bin updatedBin, DateTime dt)
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

        public void DeleteBin(int binId) //idan - changed method from deleting entire entity to remove buildingId from it.
        {
            try
            {
                Bin binToRemove = this.db.Bins.Where(x => x.BinId == binId).SingleOrDefault();
                if (binToRemove == null)
                {
                    throw new Exception("Bin not found");
                }
                binToRemove.BuildingId = null;
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
                if (bt == null)
                {
                    throw new Exception("There isn't id like this in the system.");
                }
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }

            return bt.Capacity;
        }

        public BinData DbBinToBinData(spBin_GetBinListFullDetails_Result dbBin)
        {
            BinData binData = new BinData()
            {
                binId = dbBin.BinId,
                binTypeId = dbBin.BinTypeId,
                binTypeDesc = dbBin.BinTypeDesc,
                cityAddress = dbBin.AreaDesc,
                streetAddress = dbBin.BuildingAddress,
                currentCapacity = dbBin.CurrentCapacity,
                maxCapacity = dbBin.Capacity,
                binTrashDisposalArea = dbBin.BinTrashDisposalArea,
                buildingId = dbBin.BuildingId// buildingid is nullable 
            };
            return binData;
        }

        public void BinDataToDbBin(Bin bin, BinData binData)
        {
            bin.BinTypeId = binData.binTypeId;
            bin.CurrentCapacity = binData.currentCapacity;
            bin.BuildingId = binData.buildingId;
        }
    }
}



#region old
//using BL.AtomicDataModels;
//using DAL;
//using FND;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;


//namespace BL
//{
//    public class BinBusinessLogic : BaseBusinessLogic
//    {
//        public BinBusinessLogic()
//        {
//            this.db = new WasteManagerEntities();
//        }

//        public BinBusinessLogic(WasteManagerEntities db) : base(db)
//        {

//        }
//        //TODO - Implement Methods of GET/SET/UPDATE/DELETE things from the database

//        public List<BinData> GetAllBinsData()
//        {
//            List<Bin> dbBins = null;
//            List<BinData> bins = new List<BinData>();

//            try
//            {
//                dbBins =this.db.Bins.ToList();

//                foreach (Bin dbBin in dbBins)
//                {
//                    BinData binData = new BinData()
//                    {
//                        binId = dbBin.BinId,
//                        binTypeId = dbBin.BinTypeId,
//                        binTypeDesc = dbBin.LUT_BinType.BinTypeDesc,
//                        //cityAddress = dbBin.CityAddress,
//                        currentCapacity = dbBin.CurrentCapacity,
//                        isInUser = dbBin.IsInUse,
//                        maxCapacity = dbBin.LUT_BinType.Capacity,
//                        //streetAddress = dbBin.StreetAddress,
//                        //streetNumber = dbBin.StreetNumber
//                    };

//                    bins.Add(binData);
//                }
//                return bins;
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public List<Bin> GetAllBins()
//        {
//            try
//            {
//                return this.db.Bins.ToList();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public Bin GetBin(int id)
//        {
//            try
//            {
//                return this.db.Bins.Where(x => x.BinId == id).SingleOrDefault();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public Bin AddNewBin(Bin newBin)
//        {
//            try
//            {
//                this.db.Bins.Add(newBin);
//                this.db.SaveChanges();      //TODO - check if newBin got an id from db 
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }

//            return newBin;
//        }

//        public void UpdateBin(Bin updatedBin, DateTime dt)
//        {
//            try
//            {
//                Bin oldBin = this.db.Bins.Where(x => x.BinId == updatedBin.BinId).SingleOrDefault();
//                if (oldBin != null)
//                {
//                    //oldBin.CurrentCapacity = updatedBin.CurrentCapacity;
//                    oldBin = updatedBin;
//                    //you can update more properties here.... (in the same way)
//                    AddNewBinLog(updatedBin, dt);
//                }
//                this.db.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        private void AddNewBinLog(Bin updatedBin, DateTime dt)
//        {
//            // TODO: Think about moving the code below into a class ... :S
//            BinLog bLog = new BinLog();

//            bLog.BinId = updatedBin.BinId;
//            bLog.CurrentCapacity = updatedBin.CurrentCapacity;
//            bLog.UpdateDate = dt;

//            try
//            {
//                this.db.BinLogs.Add(bLog);
//                this.db.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public void DeleteBin(int id)
//        {
//            try
//            {
//                Bin binToRemove = this.db.Bins.Where(x => x.BinId == id).SingleOrDefault();
//                this.db.Bins.Remove(binToRemove);
//                this.db.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public double GetMaxCapacityByBinType(int binTypeId)
//        {
//            LUT_BinType bt;

//            try
//            {
//                bt = this.db.LUT_BinType.Where(x => x.BinTypeId == binTypeId).SingleOrDefault();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }

//            if (bt == null)
//            {
//                throw ErrorHandler.Handle(new Exception("There isn't id like this in the system."), this);
//            }

//            return bt.Capacity;
//        }
//    }
//} 
#endregion