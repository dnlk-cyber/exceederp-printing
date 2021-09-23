using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeType : TrackUserOperation
    {
        [Key]
        public int EmployeeTypeId { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
    }
}
