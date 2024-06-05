
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;

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
        public Guid? FleetsID { get; set; }
        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [Required]
        [DisplayName("Salary Incentive ")]
        public string SalaryIncentive { get; set; }
        public bool IsExternal { get; set; } // Added
        [Required]
        public string Gender { get; set; }
        public  TypeofEmploymentenum TypeofEmploymentenum { get; set; }
        [DisplayName(" Activated Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public Nullable<DateTime> HireDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DateOfTermination { get; set; }

        public Postionenum Postionenum { get; set; }
        [Required]
        public string Postion { get; set; }
        public Genderenum Genderenum { get; set; }
        public SalaryIncentiveenum SalaryIncentiveenum { get; set; }
        [DisplayName("Education Level  ")]
        public string EducationLevel { get; set; }
        [Required]
        [DisplayName("License No")]
        public string LicenseNo { get; set; }
        [Required]
        [DisplayName("License Type ")]
        public string LicenseType { get; set; }
        public LicenseTypeenum LicenseTypeenum { get; set; }

        public LicenseLevelenum LicenseLevelenum { get; set; }
        [DisplayName("License Level ")]
        [Required]
        public string LicenseLevel { get; set; }
        [DisplayName("Training Center ")]
        public string NameOfTraningCenter { get; set; }
        public COCResult COCResult { get; set; }
        [DisplayName("Issued Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public Nullable<DateTime> LicenseIssuedDate { get; set; }
        [DisplayName("License City ")]
        [Required]
        public string LicenseCity { get; set; }
        [DisplayName("Renewal Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LicenseRenewalDate { get; set; }
        [DisplayName("Expire Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LicenseExpireDate { get; set; }
        [DisplayName("Phone No")]
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        [DisplayName("Co-sign Name ")]
        public string GuaranteeName { get; set; }
        [DisplayName("Co-sign Address ")]
        public string GuaranteeAddress { get; set; }
        [DisplayName("Co-sign Phone")]
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
    public enum COCResult
    {
        Attached,
        NotYet
    }
    public enum TypeofEmploymentenum
    {
        Seasonal,
        Permanent
    }
    public enum Postionenum
    {
        MainOperator,
        StandByOperator,
        AssistantOperator
    }
    public enum Genderenum
    {
        Male,
        Female
    }
    public enum SalaryIncentiveenum
    {
        MonthlySalary,
        DailyPremium,
        Otherincentive,
        DesertAllowance
    }
    public enum LicenseTypeenum
    {
        NewVersion,
        OldVersion
    }
    public enum LicenseLevelenum
    {
        Hizeb1,
        Hizeb2
    }

}
