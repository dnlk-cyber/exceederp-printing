using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.PaymentRequests
{
    public class FundTransferBudgetValidationBudgetController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult FundTransferBudgetValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FundTransferBudgetValidation> FundTransferBudgetValidations = _db.FundTransferBudgetValidations.Where(x=>x.FundTransferId==id);
            DataSourceResult result = FundTransferBudgetValidations.ToDataSourceResult(request, FundTransferBudgetValidation => new {
                FundTransferBudgetValidationId = FundTransferBudgetValidation.FundTransferBudgetValidationId,
                FundTransferId=id,
                Status = FundTransferBudgetValidation.Status,
                Remark = FundTransferBudgetValidation.Remark,
                OrgBranchName = FundTransferBudgetValidation.OrgBranchName,
                DateCreated = FundTransferBudgetValidation.DateCreated,
                LastModified = FundTransferBudgetValidation.LastModified,
                CreatedBy = FundTransferBudgetValidation.CreatedBy,
                ModifiedBy = FundTransferBudgetValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransferBudgetValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, FundTransferBudgetValidation FundTransferBudgetValidation)
        {
            var record = _db.FundTransfers.Find(id);
            if (record != null && record.IsBudgetApproved)
                ModelState.AddModelError("approved","You cannot manipulate this record,it is already approved");

            if (record != null && !record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("notdepapproved", "The record should be approved by requestor department first.");

            var lines = _db.FundTransferDetails.Where(x => x.FundTransferId == id);
            if (!lines.Any())
                ModelState.AddModelError("lines", "Please add line items");
          
          

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (FundTransferBudgetValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Budget Approved";
                        record.IsBudgetApproved = true;
                        record.BudgetApprovedBy = User.Identity.Name;
                        record.BudgetApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                      
                    }

                    if (FundTransferBudgetValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsBudgetApproved = false;
                        record.Status = "Budget Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new FundTransferBudgetValidation
                    {
                        FundTransferId = id,
                        Status = FundTransferBudgetValidation.Status,
                        Remark = FundTransferBudgetValidation.Remark,
                        OrgBranchName = FundTransferBudgetValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = FundTransferBudgetValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = FundTransferBudgetValidation.ModifiedBy
                    };

                    _db.FundTransferBudgetValidations.Add(entity);
                    _db.SaveChanges();
                    FundTransferBudgetValidation.FundTransferBudgetValidationId = entity.FundTransferBudgetValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request budget approval", "Success", User.Identity.Name, $"approved request {entity.FundTransferId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { FundTransferBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPaymentApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransferBudgetValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, FundTransferBudgetValidation FundTransferBudgetValidation)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region ger parent record status
                var record = _db.PaymentRequests.Find(id);

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (FundTransferBudgetValidation.Status ==ApprovalStatuses.Approve)
                    {
                       
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (FundTransferBudgetValidation.Status ==ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new FundTransferBudgetValidation
                    {
                        FundTransferId = id,
                        FundTransferBudgetValidationId = FundTransferBudgetValidation.FundTransferBudgetValidationId,
                        Status = FundTransferBudgetValidation.Status,
                        Remark = FundTransferBudgetValidation.Remark,
                        OrgBranchName = FundTransferBudgetValidation.OrgBranchName,
                        DateCreated = FundTransferBudgetValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = FundTransferBudgetValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.FundTransferBudgetValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { FundTransferBudgetValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
