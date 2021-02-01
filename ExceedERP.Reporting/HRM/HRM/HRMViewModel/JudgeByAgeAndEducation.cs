using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class JudgeByAgeAndEducation
    {
        public string BranchName { get; set; }
        public List<JudgeEduaction> JudgeEduactions { get; set; }
        public int TotalMale { get; set; }
        public int TotalFemale { get; set; }
        public int TotalEduction { get; set; }
    }
}