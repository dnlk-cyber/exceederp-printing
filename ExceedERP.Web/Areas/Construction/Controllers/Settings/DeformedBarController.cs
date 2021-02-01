using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class DeformedBarController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/DeformedBar
        public ActionResult Index()
        {
            var bars = (from item in db.InventoryItems
                        join category in db.ConstructionItemCategories
                        on item.ItemCategoryCode equals category.ItemCategoryCode
                        where category.IsReinforcementBar
                        //join justPriced in db.MaterialPriceIndexIndices
                        //on item.ItemCode equals justPriced.ItemCode
                        select new
                        {
                            ItemCode = item.ItemCode,
                            Name = item.Name
                        }).ToList();
            ViewData["ReinforcementBars"] = bars;

            return View();
        }

        public ActionResult DeformedBars_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DeformedBar> deformedBars = db.DeformedBars;
            DataSourceResult result = deformedBars.ToDataSourceResult(request, deformedBar => new {
                DeformedBarId = deformedBar.DeformedBarId,
                ItemCode = deformedBar.ItemCode,
                //Name = deformedBar.Name,
                Diameter = deformedBar.Diameter,
                UnitWeight = deformedBar.UnitWeight,
                //ConcatnatedCode = string.IsNullOrEmpty(deformedBar.ConcatnatedCode) ? deformedBar.Name + " " + deformedBar.Diameter + "mm" : deformedBar.ConcatnatedCode
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeformedBar_Create([DataSourceRequest]DataSourceRequest request, DeformedBar deformedBar)
        {
            if (ModelState.IsValid)
            {
                if (ValueExists(deformedBar)) {
                    ModelState.AddModelError("", "This bar type is regisreted already!!!");
                }
                else
                {
                    var entity = new DeformedBar
                    {
                        DeformedBarId = deformedBar.DeformedBarId,
                        ItemCode = deformedBar.ItemCode,
                        Name = InventoryHelper.GetItemName(deformedBar.ItemCode),
                        Diameter = deformedBar.Diameter,
                        UnitWeight = CalculateUnitWeight(deformedBar),
                        //ConcatnatedCode = deformedBar.Name + " [ " + deformedBar.Diameter + "mm ]"
                    };
                    db.DeformedBars.Add(entity);
                    db.SaveChanges();
                    deformedBar.DeformedBarId = entity.DeformedBarId;
                    deformedBar.UnitWeight = entity.UnitWeight;
                }
               
            }

            return Json(new[] { deformedBar }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeformedBar_Update([DataSourceRequest]DataSourceRequest request, DeformedBar deformedBar)
        {
            if (ModelState.IsValid)
            {
                var entity = new DeformedBar
                {
                    DeformedBarId = deformedBar.DeformedBarId,
                    ItemCode = deformedBar.ItemCode,
                    Name = InventoryHelper.GetItemName(deformedBar.ItemCode),
                    Diameter = deformedBar.Diameter,
                    UnitWeight = CalculateUnitWeight(deformedBar),
                    //ConcatnatedCode = deformedBar.Name + " [ " + deformedBar.Diameter + "mm ]"
                };

                db.DeformedBars.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                deformedBar.UnitWeight = entity.UnitWeight;
            }

            return Json(new[] { deformedBar }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeformedBar_Destroy([DataSourceRequest]DataSourceRequest request, DeformedBar deformedBar)
        {
            if (ModelState.IsValid)
            {
                var entity = new DeformedBar
                {
                    DeformedBarId = deformedBar.DeformedBarId,
                    ItemCode = deformedBar.ItemCode,
                    // Name = deformedBar.Name,
                    Diameter = deformedBar.Diameter,
                    UnitWeight = deformedBar.UnitWeight
                };

                db.DeformedBars.Attach(entity);
                db.DeformedBars.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { deformedBar }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalculateUnitWeight(DeformedBar model)
        {
            decimal unit = new decimal();
            if (model != null)
            {
                unit = ((decimal)0.222 * model.Diameter * model.Diameter) / 36;
                unit = Math.Round(unit, 4);
            }
            return unit;
        }
        private bool ValueExists(DeformedBar model)
        {
            bool valueExists = false;
            var existingBar = db.DeformedBars.Where(b => b.ItemCode == model.ItemCode).FirstOrDefault();
            if(existingBar != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}