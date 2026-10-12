using ExceedERP.Core.Domain.Common;
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
    public class ReligionListODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumViewModel> GetReligionList()
        {
            return Enum.GetValues(typeof(ReligionList))
                .Cast<ReligionList>()
                .Select(e => new EnumViewModel
                {
                    Value = e.ToString(), // Raw enum name
                    DisplayName = GetDisplayName(e) // Localized or default display name

                }).ToList();
        }

        private string GetDisplayName(ReligionList religion)
        {
            FieldInfo fieldInfo = religion.GetType().GetField(religion.ToString());
            if (fieldInfo != null)
            {
                DisplayAttribute displayAttribute = fieldInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.GetName();
                }
            }

            return religion.ToString(); // Fallback to raw enum name
        }
     
    }

   
}
