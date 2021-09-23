using System;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.APTS
{
    public class PatientRegister
    {
        public int PatientRegisterId { get; set; }
       
        public string PatientName { get; set; }
        public string CardNumber { get; set; }
        public int Age { get; set; }
        public Gender Sex { get; set; }
        public string NameOfOrg { get; set; }
        public DateTime RegisterDate { get; set; }
        public string RegisterDateAmh { get; set; }
       
        public string PatientSignature { get; set; }
        public string SurviceProviderName { get; set; }
        public string SurviceProviderDepartment { get; set; }
        public string SurviceProviderSign { get; set; }
        public string AccountantName { get; set; }
        public string AccountantSign { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string Unit { get; set; }
        public bool IsCredit { get; set; }
        public int DrugistId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsDone { get; set; }
        [NotMapped]
        public bool IsAllDeducted { get; set; }

    }
}
