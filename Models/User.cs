using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models
{
    [Table("Users")]
    public class User
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Display(Name = "Ime")]
        public string FirstName { get; set; }
        [Display(Name = "Prezime")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail adresa nije ispravna")]
        public string Email { get; set; }
       
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }

        public string Salt { get; set; }

        public int RecordStatusId { get; set; }
        public int UserTypeId { get; set; }
        public string ProfileImagePath { get; set; }

        [NotMapped]
        public string Fullname
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}