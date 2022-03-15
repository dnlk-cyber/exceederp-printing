using ExceedERP.Core.Domain.PrintingEstimation.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.ProductionFollowUp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public FinishedProductsReceivingNoteVM GetFinishedProducts(DateTime? startDate, DateTime? endDate, string jobPhase, string userName)
        {
            var jobs = db.JobStatuses.ToList().Where(q => q.CreatedBy == userName
                                        && q.JobPhase.ToString() == jobPhase
                                        && q.StartDate.Date >= startDate.Value.Date
                                        && q.EndDate.Date >= endDate.Value.Date
                                        ).ToList();
            var model = new FinishedProductsReceivingNoteVM();

            string to = "";
            if (jobPhase == JobPhase.Printing.ToString())
            {
                to = JobPhase.Finishing.ToString();
            }
            else if (jobPhase == JobPhase.Finishing.ToString())
            {
                to = JobPhase.Delivery.ToString();
            }
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
                    item.JobType = db.JobCategories.Find(jobOrder.JobTypeId)?.JobName;
                    item.Client = db.OrganizationCustomers.Find(jobOrder.CustomerId)?.TradeName;
                    item.JobType = db.JobCategories.Find(jobOrder.JobTypeId)?.JobName;
                    // item.Unit = db.UnitOfMeasurement.Find(jobOrder.)?.JobName;
                    item.Quantity = db.JobStatusLineItems.Where(q=>q.JobStatusId== job.JobStatusId).ToList().Sum(s=>s.Quantiy);
                    item.JobType = db.JobCategories.Find(jobOrder.JobTypeId)?.JobName;
                    decimal TotalBirr = Math.Round(item.Quantity * (decimal)jobOrder.UnitPrice,2);
                    item.TotalPriceBirr = (int)TotalBirr;
                    item.TotalPriceCent = (TotalBirr % 1) * 100;
                    item.Remarks = "";
                    item.JobType = db.JobCategories.Find(jobOrder.JobTypeId)?.JobName;
                    items.Add(item);

                }

            }
            model.FinishedProductsReceivingNoteVMItems = items;

            return model;

        }
    }
}
