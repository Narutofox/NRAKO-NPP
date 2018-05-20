using NRAKO_IvanCicek.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models
{
    public class Notification
    {
        public NotificationType Type { get; set; }
        public string Text { get; set; }

        public string Class
        {
            get
            {
                switch (Type)
                {
                    case NotificationType.Error:
                        return "alert alert-danger";
                    case NotificationType.Warning:
                        return "alert alert-warning";
                    case NotificationType.Info:
                        return "alert alert-info";
                    case NotificationType.Success:
                        return "alert alert-success";
                    default:
                        return "";
                }                
            }
        }
    }
}