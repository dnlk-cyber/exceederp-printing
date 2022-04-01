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
    public class JobSupplyandSalesODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobSupplyandSalesVM GetJobSupplyandSalesODS(DateTime? startDate, DateTime? endDate)
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
            decimal WeeklySupplyAndSalesPlan = 0;
            decimal WeeklyReceivedActual = 0;
            decimal WeeklySalesActual = 0;
            if (startDate.HasValue && endDate.HasValue)
            {
                days = (endDate.Value.Date - startDate.Value.Date).Days;
                var jobs = db.Jobs.ToList().Where(x => x.ReceivedDate.HasValue? x.ReceivedDate.Value.Date >= startDate.Value.Date
                  && x.ReceivedDate.Value.Date <= endDate.Value.Date:false).ToList();
                
                model.From = startDate?.ToString("dd/MM/yyyy");
                model.To = endDate?.ToString("dd/MM/yyyy");
                model.PreparedBy = "";
                model.CheckedBy = "";
                model.ApprovedBy = "";
                model.To = endDate?.ToString("dd/MM/yyyy");
                var jobTypes = db.JobCategories.Where(q => !q.HaveParent).ToList();
                List<JobSupplyandSalesItemVM> jobList = new List<JobSupplyandSalesItemVM>();
                foreach (var jobType in jobTypes)
                {
                    var job = new JobSupplyandSalesItemVM();
                    var jobTS = jobs.Where(q => q.JobTypeId == jobType.JobCategoryId).ToList();
                    var jobIds = jobTS.Select(s => s.JobId);
                    IEnumerable<int> deliveredJobIds = db.JobStatuses.Where(q => q.JobPhase == ProductionDepartment.SalesDelivery
                               && q.Status == Core.Domain.PrintingEstimation.Setting.JobState.Finished
                               && jobIds.Contains(q.JobId)). Select(s => s.JobStatusId);
                    var deliveredJobs = db.JobStatusLineItems.ToList().Where(q => deliveredJobIds.Contains(q.JobStatusId));
                    var period = db.GLPeriods.ToList().Where(q => (q.DateFrom.Date <= startDate.Value.Date&& startDate.Value.Date <= q.DateTo.Date)
                               && (q.DateTo.Date >= endDate.Value.Date && endDate.Value.Date >= q.DateFrom.Date));
                    if(period!=null)
                    {
                        var periods = period.Select(s => s.GLPeriodId);
                        var monthlyPlans = db.ProductionMonthlyPlans.Where(q => periods.Contains(q.GLPeriodId)).ToList();
                        var count = monthlyPlans.Count();
                        if (count >0)
                        {
                            var proMonthlyPlans = monthlyPlans.Select(s => s.ProductionMonthlyPlanId);
                            var jobTypePlan = db.ProductionMonthlyJobTypePlans.Where(q => proMonthlyPlans.Contains(q.ProductionMonthlyPlanId)
                            && q.JobTypeId == jobType.JobCategoryId).ToList();
                            if(jobTypePlan.Count()>0)
                            {
                                job.DailySupply = Math.Round(jobTypePlan.Sum(s=>s.TotalPrice) / (count*30),2);
                                job.WeeklySupplyAndSalesPlan = Math.Round(job.DailySupply*days,2);
                                job.WeeklyReceivedActual = Math.Round(jobTS.Sum(s=>s.TotalPrice),2);
                                job.WeeklySupplyPerformance = Math.Round(job.WeeklySupplyAndSalesPlan - job.WeeklyReceivedActual,2);
                                job.WeeklySalesActual = Math.Round(deliveredJobs.Sum(s=>s.TotalPrice),2);
                                job.WeeklySalesPerformance = Math.Round(job.WeeklySupplyAndSalesPlan - job.WeeklySalesActual,2);
                                WeeklySupplyAndSalesPlan += job.WeeklySupplyAndSalesPlan;
                                WeeklyReceivedActual += job.WeeklyReceivedActual;
                                WeeklySalesActual += job.WeeklySalesActual;
                            }
                        }
                    }

                    
                    job.Code = jobType?.JobCode;
                    jobList.Add(job);
                }
                model.WeeklySupplyAndSalesPlan = Math.Round(WeeklySupplyAndSalesPlan, 2);
                model.WeeklyReceivedActual = Math.Round(WeeklyReceivedActual, 2);
                model.WeeklySupplyPerformance = Math.Round(model.WeeklySupplyAndSalesPlan - model.WeeklyReceivedActual, 2);
                model.WeeklySalesActual = Math.Round(WeeklySalesActual, 2);
                model.WeeklySalesPerformance = Math.Round(WeeklySupplyAndSalesPlan - WeeklySalesActual, 2);
                model.JobSupplyandSalesItemVMs = jobList;
            }


            return model;
        }
    }
}

