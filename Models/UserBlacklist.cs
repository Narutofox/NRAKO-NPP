using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models
{
    [Table("UserBlacklists")]
    public class UserBlacklist
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserBlackListId { get; set; }
        public int IdUser { get; set; }
        public int IdUserToBlackList { get; set; }
    }
}