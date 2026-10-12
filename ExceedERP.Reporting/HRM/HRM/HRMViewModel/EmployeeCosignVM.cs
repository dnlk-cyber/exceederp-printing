using ExceedERP.Core.Domain.Common;
using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeCosignVM
    {

        public string EmployeeCode { get; set; }
        public string EmployeeFullName { get; set; }
        public string CosignerFullName { get; set; }
        public DateTime AgreementDate { get; set; }
        public string AgreementDateAm { get; set; }
        public string Occupation { get; set; }
        public string Workorganization { get; set; }
        public string CosignTypeName { get; set; } // Populated from Lookup.Name or Lookup.Title
        public string Remark { get; set; }
        public decimal? Salary { get; set; }
    }

}