using ExceedERP.Core.Domain.PrintingEstimation.Setting;
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
    public class FinishedProductsReceivingNoteODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public FinishedProductsReceivingNoteVM GetFinishedProducts(DateTime? startDate, DateTime? endDate, string jobPhase,string jobTypeId, string userName)
        {
            string to = "";

            if (startDate==null){
                startDate = DateTime.Now;
            }
            if (endDate == null)
            {
                endDate = DateTime.Now;
            }
            if (jobPhase == null)
            {
                jobPhase = JobPhase.Printing.ToString();
            }
            if (jobPhase == JobPhase.Printing.ToString())
            {
                to = JobPhase.Finishing.ToString();
            }
            else if (jobPhase == JobPhase.Finishing.ToString())
            {
                to = JobPhase.Delivery.ToString();
            }
            var jobs = db.JobStatuses.ToList().Where(x =>  (x.EndDate.HasValue ? x.EndDate.Value.Date >= startDate.Value.Date
                              && x.EndDate.Value.Date <= endDate.Value.Date : false)&&x.JobPhase.ToString()==jobPhase).ToList();
          

            if(jobTypeId!=null)
            {
                var jobOrders = db.Jobs.Where(q => q.JobTypeId.ToString() == jobTypeId).ToList().Select(s=>s.JobId);
                 jobs = jobs.Where(q => jobOrders.Contains(q.JobId)).ToList();
            }
            var model = new FinishedProductsReceivingNoteVM();
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
          
            
            var items = new List<FinishedProductsReceivingNoteVMItem>();

            model.From = jobPhase;
            model.To = to;
            model.DeliveredBy = userName;
            model.ReceivedBy = "";
            model.Date = DateTime.Now.ToString("dd/MM/yyyy");
            foreach (var job in jobs)
            {
                var item = new FinishedProductsReceivingNoteVMItem();
                var jobOrder = db.Jobs.Find(job.JobId);
                if(jobOrder!=null)
                {
                    item.JobNo = jobOrder.JobNo;
                    item.Unit = db.UoMs.Find(jobOrder.UnitId)?.UoMName;
                    item.JobType = db.JobCategories.Find(jobOrder.JobTypeId)?.JobName;
                    item.Client = db.OrganizationCustomers.Find(jobOrder.CustomerId)?.TradeName;
                    item.JobType = db.JobCategories.Find(jobOrder.JobTypeId)?.JobName;
                    item.Quantity = db.JobStatusLineItems.Where(q=>q.JobStatusId== job.JobStatusId).ToList().Sum(s=>s.Quantiy);

                    if(item.Quantity==0)
                    {
                        continue;
                    }
                        item.JobType = db.JobCategories.Find(jobOrder.JobTypeId)?.JobName;
                    decimal TotalBirr = Math.Round(item.Quantity * (decimal)jobOrder.UnitPrice,2);
                    item.TotalPriceBirr = (int)TotalBirr;
                    item.TotalPriceCent = (int)(Math.Round((TotalBirr % 1) ,2)* 100);
                    item.Remark = "";
                    item.JobType = db.JobCategories.Find(jobOrder.JobTypeId)?.JobName;
                    items.Add(item);

                }

            }
            model.FinishedProductsReceivingNoteVMItems = items;

            return model;

        }
    }
}
