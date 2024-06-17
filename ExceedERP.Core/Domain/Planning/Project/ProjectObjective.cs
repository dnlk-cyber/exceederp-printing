using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning.Project
{
    public class ProjectObjective
    {
        [Key]
        public int ProjectObjectiveId { get; set; }
        [Display(Name = "Objective Name")]
        [Required]
        public string ProjectObjectiveName { get; set; }
        public string Description { get; set; }

    }
}
