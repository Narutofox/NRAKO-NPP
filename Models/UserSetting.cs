using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models
{
    [Table("UserSettings")] 
    public class UserSetting
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserSettingId { get; set; }
        public int IdUser { get; set; }
        public bool AllowFollowing { get; set; }
        public bool ShowEmail { get; set; }
    }
}