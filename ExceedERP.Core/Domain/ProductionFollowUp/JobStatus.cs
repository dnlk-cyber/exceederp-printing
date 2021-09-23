using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.ProductionFollowUp
{
    public class JobStatus
    {
        public int JobStatusId { get; set; }
        public int JobId { get; set; }
        public string UserId { get; set; }
        //started time
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }
        //finished time
        //[Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        // [Required]
        [Display(Name = "Phase")]
        public JobPhase JobPhase { get; set; }
        // [Required]
        [Display(Name = "Type")]
        public CheckType Type { get; set; }
    }

}
