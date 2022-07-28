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
    public class ProductDeliveryODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProductDeliveryVM GetProductDelivery(string id)
        {
            var delivery = new ProductDeliveryVM();

            if (id != null)
            {
                int.TryParse(id, out int productDeliveryId);

                var productDelivery = db.ProductDeliveries.FirstOrDefault(x => x.ProductDeliveryId == productDeliveryId);
                if (productDelivery != null)
                {
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

                    delivery.LogoFileName = LogoFileName;
                    delivery.Name = Name;
                    delivery.AmharicName = AmharicName;
                    delivery.DeliveryDate = productDelivery.DeliveryDate?.ToString("MMMM d,yyyy");
                    delivery.PrepareBy = productDelivery.PreparedBy ;
                    delivery.ApprovedBy =  productDelivery.ApprovedBy ;
                    delivery.IdentificationNo = productDelivery.IdentificationNo;
                    delivery.CreditSaleNo = productDelivery.CreditSaleNo;
                    delivery.CashSaleNo = productDelivery.CashSaleNo;


                    var job = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == productDelivery.CustomerId);
                    if (job != null)
                    {
                        //delivery.Tel1 = job.ContactPhone != null ? job.ContactPhone : "________"; 
                        //delivery.POBox = job.POBox != null ? job.POBox : "________";
                        delivery.CustomerName = job.TradeName;
                        delivery.BrandName = job.BrandName;
                        //delivery.ContactName = job.ContactName != null ? job.ContactName : "________";
                        //delivery.City = job.City != null ? job.City : "________";
                        //delivery.Address = job.SubCity != null ? job.SubCity : "________";

                    }

                    delivery.DeliveryItems = new List<ProductDeliveryItemsVM>();

                    var deliveryItems = db.ProductDeliveryItems.Where(x=>x.ProductDeliveryId == productDelivery.ProductDeliveryId).ToList();
                    var items = deliveryItems.GroupBy(g => g.JobId)
                        .Select(s => new
                        {
                            JobId = s.Key,
                            TotalCost = s.Sum(su=>su.TotalCost),
                            UnitCost = s.Sum(su=>su.UnitCost),
                            Quantity = s.Sum(su=>su.Quantity)
                        });
                    foreach (var item in items)
                    {
                        var jobb = db.Jobs.Find(item.JobId);
                        var product = new ProductDeliveryItemsVM();

                        //product.ItemCode = item.ItemCode;
                        product.Unit = db.UoMs.Find(db.Jobs.Find(item.JobId)?.UnitId)?.UoMName;
                        product.JobNumber = db.Jobs.Find(item.JobId)?.JobNo;
                        product.Description = db.JobCategories.Find(db.Jobs.Find(item.JobId)?.JobTypeId)?.JobName;
                        product.JobNumber = db.Jobs.Find(item.JobId)?.JobNo;
                        product.Quantity = item.Quantity;
                        product.UnitCostBirr = (int)item.UnitCost;
                        product.UnitCostCent = (int)(Math.Round(item.UnitCost, 2) % 1 * 100);
                        product.TotalPriceBirr = (int)item.TotalCost;
                        product.TotalPriceCent = (int)(Math.Round(item.TotalCost, 2) % 1 * 100);

                        delivery.DeliveryItems.Add(product);
                    }

                    //productList.Add(delivery);
                }
            }

            return delivery;
        }
    }
}
