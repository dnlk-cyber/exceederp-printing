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
                var jobListItemIds = db.JobStatusLineItems.ToList().Where(x => x.Date.Date >= startDate.Value.Date&& x.Date.Date <= endDate.Value.Date).Select(s=>s.JobStatusId);
                jobLists = jobLists.Where(q => jobListItemIds.Contains(q.JobStatusId)).ToList();
                var jobIds = jobLists.Select(s=>s.JobId);
            jobs = db.Jobs.ToList().Where(q => jobIds.Contains(q.JobId)).ToList();
            }
            string progressStatus = null;
            if (status != null)
            {
                if (status== JobOrderStatus.OnProgress.ToString())
                {
                    progressStatus = JobOrderStatus.OnProgress.ToString();
                  
                   // jobLists = jobLists.ToList();
                }
                else if (status==JobState.ParitiallyFinished.ToString())
                {
                    jobLists = jobLists.ToList().Where(x => x.Status.ToString() == status|| x.Status.ToString()== JobState.Finished.ToString()).ToList();
                }
                else if (status == JobState.Finished.ToString())
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
                        var changeOfOrders = db.ChangeOrders.Where(q => q.JobId == jobOrder.JobId).ToList();
                        var chgQua = changeOfOrders.Sum(q => q.Quantity);
                        var chgTotPrice = changeOfOrders.Sum(q => q.Total);
                        decimal quantity = jobOrder.Quantity + chgQua;
                        decimal totalPrice = jobOrder.TotalPrice+ chgTotPrice;
                        var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobOrder.JobTypeId);
                        var client = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);

                    string  state = "";
                    string  selectedStatus = "";
                        if (jobPhase!=null&& jobPhase != ProductionDepartment.JobFollowup.ToString())
                        {
                            var jobStatus = db.JobStatuses.Where(q => q.JobId == jobOrder.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                            if(jobStatus!=null)
                            {

                            selectedStatus = jobStatus.Status.ToString();
                                if(jobPhase != ProductionDepartment.Graphic.ToString())
                                {
                                    var jobLineItems = db.JobStatusLineItems.ToList().Where(q => q.JobStatusId == jobStatus.JobStatusId&&q.Date.Date<=endDate.Value.Date).ToList();                                  
                                    if (selectedStatus != JobState.Received.ToString()&& status != JobState.Received.ToString())
                                    {    
                                    if(jobLineItems.Any())
                                    {
                                        if (progressStatus != null)
                                        {
                                            var finishedQua = jobLineItems.Sum(s => s.Quantiy);

                                            if (quantity != finishedQua)
                                            {
                                                state = progressStatus;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                            quantity = quantity - finishedQua;
                                            totalPrice = totalPrice - jobLineItems.Sum(s => s.TotalPrice);
                                        }
                                        else 
                                        {
                                            decimal finishedQua = jobLineItems.Sum(s => s.Quantiy);
                                            if (selectedStatus == JobOrderStatus.Finished.ToString())
                                            {
                                                state = selectedStatus;
                                                if (quantity > finishedQua)
                                                {
                                                    state = JobOrderStatus.ParitiallyFinished.ToString();
                                                }
                                                else
                                                {
                                                    if (status == JobState.ParitiallyFinished.ToString())
                                                    {
                                                        

                                                     continue;
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                state = JobOrderStatus.ParitiallyFinished.ToString();

                                            }
                                            quantity = finishedQua;
                                            totalPrice = jobLineItems.Sum(s => s.TotalPrice);
                                        }
                                    }
                                    else
                                    {
                                        if (progressStatus != null)
                                        {
                                            var finishedQua = jobLineItems.Sum(s => s.Quantiy);

                                            if (quantity != finishedQua)
                                            {
                                                state = progressStatus;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                            quantity = quantity - finishedQua;
                                            totalPrice = totalPrice - jobLineItems.Sum(s => s.TotalPrice);
                                        }
                                        else
                                        {
                                            continue;

                                        }
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
