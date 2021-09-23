using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
