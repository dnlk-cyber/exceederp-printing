using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class TemplateTypeODS : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumViewModel> GetTemplateType()
        {
            return Enum.GetValues(typeof(TemplateTypes))
                .Cast<TemplateTypes>()
                .Select(e => new EnumViewModel
                {
                    Value = e.ToString(), // Raw enum name
                    DisplayName = GetDisplayName(e) // Localized or default display name

                }).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumViewModel> GetTemplateTypeSetting()
        {
            return db.TemplateTypeSettings
                     .Select(e => new EnumViewModel
                     {
                         Value = e.TemplateTypeSettingId.ToString(), // usually string for dropdowns
                         DisplayName = e.TemplateType
                     })
                     .ToList();
        }



        private string GetDisplayName(TemplateTypes templateTypes)
        {
            FieldInfo fieldInfo = templateTypes.GetType().GetField(templateTypes.ToString());
            if (fieldInfo != null)
            {
                DisplayAttribute displayAttribute = fieldInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.GetName();
                }
            }

            return templateTypes.ToString(); // Fallback to raw enum name
        }
     
            public void Dispose()
            {
                db.Dispose();
            }
        
    }

   
}
