using ExceedERP.Core.Domain.PrintingEstimation.Setting;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.ProductionFollowUp.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        public JobTypeVM GetJobTypeODS(DateTime? startDate, DateTime? endDate,string jobPhase, string jobTypeId, string customerId)
        {
            var model = new JobTypeVM();
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            Image LogoFileName = null;
            string AmharicName = "";
            string Name = "";
            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var logo =
                                   db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                           a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            model.LogoFileName = LogoFileName;
            model.Name = Name;
            model.AmharicName = AmharicName;
            if (startDate.HasValue && endDate.HasValue)
            {
                model.From = startDate?.ToString("dd/MM/yyyy");
                model.To = endDate?.ToString("dd/MM/yyyy");
                var jobs = db.Jobs.ToList().Where(x => x.ReceivedDate.HasValue ?  x.ReceivedDate.Value.Date >= startDate.Value.Date
                && x.ReceivedDate.Value.Date <= endDate.Value.Date:false).ToList();
                if (jobTypeId != null)
                {
                    jobs = jobs.Where(q => q.JobTypeId.ToString() == jobTypeId).ToList();
                    model.JobCode = db.JobCategories.Where(q => q.JobCategoryId.ToString() == jobTypeId).FirstOrDefault()?.JobCode;
                }
                else
                {
                    model.JobCode = "All";
                }
                if (customerId != null)
                {
                    jobs = jobs.Where(q => q.CustomerId.ToString() == customerId).ToList();

                }
                if (jobs != null)
                {
                    List<JobTypeItemVM> jobList = new List<JobTypeItemVM>();
                    foreach (var jobOrder in jobs)
                    {
                        var job = new JobTypeItemVM();

                        job.JobNo = jobOrder.JobNo;

                        var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobOrder.JobTypeId);
                        var client = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);
                        var totalPrice = jobOrder.TotalPrice;
                        if(jobPhase!=null)
                        {
                            var jobStatus = db.JobStatuses.Where(q => q.JobId == jobOrder.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                            
                            if (jobStatus == null&& jobPhase != ProductionDepartment.JobFollowup.ToString())
                            {
                                continue;

                            }
                           
                        }
                        job.JobNo = jobOrder?.JobNo;
                        job.JobType = jobType?.JobName;
                        job.Client = client?.TradeName;
                        job.Quantity = jobOrder.Quantity;
                        job.Copy = jobOrder.Pages + "x" + jobOrder.Copies;
                        job.Size = jobOrder.FinishingSize;
                        job.TotalPrice = totalPrice;
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
