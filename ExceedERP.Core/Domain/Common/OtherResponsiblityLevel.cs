
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Common
{
   

    public enum ResponsibilityLevel
    {
        [Display(Name = "Enum_ResponsibilityLevel_TOP_MANAGMENT", ResourceType = typeof(Resources))]
        TOP_MANAGMENT,
        [Display(Name = "Enum_ResponsibilityLevel_MEDIUM_MANAGMENT", ResourceType = typeof(Resources))]
        MEDIUM_MANAGMENT,
        [Display(Name = "Enum_ResponsibilityLevel_LOW_MANAGMENT", ResourceType = typeof(Resources))]
        LOW_MANAGMENT,
        [Display(Name = "Enum_ResponsibilityLevel_STAFF", ResourceType = typeof(Resources))]
        STAFF
    }
}
