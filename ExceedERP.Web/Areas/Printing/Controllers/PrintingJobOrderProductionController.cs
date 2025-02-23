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
    public class PrintingJobOrderProductionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/PrintingJobOrderProduction
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            ViewData["JobType"] = db.PrintingJobTypes.Select(s => new
            {
                Value = s.PrintingJobTypeId,
                Text = s.JobTypeName
            }).ToList();
           
            ViewData["PaperSize"] = db.PrintingPaperSizes.Select(s => new
            {
                Value = s.PrintingPaperSizeId,
                Text = s.PaperSizeName
            }).ToList();
            ViewData["BindingStyle"] = db.PrintingBindingStyles.Select(s => new
            {
                Value = s.PrintingBindingStyleId,
                Text = s.BindingStyleName
            }).ToList();

            ViewData["Category"] = db.PrintingMaterialCategories.Select(s => new
            {
                Value = s.PrintingMaterialCategoryId,
                Text = s.CategoryName
            }).ToList();
            ViewData["Item"] = db.PrintingMaterialCategoryItems.Select(s => new
            {
                Value = s.PrintingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();
            ViewData["PrintingProcess"] = db.PrintingProcesses.Select(s => new
            {
                Value = s.PrintingProcessId,
                Text = s.ProcessName
            }).ToList();
            ViewData["PrintingMachineType"] = db.PrintingMachineTypes.Select(s => new
            {
                Value = s.PrintingMachineTypeId,
                Text = s.MachineTypeName
            }).ToList();
            return View();
        }

        public ActionResult PrintingJobOrderProduction_Read([DataSourceRequest]DataSourceRequest request)
        {
            var entity = db.PrintingJobOrderProductions;
            return Json(entity.ToDataSourceResult(request));
        }

        public ActionResult PrintingOrderProduction_Update([DataSourceRequest]DataSourceRequest request, PrintingJobOrderProduction model)
        {
            if (ModelState.IsValid)
            {
                db.PrintingJobOrderProductions.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}