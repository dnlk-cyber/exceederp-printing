using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class EquipmentConditionController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public EquipmentConditionController()
        {
        }
        //
        // GET: /FleetManagement/FleetCondition/
        public ActionResult Index()
        {

            return View();
        }


        public JsonResult FleetCondition_Read([DataSourceRequest] DataSourceRequest request)
        {

            var oper = db.FleetConditions.ToList();
            var Opers = oper.Select(o => new FleetCondition
            {
                FleetConditionId = o.FleetConditionId,
                Name = o.Name,
                Description = o.Description


            });

            return this.Json(Opers.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult FleetCondition_Create([DataSourceRequest] DataSourceRequest request, FleetCondition FleetCondition)
        {
            ModelState.Remove("FleetConditionId");
            List<FleetCondition> FleetConditions = db.FleetConditions.Where(f => f.Name == FleetCondition.Name).ToList();
            if (FleetConditions.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " License Level Already Exists ");
                return this.Json(new[] { FleetCondition }.ToDataSourceResult(request, ModelState));
            }

            if (FleetCondition != null)
            {

                FleetCondition.CreatedBy = User.Identity.Name;
                FleetCondition.DateCreated = DateTime.Now;


                db.FleetConditions.Add(FleetCondition);
                db.SaveChanges();
            }

            return this.Json(new[] { FleetCondition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult FleetCondition_Update([DataSourceRequest] DataSourceRequest request, FleetCondition FleetCondition)
        {

            List<FleetCondition> FleetConditions = db.FleetConditions.Where(f => f.Name == FleetCondition.Name && f.FleetConditionId != FleetCondition.FleetConditionId).ToList();
            if (FleetConditions.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Condition Already Exists ");
                return this.Json(new[] { FleetCondition }.ToDataSourceResult(request, ModelState));
            }

            if (FleetCondition != null)
            {
                FleetCondition.ModifiedBy = User.Identity.Name;
                FleetCondition.LastModified = DateTime.Now;


                db.FleetConditions.Attach(FleetCondition);
                db.Entry(FleetCondition).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
         
            }


            return this.Json(new[] { FleetCondition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult FleetCondition_Destroy([DataSourceRequest] DataSourceRequest request, FleetCondition FleetCondition)
        {
            var FleetConditions = db.FleetConditions.Where(x=>x.FleetConditionId == FleetCondition.FleetConditionId).FirstOrDefault();
            if (FleetConditions != null)
            {
                db.FleetConditions.Attach(FleetConditions);
                db.FleetConditions.Remove(FleetConditions);
                db.SaveChanges();
            }

            return this.Json(new[] { FleetCondition }.ToDataSourceResult(request));
        }
        [AllowAnonymous]
        public ActionResult FleetConditionDDBind()
        {

            var fleets = db.FleetConditions.Select(s => new  
            {
                Name = s.Name,
                FleetConditionId = s.FleetConditionId
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }


    }
}