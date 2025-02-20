using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HelpDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HelpDesk.DataSource
{
    [DataObject]
    public class TicketStatusODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumDisplayValue> GetTicketStatusList()
        {
            return Enum.GetValues(typeof(TicketStatus))
                       .Cast<TicketStatus>()
                       .Select(ts => new EnumDisplayValue
                       {
                           DisplayName = GetEnumDisplayName(ts),
                           Value = ts.ToString()
                       })
                       .ToList();
        }

        private string GetEnumDisplayName(TicketStatus status)
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

    public class EnumDisplayValue
    {
        public string DisplayName { get; set; }
        public string Value { get; set; }
    }

}
