using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingMachineTypeDetailController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/PrintingMachineTypeDetail
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var detail = db.PrintingMachineDetails.Where(x => x.PrintingMachineTypeId == id);
            return Json(detail.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingMachineDetail model, int id)
        {
            var machineDetails = db.PrintingMachineDetails.Where(x => x.PaperSizeId == model.PaperSizeId && x.PrintingMachineTypeId == id).ToList();
            if (machineDetails.Any())
            {
                ModelState.AddModelError(string.Empty, "Dupliacte Entry Not Allowed");
            }
            if(model.SectionUp == 0)
            {
                ModelState.AddModelError(string.Empty, "Section/Up needs value other than 0");
            }
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineDetail
                {
                    PaperSizeId = model.PaperSizeId,
                    PrintingMachineTypeId = id,
                    SectionUp = model.SectionUp
                };

                db.PrintingMachineDetails.Add(entity);
                db.SaveChanges();
                model.PrintingMachineDetailId = entity.PrintingMachineDetailId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingMachineDetail model)
        {
            var machineDetails = db.PrintingMachineDetails.Where(x => x.PaperSizeId == model.PaperSizeId && x.PrintingMachineTypeId == model.PrintingMachineTypeId).ToList();
            if (machineDetails.Any())
            {
                ModelState.AddModelError(string.Empty, "Dupliacte Entry Not Allowed");
            }
            if (model.SectionUp == 0)
            {
                ModelState.AddModelError(string.Empty, "Section/Up needs value other than 0");
            }
            if (ModelState.IsValid)
            {
                var entity = new PrintingMachineDetail
                {
                    PrintingMachineDetailId = model.PrintingMachineDetailId,
                    PaperSizeId = model.PaperSizeId,
                    PrintingMachineTypeId = model.PrintingMachineTypeId,
                    SectionUp = model.SectionUp
                };

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                model.PrintingMachineDetailId = entity.PrintingMachineDetailId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingMachineDetail model)
        {
            if (ModelState.IsValid)
            {
                var detail = db.PrintingMachineDetails.Find(model.PrintingMachineDetailId);
                db.PrintingMachineDetails.Remove(detail);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }
    }
}