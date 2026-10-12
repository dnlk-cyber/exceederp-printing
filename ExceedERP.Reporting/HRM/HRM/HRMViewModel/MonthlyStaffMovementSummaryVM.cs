using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class MonthlyStaffMovementSummaryVM
    {
        public string ProjectName { get; set; }

        public int StartMale { get; set; }
        public int StartFemale { get; set; }
        public int StartTotal => StartMale + StartFemale;

        public int NewMale { get; set; }
        public int NewFemale { get; set; }
        public int NewTotal => NewMale + NewFemale;

        public int SeparatedMale { get; set; }
        public int SeparatedFemale { get; set; }
        public int SeparatedTotal => SeparatedMale + SeparatedFemale;

        public int EndMale { get; set; }
        public int EndFemale { get; set; }
        public int EndTotal => EndMale + EndFemale;
    }
}
