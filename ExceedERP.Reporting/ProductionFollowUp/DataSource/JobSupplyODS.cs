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
    public class JobSupplyODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobSupplyVM GetJobSupplyODS(DateTime? startDate, DateTime? endDate)
        {
            var model = new JobSupplyVM();
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
            if (startDate.HasValue && endDate.HasValue)
            {
                var jobs = db.Jobs.ToList().Where(x => x.ReceivedDate.HasValue ? x.ReceivedDate.Value.Date >= startDate.Value.Date
                  && x.ReceivedDate.Value.Date <= endDate.Value.Date:false).ToList();
                model.From = startDate?.ToString("dd/MM/yyyy");
                model.To = endDate?.ToString("dd/MM/yyyy");
                model.TotalJob = jobs.Count().ToString();
                model.TotalQuantity = jobs.Sum(s=>s.Quantity).ToString();
                model.Proforma = Math.Round(jobs.Where(q=>q.hv).Sum(s=>s.TotalPrice),2).ToString();
                model.DirectOrder = Math.Round(jobs.Where(q=>!q.hv).Sum(s=>s.TotalPrice),2).ToString();
                model.To = endDate?.ToString("dd/MM/yyyy");
               
                if (jobs != null)
                {
                    List<JobSupplyItemVM> jobList = new List<JobSupplyItemVM>();
                    var jobTypes = jobs.GroupBy(g => g.JobTypeId).Select(s => new
                    {
                        JobTypeId = s.Key,
                        TotalPrice = s.Sum(su => su.TotalPrice),
                        TotalQuantity = s.Sum(su => su.Quantity)
                      }).ToList();
                    foreach (var jobOrder in jobTypes)
                    {
                        var job = new JobSupplyItemVM();


                        var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobOrder.JobTypeId);
                        job.JobType = jobType?.JobName;
                        job.Code = jobType?.JobCode;
                        job.TotalPrice = jobOrder.TotalPrice;
                        job.TotalQuantity = jobOrder.TotalQuantity;
                        jobList.Add(job);
                    }
                    model.JobSupplyItemVMs = jobList;
                }
            }


            return model;
        }
    }
}

