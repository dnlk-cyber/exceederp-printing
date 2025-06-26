using ExceedERP.Core.Domain.InfoDesk.License;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ExceedERP.Reporting.HelpDesk.DataSource
{
    [DataObject]
    public class SoftwareLicenseTypeODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumDisplayValue> GetSoftwareLicenseType()
        {
            return Enum.GetValues(typeof(LicenseStatus))
                       .Cast<LicenseStatus>()
                       .Select(ts => new EnumDisplayValue
                       {
                           DisplayName = GetEnumDisplayName(ts),
                           Value = ts.ToString()
                       })
                       .ToList();
        }

        private string GetEnumDisplayName(LicenseStatus status)
        {
            var displayAttribute = status.GetType()
                                         .GetMember(status.ToString())
                                         .First()
                                         .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute != null
                ? displayAttribute.GetName()
                : status.ToString();
        }

    }

}
