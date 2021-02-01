using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Returns
{
    
    public class StoreReturnDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult Index()
        {
            return View();
        }

          [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult StoreReturnDistributions_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<StoreReturnDistribution> storereturndistributions = _db.StoreReturnDistributions.Where(i => i.StoreReturnID == id);
            DataSourceResult result = storereturndistributions.ToDataSourceResult(request, storeReturnDistribution => new {
                StoreReturnID = storeReturnDistribution.StoreReturnID,
                StoreReturnDistributionID = storeReturnDistribution.StoreReturnDistributionID,
                AccountCode = storeReturnDistribution.AccountCode,
                AccountDesc = storeReturnDistribution.AccountDesc,
                AdditionalDescription = storeReturnDistribution.AdditionalDescription,
                Debit = storeReturnDistribution.Debit,
                Credit = storeReturnDistribution.Credit
            });

            return Json(result);
        }

          [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
         [AcceptVerbs(HttpVerbs.Post)]
         public ActionResult storeReturnDistributions_Create([DataSourceRequest]DataSourceRequest request, StoreReturnDistribution storeReturnDistribution, int id)
         {
             #region validation

             var record = _db.StoreReturns.Find(id);
             if (record != null && record.IsOnlineTransferred)
             {
                 ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
             }
             var exists = _db.StoreReturnDistributions
                 .Where(x => x.StoreReturnID ==id)
                 .Where(x => x.AccountCode == storeReturnDistribution.AccountCode);
             if (exists.Any())
             {
                 ModelState.AddModelError("exist", "The account code selected already exists.");
             }
             #endregion
             if (ModelState.IsValid)
             {
                 var entity = new StoreReturnDistribution
                 {
                     StoreReturnID = id,
                     AccountCode = storeReturnDistribution.AccountCode,
                     AccountDesc = _accountHelper.GetAccountDesc(storeReturnDistribution.AccountCode),
                     AdditionalDescription = storeReturnDistribution.AdditionalDescription,
                     Debit = storeReturnDistribution.Debit,
                     Credit = storeReturnDistribution.Credit
                 };

                 _db.StoreReturnDistributions.Add(entity);
                 _db.SaveChanges();
                 storeReturnDistribution.StoreReturnDistributionID = entity.StoreReturnDistributionID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store return distribution", "Success", User.Identity.Name, $"created store return distribution {entity.AccountCode} under record {entity.StoreReturnDistributionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeReturnDistribution }.ToDataSourceResult(request, ModelState));
         }

         [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnDistributions_Update([DataSourceRequest]DataSourceRequest request, StoreReturnDistribution storeReturnDistribution)
        {
            #region validation

            var record = _db.StoreReturns.Find(storeReturnDistribution.StoreReturnID);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            var exists = _db.StoreReturnDistributions
                .Where(x=>x.StoreReturnID==storeReturnDistribution.StoreReturnID)
                .Where(x=>x.StoreReturnDistributionID!= storeReturnDistribution.StoreReturnDistributionID)
                .Where(x => x.AccountCode == storeReturnDistribution.AccountCode);
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The account code selected already exists.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new StoreReturnDistribution
                {
                    StoreReturnID = storeReturnDistribution.StoreReturnID,
                    Debit = storeReturnDistribution.Debit,
                    Credit = storeReturnDistribution.Credit,
                    AccountCode = storeReturnDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(storeReturnDistribution.AccountCode),
                    AdditionalDescription = storeReturnDistribution.AdditionalDescription,
                    StoreReturnDistributionID = storeReturnDistribution.StoreReturnDistributionID
                     
                };

                _db.StoreReturnDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store return distribution", "Success", User.Identity.Name, $"updated store return distribution {entity.AccountCode} under record {entity.StoreReturnDistributionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeReturnDistribution }.ToDataSourceResult(request, ModelState));
        }


          [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
         public ActionResult storeReturnDistributions_Destroy([DataSourceRequest]DataSourceRequest request, StoreReturnDistribution storeReturnDistribution)
         {
             var record = _db.StoreReturns.Find(storeReturnDistribution.StoreReturnID);
             if (record != null && record.IsOnlineTransferred)
             {
                 ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
             }
             if (ModelState.IsValid)
             {
                 var entity = new StoreReturnDistribution
                 {
                     StoreReturnID = storeReturnDistribution.StoreReturnID,
                     StoreReturnDistributionID = storeReturnDistribution.StoreReturnDistributionID,
                     AccountCode = storeReturnDistribution.AccountCode,
                     AccountDesc = storeReturnDistribution.AccountDesc,
                     AdditionalDescription = storeReturnDistribution.AdditionalDescription,
                     Debit = storeReturnDistribution.Debit,
                     Credit = storeReturnDistribution.Credit
                 };

                 _db.StoreReturnDistributions.Attach(entity);
                 _db.StoreReturnDistributions.Remove(entity);
                 _db.SaveChanges();
                 #region delete log
                 UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Store return distribution " + storeReturnDistribution.StoreReturnID + " " + storeReturnDistribution.AccountCode);
                 #endregion
             }

             return Json(new[] { storeReturnDistribution }.ToDataSourceResult(request, ModelState));
         }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
