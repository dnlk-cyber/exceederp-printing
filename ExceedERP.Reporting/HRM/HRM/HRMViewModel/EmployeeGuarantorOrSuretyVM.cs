using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeGuarantorOrSuretyVM
    {
        public int GuarantorOrSuretyId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeFullName { get; set; }
        public string Remark { get; set; }
        public List<GuarantorOrSuretyDetailVM> Details { get; set; } = new List<GuarantorOrSuretyDetailVM>();
    }

    public class GuarantorOrSuretyDetailVM
    {
        public string FullName { get; set; }
        public string Organiztion { get; set; }
        public DateTime FromDate { get; set; }
        public string FromDateAm { get; set; }
        public DateTime? ToDate { get; set; }
        public string ToDateAm { get; set; }
    }


}