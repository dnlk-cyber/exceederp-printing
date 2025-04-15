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



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectJobCost> GetJobCost()// project status 
        {
            var jobOrdersCost = db.JobCosts.ToList()

                .Select(s => new SelectJobCost
                {
                    JobCostId = s.JobCostId,
                    JobId = s.JobId,
                }).ToList();

            return jobOrdersCost;
        }
    }
    public class SelectJobCost
    {
        public int JobCostId { get; set; }
        public int JobId { get; set; }

    public string jobName { get; set; }
    }
}

