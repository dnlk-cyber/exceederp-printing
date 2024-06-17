﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Planning.BusinessPlanDevelopment;

namespace ExceedERP.Web.Areas.Planning.Controllers.BusinessPlanDevelopment
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_Risk_User)]
    public class LandManagementsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            

            return View();
        }

        public ActionResult LandManagements_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LandManagement> landManagements = db.LandManagements;
            DataSourceResult result = landManagements.ToDataSourceResult(request, landManagement => new {
                LandManagementId = landManagement.LandManagementId,
                ParticularNameLandPilot = landManagement.ParticularNameLandPilot,
                Zone = landManagement.Zone,
                CityOrTown = landManagement.CityOrTown,
                Area = landManagement.Area,
                TypesOfService = landManagement.TypesOfService,
                LandHoldingCertifcateNo = landManagement.LandHoldingCertifcateNo,
                AcquiredBy = landManagement.AcquiredBy,
                DateCreated = landManagement.DateCreated,
                CreatedBy = landManagement.CreatedBy,
                LastModified = landManagement.LastModified,
                ModifiedBy = landManagement.ModifiedBy,
            });

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LandManagements_Create([DataSourceRequest]DataSourceRequest request, LandManagement landManagement)
        {
            
            if (ModelState.IsValid)
            {
                landManagement.DateCreated = DateTime.Now.Date;
                landManagement.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(landManagement);
                db.LandManagements.Add(entity);
                db.SaveChanges();
                landManagement.LandManagementId = entity.LandManagementId;

                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "land Management", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                

                #endregion
            }
          

            return Json(new[] { landManagement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LandManagements_Update([DataSourceRequest]DataSourceRequest request, LandManagement landManagement)
        {
            
            if (ModelState.IsValid)
            {
                landManagement.LastModified = DateTime.Now.Date;
                landManagement.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(landManagement);
                db.LandManagements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "landManagement", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { landManagement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LandManagements_Destroy([DataSourceRequest]DataSourceRequest request, LandManagement landManagement)
        {
            
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(landManagement);
                #region delete log
               
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "    landManagement " + landManagement.ParticularNameLandPilot);
                #endregion
                db.LandManagements.Attach(entity);
                db.LandManagements.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { landManagement }.ToDataSourceResult(request, ModelState));
        }
        private LandManagement GetMappedObject(LandManagement landManagement)
        {
            var entity = new LandManagement
            {
                LandManagementId = landManagement.LandManagementId,
                ParticularNameLandPilot = landManagement.ParticularNameLandPilot,
                Zone = landManagement.Zone,
                CityOrTown = landManagement.CityOrTown,
                Area = landManagement.Area,
                TypesOfService = landManagement.TypesOfService,
                LandHoldingCertifcateNo = landManagement.LandHoldingCertifcateNo,
                AcquiredBy = landManagement.AcquiredBy,
                DateCreated = landManagement.DateCreated,
                CreatedBy = landManagement.CreatedBy,
                LastModified = landManagement.LastModified,
                ModifiedBy = landManagement.ModifiedBy,

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
