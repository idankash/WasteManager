using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using FND;
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Security.Principal;

namespace BL
{
    public abstract class BaseBusinessLogic : IDisposable
    {
        protected WasteManagerEntities db = null;

        private bool isExternalDb;

        public BaseBusinessLogic()
        {
            db = new WasteManagerEntities();
            db.Database.CommandTimeout = 180;
            db.Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            this.isExternalDb = false;
            db.Configuration.LazyLoadingEnabled = true;
            db.Configuration.ProxyCreationEnabled = true;

        }

        public BaseBusinessLogic(WasteManagerEntities db)
        {
            this.db = db;
            this.isExternalDb = true;
        }

        protected void OpenConnection()
        {
            try
            {
                if (this.db.Database.Connection.State == ConnectionState.Closed)
                    this.db.Database.Connection.Open();
            }
            catch (Exception e)
            {
                throw ErrorHandler.Handle(e, this);
            }
        }

        public virtual void Dispose()
        {
            if (!this.isExternalDb && db != null)
            {
                db.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}


///**

//    // in trucklogic:

//    using ( Trucklogic trucklogic = new Trucklogic())       // This is a regular BL - use the 1st ctor
//    {
//        ....
//        ....

//        using (Binlogic binlogic = new BinLogic(this.db))   // This is a nested BL - use the 2nd ctor
//        {
//            ....
//        }

//    }

//*/