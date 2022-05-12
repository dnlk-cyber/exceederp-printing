using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Reporting.JobCosting.DataSource
{
    [DataObject]
    public  class JobCostingParameterOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetJobOrders()
        {
            var jobOrders = db.Jobs.ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.JobId.ToString(),
                    Text = s.JobNo
                }).ToList();

            return jobOrders;
        }
    }
}
