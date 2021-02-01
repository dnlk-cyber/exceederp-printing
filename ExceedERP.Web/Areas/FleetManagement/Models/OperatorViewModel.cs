using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class OperatorViewModel
    {
       
        public string PlateNumber { get; set; }
        public string FullName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public enum TypeofEmployment { Seasonal, Permanent }

        public Nullable<DateTime> HireDate { get; set; }
        public Nullable<DateTime> DateOfTermination { get; set; }

        public enum Postion { MainOperator, StandByOperator, AssistantOperator }

        public enum SalaryIncentive { MonthlySalary, DailyPremium, Otherincentive, DesertAllowance }

        public string EducationLevel { get; set; }

        public int LicenseNo { get; set; }
        public enum LicenseType { NewVersion, OldVersion }

        public enum LicenseLevel { Hizeb1, Hizeb2 }

        public string NameOfTraningCenter { get; set; }
        public enum COCResult { Attached, NotYet }
        public Nullable<DateTime> LicenseIssuedDate { get; set; }
        public string LicenseCity { get; set; }
        public Nullable<DateTime> LicenseRenewalDate { get; set; }
        public Nullable<DateTime> LicenseExpireDate { get; set; }

        public string MobilePhone { get; set; }

        public string Email { get; set; }

        public string GuaranteeName { get; set; }

        public string GuaranteeAddress { get; set; }

        public string GuaranteePhoneNumber { get; set; }

        public string Attachment { get; set; }

        public string OperatorPhoto { get; set; }
    }
}