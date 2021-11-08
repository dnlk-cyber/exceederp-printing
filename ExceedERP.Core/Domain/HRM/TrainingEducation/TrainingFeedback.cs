using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
   public class TrainingFeedback
    {
        public int TrainingFeedbackId { get; set; }
        public int EmployeeId { get; set; }
        public string FeedBack { get; set; }
        public int CourseTrackingId { get; set; }
    }
}
