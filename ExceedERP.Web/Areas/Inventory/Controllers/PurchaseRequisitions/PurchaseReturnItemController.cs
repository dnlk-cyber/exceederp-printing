using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.PurchaseRequisitions
{
   
    public class PurchaseReturnItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult PurchaseReturnItemss_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<PurchaseReturnItems> purchasereturnItemss = db.PurchaseReturnItemss.Where(pr => pr.PurchaseReturnID == id);
            DataSourceResult result = purchasereturnItemss.ToDataSourceResult(request, purchaseReturnItems => new {
                PurchaseReturnItemsID = purchaseReturnItems.PurchaseReturnItemsID,
                PurchaseReturnID = purchaseReturnItems.PurchaseReturnID,
                ItemCategoryCode = purchaseReturnItems.ItemCategoryCode,
                ItemCode = purchaseReturnItems.ItemCode,
                Quantity = purchaseReturnItems.Quantity,
                UnitPrice = purchaseReturnItems.UnitPrice,
                SubTotal = purchaseReturnItems.SubTotal,
                GLTaxRateID = purchaseReturnItems.GLTaxRateID,
                TaxAmount = purchaseReturnItems.TaxAmount,
                TotalAmount = purchaseReturnItems.TotalAmount,
                OrgBranchName = purchaseReturnItems.OrgBranchName,
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseReturnItemss_Create([DataSourceRequest]DataSourceRequest request, PurchaseReturnItems purchaseReturnItems , int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseReturnItems
                {
                   PurchaseReturnID = id,
                   ItemCategoryCode = purchaseReturnItems.ItemCategoryCode,
                   ItemCode = purchaseReturnItems.ItemCode,
                    Quantity = purchaseReturnItems.Quantity,
                    UnitPrice = purchaseReturnItems.UnitPrice,
                    SubTotal = purchaseReturnItems.SubTotal,
                    GLTaxRateID = purchaseReturnItems.GLTaxRateID,
                    TaxAmount = purchaseReturnItems.TaxAmount,
                    TotalAmount = purchaseReturnItems.TotalAmount,
                    OrgBranchName = purchaseReturnItems.OrgBranchName,
                   CreatedBy = User.Identity.Name,
                   DateCreated = DateTime.Now
                };

                db.PurchaseReturnItemss.Add(entity);
                db.SaveChanges();
                purchaseReturnItems.PurchaseReturnItemsID = entity.PurchaseReturnItemsID;
            }

            return Json(new[] { purchaseReturnItems }.ToDataSourceResult(request, ModelState));
        }
      
       [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseReturnItemss_Update([DataSourceRequest]DataSourceRequest request, PurchaseReturnItems purchaseReturnItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseReturnItems
                {
                    PurchaseReturnItemsID = purchaseReturnItems.PurchaseReturnItemsID,
                    PurchaseReturnID = purchaseReturnItems.PurchaseReturnID,
                    ItemCategoryCode = purchaseReturnItems.ItemCategoryCode,
                    ItemCode = purchaseReturnItems.ItemCode,
                    Quantity = purchaseReturnItems.Quantity,
                    UnitPrice = purchaseReturnItems.UnitPrice,
                    SubTotal = purchaseReturnItems.Quantity * purchaseReturnItems.UnitPrice,
                    GLTaxRateID = purchaseReturnItems.GLTaxRateID,
                    TaxAmount = purchaseReturnItems.TaxAmount,
                    TotalAmount = purchaseReturnItems.SubTotal + (purchaseReturnItems.SubTotal * purchaseReturnItems.TaxAmount / 100),
                    OrgBranchName = purchaseReturnItems.OrgBranchName,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                db.PurchaseReturnItemss.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                //UpdateDistribution(db.PurchaseReturns.Find(entity.PurchaseReturnID));
            }

            return Json(new[] { purchaseReturnItems }.ToDataSourceResult(request, ModelState));
        }
        
        private void UpdateDistribution(PurchaseReturn purchaseReturn)
        {
            if (purchaseReturn.PurchaseReturnDistribution != null)
                {
                    db.PurchaseReturnDistributions.RemoveRange(purchaseReturn.PurchaseReturnDistribution);
                    db.SaveChanges();
                }
            PurchaseReturnDistribution distribution;
            GoodReceive goodReceive = db.GoodReceives.Find(purchaseReturn.GoodReceiveId);
            if (goodReceive != null)
            {
                int poId = int.Parse(goodReceive.ReceiveRef);
                var purchaseOrder = db.PurchaseOrders.Find(poId);
                // int sid = int.Parse(PurchaseOrder.Supplier);to be updated
                ProcurementSupplier supplier = db.ProcurementSuppliers.Find(1);//to be updated
                #region Debit
                if (purchaseReturn.PurchaseReturnItems.Sum(x => x.TotalAmount) != 0)
                {
                    distribution = new PurchaseReturnDistribution();
                    if (supplier != null) distribution.AccountCode = supplier.AccountCode;
                    distribution.Debit = purchaseReturn.PurchaseReturnItems.Sum(x => x.TotalAmount);
                    distribution.Credit = 0;
                    distribution.PurchaseReturnID = purchaseReturn.PurchaseReturnID;

                    db.PurchaseReturnDistributions.Add(distribution);
                    db.SaveChanges();
                }
                #endregion
                #region credit
                foreach (var line in purchaseReturn.PurchaseReturnItems)
                {
                    if (line.SubTotal != 0 && line.TotalAmount != 0)
                    {
                        var item = db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                        if (item != null)
                        {
                            string acc = item.ItemCategory.AccountCode;
                            string[] account = acc.Split('-');
                            if (purchaseOrder != null)
                            {
                                string newacc = account[0] + "-" + account[1] + "-" + purchaseOrder.OrgStructureId + "-" +account[3];

                                distribution = new PurchaseReturnDistribution
                                {
                                    AccountCode = newacc,
                                    Debit = 0,
                                    Credit = line.TotalAmount,
                                    PurchaseReturnID = line.PurchaseReturnID,
                                    PurchaseReturnDistributionID = line.PurchaseReturnItemsID
                                };
                                db.PurchaseReturnDistributions.Add(distribution);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

            #endregion
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseReturnItemss_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseReturnItems purchaseReturnItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseReturnItems
                {
                    PurchaseReturnItemsID = purchaseReturnItems.PurchaseReturnItemsID,
                    InventoryItem = purchaseReturnItems.InventoryItem,
                    Quantity = purchaseReturnItems.Quantity,
                    UnitPrice = purchaseReturnItems.UnitPrice,
                    SubTotal = purchaseReturnItems.SubTotal,
                    GLTaxRateID = purchaseReturnItems.GLTaxRateID,
                    TaxAmount = purchaseReturnItems.TaxAmount,
                    TotalAmount = purchaseReturnItems.TotalAmount,
                    OrgBranchName = purchaseReturnItems.OrgBranchName,
                };

                db.PurchaseReturnItemss.Attach(entity);
                db.PurchaseReturnItemss.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted purchase return item " + purchaseReturnItems.PurchaseReturnID + " " + purchaseReturnItems.InventoryItem.Name);
                #endregion
            }

            return Json(new[] { purchaseReturnItems }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
