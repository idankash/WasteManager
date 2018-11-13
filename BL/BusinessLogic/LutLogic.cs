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
    public class LutLogic : BaseBusinessLogic
    {
        public LutLogic()
        {
            this.db = new WasteManagerEntities();
        }

        public LutLogic(WasteManagerEntities db) : base(db)
        {

        }


        public List<LutItem> GetLutArea()
        {
            try
            {

                List<LUT_Area> dbLutAreaList = this.db.LUT_Area.ToList();
                List<LutItem> lutArea = new List<LutItem>();
                foreach (LUT_Area dbLutAreaItem in dbLutAreaList)
                {
                    LutItem lutAreaItem = new LutItem()
                    {
                        id = dbLutAreaItem.AreaId,
                        desc = dbLutAreaItem.AreaDesc
                    };
                    lutArea.Add(lutAreaItem);
                }

                return lutArea;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public int GetAreaIdByBuilding(int buildingId)
        {
            try
            {
                return this.db.Buildings.Where(x => x.BuildingId == buildingId).SingleOrDefault().AreaId;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public int GetNumberOfBuilding(int areaId)
        {
            try
            {
                return this.db.Buildings.Where(x => x.AreaId == areaId).ToList().Count();
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public double GetAreaCurrentCapacity(int areaId)
        {
            try
            {
                List<spBin_GetBinListFullDetails_Result> dbBins = this.db.spBin_GetBinListFullDetails().ToList();
                double capacity = 0;
                foreach (spBin_GetBinListFullDetails_Result dbBin in dbBins)
                {
                    if (dbBin.AreaId == areaId)
                    {
                        capacity += dbBin.CurrentCapacity;
                    }
                }
                return capacity;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
            
        }

        public double GetAreaMaxCapacity(int areaId)
        {
            try
            {
                List<spBin_GetBinListFullDetails_Result> dbBins = this.db.spBin_GetBinListFullDetails().ToList();
                double capacity = 0;
                foreach (spBin_GetBinListFullDetails_Result dbBin in dbBins)
                {
                    if (dbBin.AreaId == areaId)
                    {
                        capacity += dbBin.Capacity;
                    }
                }
                return capacity;
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public int GetAreaTruckId(int areaId)
        {
            try
            {
                Truck truck = this.db.Trucks.Where(x => x.AreaId == areaId).SingleOrDefault();
                if(truck != null)
                {
                    return truck.TruckId;
                }
                else
                {
                    return -1; // no truck in this area
                }
            }
            catch(Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public double GetAreaAvg(LUT_Area area)
        {
            DateTime lastThreeMonths = DateTime.Now.AddDays(-90); // Three months back
            List<Building> areaBuildings;
            List<Bin> buildingBins = new List<Bin>();
            Truck truck = this.db.Trucks.Where(x => x.AreaId == area.AreaId).SingleOrDefault();

            double areaAvg, sumOfTrash = 0;

            if (truck == null)
            {
                throw new Exception("There is no truck with this id.");
            }

            area = this.db.LUT_Area.Where(x => x.AreaId == truck.AreaId).SingleOrDefault();
            if (area == null)
            {
                throw new Exception("There is no truck in this area.");
            }

            areaBuildings = this.db.Buildings.Where(x => x.AreaId == area.AreaId).ToList();//List of all the buildings in this area  
            List<Bin> allBins = this.db.Bins.ToList(); //List of all the bins in this area
            foreach (Bin bin in allBins)
            {
                if (areaBuildings.FindIndex(x => x.BuildingId == bin.BuildingId) != -1)
                {
                    buildingBins.Add(bin);
                }
            }
            List<WasteTransferLog> buildingWasteTransferLog;
            using (TruckBusinessLogic truckBL = new TruckBusinessLogic())
            {
                int id = truckBL.GetTruckIdByAreaId(area.AreaId);
                buildingWasteTransferLog = this.db.WasteTransferLogs.Where(x => x.CreatedDate >= lastThreeMonths && x.TruckId == id).ToList(); //allWasteTransferLog from the last three months
            }
             

            foreach (WasteTransferLog wtl in buildingWasteTransferLog) //doing it because linq problem with:.Where(x => x.CreatedDate >= lastThreeMonths && buildingBins.FindIndex(f => f.BinId == x.BinId) != -1).ToList(); 
            {
                if (buildingBins.FindIndex(x => x.BinId == wtl.BinId) != -1)
                {
                    sumOfTrash += wtl.TransferedCapacity;
                }
            }

            return areaAvg = sumOfTrash / buildingWasteTransferLog.Count(); //sumOfTrash / number of cleanups(Log size)
        }

        public int GetNumOfCleanups(int truckId)
        {
            try
            {
                Truck truck = this.db.Trucks.Where(x => x.TruckId == truckId).SingleOrDefault();
                LUT_Area area;
                double areaAvg;

                if (truck == null)
                {
                    throw new Exception("There is no truck with this id.");
                }

                area = this.db.LUT_Area.Where(x => x.AreaId == truck.AreaId).SingleOrDefault();
                if (area == null)
                {
                    throw new Exception("There is no truck in this area.");
                }

                areaAvg = GetAreaAvg(area);

                return (int)Math.Ceiling(areaAvg / this.db.LUT_TruckType.Where(x => x.TruckTypeId == truck.TruckTypeId).Select(x => x.Capacity).Single()); // number of cleanups
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public int GetNumOfCleanups(int truckId, int areaId)
        {
            try
            {
                if(truckId == -1)
                {
                    return 0;
                }
                Truck truck = this.db.Trucks.Where(x => x.TruckId == truckId).SingleOrDefault();
                LUT_Area area;
                double areaAvg;

                if (truck == null)
                {
                    throw new Exception("There is no truck with this id.");
                }

                area = this.db.LUT_Area.Where(x => x.AreaId == areaId).SingleOrDefault();
                if (area == null)
                {
                    throw new Exception("There is no truck in this area.");
                }

                areaAvg = GetAreaAvg(area);

                return (int)Math.Ceiling(areaAvg / this.db.LUT_TruckType.Where(x => x.TruckTypeId == truck.TruckTypeId).Select(x => x.Capacity).Single()); // number of cleanups
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public AreaData GetTotalStats()
        {
            try
            {
                List<LUT_Area> areas = this.db.LUT_Area.ToList();
                AreaData areaData = new AreaData
                {
                    area = null,
                    numOfBuildings = 0,
                    capacity = 0,
                    maxCapacity = 0,
                    numOfCleanups = 0,
                    truckId = -1
                };

                foreach(LUT_Area area in areas)
                {
                    areaData.numOfBuildings += GetNumberOfBuilding(area.AreaId);
                    areaData.capacity += GetAreaCurrentCapacity(area.AreaId);
                    areaData.maxCapacity += GetAreaMaxCapacity(area.AreaId);

                    int truckId = GetAreaTruckId(area.AreaId);
                    if(truckId > 0)
                    {
                        areaData.numOfCleanups += GetNumOfCleanups(truckId);
                    }
                }

                return areaData;
            }
            catch(Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public List<AreaData> GetAreaData()
        {
            try
            {
                List<AreaData> areasData = new List<AreaData>(); 
                List<LutItem> areas = GetLutArea();
                AreaData areaData;

                foreach (LutItem area in areas)
                {
                    areaData = new AreaData
                    {
                        area = area,
                        numOfBuildings = GetNumberOfBuilding(area.id),
                        capacity = GetAreaCurrentCapacity(area.id),
                        maxCapacity = GetAreaMaxCapacity(area.id),
                        truckId = GetAreaTruckId(area.id)
                    };

                    areaData.numOfCleanups = GetNumOfCleanups(areaData.truckId);

                    areasData.Add(areaData);
                }
                return areasData;
            }
            catch(Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }
    }
}
