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
using static ExceedERP.Reporting.ProductionFollowUp.DataSource.ProFollowupParameterODs;

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
            var jobs = db.Jobs.ToList();
            if (startDate.HasValue && endDate.HasValue)
            {
                model.From = startDate?.ToString("dd/MM/yyyy");
                model.To = endDate?.ToString("dd/MM/yyyy");
                if((jobPhase == null || jobPhase == ProductionDepartment.JobFollowup.ToString()))
                {
                    jobs = jobs.Where(x => x.ReceivedDate.HasValue ? x.ReceivedDate.Value.Date >= startDate.Value.Date
              && x.ReceivedDate.Value.Date <= endDate.Value.Date : false).ToList();
                }
               
            }
            var jobLists = new List<JobStatus>();
            if (jobPhase != null&&jobPhase != ProductionDepartment.JobFollowup.ToString())
            {
                 jobLists = db.JobStatuses.Where(x => x.JobPhase.ToString() == jobPhase).ToList();
                if (status == null)
                {
                 jobLists = jobLists.Where(q => q.Status.ToString() != JobState.Received.ToString()).ToList();
                }
                if (status==JobState.Received.ToString())
                {
                    jobLists = jobLists.Where(x => x.StartDate.HasValue ? x.StartDate.Value.Date >= startDate.Value.Date
       && x.StartDate.Value.Date <= endDate.Value.Date:false).ToList();
                }
                else
                {
                    jobLists = jobLists.Where(x => (x.StartDate.HasValue ? x.StartDate.Value.Date >= startDate.Value.Date : false)
                           && (x.EndDate.HasValue ? x.EndDate.Value.Date <= endDate.Value.Date : false)).ToList();
                }
                var jobIds = jobLists.Select(s=>s.JobId);
            jobs = db.Jobs.ToList().Where(q => jobIds.Contains(q.JobId)).ToList();
            }
            string progressStatus = null;
            if (status != null)
            {
                if (status== JobOrderStatus.OnProgress.ToString())
                {
                    progressStatus = JobOrderStatus.OnProgress.ToString();
                  
                    jobLists = jobLists.ToList().Where(x => x.Status.ToString() == JobState.ParitiallyFinished.ToString()|| x.Status.ToString() == JobState.Received.ToString()).ToList();
                }
                else if (status!=JobState.Received.ToString())
                {
                    jobLists = jobLists.ToList().Where(x => x.Status.ToString() == status).ToList();

                }           
                var jobIdss = jobLists.Select(s => s.JobId);
                 jobs = jobs.Where(q => jobIdss.Contains(q.JobId)).ToList();
            }

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
                    jobs = jobs.Where(q => q.FinishingSize == size).ToList();

                }
                if (copy != null)
                {
                    jobs = jobs.Where(q => q.Copies.ToString() == copy).ToList();

                }
                if (customerId != null)
                {
                    jobs = jobs.Where(q => q.CustomerId.ToString() == customerId).ToList();

                }
                            
                    if (jobs.Any())
                {
                    List<JobTypeItemVM> jobList = new List<JobTypeItemVM>();
                    foreach (var jobOrder in jobs)
                    {
                        var job = new JobTypeItemVM();

                        job.JobNo = jobOrder.JobNo;
                        string state = null;
                        var changeOfOrders = db.ChangeOrders.Where(q => q.JobId == jobOrder.JobId).ToList();
                        var chgQua = changeOfOrders.Sum(q => q.Quantity);
                        var chgTotPrice = changeOfOrders.Sum(q => q.Total);
                        decimal quantity = jobOrder.Quantity + chgQua;
                        decimal totalPrice = jobOrder.TotalPrice+ chgTotPrice;
                        var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobOrder.JobTypeId);
                        var client = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);

                    state = "";
                        if (jobPhase!=null&& jobPhase != ProductionDepartment.JobFollowup.ToString())
                        {
                            var jobStatus = db.JobStatuses.Where(q => q.JobId == jobOrder.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                            if(jobStatus!=null)
                            {
                                state = jobStatus.Status.ToString();                                                         
                                if(jobPhase != ProductionDepartment.Graphic.ToString())
                                {
                                    var jobLineItems = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList();                                  
                                    if (state != JobState.Received.ToString())
                                    {                
                                    if(progressStatus!=null)
                                    {
                                        quantity = quantity-jobLineItems.Sum(s => s.Quantiy);
                                        totalPrice = totalPrice-jobLineItems.Sum(s => s.TotalPrice);

                                        state = progressStatus;
                                        status = state;

                                    }
                                    else
                                    {
                                        quantity = jobLineItems.Sum(s => s.Quantiy);
                                        totalPrice = jobLineItems.Sum(s => s.TotalPrice);
                                    }                                                                                     
                                }
                            }                                                                      
                        }

                        else
                        {
                            continue;
                        }
                              
                    }
                    else
                    {
                        state = JobOrderStatus.Received.ToString();
                    }
                        job.JobNo = jobOrder?.JobNo;
                        job.JobType = jobType?.JobName;
                        job.Client = client?.TradeName;
                        job.Quantity = quantity;
                        job.Copy = jobOrder.Pages.ToString() ;
                        job.Size = jobOrder.FinishingSize + "x" + jobOrder.Copies;
                        job.TotalPrice = totalPrice;
                        job.Code = jobType?.JobCode;
                        job.Status = status!=null?status:state;
                        jobList.Add(job);
                    }
                    model.JobTypeItemVMs = jobList;
                }
            


            return model;
        }
    }
}
