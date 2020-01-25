using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class ContactFormViewModel
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
        [Required]
        [Display(Name = "Enquiry")]
        public string Message { get; set; }
        [Required]
        [Display(Name = "Please add me to your mailing list")]
        public bool Maillist { get; set; }
        public string gDPREmailMessage { get; set; }
    }
  

    public class BrochureRequestFormViewModel
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
        public IEnumerable<SelectListItem> Brochure { get; set; }
        [Required(ErrorMessage = "Please select how you would like the brochure to be sent")]
        [Display(Name = "How would you like the brochure to be sent")]
        public string BrochureSelected { get; set; }
        [RequiredIf("BrochureSelected", "Post", ErrorMessage = "Address is required if 'Post' selected.")]
        [Display(Name = "Address (For sending brochure by post)")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Please add me to your mailing list")]
        public bool Maillist { get; set; }
        public string gDPREmailMessage { get; set; }
    }
}