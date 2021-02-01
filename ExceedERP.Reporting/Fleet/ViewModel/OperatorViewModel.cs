using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class OperatorViewModel : Organization
    {


           public string FullName { get; set; }
          public string SalaryIncentive { get; set; }

         public string Gender { get; set; }
        public enum TypeofEmploymentenum { Seasonal, Permanent }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> HireDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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
         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
         public Nullable<DateTime> LicenseIssuedDate { get; set; }
          public string LicenseCity { get; set; }
         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LicenseRenewalDate { get; set; }
         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LicenseExpireDate { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string MobilePhone { get; set; }


        public string Email { get; set; }
        public string GuaranteeName { get; set; }
         public string GuaranteeAddress { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        public string GuaranteePhoneNumber { get; set; }

        public string Attachment { get; set; }

        public string OperatorPhoto { get; set; }
        public string Location { get; set; }
    


    }
}
