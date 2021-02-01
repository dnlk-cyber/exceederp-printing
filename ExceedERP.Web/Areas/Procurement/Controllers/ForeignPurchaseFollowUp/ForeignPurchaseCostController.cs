using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.ForeignPurchaseFollowUp
{
    public class ForeignPurchaseCostController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseUser, ProcurementConstants.ProcurementForeignPurchaseApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseUser, ProcurementConstants.ProcurementForeignPurchaseApprover)]
        public ActionResult ForeignPurchaseCosts_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ForeignPurchaseCost> foreignpurchasecosts = _db.ForeignPurchaseCosts.Where(x=>x.ForeignPurchaseId==id);
            DataSourceResult result = foreignpurchasecosts.ToDataSourceResult(request, foreignPurchaseCost => new {
                ForeignPurchaseCostId = foreignPurchaseCost.ForeignPurchaseCostId,
                ForeignPurchaseId = foreignPurchaseCost.ForeignPurchaseId,
                Date = foreignPurchaseCost.Date,
                Payee = foreignPurchaseCost.Payee,
                CostType = foreignPurchaseCost.CostType,
                Amount = foreignPurchaseCost.Amount,
                Currency = foreignPurchaseCost.Currency,
                ExchangeRate = foreignPurchaseCost.ExchangeRate,
                ForexAmount = foreignPurchaseCost.ForexAmount,
                Paid = foreignPurchaseCost.Paid,

                Status = foreignPurchaseCost.Status,

                IsOnlineApproved = foreignPurchaseCost.IsOnlineApproved,
                OnlineApprovedBy = foreignPurchaseCost.OnlineApprovedBy,
                OnlineApprovedTime = foreignPurchaseCost.OnlineApprovedTime,
                IsSendForApproval = foreignPurchaseCost.IsSendForApproval,
                SendForApprovalBy = foreignPurchaseCost.SendForApprovalBy,
                SendForApprovalTime = foreignPurchaseCost.SendForApprovalTime,
                IsVoid = foreignPurchaseCost.IsVoid,
                VoidBy = foreignPurchaseCost.VoidBy,
                VoidTime = foreignPurchaseCost.VoidTime,
                OrgBranchName = foreignPurchaseCost.OrgBranchName,
                DateCreated = foreignPurchaseCost.DateCreated,
                LastModified = foreignPurchaseCost.LastModified,
                CreatedBy = foreignPurchaseCost.CreatedBy,
                ModifiedBy = foreignPurchaseCost.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseCosts_Create(int id,[DataSourceRequest]DataSourceRequest request, ForeignPurchaseCost foreignPurchaseCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseCost
                {
                    ForeignPurchaseId=id,
                    Date = foreignPurchaseCost.Date,
                    Payee = foreignPurchaseCost.Payee,
                    CostType = foreignPurchaseCost.CostType,
                    Amount = foreignPurchaseCost.ForexAmount* foreignPurchaseCost.ExchangeRate,
                    Currency = foreignPurchaseCost.Currency,
                    ExchangeRate = foreignPurchaseCost.ExchangeRate,
                    ForexAmount = foreignPurchaseCost.ForexAmount,
                    Paid = foreignPurchaseCost.Paid,

                    Status = foreignPurchaseCost.Status,

                    IsOnlineApproved = foreignPurchaseCost.IsOnlineApproved,
                    OnlineApprovedBy = foreignPurchaseCost.OnlineApprovedBy,
                    OnlineApprovedTime = foreignPurchaseCost.OnlineApprovedTime,
                    IsSendForApproval = foreignPurchaseCost.IsSendForApproval,
                    SendForApprovalBy = foreignPurchaseCost.SendForApprovalBy,
                    SendForApprovalTime = foreignPurchaseCost.SendForApprovalTime,
                    IsVoid = foreignPurchaseCost.IsVoid,
                    VoidBy = foreignPurchaseCost.VoidBy,
                    VoidTime = foreignPurchaseCost.VoidTime,
                    OrgBranchName = foreignPurchaseCost.OrgBranchName,
                    DateCreated = foreignPurchaseCost.DateCreated,
                    LastModified = foreignPurchaseCost.LastModified,
                    CreatedBy = foreignPurchaseCost.CreatedBy,
                    ModifiedBy = foreignPurchaseCost.ModifiedBy
                };

                _db.ForeignPurchaseCosts.Add(entity);
                _db.SaveChanges();
                foreignPurchaseCost.ForeignPurchaseCostId = entity.ForeignPurchaseCostId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "foreign purchase cost", "Success", User.Identity.Name, $"created cost {entity.Amount} under record {entity.ForeignPurchaseId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseCost }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseCosts_Update([DataSourceRequest]DataSourceRequest request, ForeignPurchaseCost foreignPurchaseCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseCost
                {
                    ForeignPurchaseId=foreignPurchaseCost.ForeignPurchaseId,
                    ForeignPurchaseCostId = foreignPurchaseCost.ForeignPurchaseCostId,
                    Date = foreignPurchaseCost.Date,
                    Payee = foreignPurchaseCost.Payee,
                    CostType = foreignPurchaseCost.CostType,
                    Amount = foreignPurchaseCost.ForexAmount * foreignPurchaseCost.ExchangeRate,
                    Currency = foreignPurchaseCost.Currency,
                    ExchangeRate = foreignPurchaseCost.ExchangeRate,
                    ForexAmount = foreignPurchaseCost.ForexAmount,
                    Paid = foreignPurchaseCost.Paid,

                    Status = foreignPurchaseCost.Status,

                    IsOnlineApproved = foreignPurchaseCost.IsOnlineApproved,
                    OnlineApprovedBy = foreignPurchaseCost.OnlineApprovedBy,
                    OnlineApprovedTime = foreignPurchaseCost.OnlineApprovedTime,
                    IsSendForApproval = foreignPurchaseCost.IsSendForApproval,
                    SendForApprovalBy = foreignPurchaseCost.SendForApprovalBy,
                    SendForApprovalTime = foreignPurchaseCost.SendForApprovalTime,
                    IsVoid = foreignPurchaseCost.IsVoid,
                    VoidBy = foreignPurchaseCost.VoidBy,
                    VoidTime = foreignPurchaseCost.VoidTime,
                    OrgBranchName = foreignPurchaseCost.OrgBranchName,
                    DateCreated = foreignPurchaseCost.DateCreated,
                    LastModified = foreignPurchaseCost.LastModified,
                    CreatedBy = foreignPurchaseCost.CreatedBy,
                    ModifiedBy = foreignPurchaseCost.ModifiedBy
                };

                _db.ForeignPurchaseCosts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "foreign purchase cost", "Success", User.Identity.Name, $"updated cost {entity.Amount} under record {entity.ForeignPurchaseId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseCost }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseCosts_Destroy([DataSourceRequest]DataSourceRequest request, ForeignPurchaseCost foreignPurchaseCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseCost
                {
                    ForeignPurchaseCostId = foreignPurchaseCost.ForeignPurchaseCostId,
                    Date = foreignPurchaseCost.Date,
                    Payee = foreignPurchaseCost.Payee,
                    CostType = foreignPurchaseCost.CostType,
                    Amount = foreignPurchaseCost.Amount,
                    Currency = foreignPurchaseCost.Currency,
                    ExchangeRate = foreignPurchaseCost.ExchangeRate,
                    ForexAmount = foreignPurchaseCost.ForexAmount,
                    Paid = foreignPurchaseCost.Paid,

                    Status = foreignPurchaseCost.Status,

                    IsOnlineApproved = foreignPurchaseCost.IsOnlineApproved,
                    OnlineApprovedBy = foreignPurchaseCost.OnlineApprovedBy,
                    OnlineApprovedTime = foreignPurchaseCost.OnlineApprovedTime,
                    IsSendForApproval = foreignPurchaseCost.IsSendForApproval,
                    SendForApprovalBy = foreignPurchaseCost.SendForApprovalBy,
                    SendForApprovalTime = foreignPurchaseCost.SendForApprovalTime,
                    IsVoid = foreignPurchaseCost.IsVoid,
                    VoidBy = foreignPurchaseCost.VoidBy,
                    VoidTime = foreignPurchaseCost.VoidTime,
                    OrgBranchName = foreignPurchaseCost.OrgBranchName,
                    DateCreated = foreignPurchaseCost.DateCreated,
                    LastModified = foreignPurchaseCost.LastModified,
                    CreatedBy = foreignPurchaseCost.CreatedBy,
                    ModifiedBy = foreignPurchaseCost.ModifiedBy
                };

                _db.ForeignPurchaseCosts.Attach(entity);
                _db.ForeignPurchaseCosts.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseCost }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ForeignPurchaseCosts.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
               
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ForeignPurchaseCosts.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Successfully Sent.",
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

        [AuthorizeRoles(ProcurementConstants.ProcurementVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ForeignPurchaseCosts.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid)
            {
                var poreceivetypes = _db.ReceiveTypes.Where(x => x.Type == GoodReceiveType.Tender)
                    .Select(x => x.ReceiveTypeId).ToList();

                #region validate payment

                var payment = _db.ApDirectPayments
                    .Where(x => x.IsVoid == false)
                    .Where(x => x.IsOnlineApproved)
                   
                    .FirstOrDefault(x => x.SourceId == id.ToString());

                if (payment != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This cost can not be void since it is referred in direct payment  {payment.ApDirectPaymentId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
               
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ForeignPurchaseCosts.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

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
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
