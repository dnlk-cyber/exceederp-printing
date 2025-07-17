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


            return View();
        }

        public ActionResult PrintingJobOrderProduction_Read([DataSourceRequest]DataSourceRequest request)
        {
            var entity = db.PrintingProductionFollowUps;
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

        //#region PrePressCheckAndApproval
        //public JsonResult PrePressCheck(int id)
        //{
        //    Object response = null;
        //    var followUp = db.PrintingProductionFollowUps.Find(id);
        //    if (followUp != null)
        //    {
        //        followUp.IsPrePressChecked = true;
        //        followUp.PrePressCheckedBy = User.Identity.Name;
        //        followUp.PrePressCheckedTime = DateTime.Today;
        //        db.Entry(followUp).State = EntityState.Modified;
        //        db.SaveChanges();
        //        response = new { Success = true, Message = "Successfully Checked" };
        //    }
        //    else
        //    {
        //        response = new { Success = false, Message = "Something's Wrong!" };
        //    }
        //    return Json(response);
        //}

        //public JsonResult PrePressApprove(int id) 
        //{
        //    Object response = null;
        //    var productionFollowUp = db.PrintingProductionFollowUps.Find(id);
        //    if (productionFollowUp != null)
        //    {

        //        productionFollowUp.IsPrePressApproved = true;
        //        productionFollowUp.PrePressApprovedBy = User.Identity.Name;
        //        productionFollowUp.PrePressApprovedTime = DateTime.Now;

        //        db.Entry(productionFollowUp).State = EntityState.Modified;
        //        db.SaveChanges();
                
        //        response = new { Success = true, Message = "Successfully Approved" };


        //    }
        //    else
        //    {
        //        response = new { Success = false, Message = "Something's Wrong!" };
        //    }
        //    return Json(response);
        //}
        //#endregion

        //#region PressCheckAndApproval
        //public JsonResult PressCheck(int id)
        //{
        //    Object response = null;
        //    var followUp = db.PrintingProductionFollowUps.Find(id);
        //    if (followUp != null)
        //    {
        //        followUp.IsPressChecked = true;
        //        followUp.PressCheckedBy = User.Identity.Name;
        //        followUp.PressCheckedTime = DateTime.Today;
        //        db.Entry(followUp).State = EntityState.Modified;
        //        db.SaveChanges();
        //        response = new { Success = true, Message = "Successfully Checked" };
        //    }
        //    else
        //    {
        //        response = new { Success = false, Message = "Something's Wrong!" };
        //    }
        //    return Json(response);
        //}

        //public JsonResult PressApprove(int id)
        //{
        //    Object response = null;
        //    var productionFollowUp = db.PrintingProductionFollowUps.Find(id);
        //    if (productionFollowUp != null)
        //    {

        //        productionFollowUp.IsPressApproved = true;
        //        productionFollowUp.PressApprovedBy = User.Identity.Name;
        //        productionFollowUp.PressApprovedTime = DateTime.Now;

        //        db.Entry(productionFollowUp).State = EntityState.Modified;
        //        db.SaveChanges();

        //        response = new { Success = true, Message = "Successfully Approved" };


        //    }
        //    else
        //    {
        //        response = new { Success = false, Message = "Something's Wrong!" };
        //    }
        //    return Json(response);
        //}
        //#endregion

        //#region PostPressCheckAndApproval
        //public JsonResult PostPressCheck(int id)
        //{
        //    Object response = null;
        //    var followUp = db.PrintingProductionFollowUps.Find(id);
        //    if (followUp != null)
        //    {
        //        followUp.IsPostPressChecked = true;
        //        followUp.PostPressCheckedBy = User.Identity.Name;
        //        followUp.PostPressCheckedTime = DateTime.Today;
        //        db.Entry(followUp).State = EntityState.Modified;
        //        db.SaveChanges();
        //        response = new { Success = true, Message = "Successfully Checked" };
        //    }
        //    else
        //    {
        //        response = new { Success = false, Message = "Something's Wrong!" };
        //    }
        //    return Json(response);
        //}

        //public JsonResult PostPressApprove(int id)
        //{
        //    Object response = null;
        //    var productionFollowUp = db.PrintingProductionFollowUps.Find(id);
        //    if (productionFollowUp != null)
        //    {

        //        productionFollowUp.IsPostPressApproved = true;
        //        productionFollowUp.PostPressApprovedBy = User.Identity.Name;
        //        productionFollowUp.PostPressApprovedTime = DateTime.Now;

        //        db.Entry(productionFollowUp).State = EntityState.Modified;
        //        db.SaveChanges();

        //        response = new { Success = true, Message = "Successfully Approved" };


        //    }
        //    else
        //    {
        //        response = new { Success = false, Message = "Something's Wrong!" };
        //    }
        //    return Json(response);
        //}
        //#endregion
    }
}