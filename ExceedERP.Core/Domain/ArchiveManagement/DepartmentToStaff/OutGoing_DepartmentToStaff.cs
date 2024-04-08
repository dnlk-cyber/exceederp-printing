using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff
{
    public class OutGoing_DepartmentToStaff
    {
        public int OutGoing_DepartmentToStaffId { get; set; }
        [DisplayName("Department")]
        public int OrgStructureId { get; set; }
        public string Message { get; set; }
        public bool IsForAll { get; set; }
        public bool IsSent { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }       
        public string DateEt { get; set; }
        public string Time { get; set; }
        //////
        [NotMapped]
        public bool IsLogedInUserHead { get; set; }
    }
}
