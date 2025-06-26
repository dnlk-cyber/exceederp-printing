using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.EquipmentInspection;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Reporting.Fleet.ViewModel;
using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.InspectionUser, FleetManagementRoles.InspectionApprover)]

    public class AnnualEquipmentInventoryInspectionVMController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
         
        public ActionResult Index()
        {
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            {
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();
            ViewData["Type"] = db.EquipmentType.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["Name"] = db.EquipmentName.Select(s => new
            {
                Value = s.EquipmentNameID,
                Text = s.Name
            }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            }).ToList();

            return View();
        }


      
       

        public ActionResult AnnualEquipmentInventoryInspection_Read([DataSourceRequest] DataSourceRequest request,string id)
        {
            var AnnualEquipmentInventoryInspection = db.AnnualEquipmentInventoryInspection.Where(x=>x.FleetsID.ToString()==id).Select(model => new AnnualEquipmentInventoryInspectionVM
            {
                AnnualEquipmentInventoryInspectionID=model.AnnualEquipmentInventoryInspectionID,
                DateofInspection = model.DateofInspection,
                FleetsID = model.FleetsID,
                EquipmentCategoryId = model.EquipmentCategoryId,
                EquipmentNameId = model.EquipmentNameId,
                EquipmentTypeId = model.EquipmentTypeId,
               
            });

            return Json(AnnualEquipmentInventoryInspection.ToDataSourceResult(request));
        }


      
    }
}
