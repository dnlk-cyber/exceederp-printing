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
        public PrintingAndFinishingTeamPerformanceVM PrintingAndFinishingTeamPerformancesWp(string periodId, string jobPhase, string jobTypeId)
        {
            int.TryParse(periodId, out int periodIdd);
            var period = db.GLPeriods.Find(periodIdd);
          
            var model = new PrintingAndFinishingTeamPerformanceVM();

            if (period!=null)
            {
                DateTime? startDate = period.DateFrom;
                DateTime? endDate = period.DateTo;
                var previousJobs = db.Jobs.ToList();
                var latestJobs = db.Jobs.ToList();
                if (startDate.HasValue)
                {
                    previousJobs = previousJobs.Where(q => q.ReceivedDate.HasValue ? q.ReceivedDate.Value.Date < startDate.Value.Date : false).ToList();
                }
                var days = 0;

                if (startDate.HasValue && endDate.HasValue)
                {
                    latestJobs = latestJobs.Where(q => q.ReceivedDate.HasValue ? q.ReceivedDate.Value.Date >= startDate.Value.Date
                                                             && q.ReceivedDate.Value.Date <= endDate.Value.Date : false
                                                             ).ToList();
                    days = (endDate.Value.Date - startDate.Value.Date).Days;

                }
                if (jobTypeId != null)
                {
                    latestJobs = latestJobs.Where(q => q.JobTypeId.ToString() == jobTypeId).ToList();
                    previousJobs = previousJobs.Where(q => q.JobTypeId.ToString() == jobTypeId).ToList();
                }

                var latestJobsPrices = latestJobs.Sum(s => s.TotalPrice);
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
                decimal LastWeekFinishedJobs = 0;
                decimal LastWeekPendingJobs = 0;
                decimal currentWeekFinishedJobs = 0;

                if (days > 0)
                {
                    foreach (var lastJob in previousJobs)
                    {
                        var jobStatus = db.JobStatuses.Where(q => q.JobId == lastJob.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                        if (jobStatus != null)
                        {
                            var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.TotalPrice);
                            LastWeekFinishedJobs += finished;
                        }
                    }
                    foreach (var currentJob in latestJobs)
                    {
                        var jobStatus = db.JobStatuses.Where(q => q.JobId == currentJob.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                        if (jobStatus != null)
                        {
                            var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.TotalPrice);
                            currentWeekFinishedJobs += finished;
                        }
                    }



                    if (period != null)
                    {
                        var monthlyPlan = db.ProductionMonthlyPlans.FirstOrDefault(q => q.GLPeriodId == periodIdd);
                        if (monthlyPlan != null)
                        {
                            foreach (var currentJob in latestJobs)
                            {
                                var jobStatus = db.JobStatuses.Where(q => q.JobId == currentJob.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                                if (jobStatus != null)
                                {
                                    var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.TotalPrice);
                                    currentWeekFinishedJobs += finished;
                                }
                            }
                            var jobIds = previousJobs.ToList().Select(s=>s.JobId);
                            var changedQuan = db.ChangeOrders.Where(q => jobIds.Contains(q.JobId)).ToList().Sum(s=>s.Total);
                            LastWeekPendingJobs = previousJobs.Sum(s => s.TotalPrice) + changedQuan + - LastWeekPendingJobs;
                            model.From = startDate?.ToString("dd/MM/yyyy");
                            model.To = endDate?.ToString("dd/MM/yyyy"); 

                            model.LastWeekPendingJobsBirr = Math.Round(LastWeekPendingJobs, 0);
                            model.LastWeekPendingJobsCent = getDecimalPartNo(LastWeekPendingJobs);
                            model.CurrentWeekJobsBirr = Math.Round(latestJobsPrices, 0);
                            model.CurrentWeekJobsCent = getDecimalPartNo(latestJobsPrices);
                            model.TotalBirr = Math.Round((latestJobsPrices + LastWeekPendingJobs), 0);
                            model.TotalCent = getDecimalPartNo(model.TotalBirr);
                            var weekPlan = monthlyPlan.TotalPrice;
                            model.WeeklyProductionPlanBirr = Math.Round(weekPlan, 0);
                            model.WeeklyProductionPlanCent = getDecimalPartNo(weekPlan);
                            model.WeeklyProductionPerformanceBirr = Math.Round(currentWeekFinishedJobs, 0);
                            model.WeeklyProductionPerformanceCent = getDecimalPartNo(currentWeekFinishedJobs);
                            model.PendingPlanBirr = Math.Round((weekPlan - currentWeekFinishedJobs), 0);
                            model.PendingPlanCent = getDecimalPartNo(model.PendingPlanBirr);
                            model.Percent = Math.Round((model.WeeklyProductionPerformanceBirr / model.WeeklyProductionPlanBirr), 2) * 100;

                            var items = new List<PrintingAndFinishingTeamPerformanceItem>();
                            var jobTypeIds = latestJobs.ToList().Select(s => s.JobTypeId).Distinct();
                            foreach (var jobType in jobTypeIds)
                            {

                                var jobTypePlan = db.ProductionMonthlyJobTypePlans.Where(q => q.ProductionMonthlyPlanId == monthlyPlan.ProductionMonthlyPlanId
                                && q.JobTypeId == jobType).FirstOrDefault();
                                if (jobTypePlan != null)
                                {
                                    var jobTypeName = db.JobCategories.Find(jobType)?.JobName;
                                    var lastWeekJobTypes = previousJobs.Where(q => q.JobTypeId == jobType).ToList();
                                    var currentWeekJobTypes = latestJobs.Where(q => q.JobTypeId == jobType).ToList();
                                    decimal lastWeekFinishedJobTypes = 0;
                                    decimal lastWeekPendingJobTypes = 0;
                                    decimal currentWeekFinishedJobTypes = 0;
                                    foreach (var lastJob in lastWeekJobTypes)
                                    {
                                        var jobStatus = db.JobStatuses.Where(q => q.JobId == lastJob.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                                        if (jobStatus != null)
                                        {
                                            var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.TotalPrice);
                                            lastWeekFinishedJobTypes += finished;
                                        }
                                    }
                                    foreach (var currentJob in currentWeekJobTypes)
                                    {
                                        var jobStatus = db.JobStatuses.Where(q => q.JobId == currentJob.JobId && q.JobPhase.ToString() == jobPhase).FirstOrDefault();
                                        if (jobStatus != null)
                                        {
                                            var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.TotalPrice);
                                            currentWeekFinishedJobTypes += finished;
                                        }
                                    }
                                    var jobTypeIdsss = lastWeekJobTypes.ToList().Select(s => s.JobId);
                                     changedQuan = db.ChangeOrders.Where(q => jobTypeIdsss.Contains(q.JobId)).ToList().Sum(s => s.Total);
                                    lastWeekPendingJobTypes = lastWeekJobTypes.Sum(s => s.TotalPrice) + changedQuan - lastWeekFinishedJobTypes;
                                    var item = new PrintingAndFinishingTeamPerformanceItem();
                                    item.JobType = jobTypeName;
                                    item.LastWeekPendingJobs = Math.Round(lastWeekPendingJobTypes, 2);
                                    item.CurrentWeekOrderedJobs = Math.Round(currentWeekJobTypes.Sum(s => s.TotalPrice), 2);
                                    item.Total = item.LastWeekPendingJobs + item.CurrentWeekOrderedJobs;
                                    item.WeeklyPlan = Math.Round(jobTypePlan.TotalPrice, 2);
                                    item.WeeklyPerformance = currentWeekFinishedJobTypes;
                                    item.ProductionInProgress = Math.Round(item.Total - currentWeekFinishedJobTypes, 2);
                                    item.Percent = Math.Round(item.WeeklyPerformance / item.WeeklyPlan, 2) * 100;
                                    items.Add(item);
                                }



                            }
                            model.PrintingAndFinishingTeamPerformanceItems = items;
                        }
                    }


                }
            }
           
            

            return model;

        }

        public int  getDecimalPartNo(decimal number)
        {
            return (int)(Math.Round(number, 2) % 1 * 100);
        }
    }
}
