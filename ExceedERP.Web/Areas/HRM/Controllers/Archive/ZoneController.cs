﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.Core.Domain.HRM.ArchiveManagement;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
  
    [AuthorizeRoles(HrmConstants.HRArchiveAdmin)]
    public class ZoneController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Zone_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Zone> zones = db.Zone;
            DataSourceResult result = zones.ToDataSourceResult(request, zone => new {
                ZoneId = zone.ZoneId,
                ZoneName = zone.ZoneName,
                Remark = zone.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Zone_Create([DataSourceRequest]DataSourceRequest request, Zone zone)
        {
            if (ModelState.IsValid)
            {
                var checkExstance = db.Zone.ToList();
                var jobName = zone.ZoneName;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.ZoneName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This location name already exists in the database!!");
                }
                else
                {
                    var entity = new Zone
                    {
                        ZoneName = zone.ZoneName,
                        Remark = zone.Remark
                    };

                    db.Zone.Add(entity);
                    db.SaveChanges();
                    zone.ZoneId = entity.ZoneId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                  
                        UserHelper.OperationLog(Request.UserHostAddress, "Zone", "Success", User.Identity.Name, User.Identity.Name + " add new Zone called " + entity.ZoneName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                    #endregion
                }
            }

            return Json(new[] { zone }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Zone_Update([DataSourceRequest]DataSourceRequest request, Zone zone)
        {
            if (ModelState.IsValid)
            {
                var entity = new Zone
                {
                    ZoneId = zone.ZoneId,
                    ZoneName = zone.ZoneName,
                    Remark = zone.Remark
                };

                db.Zone.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
              
                    UserHelper.OperationLog(Request.UserHostAddress, "Zone", "Success", User.Identity.Name, User.Identity.Name + " updated Zone info of " + entity.ZoneName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { zone }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Zone_Destroy([DataSourceRequest]DataSourceRequest request, Zone zone)
        {
            if (ModelState.IsValid)
            {
                var entity = new Zone
                {
                    ZoneId = zone.ZoneId,
                    ZoneName = zone.ZoneName,
                    Remark = zone.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted zone information of zone name" + zone.ZoneName );
                #endregion
                db.Zone.Attach(entity);
                db.Zone.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { zone }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
