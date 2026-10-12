using System;
using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{

    public class HolidayGiftViewModel
    {
        public string Code { get; set; } // S/no. 
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public DateTime DateOfEmployment { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal HolidayGift { get; set; }
    }
}