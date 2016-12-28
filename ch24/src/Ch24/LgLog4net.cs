using System;
using Cmn.Util;
using log4net;

namespace Ch24
{
    class LgLog4net : ILg
    {
        private readonly ILog log;

        public LgLog4net(Type ty)
        {
            log = LogManager.GetLogger(ty);
        }

        public void InfoFormat(string format, params object[] args)
        {
            log.InfoFormat(format,args);
        }

        public void Info(string msg)
        {
            log.Info(msg);
        }
    }
}