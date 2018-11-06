using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL;
using BL.AtomicDataModels;
using BL.BusinessLogic;
using DAL;
using FND;

namespace WasteSimulator
{
    internal class WasteSimulator
    {

        public DateTime DestinationDateTime { get; set; }
        public DateTime SourceDateTime { get; set; }

        Random rand = new Random(Guid.NewGuid().GetHashCode());


        public WasteSimulator()
        {

        }

        public WasteSimulator(int days)
        {
            DestinationDateTime = DateTime.Now.AddDays(days);
            SourceDateTime = DateTime.Now;
        }

        public List<Bin> test()
        {
            try
            {
                using (BinBusinessLogic bl = new BinBusinessLogic())
                {
                    //List<Bin> bins = bl.GetAllBins(); error
                    RemoveExistingRecordsForOverride();
                    FillAllBinsRandomly();
                    //return bins;
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        private void RemoveExistingRecordsForOverride()
        {
            try
            {
                DateTime now = DateTime.Now;
                using (BinBusinessLogic bl = new BinBusinessLogic())
                {
                    bl.DeleteBinLogs(now);
                    bl.DeleteWasteTransferLogs(now);
                }
            }
            catch (Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }

        public class BinWithDays//Need it for TruckAllocationToRegion
        {
            public BinData bin { get; set; }
            public List<string> daysList { get; set; }// List of cleanup days
        }

        public void FillAllBinsRandomly()
        {
            try
            {
                List<BinWithDays> binWithDays = new List<BinWithDays>();

                using (BinBusinessLogic bl = new BinBusinessLogic())
                {
                    List<BinData> binsData = bl.GetAllBins();
                    binsData.RemoveAll(x => x.buildingId == null); // Not in used

                    foreach(BinData bin in binsData)
                    {
                        binWithDays.Add(new BinWithDays
                        {
                            bin = bin, 
                            daysList = null
                        });
                    }
                }

                using (BuildingsLogic bl = new BuildingsLogic()) // Add for each bin its cleanup days 
                {
                    foreach(BinWithDays bin in binWithDays)
                    {
                        bin.daysList = bl.GetDaysOfCleanups(bin.bin.buildingId);
                    }
                }


                while ((Convert.ToBoolean(SourceDateTime.Date.CompareTo(DestinationDateTime.Date)))) // Run until you reach DestinationDateTime  
                { //Do stuff each day
                    Logger.Instance.WriteInfo("Current date " + SourceDateTime.ToString(), this);
                    using (TruckBusinessLogic truckBl = new TruckBusinessLogic()) //Cleaning bins
                    {
                        foreach (BinWithDays bin in binWithDays)
                        {
                            if (bin.daysList.Contains(SourceDateTime.DayOfWeek.ToString())) // Check if its the right day for cleanup
                            {
                                truckBl.ClearingBin(bin.bin, 1, SourceDateTime);
                            }
                        }
                    }

                    using (BinBusinessLogic bl = new BinBusinessLogic()) //Filling bins
                    {
                        for(int i = 0; i < 12; i++) //24 hours a day / 2 = 12
                        {
                            foreach (BinWithDays bin in binWithDays)
                            {
                                if (rand.Next(1, 10) >3 ) //Throw garbage or not
                                {
                                    bin.bin.currentCapacity += rand.Next(1, (int)(bin.bin.maxCapacity / (12*(7/bin.daysList.Count())))); //maxCapacity/(12*(7/numOfCleanups))

                                    bl.UpdateBin(bin.bin, SourceDateTime);
                                }
                            }
                            SourceDateTime = SourceDateTime.AddHours(2); //2 hours jump
                        }
                    }
                    
                }
            }
            catch(Exception ex)
            {
                throw ErrorHandler.Handle(ex, this);
            }
        }
    }
}


#region old
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BL;
//using DAL;
//using FND;

//namespace WasteSimulator
//{
//    internal class WasteSimulator
//    {

//        public DateTime DestinationDateTime { get; set; }
//        public DateTime SourceDateTime { get; set; }

//        Random rand = new Random();


//        public WasteSimulator()
//        {

//        }

//        public WasteSimulator(int days)
//        {
//            DestinationDateTime = DateTime.Now.AddDays(days);
//            SourceDateTime = DateTime.Now;
//        }

//        public List<Bin> test()
//        {
//            try
//            {
//                using(BinBusinessLogic bl = new BinBusinessLogic())
//                {
//                    List<Bin> bins = bl.GetAllBins();
//                    FillAllBinsRandomly();
//                    return bins;
//                }
//            }
//            catch(Exception ex)
//            {
//                throw ErrorHandler.Handle(ex, this);
//            }
//        }

//        public void FillAllBinsRandomly()
//        {
//            List<Bin> binList;

//            while ((Convert.ToBoolean(SourceDateTime.Date.CompareTo(DestinationDateTime.Date))))
//            {
//                Logger.Instance.WriteInfo("Current date " + SourceDateTime.ToString(), this);
//                using (BinBusinessLogic bl = new BinBusinessLogic())
//                {
//                    binList = bl.GetAllBins();

//                    for (int i = 0; i < 12; i++)
//                    {
//                        // It means the client want to throw his garbage.. 

//                        foreach (Bin bin in binList)
//                        {
//                            if (rand.Next(0, 2) == 1)
//                            {
//                                int maxWaste = (int)bl.GetMaxCapacityByBinType(bin.BinTypeId);
//                                bin.CurrentCapacity += rand.Next(1, maxWaste / 12);

//                                bl.UpdateBin(bin, SourceDateTime);
//                            }
//                        }

//                        SourceDateTime = SourceDateTime.AddHours(2);
//                    }
//                }
//                using(TruckBusinessLogic truckBl = new TruckBusinessLogic())
//                {
//                    truckBl.ClearingBins(binList, 1, SourceDateTime);
//                }
//            }
//        }
//    }
//}


#endregion
