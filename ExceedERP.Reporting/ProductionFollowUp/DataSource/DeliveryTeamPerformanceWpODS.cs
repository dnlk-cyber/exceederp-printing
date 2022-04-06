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
    public class DeliveryTeamPerformanceWpODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintingAndFinishingTeamPerformanceVM DeliveryTeamPerformancesWp(string periodId, string jobTypeId)
        {
            int.TryParse(periodId, out int periodIdd);
            var period = db.GLPeriods.Find(periodIdd);

            var model = new PrintingAndFinishingTeamPerformanceVM();

            if (period != null)
            {
                DateTime? startDate = period.DateFrom;
                DateTime? endDate = period.DateTo;
                string jobPhase = ProductionDepartment.FinishingDelivery.ToString();
                var previousJobs = db.Jobs.ToList();
                var latestJobs = db.Jobs.ToList();
                if (startDate.HasValue)
                {
                    previousJobs = previousJobs.Where(q => q.ReceivedDate.HasValue ? q.ReceivedDate.Value.Date < startDate.Value.Date : false).ToList();
                }
                var days = 0;

                if (startDate.HasValue && endDate.HasValue)
                {
                    latestJobs = latestJobs.Where(q => q.ReceivedDate.HasValue ? q.ReceivedDate.Value.Date >= startDate.Value.Date : false
                                                             && q.ReceivedDate.HasValue ? q.ReceivedDate.Value.Date <= endDate.Value.Date : false
                                                             ).ToList();
                    days = (endDate.Value.Date - startDate.Value.Date).Days;

                }
                if (jobTypeId != null)
                {
                    latestJobs = latestJobs.Where(q => q.JobTypeId.ToString() == jobTypeId).ToList();
                    previousJobs = previousJobs.Where(q => q.JobTypeId.ToString() == jobTypeId).ToList();
                }

                var latestJobsPrices = latestJobs.Sum(s => s.TotalPrice);

                decimal LastWeekFinishedJobs = 0;
                decimal currentWeekFinishedJobs = 0;
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
                model.Date = DateTime.Now.ToString("MM/dd/yyyy");
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
                        model.TotalClient = latestJobs.Select(s => s.CustomerId).Distinct().Count().ToString();
                        var customers = latestJobs.Select(s => s.CustomerId);
                        var deliverdProducts = db.ProductDeliveries.Where(q => customers.Contains(q.CustomerId)).ToList();
                        model.DeliveryFrom = deliverdProducts.FirstOrDefault()?.ProductDeliveryId.ToString();
                        model.DeliveryTo = deliverdProducts.LastOrDefault()?.ProductDeliveryId.ToString();
                        var voidedProductDeliveryItemss = deliverdProducts.Where(q => q.DeliveryStatus == DeliveryStatus.Voided).Select(s => s.ProductDeliveryId);
                        model.CancelledDelivery = string.Join(",", voidedProductDeliveryItemss);


                        model.From = startDate?.ToString("dd/MM/yyyy");
                        model.To = endDate?.ToString("dd/MM/yyyy");

                        var items = new List<PrintingAndFinishingTeamPerformanceItem>();
                        var jobTypeIds = latestJobs.ToList().Select(s => s.JobTypeId).Distinct();
                        foreach (var jobType in jobTypeIds)
                        {

                            var jobTypePlan = db.ProductionMonthlyJobTypePlans.Where(q => q.ProductionMonthlyPlanId == monthlyPlan.ProductionMonthlyPlanId
                               && q.JobTypeId == jobType).FirstOrDefault();
                            if (jobTypePlan != null)
                                 {
                                var jobTypeRec = db.JobCategories.Find(jobType);
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
                                        var jobStatus = db.JobStatuses.Where(q => q.JobId == currentJob.JobId && q.JobPhase.ToString() == ProductionDepartment.SalesDelivery.ToString()).FirstOrDefault();
                                        if (jobStatus != null)
                                        {
                                            var finished = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(q => q.TotalPrice);
                                            currentWeekFinishedJobTypes += finished;
                                        }
                                    }
                                var jobTypeIdsss = lastWeekJobTypes.ToList().Select(s => s.JobId);
                                var changedQuan = db.ChangeOrders.Where(q => jobTypeIdsss.Contains(q.JobId)).ToList().Sum(s => s.Total);
                                    lastWeekPendingJobTypes = lastWeekJobTypes.Sum(s => s.TotalPrice) +changedQuan- lastWeekFinishedJobTypes;
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
                                    item.CurrentWeekOrderedJobs = Math.Round(currentWeekJobTypes.Sum(s => s.TotalPrice), 2);
                                    item.Total = item.LastWeekPendingJobs + item.CurrentWeekOrderedJobs;
                                    item.WeeklyPlan = Math.Round(jobTypePlan.TotalPrice, 2);
                                    item.WeeklyPerformance = currentWeekFinishedJobTypes;
                                    item.ProductionInProgress = Math.Round(item.Total - currentWeekFinishedJobTypes, 2);

                                    if (item.WeeklyPlan != 0)
                                    {
                                        item.Percent = Math.Round(item.WeeklyPerformance / item.WeeklyPlan, 2) * 100;

                                    }
                                    items.Add(item);
                                
                            }


                        }
                        model.PrintingAndFinishingTeamPerformanceItems = items;
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
