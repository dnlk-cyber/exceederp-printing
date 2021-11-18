using ExceedERP.Core.Domain.PrintingEstimation.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.ProductionFollowUp
{
    public class JobStatus
    {
        public int JobStatusId { get; set; }
        public int JobId { get; set; }
        [Display(Name = "User")]
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
        public JobState Status { get; set; }
    }

}
