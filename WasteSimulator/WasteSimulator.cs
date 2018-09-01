using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL;
using DAL;
using FND;

namespace WasteSimulator
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

        public List<Bin> test()
        {
            try
            {
                using (BinBusinessLogic bl = new BinBusinessLogic())
                {
                    //List<Bin> bins = bl.GetAllBins(); error
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

        public void FillAllBinsRandomly()
        {
            List<Bin> binList;

            while ((Convert.ToBoolean(SourceDateTime.Date.CompareTo(DestinationDateTime.Date))))
            {
                Logger.Instance.WriteInfo("Current date " + SourceDateTime.ToString(), this);
                using (BinBusinessLogic bl = new BinBusinessLogic())
                {
                    binList = null;//bl.GetAllBins(); error

                    for (int i = 0; i < 12; i++)
                    {
                        // It means the client want to throw his garbage.. 

                        foreach (Bin bin in binList)
                        {
                            if (rand.Next(0, 2) == 1)
                            {
                                int maxWaste = (int)bl.GetMaxCapacityByBinType(bin.BinTypeId);
                                bin.CurrentCapacity += rand.Next(1, maxWaste / 12);

                                bl.UpdateBin(bl.DbBinToBinData(bin), SourceDateTime);
                            }
                        }

                        SourceDateTime = SourceDateTime.AddHours(2);
                    }
                }
                using (TruckBusinessLogic truckBl = new TruckBusinessLogic())
                {
                    truckBl.ClearingBins(binList, 1, SourceDateTime);
                }
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
