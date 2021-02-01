using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderPartialPrintValidationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.ArSalesOrderView, FinanceConstants.ArSalesOrderApprover, FinanceConstants.ArSalesOrder)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderView, FinanceConstants.ArSalesOrderApprover, FinanceConstants.ArSalesOrder)]
        public ActionResult ArSalesOrderPartialPrintValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderPartialPrintValidation> arSalesOrderPartialPrintValidations = _db.ArSalesOrderPartialPrintValidations.Where(x=>x.ArSalesOrderPartialPrintId==id);
            DataSourceResult result = arSalesOrderPartialPrintValidations.ToDataSourceResult(request, arSalesOrderPartialPrintValidation => new {
                ArSalesOrderPartialPrintValidationId = arSalesOrderPartialPrintValidation.ArSalesOrderPartialPrintValidationId,
                Status = arSalesOrderPartialPrintValidation.Status,
                Remark = arSalesOrderPartialPrintValidation.Remark,
                OrgBranchName = arSalesOrderPartialPrintValidation.OrgBranchName,
                DateCreated = arSalesOrderPartialPrintValidation.DateCreated,
                LastModified = arSalesOrderPartialPrintValidation.LastModified,
                CreatedBy = arSalesOrderPartialPrintValidation.CreatedBy,
                ModifiedBy = arSalesOrderPartialPrintValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPartialPrintValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ArSalesOrderPartialPrintValidation arSalesOrderPartialPrintValidation)
        {
            var record = _db.ArSalesOrderPartialPrints.Find(id);
          
            #region Checking the record

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (record.ArSalesOrderPartials.Count == 0 && arSalesOrderPartialPrintValidation.Status == ApprovalStatuses.Approve)
                {
                    string message = "The Record Does not Contain Any Item";
                    ModelState.AddModelError(String.Empty, message);
                }
               
                
            }
            else
            {
                ModelState.AddModelError("No Found", "Record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (arSalesOrderPartialPrintValidation.Status == ApprovalStatuses.Approve)
                    {
                        //PostToSecondaryLedger(id);
                        record.IsPostedToSecondLedger = true;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                       

                    }
                    else if (arSalesOrderPartialPrintValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new ArSalesOrderPartialPrintValidation
                    {
                        ArSalesOrderPartialPrintId = id,
                        Status = arSalesOrderPartialPrintValidation.Status,
                        Remark = arSalesOrderPartialPrintValidation.Remark,
                        OrgBranchName = arSalesOrderPartialPrintValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = arSalesOrderPartialPrintValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = arSalesOrderPartialPrintValidation.ModifiedBy
                    };

                    _db.ArSalesOrderPartialPrintValidations.Add(entity);
                    _db.SaveChanges();
                    arSalesOrderPartialPrintValidation.ArSalesOrderPartialPrintValidationId = entity.ArSalesOrderPartialPrintValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AR sales order partial print approval", "Success", User.Identity.Name, $"approved sales order partial {entity.ArSalesOrderPartialPrintId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { arSalesOrderPartialPrintValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPartialPrintValidations_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderPartialPrintValidation arSalesOrderPartialPrintValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPartialPrintValidation
                {
                    ArSalesOrderPartialPrintId = arSalesOrderPartialPrintValidation.ArSalesOrderPartialPrintId,
                    ArSalesOrderPartialPrintValidationId = arSalesOrderPartialPrintValidation.ArSalesOrderPartialPrintValidationId,
                    Status = arSalesOrderPartialPrintValidation.Status,
                    Remark = arSalesOrderPartialPrintValidation.Remark,
                    OrgBranchName = arSalesOrderPartialPrintValidation.OrgBranchName,
                    DateCreated = arSalesOrderPartialPrintValidation.DateCreated,
                    LastModified = arSalesOrderPartialPrintValidation.LastModified,
                    CreatedBy = arSalesOrderPartialPrintValidation.CreatedBy,
                    ModifiedBy = arSalesOrderPartialPrintValidation.ModifiedBy
                };

                _db.ArSalesOrderPartialPrintValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderPartialPrintValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPartialPrintValidations_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderPartialPrintValidation arSalesOrderPartialPrintValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPartialPrintValidation
                {
                    ArSalesOrderPartialPrintId = arSalesOrderPartialPrintValidation.ArSalesOrderPartialPrintId,
                    ArSalesOrderPartialPrintValidationId = arSalesOrderPartialPrintValidation.ArSalesOrderPartialPrintValidationId,
                    Status = arSalesOrderPartialPrintValidation.Status,
                    Remark = arSalesOrderPartialPrintValidation.Remark,
                    OrgBranchName = arSalesOrderPartialPrintValidation.OrgBranchName,
                    DateCreated = arSalesOrderPartialPrintValidation.DateCreated,
                    LastModified = arSalesOrderPartialPrintValidation.LastModified,
                    CreatedBy = arSalesOrderPartialPrintValidation.CreatedBy,
                    ModifiedBy = arSalesOrderPartialPrintValidation.ModifiedBy
                };

                _db.ArSalesOrderPartialPrintValidations.Attach(entity);
                _db.ArSalesOrderPartialPrintValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderPartialPrintValidation }.ToDataSourceResult(request, ModelState));
        }

        private void PostToSecondaryLedger(int id)
        {
            SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
            string user = User.Identity.Name;
            var record = _db.ArSalesOrders.Find(id);
            if (record != null)
            {
                #region header

                var ledger = new PostJournalVm
                {
                    PostJournalVmId = id,
                    JournalName = "",
                    JournalNumber = "",
                    Description = "",
                    CategoryName = "",
                    SourcesName = "AR Sales Order",
                    EffectiveDate = record.OrderDate,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.OrderDate),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.OrderDate),
                    PostingSource = JournalSources.Sales,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.ArSalesOrderPartialPrintDistributions.Where(x => x.ArSalesOrderPartialPrintId == id).ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.ArSalesOrderPartialPrintDistributionId,
                        AccountCode = distribution.AccountCode,
                        Debit = distribution.Debit,
                        Credit = distribution.Credit
                    };
                    ledger.PostJournalLineVms.Add(ledgerline);
                }

                #endregion

                secondLedgerHelper.PostToSecondLedger(ledger, user);
            }
        }
        void TransferToStandardCollection(ArSalesOrder arSalesOrder)
        {
            throw new NotImplementedException();
        }
        void TransferToInvoice(ArSalesOrder arSalesOrder)
        {
            throw new NotImplementedException();
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
