using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underlauncher
{
    public static class ProgressManager
    {
        public static Progress<int> downloadProg = new Progress<int>();
        public static string progressDescription { get; set; }

        private static long _TotalBytes;
        private static long _BytesDownloaded;
        private static long _PreviousRecv;

        public static void UpdateTotalDownloadProgress(long bytesRecv)
        {
            if (_TotalBytes > 0 && bytesRecv >= 0)
            {
                _BytesDownloaded += (bytesRecv - _PreviousRecv);
                _PreviousRecv = bytesRecv;
                double prog = (((double)_BytesDownloaded) / ((double)_TotalBytes)) * 100;
                ((IProgress<int>)downloadProg).Report(Convert.ToInt32(prog));
            }
        }

        public static void NotifyNewFile()
        {
            _PreviousRecv = 0;
        }

        public static void SetTotalBytes(long count)
        {
            Reset();
            _TotalBytes = count;
        }

        public static void Reset()
        {
            _BytesDownloaded = 0;
            _PreviousRecv = 0;
            progressDescription = "";
            ((IProgress<int>)downloadProg).Report(0);
        }

        public static void WriteDebugValues(Logger log)
        {
            log.WriteEntry("Download completed. Total: " + _TotalBytes + ". Downloaded: " + _BytesDownloaded + ". Progress: " + ((((double)_BytesDownloaded) / ((double)_TotalBytes)) * 100).ToString());
        }
    }
}
