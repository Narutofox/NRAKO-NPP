using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models
{
    [Table("UsersFollowings")]
    public class UserFollow
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserFollowId { get; set; }
        public int IdUser { get; set; }
        public int IdUserToFollow { get; set; }
    }
}