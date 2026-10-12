using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class RecruitmentSummary
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
       
        public string EduLevel { get; set; }
       
        public string EduField { get; set; }
       
        public string RelevantExperience { get; set; }

        
        public decimal RelatedExperience { get; set; }
       
        public decimal Gender { get; set; }
      
        public decimal Disability { get; set; }
       
        public decimal Profile { get; set; }
       
        public string Training { get; set; }

        public decimal Interview { get; set; }
        public decimal TheoryExam { get; set; }
       
        public decimal PracticalExam { get; set; }
        public string Discipline { get; set; }
      
        public decimal Total { get; set; }
        public decimal Nation { get; set; }
        public int Rank { get; set; }
        public decimal Performance { get; set; }
        public string RecruitmentResult { get; set; }
        public decimal OtherResult { get; set; }
        public string Remark { get; set; }
    }
}