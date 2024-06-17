using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
    public class BSCActual
    {
        [Key]
        public int BSCActualId { get; set; }
        [Required]
        [Display(Name ="Objective")]
        public int ObjectiveId { get; set; }
        public Objective Objective { get; set; }
        [Display(Name = "Work Unit")]
        [Required]
        public int OrgStructureId { get; set; }
        public int? BSCPlanId { get; set; }
        public bool IsCopied { get; set; }
        public bool IsFirstLevelApproved { get; set; }
        public bool IsSecondLevel { get; set; }
        public bool IsThirdLevel { get; set; }
        [NotMapped]
        public bool HasChild { get; set; }
        public int ActualId { get; set; }
        public Actual Actuall { get; set; }

       

    }

}
