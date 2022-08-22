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
    public class PrintingAndFinishingTeamPerformanceODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProductionPerformanceVM PrintingAndFinishingTeamPerformances(DateTime? startDate, DateTime? endDate, string jobPhase, string jobTypeId)
        {
            var previousJobs = new List<JobStatus>();
            var latestJobs = new List<JobStatus>();

            if (startDate.HasValue)
            {
                previousJobs = db.JobStatuses.ToList().Where(q => q.StartDate.HasValue ? q.StartDate.Value.Date < startDate.Value.Date:false).ToList();
            }
            var days = 0;

            if (startDate.HasValue&& endDate.HasValue)
            {
               latestJobs = db.JobStatuses.ToList().Where(q => (q.StartDate.HasValue && q.EndDate.HasValue) ? q.StartDate.Value.Date >= startDate.Value.Date
                                                        && q.EndDate.Value.Date <= endDate.Value.Date:false
                                                        ).ToList();
                days = (endDate.Value.Date - startDate.Value.Date).Days;

            }
            if(jobTypeId!=null)
            {
                var jobIds = db.Jobs.Where(q => q.JobTypeId.ToString() == jobTypeId).Select(s => s.JobId).ToList();
                latestJobs = latestJobs.Where(q => jobIds.Contains(q.JobId)).ToList();
                previousJobs = previousJobs.Where(q => jobIds.Contains(q.JobId)).ToList();
            }

            var model = new ProductionPerformanceVM();
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
            decimal lastWeekFinishedJobs = 0;
            decimal lastWeekPendingJobs = 0;
            model.From = startDate?.ToString("dd/MM/yyyy");
            model.To = endDate?.ToString("dd/MM/yyyy");
            decimal currentWeekFinishedJobs = 0;
            decimal currentWeekOrderedJobs = 0;

                foreach (var lastJob in previousJobs)
                {
                    var jobStatus = db.JobStatuses.Where(q => q.JobId == lastJob.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                    if (jobStatus != null)
                    {
                        var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.Quantiy);
                        lastWeekFinishedJobs += finished;
                    }
                }
                foreach (var currentJob in latestJobs)
                {
                    var jobStatus = db.JobStatuses.Where(q => q.JobId == currentJob.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                    if (jobStatus != null)
                    {
                        var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.Quantiy);
                        currentWeekFinishedJobs += finished;
                    }
                }
                


                var period = db.GLPeriods.ToList().Where(q => (q.DateFrom.Date <= startDate.Value.Date && startDate.Value.Date <= q.DateTo.Date)
                              || (q.DateTo.Date >= endDate.Value.Date && endDate.Value.Date >= q.DateFrom.Date));

                var monthlyPlans = new List<ProductionMonthlyPlan>();
               
                var items = new List<PrintingAndFinishingTeamPerformanceItem>();
            var latJobOrderIds = latestJobs.Select(s => s.JobId);
            var preJobOrderIds = previousJobs.Select(s => s.JobId);
            var latestJobOrders = db.Jobs.Where(q => latJobOrderIds.Contains(q.JobId)).ToList();
            var preJobOrders = db.Jobs.Where(q => preJobOrderIds.Contains(q.JobId)).ToList();
            var jobTypeIds = latestJobOrders.Select(s => s.JobTypeId);
            var jobTypess = db.JobCategories.Where(q=> jobTypeIds.Contains(q.JobCategoryId)).ToList();
           

            foreach (var jobType in jobTypess)
                {
                            var proMonthlyPlans = monthlyPlans.Select(s => s.ProductionMonthlyPlanId);
                            var jobTypePlans = db.ProductionMonthlyJobTypePlans.Where(q => proMonthlyPlans.Contains(q.ProductionMonthlyPlanId)
                            && q.JobTypeId == jobType.JobCategoryId).ToList();
                            var jobTypeName = jobType.JobName;
                            var lastWeekJobTypes = preJobOrders.Where(q => q.JobTypeId == jobType.JobCategoryId).ToList();
                            var currentWeekJobTypes = latestJobOrders.Where(q => q.JobTypeId == jobType.JobCategoryId).ToList();
                decimal lastWeekFinishedJobTypes = 0;
                            decimal lastWeekPendingJobTypes = 0;
                            decimal currentWeekFinishedJobTypes = 0;
                            decimal currentWeekOrderedJobTypes = 0;

                            foreach (var lastJob in lastWeekJobTypes)
                            {
                                var jobStatus = db.JobStatuses.Where(q => q.JobId == lastJob.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                                if (jobStatus != null)
                                {
                                    var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.Quantiy);
                                    lastWeekFinishedJobTypes += finished;
                                }
                            }
                            foreach (var currentJob in currentWeekJobTypes)
                            {
                                var jobStatus = db.JobStatuses.Where(q => q.JobId == currentJob.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                                if (jobStatus != null)
                                {
                                    var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.Quantiy);
                                    currentWeekFinishedJobTypes += finished;

                                }
                            }
                            var currentJobIds = currentWeekJobTypes.Select(s => s.JobId);
                            var currentChangeOrderQuan = db.ChangeOrders.Where(q => currentJobIds.Contains(q.JobId)).ToList().Sum(s => s.Quantity);
                            currentWeekOrderedJobTypes = currentWeekJobTypes.Sum(q => q.Quantity) + currentChangeOrderQuan;
                            var prevJobIds = lastWeekJobTypes.Select(s => s.JobId);
                            var prevChangeOrderQuan = db.ChangeOrders.Where(q => prevJobIds.Contains(q.JobId)).ToList().Sum(s => s.Quantity);
                            currentWeekOrderedJobTypes = currentWeekJobTypes.Sum(q => q.Quantity) + prevChangeOrderQuan;
                            lastWeekPendingJobTypes = lastWeekJobTypes.Sum(s => s.Quantity) + prevChangeOrderQuan - lastWeekFinishedJobTypes;
                            var item = new PrintingAndFinishingTeamPerformanceItem();
                            item.JobType = jobTypeName;
                            item.LastWeekPendingJobs = Math.Round(lastWeekPendingJobTypes, 2);
                            item.CurrentWeekOrderedJobs = Math.Round(currentWeekOrderedJobTypes, 2);
                            item.Total = item.LastWeekPendingJobs + item.CurrentWeekOrderedJobs;
                            item.WeeklyPlan = 0;
                            item.WeeklyPerformance = currentWeekFinishedJobTypes;
                            item.ProductionInProgress = Math.Round(item.Total - currentWeekFinishedJobTypes, 2);
                            item.Percent = 0;
                            //item.Percent = Math.Round(item.WeeklyPerformance / item.WeeklyPlan, 2) * 100;
                            items.Add(item);
                currentWeekOrderedJobs += currentWeekOrderedJobTypes;
                lastWeekPendingJobs += lastWeekPendingJobTypes;


                }

                
              
                model.LastWeekPendingJobsBirr = Math.Round(lastWeekPendingJobs, 0);
                model.LastWeekPendingJobsCent = getDecimalPartNo(lastWeekPendingJobs);
                model.CurrentWeekJobsBirr = Math.Round(currentWeekOrderedJobs, 0);
                model.CurrentWeekJobsCent = getDecimalPartNo(currentWeekOrderedJobs);
            var total = currentWeekOrderedJobs + lastWeekPendingJobs;
                model.TotalBirr = Math.Round(total, 0);
                model.TotalCent = getDecimalPartNo(total);
                model.WeeklyProductionPlanBirr = 0;
                //model.WeeklyProductionPlanBirr = Math.Round(weekPlan, 0);
                model.WeeklyProductionPlanCent = 0;
                //model.WeeklyProductionPlanCent = getDecimalPartNo(weekPlan);
                model.WeeklyProductionPerformanceBirr = 0;
                model.WeeklyProductionPerformanceCent = 0;
                model.PendingPlanBirr = 0;
                model.PendingPlanCent = 0;
                model.Percent = 0 * 100;
                model.PrintingAndFinishingTeamPerformanceItems = items;
            
            

            return model;

        }

        public int  getDecimalPartNo(decimal number)
        {
            return (int)(Math.Round(number, 2) % 1 * 100);
        }
    }
}
