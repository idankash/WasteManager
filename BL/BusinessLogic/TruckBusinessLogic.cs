using BL.AtomicDataModels;

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

        public List<TruckData> GetAllTrucks()
        {
            List<Truck> dbTrucks = null;
            List<TruckData> trucks = new List<TruckData>();

            try
            {
                dbTrucks = this.db.Trucks.ToList();

                TruckData truckData;
                foreach (Truck dbTruck in dbTrucks)
                {
                    truckData = DbTruckToTruckData(dbTruck);
                    trucks.Add(truckData);
                }
                return trucks;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public TruckData GetTruck(int truckId)
        {
            Truck truck;
            try
            {
                truck = this.db.Trucks.Where(x => x.TruckId == truckId).SingleOrDefault();
                TruckData truckData = DbTruckToTruckData(truck);
                return truckData;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public List<TruckType> GetAllTypes()
        {
            try
            {
                List<LUT_TruckType> dbtruckTypes = this.db.LUT_TruckType.ToList();

                List<TruckType> truckTypes = new List<TruckType>();

                foreach (LUT_TruckType dbtruck in dbtruckTypes)
                {
                    TruckType truckType = new TruckType()
                    {
                        truckTypeId = dbtruck.TruckTypeId,
                        capacity = dbtruck.Capacity,
                        truckTypeDesc = dbtruck.TruckTypeDesc
                    };

                    truckTypes.Add(truckType);
                }

                return truckTypes;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public string GetBinDesc(int binTypeId)
        {
            try
            {

                string desc = this.db.LUT_BinType.Where(x => x.BinTypeId == binTypeId).Select(x => x.BinTypeDesc).SingleOrDefault();
                if(desc != null)
                {
                    return desc;
                }
                else
                {
                    throw new Exception("There is no day with this id.");
                }
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public TruckData AddNewTruck(TruckData newTruck)
        {
            try
            {
                Truck truck = TruckDataToDbTruck(newTruck);

                this.db.Trucks.Add(truck);
                this.db.SaveChanges();
                return newTruck;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public void UpdateTruck(TruckData updatedTruck)
        {
            try
            {
                Truck dbTruck = this.db.Trucks.Where(x => x.TruckId == updatedTruck.truckId).SingleOrDefault();
                if (dbTruck == null)
                {
                    throw new Exception("Truck not found");
                }

                dbTruck.TruckId = updatedTruck.truckId;
                dbTruck.TruckTypeId = updatedTruck.truckTypeId;
                dbTruck.AreaId = updatedTruck.areaId;
                dbTruck.CurrentCapacity = updatedTruck.currentCapacity;

                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public void DeleteTruck(int truckId)
        {
            try
            {
                Truck truck = this.db.Trucks.Where(x => x.TruckId == truckId).SingleOrDefault();
                truck.AreaId = null;
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        // TODO - add TransactionScope to this method (lior).
        public void ClearingBins(List<Bin> binList, int truckId, DateTime currentDateTime)  //Go over all the bins and unloading them.
        {

            try
            {
                TruckData truck = GetTruck(truckId);

                foreach (Bin bin in binList)
                {
                    double transferredCapacity = bin.CurrentCapacity;
                    truck.currentCapacity += transferredCapacity;
                    this.UpdateTruck(truck);

                    bin.CurrentCapacity = 0;

                    using (BinBusinessLogic binBl = new BinBusinessLogic(this.db))
                    {
                        binBl.UpdateBin(bin, currentDateTime);
                    }

                    AddWasteTransferLog(truck.truckId, bin.BinId, transferredCapacity, currentDateTime);

                }
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }

        }

        // TODO - add TransactionScope to this method (lior).
        public void ClearingBin(BinData bin, int truckId, DateTime currentDateTime)  //Clear one bin for the simulator
        {

            try
            {
                TruckData truck = GetTruck(truckId);

                double transferredCapacity = bin.currentCapacity;
                truck.currentCapacity += transferredCapacity;
                this.UpdateTruck(truck);

                bin.currentCapacity = 0;

                using (BinBusinessLogic binBl = new BinBusinessLogic(this.db))
                {
                    binBl.UpdateBin(bin, currentDateTime);
                }

                AddWasteTransferLog(truck.truckId, bin.binId, transferredCapacity, currentDateTime);

            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }

        }

        private void AddWasteTransferLog(int truckId, int binId, double transferredCapacity, DateTime currentDateTime)
        {
            try
            {
                WasteTransferLog wasteTransferLog = new WasteTransferLog()
                {
                    TruckId = truckId,
                    BinId = binId,
                    TransferedCapacity = transferredCapacity,
                    CreatedDate = currentDateTime
                };

                this.db.WasteTransferLogs.Add(wasteTransferLog);

                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public Truck TruckDataToDbTruck(TruckData truckData)
        {
            Truck truck = new Truck()
            {
                TruckId = truckData.truckId,
                TruckTypeId = truckData.truckTypeId,
                CurrentCapacity = truckData.currentCapacity,
                AreaId = truckData.areaId
            };
            return truck;
        }

        public TruckData DbTruckToTruckData(Truck truck)
        {
            List<spTruck_GetTruckListFullDetails_Result> dbTrucks = this.db.spTruck_GetTruckListFullDetails().ToList();
            spTruck_GetTruckListFullDetails_Result currentTruck = dbTrucks.Find(x => x.TruckId == truck.TruckId);
            TruckData truckData = new TruckData()
            {
                truckId = currentTruck.TruckId,
                truckTypeId = currentTruck.TruckTypeId,
                truckTypeDesc = currentTruck.TruckTypeDesc,
                areaId = currentTruck.AreaId,
                areaDesc = currentTruck.AreaDesc,
                currentCapacity = currentTruck.CurrentCapacity,
                maxCapacity = currentTruck.Capacity
            };
            return truckData;
        }

        public struct TruckCleanUp//Need it for TruckAllocationToRegion
        {
            public int typeId { get; set; }
            public int numOfCleanups { get; set; }
        }

        public List<TruckCleanUp> TruckAllocationToRegion(LUT_Area area)// return the smallest trcuk with the lowest number of cleanups
        {
            List<LUT_TruckType> listOfTruckType;
            DateTime lastThreeMonths = DateTime.Now.AddDays(-90); // Three months back
            List<Building> areaBuildings;
            List<Bin> buildingBins = new List<Bin>();
            List<WasteTransferLog> buildingWasteTransferLog;
            double areaAvg, sumOfTrash = 0;

            try
            {
                areaBuildings = this.db.Buildings.Where(x => x.AreaId == area.AreaId).ToList();//List of all the buildings in this area  

                //buildingBins = this.db.Bins.Where(x => areaBuildings.FindIndex(f => f.BuildingId == x.BuildingId) != -1).ToList();//List of all the bins in this area
                List<Bin> allBins = this.db.Bins.ToList(); // linq doesn't like findindex same as above statement
                foreach (Bin bin in allBins)
                {
                    if(areaBuildings.FindIndex(x => x.BuildingId == bin.BuildingId) != -1)
                    {
                        buildingBins.Add(bin);
                    }
                }

                buildingWasteTransferLog = this.db.WasteTransferLogs.Where(x => x.CreatedDate >= lastThreeMonths).ToList(); //allWasteTransferLog from the last three months

                foreach (WasteTransferLog wtl in buildingWasteTransferLog) //doing it because linq problem with:.Where(x => x.CreatedDate >= lastThreeMonths && buildingBins.FindIndex(f => f.BinId == x.BinId) != -1).ToList(); 
                {
                    if (buildingBins.FindIndex(x => x.BinId == wtl.BinId) != -1)
                    {
                        sumOfTrash += wtl.TransferedCapacity;
                    }
                }

                areaAvg = sumOfTrash / buildingWasteTransferLog.Count(); //sumOfTrash / number of cleanup(Log size)

                listOfTruckType = this.db.LUT_TruckType.ToList().OrderBy(x => x.Capacity).ToList();//All truckType order by capacity
                List<TruckCleanUp> truckCleanupList = new List<TruckCleanUp>();
                TruckCleanUp currentTrcuk = new TruckCleanUp();

                foreach (LUT_TruckType ltt in listOfTruckType)//Calculating for each truck its numberofcleanups
                {
                    currentTrcuk.typeId = ltt.TruckTypeId;
                    currentTrcuk.numOfCleanups = (int)Math.Ceiling(areaAvg / ltt.Capacity);//Round up numOfCleanups
                    truckCleanupList.Add(currentTrcuk);
                }

                return truckCleanupList;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public int WorkSchedule() //1 = success 0 = failure
        {

            try
            {
                List<LUT_Area> listOfArea = this.db.LUT_Area.ToList();//List of area
                List<Truck> truckList = this.db.Trucks.ToList(); //List of available trucks
                List<TruckCleanUp> bestFit; //List to store the answer of TruckAllocationToRegion
                List<LUT_TruckType> currentTruckTypes = new List<LUT_TruckType>(); //List of available truckType
                Truck truckToUpdate;
                int indexOfbesttruck;

                foreach (LUT_Area area in listOfArea)//For each area we calculate the best truck and update its areaId
                {
                    bestFit = TruckAllocationToRegion(area);

                    //currentTruckTypes = this.db.LUT_TruckType.Where(x => truckList.FindIndex(f => f.TruckTypeId == x.TruckTypeId) != -1).ToList();//linq doesn't like FindIndex
                    List<LUT_TruckType> truckTypes = this.db.LUT_TruckType.ToList();//Same as the above statement
                    foreach (LUT_TruckType type in truckTypes)
                    {
                        if(truckList.FindIndex(x => x.TruckTypeId == type.TruckTypeId) != -1)
                        {
                            currentTruckTypes.Add(type);
                        }
                    }

                    TruckCleanUp bestTruck = bestFit.First();
                    foreach (TruckCleanUp truck in bestFit)//searching for the smallest truck with the lowest number of cleanups
                    {
                        if (truckList.FindIndex(x => x.TruckTypeId == truck.typeId) != -1) //Checking for the best truck available
                        {
                            if (truck.numOfCleanups < bestTruck.numOfCleanups) //Is available && better
                            {
                                bestTruck = truck;
                            }
                            else if (truckList.FindIndex(x => x.TruckTypeId == bestTruck.typeId) == -1) //If bestfit isn't available 
                            {
                                bestTruck = truck;
                            }
                        }
                    }
                    if (bestTruck.typeId == bestFit.First().typeId && truckList.FindIndex(x => x.TruckTypeId == bestTruck.typeId) == -1)
                    { //If we are here its mean that we still have area to clean but not enough trucks available
                        return 0;
                    }

                    indexOfbesttruck = truckList.FindIndex(x => x.TruckTypeId == bestTruck.typeId); //Getting the index of the truck we want to update
                    truckToUpdate = truckList.ElementAt(indexOfbesttruck);
                    truckList.RemoveAt(indexOfbesttruck); //Removing the best truck from the available list

                    truckToUpdate.AreaId = area.AreaId;

                    TruckData truckData = DbTruckToTruckData(truckToUpdate);
                    this.UpdateTruck(truckData); //Adding the right areaId

                }

                return 1;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }

        }
    }
}


#region old
//using DAL;
//using FND;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BL
//{
//    public class TruckBusinessLogic : BaseBusinessLogic
//    {
//        public TruckBusinessLogic()
//        {
//            this.db = new WasteManagerEntities();
//        }

//        public TruckBusinessLogic(WasteManagerEntities db) : base(db)
//        {

//        }

//        // TRUCK - CRUD 

//        public List<Truck> GetAllTrucks()
//        {
//            try
//            {
//                return this.db.Trucks.ToList();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public Truck GetTruck(int id)
//        {
//            try
//            {
//                return this.db.Trucks.Where(x => x.TruckId == id).SingleOrDefault();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public Truck AddNewTruck(Truck newTruck)
//        {
//            try
//            {
//                this.db.Trucks.Add(newTruck);
//                this.db.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }

//            return newTruck;
//        }

//        public void UpdateTruck(Truck updatedTruck)
//        {
//            try
//            {
//                Truck truck = this.db.Trucks.Where(x => x.TruckId == updatedTruck.TruckId).SingleOrDefault();
//                if (truck != null)
//                {
//                    truck = updatedTruck;
//                }
//                this.db.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public void DeleteTruck(int id)
//        {
//            try
//            {
//                Truck truck = this.db.Trucks.Where(x => x.TruckId == id).SingleOrDefault();
//                this.db.Trucks.Remove(truck);
//                this.db.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public void ClearingBins(List<Bin> binList, int truckId, DateTime currentDateTime)  //Go over all the bins and unloading them.
//        {
//            try
//            {
//                Truck truck = GetTruck(truckId);

//                foreach(Bin bin in binList)
//                {
//                    double transferredCapacity = bin.CurrentCapacity;
//                    truck.CurrentCapacity += transferredCapacity;
//                    this.UpdateTruck(truck);

//                    bin.CurrentCapacity = 0;
//                    using (BinBusinessLogic binBl = new BinBusinessLogic(this.db))
//                    {
//                        binBl.UpdateBin(bin, currentDateTime);
//                    }

//                    AddWasteTransferLog(truck.TruckId, bin.BinId, transferredCapacity, currentDateTime);

//                }
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }

//        }

//        private void AddWasteTransferLog(int truckId, int binId, double transferredCapacity, DateTime currentDateTime)
//        {
//            try
//            {
//                WasteTransferLog wasteTransferLog = new WasteTransferLog()
//                {
//                    TruckId = truckId,
//                    BinId = binId,
//                    TransferedCapacity = transferredCapacity,
//                    CreatedDate = currentDateTime
//                };

//                this.db.WasteTransferLogs.Add(wasteTransferLog);

//                this.db.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }
//    } 
//}
#endregion