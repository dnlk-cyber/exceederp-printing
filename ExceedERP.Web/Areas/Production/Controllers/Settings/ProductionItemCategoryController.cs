using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Production;

namespace ExceedERP.Web.Areas.Production.Controllers.Settings
{
    //[AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class ProductionItemCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var types = db.ItemCategorys.ToList();
            ViewData["Categories"] = types.Select(x => new { ItemCategoryCode = x.ItemCategoryCode, Name = x.Name });

            return View();
        }

        private string GetName(string CategoryCode)
        {
            var categories = db.ItemCategorys.Where(x => x.ItemCategoryCode == CategoryCode).FirstOrDefault();
            if (categories != null)
            {
                return categories.Name;
            }
            return "";
        }
        public ActionResult ItemCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionItemCategory> itemCategories = db.ProductionItemCategories;
            DataSourceResult result = itemCategories.ToDataSourceResult(request, itemCategory => new
            {
                ProductionItemCategoryId = itemCategory.ProductionItemCategoryId,
                ItemCategoryCode = itemCategory.ItemCategoryCode,
                //IsFuel = itemCategory.IsFuel,
                //IsReinforcementBar = itemCategory.IsReinforcementBar,
                Remark = itemCategory.Remark,
                Name = itemCategory.Name
            });

            return Json(result);
        }

        private bool CheckExistance(string categoryCode)
        {
            var itemCategory = db.ProductionItemCategories.Where(x => x.ItemCategoryCode == categoryCode).FirstOrDefault();
            if (itemCategory != null)
            {
                return true;
            }
            return false;
        }
        //private bool IsInvalidSelection( itemCategory model) {
        //    bool isInvalid = false;
        //    if(model.IsFuel && model.IsReinforcementBar)
        //    {
        //        isInvalid = true;
        //    }
        //    return isInvalid;
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemCategory_Create([DataSourceRequest]DataSourceRequest request, ProductionItemCategory itemCategory)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(itemCategory.ItemCategoryCode))
                {
                    ModelState.AddModelError("", "This  Item Category type is already registered!!");
                }

                else
                {
                    var entity = new ProductionItemCategory
                    {
                        ProductionItemCategoryId = itemCategory.ProductionItemCategoryId,
                        ItemCategoryCode = itemCategory.ItemCategoryCode,
                        //IsFuel = itemCategory.IsFuel,
                        //IsReinforcementBar = itemCategory.IsReinforcementBar,
                        Name = GetName(itemCategory.ItemCategoryCode),
                        Remark = itemCategory.Remark
                    };

                    db.ProductionItemCategories.Add(entity);
                    db.SaveChanges();
                    itemCategory.ProductionItemCategoryId = entity.ProductionItemCategoryId;

                }
            }

            return Json(new[] { itemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemCategory_Update([DataSourceRequest]DataSourceRequest request, ProductionItemCategory itemCategory)
        {
            if (ModelState.IsValid)
            {
                //if (IsInvalidSelection(itemCategory)) {
                //    ModelState.AddModelError("", "Fuel and Reinforcement bar!!! invalid selecdtion");
                //}
                //else
                //{
                var entity = new ProductionItemCategory
                {
                    ProductionItemCategoryId = itemCategory.ProductionItemCategoryId,
                    ItemCategoryCode = itemCategory.ItemCategoryCode,
                    //IsFuel = itemCategory.IsFuel,
                    //IsReinforcementBar = itemCategory.IsReinforcementBar,
                    Name = GetName(itemCategory.ItemCategoryCode),
                    Remark = itemCategory.Remark
                };

                db.ProductionItemCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                //}
            }

            return Json(new[] { itemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemCategory_Destroy([DataSourceRequest]DataSourceRequest request, ProductionItemCategory itemCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionItemCategory
                {
                    ProductionItemCategoryId = itemCategory.ProductionItemCategoryId,
                    ItemCategoryCode = itemCategory.ItemCategoryCode,
                    //IsFuel = itemCategory.IsFuel,
                    //IsReinforcementBar = itemCategory.IsReinforcementBar,
                    Name = GetName(itemCategory.ItemCategoryCode),
                    Remark = itemCategory.Remark
                };

                db.ProductionItemCategories.Attach(entity);
                db.ProductionItemCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { itemCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
