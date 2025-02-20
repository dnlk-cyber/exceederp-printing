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
   public class SupportGroupODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupportGroupVM> GetSupportGroup()
        {
            List<SupportGroupVM> groupList = new List<SupportGroupVM>();

            var groups = db.SupportGroup_HelpDesks.ToList();
            foreach(var group in groups)
            {
                var gr = new SupportGroupVM();

                gr.SupportGroupId = group.SupportGroup_HelpDeskId;
                gr.Name = group.Name;

                groupList.Add(gr);
            }

            return groupList;
        }
    }
}
