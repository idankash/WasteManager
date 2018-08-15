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

        // TODO - Change the return type to List<TruckData>   (first, we need to impl' TruckData ofcourse...)
        //        Do it like we did in binlogic.getallbins()  
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

        // TODO - change return type to TruckData
        // TODO - change input parameter from 'id' to 'truckId'
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

        // TODO - change input parameter to TruckData
        //          Inside the body of the method we'll create a new truck of type Truck,
        //          and then assign it's properties - truck.someProp = truckData.someProp
        //          lastly we'll use    this.db.Trucks.Add(truck); and save changes...
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

        // TODO - Same goes here, your input parameter is going to be TruckData because that's what the client knows...
        //  So you'll get the Truck by it's id,
        //  but then you'll assign each field like  'dbTruck.someProp = updatedTruckData.someProp'
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
        

        // TODO - change input parameter from 'id' to 'truckId'
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

        // TODO -   who is calling this method? if it's the simulator only it's fine
        //          otherwise, if the client is also calling it, then it can't pass List<Bin>, perhaps maybe pass List<int> binIds?

        // TODO - add TransactionScope to this method (lior).
        public void ClearingBins(List<Bin> binList, int truckId, DateTime currentDateTime)  //Go over all the bins and unloading them.
        {
            try
            {
                Truck truck = GetTruck(truckId);

                foreach (Bin bin in binList)
                {
                    double transferredCapacity = bin.CurrentCapacity;
                    truck.CurrentCapacity += transferredCapacity;
                    this.UpdateTruck(truck);

                    bin.CurrentCapacity = 0;
                    using (BinBusinessLogic binBl = new BinBusinessLogic(this.db))
                    {
                        binBl.UpdateBin(bin, currentDateTime);
                    }

                    AddWasteTransferLog(truck.TruckId, bin.BinId, transferredCapacity, currentDateTime);

                }
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

        public Suggestion HandleEfficientCapacity(Building building)
        {

            double percentage = 0, sumOfTrash = 0;
            DateTime lastThreeMonths = DateTime.Now.AddDays(-90); // Three months back
            List<Bin> buildingBins;
            List<WasteTransferLog> buildingWasteTransferLog;

            try
            {
                buildingBins = this.db.Bins.Where(x => x.BuildingId == building.BuildingId).ToList(); //All the bins in the building

                buildingWasteTransferLog = this.db.WasteTransferLogs
                                                .Where(x => x.CreatedDate >= lastThreeMonths && buildingBins.FindIndex(f => f.BinId == x.BinId) != -1).ToList(); //buildingWasteTransferLog from the last three months, in the building

                foreach (WasteTransferLog wtl in buildingWasteTransferLog)
                {
                    sumOfTrash += wtl.TransferedCapacity;
                }

                double sumMaxCapacity = this.db.LUT_BinType
                                              .Where(x => buildingBins.FindIndex(f => f.BinTypeId == x.BinTypeId) != -1)
                                              .Select(g => g.Capacity).Sum();

                double fullSumMaxCapacity = sumMaxCapacity * 12 * building.LUT_Weekdays.ToList().Count; //100% full each cleanup(three months = 12 weeks)
                                                                                                        //sumMax*12*number of cleanup each week
                percentage = (sumOfTrash * 100) / fullSumMaxCapacity; //Calculate percentage

                if (percentage >= 80) //More than 80% === add/replace bin
                {
                    if (building.LUT_Weekdays.ToList().Count == 1)
                    {
                        int dayId = building.LUT_Weekdays.Select(x => x.WeekdayId).SingleOrDefault();
                        int addDayId = this.db.LUT_Weekdays.Where(x => x.WeekdayId == (dayId + 3) % 6)
                                                           .Select(x => x.WeekdayId).SingleOrDefault();
                        Suggestion suj = new Suggestion()
                        {
                            suggestionAction = SuggestionAction.add,
                            suggestionEntity = SuggestionEntity.Day,
                            EntityIds = new List<int>()
                                    {
                                        addDayId
                                    }
                        };

                        return suj;
                    }
                    else
                    {
                        double desparea = 0;//BinTrashDisposalArea
                        foreach (Bin bin in buildingBins) //For each bin in the building we add its BinTrashDisposalArea
                        {
                            desparea += (double)(this.db.LUT_BinType
                                                .Where(x => x.BinTypeId == bin.BinTypeId)
                                                .Select(g => g.BinTrashDisposalArea).SingleOrDefault());
                        }

                        double remainingDisposalArea = building.TrashDisposalArea - desparea;

                        List<LUT_BinType> binTypeList = this.db.LUT_BinType.ToList();
                        binTypeList.OrderByDescending(x => x.BinTrashDisposalArea); //Sort bintype by BinTrashDisposalArea(descending)
                        foreach (LUT_BinType binType in binTypeList)
                        {
                            if (binType.BinTrashDisposalArea <= remainingDisposalArea)
                            {
                                Suggestion suj = new Suggestion()
                                {
                                    suggestionAction = SuggestionAction.add,
                                    suggestionEntity = SuggestionEntity.Bin,
                                    EntityIds = new List<int>()
                                    {
                                        binType.BinTypeId
                                    }
                                };
                                return suj; //Retrun the largest bin that fit 
                            }
                        }
                        //if we are here we have to replace bin
                        List<int> binTypeId = this.db.LUT_BinType
                                                       .Where(x => buildingBins.FindIndex(f => f.BinTypeId == x.BinTypeId) != -1)
                                                       .Select(x => x.BinTypeId).ToList();

                        List<List<int>> allCombos = GetAllCombos(binTypeId); //List of all the combinations
                        allCombos.OrderBy(x => x.Count); //Sort by numbers of bin

                        double currentDisposalArea, currentMaxCapacity;
                        List<int> bestCombo = new List<int>();
                        foreach (List<int> combo in allCombos) //Go over allCombos 
                        {
                            currentDisposalArea = 0;
                            currentMaxCapacity = 0;

                            foreach (int bint in combo)
                            {
                                currentMaxCapacity += this.db.LUT_BinType.Where(x => x.BinTypeId == bint) //Calculating currentMaxCapacity
                                                                          .Select(x => x.Capacity).SingleOrDefault();

                                currentDisposalArea += (double)this.db.LUT_BinType.Where(x => x.BinTypeId == bint) //Calculating currentDisposalArea
                                                                          .Select(x => x.BinTrashDisposalArea).SingleOrDefault();

                                bestCombo.Add(bint); //Creating the return string
                            }

                            if (currentDisposalArea <= desparea && currentMaxCapacity > sumMaxCapacity)//Check if this combo is good === combo.disposalArea <= building.disposalArea
                            {                                                                                           // && combo.maxCapacity > previousBuildingCombo.maxCapacity
                                Suggestion suj = new Suggestion()
                                {
                                    suggestionAction = SuggestionAction.add,
                                    suggestionEntity = SuggestionEntity.Bin,
                                    EntityIds = bestCombo
                                };
                                return suj; //Best combo
                            }

                        }
                        return null; //Cann't add (Should not happen)
                    }
                }
                else if (percentage <= 60) //Less than 60% === remove/replace bin
                {
                    if (building.LUT_Weekdays.ToList().Count == 2)
                    {
                        int dayId = building.LUT_Weekdays.LastOrDefault().WeekdayId;
                        Suggestion suj = new Suggestion()
                        {
                            suggestionAction = SuggestionAction.remove,
                            suggestionEntity = SuggestionEntity.Day,
                            EntityIds = new List<int>()
                                    {
                                        dayId
                                    }
                        };
                        return suj;
                    }
                    else //remove the smallest bin
                    {
                        List<LUT_BinType> buildingBinTypeList = this.db.LUT_BinType
                                                                    .Where(x => buildingBins.FindIndex(f => f.BinTypeId == x.BinTypeId) != -1).ToList();

                        buildingBinTypeList.OrderBy(x => x.Capacity);

                        int binId = buildingBinTypeList.First().BinTypeId;

                        Suggestion suj = new Suggestion()
                        {
                            suggestionAction = SuggestionAction.remove,
                            suggestionEntity = SuggestionEntity.Bin,
                            EntityIds = new List<int>()
                                    {
                                        binId
                                    }
                        };

                        return suj;
                    }
                }
                else //(Should not happen)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }

        }

        private List<List<int>> GetAllCombos(List<int> list) //For HandleEfficientCapacity func
        {
            int comboCount = (int)Math.Pow(2, list.Count) - 1;
            List<List<int>> result = new List<List<int>>();
            for (int i = 1; i < comboCount + 1; i++)
            {
                // make each combo here
                result.Add(new List<int>());
                for (int j = 0; j < list.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(list[j]);
                }
            }
            return result;
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
            List<Bin> buildingBins;
            List<WasteTransferLog> buildingWasteTransferLog;
            double areaAvg, sumOfTrash = 0;

            try
            {//ask lior about using the connections instead of line 269
                areaBuildings = this.db.Buildings.Where(x => x.AreaId == area.AreaId).ToList();//List of all the buildings in this area  

                buildingBins = this.db.Bins.Where(x => areaBuildings.FindIndex(f => f.BuildingId == x.BuildingId) != -1).ToList();//List of all the bins in this area

                buildingWasteTransferLog = this.db.WasteTransferLogs
                                                    .Where(x => x.CreatedDate >= lastThreeMonths && buildingBins.FindIndex(f => f.BinId == x.BinId) != -1).ToList(); //buildingWasteTransferLog from the last three months
                                                                                                                                                                     //From this area

                foreach (WasteTransferLog wtl in buildingWasteTransferLog)
                {
                    sumOfTrash += wtl.TransferedCapacity;
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
                List<LUT_TruckType> currentTruckTypes; //List of available truckType
                Truck truckToUpdate;
                int indexOfbesttruck;

                foreach (LUT_Area area in listOfArea)//For each area we calculate the best truck and update its areaId
                {
                    bestFit = TruckAllocationToRegion(area);
                    currentTruckTypes = this.db.LUT_TruckType.Where(x => truckList.FindIndex(f => f.TruckTypeId == x.TruckTypeId) != -1).ToList();

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
                    this.UpdateTruck(truckToUpdate); //Adding the right areaId

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
