using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class MonthlyFuelCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/MonthlyFuelCost
        public ActionResult Index()
        {
            List<YearViewModel> years = new List<YearViewModel>();
            //List<string> years = new List<string>();
            for(int i = -2; i <= 0; i++)
            {
                var yearValue = DateTime.Now.AddYears(i).ToString("yyyy");
                var year = new YearViewModel();
                year.Title = yearValue;
                years.Add(year);
                //years.Add(DateTime.Now.AddYears(i).ToString("yyyy"));
            }

            ViewData["Years"] = years;

            return View();
        }

        public ActionResult MonthlyFuelCost_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MonthlyFuelCost> fuelCost = db.MonthlyFuelCost;
            DataSourceResult result = fuelCost.ToDataSourceResult(request, monthlyFuelCost => new {
                MonthlyFuelCostId = monthlyFuelCost.MonthlyFuelCostId,
                Month = monthlyFuelCost.Month,
                Year = monthlyFuelCost.Year,
                FuelType = monthlyFuelCost.FuelType,
                Price = monthlyFuelCost.Price
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyFuelCost_Create([DataSourceRequest]DataSourceRequest request, MonthlyFuelCost monthlyFuelCost)
        {
            if (ModelState.IsValid)
            {
                if (this.ValueExists(monthlyFuelCost)) {
                    ModelState.AddModelError("ValueExists", "Month-Fuel type pair exists");
                }
                var entity = new MonthlyFuelCost
                {
                    MonthlyFuelCostId = monthlyFuelCost.MonthlyFuelCostId,
                    Month = monthlyFuelCost.Month,
                    Year = monthlyFuelCost.Year,
                    FuelType = monthlyFuelCost.FuelType,
                    Price = monthlyFuelCost.Price
                };

                db.MonthlyFuelCost.Add(entity);
                db.SaveChanges();
                monthlyFuelCost.MonthlyFuelCostId = entity.MonthlyFuelCostId;
            }

            return Json(new[] { monthlyFuelCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyFuelCost_Update([DataSourceRequest]DataSourceRequest request, MonthlyFuelCost monthlyFuelCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyFuelCost
                {
                    MonthlyFuelCostId = monthlyFuelCost.MonthlyFuelCostId,
                    Month = monthlyFuelCost.Month,
                    Year = monthlyFuelCost.Year,
                    FuelType = monthlyFuelCost.FuelType,
                    Price = monthlyFuelCost.Price
                };

                db.MonthlyFuelCost.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { monthlyFuelCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyFuelCost_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyFuelCost monthlyFuelCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyFuelCost
                {
                    MonthlyFuelCostId = monthlyFuelCost.MonthlyFuelCostId,
                    Month = monthlyFuelCost.Month,
                    Year = monthlyFuelCost.Year,
                    FuelType = monthlyFuelCost.FuelType,
                    Price = monthlyFuelCost.Price
                };

                db.MonthlyFuelCost.Attach(entity);
                db.MonthlyFuelCost.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { monthlyFuelCost }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private bool ValueExists(MonthlyFuelCost model)
        {
            bool monthFuelCostExists = false;
            var list = db.MonthlyFuelCost.ToList();
            foreach (var item in list) {
                if(item.Month == model.Month && item.FuelType == model.FuelType && item.Year == model.Year)
                {
                    monthFuelCostExists = true;
                }
            }
            return monthFuelCostExists;
        }
    }
}