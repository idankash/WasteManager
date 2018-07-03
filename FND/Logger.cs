using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FND
{
    public class Logger
    {
        private static Logger instance = new Logger();

        public static Logger Instance
        {
            get
            {
                return instance;
            }
        }
        private Logger()
        {
            XmlConfigurator.Configure();
        }

        public void SerializeDebug(object obj, object sender)
        {
            ILog logger = LogManager.GetLogger(sender.GetType());
            if (logger.IsDebugEnabled)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                StringWriter str = new StringWriter();
                serializer.Serialize(str, obj);
                logger.Debug(str.ToString());
            }
        }

        public void WriteDebug(string message, object sender)
        {
            // ILog logger = LogManager.GetLogger(sender.GetType());
            ILog logger = LogManager.GetLogger("Main");
            logger.Debug(message);
        }

        public void WriteDebug(Exception e, object sender)
        {
            //ILog logger = LogManager.GetLogger(sender.GetType());
            ILog logger = LogManager.GetLogger("Main");
            logger.Debug(e);
        }

        public void WriteError(Exception e, object sender)
        {

            //ILog logger = LogManager.GetLogger(sender.GetType());
            ILog logger = LogManager.GetLogger("Main");
            logger.Error(e);
        }

        public void WriteError(string message, object sender)
        {
            //ILog logger = LogManager.GetLogger(sender.GetType());
            ILog logger = LogManager.GetLogger("Main");
            logger.Error(message);
        }


        public void WriteWarning(Exception e, object sender)
        {
            // ILog logger = LogManager.GetLogger(sender.GetType());
            ILog logger = LogManager.GetLogger("Main");
            logger.Warn(e);
        }

        public void WriteWarning(string message, object sender)
        {
            // ILog logger = LogManager.GetLogger(sender.GetType());
            ILog logger = LogManager.GetLogger("Main");
            logger.Warn(message);
        }

        public void WriteInfo(Exception e, object sender)
        {
            // ILog logger = LogManager.GetLogger(sender.GetType());
            ILog logger = LogManager.GetLogger("Main");
            logger.Info(e);
        }

        public void WriteInfo(string message, object sender)
        {

            //   ILog logger = LogManager.GetLogger(sender.GetType());
            ILog logger = LogManager.GetLogger("Main");
            logger.Info(message);
        }



    }
}
