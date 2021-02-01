using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
    public class LineCheckList : Operations
    {
        public int LineCheckListId { get; set; }
        public int CheckListId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public virtual CheckList CheckList { get; set; }
    }
}
