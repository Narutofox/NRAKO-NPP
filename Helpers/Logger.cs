using System;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace NRAKO_IvanCicek.Helpers
{
    public class Logger
    {
        private static Logger _instance;
        private static object syncRoot = new Object();

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                       _instance = new Logger();
                    }
                }

                return _instance;
            }
        }

        public void Log(string text, string path ="")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ " + DateTime.Now + " ] ");
            sb.Append(Environment.NewLine);
            sb.Append(text);
            SaveToFile(sb, path);
        }

        public void LogException(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ " + DateTime.Now + " ] ");
            sb.Append(Environment.NewLine);
            sb.Append("InnerException: " + ex.InnerException);
            sb.Append(Environment.NewLine);
            sb.Append("Source: " + ex.Source);
            sb.Append(Environment.NewLine);
            sb.Append("Message: " + ex.Message);
            sb.Append(Environment.NewLine);
            sb.Append("StackTrace: " + ex.StackTrace);
            sb.Append(Environment.NewLine);
            SaveToFile(sb);
        }

        private void SaveToFile(StringBuilder sb,string path = "")
        {

            if (!String.IsNullOrEmpty(path))
            {
                if (File.Exists(HostingEnvironment.MapPath(path)) == false)
                {
                    using (FileStream fs = File.Create(HostingEnvironment.MapPath(path) ?? throw new InvalidOperationException()))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(sb.ToString());
                        fs.Write(info, 0, info.Length);
                    }
                }
                else
                {
                    File.AppendAllText(HostingEnvironment.MapPath(path) ?? throw new InvalidOperationException(), sb.ToString());
                }
            }
            else
            {
                if (File.Exists(HostingEnvironment.MapPath("~/Content/Logs/ErrorLog.txt")) == false)
                {
                    using (FileStream fs = File.Create(HostingEnvironment.MapPath("~/Content/Logs/ErrorLog.txt") ?? throw new InvalidOperationException()))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(sb.ToString());
                        fs.Write(info, 0, info.Length);
                    }
                }
                else
                {
                    File.AppendAllText(HostingEnvironment.MapPath("~/Content/Logs/ErrorLog.txt") ?? throw new InvalidOperationException(), sb.ToString());
                }

            }
        }
    }
}