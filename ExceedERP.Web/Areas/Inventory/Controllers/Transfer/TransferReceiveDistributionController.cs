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
   
    public class TransferReceiveDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private AccountHelper accountHelper = new AccountHelper();

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyStoreKeeper, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult ItemTransferDistributions_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<ItemTransferReceiveDistribution> itemtransferdistributions = _db.ItemTransferReceiveDistribution.Where(x => x.ItemTransferReceiveId == id);
            DataSourceResult result = itemtransferdistributions.ToDataSourceResult(request, itemTransferDistribution => new {
                ItemTransferReceiveDistributionId = itemTransferDistribution.ItemTransferReceiveDistributionId,
                ItemTransferReceiveId = itemTransferDistribution.ItemTransferReceiveId,
                AccountCode = itemTransferDistribution.AccountCode,
                AccountDesc = itemTransferDistribution.AccountDesc,
                Debit = itemTransferDistribution.Debit,
                Credit = itemTransferDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ItemTransferReceiveDistribution itemTransferDistribution)
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
                var entity = new ItemTransferReceiveDistribution
                {
                    AccountCode = itemTransferDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(itemTransferDistribution.AccountCode),
                    Debit = itemTransferDistribution.Debit,
                    Credit = itemTransferDistribution.Credit
                };

                _db.ItemTransferReceiveDistribution.Add(entity);
                _db.SaveChanges();
                itemTransferDistribution.ItemTransferReceiveDistributionId = entity.ItemTransferReceiveDistributionId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer receive distribution", "Success", User.Identity.Name, $"created transfer receive distribution {entity.AccountCode} under record {entity.ItemTransferReceiveId} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { itemTransferDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferDistributions_Update([DataSourceRequest]DataSourceRequest request, ItemTransferReceiveDistribution itemTransferDistribution)
        {
            #region validation

            var record = _db.ItemTransfers.Find(itemTransferDistribution.ItemTransferReceiveId);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            //var exists = _db.ItemTransferReceiveDistribution.Where(x => x.ItemTransferReceiveDistributionId != itemTransferDistribution.ItemTransferReceiveDistributionId).Where(x => x.Description == itemTransferDistribution.Description);
            //if (exists.Any())
            //{
            //    ModelState.AddModelError("exist", "The account code selected already exists.");
            //}
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferReceiveDistribution
                {
                    ItemTransferReceiveDistributionId = itemTransferDistribution.ItemTransferReceiveDistributionId,
                    AccountCode = itemTransferDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(itemTransferDistribution.AccountCode),
                    Debit = itemTransferDistribution.Debit,
                    Credit = itemTransferDistribution.Credit
                };

                _db.ItemTransferReceiveDistribution.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer receive distribution", "Success", User.Identity.Name, $"updated transfer receive distribution {entity.AccountCode} under record {entity.ItemTransferReceiveId} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { itemTransferDistribution }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ItemTransferReceiveDistribution itemTransferDistribution)
        {
            var record = _db.ItemTransferReceive.Find(itemTransferDistribution.ItemTransferReceiveId);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferReceiveDistribution
                {
                    ItemTransferReceiveDistributionId = itemTransferDistribution.ItemTransferReceiveDistributionId,
                    AccountCode = itemTransferDistribution.AccountCode,
                    AccountDesc = itemTransferDistribution.AccountDesc,
                    Debit = itemTransferDistribution.Debit,
                    Credit = itemTransferDistribution.Credit
                };

                _db.ItemTransferReceiveDistribution.Attach(entity);
                _db.ItemTransferReceiveDistribution.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Item transfer distribution " + itemTransferDistribution.ItemTransferReceiveId + " " + itemTransferDistribution.AccountCode);
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
