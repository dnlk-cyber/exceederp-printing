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
    public class ProductDeliveryODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProductDeliveryVM> GetProductDelivery(string id)
        {
            List<ProductDeliveryVM> productList = new List<ProductDeliveryVM>();
            if (id != null)
            {
                int.TryParse(id, out int productDeliveryId);

                var productDelivery = db.ProductDeliveries.FirstOrDefault(x => x.ProductDeliveryId == productDeliveryId);
                if (productDelivery != null)
                {
                    var delivery = new ProductDeliveryVM();

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
                    foreach(var item in deliveryItems)
                    {
                        var unit = db.Units.Find(item.UnitId);
                        var product = new ProductDeliveryItemsVM();

                        product.ItemCode = item.ItemCode;
                        product.JobNumber = db.Jobs.Find(item.JobId)?.JobNo;
                        product.Description = item.Description;
                        product.JobNumber = db.Jobs.Find(item.JobId)?.JobNo;
                        product.Unit = unit?.UOM;
                        product.Quantity = item.Quantity;
                        product.UnitCost = item.UnitCost;
                        product.TotalPrice = item.TotalCost;

                        delivery.DeliveryItems.Add(product);
                    }

                    productList.Add(delivery);
                }
            }

            return productList;
        }
    }
}
