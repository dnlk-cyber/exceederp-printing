using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    //[AuthorizeRoles(MaintenanceRoles.MaintenanceAdmin)]  
    public class RepairTypeController : Controller
    {
        IRepairTypeService _RepairTypeService;
        private ExceedDbContext _db = new ExceedDbContext();

        public RepairTypeController()
        {
            _RepairTypeService = new RepairTypeService();
        }
        
        public ActionResult Index()
        {
            ViewData["Categories"] = _db.ItemCategorys.Select(s => new
            {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
            return View();
        }

        public JsonResult RepairType_Read([DataSourceRequest] DataSourceRequest request)
        {
            _RepairTypeService = new RepairTypeService();
            var oper = _RepairTypeService.GetAll();
            var Opers = oper.Select(o => new RepairType
            {
                RepairTypeID = o.RepairTypeID,
                Name = o.Name,
                Weight = o.Weight,
                Code = o.Code,
                CategoryCode = o.CategoryCode,
                ServiceGroup = o.ServiceGroup,
                SubCategoryCode = o.SubCategoryCode,
                StandaredTime = o.StandaredTime,
                AutomotiveStandaredHour = o.AutomotiveStandaredHour,
                PayableTime = o.PayableTime,
                Difference = o.Difference,
                LabourRateAtCompany = o.LabourRateAtCompany,
                LabourRateAtCustomer = o.LabourRateAtCustomer,
                TotalAmountCompany = o.TotalAmountCompany,
                TotalAmountCustomer = o.TotalAmountCustomer,
                Remark = o.Remark,
                Place=o.Place,
                Reason=o.Reason,
                Time=o.Time,
                Process=o.Process
            });

            return this.Json(Opers.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RepairType_Create([DataSourceRequest] DataSourceRequest request, RepairType RepairType)
        {
            ModelState.Remove("RepairTypeID");
            List<RepairType> RepairTypes = _RepairTypeService.GetAll().Where(f => f.Name == RepairType.Name).ToList();
            if (RepairTypes.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Repair Type with Name Already Exists ");
            }
            List<RepairType> RepairTypeCodes = _RepairTypeService.GetAll().Where(f => f.Code == RepairType.Code).ToList();
            if (RepairTypeCodes.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Repair Type with Code Already Exists ");
            }
            else
            {
                RepairType.RepairTypeID = Guid.NewGuid();

                if (RepairType != null)
                {
                    _RepairTypeService.Insert(RepairType);
                }
            }
            return this.Json(new[] { RepairType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RepairType_Update([DataSourceRequest] DataSourceRequest request, RepairType RepairType)
        {
            ModelState.Remove("RepairTypeID");

            List<RepairType> RepairTypes = _RepairTypeService.GetAll().Where(f => f.Name == RepairType.Name && f.RepairTypeID != RepairType.RepairTypeID).ToList();
            if (RepairTypes.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Repair Type with Name Already Exists ");
            }
            List<RepairType> RepairTypeCodes = _RepairTypeService.GetAll().Where(f => f.Code == RepairType.Code && f.RepairTypeID != RepairType.RepairTypeID).ToList();
            if (RepairTypeCodes.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Repair Type with Code Already Exists ");

            }
            if (RepairType != null)
            {
                RepairType.ModifiedBy = User.Identity.Name;
                RepairType.LastModified = DateTime.Now;

                _db.RepairType.Attach(RepairType);
                _db.Entry(RepairType).State = EntityState.Modified;
                _db.SaveChanges();
            }


            return this.Json(new[] { RepairType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RepairType_Destroy([DataSourceRequest] DataSourceRequest request, RepairType RepairType)
        {
            var RepairTypes = _RepairTypeService.GetById(RepairType.RepairTypeID);
            if (RepairTypes != null)
            {
                _RepairTypeService.Delete(RepairTypes);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Repair Type number " + RepairType.RepairTypeID);

            }

            return this.Json(new[] { RepairType }.ToDataSourceResult(request));
        }
        
        public ActionResult RepairTypeDDBind()
        {

            var fleets = _RepairTypeService.GetAll().Select(s => new
            {
                Name = s.Code + " - " + s.Name,
                Value = s.RepairTypeID
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }


    }
}