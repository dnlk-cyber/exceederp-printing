using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class ConstructionItemCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var types = db.ItemCategorys.Select(x => new { x.ItemCategoryCode, x.Name }).ToList();
            ViewData["Categories"] = types;

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
        public ActionResult ConstructionItemCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ConstructionItemCategory> ConstructionItemCategories = db.ConstructionItemCategories;
            DataSourceResult result = ConstructionItemCategories.ToDataSourceResult(request, ConstructionItemCategory => new
            {
                ConstructionItemCategoryId = ConstructionItemCategory.ConstructionItemCategoryId,
                ItemCategoryCode = ConstructionItemCategory.ItemCategoryCode,
                IsFuel = ConstructionItemCategory.IsFuel,
                IsReinforcementBar = ConstructionItemCategory.IsReinforcementBar,
                Remark = ConstructionItemCategory.Remark,
                Name = ConstructionItemCategory.Name
            });

            return Json(result);
        }

        private bool CheckExistance(string ItemCategoryCode)
        {
            var ConstructionItemCategory = db.ConstructionItemCategories.Where(x => x.ItemCategoryCode == ItemCategoryCode).FirstOrDefault();
            if (ConstructionItemCategory != null)
            {
                return true;
            }
            return false;
        }
        private bool IsInvalidSelection(ConstructionItemCategory model)
        {
            bool isInvalid = false;
            if (model.IsFuel && model.IsReinforcementBar)
            {
                isInvalid = true;
            }
            return isInvalid;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConstructionItemCategory_Create([DataSourceRequest]DataSourceRequest request, ConstructionItemCategory ConstructionItemCategory)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(ConstructionItemCategory.ItemCategoryCode))
                {
                    ModelState.AddModelError("", "This Construction Item Category type is already registered!!");
                }
                else if (IsInvalidSelection(ConstructionItemCategory))
                {
                    ModelState.AddModelError("", "Fuel and Reinforcement bar!!! invalid selecdtion");
                }
                else
                {
                    var entity = new ConstructionItemCategory
                    {
                        ConstructionItemCategoryId = ConstructionItemCategory.ConstructionItemCategoryId,
                        ItemCategoryCode = ConstructionItemCategory.ItemCategoryCode,
                        IsFuel = ConstructionItemCategory.IsFuel,
                        IsReinforcementBar = ConstructionItemCategory.IsReinforcementBar,
                        Name = GetName(ConstructionItemCategory.ItemCategoryCode),
                        Remark = ConstructionItemCategory.Remark
                    };

                    db.ConstructionItemCategories.Add(entity);
                    db.SaveChanges();
                    ConstructionItemCategory.ConstructionItemCategoryId = entity.ConstructionItemCategoryId;

                }
            }

            return Json(new[] { ConstructionItemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConstructionItemCategory_Update([DataSourceRequest]DataSourceRequest request, ConstructionItemCategory ConstructionItemCategory)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidSelection(ConstructionItemCategory))
                {
                    ModelState.AddModelError("", "Fuel and Reinforcement bar!!! invalid selecdtion");
                }
                else
                {
                    var entity = new ConstructionItemCategory
                    {
                        ConstructionItemCategoryId = ConstructionItemCategory.ConstructionItemCategoryId,
                        ItemCategoryCode = ConstructionItemCategory.ItemCategoryCode,
                        IsFuel = ConstructionItemCategory.IsFuel,
                        IsReinforcementBar = ConstructionItemCategory.IsReinforcementBar,
                        Name = GetName(ConstructionItemCategory.ItemCategoryCode),
                        Remark = ConstructionItemCategory.Remark
                    };

                    db.ConstructionItemCategories.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { ConstructionItemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConstructionItemCategory_Destroy([DataSourceRequest]DataSourceRequest request, ConstructionItemCategory ConstructionItemCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConstructionItemCategory
                {
                    ConstructionItemCategoryId = ConstructionItemCategory.ConstructionItemCategoryId,
                    ItemCategoryCode = ConstructionItemCategory.ItemCategoryCode,
                    IsFuel = ConstructionItemCategory.IsFuel,
                    IsReinforcementBar = ConstructionItemCategory.IsReinforcementBar,
                    Name = ConstructionItemCategory.Name,
                    Remark = ConstructionItemCategory.Remark
                };

                db.ConstructionItemCategories.Attach(entity);
                db.ConstructionItemCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ConstructionItemCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
