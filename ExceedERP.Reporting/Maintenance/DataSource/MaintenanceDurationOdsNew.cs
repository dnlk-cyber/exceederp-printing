using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    class MaintenanceDurationOdsNew
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MaintDurationViewModel> GetDuration(string num)
        {
            List<MaintDurationViewModel> list = new List<MaintDurationViewModel>();
            if(int.TryParse(num, out int jobNum))
            {
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.Num == jobNum);
                if(jobOrder != null)
                {

                }
            }
            return list;
        }

        public class MaintDurationViewModel
        {
            public string PlateNo { get; set; }
            public DateTime MyProperty { get; set; }
        }
    }
}
