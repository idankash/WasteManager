using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTest
{
    internal class WasteSimulator
    {

        public DateTime DestinationDateTime { get; set; }
        public DateTime SourceDateTime { get; set; }

        Random rand = new Random();


        public WasteSimulator()
        {

        }

        public WasteSimulator(int days)
        {
            DestinationDateTime = DateTime.Now.AddDays(days);
            SourceDateTime = DateTime.Now;
        }
        public void FillAllBinsRandomly()
        {
            while ((Convert.ToBoolean(SourceDateTime.Date.CompareTo(DestinationDateTime.Date))))
            {
                using (BusinessLogic bl = new BusinessLogic())
                {
                    List<Bin> binList = bl.GetAllBins();
                    
                    for (int i = 0; i < 12; i++)
                    {
                        // It means the client want to throw his garbage.. 






                        foreach (Bin bin in binList)
                        {
                            if (rand.Next(0, 2) == 1)
                            {
                                int maxWaste = (int)bl.GetMaxCapacityByBinType(bin.BinTypeId);
                                bin.CurrentCapacity += rand.Next(1, maxWaste / 12);

                                bl.UpdateBin(bin, SourceDateTime);
                            }
                        }

                        SourceDateTime = SourceDateTime.AddHours(2);
                    }

                    Truck truck = bl.GetTruck(1);

                    foreach (Bin bin in binList)
                    {
                        truck.CurrentCapacity += bin.CurrentCapacity;
                        bl.UpdateTruck(truck);

                        bin.CurrentCapacity = 0;
                        bl.UpdateBin(bin, SourceDateTime);
                    }
                }

            }
        }
    }
}


