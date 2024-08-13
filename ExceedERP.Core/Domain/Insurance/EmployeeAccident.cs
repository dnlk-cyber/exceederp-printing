using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Insurance
{
    public class EmployeeAccident
    {
        public int EmployeeAccidentId { get; set; }
        [DisplayName("Employee Name")]
        public int EmployeeId { get; set; }
      //  public Person_Employee Person_Employee { get; set; }
        [DisplayName("Branch Name")]
        public int BranchId { get; set; }
        [DisplayName("Department")]
        public int? OrgStructureId { get; set; }
        [DisplayName("Job Title")]
        public int? LookupId { get; set; }
        [DisplayName("Is At Work Place")]
        public bool IsAtWorkPlace { get; set; }
        [DisplayName("Is On Duty")]
        public bool IsOnDuety { get; set; }
        [Display(Name = "Is out oF system")]
        public bool IsOutOFSystem { get; set; }
        [Display(Name = "Daily Labour")]
        public string DailyLabour { get; set; }
        public string Witness { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Accident Date")]
        public DateTime AccidentDate { get; set; }
        [DataType(DataType.Time)]
        [Display(Name = "Accident Time")]
        public DateTime? AccidentTime { get; set; }
        public string LevelOfInjuired { get; set; }
        [DisplayName("Accident Date")]
        public string AccidentDateAmh { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsSendForApproval { get; set; }
        [NotMapped]
        public decimal Salary { get; set; }
        [NotMapped]
        public string EmployeementType { get; set; }
        [Display(Name = "Birth Date")]
        [NotMapped]
        public string  BirthDate { get; set; }
        public string Description { get; set; }

    }
}
