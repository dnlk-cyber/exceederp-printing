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
        public JobTypeVM GetJobTypeODS(DateTime? startDate, DateTime? endDate,string jobPhase, string status,  string jobTypeId, string size, string copy, string customerId)
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
                if (size != null)
                {
                    jobs = jobs.Where(q => q.FinishingSize.ToString() == customerId).ToList();

                }
                if (copy != null)
                {
                    jobs = jobs.Where(q => q.Copies.ToString() == customerId).ToList();

                }
                if (customerId != null)
                {
                    jobs = jobs.Where(q => q.CustomerId.ToString() == customerId).ToList();

                }
                //if(jobPhase==null)
                //{
                //    jobPhase = ProductionDepartment.JobFollowup.ToString();
                //}
                if (jobs != null)
                {
                    List<JobTypeItemVM> jobList = new List<JobTypeItemVM>();
                    foreach (var jobOrder in jobs)
                    {
                        var job = new JobTypeItemVM();

                        job.JobNo = jobOrder.JobNo;
                        string state = null;
                        decimal quantity = jobOrder.Quantity;
                        decimal totalPrice = jobOrder.TotalPrice;
                        var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobOrder.JobTypeId);
                        var client = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);
                        state = JobState.InProgress.ToString();
                        var jobStatus = db.JobStatuses.Where(q => q.JobId == jobOrder.JobId && q.JobPhase == ProductionDepartment.FinishingDelivery).FirstOrDefault();
                        if(jobStatus!=null)
                        {
                            state = jobStatus.Status.ToString();
                        }
                        if (jobPhase!=null)
                        {
                            jobStatus = db.JobStatuses.Where(q => q.JobId == jobOrder.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                            state = jobStatus?.Status.ToString();
                            if(jobStatus!=null)
                            {
                                if(status!=null)
                                {
                                    if (jobStatus.Status.ToString() != status)
                                    {
                                        continue;
                                    }
                                }
                                
                                var jobLineItems = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList();
                                quantity = jobLineItems.Sum(s => s.Quantiy);
                                totalPrice = jobLineItems.Sum(s => s.TotalPrice);
                                if(quantity==0&& totalPrice==0)
                                {
                                    continue;

                                }
                                state = jobStatus.Status.ToString();
                            }
                            else
                            {
                                continue;
                            }
                            if (jobStatus == null&& jobPhase != ProductionDepartment.JobFollowup.ToString()|| status ==JobState.Delivered.ToString())
                            {
                                continue;
                            }
                            
                        }
                        job.JobNo = jobOrder?.JobNo;
                        job.JobType = jobType?.JobName;
                        job.Client = client?.TradeName;
                        job.Quantity = quantity;
                        job.Copy = jobOrder.Pages.ToString() ;
                        job.Size = jobOrder.FinishingSize + "x" + jobOrder.Copies;
                        job.TotalPrice = totalPrice;
                        job.Code = jobType?.JobCode;
                        job.Status = state;
                        jobList.Add(job);
                    }
                    model.JobTypeItemVMs = jobList;
                }
            }


            return model;
        }
    }
}
