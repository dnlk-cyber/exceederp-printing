using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.MiscCollection
{
    public class ArMiscelaneousCollectionValidationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        public ActionResult Index()
        {
            return View();
        }

      [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        public ActionResult ARMiscelaneousCollectionValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ARMiscelaneousCollectionValidation> armiscelaneouscollectionvalidations = _db.ARMiscelaneousCollectionValidations.Where(x=>x.ARMiscelaneousCollectionID==id);
            DataSourceResult result = armiscelaneouscollectionvalidations.ToDataSourceResult(request, aRMiscelaneousCollectionValidation => new {
                ARMiscelaneousCollectionValidationID = aRMiscelaneousCollectionValidation.ARMiscelaneousCollectionValidationID,
                ARMiscelaneousCollectionID = id,
                Status = aRMiscelaneousCollectionValidation.Status,
                Remark = aRMiscelaneousCollectionValidation.Remark,
                OrgBranchName = aRMiscelaneousCollectionValidation.OrgBranchName,
                DateCreated = aRMiscelaneousCollectionValidation.DateCreated,
                LastModified = aRMiscelaneousCollectionValidation.LastModified,
                CreatedBy = aRMiscelaneousCollectionValidation.CreatedBy,
                ModifiedBy = aRMiscelaneousCollectionValidation.ModifiedBy
            });

            return Json(result);
        }

      [AuthorizeRoles( FinanceConstants.AccountReceivableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARMiscelaneousCollectionValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ARMiscelaneousCollectionValidation aRMiscelaneousCollectionValidation)
        {
            #region validated record
            ARMiscelaneousCollection record = _db.ARMiscelaneousCollections.Find(id);

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("valid", "You cannot manipulate this record");
            #endregion
            if (ModelState.IsValid)
            {
               
                #region validate 

               
                if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (aRMiscelaneousCollectionValidation.Status == ApprovalStatuses.Approve)
                    {
                        #region pass data for reconciliation

                        //foreach (ARMiscelaneousCollectionDistribution distribution in record.ARMiscelaneousCollectionDistribution)
                        //{
                        //    string sourceid = record.ARMiscelaneousCollectionID + "-" + distribution.ARMiscelaneousCollectionDistributionID;

                        //    if (_accountHelper.IsBankAccount(distribution.AccountCode))
                        //    {
                        //        var book = new ReconciliationBookAccount
                        //        {
                        //            CreatedBy = User.Identity.Name,
                        //            DateCreated = DateTime.Now,
                        //            AccountCode = distribution.AccountCode,
                        //            Debit = distribution.Debit,
                        //            Credit = distribution.Credit,
                        //            Date = record.ReceiptDate,
                        //            Type = "Misc Collection",
                        //            ReferenceDoc = !String.IsNullOrWhiteSpace(record.ReceiptNumber) ? record.ReceiptNumber : "NA",
                        //            Reference = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                        //            Source = JournalSources.AR_MiscCollection.ToString(),
                        //            SourceID = sourceid,
                        //            AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                        //            SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                        //        };
                        //        _db.ReconcilationBookAccounts.Add(book);
                        //        _db.SaveChanges();
                        //    }
                        //}
                        #endregion
                        //PostToSecondaryLedger(id);
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (aRMiscelaneousCollectionValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

              
                    var entity = new ARMiscelaneousCollectionValidation
                    {
                        ARMiscelaneousCollectionID = id,
                        Status = aRMiscelaneousCollectionValidation.Status,
                        Remark = aRMiscelaneousCollectionValidation.Remark,
                        OrgBranchName = aRMiscelaneousCollectionValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aRMiscelaneousCollectionValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aRMiscelaneousCollectionValidation.ModifiedBy
                    };

                    _db.ARMiscelaneousCollectionValidations.Add(entity);
                    _db.SaveChanges();
                    aRMiscelaneousCollectionValidation.ARMiscelaneousCollectionValidationID =
                        entity.ARMiscelaneousCollectionValidationID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR misc collection approval", "Success", User.Identity.Name, $"approved misc collection {entity.ARMiscelaneousCollectionID} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRMiscelaneousCollectionValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
