using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingProductionAllottedRawMaterialController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id) 
        {
            var entity = db.PrintingProductionAllottedRawMaterials.Where(x => x.PrintingProductionFollowUpId == id);
            return Json(entity.ToDataSourceResult(request));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingProductionAllottedRawMaterial model)
        {
            if (ModelState.IsValid)
            {
                db.PrintingProductionAllottedRawMaterials.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}