using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionStores
{
    public class ProductionIssueItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult IssueItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductionIssueItem> items = db.ProductionIssueItems.Where(itm => itm.ProductionIssueId == id);
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                ProductionIssueItemId = item.ProductionIssueItemId,
                ProductionIssueId = item.ProductionIssueId,
                ItemCategoryCode = item.ItemCategoryCode,
                ItemCode = item.ItemCode,
                IssuedQuantity = item.IssuedQuantity,
                UnitPrice = item.UnitPrice,
                AdditionalCost = item.AdditionalCost,
                Total = item.Total,

                OrgBranchName = item.OrgBranchName,
                DateCreated = item.DateCreated,
                LastModified = item.LastModified,
                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy
            });

            return Json(result);
        }

        private bool ItemExists(ProductionIssueItem record, int headerId)
        {
            return
                db.ProductionIssueItems
                .FirstOrDefault(i =>
                    i.ProductionIssueId == headerId &&
                    i.ItemCode == record.ItemCode &&
                    i.ProductionIssueItemId != record.ProductionIssueItemId
                    ) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueItem_Create([DataSourceRequest]DataSourceRequest request, ProductionIssueItem item, int id)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(item, id))
                {
                    ModelState.AddModelError("", "Item Exists!!");
                }
                else
                {
                    var entity = new ProductionIssueItem
                    {
                        ProductionIssueItemId = item.ProductionIssueItemId,
                        ProductionIssueId = id,
                        ItemCategoryCode = item.ItemCategoryCode,
                        ItemCode = item.ItemCode,
                        IssuedQuantity = item.IssuedQuantity,
                        UnitPrice = this.GetAverageUnitPrice(item.ItemCode, id),
                        AdditionalCost = item.AdditionalCost,
                        Total = item.Total,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = item.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = item.ModifiedBy
                    };
                    entity.Total = (entity.UnitPrice * entity.IssuedQuantity) + entity.AdditionalCost;

                    db.ProductionIssueItems.Add(entity);
                    db.SaveChanges();
                    item.ProductionIssueItemId = entity.ProductionIssueItemId;
                    new ProductionDistributionHelper().UpdateIssueDistribution(GetIssueHeader(entity.ProductionIssueId));

                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private ProductionIssue GetIssueHeader(int productionIssueId)
        {
            return db.ProductionIssues.Find(productionIssueId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueItem_Update([DataSourceRequest]DataSourceRequest request, ProductionIssueItem item)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(item, item.ProductionIssueId))
                {
                    ModelState.AddModelError("", "Item Exists!!");
                }
                else
                {
                    var entity = new ProductionIssueItem
                    {
                        ProductionIssueItemId = item.ProductionIssueItemId,
                        ProductionIssueId = item.ProductionIssueId,
                        ItemCategoryCode = item.ItemCategoryCode,
                        ItemCode = item.ItemCode,
                        IssuedQuantity = item.IssuedQuantity,
                        UnitPrice = this.GetAverageUnitPrice(item.ItemCode, item.ProductionIssueId),
                        AdditionalCost = item.AdditionalCost,
                        Total = item.Total,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = item.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = item.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };
                    entity.Total = (entity.UnitPrice * entity.IssuedQuantity) + entity.AdditionalCost;


                    db.ProductionIssueItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    new ProductionDistributionHelper().UpdateIssueDistribution(GetIssueHeader(entity.ProductionIssueId));
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueItem_Destroy([DataSourceRequest]DataSourceRequest request, ProductionIssueItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionIssueItem
                {
                    ProductionIssueItemId = item.ProductionIssueItemId,
                    ProductionIssueId = item.ProductionIssueId,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    IssuedQuantity = item.IssuedQuantity,
                    UnitPrice = item.UnitPrice,
                    AdditionalCost = item.AdditionalCost,
                    Total = item.Total,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy
                };

                db.ProductionIssueItems.Attach(entity);
                db.ProductionIssueItems.Remove(entity);
                db.SaveChanges();

                new ProductionDistributionHelper().UpdateIssueDistribution(GetIssueHeader(entity.ProductionIssueId));
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private decimal GetAverageUnitPrice(string itemCode, int headerId)
        {
            var price = new decimal();
            var issue = db.ProductionIssues.Find(headerId);
            if(issue != null)
            {
                price = InventoryHelper.GetItemAveragePrice(itemCode, issue.StoreCode);
            }
            else
            {
                price = InventoryHelper.GetUnitPrice(itemCode);
            }
            return price;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}