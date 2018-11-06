using BL.AtomicDataModels;
using DAL;
using FND;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BusinessLogic
{
    public class BuildingsLogic : BaseBusinessLogic
    {
        public BuildingsLogic()
        {
            this.db = new WasteManagerEntities();
        }

        public BuildingsLogic(WasteManagerEntities db) : base(db)
        {

        }

        public List<BuildingData> GetBuildings()
        {
            try
            {
                List<Building> buildings = this.db.Buildings.ToList();
                List<BuildingData> buildingsData = new List<BuildingData>();

                foreach(Building buidling in buildings)
                {
                    BuildingData buildingData = new BuildingData()
                    {
                        streetName = buidling.StreetName,
                        streetNumber = buidling.StreetNumber,
                        buildingId = buidling.BuildingId,
                        areaId = buidling.AreaId
                    };
                    buildingsData.Add(buildingData);
                }
                return buildingsData;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public int GetNumberOfDays(int buildingId)
        {
            try
            {
                Building building = this.db.Buildings.Where(x => x.BuildingId == buildingId).SingleOrDefault();
                if(building != null)
                {
                    return building.LUT_Weekdays.Count();
                }
                else
                {
                    throw new Exception("There is no building with this id.");
                }
            }
            catch(Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public List<string> GetDaysOfCleanups(int? buildingId)
        {
            try
            {
                Building building = this.db.Buildings.Where(x => x.BuildingId == buildingId).SingleOrDefault();
                if (building != null)
                {

                    return building.LUT_Weekdays.Select(x => x.WeekdayDesc).ToList();
                }
                else
                {
                    throw new Exception("There is no building with this id.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double GetBinsAreaDisposal(int buildingId)
        {
            try
            {
                double sum = 0;
                List<BinData> binsData;

                using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic(this.db))
                {
                    binsData = binBusinessLogic.GetBinsByBuilding(buildingId);
                }

                Building building = this.db.Buildings.Where(x => x.BuildingId == buildingId).SingleOrDefault();
                if(building != null)
                {
                   foreach(BinData bindata in binsData)
                    {
                        sum += bindata.binTrashDisposalArea;
                    }
                    return sum;
                }
                else
                {
                    throw new Exception("There is no building with this id.");
                }
            }
            catch(Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public double GetAvgCapacity(int buildingId) 
        {
            try
            {
                double sumOfTrash = 0;
                //!!!
                DateTime lastThreeMonths = this.db.BinLogs.OrderByDescending(x => x.UpdateDate).ToList().First().UpdateDate.AddDays(-90); //last three month from the last day the simulator run
                List<spBin_GetBinListFullDetails_Result> buildingBins;
                List<WasteTransferLog> allgWasteTransferLog;

                Building building = this.db.Buildings.Where(x => x.BuildingId == buildingId).SingleOrDefault();

                if (building != null)
                {
                    buildingBins = this.db.spBin_GetBinListFullDetails().Where(x => x.BuildingId == building.BuildingId).ToList(); //All the bins in the building

                    allgWasteTransferLog = this.db.WasteTransferLogs.Where(x => x.CreatedDate >= lastThreeMonths).ToList(); //allWasteTransferLog from the last three months

                    foreach (WasteTransferLog wtl in allgWasteTransferLog) //!!!doing it because linq problem with:.Where(x => x.CreatedDate >= lastThreeMonths && buildingBins.FindIndex(f => f.BinId == x.BinId) != -1).ToList(); 
                    {
                       if(buildingBins.FindIndex(x => x.BinId == wtl.BinId) != -1)
                        {
                            sumOfTrash += wtl.TransferedCapacity;
                        }
                    }

                    double sumMaxCapacity = 0;
                    foreach(spBin_GetBinListFullDetails_Result buildingBin in buildingBins)
                    {
                        sumMaxCapacity += buildingBin.Capacity;
                    }
                    double fullSumMaxCapacity = sumMaxCapacity * 12 * building.LUT_Weekdays.ToList().Count; //100% full each cleanup(three months = 12 weeks)
                                                                                                            //sumMax*12*number of cleanup each week
                    return ((sumOfTrash * 100) / fullSumMaxCapacity); //Calculate percentage
                }
                else
                {
                    throw new Exception("There is no building with this id.");
                }
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public string GetAreaDesc(int areaId)
        {
            try
            {
                LUT_Area area = this.db.LUT_Area.Where(x => x.AreaId == areaId).SingleOrDefault();

                if(area != null)
                {
                    return area.AreaDesc;
                }
                else
                {
                    throw new Exception("There is no area with this id.");
                }
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public string GetDayDesc(int dayId)
        {
            try
            {
                LUT_Weekdays weekday = this.db.LUT_Weekdays.Where(x => x.WeekdayId == dayId).SingleOrDefault();

                if (weekday != null)
                {
                    return weekday.WeekdayDesc;
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

        public string GetBinDesc(int binTypeId)
        {
            try
            {
                using (TruckBusinessLogic truckBusinessLogic = new TruckBusinessLogic(this.db))
                {
                    return truckBusinessLogic.GetBinDesc(binTypeId);
                }
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public Suggestion HandleEfficientCapacity(int buildingId)
        {

            Building building;
            double percentage = 0, sumOfTrash = 0;
            DateTime lastThreeMonths = this.db.BinLogs.OrderByDescending(x => x.UpdateDate).ToList().First().UpdateDate.AddDays(-90); //last three month from the last day the simulator run
            List<spBin_GetBinListFullDetails_Result> buildingBins;
            List<WasteTransferLog> allgWasteTransferLog;

            try
            {
                building = this.db.Buildings.Where(x => x.BuildingId == buildingId).SingleOrDefault();
                if(building == null)
                {
                    throw new Exception("There is no building with this id.");
                }
                buildingBins = this.db.spBin_GetBinListFullDetails().Where(x => x.BuildingId == building.BuildingId).ToList(); //All the bins in the building

                allgWasteTransferLog = this.db.WasteTransferLogs.Where(x => x.CreatedDate >= lastThreeMonths).ToList(); //allWasteTransferLog from the last three months

                foreach (WasteTransferLog wtl in allgWasteTransferLog) //doing it because linq problem with:.Where(x => x.CreatedDate >= lastThreeMonths && buildingBins.FindIndex(f => f.BinId == x.BinId) != -1).ToList(); 
                {
                    if (buildingBins.FindIndex(x => x.BinId == wtl.BinId) != -1)
                    {
                        sumOfTrash += wtl.TransferedCapacity;
                    }
                }

                double sumMaxCapacity = 0;
                foreach (spBin_GetBinListFullDetails_Result buildingBin in buildingBins)
                {
                    sumMaxCapacity += buildingBin.Capacity;
                }

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
                            entityIds = new List<int>()
                                    {
                                        addDayId
                                    }
                        };

                        return suj;
                    }
                    else
                    {
                        double desparea = 0;//BinTrashDisposalArea
                        foreach (spBin_GetBinListFullDetails_Result bin in buildingBins) //For each bin in the building we add its BinTrashDisposalArea
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
                                    entityIds = new List<int>()
                                    {
                                        binType.BinTypeId
                                    }
                                };
                                return suj; //Retrun the largest bin that fit 
                            }
                        }
                        //if we are here we have to replace bin
                        //List<int> binTypeId = this.db.LUT_BinType //linq doesn't like FindIndex
                        //                             .Where(x => buildingBins.FindIndex(f => f.BinTypeId == x.BinTypeId) != -1)
                        //                             .Select(x => x.BinTypeId).ToList();

                        List<LUT_BinType> binTypes = this.db.LUT_BinType.ToList(); //Same as the above statement
                        List<int> binTypeId = new List<int>();
                        foreach(LUT_BinType binType in binTypes)
                        {
                            if(buildingBins.FindIndex(x => x.BinTypeId == binType.BinTypeId) != -1)
                            {
                                binTypeId.Add(binType.BinTypeId);
                            }
                        }

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
                                    suggestionAction = SuggestionAction.change,
                                    suggestionEntity = SuggestionEntity.Bin,
                                    entityIds = bestCombo
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
                            entityIds = new List<int>()
                                    {
                                        dayId
                                    }
                        };
                        return suj;
                    }
                    else //remove the smallest bin
                    {
                        // List<LUT_BinType> buildingBinTypeList = this.db.LUT_BinType //linq doesn't like FindIndex
                        //                                             .Where(x => buildingBins.FindIndex(f => f.BinTypeId == x.BinTypeId) != -1).ToList(); 


                        List<LUT_BinType> buildingBinTypeList = new List<LUT_BinType>(); //Same as the above statement
                        List<LUT_BinType> allbuildingBinTypeList = this.db.LUT_BinType.ToList(); //Same as the above statement

                        foreach (LUT_BinType binType in allbuildingBinTypeList)
                        {
                            if(buildingBins.FindIndex(x => x.BinTypeId == binType.BinTypeId) != -1)
                            {
                                buildingBinTypeList.Add(binType);
                            }
                        }

                        buildingBinTypeList.OrderBy(x => x.Capacity);

                        if(buildingBinTypeList.Count() > 1)
                        {
                            int binId = buildingBinTypeList.First().BinTypeId;

                            Suggestion suj = new Suggestion()
                            {
                                suggestionAction = SuggestionAction.remove,
                                suggestionEntity = SuggestionEntity.Bin,
                                entityIds = new List<int>()
                                    {
                                        binId
                                    }
                            };
                            return suj;
                        }
                        else
                        {
                            Suggestion suj = new Suggestion()
                            {
                                suggestionAction = SuggestionAction.nothing,
                            };
                            return suj;
                        }

                    }
                }
                else 
                {
                    Suggestion suj = new Suggestion()
                    {
                        suggestionAction = SuggestionAction.nothing,
                    };
                    return suj;
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

        public bool ImplementSuggestion(Suggestion suggestion, int buildingId)
        {
            try
            {
                if ((int)suggestion.suggestionAction == 0 && (int)suggestion.suggestionEntity == 1)// add day
                {
                    AddCleanupDay(suggestion.entityIds[0], buildingId);
                    return true;
                }
                else if ((int)suggestion.suggestionAction == 0 && (int)suggestion.suggestionEntity == 0)// add bin
                {
                    AddBin(suggestion.entityIds[0], buildingId);
                    return true;
                }
                else if ((int)suggestion.suggestionAction == 1 && (int)suggestion.suggestionEntity == 1)// remove day
                {
                    RemoveCleanupDay(suggestion.entityIds[0], buildingId);
                    return true;
                }
                else if ((int)suggestion.suggestionAction == 1 && (int)suggestion.suggestionEntity == 0)// remove bin
                {
                    RemoveBin(suggestion.entityIds[0], buildingId);
                    return true;
                }
                else if ((int)suggestion.suggestionAction == 2 && (int)suggestion.suggestionEntity == 0)// change bins
                {
                    using(BinBusinessLogic binBusinessLogic = new BinBusinessLogic(this.db))
                    {
                        List<BinData> bins = binBusinessLogic.GetBinsByBuilding(buildingId);
                        foreach(BinData bin in bins) // removin all bins from the building
                        {
                            RemoveBin(bin.binTypeId, buildingId);
                        }

                        foreach(int typeId in suggestion.entityIds)// adding all bins to the building
                        {
                            AddBin(typeId, buildingId);
                        }
                        return true;
                    }
                
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public void AddCleanupDay(int dayId, int buildingId)
        {
            try
            {
                LUT_Weekdays weekday = this.db.LUT_Weekdays.Where(x => x.WeekdayId == dayId).SingleOrDefault();
                Building building = this.db.Buildings.Where(x => x.BuildingId == buildingId).SingleOrDefault();
                if (building == null)
                {
                    throw new Exception("There is no building with this id.");
                }
                if (weekday == null)
                {
                    throw new Exception("There is no day with this id.");
                }

                building.LUT_Weekdays.Add(weekday);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public void RemoveCleanupDay(int dayId, int buildingId) //!!! 'Unable to update the EntitySet 'BuildingWasteTransferWeekdays' because it has a DefiningQuery and no <DeleteFunction> element exists in the <ModificationFunctionMapping> element to support the current operation.'
        {
            try
            {
                Building building = this.db.Buildings.Where(x => x.BuildingId == buildingId).SingleOrDefault();
                LUT_Weekdays weekday = building.LUT_Weekdays.Where(x => x.WeekdayId == dayId).SingleOrDefault();
                if (building == null)
                {
                    throw new Exception("There is no building with this id.");
                }
                if (weekday == null)
                {
                    throw new Exception("There is no day with this id.");
                }
                building.LUT_Weekdays.Remove(weekday);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public void AddBin(int binTypeId, int buildingId)// Asign old bin, if no exist Creating new and asign it to the building
        {
            try
            {
                List<Bin> bins = this.db.Bins.Where(x => x.BuildingId == null).ToList();
                Bin currentBin = bins.Find(x => x.BinTypeId == binTypeId);
                if (currentBin != null)
                {
                    using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic(this.db))
                    {
                        currentBin.BuildingId = buildingId;
                        currentBin.CurrentCapacity = 0;
                        binBusinessLogic.UpdateBin(currentBin, DateTime.Now);
                    }
                }
                else
                {
                    BinData newBin = new BinData();
                    Building building = this.db.Buildings.Where(x => x.BuildingId == buildingId).SingleOrDefault();
                    if (building != null)
                    {
                        newBin.cityAddress = building.LUT_Area.AreaDesc;
                        newBin.streetAddress = building.StreetName + " " + building.StreetNumber;
                    }
                    else
                    {
                        throw new Exception("There is no building with this id.");
                    }

                    using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic(this.db))
                    {

                        newBin.binTypeId = binTypeId;
                        newBin.buildingId = buildingId;
                        newBin.binTypeDesc = GetBinDesc(binTypeId);
                        newBin.currentCapacity = 0;
                        newBin.maxCapacity = binBusinessLogic.GetMaxCapacityByBinType(binTypeId);
                        newBin.binTrashDisposalArea = binBusinessLogic.GetbinTrashDisposalArea(binTypeId);

                        binBusinessLogic.AddNewBin(newBin);
                    }
                }

            }
            catch(Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public void RemoveBin(int binTypeId, int buildingId)
        {
            try
            {
                Bin bin = this.db.Bins.Where(x => x.BuildingId == buildingId && x.BinTypeId == binTypeId).FirstOrDefault();
                if (bin != null)
                {
                    using (BinBusinessLogic binBusinessLogic = new BinBusinessLogic(this.db))
                    {
                        binBusinessLogic.DeleteBin(bin.BinId);
                    }
                }
                else
                {
                    throw new Exception("There is no building with this id.");
                }
            }
            catch(Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }
    }
}
