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
    public class PrintingAndFinishingTeamPerformanceWpODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProductionPerformanceVM PrintingAndFinishingTeamPerformancesWp(string yearlyProductionPlanId, string halfYearProductionPlanId, string quarterYearProductionPlanId, string monthlyProductionPlanId, string weeklyProductionPlanId, string jobTypeProductionPlanId,string jobTypeId)
        {
          
            var model = new ProductionPerformanceVM();
            //var previousJobs = new List<Job>();
            //var latestJobs = new List<Job>();
            int.TryParse(weeklyProductionPlanId, out int weeklyPlanId);
            int.TryParse(monthlyProductionPlanId, out int monthlyPlanId);
            int.TryParse(quarterYearProductionPlanId, out int quarterYearPlanId);
            int.TryParse(halfYearProductionPlanId, out int halfYearPlanId);
            int.TryParse(yearlyProductionPlanId, out int yearlyPlanId);
            DateTime? startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now;
            var productionPlans = new List<ProductionJobTypePlan>();
            if (weeklyProductionPlanId != null)
            {
                var weeklyPlan = db.WeeklyProductionPlans.Find(weeklyPlanId);
                if (weeklyPlan != null)
                {
                    startDate = weeklyPlan.StartDate;
                    endDate = weeklyPlan.EndDate;
                    productionPlans = db.ProductionJobTypePlans.Where(q => q.WeeklyProductionPlanId == weeklyPlanId).ToList();
                }
            }
            else if (monthlyProductionPlanId != null)
            {
                var monthlyPlan = db.MonthlyProductionPlans.Find(monthlyPlanId);
                if (monthlyPlan != null)
                {
                    startDate = monthlyPlan.StartDate;
                    endDate = monthlyPlan.EndDate;
                    productionPlans = db.ProductionJobTypePlans.Where(q => q.WeeklyPlan.MonthlyPlan.MonthlyProductionPlanId == monthlyPlanId).ToList();

                }
            }
            else if (quarterYearProductionPlanId != null)
            {
                var quarterlyPlan = db.QuarterlyProductionPlans.Find(quarterYearPlanId);
                if (quarterlyPlan != null)
                {
                    startDate = quarterlyPlan.StartDate;
                    endDate = quarterlyPlan.EndDate;
                    productionPlans = db.ProductionJobTypePlans.Where(q => q.WeeklyPlan.MonthlyPlan.QuarterlyProductionPlan.QuarterlyProductionPlanId == quarterYearPlanId).ToList();

                }
            }
            else if (halfYearProductionPlanId != null)
            {
                var halfYearlyPlan = db.HalfYearlyProductionPlans.Find(halfYearPlanId);
                if (halfYearlyPlan != null)
                {
                    startDate = halfYearlyPlan.StartDate;
                    endDate = halfYearlyPlan.EndDate;
                    productionPlans = db.ProductionJobTypePlans.Where(q => q.WeeklyPlan.MonthlyPlan.QuarterlyProductionPlan.HalfYearlyProductionPlan.HalfYearlyProductionPlanId == halfYearPlanId).ToList();

                }
            }
            else if (yearlyProductionPlanId != null)
            {
                var yearlyPlan = db.YearlyProductionPlans.Find(yearlyPlanId);
                if (yearlyPlan != null)
                {
                    startDate = yearlyPlan.StartDate;
                    endDate = yearlyPlan.EndDate;
                    productionPlans = db.ProductionJobTypePlans.Where(q => q.WeeklyPlan.MonthlyPlan.QuarterlyProductionPlan.HalfYearlyProductionPlan.YearlyPlan.YearlyProductionPlanId == yearlyPlanId).ToList();
                }
            }

            endDate = endDate.Value.AddDays(300);

            if (jobTypeId != null)
                {
                    
                }

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
                decimal monthlyPlans = 0;
                decimal totalPlan = 0;
                decimal lastWeekPendingJobs = 0;
                decimal currentWeekFinishedJobs = 0;
                decimal currentWeekOrderedJobs = 0;                        
                model.From = startDate?.ToString("dd/MM/yyyy");
                model.To = endDate?.ToString("dd/MM/yyyy");
            var jobTypes = db.JobCategories.Where(q => !q.HaveParent).ToList();
            if (jobTypeProductionPlanId != null)
            {
                var jobTypePlan = db.ProductionJobTypePlans.Where(q => q.ProductionJobTypePlanId.ToString() == jobTypeProductionPlanId).FirstOrDefault();
                if (jobTypePlan != null)
                {
                    jobTypes = jobTypes.Where(q => q.JobCategoryId == jobTypePlan.JobType.JobCategoryId).ToList();
                }
            }
            var items = new List<PrintingAndFinishingTeamPerformanceItem>();
            foreach (var jobType in jobTypes)
            {
                var item = new PrintingAndFinishingTeamPerformanceItem();
                decimal lastWeekFinishedJobTypes = 0;
                decimal lastWeekOrderedJobTypes = 0;
                decimal lastWeekPendingJobTypes = 0;
                decimal currentWeekFinishedJobTypes = 0;
                decimal currentWeekOrderedJobTypes = 0;

                var jobOrders = db.Jobs.Where(q => q.JobTypeId == jobType.JobCategoryId).ToList();
              
                var jobOrdeIds = jobOrders.Select(s=>s.JobId).Distinct();
                var deliveredJobStatusIds = db.JobStatuses.Where(q => jobOrdeIds.Contains(q.JobId)
                &&q.JobPhase == ProductionDepartment.Finishing
                && (q.Status == JobState.ParitiallyFinished || q.Status == JobState.Finished)).ToList().Select(s=>s.JobStatusId).Distinct();

                var orderedJobStatusIds = db.JobStatuses.Where(q => jobOrdeIds.Contains(q.JobId)
                && q.JobPhase == ProductionDepartment.Printing)
                .ToList().Select(s => s.JobStatusId).Distinct();
                var jobLineItems = db.JobStatusLineItems.ToList();

                List<JobStatusLineItem> deliveredJobs = jobLineItems.Where(q =>
                               q.Date.Date >= startDate.Value.Date)
                               .Where(q => q.Date.Date <= endDate.Value.Date)
                               .Where(q => deliveredJobStatusIds.Contains(q.JobStatusId)  )                           //
                                .ToList();
                List<JobStatusLineItem> orderedJobs = jobLineItems.Where(q =>
                               q.Date.Date >= startDate.Value.Date
                               && q.Date.Date <= endDate.Value.Date
                               &&  orderedJobStatusIds.Contains(q.JobStatusId)                         //
                               ).ToList();
                List<JobStatusLineItem> lastWeekDeliveredJobs = jobLineItems.Where(q =>
                               q.Date.Date <= startDate.Value.Date
                               && deliveredJobStatusIds.Contains(q.JobStatusId)                          //
                               ).ToList();
                List<JobStatusLineItem> lastWeekOrderedJobs = jobLineItems.Where(q =>
                             q.Date.Date <= startDate.Value.Date
                               && orderedJobStatusIds.Contains(q.JobStatusId)                         //
                             ).ToList();
                lastWeekOrderedJobTypes = lastWeekOrderedJobs.Sum(s => s.JobStatus.JobOrder.Quantity);
                currentWeekOrderedJobTypes = orderedJobs.Sum(s => s.JobStatus.JobOrder.Quantity);
                currentWeekFinishedJobTypes = deliveredJobs.Sum(s => s.Quantiy);
                lastWeekFinishedJobTypes = lastWeekDeliveredJobs.Sum(s => s.Quantiy);
                lastWeekPendingJobTypes = lastWeekOrderedJobTypes - lastWeekFinishedJobTypes;
                var jobTypePlans = productionPlans.Where(q => q.JobTypeId == jobType.JobCategoryId).ToList();
             
                item.JobType = jobType.JobName;
                item.LastWeekPendingJobs = Math.Round(lastWeekPendingJobTypes, 2);
                item.CurrentWeekOrderedJobs = Math.Round(currentWeekOrderedJobTypes, 2);
                item.Total = item.LastWeekPendingJobs + item.CurrentWeekOrderedJobs;
                item.WeeklyPlan = Math.Round((decimal)jobTypePlans.Sum(s=>s.ProductionPlanQuantity), 2);
                item.WeeklyPerformance = currentWeekFinishedJobTypes;
                item.ProductionInProgress = Math.Round(item.Total - currentWeekFinishedJobTypes, 2);
               // item.Percent = Math.Round(item.WeeklyPerformance / item.WeeklyPlan, 2) * 100;
                items.Add(item);
                totalPlan += item.WeeklyPlan;
                currentWeekOrderedJobs += currentWeekOrderedJobTypes;
                lastWeekPendingJobs += lastWeekPendingJobTypes;
                currentWeekFinishedJobs += currentWeekFinishedJobTypes;
                currentWeekOrderedJobs += currentWeekOrderedJobTypes;
                currentWeekOrderedJobs += currentWeekOrderedJobTypes;

            }
                    model.LastWeekPendingJobsBirr = Math.Round(lastWeekPendingJobs, 0);
                    model.LastWeekPendingJobsCent = getDecimalPartNo(lastWeekPendingJobs);
                    model.CurrentWeekJobsBirr = Math.Round(currentWeekOrderedJobs, 0);
                    model.CurrentWeekJobsCent = getDecimalPartNo(currentWeekOrderedJobs);
                    model.TotalBirr = Math.Round((currentWeekOrderedJobs + lastWeekPendingJobs), 0);
                    model.TotalCent = getDecimalPartNo(model.TotalBirr);
                    model.WeeklyProductionPlanBirr = Math.Round(totalPlan, 0);
                    model.WeeklyProductionPlanCent = getDecimalPartNo(totalPlan);
                    model.WeeklyProductionPerformanceBirr = Math.Round(currentWeekFinishedJobs, 0);
                    model.WeeklyProductionPerformanceCent = getDecimalPartNo(currentWeekFinishedJobs);
                    model.PendingPlanBirr = Math.Round((totalPlan - currentWeekFinishedJobs), 0);
                    model.PendingPlanCent = getDecimalPartNo(model.PendingPlanBirr);
                   // model.Percent = Math.Round((model.WeeklyProductionPerformanceBirr / model.WeeklyProductionPlanBirr), 2) * 100;
                    model.PrintingAndFinishingTeamPerformanceItems = items;
                   
                   return model;

        }

        public int  getDecimalPartNo(decimal number)
        {
            return (int)(Math.Round(number, 2) % 1 * 100);
        }
    }
}
