using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

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
