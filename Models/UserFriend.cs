using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models
{
    [Table("UserFriends")]
    public class UserFriend
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserFriendId { get; set; }
        public int IdUser { get; set; }
        public int IdUserToFriendList { get; set; }
        public bool RequestAccepted { get; set; }

        [NotMapped]
        public User Friend { get; set; }
        [NotMapped]
        // Označava jeli osoba koja je ulogirana poslala zahtjev za prijateljstvo
        public bool RequestSent { get; set; } = false;
    }
}