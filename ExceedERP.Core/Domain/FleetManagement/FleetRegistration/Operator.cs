
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class Operator : Operations
    {

        public Operator()
        {

            this.TireIssues = new HashSet<TireIssue>();
            this.TireReturns = new HashSet<TireReturn>();

        }
         
        [ScaffoldColumn(false)]
        public Guid OperatorID { get; set; }
        [DisplayName(" Employee ")]
        public int EmployeeId { get; set; }
        [DisplayName(" Equipment ")]
        public Guid FleetsID { get; set; }
        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [Required]
        [DisplayName("Salary Incentive ")]
        public string SalaryIncentive { get; set; }

        [Required]
        public string Gender { get; set; }
        public enum TypeofEmploymentenum { Seasonal, Permanent }
        [DisplayName(" Activated Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public Nullable<DateTime> HireDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DateOfTermination { get; set; }

        public enum Postionenum { MainOperator, StandByOperator, AssistantOperator }
        [Required]
        public string Postion { get; set; }
        public enum Genderenum { Male, Female }
        public enum SalaryIncentiveenum { MonthlySalary, DailyPremium, Otherincentive, DesertAllowance }
        [DisplayName("Education Level  ")]
        public string EducationLevel { get; set; }
        [Required]
        public string LicenseNo { get; set; }
        [Required]
        [DisplayName("License Type ")]
        public string LicenseType { get; set; }
        public enum LicenseTypeenum { NewVersion, OldVersion }

        public enum LicenseLevelenum { Hizeb1, Hizeb2 }
        [DisplayName("License Level ")]
        [Required]
        public string LicenseLevel { get; set; }
        [DisplayName("Name Of Training Center ")]
        public string NameOfTraningCenter { get; set; }
        public enum COCResult { Attached, NotYet }
        [DisplayName("License Issued Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public Nullable<DateTime> LicenseIssuedDate { get; set; }
        [DisplayName("License City ")]
        [Required]
        public string LicenseCity { get; set; }
        [DisplayName("License Renewal Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LicenseRenewalDate { get; set; }
        [DisplayName("License Expire Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LicenseExpireDate { get; set; }


        public string MobilePhone { get; set; }


        public string Email { get; set; }
        [DisplayName("Guarantee Name ")]
        public string GuaranteeName { get; set; }
        [DisplayName("Guarantee Address ")]
        public string GuaranteeAddress { get; set; }
        [DisplayName("Guarantee Phone Number  ")]
        public string GuaranteePhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public string Attachment { get; set; }
        [NotMapped]
        [DisplayName("Job Title")]
        public string JobTitle { get; set; }

        public string OperatorPhoto { get; set; }



        public virtual ICollection<TireReturn> TireReturns { get; set; }
        public virtual ICollection<TireIssue> TireIssues { get; set; }


    }
}
