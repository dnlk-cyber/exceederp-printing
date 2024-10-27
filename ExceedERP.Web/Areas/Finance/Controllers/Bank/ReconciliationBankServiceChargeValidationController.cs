using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Finance.Controllers.Bank
{
    [AuthorizeRoles(FinanceConstants.BankReconciliationUser, FinanceConstants.BankReconciliationApprover)]
    public class ReconciliationBankServiceChargeValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReconciliationBankServiceChargeValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ReconciliationBankServiceChargeValidation> reconciliationbankservicechargevalidations = db.ReconciliationBankServiceChargeValidations.Where(x=>x.ReconciliationScheduleId==id);
            DataSourceResult result = reconciliationbankservicechargevalidations.ToDataSourceResult(request, reconciliationBankServiceChargeValidation => new {
                ReconciliationBankServiceChargeValidationId = reconciliationBankServiceChargeValidation.ReconciliationBankServiceChargeValidationId,
                Status = reconciliationBankServiceChargeValidation.Status,
                Remark = reconciliationBankServiceChargeValidation.Remark,
                OrgBranchName = reconciliationBankServiceChargeValidation.OrgBranchName,
                DateCreated = reconciliationBankServiceChargeValidation.DateCreated,
                LastModified = reconciliationBankServiceChargeValidation.LastModified,
                CreatedBy = reconciliationBankServiceChargeValidation.CreatedBy,
                ModifiedBy = reconciliationBankServiceChargeValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.BankReconciliationApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankServiceChargeValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ReconciliationBankServiceChargeValidation reconciliationBankServiceChargeValidation)
        {
            #region validated record
            var record = db.ReconciliationSchedules.Find(id);

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            #endregion
            if (ModelState.IsValid)
            {
                if (record != null && !record.IsOnlineApproved)
                {

                    if (reconciliationBankServiceChargeValidation.Status == ApprovalStatuses.Approve)
                    {
                        //#region pass data for reconciliation

                        //foreach (var distribution in record.ReconciliationBankServiceChargeDistributions)
                        //{
                        //    //check if this is bank account
                        //    if (AccountHelper.IsBankAccount(distribution.Description))
                        //    {
                        //        var book = new ReconciliationBookAccount
                        //        {
                        //            CreatedBy = User.Identity.Name,
                        //            DateCreated = DateTime.Now,
                        //            Description = distribution.Description,
                        //            Debit = distribution.Debit,
                        //            Credit = distribution.Credit,
                        //            Date = DateTime.Now,
                        //            Type = "Bank Service Charge",
                        //            ReferenceDoc = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                        //            Reference = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                        //            Source = JournalSources.BankServiceCharge.ToString(),
                        //            SourceID = record.ReconciliationScheduleId
                        //        };
                        //        db.ReconcilationBookAccounts.Add(book);
                        //        db.SaveChanges();
                        //    }
                        //}

                        //#endregion

                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (reconciliationBankServiceChargeValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var entity = new ReconciliationBankServiceChargeValidation
                    {
                        ReconciliationScheduleId = id,
                        Status = reconciliationBankServiceChargeValidation.Status,
                        Remark = reconciliationBankServiceChargeValidation.Remark,
                        OrgBranchName = reconciliationBankServiceChargeValidation.OrgBranchName,
                        DateCreated = reconciliationBankServiceChargeValidation.DateCreated,
                        LastModified = reconciliationBankServiceChargeValidation.LastModified,
                        CreatedBy = reconciliationBankServiceChargeValidation.CreatedBy,
                        ModifiedBy = reconciliationBankServiceChargeValidation.ModifiedBy
                    };

                    db.ReconciliationBankServiceChargeValidations.Add(entity);
                    db.SaveChanges();
                    reconciliationBankServiceChargeValidation.ReconciliationBankServiceChargeValidationId =
                        entity.ReconciliationBankServiceChargeValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation service charge", "Success", User.Identity.Name, $"approved service charge of schedule {entity.ReconciliationScheduleId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { reconciliationBankServiceChargeValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankServiceChargeValidations_Update([DataSourceRequest]DataSourceRequest request, ReconciliationBankServiceChargeValidation reconciliationBankServiceChargeValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankServiceChargeValidation
                {
                    ReconciliationScheduleId = reconciliationBankServiceChargeValidation.ReconciliationScheduleId,
                    ReconciliationBankServiceChargeValidationId = reconciliationBankServiceChargeValidation.ReconciliationBankServiceChargeValidationId,
                    Status = reconciliationBankServiceChargeValidation.Status,
                    Remark = reconciliationBankServiceChargeValidation.Remark,
                    OrgBranchName = reconciliationBankServiceChargeValidation.OrgBranchName,
                    DateCreated = reconciliationBankServiceChargeValidation.DateCreated,
                    LastModified = reconciliationBankServiceChargeValidation.LastModified,
                    CreatedBy = reconciliationBankServiceChargeValidation.CreatedBy,
                    ModifiedBy = reconciliationBankServiceChargeValidation.ModifiedBy
                };

                db.ReconciliationBankServiceChargeValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { reconciliationBankServiceChargeValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankServiceChargeValidations_Destroy([DataSourceRequest]DataSourceRequest request, ReconciliationBankServiceChargeValidation reconciliationBankServiceChargeValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankServiceChargeValidation
                {
                    ReconciliationBankServiceChargeValidationId = reconciliationBankServiceChargeValidation.ReconciliationBankServiceChargeValidationId,
                    Status = reconciliationBankServiceChargeValidation.Status,
                    Remark = reconciliationBankServiceChargeValidation.Remark,
                    OrgBranchName = reconciliationBankServiceChargeValidation.OrgBranchName,
                    DateCreated = reconciliationBankServiceChargeValidation.DateCreated,
                    LastModified = reconciliationBankServiceChargeValidation.LastModified,
                    CreatedBy = reconciliationBankServiceChargeValidation.CreatedBy,
                    ModifiedBy = reconciliationBankServiceChargeValidation.ModifiedBy
                };

                db.ReconciliationBankServiceChargeValidations.Attach(entity);
                db.ReconciliationBankServiceChargeValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { reconciliationBankServiceChargeValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
