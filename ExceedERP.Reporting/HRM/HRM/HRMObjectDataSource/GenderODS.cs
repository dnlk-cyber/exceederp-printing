using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Allowance;
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

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class GenderODS
    {

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumViewModel> GetGender()
        {
            return Enum.GetValues(typeof(GenderBenefit))
                .Cast<GenderBenefit>()
                .Select(e => new EnumViewModel
                {
                    Value = e.ToString(), // Raw enum name
                    DisplayName = GetDisplayName(e) // Localized or default display name

                }).ToList();
        }

        private string GetDisplayName(GenderBenefit genderBenefit)
        {
            FieldInfo fieldInfo = genderBenefit.GetType().GetField(genderBenefit.ToString());
            if (fieldInfo != null)
            {
                DisplayAttribute displayAttribute = fieldInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.GetName();
                }
            }

            return genderBenefit.ToString(); // Fallback to raw enum name
        }
     
    }

   
}
