using ExceedERP.Core.Domain.Printing;
using ExceedERP.Core.Domain.Printing.PrintingProduction;
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
    public class PrintingProductionFollowUpController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewDataMethod();
            return View();
        }

        public ActionResult IndexPress()
        {
            ViewDataMethod();
            return View();
        }
        public ActionResult IndexPostPress()
        {
            ViewDataMethod();
            return View();
        }

        private void ViewDataMethod()
        {
            ViewData["Machine"] = db.PrintingMachineTypes.Select(s => new
            {
                Text = s.MachineTypeName,
                Value = s.PrintingMachineTypeId
            });

            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();


            ViewData["PaperSize"] = db.PrintingPaperSizes.Select(s => new
            {
                Value = s.PrintingPaperSizeId,
                Text = s.PaperSizeName
            }).ToList();

            ViewData["PrintingMaterialCategory"] = db.PrintingMaterialCategories.Select(s => new
            {
                Value = s.PrintingMaterialCategoryId,
                Text = s.CategoryName
            }).ToList();
            ViewData["PItem"] = db.PrintingMaterialCategoryItems.Select(s => new
            {
                Value = s.PrintingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();
            ViewData["PrintingProcess"] = db.PrintingProcesses.Select(s => new
            {
                Value = s.PrintingProcessId,
                Text = s.LabourActivity
            }).ToList();
            ViewData["PrintingMachineType"] = db.PrintingMachineTypes.Select(s => new
            {
                Value = s.PrintingMachineTypeId,
                Text = s.MachineTypeName
            }).ToList();


            ViewData["Operator"] = db.Person_Employee.Select(s => new
            {
                Text = s.FirstNameEnglish + " " + s.MiddleNameEnglish + " " + s.LastNameEnglish,
                Value = s.EmployeeId
            }).ToList();


        }
        public ActionResult PrePress_Read([DataSourceRequest]DataSourceRequest request)
        {
            var entity = db.PrintingProductionFollowUps;
            return Json(entity.ToDataSourceResult(request));
        }
        public ActionResult Press_Read([DataSourceRequest]DataSourceRequest request)
        {
            var entity = db.PrintingProductionFollowUps.Where(x => x.IsPrePressApproved);
            return Json(entity.ToDataSourceResult(request));
        }
        public ActionResult PostPress_Read([DataSourceRequest]DataSourceRequest request)
        {
            var entity = db.PrintingProductionFollowUps.Where(x => x.IsPressApproved);
            return Json(entity.ToDataSourceResult(request));
        }

        public ActionResult PrintingOrderProduction_Update([DataSourceRequest]DataSourceRequest request, PrintingProductionFollowUp model)
        {
            if (ModelState.IsValid)
            {
                db.PrintingProductionFollowUps.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

       
    }
}