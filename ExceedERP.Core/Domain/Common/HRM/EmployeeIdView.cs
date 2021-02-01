using System;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class EmployeeIdView
    {
        public int PersonId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string FullNameEng { get; set; }
        public string FullNameWithOutCode { get; set; }
        public string URL { get; set; }
        public bool IsActive { get; set; }
        public EmployeeStatus Status { get; set; }
        public int Age { get; set; }
        public DateTime PlacmentDate { get; set; }
        public AssignmentType AssignmentType { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime HiredDate { get; set; }
        
    }
}