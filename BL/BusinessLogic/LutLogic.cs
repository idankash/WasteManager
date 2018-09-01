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

    }
}
