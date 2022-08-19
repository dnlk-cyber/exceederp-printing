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
    public class JobSupplyandSalesWpODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobSupplyandSalesVM GetJobSupplyandSalesWpODS(string yearlyProductionPlanId, string halfYearProductionPlanId, string quarterYearProductionPlanId, string monthlyProductionPlanId, string weeklyProductionPlanId, string jobTypeProductionPlanId)
        {
            var model = new JobSupplyandSalesVM();
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
            decimal days = 0;
            var previousJobs = new List<Job>();
            decimal WeeklySupplyAndSalesPlan = 0;
            decimal WeeklyReceivedActual = 0;
            decimal WeeklySalesActual = 0;
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

             previousJobs = db.Jobs.ToList().Where(q => q.ReceivedDate.HasValue ? q.ReceivedDate.Value.Date < startDate.Value.Date : false).ToList();

            var jobs = db.Jobs.ToList().Where(x => x.ReceivedDate.HasValue ? x.ReceivedDate.Value.Date >= startDate.Value.Date
              && x.ReceivedDate.Value.Date <= endDate.Value.Date : false).ToList();

            model.From = startDate?.ToString("dd/MM/yyyy");
            model.To = endDate?.ToString("dd/MM/yyyy");
            model.PreparedBy = "";
            model.CheckedBy = "";
            model.ApprovedBy = "";
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

            List<JobSupplyandSalesItemVM> jobList = new List<JobSupplyandSalesItemVM>();
            foreach (var jobType in jobTypes)
            {
                var job = new JobSupplyandSalesItemVM();
                var jobTS = jobs.Where(q => q.JobTypeId == jobType.JobCategoryId).ToList();
                var jobIds = jobTS.Select(s => s.JobId);
                var changeOrderQuan = db.ChangeOrders.Where(q => jobIds.Contains(q.JobId)).ToList().Sum(q => q.Quantity);
                previousJobs = previousJobs.Where(q => q.JobTypeId == jobType.JobCategoryId).ToList();
                var prevJobIds = previousJobs.Select(s => s.JobId);
                decimal lastWeekFinishedJobs = 0;
                var jobStatus = db.JobStatuses.ToList().Where(q => prevJobIds.Contains(q.JobId) && q.JobPhase == ProductionDepartment.SalesDelivery).FirstOrDefault();
                if (jobStatus != null)
                {
                    var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.Quantiy);
                    lastWeekFinishedJobs += finished;
                }
     
                var deliveredJobs = db.JobStatusLineItems.ToList().Where(q =>
                                q.Date.Date >= startDate.Value.Date
                                && q.Date.Date <= endDate.Value.Date
                                &&q.JobStatus.JobOrder.JobTypeId==jobType.JobCategoryId
                               && q.JobStatus.JobPhase==ProductionDepartment.SalesDelivery
                               && (q.JobStatus.Status==JobState.ParitiallyFinished|| q.JobStatus.Status == JobState.Finished)
                                ).ToList();

                var jobTypePlans = productionPlans.Where(q =>q.JobTypeId == jobType.JobCategoryId).ToList();
             
                    //  job.DailySupply = Math.Round((decimal)jobTypePlan.TotalProduct / days, 2);
                    var jobtotalPlan = jobTypePlans.Sum(q => q.ProductionPlanQuantity);
                    job.PreviousWeeklySupply = Math.Round(previousJobs.Sum(s => s.Quantity) - lastWeekFinishedJobs, 2);
                    job.WeeklySupplyAndSalesPlan = Math.Round((decimal)jobtotalPlan, 2);
                    job.WeeklyReceivedActual = Math.Round(jobTS.Sum(s => s.Quantity) + changeOrderQuan, 2);
                    job.WeeklySupplyPerformance = Math.Round(job.WeeklySupplyAndSalesPlan - job.WeeklyReceivedActual, 2);
                    job.WeeklySalesActual = Math.Round(deliveredJobs.Sum(s => s.Quantiy), 2);
                    job.WeeklySalesPerformance = Math.Round(job.WeeklySupplyAndSalesPlan - job.WeeklySalesActual, 2);
                    WeeklySupplyAndSalesPlan += job.WeeklySupplyAndSalesPlan;
                    WeeklyReceivedActual += job.WeeklyReceivedActual;
                    WeeklySalesActual += job.WeeklySalesActual;                


                job.Code = jobType?.JobCode;
                jobList.Add(job);
            }
            model.WeeklySupplyAndSalesPlan = Math.Round(WeeklySupplyAndSalesPlan, 2);
            model.WeeklyReceivedActual = Math.Round(WeeklyReceivedActual, 2);
            model.WeeklySupplyPerformance = Math.Round(model.WeeklySupplyAndSalesPlan - model.WeeklyReceivedActual, 2);
            model.WeeklySalesActual = Math.Round(WeeklySalesActual, 2);
            model.WeeklySalesPerformance = Math.Round(WeeklySupplyAndSalesPlan - WeeklySalesActual, 2);
            model.JobSupplyandSalesItemVMs = jobList;


        


            return model;
        }
    }
}

