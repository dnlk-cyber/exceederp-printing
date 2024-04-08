using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    ///  Sending Letter to department 
    public class OutGoing_DepartmentToDepartment
    {
        public int OutGoing_DepartmentToDepartmentId { get; set; }
        [DisplayName("Department")]
        public int OrgstructureId { get; set; }    
        public string Message { get; set; }
        public bool IsForAll { get; set; }
        public bool IsSent { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }
        public string DateEt { get; set; }
        public string Time { get; set; }
    }
}
