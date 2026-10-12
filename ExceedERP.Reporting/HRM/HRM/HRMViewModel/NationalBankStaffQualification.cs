using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class NationalBankStaffQualificationSummaryVM
    {
        public string InstitutionName { get; set; }
        public string ReportType { get; set; }
        public string PeriodEnding { get; set; }

        public List<EducationLevelRowVM> EducationLevels { get; set; }
    }

    public class EducationLevelRowVM
    {
        public string EducationLevel { get; set; } // Diploma, Degree, Master...

        // Employment Type
        public int Permanent { get; set; }
        public int Temporary { get; set; }
        public int Others { get; set; }
        public int TotalEmployment => Permanent + Temporary+Others;

        // Gender
        public int Male { get; set; }
        public int Female { get; set; }
        public int TotalGender => Male + Female;

        // Grand Total (optional but useful)
        public int GrandTotal => TotalEmployment; // or TotalGender (should match)
    }
}
