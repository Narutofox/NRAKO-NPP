using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models
{
    public class UserProfile
    {
        public int UserId { get; set; }
        [Display(Name = "Ime")]
        public string FirstName { get; set; }
        [Display(Name = "Prezime")]
        public string LastName { get; set; }
        public string Fullname
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string ProfileImagePath { get; set; }
        public bool AllowFollowing { get; set; }
        public bool ShowEmail { get; set; }

        public bool AreFriends { get; set; } = false;
        public bool FriendRequestSend { get; set; } = false;
        public bool IsFollowing { get; set; } = false;
        /// <summary>
        /// Ako je ovo istinitio vlasnika profila je blokirao osobu koja pregleda profil
        /// </summary>
        public bool IsBlocked { get; set; } = false;
        /// <summary>
        /// Ako je ovo istinitio osoba koja pregledava profil je blokirala vlasnika profila
        /// </summary>
        public bool IsBlocking { get; set; } = false;
    }
}