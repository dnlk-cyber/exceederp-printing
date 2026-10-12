using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeBenefitVM
    {
        public string EmployeeCode { get; set; }
        public  string EmployeeFullName { get; set; }
        public string FiscalYear { get; set; }
        public string JobTitle { get; set; }
        public string OrgStructure { get; set; }
        public string Branch { get; set; }
        public string BusinessUnit { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public List<EmployeeBenefitEachVM> Details { get; set; } = new List<EmployeeBenefitEachVM>();
    }
    public class EmployeeBenefitEachVM
    {
        public string BenefitType { get; set; }
        public string UnitofMeasurement { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UtilizedQuantity { get; set; }  // Renamed from UsedQuantity
        public bool IsUtilized { get; set; }  // Renamed from IsTaken
        public string Remark { get; set; }
    }


}