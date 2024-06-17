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
    public class BusinessPlanValidationsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            

            return View();
        }

        public ActionResult BusinessPlanValidations_Read([DataSourceRequest]DataSourceRequest request,int bPId)
        {
            IQueryable<BusinessPlanValidation> businessPlans = db.BusinessPlanValidations.Where(x=>x.BusinessPlanId==bPId);
            DataSourceResult result = businessPlans.ToDataSourceResult(request, businessPlan => new {
                BusinessPlanValidationId = businessPlan.BusinessPlanValidationId,
                BusinessPlanId = businessPlan.BusinessPlanId,
                Date = businessPlan.Date,
                Status = businessPlan.Status,
                Description = businessPlan.Description,

            });

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessPlanValidations_Create([DataSourceRequest]DataSourceRequest request, BusinessPlanValidation businessPlan,int bPId)
        {
            
            if (ModelState.IsValid)
            {
                businessPlan.BusinessPlanId = bPId;
                var entity = GetMappedObject(businessPlan);
                db.BusinessPlanValidations.Add(entity);
                db.SaveChanges();
                businessPlan.BusinessPlanValidationId = entity.BusinessPlanValidationId;
                var master = db.BusinessPlans.Find(bPId);
                if(master != null)
                {
                    master.Status = businessPlan.Status;
                    db.Entry(master).State = EntityState.Modified;
                    db.SaveChanges();
                }
                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "Bussiness Plan", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                

                #endregion
            }
          

            return Json(new[] { businessPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessPlanValidations_Update([DataSourceRequest]DataSourceRequest request, BusinessPlanValidation businessPlan)
        {
            
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(businessPlan);
                db.BusinessPlanValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                var master = db.BusinessPlans.Find(businessPlan.BusinessPlanId);
                if (master != null)
                {
                    master.Status = businessPlan.Status;
                    db.Entry(master).State = EntityState.Modified;
                    db.SaveChanges();
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bussiness Plan", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { businessPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessPlanValidations_Destroy([DataSourceRequest]DataSourceRequest request, BusinessPlanValidation businessPlan)
        {
            
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(businessPlan);
                #region delete log
               
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "    Bussinss Plan " + businessPlan.BusinessPlanValidationId.ToString());
                #endregion
                db.BusinessPlanValidations.Attach(entity);
                db.BusinessPlanValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { businessPlan }.ToDataSourceResult(request, ModelState));
        }
        private BusinessPlanValidation GetMappedObject(BusinessPlanValidation businessPlan)
        {
            var entity = new BusinessPlanValidation
            {
                BusinessPlanValidationId = businessPlan.BusinessPlanValidationId,
                BusinessPlanId = businessPlan.BusinessPlanId,
                Status = businessPlan.Status,
                Date = businessPlan.Date,
                Description = businessPlan.Description,

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
