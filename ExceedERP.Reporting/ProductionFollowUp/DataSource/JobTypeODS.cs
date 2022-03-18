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
    public class JobTypeODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobTypeVM GetJobTypeODS(DateTime? startDate, DateTime? endDate)
        {
            var model = new JobTypeVM();
            if (startDate.HasValue && endDate.HasValue)
            {
                model.From = startDate?.ToString("dd/MM/yyyy");
                model.To = endDate?.ToString("dd/MM/yyyy");
                var jobs = db.Jobs.ToList().Where(x => x.DateCreated.Value.Date >= startDate.Value.Date
                && x.DateCreated.Value.Date <= endDate.Value.Date).ToList();
                if (jobs != null)
                {
                    List<JobTypeItemVM> jobList = new List<JobTypeItemVM>();
                    foreach (var jobOrder in jobs)
                    {
                        var job = new JobTypeItemVM();

                        job.JobNo = jobOrder.JobNo;

                        var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobOrder.JobTypeId);
                        var client = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);

                        job.JobType = jobType?.JobName;
                        job.Client = client?.TradeName;
                        job.Quantity = jobOrder.Quantity + "x" + jobOrder.Pages;
                        job.Size = jobOrder.FinishingSize + "x" + jobOrder.Copies;
                        job.TotalPrice = jobOrder.TotalPrice;
                        job.Code = jobType?.JobCode;
                        jobList.Add(job);
                    }
                    model.JobTypeItemVMs = jobList;
                }
            }


            return model;
        }
    }
}
