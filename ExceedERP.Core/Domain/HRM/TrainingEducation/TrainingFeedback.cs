using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
   public class TrainingFeedback
    {
        public int TrainingFeedbackId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "FeedBack", ResourceType = typeof(Localization.Resources))]
        public string FeedBack { get; set; }
        public int CourseTrackingId { get; set; }
    }
}
