using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{


    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpDeliveryUser)]
    public class ProductionIssueDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //private AccountHelper accountHelper = new AccountHelper();

       // [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult Index()
        {
            return View();
        }

        //[AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult IssueDistributions_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<IssueDistribution> issuedistributions = db.IssueDistributions.Where(ii => ii.IssueID == id);
            DataSourceResult result = issuedistributions.ToDataSourceResult(request, issueDistribution => new {
                IssueID = issueDistribution.IssueID,
                IssueDistributionID = issueDistribution.IssueDistributionID,
                IssueItemIDTrackBack = issueDistribution.IssueItemIDTrackBack,
                AccountCode = issueDistribution.AccountCode,
                AccountDesc = issueDistribution.AccountDesc,
                AdditionalDescription = issueDistribution.AdditionalDescription,
                Debit = issueDistribution.Debit,
                Credit = issueDistribution.Credit
            });

            return Json(result);
        }

       // [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueDistributions_Create([DataSourceRequest]DataSourceRequest request, IssueDistribution issueDistribution , int id )
        {
            #region validation

            var record = db.Issues.Find(id);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            var exists = db.IssueDistributions.Where(x => x.AccountCode == issueDistribution.AccountCode);
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The account code selected already exists.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IssueDistribution
                {
                    IssueID = id,
                    IssueItemIDTrackBack = issueDistribution.IssueItemIDTrackBack,
                    AccountCode = issueDistribution.AccountCode,
                //    AccountDesc = accountHelper.GetAccountDesc(issueDistribution.AccountCode),
                    AdditionalDescription = issueDistribution.AdditionalDescription,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                db.IssueDistributions.Add(entity);
                db.SaveChanges();
                issueDistribution.IssueDistributionID = entity.IssueDistributionID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue distribution", "Success", User.Identity.Name, $"created issue distribution {entity.AccountCode} under record {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

        // [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueDistributions_Update([DataSourceRequest]DataSourceRequest request, IssueDistribution issueDistribution)
        {
            #region validation

            var record = db.Issues.Find(issueDistribution.IssueID);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            //var exists = db.IssueDistributions.Where(x => x.IssueDistributionID != issueDistribution.IssueDistributionID).Where(x => x.Description == issueDistribution.Description);
            //if (exists.Any())
            //{
            //    ModelState.AddModelError("exist", "The account code selected already exists.");
            //}
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IssueDistribution
                {
                    IssueID = issueDistribution.IssueID,
                    IssueDistributionID = issueDistribution.IssueDistributionID,
                    IssueItemIDTrackBack = issueDistribution.IssueItemIDTrackBack,
                    AccountCode = issueDistribution.AccountCode,
                    //AccountDesc = accountHelper.GetAccountDesc(issueDistribution.AccountCode),
                    AdditionalDescription = issueDistribution.AdditionalDescription,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                db.IssueDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue distribution", "Success", User.Identity.Name, $"updated issue distribution {entity.AccountCode} under record {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

         //[AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueDistributions_Destroy([DataSourceRequest]DataSourceRequest request, IssueDistribution issueDistribution)
        {
            #region validation

            var record = db.Issues.Find(issueDistribution.Issue);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
           
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IssueDistribution
                {
                    IssueID = issueDistribution.IssueID,
                    IssueDistributionID = issueDistribution.IssueDistributionID,
                    IssueItemIDTrackBack = issueDistribution.IssueItemIDTrackBack,
                    AccountCode = issueDistribution.AccountCode,
                    AccountDesc = issueDistribution.AccountDesc,
                    AdditionalDescription = issueDistribution.AdditionalDescription,
                    Debit = issueDistribution.Debit,
                    Credit = issueDistribution.Credit
                };

                db.IssueDistributions.Attach(entity);
                db.IssueDistributions.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Issue distribution " + issueDistribution.IssueID + " " + issueDistribution.AccountCode);
                #endregion
            }

            return Json(new[] { issueDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
