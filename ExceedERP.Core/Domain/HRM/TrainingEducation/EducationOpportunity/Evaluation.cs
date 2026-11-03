using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
   public class Evaluation
    {
        public int EvaluationId { get; set; }
        public decimal Start { get; set; }
        public decimal End { get; set; }
        public decimal Result { get; set; }
    }
}
