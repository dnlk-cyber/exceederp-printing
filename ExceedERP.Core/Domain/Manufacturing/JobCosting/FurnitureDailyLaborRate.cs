using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting
{
    public class FurnitureDailyLaborRate : TrackUserSettingOperation
    {
        public int FurnitureDailyLaborRateId { get; set; }
        public string CostCenter { get; set; }
        public int GlFiscalYearId { get; set; }
        public int GLPeriodId { get; set; }
        public decimal? TotalSalary { get; set; }
        public decimal? TotalExpectedHour { get; set; }
        public decimal? DLR { get; set; }
        public FurnitureEmployementType EmployeeType { get; set; }
    }

    public class FurnitureDLRSalary : TrackUserSettingOperation
    {
        public int FurnitureDLRSalaryId { get; set; }
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
    public class FurnitureDLRHour : TrackUserSettingOperation
    {

        public int FurnitureDLRHourId { get; set; }
        public int DailyLaborRateId { get; set; }
        public decimal NoOfDay { get; set; }
        public decimal NoOfWorkingHour { get; set; }
        public decimal NoOfEmployee { get; set; }
        public decimal ExpectedTime { get; set; }

    }
}
