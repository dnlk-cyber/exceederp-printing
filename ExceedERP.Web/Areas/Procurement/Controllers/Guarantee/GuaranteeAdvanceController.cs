using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Procurement;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;


namespace ExceedERP.Web.Areas.Procurement.Controllers.Guarantee
{
    public class GuaranteeAdvanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser, ProcurementRoles.ProcurementBondApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser, ProcurementRoles.ProcurementBondApprover)]
        public ActionResult Guarantees_Read([DataSourceRequest]DataSourceRequest request, bool isExpired, bool isSent)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<Core.Domain.Procurement.Guarantee.Guarantee> guarantees = db.Guarantees.Where(x => x.GuaranteeType == GuaranteeTypes.AdvancePayment).Where(x => mybraches.Contains(x.BranchId));

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                guarantees = guarantees.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }
           
            if (isExpired)
            {
                List<int> expiredList = GuaranteeHelper.GetExpiredList();
                guarantees = guarantees.Where(x => expiredList.Contains(x.GuaranteeId))
                    .Where(x=>x.RenewalRequests.Count==0);
            }
            else if(isSent)
            {
                List<int> renewalList = GuaranteeHelper.GetRenewalList();
                guarantees = guarantees.Where(x => renewalList.Contains(x.GuaranteeId));
            }

            DataSourceResult result = guarantees.ToDataSourceResult(request, guarantee => new
            {
                GuaranteeId = guarantee.GuaranteeId,
                Purpose = guarantee.Purpose,
                SupplierName = guarantee.SupplierName,
                SupplierId = guarantee.SupplierId,
                ProcurementCategory = guarantee.ProcurementCategory,
                GuaranteeType = guarantee.GuaranteeType,
                GuaranteeMode = guarantee.GuaranteeMode,
                GuaranteeRefNo = guarantee.GuaranteeRefNo,
                ProcurementRefNo = guarantee.ProcurementRefNo,
                ContractRefNo = guarantee.ContractRefNo,
                Amount = guarantee.Amount,
                Currency = guarantee.Currency,
                IssueDate = guarantee.IssueDate,
                Bank = guarantee.Bank,
                BankBranch = guarantee.BankBranch,
                ValidityPeriod = guarantee.ValidityPeriod,
                ExpireDate = guarantee.ExpireDate,
                ValidityStatus = guarantee.ValidityStatus,
                RenewalStatus =  guarantee.RenewalStatus,
                IsExpired=GuaranteeHelper.IsGuaranteeExpired(guarantee.GuaranteeId),

                BranchId = guarantee.BranchId,
                BusinessUnitId = guarantee.BusinessUnitId,
               
                Status = guarantee.Status,
                IsOnlineApproved = guarantee.IsOnlineApproved,
                OnlineApprovedBy = guarantee.OnlineApprovedBy,
                OnlineApprovedTime = guarantee.OnlineApprovedTime,
                IsSendForApproval = guarantee.IsSendForApproval,
                SendForApprovalBy = guarantee.SendForApprovalBy,
                SendForApprovalTime = guarantee.SendForApprovalTime,
                isVoid = guarantee.isVoid,
                VoidBy = guarantee.VoidBy,
                VoidTime = guarantee.VoidTime,
                OrgBranchName = guarantee.OrgBranchName,
                DateCreated = guarantee.DateCreated,
                LastModified = guarantee.LastModified,
                CreatedBy = guarantee.CreatedBy,
                ModifiedBy = guarantee.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Guarantees_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.Guarantee.Guarantee guarantee)
        {
            if (guarantee.GuaranteeType != GuaranteeTypes.AdvancePayment)
                ModelState.AddModelError("notallowed", "You can add only advance gurantee");

            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.Guarantee.Guarantee
                {
                    BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
                    BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                    Purpose = guarantee.Purpose,
                    SupplierName = guarantee.SupplierName,
                    SupplierId = guarantee.SupplierId,
                    ProcurementCategory = guarantee.ProcurementCategory,
                    GuaranteeType = guarantee.GuaranteeType,
                    GuaranteeMode = guarantee.GuaranteeMode,
                    GuaranteeRefNo = guarantee.GuaranteeRefNo,
                    ProcurementRefNo = guarantee.ProcurementRefNo,
                    ContractRefNo = guarantee.ContractRefNo,
                    Amount = guarantee.Amount,
                    Currency = guarantee.Currency,
                    IssueDate = guarantee.IssueDate.Date,
                    Bank = guarantee.Bank,
                    BankBranch = guarantee.BankBranch,
                    ValidityPeriod = guarantee.ValidityPeriod,
                    ExpireDate = guarantee.IssueDate.AddDays(guarantee.ValidityPeriod),
                    ValidityStatus = guarantee.ValidityStatus,
                    RenewalStatus = guarantee.RenewalStatus,

                    IsSendForApproval = guarantee.IsSendForApproval,
                    Status = guarantee.Status,
                    IsOnlineApproved = guarantee.IsOnlineApproved,
                    OnlineApprovedBy = guarantee.OnlineApprovedBy,
                    OnlineApprovedTime = guarantee.OnlineApprovedTime,
                    isVoid = guarantee.isVoid,
                    VoidBy = guarantee.VoidBy,
                    VoidTime = guarantee.VoidTime,
                    OrgBranchName = guarantee.OrgBranchName,
                    DateCreated = guarantee.DateCreated,
                    LastModified = guarantee.LastModified,
                    CreatedBy = guarantee.CreatedBy,
                    ModifiedBy = guarantee.ModifiedBy
                };

                db.Guarantees.Add(entity);
                db.SaveChanges();
                guarantee.GuaranteeId = entity.GuaranteeId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "guarantee management", "Success", User.Identity.Name, $"created guarantee {entity.GuaranteeId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { guarantee }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Guarantees_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.Guarantee.Guarantee guarantee)
        {
            if (guarantee.GuaranteeType != GuaranteeTypes.AdvancePayment)
                ModelState.AddModelError("notallowed", "You can add only advance gurantee");

            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.Guarantee.Guarantee
                {
                    GuaranteeId = guarantee.GuaranteeId,
                    BranchId = guarantee.BranchId,
                    BusinessUnitId = guarantee.BusinessUnitId,
                    Purpose = guarantee.Purpose,
                    SupplierName = guarantee.SupplierName,
                    SupplierId = guarantee.SupplierId,
                    ProcurementCategory = guarantee.ProcurementCategory,
                    GuaranteeType = guarantee.GuaranteeType,
                    GuaranteeMode = guarantee.GuaranteeMode,
                    GuaranteeRefNo = guarantee.GuaranteeRefNo,
                    ProcurementRefNo = guarantee.ProcurementRefNo,
                    ContractRefNo = guarantee.ContractRefNo,
                    Amount = guarantee.Amount,
                    Currency = guarantee.Currency,
                    IssueDate = guarantee.IssueDate,
                    Bank = guarantee.Bank,
                    BankBranch = guarantee.BankBranch,
                    ValidityPeriod = guarantee.ValidityPeriod,
                    ExpireDate = guarantee.IssueDate.AddDays(guarantee.ValidityPeriod),
                    ValidityStatus = guarantee.ValidityStatus,
                    RenewalStatus = guarantee.RenewalStatus,

                    IsSendForApproval = guarantee.IsSendForApproval,
                    Status = guarantee.Status,
                    IsOnlineApproved = guarantee.IsOnlineApproved,
                    OnlineApprovedBy = guarantee.OnlineApprovedBy,
                    OnlineApprovedTime = guarantee.OnlineApprovedTime,
                    isVoid = guarantee.isVoid,
                    VoidBy = guarantee.VoidBy,
                    VoidTime = guarantee.VoidTime,
                    OrgBranchName = guarantee.OrgBranchName,
                    DateCreated = guarantee.DateCreated,
                    LastModified = guarantee.LastModified,
                    CreatedBy = guarantee.CreatedBy,
                    ModifiedBy = guarantee.ModifiedBy
                };

                db.Guarantees.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "guarantee management", "Success", User.Identity.Name, $"updated guarantee {entity.GuaranteeId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { guarantee }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Guarantees_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.Guarantee.Guarantee guarantee)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.Guarantee.Guarantee
                {
                    GuaranteeId = guarantee.GuaranteeId,
                    BranchId = guarantee.BranchId,
                    BusinessUnitId = guarantee.BusinessUnitId,
                    Purpose = guarantee.Purpose,
                    SupplierName = guarantee.SupplierName,
                    SupplierId = guarantee.SupplierId,
                    ProcurementCategory = guarantee.ProcurementCategory,
                    GuaranteeType = guarantee.GuaranteeType,
                    GuaranteeMode = guarantee.GuaranteeMode,
                    GuaranteeRefNo = guarantee.GuaranteeRefNo,
                    ProcurementRefNo = guarantee.ProcurementRefNo,
                    ContractRefNo = guarantee.ContractRefNo,
                    Amount = guarantee.Amount,
                    Currency = guarantee.Currency,
                    IssueDate = guarantee.IssueDate,
                    Bank = guarantee.Bank,
                    BankBranch = guarantee.BankBranch,
                    ValidityPeriod = guarantee.ValidityPeriod,
                    ExpireDate = guarantee.ExpireDate,
                    ValidityStatus = guarantee.ValidityStatus,
                    RenewalStatus = guarantee.RenewalStatus,

                    IsSendForApproval = guarantee.IsSendForApproval,
                    Status = guarantee.Status,
                    IsOnlineApproved = guarantee.IsOnlineApproved,
                    OnlineApprovedBy = guarantee.OnlineApprovedBy,
                    OnlineApprovedTime = guarantee.OnlineApprovedTime,
                    isVoid = guarantee.isVoid,
                    VoidBy = guarantee.VoidBy,
                    VoidTime = guarantee.VoidTime,
                    OrgBranchName = guarantee.OrgBranchName,
                    DateCreated = guarantee.DateCreated,
                    LastModified = guarantee.LastModified,
                    CreatedBy = guarantee.CreatedBy,
                    ModifiedBy = guarantee.ModifiedBy
                };

                db.Guarantees.Attach(entity);
                db.Guarantees.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { guarantee }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.Guarantees.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.Guarantees.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
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
        

       [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        public JsonResult Release(int id)
        {
            var record = db.Guarantees.Find(id);
            if (record != null)
            {
                record.ValidityStatus = GuaranteeValidityStatus.Released;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Updated.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

          
            return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        public JsonResult Forfeit(int id)
        {
            var record = db.Guarantees.Find(id);
            if (record != null)
            {
                record.ValidityStatus = GuaranteeValidityStatus.Forfeited;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Updated.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
