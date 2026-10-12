using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Health.DataSource
{
    [DataObject]
    public class HealthBenefitTypeODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumViewModel> GetHealthBenefitType()
        {
            return Enum.GetValues(typeof(HealthBenefitType))
                .Cast<HealthBenefitType>()
                .Select(e => new EnumViewModel
                {
                    Value = e.ToString(), // Raw enum name
                    DisplayName = GetDisplayName(e) // Localized or default display name

                }).ToList();
        }
        private string GetDisplayName(HealthBenefitType requestStatus)
        {
            FieldInfo fieldInfo = requestStatus.GetType().GetField(requestStatus.ToString());
            if (fieldInfo != null)
            {
                DisplayAttribute displayAttribute = fieldInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.GetName();
                }
            }

            return requestStatus.ToString(); // Fallback to raw enum name
        }
     
    }


    
}
