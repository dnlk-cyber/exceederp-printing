using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class Person_Contact
    {
        public int Person_ContactID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Father Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Grand Father Name")]
        [Required]
        public string LastName { get; set; }
        public string Nationality { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOfBirth { get; set; }
        public string Title { get; set; }
        [Required]
        public string Gender { get; set; }
        public int CategoryID { get; set; }
        public bool IsActive { get; set; }
        public string Remark { get; set; }
    }

  

   
   
}
