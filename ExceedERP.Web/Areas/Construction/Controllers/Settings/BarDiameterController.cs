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
    public class BarDiameterController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/BarDiameter
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BarDiameters_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BarDiameter> barDiameters = db.BarDiameter;
            DataSourceResult result = barDiameters.ToDataSourceResult(request, barDiameter => new {
                BarDiameterId = barDiameter.BarDiameterId,
                ConcatnatedCode = barDiameter.ConcatnatedCode,
                Diameter = barDiameter.Diameter,
                UnitWeight = CalculateUnitWeight(barDiameter),
                Description = barDiameter.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BarDiameter_Create([DataSourceRequest]DataSourceRequest request, BarDiameter barDiameter)
        {
            if (ModelState.IsValid)
            {
                var entity = new BarDiameter
                {
                    BarDiameterId = barDiameter.BarDiameterId,
                    ConcatnatedCode = barDiameter.ConcatnatedCode,
                    Diameter = barDiameter.Diameter,
                    UnitWeight = CalculateUnitWeight(barDiameter),
                    Description = barDiameter.Description
                };

                db.BarDiameter.Add(entity);
                db.SaveChanges();
                barDiameter.BarDiameterId = entity.BarDiameterId;
                barDiameter.UnitWeight = entity.UnitWeight;
            }

            return Json(new[] { barDiameter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BarDiameter_Update([DataSourceRequest]DataSourceRequest request, BarDiameter barDiameter)
        {
            if (ModelState.IsValid)
            {
                var entity = new BarDiameter
                {
                    BarDiameterId = barDiameter.BarDiameterId,
                    ConcatnatedCode = barDiameter.ConcatnatedCode,
                    Diameter = barDiameter.Diameter,
                    UnitWeight = CalculateUnitWeight(barDiameter),
                    Description = barDiameter.Description
                };

                db.BarDiameter.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { barDiameter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BarDiameter_Destroy([DataSourceRequest]DataSourceRequest request, BarDiameter barDiameter)
        {
            if (ModelState.IsValid)
            {
                var entity = new BarDiameter
                {
                    BarDiameterId = barDiameter.BarDiameterId,
                    ConcatnatedCode = barDiameter.ConcatnatedCode,
                    Diameter = barDiameter.Diameter,
                    UnitWeight = barDiameter.UnitWeight,
                    Description = barDiameter.Description
                };

                db.BarDiameter.Attach(entity);
                db.BarDiameter.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { barDiameter }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalculateUnitWeight( BarDiameter model) {
            decimal unit = new decimal();
            if(model != null)
            {
                unit = ((decimal)0.222 * model.Diameter * model.Diameter) / 36;
            }
            return unit;
        }
    }
}