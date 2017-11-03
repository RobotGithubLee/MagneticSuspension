using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MagneticSuspension
{
    class LogClass
    {
        static private string logpathlog = AppDomain.CurrentDomain.BaseDirectory + "Log\\log.txt";

        static public void writelog(string classname, string logText)
        {
            string path = logpathlog;
            if (!File.Exists(path)) 
            {
                // Create a file to write to.
                using (File.Create(path)) { }
            }

            FileInfo fileinfo = new FileInfo(path);
            if (fileinfo.Length > 1024 * 1024 * 2)
            {
                File.Move(path, AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMddHHmmss") + "log.txt");

                if (!File.Exists(path))
                {
                    using (File.Create(path)) { }
                }

            }

            using (StreamWriter sw = File.AppendText(path))
            {
               // sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + classname+" "+logText + "\t\n");
                sw.WriteLine(logText + "\t\n");
                sw.Close();
            }

        }
    }
}
