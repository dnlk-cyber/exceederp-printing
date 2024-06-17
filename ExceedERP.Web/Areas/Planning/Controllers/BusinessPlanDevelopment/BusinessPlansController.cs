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
    public class BusinessPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            

            return View();
        }

        public ActionResult BusinessPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BusinessPlan> businessPlans = db.BusinessPlans;
            DataSourceResult result = businessPlans.ToDataSourceResult(request, businessPlan => new {
                BusinessPlanId = businessPlan.BusinessPlanId,
                NameOfBusinessPlan = businessPlan.NameOfBusinessPlan,
                ProductType = businessPlan.ProductType,
                StudyMethod = businessPlan.StudyMethod,
                Status = businessPlan.Status,
                DateCreated = businessPlan.DateCreated,
                CreatedBy = businessPlan.CreatedBy,
                LastModified = businessPlan.LastModified,
                ModifiedBy = businessPlan.ModifiedBy,
            });

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessPlans_Create([DataSourceRequest]DataSourceRequest request, BusinessPlan businessPlan)
        {
            
            if (ModelState.IsValid)
            {
                businessPlan.DateCreated = DateTime.Now.Date;
                businessPlan.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(businessPlan);
                db.BusinessPlans.Add(entity);
                db.SaveChanges();
                businessPlan.BusinessPlanId = entity.BusinessPlanId;

                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "Bussiness Plan", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                

                #endregion
            }
          

            return Json(new[] { businessPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessPlans_Update([DataSourceRequest]DataSourceRequest request, BusinessPlan businessPlan)
        {
            
            if (ModelState.IsValid)
            {
                businessPlan.LastModified = DateTime.Now.Date;
                businessPlan.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(businessPlan);
                db.BusinessPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bussiness Plan", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { businessPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessPlans_Destroy([DataSourceRequest]DataSourceRequest request, BusinessPlan businessPlan)
        {
            
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(businessPlan);
                #region delete log
               
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "    Bussinss Plan " + businessPlan.NameOfBusinessPlan);
                #endregion
                db.BusinessPlans.Attach(entity);
                db.BusinessPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { businessPlan }.ToDataSourceResult(request, ModelState));
        }
        private BusinessPlan GetMappedObject(BusinessPlan businessPlan)
        {
            var entity = new BusinessPlan
            {
                BusinessPlanId = businessPlan.BusinessPlanId,
                NameOfBusinessPlan = businessPlan.NameOfBusinessPlan,
                ProductType = businessPlan.ProductType,
                StudyMethod = businessPlan.StudyMethod,
                Status = businessPlan.Status,
                DateCreated = businessPlan.DateCreated,
                CreatedBy = businessPlan.CreatedBy,
                LastModified = businessPlan.LastModified,
                ModifiedBy = businessPlan.ModifiedBy,

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
