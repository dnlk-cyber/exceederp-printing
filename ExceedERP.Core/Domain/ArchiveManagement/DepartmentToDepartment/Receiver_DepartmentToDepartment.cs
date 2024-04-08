using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class Receiver_DepartmentToDepartment
    {
        public int Receiver_DepartmentToDepartmentId { get; set; }
        public int OutGoing_DepartmentToDepartmentId { get; set; }
        [DisplayName("Department")]
        public int OrgStructureId { get; set; }
        [NotMapped]
        public int[] ReceiverSelected { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }
        public string DateEt { get; set; }
        public string Time { get; set; }
        public bool IsSent { get; set; }
    }
}
