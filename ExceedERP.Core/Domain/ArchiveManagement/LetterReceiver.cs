using System.ComponentModel;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class LetterReceiver
    {
        public int LetterReceiverId { get; set; }
        public int LetterForMultipleStaffId { get; set; }
        [DisplayName("Employee")]
        public int? EmployeeId { get; set; }
        [DisplayName("Is For All Employees ?")]
        public bool IsForAllEmployee { get; set; }
        public bool IsTransfered { get; set; }
        public int[] EmployeesSelected { get; set; }
    }
}