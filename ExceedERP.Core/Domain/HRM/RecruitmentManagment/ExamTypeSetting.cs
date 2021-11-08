using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class ExamTypeSetting
    {
        public int ExamTypeSettingId { get; set; }
        public RecruitmentSource ExamSource { get; set; }
        public ExamType ExamType { get; set; }
        public decimal SettingVlaue { get; set; }
        public string Remark { get; set; }
    }
}
