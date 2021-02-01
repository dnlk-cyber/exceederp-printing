using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Fleet.Printing.ViewModel
{
    public class OperatorViewModel : Organization
    {
        public Guid OperatorID { get; set; }
         public int EmployeeId { get; set; }
         public Guid FleetsID { get; set; }
          public string FullName { get; set; }
        public string SalaryIncentive { get; set; }

          public string Gender { get; set; }
        public enum TypeofEmploymentenum { Seasonal, Permanent }
          public Nullable<DateTime> HireDate { get; set; }
          public Nullable<DateTime> DateOfTermination { get; set; }

        public enum Postionenum { MainOperator, StandByOperator, AssistantOperator }
        public string Postion { get; set; }
        public enum Genderenum { Male, Female }
        public enum SalaryIncentiveenum { MonthlySalary, DailyPremium, Otherincentive, DesertAllowance }
         public string EducationLevel { get; set; }
        public string LicenseNo { get; set; }
         public string LicenseType { get; set; }
        public enum LicenseTypeenum { NewVersion, OldVersion }

        public enum LicenseLevelenum { Hizeb1, Hizeb2 }
         public string LicenseLevel { get; set; }
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

        public bool IsActive { get; set; }

        public string Attachment { get; set; }
         public string JobTitle { get; set; }

        public string OperatorPhoto { get; set; }

     
      
    }
}
