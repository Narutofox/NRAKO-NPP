using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace NRAKO_IvanCicek.Helpers
{
    public class Logger
    {
        private static Logger instance;
        private static object syncRoot = new Object();

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                       instance = new Logger();
                    }
                }

                return instance;
            }
        }

        public void Log(string text, string path ="")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ " + DateTime.Now + " ] ");
            sb.Append(System.Environment.NewLine);
            sb.Append(text);
            SaveToFile(sb, path);
        }

        public void LogException(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ " + DateTime.Now + " ] ");
            sb.Append(System.Environment.NewLine);
            sb.Append("InnerException: " + ex.InnerException);
            sb.Append(System.Environment.NewLine);
            sb.Append("Source: " + ex.Source);
            sb.Append(System.Environment.NewLine);
            sb.Append("Message: " + ex.Message);
            sb.Append(System.Environment.NewLine);
            sb.Append("StackTrace: " + ex.StackTrace);
            sb.Append(System.Environment.NewLine);
            SaveToFile(sb);
        }

        private void SaveToFile(StringBuilder sb,string path = "")
        {

            if (!String.IsNullOrEmpty(path))
            {
                if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(path)) == false)
                {
                    using (FileStream fs = File.Create(System.Web.Hosting.HostingEnvironment.MapPath(path)))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(sb.ToString());
                        fs.Write(info, 0, info.Length);
                    };
                }
                else
                {
                    System.IO.File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath(path), sb.ToString());
                }
            }
            else
            {
                if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Logs/ErrorLog.txt")) == false)
                {
                    using (FileStream fs = File.Create(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Logs/ErrorLog.txt")))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(sb.ToString());
                        fs.Write(info, 0, info.Length);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Logs/ErrorLog.txt"), sb.ToString());
                }

            }
        }
    }
}