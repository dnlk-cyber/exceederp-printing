using System;
using System.Collections.Generic;
using System.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class ClearanceLetterVM
    {
        public string EmployeeName { get; set; }

        public string EmployeeId { get; set; }

        public string JobTitle { get; set; }

        public string CompanyName { get; set; }
        public string Department { get; set; }

        public string ClearanceDate { get; set; }

        public string Message { get; set; }

        public List<ClearanceLetterDetailVM> ClearanceLetterDetails { get; set; }
            = new List<ClearanceLetterDetailVM>();
    }

    public class ClearanceLetterDetailVM
    {
        public int SerialNo { get; set; }

        // Department/Section name
        public string Department { get; set; }

        // Person responsible
        public string HeadName { get; set; }

        // Position of approver
        public string JobTitle { get; set; }

        // Remark or responsibility status
        public string Remark { get; set; }

        // Signature image path or base64 string
        public Image Signature { get; set; }

        public DateTime? Date { get; set; }
    }
}