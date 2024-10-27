using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApInvoiceValidationController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
        public ActionResult APPaybleInvoiceValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPaybleInvoiceValidation> appaybleinvoicevalidations = _db.APPaybleInvoiceValidations.Where(x=>x.ApPaybleInvoicesId==id);
            DataSourceResult result = appaybleinvoicevalidations.ToDataSourceResult(request, aPPaybleInvoiceValidation => new {
                ApPaybleInvoiceValidationId = aPPaybleInvoiceValidation.ApPaybleInvoiceValidationId,
                ApPaybleInvoicesId=id,
                Status = aPPaybleInvoiceValidation.Status,
                Remark = aPPaybleInvoiceValidation.Remark,
                OrgBranchName = aPPaybleInvoiceValidation.OrgBranchName,
                DateCreated = aPPaybleInvoiceValidation.DateCreated,
                LastModified = aPPaybleInvoiceValidation.LastModified,
                CreatedBy = aPPaybleInvoiceValidation.CreatedBy,
                ModifiedBy = aPPaybleInvoiceValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoiceValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPaybleInvoiceValidation aPPaybleInvoiceValidation)
        {
            var record = _db.APPaybleInvoices.Find(id);
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved","cannot manipulate this record,it is already approved");

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (aPPaybleInvoiceValidation.Status == ApprovalStatuses.Approve)
                    {
                        //PostToSecondaryLedger(id);
                        record.InvoiceStatus = InvoiceStatus.Open;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (aPPaybleInvoiceValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new ApPaybleInvoiceValidation
                    {
                        ApPaybleInvoicesId = id,
                        Status = aPPaybleInvoiceValidation.Status,
                        Remark = aPPaybleInvoiceValidation.Remark,
                        OrgBranchName = aPPaybleInvoiceValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPPaybleInvoiceValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPPaybleInvoiceValidation.ModifiedBy
                    };

                    _db.APPaybleInvoiceValidations.Add(entity);
                    _db.SaveChanges();
                    aPPaybleInvoiceValidation.ApPaybleInvoiceValidationId = entity.ApPaybleInvoiceValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP invoice approval", "Success", User.Identity.Name, $"approved invoice {entity.ApPaybleInvoicesId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aPPaybleInvoiceValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoiceValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, ApPaybleInvoiceValidation aPPaybleInvoiceValidation)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region ger parent record status
                ApPaybleInvoices record = _db.APPaybleInvoices.Find(id);

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (aPPaybleInvoiceValidation.Status ==ApprovalStatuses.Approve)
                    {
                       
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (aPPaybleInvoiceValidation.Status ==ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new ApPaybleInvoiceValidation
                    {
                        ApPaybleInvoicesId = id,
                        ApPaybleInvoiceValidationId = aPPaybleInvoiceValidation.ApPaybleInvoiceValidationId,
                        Status = aPPaybleInvoiceValidation.Status,
                        Remark = aPPaybleInvoiceValidation.Remark,
                        OrgBranchName = aPPaybleInvoiceValidation.OrgBranchName,
                        DateCreated = aPPaybleInvoiceValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aPPaybleInvoiceValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.APPaybleInvoiceValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { aPPaybleInvoiceValidation }.ToDataSourceResult(request, ModelState));
        }


        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
