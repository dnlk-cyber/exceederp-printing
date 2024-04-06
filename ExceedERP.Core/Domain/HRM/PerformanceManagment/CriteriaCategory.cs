using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class CriteriaCategory
    {
        [Key]
        public int CriteriaCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
