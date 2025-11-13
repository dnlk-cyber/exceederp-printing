using ExceedERP.Core.Domain.Printing;
using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintQualityInspectionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var entity = db.PrintingQualityInspections.Where(x => x.PrintingProductionFollowUpId == id);
            return Json(entity.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingQualityInspection model, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingQualityInspection
                {
                   PrintQualityGrade = model.PrintQualityGrade,
                   CreatedBy = User.Identity.Name,
                   DateCreated = DateTime.Today,
                   DateModified = null,
                   Description = model.Description,
                   ModifiedBy = null,
                   PrintingProductionFollowUpId = id
                };
                db.PrintingQualityInspections.Add(entity);               
                db.SaveChanges();
                
                model.PrintingQualityInspectionId = entity.PrintingQualityInspectionId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingQualityInspection model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.DateModified = DateTime.Now;
                db.PrintingQualityInspections.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingQualityInspection model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingQualityInspections.Find(model.PrintingQualityInspectionId);
                db.PrintingQualityInspections.Remove(entity);
                
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}