using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    [Table("EmployeeCategory")]
    public class EmployeeCategory
    {
        public int EmployeeCategoryID { get; set; }

        public string Description { get; set; }

        public int Index { get; set; }

        public int? Parent { get; set; }


        public bool IsActive { get; set; }

        public string Remark { get; set; }
    }
}
