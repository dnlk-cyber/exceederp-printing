using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.SharedDataSources
{
   
        [DataObject]
        public class LeaveStatusObjectDataSource
        {
            private ExceedDbContext db = new ExceedDbContext();

            [DataObjectMethod(DataObjectMethodType.Select)]
            public List<EnumViewModel> GetLeaveStatus()
            {
                return Enum.GetValues(typeof(RequestStatus))
                    .Cast<RequestStatus>()
                    .Select(e => new EnumViewModel
                    {
                        Value = e.ToString(), // Raw enum name
                    DisplayName = GetDisplayName(e) // Localized or default display name

                }).ToList();
            }

            private string GetDisplayName(RequestStatus evaluationType)
            {
                FieldInfo fieldInfo = evaluationType.GetType().GetField(evaluationType.ToString());
                if (fieldInfo != null)
                {
                    DisplayAttribute displayAttribute = fieldInfo
                        .GetCustomAttribute<DisplayAttribute>();

                    if (displayAttribute != null)
                    {
                        return displayAttribute.GetName();
                    }
                }

                return evaluationType.ToString(); // Fallback to raw enum name
            }

        }

    
}