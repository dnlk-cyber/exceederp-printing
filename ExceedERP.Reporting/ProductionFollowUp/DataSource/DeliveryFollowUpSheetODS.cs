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
    public class DeliveryFollowUpSheetODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public DeliveryFollowUpSheetVM GetDeliveredProduct(DateTime? startDate, DateTime? endDate, string customerID)
        {
            var model = new DeliveryFollowUpSheetVM();
            var productDeliverys = db.ProductDeliveries.ToList();

            if (customerID != null)
            {
                productDeliverys = productDeliverys.Where(q => q.CustomerId.ToString() == customerID).ToList();
            }
            if(startDate.HasValue)
            {
                productDeliverys = productDeliverys.Where(q => q.DeliveryDate.HasValue?q.DeliveryDate.Value.Date >= startDate.Value.Date:false).ToList();
                model.From = startDate?.ToString("MM/dd/yyyy");

            }
            if (endDate.HasValue)
            {
                productDeliverys = productDeliverys.Where(q => q.DeliveryDate.HasValue ? q.DeliveryDate.Value.Date <= endDate.Value.Date:false).ToList();
                model.To = endDate?.ToString("MM/dd/yyyy");
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
            var items = new List<DeliveryFollowUpSheetItemsVM>();
            decimal grandTotal = 0;
            foreach (var productDelivery in productDeliverys)
            {
                var item = new DeliveryFollowUpSheetItemsVM();
                
                var deliveries = db.ProductDeliveryItems.ToList().Where(q => q.ProductDeliveryId == productDelivery.ProductDeliveryId);
                var jobs = deliveries.Select(s => s.JobId);
                var jobTypeIds = db.Jobs.Where(q => jobs.Contains(q.JobId)).Select(s=>s.JobTypeId).Distinct();
                var jobTypes = db.JobCategories.Where(q => jobTypeIds.Contains(q.JobCategoryId)).Select(s=>s.JobCode).Distinct().ToArray();
                   item.DoNo = productDelivery.ProductDeliveryId.ToString();
                   item.Code = string.Join(",", jobTypes);
                   item.Date = productDelivery.DateCreated?.ToString("MM/dd/yyyy");
                   item.Customer = db.OrganizationCustomers.Find(productDelivery.CustomerId)?.TradeName;
                   item.Credit = productDelivery.Credit.ToString();
                   item.Amount = deliveries.Sum(s=>s.Amount);
                   item.VAT = deliveries.Sum(s => s.VAT);
                   item.Total = item.Amount + item.VAT;
                   grandTotal += deliveries.Sum(s => s.TotalCost);
                   item.GrandTotal = grandTotal;
                       
                        items.Add(item);
                    
                
            }
            model.DeliveryFollowUpSheetItemsVMs = items;
            return model;
        }
    }
}
