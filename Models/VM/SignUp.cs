using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models.VM
{
    public class SignUp
    {
        [Required(ErrorMessage = "Polje {0} je obavezno")]
        [Display(Name = "Ime")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Polje {0} je obavezno")]
        [Display(Name = "Prezime")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Polje {0} je obavezno")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail adresa nije ispravna")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Polje {0} je obavezno")]
        [StringLength(100, ErrorMessage = "{0} mora imati minimalnu dužinu od {2} znakova.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }
    }
}