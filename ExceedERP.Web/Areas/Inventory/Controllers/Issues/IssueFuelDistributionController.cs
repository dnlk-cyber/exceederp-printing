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

namespace ExceedERP.Web.Areas.Inventory.Controllers.Issues
{
    [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]

    public class IssueFuelDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult IssueFuelDistributions_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<IssueDistribution> issuefueldistributions = db.IssueDistributions.Where(isf => isf.IssueID == id);
            DataSourceResult result = issuefueldistributions.ToDataSourceResult(request, issueFuelDistribution => new
            {
                IssueDistributionID = issueFuelDistribution.IssueDistributionID,
                IssueID = issueFuelDistribution.IssueID,
                IssueItemIDTrackBack = issueFuelDistribution.IssueItemIDTrackBack,
                AccountCode = issueFuelDistribution.AccountCode,
                AccountDesc = issueFuelDistribution.AccountDesc,
                Debit = issueFuelDistribution.Debit,
                Credit = issueFuelDistribution.Credit
            });

            return Json(result);
        }

         [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuelDistributions_Create([DataSourceRequest]DataSourceRequest request, IssueDistribution issueFuelDistribution , int id )
        {
            if (ModelState.IsValid)
            {
                var entity = new IssueDistribution
                {
                    IssueID = id,
                    IssueItemIDTrackBack = issueFuelDistribution.IssueItemIDTrackBack,
                    AccountCode = issueFuelDistribution.AccountCode,
                    AccountDesc = issueFuelDistribution.AccountDesc,
                    Debit = issueFuelDistribution.Debit,
                    Credit = issueFuelDistribution.Credit
                };

                db.IssueDistributions.Add(entity);
                db.SaveChanges();
                issueFuelDistribution.IssueDistributionID = entity.IssueDistributionID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue fuel distribution", "Success", User.Identity.Name, $"created issue distribution {entity.AccountCode} under record {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issueFuelDistribution }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuelDistributions_Update([DataSourceRequest]DataSourceRequest request, IssueDistribution issueFuelDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new IssueDistribution
                {
                    IssueDistributionID = issueFuelDistribution.IssueDistributionID,
                    IssueID = issueFuelDistribution.IssueID,
                    IssueItemIDTrackBack = issueFuelDistribution.IssueItemIDTrackBack,
                    AccountCode = issueFuelDistribution.AccountCode,
                    AccountDesc = issueFuelDistribution.AccountDesc,
                    Debit = issueFuelDistribution.Debit,
                    Credit = issueFuelDistribution.Credit
                };

                db.IssueDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue fuel distribution", "Success", User.Identity.Name, $"updated issue distribution {entity.AccountCode} under record {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issueFuelDistribution }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuelDistributions_Destroy([DataSourceRequest]DataSourceRequest request, IssueDistribution issueFuelDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new IssueDistribution
                {
                    IssueDistributionID = issueFuelDistribution.IssueDistributionID,
                    IssueID = issueFuelDistribution.IssueID,
                    IssueItemIDTrackBack = issueFuelDistribution.IssueItemIDTrackBack,
                    AccountCode = issueFuelDistribution.AccountCode,
                    AccountDesc = issueFuelDistribution.AccountDesc,
                    Debit = issueFuelDistribution.Debit,
                    Credit = issueFuelDistribution.Credit
                };

                db.IssueDistributions.Attach(entity);
                db.IssueDistributions.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Issue fuel distribution " + issueFuelDistribution.IssueID + " " + issueFuelDistribution.AccountCode);
                #endregion
            }

            return Json(new[] { issueFuelDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
