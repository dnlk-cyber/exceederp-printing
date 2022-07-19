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
    public class DeliveryTeamPerformanceODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintingAndFinishingTeamPerformanceVM DeliveryTeamPerformances(DateTime? startDate, DateTime? endDate, string jobTypeId)
        {
            string jobPhase = ProductionDepartment.FinishingDelivery.ToString();
            var deliverdProducts = db.ProductDeliveries.ToList();

            var finishingDeliveryPreviousJobStatusLists = db.JobStatuses.Where(q => q.JobPhase == ProductionDepartment.FinishingDelivery).ToList();
            var salesDeliveryPreviousJobStatusLists = db.JobStatuses.Where(q=>q.JobPhase== ProductionDepartment.SalesDelivery).ToList();
      
            var latestJobs = new List<Job>();
            var prevJobs = new List<Job>();
            var finishingDeliveryLatestJobStatusLists =new  List<JobStatus>();
            var salesDeliveryLatestJobStatusLists = new List<JobStatus>();
            if (startDate.HasValue)
            {
                finishingDeliveryPreviousJobStatusLists = finishingDeliveryPreviousJobStatusLists.Where(q => q.StartDate.HasValue ? q.StartDate.Value.Date < startDate.Value.Date:false).ToList();
                salesDeliveryPreviousJobStatusLists = salesDeliveryPreviousJobStatusLists.Where(q => q.StartDate.HasValue ? q.StartDate.Value.Date < startDate.Value.Date:false).ToList();
            }
   

            if (startDate.HasValue&& endDate.HasValue)
            {
                finishingDeliveryLatestJobStatusLists = finishingDeliveryPreviousJobStatusLists.Where(q => q.StartDate.HasValue ? q.StartDate.Value.Date >= startDate.Value.Date : false
                                                                          && q.EndDate.HasValue ? q.EndDate.Value.Date <= endDate.Value.Date : false).ToList();
                salesDeliveryLatestJobStatusLists = salesDeliveryPreviousJobStatusLists.Where(q => q.StartDate.HasValue ? q.StartDate.Value.Date >= startDate.Value.Date : false
                                                                          && q.EndDate.HasValue ? q.EndDate.Value.Date <= endDate.Value.Date : false).ToList();
                deliverdProducts = db.ProductDeliveries.ToList().Where(q => q.DeliveryDate.HasValue ? q.DeliveryDate.Value.Date >= startDate.Value.Date : false
                                                                          && q.DeliveryDate.HasValue ? q.DeliveryDate.Value.Date <= endDate.Value.Date : false
                                                                          ).ToList();
            }
            var delProIds = deliverdProducts.Select(s => s.ProductDeliveryId);
            var jobIds = db.ProductDeliveryItems.Where(q => delProIds.Contains(q.ProductDeliveryId)).ToList().Select(s=>s.JobId).Distinct();
            latestJobs = db.Jobs.Where(q => jobIds.Contains(q.JobId)).ToList();
            var prevIds = salesDeliveryPreviousJobStatusLists.Select(s=>s.JobId);
            prevJobs = db.Jobs.Where(q => prevIds.Contains(q.JobId)).ToList();
            if (jobTypeId != null)
            {

                prevJobs = prevJobs.Where(q => q.JobTypeId.ToString() == jobTypeId).ToList();
                latestJobs = latestJobs.Where(q => q.JobTypeId.ToString() == jobTypeId).ToList();
            }
            var jobTypeIds = latestJobs.ToList().Select(s => s.JobTypeId).Distinct();

            var latestJobsPrices = latestJobs.Sum(s=>s.Quantity);
            var model = new PrintingAndFinishingTeamPerformanceVM();

            decimal lastWeekFinishedJobs = 0;
            decimal currentWeekFinishedJobs = 0;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            Image LogoFileName = null;
            string AmharicName = "";
            string Name = "";
            if (organization!=null)
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
            model.Date = DateTime.Now.ToString("MM/dd/yyyy");

            var monthlyPlans  = new List<ProductionMonthlyPlan>();
               
               
                model.TotalClient = deliverdProducts.Select(s => s.CustomerId).Count().ToString();
                model.DeliveryFrom = deliverdProducts.FirstOrDefault()?.ProductDeliveryId.ToString();
                model.DeliveryTo = deliverdProducts.LastOrDefault()?.ProductDeliveryId.ToString();
                var voidedProductDeliveryItemss = deliverdProducts.Where(q => q.DeliveryStatus == DeliveryStatus.Voided).Select(s => s.ProductDeliveryId);
                model.CancelledDelivery = string.Join(",", voidedProductDeliveryItemss);


                model.From = startDate?.ToString("dd/MM/yyyy"); 
                model.To = endDate?.ToString("dd/MM/yyyy"); 
                
                var items = new List<PrintingAndFinishingTeamPerformanceItem>();
                foreach (var jobType in jobTypeIds)
                {
                
                            var proMonthlyPlans = monthlyPlans.Select(s => s.ProductionMonthlyPlanId);
                            var jobTypePlans = db.ProductionMonthlyJobTypePlans.Where(q => proMonthlyPlans.Contains(q.ProductionMonthlyPlanId)
                            && q.JobTypeId == jobType).ToList();
                            //var plans = db.ProductionMonthlyPlans.FirstOrDefault();
                            var jobTypeRec = db.JobCategories.Find(jobType);
                            var lastWeekJobTypes = prevJobs.Where(q => q.JobTypeId == jobType).ToList();
                            var currentWeekJobTypes = latestJobs.Where(q => q.JobTypeId == jobType).ToList();
                            decimal lastWeekFinishedJobTypes = 0;
                            decimal lastWeekSalesDeliveredJobTypes = 0;
                            decimal lastWeekPendingJobTypes = 0;
                            decimal currentWeekFinishedJobTypes = 0;
                            decimal currentWeekOrderedJobTypes = 0;
                            foreach (var lastJob in lastWeekJobTypes)
                            {
                                var jobStatus = db.JobStatuses.Where(q => q.JobId == lastJob.JobId && q.JobPhase.ToString() == ProductionDepartment.FinishingDelivery.ToString()).FirstOrDefault();
                                if (jobStatus != null)
                                {
                                    var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.Quantiy);
                                    lastWeekFinishedJobTypes += finished;
                      
                                }
                                 jobStatus = db.JobStatuses.Where(q => q.JobId == lastJob.JobId && q.JobPhase.ToString() == ProductionDepartment.SalesDelivery.ToString()).FirstOrDefault();
                                if (jobStatus != null)
                                {

                                    var delivered = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.Quantiy);
                                    lastWeekSalesDeliveredJobTypes += delivered;
                                }
                }
                            foreach (var currentJob in currentWeekJobTypes)
                            {
                                var jobStatus = db.JobStatuses.Where(q => q.JobId == currentJob.JobId && q.JobPhase.ToString() == ProductionDepartment.SalesDelivery.ToString()).FirstOrDefault();
                                if (jobStatus != null)
                                {
                                     var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.Quantiy);
                                    currentWeekFinishedJobTypes += finished;
                                }
                                var jobStatuses = db.JobStatuses.Where(q => q.JobId == currentJob.JobId && q.JobPhase.ToString() == ProductionDepartment.FinishingDelivery.ToString()).FirstOrDefault();
                                if (jobStatuses != null)
                                {
                                    currentWeekOrderedJobTypes += currentJob.Quantity;
                                }
                            }
                            var jobTypeIdsss = lastWeekJobTypes.ToList().Select(s => s.JobId);
                            var changedQuan = db.ChangeOrders.Where(q => jobTypeIdsss.Contains(q.JobId)).ToList().Sum(s => s.Total);
                            lastWeekPendingJobTypes = lastWeekFinishedJobTypes-lastWeekSalesDeliveredJobTypes;
                            var item = new PrintingAndFinishingTeamPerformanceItem();
                            var jobC = jobTypeRec?.JobCode;
                            var jobN = jobTypeRec?.JobName;
                            if (jobTypeRec.HaveParent)
                            {
                                var parentRec = db.JobCategories.Find(jobTypeRec.ParentId);
                                jobC = parentRec?.JobCode;
                                jobN = parentRec?.JobName;
                            }
                            item.JobCode = jobC;
                            item.JobType = jobN;
                            item.LastWeekPendingJobs = Math.Round(lastWeekPendingJobTypes, 2);
                            item.CurrentWeekOrderedJobs = Math.Round(currentWeekOrderedJobTypes, 2);
                            item.Total = item.LastWeekPendingJobs + item.CurrentWeekOrderedJobs;
                            item.WeeklyPlan = 0;
                            item.WeeklyPerformance = currentWeekFinishedJobTypes;
                            item.ProductionInProgress = Math.Round(item.Total - currentWeekFinishedJobTypes, 2);

                            if (item.WeeklyPlan!=0)
                            {
                                item.Percent = Math.Round(item.WeeklyPerformance / item.WeeklyPlan, 2) * 100;

                            }
                            items.Add(item);
                      
                model.PrintingAndFinishingTeamPerformanceItems = items;
            }
            

            return model;

        }

        public int  getDecimalPartNo(decimal number)
        {
            return (int)(Math.Round(number, 2) % 1 * 100);
        }
    }
}
