using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    [AuthorizeRoles(FleetManagementRoles.ResourceApprover)]
    public class EquipmentTypeController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        public  IEquipmentTypeService _EquipmentTypeService;
        public  IAccessoriesService _AccessoriesService;
        public  IFleetHistoryService _FleetHistoryService;
        public  IEquipmentIdentityService _EquipmentIdentityService;

        public EquipmentTypeController()
        {
            _EquipmentTypeService = new EquipmentTypeService();
            _EquipmentIdentityService = new EquipmentIdentityService();
            _AccessoriesService = new AccessoriesService();
            _FleetHistoryService = new FleetHistoryService();
        }
        //
        // GET: /FleetManagement/EquipmentType/
        public ActionResult Index()
        {
            ViewData["AssetCategory"] = db.FixedAssetCategories.ToList();
            ViewData["EquipmentIdentity"] = db.EquipmentIdentity.ToList();
           
                var equipSubCats = db.FixedAssetSubCategories.Select(s => new {
                    Id = s.FixedAssetSubCategoryID,
                    Name = s.SubCategoryCode + "_" + s.SubCategoryName
                }).ToList();
                ViewData["FixedAssetSubCategories"] = equipSubCats;
            var equipCatNames = db.FixedAssetSubCategoryNames.Select(s => new {
                Id = s.FixedAssetSubCategoryNameId,
                s.Name
            }).ToList();
            ViewData["FixedAssetSubCategoryNames"] = equipCatNames;

            return View();
        }



        public ActionResult EquipmentType_Read([DataSourceRequest] DataSourceRequest request)
        {
            var equipmentType = _EquipmentTypeService.GetAll();
            var EquipmentTypes = equipmentType.Select(e => new EquipmentType
            {
                EquipmentIdentityID = e.EquipmentIdentityID,
                FixedAssetSubCategoryId = e.FixedAssetSubCategoryId,
                EquipmentTypeID = e.EquipmentTypeID,
                CategoryName = e.CategoryName,
                EquipmentTypeName = e.EquipmentTypeName,
                Description = e.Description,
                TimeSheetType = e.TimeSheetType


            }
            );

            return this.Json(EquipmentTypes.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }



        public ActionResult EquipmentType_Create([DataSourceRequest] DataSourceRequest request, EquipmentType EquipmentType)
        {
            ModelState.Remove("EquipmentTypeID");
            List<EquipmentType> fleetsplate = _EquipmentTypeService.GetAll().Where(f => f.EquipmentTypeName == EquipmentType.EquipmentTypeName).ToList();
            if (fleetsplate.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Equipment Type Name Already Exists ");
                return Json(new[] { EquipmentType }.ToDataSourceResult(request, ModelState));
            }
            else
            {
             
                 if (EquipmentType != null)
                {
                    EquipmentType.EquipmentTypeID = Guid.NewGuid();
                    EquipmentType.CreatedBy = User.Identity.Name;
                    EquipmentType.DateCreated = DateTime.Now;

                    _EquipmentTypeService.Insert(EquipmentType);

                }
            }
            return Json(new[] { EquipmentType }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentType_Update([DataSourceRequest] DataSourceRequest request, EquipmentType EquipmentType)
        {
            List<EquipmentType> fleetsplate = _EquipmentTypeService.GetAll().Where(f => f.EquipmentTypeName == EquipmentType.EquipmentTypeName && f.EquipmentTypeID != EquipmentType.EquipmentTypeID).ToList();
            if (fleetsplate.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Equipment Type Name Already Exists ");
                return Json(new[] { EquipmentType }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                if (EquipmentType != null)
                {
                    EquipmentType.ModifiedBy = User.Identity.Name;
                    EquipmentType.LastModified = DateTime.Now;

                    _EquipmentTypeService = new EquipmentTypeService();
                    _EquipmentTypeService.Update(EquipmentType);


                }
            }

            return Json(new[] { EquipmentType }.ToDataSourceResult(request));
        }


        public ActionResult EquipmentType_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentType EquipmentType)
        {
            var fleet = _EquipmentTypeService.GetById(EquipmentType.EquipmentTypeID);
            if (fleet != null)
            {

                _EquipmentTypeService.Delete(fleet);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Type number " + EquipmentType.EquipmentTypeID);

            }

            return Json(new[] { EquipmentType }.ToDataSourceResult(request));
        }
       
    }
}