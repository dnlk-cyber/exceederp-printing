using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class RecruitmentCriteriaTypeObjectDataSource
    {

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumViewModel> GetCriteriaType()
        {
            return Enum.GetValues(typeof(ExamType))
                .Cast<ExamType>()
                .Select(e => new EnumViewModel
                {
                    Value = e.ToString(), // Raw enum name
                    DisplayName = GetDisplayName(e) // Localized or default display name

                }).ToList();
        }
        private string GetDisplayName(ExamType examType)
        {
            FieldInfo fieldInfo = examType.GetType().GetField(examType.ToString());
            if (fieldInfo != null)
            {
                DisplayAttribute displayAttribute = fieldInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.GetName();
                }
            }

            return examType.ToString(); // Fallback to raw enum name
        }
   


    }
}