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

    }
}
