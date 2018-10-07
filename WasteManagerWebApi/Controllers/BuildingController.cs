using BL.AtomicDataModels;
using BL.BusinessLogic;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WasteManagerWebApi.ViewDataModels;

namespace WasteManagerWebApi.Controllers
{
    public class BuildingController : BaseController
    {
        [HttpGet]
        public EfficientAllocationViewModel GetEfficientAllocationViewModel()
        {
            try
            {
                EfficientAllocationViewModel viewModel = new EfficientAllocationViewModel()
                {
                    buildings = new List<BuildingData>()
                };

                List<BuildingData> buildings;
                BuildingData buildingFull;

                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    buildings = buildingLogic.GetBuildings().ToList();

                    foreach (BuildingData building in buildings)
                    {
                        buildingFull = new BuildingData()
                        {
                            streetName = building.streetName,
                            streetNumber = building.streetNumber,
                            buildingId = building.buildingId,
                            areaId = building.areaId
                        };

                        viewModel.buildings.Add(buildingFull);
                    }
                }

                return viewModel;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }   
        
        [HttpGet]
        public string GetAreaDesc(int areaId)
        {
            try
            {
                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    return buildingLogic.GetAreaDesc(areaId);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public int GetNumberOfDays(int buildingId)
        {
            try
            {
                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    return buildingLogic.GetNumberOfDays(buildingId);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public double GetBinsAreaDisposal(int buildingId)
        {
            try
            {
                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    return buildingLogic.GetBinsAreaDisposal(buildingId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public double GetAvgCapacity(int buildingId)
        {
            try
            {
                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    return buildingLogic.GetAvgCapacity(buildingId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public Suggestion HandleEfficientCapacity(int buildingId)
        {
            try
            {
                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    return buildingLogic.HandleEfficientCapacity(buildingId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public string GetDayDesc(int dayId)
        {
            try
            {
                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    return buildingLogic.GetDayDesc(dayId);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public string GetBinDesc(int binTypeId)
        {
            try
            {
                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    return buildingLogic.GetBinDesc(binTypeId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public bool ImplementSuggestion([FromBody] ImplementSuggestionData implementSuggestionData)
        {
            try
            {
                using (BuildingsLogic buildingLogic = new BuildingsLogic())
                {
                    return buildingLogic.ImplementSuggestion(implementSuggestionData.suggestion, implementSuggestionData.buildingId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}