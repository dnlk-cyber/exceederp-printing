using ExceedERP.Core.Domain.PrintingEstimation.Setting;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Reporting.ProductionFollowUp.DataSource
{
    [DataObject]
    public class ProFollowupParameterODs
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetPaperSize(DateTime? startDate, DateTime? endDate)
        {
            var jobs = new List<Job>();
            if(startDate.HasValue&& endDate.HasValue)
            {
                jobs = db.Jobs.ToList().Where(x => x.ReceivedDate.HasValue ? x.ReceivedDate.Value.Date >= startDate.Value.Date
             && x.ReceivedDate.Value.Date <= endDate.Value.Date : false).ToList();
            }
           
            List<SelectListItem> paperSizes = new List<SelectListItem>();
             paperSizes= jobs.Where(q=>q.FinishingSize!=null).Select(
        s => new SelectListItem {
            Text = s.FinishingSize,
            Value = s.FinishingSize
        }).Distinct().ToList();

            return paperSizes;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetCopies(DateTime? startDate, DateTime? endDate, string size)
        {
            var jobs = new List<Job>();
            if (startDate.HasValue && endDate.HasValue)
            {
                jobs = db.Jobs.ToList().Where(x => x.ReceivedDate.HasValue ? x.ReceivedDate.Value.Date >= startDate.Value.Date
             && x.ReceivedDate.Value.Date <= endDate.Value.Date : false).ToList();
            }
            if(size!=null)
            {
                jobs = jobs.Where(q => q.FinishingSize == size).ToList();
            }
            List<SelectListItem> copies = new List<SelectListItem>();
            copies = jobs.Select(
       s => new SelectListItem
       {
          
           Text = s.Copies.ToString(),
           Value = s.Copies.ToString()
            
          
       }).ToList();

            return copies;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetJobStatuses()
        {
            List<SelectListItem> jobPhases = new List<SelectListItem>();
            jobPhases = (Enum.GetValues(typeof(JobState)).Cast<JobState>().Select(
        e => new SelectListItem() { Text = e.ToString(), Value = e.ToString() })).ToList();

            return jobPhases;
        }
        public enum JobOrderStatus
        {
            Received=0,
            OnProgress = 3,
            ParitiallyFinished=1,
            Finished=2

        }
        public List<SelectListItem> GetJobOrderStatuses()
        {
            List<SelectListItem> jobPhases = new List<SelectListItem>();
            jobPhases = (Enum.GetValues(typeof(JobOrderStatus)).Cast<JobOrderStatus>().Select(
        e => new SelectListItem() { Text = e.ToString(), Value = e.ToString() })).ToList();

            return jobPhases;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetAllJobPhases()
        {
            List<SelectListItem> jobPhases = new List<SelectListItem>();
            jobPhases = (Enum.GetValues(typeof(ProductionDepartment)).Cast<ProductionDepartment>().Select(
        e => new SelectListItem() { Text = e.ToString(), Value = e.ToString() })).ToList();

            return jobPhases;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetJobPhases()
        {
            List<SelectListItem> jobPhases = new List<SelectListItem>();
            jobPhases.Add(new SelectListItem
            {
                Value = JobPhase.Printing.ToString(),
                Text = JobPhase.Printing.ToString()
            });
            jobPhases.Add(new SelectListItem
            {
                Value = JobPhase.Finishing.ToString(),
                Text = JobPhase.Finishing.ToString()
            });
            {

            };
            return jobPhases;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetCustomers()
        {
            var custList = db.OrganizationCustomers.ToList()
                .Select(s =>  new SelectListItem{
                    Value = s.OrganizationCustomerID.ToString(),
                    Text = s.TradeName
                }).Distinct().ToList();

            return custList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetJobTypes()
        {
            var custList = db.JobCategories.Where(q=>!q.HaveParent).ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.JobCategoryId.ToString(),
                    Text = s.JobName
                }).ToList();

            return custList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetYearlyProductionPlans()
        {
            var fisYears = db.YearlyProductionPlans.ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.YearlyProductionPlanId.ToString(),
                    Text = s.GlFiscalYear.Name
                }).ToList();

            return fisYears;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetHalfYearProductionPlans(string yearlyProductionPlanId)
        {
            var halfYearPlans = db.HalfYearlyProductionPlans.ToList().Where(q=>q.YearlyProductionPlanId.ToString()== yearlyProductionPlanId)
                .Select(s => new SelectListItem
                {
                    Value = s.HalfYearlyProductionPlanId.ToString(),
                    Text = s.NameAm
                }).ToList();

            return halfYearPlans;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetQuarterYearProductionPlans(string halfYearProductionPlanId)
        {
            var quarterYearPlans = db.QuarterlyProductionPlans.ToList().Where(q => q.HalfYearlyProductionPlanId.ToString() == halfYearProductionPlanId)
                .Select(s => new SelectListItem
                {
                    Value = s.QuarterlyProductionPlanId.ToString(),
                    Text = s.NameAm
                }).ToList();

            return quarterYearPlans;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetMonthlyProductionPlans(string quarterYearProductionPlanId)
        {
            var periods = db.MonthlyProductionPlans.ToList().Where(q => q.QuarterlyProductionPlanId.ToString() == quarterYearProductionPlanId)
                .Select(s => new SelectListItem
                {
                    Value = s.MonthlyProductionPlanId.ToString(),
                    Text = s.NameAm
                }).ToList();

            return periods;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetWeeklyProductionPlans(string monthlyProductionPlanId)
        {
            var periods = db.WeeklyProductionPlans.ToList().Where(q => q.MonthlyProductionPlanId.ToString() == monthlyProductionPlanId)
                .Select(s => new SelectListItem
                {
                    Value = s.WeeklyProductionPlanId.ToString(),
                    Text = s.NameAm
                }).ToList();

            return periods;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetJobTypeProductionPlans(string weeklyProductionPlanId)
        {
            var periods = db.ProductionJobTypePlans.ToList().Where(q => q.WeeklyProductionPlanId.ToString() == weeklyProductionPlanId)
                .Select(s => new SelectListItem
                {
                    Value = s.JobTypeId.ToString(),
                    Text = s.JobType.JobName
                }).ToList();

            return periods;
        }

    }
}
