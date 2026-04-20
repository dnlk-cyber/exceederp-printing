﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    //[AuthorizeRoles(MaintenanceRoles.MaintenanceAdmin)]
    public class ServiceGroupSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SGSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ServiceGroupSetting> SGSetting = db.ServiceGroupSettings;
                DataSourceResult Result = SGSetting.ToDataSourceResult(request, sgSetting => new {
                    ServiceGroupSettingId = sgSetting.ServiceGroupSettingId,
                    ServiceGroupName = sgSetting.ServiceGroupName,
                Description = sgSetting.Description,
                DateCreated = sgSetting.DateCreated,
                LastModified = sgSetting.LastModified,
                CreatedBy = sgSetting.CreatedBy,
                ModifiedBy = sgSetting.ModifiedBy
            });

            return Json(Result,JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SGSetting_Create([DataSourceRequest]DataSourceRequest request, ServiceGroupSetting sgSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new ServiceGroupSetting
                {
                    ServiceGroupSettingId = sgSetting.ServiceGroupSettingId,
                    ServiceGroupName = sgSetting.ServiceGroupName,
                    Description = sgSetting.Description,
                    DateCreated =DateTime.Now,
                    CreatedBy = User.Identity.Name
                 };

                db.ServiceGroupSettings.Add(entity);
                db.SaveChanges();
                sgSetting.ServiceGroupSettingId = entity.ServiceGroupSettingId;
            }

            return Json(new[] { sgSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SGSetting_Update([DataSourceRequest]DataSourceRequest request, ServiceGroupSetting sgSetting)
        {
            var sgSettings = db.ServiceGroupSettings.FirstOrDefault(x => x.ServiceGroupSettingId == sgSetting.ServiceGroupSettingId);
            if (sgSetting == null)
                ModelState.AddModelError("Record Not Found", "Record Not Found");
            if (ModelState.IsValid)
            {


                sgSettings.ServiceGroupName = sgSetting.ServiceGroupName;
                sgSettings.Description = sgSetting.Description;
                sgSettings.DateCreated = sgSetting.DateCreated;
                sgSettings.CreatedBy = sgSetting.CreatedBy;
                sgSettings.LastModified = DateTime.Now;
                sgSettings. ModifiedBy = User.Identity.Name;
                

                
                db.Entry(sgSettings).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { sgSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SGSetting_Destroy([DataSourceRequest]DataSourceRequest request, ServiceGroupSetting sgSetting)
        {
            var sgsetting = db.ServiceGroupSettings.FirstOrDefault(x => x.ServiceGroupSettingId == sgSetting.ServiceGroupSettingId);
            if(sgsetting == null)
            {
                ModelState.AddModelError("Record Not Found", "Record Not Found");
            }
            if (ModelState.IsValid)
            {
                
                db.ServiceGroupSettings.Remove(sgsetting);
                db.SaveChanges();
            }

            return Json(new[] { sgSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
