using ExceedERP.Core.Domain.CRM;
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
        public List<SelectListItem> GetFiscalYears()
        {
            var fisYears = db.GLFiscalYears.ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.GlFiscalYearId.ToString(),
                    Text = s.Name
                }).ToList();

            return fisYears;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetPeriods(string glFiscalYearId)
        {
            var periods = db.GLPeriods.ToList().Where(q=>q.GlFiscalYearId.ToString()== glFiscalYearId)
                .Select(s => new SelectListItem
                {
                    Value = s.GLPeriodId.ToString(),
                    Text = s.Name
                }).ToList();

            return periods;
        }
    }
}
