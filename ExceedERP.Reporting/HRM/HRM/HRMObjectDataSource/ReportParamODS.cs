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
    public class LeaveTypeODS
    {

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumViewModel> GetLeaveType()
        {
            return Enum.GetValues(typeof(LeaveType))
                .Cast<LeaveType>()
                .Select(e => new EnumViewModel
                {
                    Value = e.ToString(), // Raw enum name
                    DisplayName = GetDisplayName(e) // Localized or default display name

                }).ToList();
        }

        private string GetDisplayName(LeaveType evaluationType)
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

    [DataObject]
    public class VacancyTypeODS
    {

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumViewModel> GetVacancyType()
        {
            return Enum.GetValues(typeof(RecruitmentSource))
                .Cast<RecruitmentSource>()
                .Select(e => new EnumViewModel
                {
                    Value = e.ToString(), // Raw enum name
                    DisplayName = GetDisplayName(e) // Localized or default display name

                }).ToList();
        }

        private string GetDisplayName(RecruitmentSource evaluationType)
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
