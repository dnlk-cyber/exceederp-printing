using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class ShipmentDetailOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ShipmentDetailViewModel> GetShipmentData(string branchId, string storeId, string customerId, DateTime start, DateTime end, string productCategoryId, string productId, string jobOrderNo)
        {
          
            List<ShipmentDetailViewModel> list = new List<ShipmentDetailViewModel>();
            var temp = _db.SalesShipments.ToList();
            var shipments = new List<SalesShipment>();

            foreach (var o in temp)
            {
                if (o.ShipmentDate.Date <= end.Date && o.ShipmentDate.Date >= start.Date)
                {
                    shipments.Add(o);
                }
            }
            if (branchId != null)
            {
                int bId = int.Parse(branchId);
                shipments = shipments.Where(o => o.BranchId == bId).ToList();
            }
            if (customerId != null)
            {
                int cId = int.Parse(customerId);
                shipments = shipments.Where(o => o.OrganizationCustomerId == cId).ToList();
            }
            if(jobOrderNo != null)
            {
                var reqNumbers = new HashSet<string>(_db.StoreRequisitions.Where(s => s.IsForMaintenance && s.JobOrderNo == jobOrderNo).Select(s => s.StoreRequisitionID.ToString()));
                shipments = shipments.Where(s => s.Source == ShipmentSource.MaintenanceStoreRequisition && reqNumbers.Contains(s.StoreRequisitionNumber)).ToList();
            }
            var tempList = new List<SalesShipment>();
            tempList.AddRange(shipments);
            if (!string.IsNullOrEmpty(storeId))
            {
                int sId = int.Parse(storeId);
                var lineItemsInStore =
                    _db.SalesShipmentLineItem.Where(l =>  l.SalesStoreId == sId).ToList();
                var vIds = new HashSet<int>(lineItemsInStore.Select(l => l.SalesShipmentId));
                foreach (SalesShipment ss in tempList)
                {
                    var voucher = ss.SalesShipmentId;
                    if (!vIds.Contains(voucher))
                    {
                        shipments.Remove(ss);
                    }
                }
            }
            foreach (SalesShipment ss in shipments)
            {
                string source = "";
                string docNo = "";
                switch (ss.Source){
                    case ShipmentSource.SalesInvoice:
                        source = "Sales Invoice";
                        docNo = ss.CashSalesInvoiceID.Value.ToString();
                        break;
                    case ShipmentSource.SalesOrder:
                        source = "Sales Order";
                        docNo = ss.SalesOrderVoucherID.Value.ToString();
                        break;
                    case ShipmentSource.MaintenanceStoreRequisition:
                        source = "Maitenance SR";
                        docNo = ss.StoreRequisitionNumber;
                        break;
                    default:
                        break;
                }
                var record = new ShipmentDetailViewModel
                {
                    ShipmentNumber = ss.SalesShipmentId,
                    Customer = GetCustomerName(ss.OrganizationCustomerId),
                    //DocumentType = ss.Source== ShipmentSource.SalesInvoice ? "Sales Invoice" : "Sales Order",
                    DocumentType = source,
                    //DocumentNumber = ss.Source == ShipmentSource.SalesInvoice ? ss.CashSalesInvoiceID.Value : ss.SalesOrderVoucherID.Value,
                    DocumentNumber = docNo,
                    FsInvoiceNumber = ss.FsInvoiceNumber,
                    ShipmentReference = "",
                    ShipmentDate = ss.ShipmentDate,
                    //OrderDueDate = so.Dude,
                    ShipmentStatus = ss.Status,
                    //QuotationTerm = GetQuotationTerm(so.QuotationTermId),
                };
                var lineItems = _db.SalesShipmentLineItem.Where(l => l.SalesShipmentId == ss.SalesShipmentId).ToList();
                if (!string.IsNullOrEmpty(productCategoryId))
                {
                    //int catId = 0;
                    //int.TryParse(productCategoryId, out catId);
                    var productIdsUnderCategory = new HashSet<int>(_db.Products.Where(c => c.CategoryID == productCategoryId).ToList().Select(c => c.ProductID));
                    lineItems = lineItems.Where(l => productIdsUnderCategory.Contains(l.ProductId)).ToList();
                }
                if (!string.IsNullOrEmpty(productId))
                {
                    int prodId = 0;
                    int.TryParse(productId, out prodId);
                    lineItems = lineItems.Where(l => l.ProductId == prodId).ToList();
                }
                if (lineItems.Any())
                {
                    foreach (SalesShipmentLineItem lineItem in lineItems)
                    {
                        Product currentProduct = GetProduct(lineItem.ProductId);
                        if(currentProduct != null)
                        {
                            var lineRecord = new ShipmentDetailViewModel
                            {
                                ShipmentNumber = record.ShipmentNumber,
                                Customer = record.Customer,
                                DocumentNumber = record.DocumentNumber,
                                ShipmentReference = "",
                                ShipmentDate = ss.ShipmentDate,
                                ShipmentStatus = ss.Status,
                                DocumentType = record.DocumentType,
                                FsInvoiceNumber = record.FsInvoiceNumber,

                                Class = lineItem.Class,
                                Variety = lineItem.Variety,
                                ItemCategory = GetProductCategory(lineItem.ProductId),
                                ItemCode = currentProduct.ItemCode,
                                ItemName = currentProduct.Name,
                                Store = GetSalesStore(lineItem.SalesStoreId),
                                UoM = currentProduct.UOM,
                                UnitPrice = lineItem.Price,
                                TotalQuantity = lineItem.TotalQuantity,
                                ShipedQuantity = lineItem.ShipedQuantity,
                                RemainingQuantity = lineItem.RemainingQuantity,
                                Remarks = lineItem.Remark
                            };
                            list.Add(lineRecord);
                        }
                    }
                }
            }
            return list;
        }

        private string GetSalesStore(int salesStoreId)
        {
            return _db.SalesStores.Find(salesStoreId)?.Description;
        }

        private Product GetProduct(int productId)
        {
            return _db.Products.Find(productId);
        }

        private string GetProductCategory(int productId)
        {
            var cat = string.Empty;
            var product = _db.Products.Find(productId);
            if(product != null){
                cat =  _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == product.CategoryID)?.Description;
            }
            return cat;
        }
        private string GetCustomerName(int id)
        {
            string name = "";
            var org = _db.OrganizationCustomers.Find(id);
            if(org != null)
            {
                name = org.TradeName;
            }
            return name;
        }
    }
}
