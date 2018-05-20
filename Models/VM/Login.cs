using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models.VM
{
    public class Login
    {
        [Required(ErrorMessage = "Polje {0} je obavezno")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail adresa nije ispravna")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Polje {0} je obavezno")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }
    }
}