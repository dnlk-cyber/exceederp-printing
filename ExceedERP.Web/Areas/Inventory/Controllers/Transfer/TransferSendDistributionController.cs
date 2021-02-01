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

namespace ExceedERP.Web.Areas.Inventory.Controllers.Transfer
{
   
    public class TransferSendDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        AccountHelper accountHelper = new AccountHelper();

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult ItemTransferDistributions_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<ItemTransferDistribution> itemtransferdistributions = _db.ItemTransferDistributions.Where(x => x.ItemTransferID == id);
            DataSourceResult result = itemtransferdistributions.ToDataSourceResult(request, itemTransferDistribution => new {
                ItemTransferDistributionID = itemTransferDistribution.ItemTransferDistributionID,
                ItemTransferID = itemTransferDistribution.ItemTransferID,
                AccountCode = itemTransferDistribution.AccountCode,
                AccountDesc = itemTransferDistribution.AccountDesc,
                AdditionalDescription = itemTransferDistribution.AdditionalDescription,
                Debit = itemTransferDistribution.Debit,
                Credit = itemTransferDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ItemTransferDistribution itemTransferDistribution)
        {
            #region validation

            var record = _db.ItemTransfers.Find(id);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            //var exists = _db.ItemTransferDistributions.Where(x => x.Description == itemTransferDistribution.Description);
            //if (exists.Any())
            //{
            //    ModelState.AddModelError("exist", "The account code selected already exists.");
            //}
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferDistribution
                {
                    ItemTransferID=id,
                    AccountCode = itemTransferDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(itemTransferDistribution.AccountCode),
                    AdditionalDescription = itemTransferDistribution.AdditionalDescription,
                    Debit = itemTransferDistribution.Debit,
                    Credit = itemTransferDistribution.Credit
                };

                _db.ItemTransferDistributions.Add(entity);
                _db.SaveChanges();
                itemTransferDistribution.ItemTransferDistributionID = entity.ItemTransferDistributionID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer send distribution", "Success", User.Identity.Name, $"created transfer send distribution {entity.AccountCode} under record {entity.ItemTransferID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { itemTransferDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferDistributions_Update([DataSourceRequest]DataSourceRequest request, ItemTransferDistribution itemTransferDistribution)
        {
            #region validation

            var record = _db.ItemTransfers.Find(itemTransferDistribution.ItemTransferID);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            //var exists = _db.ItemTransferDistributions.Where(x => x.ItemTransferDistributionID != itemTransferDistribution.ItemTransferDistributionID).Where(x => x.Description == itemTransferDistribution.Description);
            //if (exists.Any())
            //{
            //    ModelState.AddModelError("exist", "The account code selected already exists.");
            //}
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferDistribution
                {
                    ItemTransferID = itemTransferDistribution.ItemTransferID,
                    ItemTransferDistributionID = itemTransferDistribution.ItemTransferDistributionID,
                    AccountCode = itemTransferDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(itemTransferDistribution.AccountCode),
                    AdditionalDescription = itemTransferDistribution.AdditionalDescription,
                    Debit = itemTransferDistribution.Debit,
                    Credit = itemTransferDistribution.Credit
                };

                _db.ItemTransferDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer send distribution", "Success", User.Identity.Name, $"updated transfer send distribution {entity.AccountCode} under record {entity.ItemTransferID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { itemTransferDistribution }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ItemTransferDistribution itemTransferDistribution)
        {
            var record = _db.ItemTransfers.Find(itemTransferDistribution.ItemTransferID);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferDistribution
                {
                    ItemTransferID = itemTransferDistribution.ItemTransferID,
                    ItemTransferDistributionID = itemTransferDistribution.ItemTransferDistributionID,
                    AccountCode = itemTransferDistribution.AccountCode,
                    AccountDesc = itemTransferDistribution.AccountDesc,
                    AdditionalDescription = itemTransferDistribution.AdditionalDescription,
                    Debit = itemTransferDistribution.Debit,
                    Credit = itemTransferDistribution.Credit
                };

                _db.ItemTransferDistributions.Attach(entity);
                _db.ItemTransferDistributions.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Item transfer distribution " + itemTransferDistribution.ItemTransferID + " " + itemTransferDistribution.AccountCode);
                #endregion
            }

            return Json(new[] { itemTransferDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
