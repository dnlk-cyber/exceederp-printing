using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
   public class EmployeeAccident
    {
        public int EmployeeAccidentId { get; set; }
        [DisplayName("Employee Name")]
        public int EmployeeId { get; set; }
        public Person_Employee Person_Employee { get; set; }
        [DisplayName("Branch Name")]
        public int BranchId { get; set; }
        [DisplayName("Department")]
        public int OrgStructureId { get; set; }
        [DisplayName("Job Title")]
        public int LookupId { get; set; }
        [DisplayName("Is At Work Place")]
        public bool IsAtWorkPlace { get; set; }
        [DisplayName("Is On Duty")]
        public bool IsOnDuety { get; set; }
        public string Witness { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Accident Date")]
        public DateTime AccidentDate { get; set; }
        [DataType(DataType.Time)]
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
        public string Description { get; set; }
    }
}
