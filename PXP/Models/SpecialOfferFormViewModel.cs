using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class SpecialOfferFormViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        [Display(Name = "Position/Job Title")]
        public string Position { get; set; }
        [Required]
        [Display(Name = "Telephone")]
        public string Tel { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Message")]
        public string Message { get; set; }
        [Required]
        [Display(Name = "Please add me to your mailing list")]
        public bool Maillist { get; set; }
        public string gDPREmailMessage { get; set; }
    }
}