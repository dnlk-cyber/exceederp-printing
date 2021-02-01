using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Budget.Task
{
    public class TaskCategory : TrackUserOperation
    {
        public int TaskCategoryId { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<TaskPlan> TaskPlan { get; set; }
    }
}
