using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.ProductionFollowUp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.DataSource
{
    [DataObject]
    public class JobSupplyODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobSupplyVM GetJobSupplyODS(DateTime? startDate, DateTime? endDate)
        {
            var model = new JobSupplyVM();
            if (startDate.HasValue && endDate.HasValue)
            {
                var jobs = db.Jobs.ToList().Where(x => x.DateCreated.Value.Date >= startDate.Value.Date
                  && x.DateCreated.Value.Date <= endDate.Value.Date).ToList();
                model.From = startDate?.ToString("dd/MM/yyyy");
                model.To = endDate?.ToString("dd/MM/yyyy");
                model.TotalJob = jobs.Count().ToString();
                model.Proforma = Math.Round(jobs.Where(q=>q.hv).Sum(s=>s.TotalPrice),2).ToString();
                model.DirectOrder = Math.Round(jobs.Where(q=>!q.hv).Sum(s=>s.TotalPrice),2).ToString();
                model.To = endDate?.ToString("dd/MM/yyyy");
               
                if (jobs != null)
                {
                    List<JobSupplyItemVM> jobList = new List<JobSupplyItemVM>();
                    foreach (var jobOrder in jobs)
                    {
                        var job = new JobSupplyItemVM();


                        var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobOrder.JobTypeId);
                        var client = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);

                        job.JobType = jobType?.JobName;
                        job.Code = jobType?.JobCode;
                        job.TotalPrice = jobOrder.TotalPrice;
                        jobList.Add(job);
                    }
                    model.JobSupplyItemVMs = jobList;
                }
            }


            return model;
        }
    }
}

