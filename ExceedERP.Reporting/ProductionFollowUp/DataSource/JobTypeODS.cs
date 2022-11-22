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
             List<int> jobListItemIds = new List<int>();
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
                    jobListItemIds = db.JobStatusLineItems.ToList().Where(x => x.Date.Date >= startDate.Value.Date && x.Date.Date <= endDate.Value.Date).Select(s => s.JobStatusId).ToList();
                    jobLists = jobLists.Where(q => q.Status.ToString() != JobState.Received.ToString()).Where(q => jobListItemIds.Contains(q.JobStatusId)).ToList();
                }
                else if (status == JobOrderStatus.OnProgress.ToString())
                {
                     jobListItemIds = db.JobStatusLineItems.ToList().Where(x => x.Date.Date >= startDate.Value.Date && x.Date.Date <= endDate.Value.Date).Select(s => s.JobStatusId).ToList();
                    jobLists = jobLists.Where(q => jobListItemIds.Contains(q.JobStatusId)).ToList();
                }
                else if(status == JobOrderStatus.Received.ToString()&& jobPhase != ProductionDepartment.Printing.ToString())
                {
                  
                     if (jobPhase == ProductionDepartment.Finishing.ToString())
                    {
                        jobLists = db.JobStatuses.Where(q => q.JobPhase.ToString() == ProductionDepartment.Printing.ToString()).ToList();
                       
                    }
                    else if (jobPhase == ProductionDepartment.FinishingDelivery.ToString())
                    {
                        jobLists = db.JobStatuses.Where(q => q.JobPhase.ToString() == ProductionDepartment.Finishing.ToString()).ToList();
                       
                    }
                    else if (jobPhase == ProductionDepartment.SalesDelivery.ToString())
                    {
                        jobLists = db.JobStatuses.Where(q => q.JobPhase.ToString() == ProductionDepartment.FinishingDelivery.ToString()).ToList();
                     
                    }
                    jobListItemIds = db.JobStatusLineItems.ToList().Where(x => x.Date.Date >= startDate.Value.Date && x.Date.Date <= endDate.Value.Date).Select(s => s.JobStatusId).ToList();
                    jobLists = jobLists.Where(q => jobListItemIds.Contains(q.JobStatusId)).ToList();
                }
                else if (status!=JobOrderStatus.Received.ToString())
                {
                     jobListItemIds = db.JobStatusLineItems.ToList().Where(x => x.Date.Date >= startDate.Value.Date && x.Date.Date <= endDate.Value.Date).Select(s => s.JobStatusId).ToList();
                    jobLists = jobLists.Where(q => jobListItemIds.Contains(q.JobStatusId)).ToList();
                }
                else
                {
                    jobLists = jobLists.ToList().Where(x => x.StartDate.Value.Date >= startDate.Value.Date && x.StartDate.Value.Date <= endDate.Value.Date).ToList();

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
                    var receQua = quantity;
                    decimal totalPrice = jobOrder.TotalPrice + chgTotPrice;
                    var jobItems = db.JobStatusLineItems.ToList().Where(q=>q.Date.Date >= startDate.Value.Date && q.Date.Date <= endDate.Value.Date).ToList();

                    if (status == JobOrderStatus.OnProgress.ToString()||status == JobOrderStatus.Received.ToString())
                    {
                        if (jobPhase == ProductionDepartment.Finishing.ToString())
                        {
                            var jobStatusIds = db.JobStatuses.Where(q => q.JobPhase.ToString() == ProductionDepartment.Printing.ToString()&&q.JobId==jobOrder.JobId).ToList().Select(s=>s.JobStatusId);
                            var totalOrders = jobItems.Where(q => jobStatusIds.Contains(q.JobStatusId)).ToList();
                            quantity = totalOrders.Sum(s => s.Quantiy);
                            totalPrice = totalOrders.Sum(s => s.TotalPrice);
                        }
                        else if (jobPhase == ProductionDepartment.FinishingDelivery.ToString())
                        {
                            var jobStatusIds = db.JobStatuses.Where(q => q.JobPhase.ToString() == ProductionDepartment.Finishing.ToString() && q.JobId == jobOrder.JobId).ToList().Select(s => s.JobStatusId);
                            var totalOrders = jobItems.Where(q => jobStatusIds.Contains(q.JobStatusId)).ToList();
                            quantity = totalOrders.Sum(s => s.Quantiy);
                            totalPrice = totalOrders.Sum(s => s.TotalPrice);
                        }
                        else if (jobPhase == ProductionDepartment.SalesDelivery.ToString())
                        {
                            var jobStatusIds = db.JobStatuses.Where(q => q.JobPhase.ToString() == ProductionDepartment.FinishingDelivery.ToString() && q.JobId == jobOrder.JobId).ToList().Select(s => s.JobStatusId);
                            var totalOrders = jobItems.Where(q => jobStatusIds.Contains(q.JobStatusId)).ToList();
                            quantity = totalOrders.Sum(s => s.Quantiy);
                            totalPrice = totalOrders.Sum(s => s.TotalPrice);
                        }

                        if(quantity==0)
                        {
                            continue;
                        }
                    }
                
                    var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobOrder.JobTypeId);
                        var client = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);

                    string  state = "";
                    string  selectedStatus = "";
                    if (jobPhase != null && jobPhase != ProductionDepartment.JobFollowup.ToString())
                    {
                        var jobStatus = db.JobStatuses.Where(q => q.JobId == jobOrder.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                        if (jobStatus != null)
                        {

                            selectedStatus = jobStatus.Status.ToString();
                            if (jobPhase != ProductionDepartment.Graphic.ToString())
                            {
                                var jobLineItems = db.JobStatusLineItems.ToList().Where(q => q.JobStatusId == jobStatus.JobStatusId && q.Date.Date >= startDate.Value.Date && q.Date.Date <= endDate.Value.Date).ToList();
                                if (selectedStatus != JobState.Received.ToString() && status != JobState.Received.ToString())
                                {
                                    if (jobLineItems.Any())
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
                                            //if (selectedStatus == JobOrderStatus.Finished.ToString())
                                            //{
                                                state = selectedStatus;
                                                if (quantity != finishedQua)
                                                {
                                                if (status == JobState.Finished.ToString())
                                                {
                                                    continue;
                                                }
                                                state = JobOrderStatus.ParitiallyFinished.ToString();
                                                }
                                                else
                                                {
                                                    if (status == JobState.ParitiallyFinished.ToString())
                                                    {
                                                        continue;
                                                    }
                                                }

                                            //}
                                            //else
                                            //{
                                            //    state = JobOrderStatus.ParitiallyFinished.ToString();

                                            //}
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

                                }

                            }
                        }

                        //else
                        //{
                        //    continue;
                        //}

                    }
                    else
                    {
                        state = JobOrderStatus.Received.ToString();
                    }
                    if(quantity<0)
                    {
                        continue;
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
