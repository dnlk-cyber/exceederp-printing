using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class Announcment:TrackUserOperation
    {
        [Key]
        public int AnnouncmentId { get; set; }
        [Display(Name = "HR_Company_Announcment_Title", ResourceType = typeof(Localization.Resources))]
        public string Title { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_Company_Announcment_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set;}
        [Required]
        [Display(Name = "HR_Company_Announcment_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAnnAmharic { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}