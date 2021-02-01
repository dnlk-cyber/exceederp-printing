using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Budget.Task
{
    public class TaskUnit : TrackUserOperation
    {
        public int TaskUnitId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
