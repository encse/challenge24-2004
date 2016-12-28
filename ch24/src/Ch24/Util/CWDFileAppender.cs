using System.IO;
using log4net.Appender;

namespace Ch24.Util
{
    public class CWDFileAppender : FileAppender
    {
        public override string File
        {
            set
            {
                base.File = Path.Combine(Directory.GetCurrentDirectory(), value);
            }
        }
    }
}
