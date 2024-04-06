using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class ComplainDetail
    {
        [Key]
        public int ComplainDetailId { get; set; }
        public int ComplainId { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Response", ResourceType = typeof(Localization.Resources))]
        public string Response { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Rpt_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "Rpt_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAm { get; set; }
        [Display(Name = "Rpt_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}
