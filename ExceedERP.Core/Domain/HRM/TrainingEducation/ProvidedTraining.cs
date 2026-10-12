using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    public class ProvidedTraining : TrackUserOperation
    {
        [Key]
        public int ProvidedTrainingId { get; set; }

        [Required]
        [Display(Name = "Training Name")]
        public string TrainingName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Duration")]
        public int? Duration { get; set; }

        [Display(Name = "Duration Unit")]
        public string DurationUnit { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Required]
        public int MainCompanyId { get; set; }

        public virtual MainCompany MainCompany { get; set; }
    }
}
