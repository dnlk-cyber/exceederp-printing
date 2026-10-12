namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
  
        public class DayAbsentFlatViewModel
        {
            public int EmployeeId { get; set; }
            public string EmployeeCode { get; set; }
            public string FullName { get; set; }
            public string WorkUnit { get; set; }
            public string BranchName { get; set; }

            // Dynamic Pivot Dimensions
            public int GLPeriodId { get; set; }
            public int PeriodIndex { get; set; }
            public string PeriodName { get; set; } // e.g., "July 2025", "August 2025"

            // Metric
            public decimal AbsentDays { get; set; }
        }
    
    //public class DayAbsentReportViewModel
    //{
    //    public int EmployeeId { get; set; }
    //    public string EmployeeCode { get; set; }
    //    public string FullName { get; set; }
    //    public string WorkUnit { get; set; }
    //    public string BranchName { get; set; }
    //    public string BusinessUnit { get; set; }

    //    // Fiscal Year Monthly Breakdown (July to June)
    //    public int Month1_Jul { get; set; }
    //    public int Month2_Aug { get; set; }
    //    public int Month3_Sep { get; set; }
    //    public int Month4_Oct { get; set; }
    //    public int Month5_Nov { get; set; }
    //    public int Month6_Dec { get; set; }
    //    public int Month7_Jan { get; set; }
    //    public int Month8_Feb { get; set; }
    //    public int Month9_Mar { get; set; }
    //    public int Month10_Apr { get; set; }
    //    public int Month11_May { get; set; }
    //    public int Month12_Jun { get; set; }

    //    public int TotalAbsentDays { get; set; }
    //}
}