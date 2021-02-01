using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Common
{


    public enum AssignmentType
    {
        [Display(Name = "Enum_AssignmentType_NEW", ResourceType = typeof(Resources))]
        NEW,
        [Display(Name = "Enum_AssignmentType_PROMOTION", ResourceType = typeof(Resources))]
        PROMOTION,
        [Display(Name = "Enum_AssignmentType_YEARLY_PROMOTION", ResourceType = typeof(Resources))]
        YEARLY_PROMOTION,
        [Display(Name = "Enum_AssignmentType_TRANSFER", ResourceType = typeof(Resources))]
        TRANSFER,
        [Display(Name = "Enum_AssignmentType_DEMOTION", ResourceType = typeof(Resources))]
        DEMOTION,
        [Display(Name = "Enum_AssignmentType_RECOMENDATION", ResourceType = typeof(Resources))]
        RECOMENDATION,
        [Display(Name = "Enum_AssignmentType_ASSIGNMENT", ResourceType = typeof(Resources))]
        ASSIGNMENT,
        [Display(Name = "Enum_AssignmentType_STRUCTURE_CHANGE", ResourceType = typeof(Resources))]
        STRUCTURE_CHANGE


    }
}
