using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantTotalResult
    {

        public string Name { get; set; }
        public int Age { get; set; }
        public Gender Sex { get; set; }  
        public decimal EduLevel { get; set; }  
        public decimal EduField { get; set; }   
        public decimal RelevantExperience { get; set; }  
        public decimal RelatedExperience { get; set; }  
        public decimal Gender { get; set; }    
        public decimal Disability { get; set; }  
        public decimal Profile { get; set; }  
        public decimal Training { get; set; } 
        public decimal Interview { get; set; }
        public decimal TheoryExam { get; set; } 
        public decimal PracticalExam { get; set; }
        public decimal Discipline { get; set; }  
        public decimal Total { get; set; }
        public decimal Nation { get; set; }
        public int Rank { get; set; }
        public decimal Performance { get; set; }
        public RecruitmentResult RecruitmentResult { get; set; }
    }
}
