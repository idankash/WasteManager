using DAL;
using FND;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasteSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            using (BusinessLogic bl = new BusinessLogic())
            {
                
                List<Bin> binList = bl.GetAllBins();

                foreach (Bin bin in binList)
                {
                    Console.WriteLine($"Trash Id=  { bin.BinId}  Address= { bin.CityAddress}  { bin.StreetAddress}   { bin.StreetNumber} ");
                }
                Console.WriteLine("================");


                Bin singleBin = bl.GetBin(1);
                Console.WriteLine("Id = {0}, Old Capacity = {1}", singleBin.BinId, singleBin.CurrentCapacity);
                singleBin.CurrentCapacity += 20;
                bl.UpdateBin(singleBin,DateTime.Now);
                Console.WriteLine("Id = {0}, New Capacity = {1}", singleBin.BinId, singleBin.CurrentCapacity);

     
            }
            */

            Logger.Instance.WriteInfo("before call sql server", null);

            WasteSimulator ws = new WasteSimulator(30);

            //ws.FillAllBinsRandomly();
            List<Bin> bins = ws.test();


            Logger.Instance.WriteInfo("after call sql server", null);

            foreach (Bin bin in bins)
            {
                Console.WriteLine(bin.BinId);
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

//namespace WasteSimulator
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            /*
//            using (BusinessLogic bl = new BusinessLogic())
//            {

//                List<Bin> binList = bl.GetAllBins();

//                foreach (Bin bin in binList)
//                {
//                    Console.WriteLine($"Trash Id=  { bin.BinId}  Address= { bin.CityAddress}  { bin.StreetAddress}   { bin.StreetNumber} ");
//                }
//                Console.WriteLine("================");


//                Bin singleBin = bl.GetBin(1);
//                Console.WriteLine("Id = {0}, Old Capacity = {1}", singleBin.BinId, singleBin.CurrentCapacity);
//                singleBin.CurrentCapacity += 20;
//                bl.UpdateBin(singleBin,DateTime.Now);
//                Console.WriteLine("Id = {0}, New Capacity = {1}", singleBin.BinId, singleBin.CurrentCapacity);


//            }
//            */

//            Logger.Instance.WriteInfo("before call sql server",null);

//            WasteSimulator ws = new WasteSimulator(30);

//            //ws.FillAllBinsRandomly();
//            List<Bin> bins = ws.test();


//            Logger.Instance.WriteInfo("after call sql server", null);

//            foreach (Bin bin in bins)
//            {
//                Console.WriteLine(bin.BinId);
//            }


//        }
//    }
//} 
#endregion
