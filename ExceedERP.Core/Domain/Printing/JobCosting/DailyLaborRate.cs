using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.printing.PrintingEstimation.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.JobCosting
{
    public class DailyLaborRate : TrackUserSettingOperation 
    {
        public int DailyLaborRateId { get; set; }
        public string CostCenter { get; set; }
        public int GlFiscalYearId { get; set; }
        public int GLPeriodId { get; set; }
        public decimal? TotalSalary { get; set; }
        public decimal? TotalExpectedHour { get; set; }
        public decimal? DLR { get; set; }
        public EmployementType EmployeeType { get; set; }
    }

    public class DLRSalary : TrackUserSettingOperation
    {
        public int DLRSalaryId { get; set; }
        public int DailyLaborRateId { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? LessIndirectLabor { get; set; }
        public decimal NetSalary { get; set; }
        public decimal? LessAbsence { get; set; }
        public decimal? Total { get; set; }
        public decimal? Pension { get; set; }
        public decimal? TotalSalary { get; set; }
        public decimal? NoOfDay { get; set; }
        public decimal? RatePerDay { get; set; }
        public decimal? NoOfEmployee { get; set; }
    }
    public class DLRHour : TrackUserSettingOperation
    {

        public int DLRHourId { get; set; }
        public int DailyLaborRateId { get; set; }
        public decimal NoOfDay { get; set; }
        public decimal NoOfWorkingHour { get; set; }
        public decimal NoOfEmployee { get; set; }
        public decimal ExpectedTime { get; set; }

    }
}
