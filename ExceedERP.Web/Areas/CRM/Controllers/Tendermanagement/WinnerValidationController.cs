using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM.TenderManagement;
using ExceedERP.Core.Domain.Inventory;
//using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	public class WinnerValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        // [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover, ProcurementConstants.ProcurementForeignPurchaseApprover)]
        public ActionResult Index()
        {
            return View();
        }

        // [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover, FinanceConstants.AccountPayableView, ProcurementConstants.ProcurementForeignPurchaseUser)]
        public ActionResult ForeignPurchaseValidations_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TenderWinnerValidation> winnerValidations = _db.TenderWinnerValidations.Where(x => x.SalesLotId == id);
            DataSourceResult result = winnerValidations.ToDataSourceResult(request, foreignPurchaseValidation => new {
                TenderWinnerValidationId = foreignPurchaseValidation.TenderWinnerValidationId,
                SalesLotId= foreignPurchaseValidation.SalesLotId,
                Status = foreignPurchaseValidation.Status,
                Remark = foreignPurchaseValidation.Remark,
                DateCreated = foreignPurchaseValidation.DateCreated,
                LastModified = foreignPurchaseValidation.LastModified,
                CreatedBy = foreignPurchaseValidation.CreatedBy,
                ModifiedBy = foreignPurchaseValidation.ModifiedBy
            });

            return Json(result);
        }

        //[AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseValidations_Create(int id, [DataSourceRequest]DataSourceRequest request, TenderWinnerValidation winnerValidation)
        {
            var record = _db.SalesLots.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (record != null && !record.IsSendForApproval)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: Record is Not Send Approval");
            }

             if (record != null && record.IsSendForInvoice)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record:Already Send For Invoice");
            }
            
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (winnerValidation.Status == ApprovalStatuses.Approve)
                    {
                        var updateTender = _db.SaleTenders.Find(record.SaleTenderId);
                        if(updateTender!=null)
                        {
                            updateTender.IsOnlineApproved = true;
                            updateTender.IsSendForApproval = true;
                            _db.Entry(updateTender).State = EntityState.Modified;
                        }
                        record.IsOnlineApproved = true;
                        record.IsVoid = false;
                        record.Status = "Approved";
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.SalesLots.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (winnerValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.SalesLots.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new TenderWinnerValidation
                    {
                        SalesLotId = id,
                        Status = winnerValidation.Status,
                        Remark = winnerValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = winnerValidation.LastModified,
                        CreatedBy =User.Identity.Name,
                        ModifiedBy = winnerValidation.ModifiedBy
                    };

                    _db.TenderWinnerValidations.Add(entity);
                    _db.SaveChanges();
                    winnerValidation.SalesLotId = entity.SalesLotId;
                    #region operation log
                   // UserHelper.OperationLog(Request.UserHostAddress, "foreign purchase approval", "Success", User.Identity.Name, $"approved foreign purchase {entity.ForeignPurchaseId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { winnerValidation }.ToDataSourceResult(request, ModelState));
        }

        // [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseValidations_Update([DataSourceRequest]DataSourceRequest request, TenderWinnerValidation winnerValidation)
        {
            var record = _db.SalesLots.Find(winnerValidation.SalesLotId);
            if (ModelState.IsValid)
            {
                if (winnerValidation.Status == ApprovalStatuses.Approve)
                {
                    var updateTender = _db.SaleTenders.Find(record.SaleTenderId);
                    if (updateTender != null)
                    {
                        updateTender.IsOnlineApproved = true;
                        updateTender.IsSendForApproval = true;
                        updateTender.IsVoid = false;
                        _db.Entry(updateTender).State = EntityState.Modified;
                    }
                    record.IsOnlineApproved = true;
                    record.Status = "Approved";
                    record.OnlineApprovedBy = User.Identity.Name;
                    record.OnlineApprovedTime = DateTime.Now.Date;
                    record.IsOnlineApproved = true;
                    record.IsVoid = false;
                    //_db.SalesLots.Attach(record);
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                if (winnerValidation.Status == ApprovalStatuses.Reject)
                {

                    record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                    record.OnlineApprovedBy = User.Identity.Name;
                    record.OnlineApprovedTime = DateTime.Now.Date;
                    _db.SalesLots.Attach(record);
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                var entity = new TenderWinnerValidation
                {
                    SalesLotId = winnerValidation.SalesLotId,
                    Status = winnerValidation.Status,
                    Remark = winnerValidation.Remark,
                    TenderWinnerValidationId=winnerValidation.TenderWinnerValidationId,
                  //  OrgBranchName = foreignPurchaseValidation.OrgBranchName,
                    DateCreated = winnerValidation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = winnerValidation.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.TenderWinnerValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { winnerValidation }.ToDataSourceResult(request, ModelState));
        }

        // [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseValidations_Destroy([DataSourceRequest]DataSourceRequest request, TenderWinnerValidation winnerValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderWinnerValidation
                {
                    SalesLotId = winnerValidation.SalesLotId,
                    Status = winnerValidation.Status,
                    Remark = winnerValidation.Remark,
                  //  OrgBranchName = winnerValidation.OrgBranchName,
                    DateCreated = winnerValidation.DateCreated,
                    LastModified = winnerValidation.LastModified,
                    CreatedBy = winnerValidation.CreatedBy,
                    ModifiedBy = winnerValidation.ModifiedBy
                };

                _db.TenderWinnerValidations.Attach(entity);
                _db.TenderWinnerValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { winnerValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}