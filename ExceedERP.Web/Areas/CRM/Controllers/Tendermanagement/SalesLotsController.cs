using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM.TenderManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.CRM;
using System.Collections.Generic;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
    // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class SalesLotsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["productCategories"] = db.ProductCategories.ToList();
            ViewData["products"] = db.Products.ToList();
            ViewData["productCategoriess"] = db.SalesLotItems.ToList();
            ViewData["productss"] = db.SalesLotItems.ToList();
            ViewData["saleslotPurchase"] = db.SalesLotPurchases.Select(prop => new {
                Value = prop.SalesLotPurchaseId,
                Text=prop.CustomerName
            }).ToList();
            ViewData["branch"] = db.Branch.Select(s => new
            {
                Code = s.BranchId,
                Name = s.Name
            }).ToList();
            ViewData["stores"] = db.InventoryStores.Select(s => new
            {
                StoreCode = s.StoreCode,
                Name = s.Name
            }).ToList();
            ViewData["businessunit"] = db.BusinessUnits.Select(s => new
            {
                Code = s.BusinessUnitId,
                Name = s.Name
            }).ToList();
            ViewData["participant"] = db.SalesLotPurchases.Select(s => new
            {
                Value = s.SalesLotPurchaseId,
                Text = s.CustomerName
            }).ToList();
            var fetchTender = db.SaleTenders.ToList();
            ViewData["tender"] = fetchTender.Select(st => new { Value=st.SaleTenderId, Text=st.TenderName });
            return View();
        }
        public JsonResult Void(int id)
        {
            var record = db.SalesLots.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid&&!record.IsSendForInvoice)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                //db.SalesLots.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var fetchApproval = db.TenderWinnerValidations.FirstOrDefault(x=>x.SalesLotId==id);
                 if(fetchApproval!=null)
                {
                    db.TenderWinnerValidations.Remove(fetchApproval);
                    db.SaveChanges();
                }
                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSendForInvoice)
            {
                var formattedData = new
                {
                    Message = "Record already send for invoice.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            } if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        public JsonResult SendForApproval(int id)
        {
            var record = db.SalesLots.Find(id);
            var checkWinners = db.Winners.Where(sli => sli.SalesLotId == id).ToList();
            if (!checkWinners.Any())
            {
                var formattedData = new
                {
                    Message = "Winners table does not have data under this lot",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var checkDate = db.SaleTenders.SingleOrDefault(x => x.SaleTenderId == record.SaleTenderId);
            if (checkDate==null)
            {
                var formattedData = new
                {
                    Message = "record does not exit in tender",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            //if (checkDate.EvaluationDate.Date < DateTime.Now.Date)
            //    {
            //        var formattedData = new
            //        {
            //            Message = "You can not approve Winner after Evaluation Date",
            //            Status = "NOT_OK"
            //        };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            //    if (checkDate.EvaluationDate.Date > DateTime.Now.Date)
            //    {
            //        var formattedData = new
            //        {
            //            Message = "You can not approve Winner before Evaluation Date",
            //            Status = "NOT_OK"
            //        };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            
            var checkItems = db.SalesLotItems.Where(sli => sli.SalesLotId == id).ToList();
            if(!checkItems.Any())
            {
                var formattedData = new
                {
                    Message = "You have to Add Items under The Lot",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
          
            if (checkItems.Any())
            {
                if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.Winners.Count > 0)
                {

                    record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                    record.IsSendForApproval = true;
                    record.SendForApprovalBy = User.Identity.Name;
                    record.SendForApprovalTime = DateTime.Now;
                    db.SalesLots.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully Sent.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }
          

           

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;

         
        }
        public JsonResult toCashSalesInvoice(int id)
        {
            var record = db.SalesLots.Find(id);
            var FetchStoreId = db.SalesStores.SingleOrDefault(ss => ss.StoreCode == record.StoreCode);
            if(FetchStoreId == null)
            {
                var formattedData = new
                {
                    Message = "No store in inventory stores with this record",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
      
            var checkDate = db.SaleTenders.SingleOrDefault(x => x.SaleTenderId == record.SaleTenderId);
            if(checkDate.EvaluationDate.Date>DateTime.Now.Date)
            {
                var formattedData = new
                {
                    Message = "You can not send to cash sales invoice before Evaluation Date",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
         
           
            var checkItems = db.SalesLotItems.Where(sli => sli.SalesLotId == id).ToList();
            if(!checkItems.Any())
            {
                var formattedData = new
                {
                    Message = "You have to Add Items under The Lot",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }



            if (!record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "Record should be sent for Approval First",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "Record is not Approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSendForInvoice)
            {
                var formattedData = new
                {
                    Message = "This data set is already send for invoice",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (checkItems.Any())
            {
                if (record != null && record.IsSendForApproval && record.IsOnlineApproved && record.Winners.Count > 0&&!record.IsSendForInvoice)
                {
                    var tins = db.Winners.Where(sli => sli.SalesLotId == id).Select(x => x.TinNumber).Distinct().ToList();
                    foreach (var t in tins)
                    {
                        var winner = db.Winners.Where(sli => sli.SalesLotId == id).Where(x => x.TinNumber == t).FirstOrDefault();
                        int count = 0;
                        if (winner != null)
                        {
                            var tinNumbers = new HashSet<string>();
                            var invoiceTinNumbers = new HashSet<string>();
                            var checkAvailablity = db.OrganizationCustomers.SingleOrDefault(oc => oc.TinNo == winner.TinNumber);
                            if (checkAvailablity == null)
                            {
                                if (!tinNumbers.Contains(winner.TinNumber))
                                {
                                    tinNumbers.Add(winner.TinNumber);
                                    var organizationCustomer = new OrganizationCustomer();
                                    organizationCustomer.BrandName = record.BidParticpatePlace;
                                    organizationCustomer.Category = winner.ProductCategoryDesc;
                                    organizationCustomer.CreatedBy = User.Identity.Name;
                                    organizationCustomer.DateCreated = DateTime.Now;
                                    organizationCustomer.LastModified = organizationCustomer.LastModified;
                                    organizationCustomer.OrganizationCustomerID = organizationCustomer.OrganizationCustomerID; // Generate a new ID if needed
                                    organizationCustomer.BusinessType = winner.CustomerName;
                                    organizationCustomer.TinNo = winner.TinNumber;
                                    organizationCustomer.TradeName = winner.CustomerName;
                                    organizationCustomer.EsclationLevel = null;
                                    db.OrganizationCustomers.Add(organizationCustomer);
                                    db.SaveChanges();

                                    //header

                                    var invoiceNumber = new CashSalesInvoice();
                                    invoiceNumber.OrganizationCustomerId = organizationCustomer.OrganizationCustomerID;
                                    invoiceNumber.CashSalesInvoiceID = invoiceNumber.CashSalesInvoiceID;
                                    invoiceNumber.CustomerTINNumber = winner.TinNumber;
                                    invoiceNumber.BranchId = record.BranchId;
                                    invoiceNumber.IssuedDate = DateTime.Now;
                                    invoiceNumber.Date = DateTime.Now;
                                    invoiceNumber.PaymentMethod ="CASH";
                                    //invoiceNumber.OrganizationCustomer = winner.CustomerName; // Generate a new ID if needed
                                    invoiceNumber.CRMPeriodId = 0;
                                    invoiceNumber.PaymentTermsID = 0;
                                    invoiceNumber.StoreID = FetchStoreId.SalesStoreId;
                                    db.CashSalesInvoices.Add(invoiceNumber);
                                      db.SaveChanges();
                                    count += 1;

                                    //items

                                    var winnerItems = db.Winners.Where(sli => sli.SalesLotId == id).Where(x => x.TinNumber == t).ToList();
                                    foreach (var wi in winnerItems)
                                    {
                                        if (wi.TinNumber == winner.TinNumber)
                                        {
                                            var iteminfo = db.SalesLotItems.SingleOrDefault(sli => sli.SalesLotItemId == wi.SalesLotItemId);
                                            var saveLineItem = new LineItem();
                                            saveLineItem.LineItemID = saveLineItem.LineItemID;
                                            saveLineItem.ThingID = wi.ProductID;
                                            saveLineItem.Voucher = invoiceNumber.CashSalesInvoiceID.ToString();
                                            saveLineItem.Quantity = iteminfo.Quantity;
                                            saveLineItem.UnitAmount = wi.WinningPrice / iteminfo.Quantity;
                                            saveLineItem.TaxAmount = wi.WinningPrice * (15m / 100m);
                                            saveLineItem.CalculatedCost = wi.WinningPrice;                                           
                                            saveLineItem.VoucherDefinitionID = "2";
                                            saveLineItem.StoreID = FetchStoreId.SalesStoreId;
                                            db.LineItems.Add(saveLineItem);
                                            db.SaveChanges();
                                        }
                                    }


                                }
                            }

                            else
                            {
                                if (!tinNumbers.Contains(winner.TinNumber))
                                {
                                    tinNumbers.Add(winner.TinNumber);

                                    var invoiceNumber = new CashSalesInvoice();
                                    invoiceNumber.OrganizationCustomerId = checkAvailablity.OrganizationCustomerID;
                                    invoiceNumber.CashSalesInvoiceID = invoiceNumber.CashSalesInvoiceID;
                                    invoiceNumber.CustomerTINNumber = winner.TinNumber;
                                    invoiceNumber.BranchId =record.BranchId;
                                    //invoiceNumber.OrganizationCustomer = winner.CustomerName; // Generate a new ID if needed
                                    invoiceNumber.CRMPeriodId = 0;
                                    invoiceNumber.IssuedDate =DateTime.Today;
                                    invoiceNumber.Date =DateTime.Today;
                                    invoiceNumber.PaymentTermsID = 0;
                                    invoiceNumber.PaymentMethod = "CASH";
                                    
                                    invoiceNumber.StoreID = FetchStoreId.SalesStoreId;
                                    db.CashSalesInvoices.Add(invoiceNumber);
                                    db.SaveChanges();
                                    count += 1;
                                    //items

                                    var winnerItems = db.Winners.Where(sli => sli.SalesLotId == id).Where(x => x.TinNumber == t).ToList();
                                    foreach (var wi in winnerItems)
                                    {
                                        if (wi.TinNumber == winner.TinNumber)
                                        {
                                            var iteminfo = db.SalesLotItems.SingleOrDefault(sli => sli.SalesLotItemId == wi.SalesLotItemId);
                                            var saveLineItem = new LineItem();
                                            saveLineItem.ThingID = wi.ProductID;
                                            saveLineItem.Voucher = invoiceNumber.CashSalesInvoiceID.ToString();
                                            saveLineItem.Quantity = iteminfo.Quantity;
                                            saveLineItem.UnitAmount = wi.WinningPrice / iteminfo.Quantity;
                                            saveLineItem.TaxAmount = wi.WinningPrice * (15m / 100m);
                                            saveLineItem.LineItemID = saveLineItem.LineItemID;
                                            saveLineItem.CalculatedCost = wi.WinningPrice;
                                            saveLineItem.StoreID = FetchStoreId.SalesStoreId;
                                            saveLineItem.VoucherDefinitionID = "2";
                                            db.LineItems.Add(saveLineItem);
                                            db.SaveChanges();
                                        }
                                    }

                                }
                            }
                          

                            if(count>0)
                            {
                                record.IsSendForInvoice = true;
                                db.Entry(record).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                        }

                    }
                    

                    var formattedData = new
                    {
                        Message = "Successfully Sent.",
                        Status = "OK"
                    };

                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }

                if (record.Winners.Count<0)
            {
                var formattedData = new
                {
                    Message = "You can't Approve With null Winners",
                    ApproveStatus = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            
            
            return null;
        }
        public ActionResult SalesLots_Read([DataSourceRequest]DataSourceRequest request)
        {
           var salesLots= db.SalesLots.ToList();
            DataSourceResult result = salesLots.ToDataSourceResult(request, salesLot => new {
                SalesLotId = salesLot.SalesLotId,
                SaleTenderId = salesLot.SaleTenderId,
                LotName = salesLot.LotName,
                BidParticpatePlace=salesLot.BidParticpatePlace,
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified,
                Description = salesLot.Description,               
                Status = salesLot.Status,
                IsVoid = salesLot.IsVoid,
                IsSendForInvoice = salesLot.IsSendForInvoice,
                VoidBy = salesLot.VoidBy,
                VoidTime = salesLot.VoidTime,
                BranchId = salesLot.BranchId,
                BusinessUnitiId = salesLot.BusinessUnitiId,
                StoreCode = salesLot.StoreCode,
                IsOnlineApproved = salesLot.IsOnlineApproved,
                OnlineApprovedBy = salesLot.OnlineApprovedBy,
                OnlineApprovedTime = salesLot.OnlineApprovedTime,
                IsSendForApproval = salesLot.IsSendForApproval,
                SendForApprovalBy = salesLot.SendForApprovalBy,
                SendForApprovalTime = salesLot.SendForApprovalTime,
                IsOnlineTransferred = salesLot.IsOnlineTransferred,
                IsOnlineTransferredBy = salesLot.IsOnlineTransferredBy,
                OnlineTransferredTime = salesLot.OnlineTransferredTime,

            });

            return Json(result);
        }

        public ActionResult SalesLots_Create([DataSourceRequest]DataSourceRequest request, SalesLot salesLot)
        {
           // salesLot.SaleTenderId = salesTenderId;
            if (ModelState.IsValid)
            {
                salesLot.DateCreated = DateTime.Now;
                salesLot.CreatedBy = User.Identity.Name;
                salesLot.IsSendForInvoice = false;
                var entity = GetMappedObject(salesLot);
                db.SalesLots.Add(entity);
                db.SaveChanges();
                salesLot.SalesLotId = entity.SalesLotId;
            }

            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLots_Update([DataSourceRequest]DataSourceRequest request, SalesLot salesLot)
        {
            
            if (ModelState.IsValid)
            {
                salesLot.LastModified = DateTime.Now;
                salesLot.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(salesLot);
                db.SalesLots.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLots_Destroy([DataSourceRequest]DataSourceRequest request, SalesLot salesLot)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(salesLot);
                db.SalesLots.Attach(entity);
                db.SalesLots.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }

        private SalesLot GetMappedObject(SalesLot salesLot)
        {
            var entity = new SalesLot
            {
                SalesLotId = salesLot.SalesLotId,
                SaleTenderId = salesLot.SaleTenderId,
                LotName = salesLot.LotName,
                BidParticpatePlace = salesLot.BidParticpatePlace,
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified,
                Description = salesLot.Description,
                IsSendForInvoice = salesLot.IsSendForInvoice,
                Status = salesLot.Status,
                IsVoid = salesLot.IsVoid,
                VoidBy = salesLot.VoidBy,
                VoidTime = salesLot.VoidTime,
                BranchId = salesLot.BranchId,
                BusinessUnitiId = salesLot.BusinessUnitiId,
                StoreCode = salesLot.StoreCode,
                IsOnlineApproved = salesLot.IsOnlineApproved,
                OnlineApprovedBy = salesLot.OnlineApprovedBy,
                OnlineApprovedTime = salesLot.OnlineApprovedTime,
                IsSendForApproval = salesLot.IsSendForApproval,
                SendForApprovalBy = salesLot.SendForApprovalBy,
                SendForApprovalTime = salesLot.SendForApprovalTime,
                IsOnlineTransferred = salesLot.IsOnlineTransferred,
                IsOnlineTransferredBy = salesLot.IsOnlineTransferredBy,
                OnlineTransferredTime = salesLot.OnlineTransferredTime,
                

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
