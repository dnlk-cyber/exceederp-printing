using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.ShipmentApprover, CRMRoles.ShipmentUser, CRMRoles.IncompleteShipmentAdmin)]
    public class SalesShipmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult Shipments_Read([DataSourceRequest]DataSourceRequest request, bool showTransferableOnly = false)
        {

            //var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            //var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));
            var assignedStores = db.SalesPersonAssignments.Where(a => a.UserName == User.Identity.Name).Select(a => a.SalesStoreId).ToList();


            IQueryable<SalesShipment> shipments = db.SalesShipments.Where(s => assignedStores.Contains(s.StoreID)).OrderByDescending(s => s.SalesShipmentId);
            if (showTransferableOnly)
            {
                shipments = shipments.Where(s => s.IsOnlineApproved && !s.IsOnlineTransferred);
            }
            DataSourceResult result = shipments.ToDataSourceResult(request, shipment => new SalesShipment
            {
                SalesShipmentId = shipment.SalesShipmentId,
                ShipmentDate = shipment.ShipmentDate,
                CashSalesInvoiceID = shipment.CashSalesInvoiceID,
                SalesOrderVoucherID = shipment.SalesOrderVoucherID,
                Source = shipment.Source,
                OrganizationCustomerId = shipment.OrganizationCustomerId,
                BranchId = shipment.BranchId,
                StoreID = shipment.StoreID,
                CRMPeriodId = shipment.CRMPeriodId,
                MethodOfShipment = shipment.MethodOfShipment,
                PlateNo = shipment.PlateNo,
                FsInvoiceNumber = shipment.FsInvoiceNumber,
                TrailerPlateNo = shipment.TrailerPlateNo,
                JobOrderNumber = shipment.JobOrderNumber,
                StoreRequisitionNumber = shipment.StoreRequisitionNumber,

                Status = shipment.Status,
                IsVoid = shipment.IsVoid,
                IsOnlineApproved = shipment.IsOnlineApproved,
                OnlineApprovedBy = shipment.OnlineApprovedBy,
                IsOnlineTransferred = shipment.IsOnlineTransferred,
                IsOnlineTransferredBy = shipment.IsOnlineTransferredBy,
                OnlineApprovedTime = shipment.OnlineApprovedTime,
                OnlineTransferredTime = shipment.OnlineTransferredTime,
                IsSendForApproval = shipment.IsSendForApproval,
                SendForApprovalBy = shipment.SendForApprovalBy,
                SendForApprovalTime = shipment.SendForApprovalTime,
                VoidBy = shipment.VoidBy,
                VoidTime = shipment.VoidTime,

               // IsJobOrderClosed = shipment.Source== ShipmentSource.MaintenanceStoreRequisition ?  CRMMaintenanceIntegrationHelper.IsJobOrderClosed(shipment.JobOrderNumber) : false,

                CreatedBy = shipment.CreatedBy,
                DateCreated = shipment.DateCreated,
                LastModified = shipment.LastModified,
                ModifiedBy = shipment.ModifiedBy,
                OrgBranchName = shipment.OrgBranchName,
                Remark = shipment.Remark
            });

            return Json(result);
        }
        /// <summary>
        /// returns all sales shipments that are approved but not been sent to bincard and 
        /// didn't affect the store balance and so on
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizeRoles(CRMRoles.IncompleteShipmentAdmin)]
        public ActionResult IncompleteShipments_Read([DataSourceRequest]DataSourceRequest request)
        {

            var concatnatedRef =
                db.SalesShipments.Where(s => s.IsOnlineApproved)
                .Select(s => new
                {
                    Id = s.SalesShipmentId,
                    ConcRef = "Shipment " + s.SalesShipmentId.ToString()
                }).ToList();
            //var refHash = new HashSet<string>(concatnatedRef.Select(c => c.ConcRef));

            var binCardFromShipment = new HashSet<string>(db.BinCardLines.Where(b => b.ReferenceDoc.Contains("Shipment")).Select(s => s.ReferenceDoc));
            var missingId = new HashSet<int>(concatnatedRef.Where(c => !binCardFromShipment.Contains(c.ConcRef)).ToList().Select(s => s.Id));
            var missingShipment = db.SalesShipments.Where(s => s.IsOnlineApproved && missingId.Contains(s.SalesShipmentId));
            //HashSet<int> shipmentIds = new HashSet<int>();

            //IQueryable<SalesShipment> shipments = db.SalesShipments.Where(s => assignedStores.Contains(s.StoreID)).OrderByDescending(s => s.SalesShipmentId);
            //if (showTransferableOnly)
            //{
            //    shipments = shipments.Where(s => s.IsOnlineApproved && !s.IsOnlineTransferred);
            //}
            DataSourceResult result = missingShipment.ToDataSourceResult(request, shipment => new SalesShipment
            {
                SalesShipmentId = shipment.SalesShipmentId,
                ShipmentDate = shipment.ShipmentDate,
                CashSalesInvoiceID = shipment.CashSalesInvoiceID,
                SalesOrderVoucherID = shipment.SalesOrderVoucherID,
                Source = shipment.Source,
                OrganizationCustomerId = shipment.OrganizationCustomerId,
                BranchId = shipment.BranchId,
                StoreID = shipment.StoreID,
                CRMPeriodId = shipment.CRMPeriodId,
                MethodOfShipment = shipment.MethodOfShipment,
                PlateNo = shipment.PlateNo,
                FsInvoiceNumber = shipment.FsInvoiceNumber,
                TrailerPlateNo = shipment.TrailerPlateNo,

                Status = shipment.Status,
                IsVoid = shipment.IsVoid,
                IsOnlineApproved = shipment.IsOnlineApproved,
                OnlineApprovedBy = shipment.OnlineApprovedBy,
                IsOnlineTransferred = shipment.IsOnlineTransferred,
                IsOnlineTransferredBy = shipment.IsOnlineTransferredBy,
                OnlineApprovedTime = shipment.OnlineApprovedTime,
                OnlineTransferredTime = shipment.OnlineTransferredTime,
                IsSendForApproval = shipment.IsSendForApproval,
                SendForApprovalBy = shipment.SendForApprovalBy,
                SendForApprovalTime = shipment.SendForApprovalTime,
                VoidBy = shipment.VoidBy,
                VoidTime = shipment.VoidTime,

                CreatedBy = shipment.CreatedBy,
                DateCreated = shipment.DateCreated,
                LastModified = shipment.LastModified,
                ModifiedBy = shipment.ModifiedBy,
                OrgBranchName = shipment.OrgBranchName,
                Remark = shipment.Remark
            });

            return Json(result);
        }
        /// <summary>
        /// returns sales shipment headers with  missing line item(s) from bin card
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizeRoles(CRMRoles.IncompleteShipmentAdmin)]
        public ActionResult PartiallyIncompleteShipments_Read([DataSourceRequest]DataSourceRequest request, int branchId = 0, int storeId = 0)
        {
            List<SalesShipment> listToReturn = new List<SalesShipment>();
            var allApprovedShipments = db.SalesShipments.Where(s => s.IsOnlineApproved && s.BranchId == branchId && s.StoreID == storeId).ToList();

            foreach (var shipment in allApprovedShipments)
            {
                var lines = db.SalesShipmentLineItem.Where(l => l.SalesShipmentId == shipment.SalesShipmentId).ToList();
                var binCardLines = db.BinCardLines.Where(b => b.ReferenceDoc == "Shipment " + shipment.SalesShipmentId).ToList();

                // there are missing items
                // in case of void, there may be more lines in bincard than in shipment
                // lines.Count != binCardLines.Count will fail in that case.
                if (lines.Count > binCardLines.Count && binCardLines.Count > 0)
                {
                    listToReturn.Add(shipment);
                }
            }


            DataSourceResult result = listToReturn.AsQueryable().ToDataSourceResult(request, shipment => new SalesShipment
            {
                SalesShipmentId = shipment.SalesShipmentId,
                ShipmentDate = shipment.ShipmentDate,
                CashSalesInvoiceID = shipment.CashSalesInvoiceID,
                SalesOrderVoucherID = shipment.SalesOrderVoucherID,
                Source = shipment.Source,
                OrganizationCustomerId = shipment.OrganizationCustomerId,
                BranchId = shipment.BranchId,
                StoreID = shipment.StoreID,
                CRMPeriodId = shipment.CRMPeriodId,
                MethodOfShipment = shipment.MethodOfShipment,
                PlateNo = shipment.PlateNo,
                FsInvoiceNumber = shipment.FsInvoiceNumber,
                TrailerPlateNo = shipment.TrailerPlateNo,

                Status = shipment.Status,
                IsVoid = shipment.IsVoid,
                IsOnlineApproved = shipment.IsOnlineApproved,
                OnlineApprovedBy = shipment.OnlineApprovedBy,
                IsOnlineTransferred = shipment.IsOnlineTransferred,
                IsOnlineTransferredBy = shipment.IsOnlineTransferredBy,
                OnlineApprovedTime = shipment.OnlineApprovedTime,
                OnlineTransferredTime = shipment.OnlineTransferredTime,
                IsSendForApproval = shipment.IsSendForApproval,
                SendForApprovalBy = shipment.SendForApprovalBy,
                SendForApprovalTime = shipment.SendForApprovalTime,
                VoidBy = shipment.VoidBy,
                VoidTime = shipment.VoidTime,

                CreatedBy = shipment.CreatedBy,
                DateCreated = shipment.DateCreated,
                LastModified = shipment.LastModified,
                ModifiedBy = shipment.ModifiedBy,
                OrgBranchName = shipment.OrgBranchName,
                Remark = shipment.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Shipment_Create([DataSourceRequest]DataSourceRequest request, SalesShipment shipment, string voucherDefinitionID)
        {
            if (shipment.Source == ShipmentSource.SalesInvoice)
            {
                var selectedInv = db.CashSalesInvoices.Find(shipment.CashSalesInvoiceID);
                if (selectedInv != null)
                {
                    shipment.FsInvoiceNumber = selectedInv.FsInvoiceNo;

                    if (shipment.StoreID == 0)
                    {
                        shipment.StoreID = selectedInv.StoreID;
                    }
                }
            }
            else if (shipment.Source == ShipmentSource.SalesOrder)
            {
                var selectedSO = db.SalesOrderVouchers.Find(shipment.SalesOrderVoucherID);
                if (selectedSO != null && shipment.StoreID == 0)
                {
                    shipment.StoreID = selectedSO.StoreID;
                }
            }
            if (shipment.OrganizationCustomerId == 0 && string.IsNullOrEmpty(shipment.NewCustomer))
            {
                ModelState.AddModelError("", "Select Existing customer or Insert Name of new one");
            }
            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (shipment.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CRMSalesHelper.CreateTempCustomer(shipment.NewCustomer);
                }
                else
                {
                    OrgCustomer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == shipment.OrganizationCustomerId).FirstOrDefault();
                }

                var entity = new SalesShipment
                {
                    CashSalesInvoiceID = shipment.CashSalesInvoiceID,
                    SalesOrderVoucherID = shipment.SalesOrderVoucherID,
                    ShipmentDate = shipment.ShipmentDate,
                    Status = OperationStatuses.New.ToString(),
                    Source = shipment.Source,
                    OrganizationCustomerId = OrgCustomer.OrganizationCustomerID,
                    BranchId = shipment.BranchId,
                    StoreID = shipment.StoreID,
                    CRMPeriodId = shipment.CRMPeriodId,
                    MethodOfShipment = shipment.MethodOfShipment,
                    PlateNo = shipment.PlateNo,
                    FsInvoiceNumber = shipment.FsInvoiceNumber,
                    TrailerPlateNo = shipment.TrailerPlateNo,
                    JobOrderNumber = shipment.JobOrderNumber,
                    StoreRequisitionNumber = shipment.StoreRequisitionNumber,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = shipment.Remark
                };

                db.SalesShipments.Add(entity);
                db.SaveChanges();
                shipment.SalesShipmentId = entity.SalesShipmentId;

                AddLineItemsOnHeaderCreate(entity, voucherDefinitionID);
                new CRMSalesHelper().UpdateInvoiceOrSalesOrderShipmentStatusOnShipmentEdit(entity);
                new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(entity);
                UserHelper.OperationLog(Request.UserHostAddress, "Create", "Success", User.Identity.Name, "Added new  Shipment Header  " + shipment.SalesShipmentId, Modules.CRM);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Shipment Create", "Success", User.Identity.Name, $"Shipment {entity.SalesShipmentId} created.", Modules.CRM);
                #endregion
            }

            return Json(new[] { shipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Shipment_Update([DataSourceRequest]DataSourceRequest request, SalesShipment shipment)
        {
            if (shipment.OrganizationCustomerId == 0 && string.IsNullOrEmpty(shipment.NewCustomer))
            {
                ModelState.AddModelError("", "Select Existing customer or Insert Name of new one");
            }
            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (shipment.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CRMSalesHelper.CreateTempCustomer(shipment.NewCustomer);
                }
                else
                {
                    OrgCustomer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == shipment.OrganizationCustomerId).FirstOrDefault();
                }
                var entity = new SalesShipment
                {
                    SalesShipmentId = shipment.SalesShipmentId,
                    ShipmentDate = shipment.ShipmentDate,
                    CashSalesInvoiceID = shipment.CashSalesInvoiceID,
                    SalesOrderVoucherID = shipment.SalesOrderVoucherID,
                    Source = shipment.Source,
                    OrganizationCustomerId = OrgCustomer.OrganizationCustomerID,
                    BranchId = shipment.BranchId,
                    StoreID = shipment.StoreID,
                    CRMPeriodId = shipment.CRMPeriodId,
                    MethodOfShipment = shipment.MethodOfShipment,
                    PlateNo = shipment.PlateNo,
                    FsInvoiceNumber = shipment.FsInvoiceNumber,
                    TrailerPlateNo = shipment.TrailerPlateNo,
                    JobOrderNumber = shipment.JobOrderNumber,
                    StoreRequisitionNumber = shipment.StoreRequisitionNumber,

                    Status = shipment.Status,
                    IsVoid = shipment.IsVoid,
                    IsOnlineApproved = shipment.IsOnlineApproved,
                    OnlineApprovedBy = shipment.OnlineApprovedBy,
                    IsOnlineTransferred = shipment.IsOnlineTransferred,
                    IsOnlineTransferredBy = shipment.IsOnlineTransferredBy,
                    OnlineApprovedTime = shipment.OnlineApprovedTime,
                    OnlineTransferredTime = shipment.OnlineTransferredTime,
                    IsSendForApproval = shipment.IsSendForApproval,
                    SendForApprovalBy = shipment.SendForApprovalBy,
                    SendForApprovalTime = shipment.SendForApprovalTime,
                    VoidBy = shipment.VoidBy,
                    VoidTime = shipment.VoidTime,

                    CreatedBy = shipment.CreatedBy,
                    DateCreated = shipment.DateCreated,
                    OrgBranchName = shipment.OrgBranchName,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                    Remark = shipment.Remark
                };

                db.SalesShipments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Shipment Edit", "Success", User.Identity.Name, $"Shipment {entity.SalesShipmentId} updated.", Modules.CRM);
                #endregion
            }

            return Json(new[] { shipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Shipment_Destroy([DataSourceRequest]DataSourceRequest request, SalesShipment shipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesShipment
                {
                    SalesShipmentId = shipment.SalesShipmentId,
                    ShipmentDate = shipment.ShipmentDate,
                    CashSalesInvoiceID = shipment.CashSalesInvoiceID,
                    SalesOrderVoucherID = shipment.SalesOrderVoucherID,
                    Source = shipment.Source,
                    OrganizationCustomerId = shipment.OrganizationCustomerId,
                    BranchId = shipment.BranchId,
                    StoreID = shipment.StoreID,
                    CRMPeriodId = shipment.CRMPeriodId,
                    MethodOfShipment = shipment.MethodOfShipment,
                    PlateNo = shipment.PlateNo,
                    FsInvoiceNumber = shipment.FsInvoiceNumber,
                    TrailerPlateNo = shipment.TrailerPlateNo,
                    JobOrderNumber = shipment.JobOrderNumber,
                    StoreRequisitionNumber = shipment.StoreRequisitionNumber,

                    Status = shipment.Status,
                    IsVoid = shipment.IsVoid,
                    IsOnlineApproved = shipment.IsOnlineApproved,
                    OnlineApprovedBy = shipment.OnlineApprovedBy,
                    IsOnlineTransferred = shipment.IsOnlineTransferred,
                    IsOnlineTransferredBy = shipment.IsOnlineTransferredBy,
                    OnlineApprovedTime = shipment.OnlineApprovedTime,
                    OnlineTransferredTime = shipment.OnlineTransferredTime,
                    IsSendForApproval = shipment.IsSendForApproval,
                    SendForApprovalBy = shipment.SendForApprovalBy,
                    SendForApprovalTime = shipment.SendForApprovalTime,
                    VoidBy = shipment.VoidBy,
                    VoidTime = shipment.VoidTime,

                    CreatedBy = shipment.CreatedBy,
                    DateCreated = shipment.DateCreated,
                    LastModified = shipment.LastModified,
                    ModifiedBy = shipment.ModifiedBy,
                    OrgBranchName = shipment.OrgBranchName,
                    Remark = shipment.Remark
                };

                db.SalesShipments.Attach(entity);
                db.SalesShipments.Remove(entity);
                db.SaveChanges();
                UpdateSalesInvoiceOrSalesOrderShippedStatus(entity);
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Shipment Header  " + shipment.SalesShipmentId, Modules.CRM);

            }

            return Json(new[] { shipment }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateSalesInvoiceOrSalesOrderShippedStatus(SalesShipment entity)
        {
            if (entity.Source == ShipmentSource.SalesOrder)
            {
                var order = db.SalesOrderVouchers.Find(entity.SalesOrderVoucherID);
                if (order != null)
                {
                    if (new CRMSalesHelper().SalesOrderVoucherHasUnShipedLineLitems(order, "1") == false)
                    {
                        order.IsShipped = true;

                        db.SalesOrderVouchers.Attach(order);
                        db.Entry(order).State = EntityState.Modified;
                    }
                    else
                    {
                        order.IsShipped = false;

                        db.SalesOrderVouchers.Attach(order);
                        db.Entry(order).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }

            }
            else
            {
                var invoice = db.CashSalesInvoices.Find(entity.CashSalesInvoiceID);
                if (invoice != null)
                {
                    if (new CRMSalesHelper().SalesInvoiceHasUnShipedLineLitems(invoice, "2") == false)
                    {
                        invoice.IsShipped = true;

                        db.CashSalesInvoices.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;
                    }
                    else
                    {
                        invoice.IsShipped = false;

                        db.CashSalesInvoices.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
        }

        #region Local Methods
        private void AddLineItemsOnHeaderCreate(SalesShipment header, string voucherDefinitionID)
        {
            if (header.Source == ShipmentSource.SalesInvoice)
            {
                var lineItemsInInvoice = db.LineItems.Where(li => li.Voucher == header.CashSalesInvoiceID.ToString() && li.VoucherDefinitionID == voucherDefinitionID.ToString()).ToList();

                if (lineItemsInInvoice.Any())
                {
                    foreach (LineItem lineItem in lineItemsInInvoice)
                    {
                        decimal remainingQty = lineItem.Quantity;
                        decimal shippedBefore = 0;
                        var existingShipmentLineItem = db.SalesShipmentLineItem.Where(li => li.LineItemId == lineItem.LineItemID).ToList();
                        if (existingShipmentLineItem.Any())
                        {
                            shippedBefore = existingShipmentLineItem.Sum(i => i.ShipedQuantity);
                            remainingQty = lineItem.Quantity - shippedBefore;
                        }
                        if (remainingQty > 0)
                        {
                            var shipmentLineItemRecord = new SalesShipmentLineItem
                            {
                                SalesShipmentId = header.SalesShipmentId,
                                LineItemId = lineItem.LineItemID,
                                ProductId = lineItem.ThingID,
                                SalesStoreId = lineItem.StoreID,
                                Price = lineItem.UnitAmount,
                                TotalQuantity = lineItem.Quantity,
                                SubTotal = lineItem.CalculatedCost,
                                TaxAmount = lineItem.TaxAmount,
                                TotalAmount = lineItem.TotalAmount,
                                ShipedQuantityTilPreShipment = shippedBefore,
                                ShipedQuantity = remainingQty,
                                RemainingQuantity = 0,

                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now,
                            };

                            db.SalesShipmentLineItem.Add(shipmentLineItemRecord);
                            db.SaveChanges();
                        }
                    }
                }
            }
            else if (header.Source == ShipmentSource.SalesOrder)
            {
                var lineItemsInSalesVoucher = db.LineItems.Where(li => li.Voucher == header.SalesOrderVoucherID.ToString() && li.VoucherDefinitionID == "1").ToList();

                if (lineItemsInSalesVoucher.Any())
                {
                    foreach (LineItem lineItem in lineItemsInSalesVoucher)
                    {
                        decimal remainingQty = lineItem.Quantity;
                        decimal shippedBefore = 0;
                        var existingShipmentLineItem = db.SalesShipmentLineItem.Where(li => li.LineItemId == lineItem.LineItemID).ToList();
                        if (existingShipmentLineItem.Any())
                        {
                            shippedBefore = existingShipmentLineItem.Sum(i => i.ShipedQuantity);
                            remainingQty = lineItem.Quantity - shippedBefore;
                        }
                        if (remainingQty > 0)
                        {
                            var shipmentLineItemRecord = new SalesShipmentLineItem
                            {
                                SalesShipmentId = header.SalesShipmentId,
                                LineItemId = lineItem.LineItemID,
                                ProductId = lineItem.ThingID,
                                SalesStoreId = lineItem.StoreID,
                                Price = lineItem.UnitAmount,
                                TotalQuantity = lineItem.Quantity,
                                SubTotal = lineItem.CalculatedCost,
                                TaxAmount = lineItem.TaxAmount,
                                TotalAmount = lineItem.TotalAmount,
                                ShipedQuantityTilPreShipment = shippedBefore,
                                ShipedQuantity = remainingQty,
                                RemainingQuantity = 0,

                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now,
                            };

                            db.SalesShipmentLineItem.Add(shipmentLineItemRecord);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        //public OrganizationCustomer CreateTempCustomer(string Name)
        //{
        //    var OrgCustomer = new OrganizationCustomer();
        //    var existingCustomerByThisName = db.OrganizationCustomers.FirstOrDefault(c => c.TradeName.ToLower() == Name.ToLower());
        //    if (existingCustomerByThisName != null)
        //    {
        //        return existingCustomerByThisName;
        //    }
        //    else
        //    {
        //        var OrgCustomerCategory = db.OrganizationCategories.FirstOrDefault();
        //        if (OrgCustomerCategory != null)
        //        {
        //            OrgCustomer.BrandName = Name;
        //            OrgCustomer.TradeName = Name;
        //            OrgCustomer.Category = OrgCustomerCategory.OrganizationCategoryID.ToString();
        //            OrgCustomer.BusinessType = "Business Type";
        //            OrgCustomer.CustomerType = CustomerType.LOCAL;
        //            OrgCustomer.IsTemporary = true;
        //            OrgCustomer.IsActive = false;
        //            OrgCustomer.IsCredit = false;

        //            db.OrganizationCustomers.Add(OrgCustomer);
        //            int result = db.SaveChanges();

        //            if (result == 1)
        //                return OrgCustomer;
        //        }
        //        return null;
        //    }
        //}
        #endregion
        #region public methods
        public JsonResult SendForApproval(int id)
        {
            var record = db.SalesShipments.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {
                #region validate line items
                var lineItems = db.SalesShipmentLineItem.Where(li => li.SalesShipmentId == record.SalesShipmentId).ToList();
                if (!lineItems.Any())
                {
                    var d = new
                    {
                        Message = "No Line Items found. Please add Line Items first",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (SalesShipmentLineItem lineItem in lineItems)
                    {
                        if (lineItem.TotalQuantity - (lineItem.ShipedQuantity + lineItem.ShipedQuantityTilPreShipment) < 0)
                        {
                            return Json(new { Message = "Some line items have invalid shipment quantity.", Status = "NOT_OK" });
                        }
                    }
                }
                #endregion

                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.SalesShipments.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Shipment not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
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
        [AuthorizeRoles(CRMRoles.CRMVoid)]
        public JsonResult Void(int id)
        {
            var record = db.SalesShipments.Find(id);
            var binCardLines = new List<BinCardLine>();
            if (record != null && record.IsOnlineApproved /*&& !record.IsVoid*/ && !record.IsOnlineTransferred)
            {
                #region remove linked sales quote lines
                if (record.Source == ShipmentSource.MaintenanceStoreRequisition)
                {
                    var salesQuoteOfCurrentJobOrder = db.SalesQuoteVouchers.FirstOrDefault(s => s.Source == SalesQuoteSource.Maintenance && s.JobOrderNumber == record.JobOrderNumber);
                    if (salesQuoteOfCurrentJobOrder != null && (salesQuoteOfCurrentJobOrder.LastObjectState == CRMVoucherStatusConstatns.Editing || salesQuoteOfCurrentJobOrder.LastObjectState == CRMConstants.UnForwarded))
                    {
                        var quoteLines = db.LineItems.Where(l => l.Voucher == salesQuoteOfCurrentJobOrder.SalesQuoteVoucherID.ToString() && l.VoucherDefinitionID == "0").ToList();
                        foreach (var quoteLine in quoteLines)
                        {
                            if (quoteLine.StoreRequisitionNumber == record.StoreRequisitionNumber && quoteLine.ShipmentNumber == record.SalesShipmentId.ToString())
                            {
                                db.LineItems.Attach(quoteLine);
                                db.LineItems.Remove(quoteLine);
                                db.SaveChanges();
                            }
                        }
                    }
                    else if (salesQuoteOfCurrentJobOrder != null && (salesQuoteOfCurrentJobOrder.IsMiddleOnlineApproved || salesQuoteOfCurrentJobOrder.IsSecondOnlineApproved || salesQuoteOfCurrentJobOrder.IsOnlineApproved))
                    {
                        return Json(new
                        {
                            Message = $"Sales quote {salesQuoteOfCurrentJobOrder.SalesQuoteVoucherID} is in {salesQuoteOfCurrentJobOrder.LastObjectState} state. You cannot void current shipment.",
                            Status = "NOT_OK"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                //record.LastObjectState = CRMConstants.UnForwarded;
                db.SalesShipments.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region upddate stock
                var shipmentLineItems = db.SalesShipmentLineItem.Where(li => li.SalesShipmentId == record.SalesShipmentId).ToList();
                foreach (SalesShipmentLineItem shipmentLineItem in shipmentLineItems)
                {
                    var salesStore = db.SalesStores.Find(shipmentLineItem.SalesStoreId);
                    InventoryStore Store =
                      db.InventoryStores.FirstOrDefault(
                          x => x.StoreCode == salesStore.StoreCode);
                    Product product =
                        db.Products.FirstOrDefault(x => x.ProductID == shipmentLineItem.ProductId);

                    StoreItemAssignment storeItem =
                        db.StoreItemAssignments
                        .Where(x => x.InventoryStore.StoreCode == Store.StoreCode &&
                                    x.InventoryItem.ItemCode == product.ItemCode)
                                    .OrderByDescending(x => x.StoreItemAssignmentID).FirstOrDefault();

                    var branchItem = db.BranchItemAssignments.Where(x => x.BranchId == record.BranchId).FirstOrDefault(x => x.ItemCode == product.ItemCode);
                    InventoryItem stockItem = db.InventoryItems.Find(product.ItemCode);

                    if (Store != null && product != null && storeItem != null)
                    {
                        decimal finalQuantity = storeItem.Balance + shipmentLineItem.ShipedQuantity;

                        storeItem.Balance = finalQuantity;

                        db.StoreItemAssignments.Attach(storeItem);
                        db.Entry(storeItem).State = EntityState.Modified;
                        //db.SaveChanges();



                        #region update bin card

                        #region bin card line
                        var uPrice = InventoryHelper.GetItemAveragePrice(product.ItemCode, Store.StoreCode);
                        BinCardLine binCardLine = new BinCardLine
                        {
                            StoreItemAssignment = storeItem,
                            ReferenceDoc = "Void Shipment " + record.SalesShipmentId.ToString(),
                            Reference = record.Source == ShipmentSource.SalesInvoice ? "Invoice " + record.CashSalesInvoiceID : "Sales Order " + record.SalesOrderVoucherID,
                            Date = record.ShipmentDate,
                            //IssuedQTY = shipmentLineItem.ShipedQuantity,
                            //IssuedUPrice = shipmentLineItem.Price,
                            //IssuedTPrice = shipmentLineItem.TotalAmount,
                            ReceivedQTY = shipmentLineItem.ShipedQuantity,
                            ReceivedUPrice = shipmentLineItem.Price,
                            ReceivedTPrice = shipmentLineItem.TotalAmount,

                            BalanceQTY = storeItem.Balance,
                            BalanceUPrice = uPrice,
                            BalanceTPrice = uPrice * storeItem.Balance,
                            IssuedTo = "",
                            //IssuedTo = shipmentRecord.OrganizationCustomerId.ToString(),
                            //new    
                            StoreCode = GetInventoryStoreCode(shipmentLineItem.SalesStoreId),
                            GLPeriodId = GetGlPeriod(record.CRMPeriodId),
                            TransactionType = BinCardTransactionTypes.Issue,
                            StatusType = BinCardTransactionStatus.Void,

                        };
                        db.BinCardLines.Add(binCardLine);
                        binCardLines.Add(binCardLine);
                        #endregion

                        #endregion

                    }

                    if (branchItem != null)
                    {
                        branchItem.Balance = branchItem.Balance + shipmentLineItem.ShipedQuantity;
                        db.Entry(branchItem).State = EntityState.Modified;
                        //_db.SaveChanges();

                    }
                    if (stockItem != null)
                    {
                        stockItem.Balance = stockItem.Balance + shipmentLineItem.ShipedQuantity;
                        db.Entry(stockItem).State = EntityState.Modified;
                        // _db.SaveChanges();
                    }
                }
                // save all changes
                db.SaveChanges();
                #endregion
                List<int> ids = binCardLines.Select(b => b.BinCardLineID).ToList();
                var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.AutoRecalculateBincardLines(ids));
                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
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

            var lineItems = db.SalesShipmentLineItem.Where(l => l.SalesShipmentId == record.SalesShipmentId).ToList();
            if (!lineItems.Any())
            {
                var formattedData = new
                {
                    Message = "Please add lines and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            //if (record.IsVoid)
            //{
            //    var formattedData = new
            //    {
            //        Message = "The record is already void. You can not void again.",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
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
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = db.SalesShipments.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var formattedData = new
                {
                    Message = "The record is already transffered, it can't be retransferred.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

                #endregion

            }
            if (record != null && !record.IsOnlineTransferred)
            {

                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                var distributions = db.SalesShipmentDistribution.Where(d => d.SalesShipmentId == record.SalesShipmentId).ToList();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                //var distribution = record.ArSalesOrderDistribution.ToList();
                if (distributions.Any())
                {
                    debit = distributions.Sum(x => x.Debit);
                    credit = distributions.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit <= 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit should be positive number.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period
                var period = db.GLPeriods.Where(x => x.DateFrom <= record.ShipmentDate.Date)
                    .Where(x => x.DateTo >= record.ShipmentDate.Date).ToList();
                if (period.Any())
                {
                    if (period.Count() == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name.ToString();
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod +
                                              " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var formattedData = new
                            {
                                Message = "Period not found.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the shipment date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There are no periods defined that contain the Shipment period and date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion

                if (distributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "CRM Shipment",
                        Description = !String.IsNullOrEmpty(description) ? description : "CRM Shipment",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "CRM Shipment",
                        SourcesName = "CRM",
                        JournalNumber = "CRMSHIPMENT." + record.SalesShipmentId.ToString(),
                        SourceId = record.CashSalesInvoiceID.ToString(),
                        Source = JournalSources.Sales.ToString(),
                        EffectiveDate = record.ShipmentDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId = 0,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = record.Remark,
                        ReferenceDoc = "",
                        InvoiceReferenceDoc = "",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distributions)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    record.OnlineTransferredTime = DateTime.Now;
                    record.IsOnlineTransferredBy = User.Identity.Name;

                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                    var formattedData = new
                    {
                        Message = "Successfully transfered.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion

            }
            else
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
        public JsonResult RegenerateDistribution(int id)
        {
            var record = db.SalesShipments.Find(id);
            if ((record != null && !record.IsOnlineTransferred))
            {
                new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(record);

                return Json(new { Message = $"Distribution Generated for Shipment number {record.SalesShipmentId}.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Shipment not found!",
                    Status = "NOT_OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is already transfered you cannot regenerate distribution.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        #endregion
        private static string GetInventoryStoreCode(int salesStoreId)
        {
            var db = new ExceedDbContext();
            return db.SalesStores.Find(salesStoreId)?.StoreCode;
        }
        private static int GetGlPeriod(int crmPeriodId)
        {
            var db = new ExceedDbContext();

            int id = 0;
            var crmPeriod = db.CRMPeriods.Find(crmPeriodId);
            if (crmPeriod != null)
            {
                var glPeriod = db.GLPeriods.FirstOrDefault(p => p.Name == crmPeriod.Name);
                if (glPeriod != null)
                {
                    id = glPeriod.GLPeriodId;
                }
            }
            return id;
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}