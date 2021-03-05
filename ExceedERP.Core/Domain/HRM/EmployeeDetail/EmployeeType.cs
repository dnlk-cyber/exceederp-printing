using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
