using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantExamResult
    {
        [Key]
        public int ApplicantResultId { get; set; }       
        public int ApplicantScreeningId { get; set; }     
        public ExamType ExamType { get; set; }       
        public decimal SettingValue { get; set; }        
        public decimal CurrentResult { get; set; }      
        public string Remark { get; set; }
    }
}