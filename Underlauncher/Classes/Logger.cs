using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underlauncher
{
    public class Logger
    {
        private string _LogFile;

        public Logger(string logFile)
        {
            _LogFile = logFile;
        }

        public void WriteEntry(string message)
        {
            if (!File.Exists(_LogFile))
            {
                File.Create(_LogFile).Close();
            }

            try
            {
                System.IO.StreamWriter debugFile = new System.IO.StreamWriter(_LogFile, true);
                debugFile.WriteLine("[" + DateTime.Now + "]: " + message);
                debugFile.Close();
            }

            catch { }
        }
    }
}
