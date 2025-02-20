using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HelpDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HelpDesk.DataSource
{
    [DataObject]
   public class TicketTypeODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TicketTypeVM> GetTicketType()
        {
            List<TicketTypeVM> ticketTypeList = new List<TicketTypeVM>();
            var types = db.TicketType_HelpDesks.ToList();
            foreach(var type in types)
            {
                var ty = new TicketTypeVM();

                ty.TicketTypeId = type.TicketType_HelpDeskId;
                ty.TicketType = type.Type;

                ticketTypeList.Add(ty);
            }
            return ticketTypeList;
        }
    }
}
