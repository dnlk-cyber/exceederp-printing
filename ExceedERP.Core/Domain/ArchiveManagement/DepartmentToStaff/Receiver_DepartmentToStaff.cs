using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff
{
    public class Receiver_DepartmentToStaff
    {
        public int Receiver_DepartmentToStaffId { get; set; }
        public int OutGoing_DepartmentToStaffId { get; set; }
        [DisplayName("Employee")]
        public int EmployeeId { get; set; }
        [DisplayName("Employee")]
        [NotMapped]
        public int[] ReceiverSelected { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }
        public string DateEt { get; set; }
        public string Time { get; set; }
        public bool IsSent { get; set; }
        /// for forwarding .. if the head select for all ,, employees under his department is added automaticaly 
        [NotMapped]
        public bool IsForAll { get; set; }
    }
}
