using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class InternalApplicantExamResult
    {
        public int InternalApplicantExamResultId { get; set; }
        public int InternalApplicantScreeningId { get; set; }
        public ExamType ExamType { get; set; }
        public decimal SettingValue { get; set; }
        public decimal CurrentResult { get; set; }
        public decimal Total { get; set; }
  
        public string Remark { get; set; }
    }
}
