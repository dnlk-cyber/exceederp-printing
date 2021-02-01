using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderPartialPrintController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArSalesOrderPartialPrints_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderPartialPrint> arsalesorderpartialprints = _db.ArSalesOrderPartialPrints.Where(x=>x.ArSalesOrderId==id);
            DataSourceResult result = arsalesorderpartialprints.ToDataSourceResult(request, arSalesOrderPartialPrint => new {
                ArSalesOrderPartialPrintId = arSalesOrderPartialPrint.ArSalesOrderPartialPrintId,
                ArSalesOrderId = arSalesOrderPartialPrint.ArSalesOrderId,
                Date = arSalesOrderPartialPrint.Date,
                Description = arSalesOrderPartialPrint.Description,
                Status = arSalesOrderPartialPrint.Status,
                Remark = arSalesOrderPartialPrint.Remark,
                IsReceiptPrinted = arSalesOrderPartialPrint.IsReceiptPrinted,
                IsVoid = arSalesOrderPartialPrint.IsVoid,
                VoidBy = arSalesOrderPartialPrint.VoidBy,
                VoidTime = arSalesOrderPartialPrint.VoidTime,
                IsOnlineApproved = arSalesOrderPartialPrint.IsOnlineApproved,
                OnlineApprovedBy = arSalesOrderPartialPrint.OnlineApprovedBy,
                OnlineApprovedTime = arSalesOrderPartialPrint.OnlineApprovedTime,
                IsOnlineTransferred = arSalesOrderPartialPrint.IsOnlineTransferred,
                IsOnlineTransferredBy = arSalesOrderPartialPrint.IsOnlineTransferredBy,
                OnlineTransferredTime = arSalesOrderPartialPrint.OnlineTransferredTime,
                IsSendForApproval = arSalesOrderPartialPrint.IsSendForApproval,
                OrgBranchName = arSalesOrderPartialPrint.OrgBranchName,
                SendForApprovalBy = arSalesOrderPartialPrint.SendForApprovalBy,
                SendForApprovalTime = arSalesOrderPartialPrint.SendForApprovalTime,
                DateCreated = arSalesOrderPartialPrint.DateCreated,
                LastModified = arSalesOrderPartialPrint.LastModified,
                CreatedBy = arSalesOrderPartialPrint.CreatedBy,
                ModifiedBy = arSalesOrderPartialPrint.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPartialPrints_Create(int id,[DataSourceRequest]DataSourceRequest request, ArSalesOrderPartialPrint arSalesOrderPartialPrint)
        {
          
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPartialPrint
                {
                    ArSalesOrderId =id,
                    Date = arSalesOrderPartialPrint.Date,
                    Description = arSalesOrderPartialPrint.Description,
                    Status = arSalesOrderPartialPrint.Status,
                    Remark = arSalesOrderPartialPrint.Remark,
                    IsReceiptPrinted = arSalesOrderPartialPrint.IsReceiptPrinted,
                    IsVoid = arSalesOrderPartialPrint.IsVoid,
                    VoidBy = arSalesOrderPartialPrint.VoidBy,
                    VoidTime = arSalesOrderPartialPrint.VoidTime,
                    IsOnlineApproved = arSalesOrderPartialPrint.IsOnlineApproved,
                    OnlineApprovedBy = arSalesOrderPartialPrint.OnlineApprovedBy,
                    OnlineApprovedTime = arSalesOrderPartialPrint.OnlineApprovedTime,
                    IsOnlineTransferred = arSalesOrderPartialPrint.IsOnlineTransferred,
                    IsOnlineTransferredBy = arSalesOrderPartialPrint.IsOnlineTransferredBy,
                    OnlineTransferredTime = arSalesOrderPartialPrint.OnlineTransferredTime,
                    IsSendForApproval = arSalesOrderPartialPrint.IsSendForApproval,
                    OrgBranchName = arSalesOrderPartialPrint.OrgBranchName,
                    SendForApprovalBy = arSalesOrderPartialPrint.SendForApprovalBy,
                    SendForApprovalTime = arSalesOrderPartialPrint.SendForApprovalTime,
                    DateCreated = arSalesOrderPartialPrint.DateCreated,
                    LastModified = arSalesOrderPartialPrint.LastModified,
                    CreatedBy = arSalesOrderPartialPrint.CreatedBy,
                    ModifiedBy = arSalesOrderPartialPrint.ModifiedBy
                };

                _db.ArSalesOrderPartialPrints.Add(entity);
                _db.SaveChanges();
                arSalesOrderPartialPrint.ArSalesOrderPartialPrintId = entity.ArSalesOrderPartialPrintId;

                SalesOrderItems(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order partial print", "Success", User.Identity.Name, $"created sales order partial print {entity.ArSalesOrderPartialPrintId} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderPartialPrint }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPartialPrints_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderPartialPrint arSalesOrderPartialPrint)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPartialPrint
                {
                    ArSalesOrderId = arSalesOrderPartialPrint.ArSalesOrderId,
                    ArSalesOrderPartialPrintId = arSalesOrderPartialPrint.ArSalesOrderPartialPrintId,
                    Date = arSalesOrderPartialPrint.Date,
                    Description = arSalesOrderPartialPrint.Description,
                    Status = arSalesOrderPartialPrint.Status,
                    Remark = arSalesOrderPartialPrint.Remark,
                    IsReceiptPrinted = arSalesOrderPartialPrint.IsReceiptPrinted,
                    IsVoid = arSalesOrderPartialPrint.IsVoid,
                    VoidBy = arSalesOrderPartialPrint.VoidBy,
                    VoidTime = arSalesOrderPartialPrint.VoidTime,
                    IsOnlineApproved = arSalesOrderPartialPrint.IsOnlineApproved,
                    OnlineApprovedBy = arSalesOrderPartialPrint.OnlineApprovedBy,
                    OnlineApprovedTime = arSalesOrderPartialPrint.OnlineApprovedTime,
                    IsOnlineTransferred = arSalesOrderPartialPrint.IsOnlineTransferred,
                    IsOnlineTransferredBy = arSalesOrderPartialPrint.IsOnlineTransferredBy,
                    OnlineTransferredTime = arSalesOrderPartialPrint.OnlineTransferredTime,
                    IsSendForApproval = arSalesOrderPartialPrint.IsSendForApproval,
                    OrgBranchName = arSalesOrderPartialPrint.OrgBranchName,
                    SendForApprovalBy = arSalesOrderPartialPrint.SendForApprovalBy,
                    SendForApprovalTime = arSalesOrderPartialPrint.SendForApprovalTime,
                    DateCreated = arSalesOrderPartialPrint.DateCreated,
                    LastModified = arSalesOrderPartialPrint.LastModified,
                    CreatedBy = arSalesOrderPartialPrint.CreatedBy,
                    ModifiedBy = arSalesOrderPartialPrint.ModifiedBy
                };

                _db.ArSalesOrderPartialPrints.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order partial print", "Success", User.Identity.Name, $"updated sales order partial print {entity.ArSalesOrderPartialPrintId} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderPartialPrint }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPartialPrints_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderPartialPrint arSalesOrderPartialPrint)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPartialPrint
                {
                    ArSalesOrderPartialPrintId = arSalesOrderPartialPrint.ArSalesOrderPartialPrintId,
                    ArSalesOrderId = arSalesOrderPartialPrint.ArSalesOrderId,
                    Date = arSalesOrderPartialPrint.Date,
                    Description = arSalesOrderPartialPrint.Description,
                    Status = arSalesOrderPartialPrint.Status,
                    Remark = arSalesOrderPartialPrint.Remark,
                    IsReceiptPrinted = arSalesOrderPartialPrint.IsReceiptPrinted,
                    IsVoid = arSalesOrderPartialPrint.IsVoid,
                    VoidBy = arSalesOrderPartialPrint.VoidBy,
                    VoidTime = arSalesOrderPartialPrint.VoidTime,
                    IsOnlineApproved = arSalesOrderPartialPrint.IsOnlineApproved,
                    OnlineApprovedBy = arSalesOrderPartialPrint.OnlineApprovedBy,
                    OnlineApprovedTime = arSalesOrderPartialPrint.OnlineApprovedTime,
                    IsOnlineTransferred = arSalesOrderPartialPrint.IsOnlineTransferred,
                    IsOnlineTransferredBy = arSalesOrderPartialPrint.IsOnlineTransferredBy,
                    OnlineTransferredTime = arSalesOrderPartialPrint.OnlineTransferredTime,
                    IsSendForApproval = arSalesOrderPartialPrint.IsSendForApproval,
                    OrgBranchName = arSalesOrderPartialPrint.OrgBranchName,
                    SendForApprovalBy = arSalesOrderPartialPrint.SendForApprovalBy,
                    SendForApprovalTime = arSalesOrderPartialPrint.SendForApprovalTime,
                    DateCreated = arSalesOrderPartialPrint.DateCreated,
                    LastModified = arSalesOrderPartialPrint.LastModified,
                    CreatedBy = arSalesOrderPartialPrint.CreatedBy,
                    ModifiedBy = arSalesOrderPartialPrint.ModifiedBy
                };

                _db.ArSalesOrderPartialPrints.Attach(entity);
                _db.ArSalesOrderPartialPrints.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderPartialPrint }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderReceipt)]
        public JsonResult SendforApproval(string id)
        {
            int myid = int.Parse(id);
            var record = _db.ArSalesOrderPartialPrints.Find(myid);
            #region check segment length validation

            var distributions = _db.ArSalesOrderPartialPrintDistributions.Where(x => x.ArSalesOrderPartialPrintId == myid).ToList();
            AccountHelper accountHelper = new AccountHelper();
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

            }


            #endregion
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ArSalesOrderPartials.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ArSalesOrderPartialPrints.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }
            if (record != null && record.ArSalesOrderPartials.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "You cannot manipulate this record.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "This record is already sent for approval.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            decimal sumTotalinvoiceLines = 0;
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            try
            {
                sumTotalinvoiceLines = record.ArSalesOrderPartials.Sum(x => x.TotalAmount);

            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add sales lines and try again",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            //try
            //{
            //    sumTotalDisDebit = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).Sum(x => x.Debit);
            //    sumTotalDisCredit = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).Sum(x => x.Credit);
            //}
            //catch (Exception e)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Please add distribution and try again",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}

            //if (sumTotalDisDebit != sumTotalDisCredit)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Total Debit and Total Credit is not equal.Please correct it and try again",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            //if (sumTotalDisDebit != sumTotalinvoiceLines)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Total of distribution and total of invoice lines is not equal.Please correct it and try again",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            if (!record.IsOnlineApproved)
            {

            }
            return null;
        }

        private void SalesOrderItems(ArSalesOrderPartialPrint partialPrint)
        {
            var salesorderline = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == partialPrint.ArSalesOrderId).ToList();

            foreach (var arSalesOrderPartial in salesorderline)
            {
                var partials = _db.ArSalesOrderPartials
                    .Where(x => x.SubItem == arSalesOrderPartial.SubItem)
                    .Where(x => x.UnitPrice == arSalesOrderPartial.UnitPrice)
                    .Where(x => x.ArSalesOrderPartialPrint.ArSalesOrderId == partialPrint.ArSalesOrderId).AsNoTracking().ToList();

                decimal orignalQty = arSalesOrderPartial.Quantity;
                decimal totalPartial = 0;
                if (partials.Any())
                    totalPartial = partials.Sum(x => x.Quantity);//+ arSalesOrderPartial.Quantity;

                if (orignalQty > totalPartial)
                {
                    var entity = new ArSalesOrderPartial
                    {
                        ArSalesOrderPartialPrintId = partialPrint.ArSalesOrderPartialPrintId,
                        ItemName = arSalesOrderPartial.ItemName,
                        SubItem = arSalesOrderPartial.SubItem,
                        AccountCode = arSalesOrderPartial.AccountCode,
                        AccountDesc = arSalesOrderPartial.AccountDesc,
                        UoM = arSalesOrderPartial.UoM,
                        Quantity = orignalQty- totalPartial,
                        UnitPrice = arSalesOrderPartial.UnitPrice,
                        SalesPrice = arSalesOrderPartial.SalesPrice,
                        Discount = arSalesOrderPartial.Discount,
                        Commission = arSalesOrderPartial.Commission,
                        SubTotal = arSalesOrderPartial.SubTotal,
                        GLTaxRateID = arSalesOrderPartial.GLTaxRateID,
                        TaxAmount = arSalesOrderPartial.TaxAmount,
                        TotalAmount = arSalesOrderPartial.TotalAmount,
                        OrgBranchName = arSalesOrderPartial.OrgBranchName,
                        DateCreated = arSalesOrderPartial.DateCreated,
                        LastModified = arSalesOrderPartial.LastModified,
                        CreatedBy = arSalesOrderPartial.CreatedBy,
                        ModifiedBy = arSalesOrderPartial.ModifiedBy
                    };

                    _db.ArSalesOrderPartials.Add(entity);
                    _db.SaveChanges();
               }
            }
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
