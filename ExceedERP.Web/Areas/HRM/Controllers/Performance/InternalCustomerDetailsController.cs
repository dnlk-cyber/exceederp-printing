﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, 
                    HrmConstants.HRPerformanceUser,
                    HrmConstants.HRPerformanceReport)]

    public class InternalCustomerDetailsController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult InternalCustomerDetails_Read([DataSourceRequest]DataSourceRequest request, int intenalCusId)
        {
            IQueryable<InternalCustomerDetail> internalCustomers = db.InternalCustomerDetails.Where(x=>x.InternalCustomerId == intenalCusId);
            DataSourceResult result = internalCustomers.ToDataSourceResult(request, internalCustomer => new {

                InternalCustomerDetailId = internalCustomer.InternalCustomerDetailId,
                InternalCustomerId = internalCustomer.InternalCustomerId,
                TobeEvalutedEmployeeId = internalCustomer.TobeEvalutedEmployeeId,


            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerDetails_Create([DataSourceRequest]DataSourceRequest request, InternalCustomerDetail internalCustomer,int intenalCusId)
        {
            //var nuOfEvaluater = db.InternalCustomerPerformanceEvaluations.Find(iCId);
            //var employee = db.InternalCustomerDetails.Where(x => x.InternalCustomerPerformanceEvaluationId == iCId).ToList();
            //if (nuOfEvaluater != null&& employee.Count() >= nuOfEvaluater.NumberOfEmployee)
            //{
            //    ModelState.AddModelError("", $"The Resquested Evaluators can't exceed {nuOfEvaluater.NumberOfEmployee}!!");

            
            //}

            if (db.InternalCustomerDetails.Where(x => x.TobeEvalutedEmployeeId == internalCustomer.TobeEvalutedEmployeeId&& x.InternalCustomerId == intenalCusId).Any())
            {
                ModelState.AddModelError("", $"The Employee already registered !!");

            }
            if (ModelState.IsValid)
            {
                internalCustomer.InternalCustomerId = intenalCusId;
                var entity = GetMappedObject(internalCustomer);
                db.InternalCustomerDetails.Add(entity);
                db.SaveChanges();
                internalCustomer.InternalCustomerDetailId = entity.InternalCustomerDetailId;
            }

            return Json(new[] { internalCustomer }.ToDataSourceResult(request, ModelState),JsonRequestBehavior.AllowGet);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerDetails_Update([DataSourceRequest]DataSourceRequest request, InternalCustomerDetail internalCustomer)
        {
            if (db.InternalCustomerDetails.Where(x =>  x.TobeEvalutedEmployeeId == internalCustomer.TobeEvalutedEmployeeId && x.InternalCustomerId == internalCustomer.InternalCustomerId &&x.InternalCustomerDetailId !=internalCustomer.InternalCustomerDetailId).Any())
            {
                ModelState.AddModelError("", $"The Employee already registered select another!!");

            }
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(internalCustomer);
                db.InternalCustomerDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { internalCustomer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerDetails_Destroy([DataSourceRequest]DataSourceRequest request, InternalCustomerDetail internalCustomer)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(internalCustomer);
                db.InternalCustomerDetails.Attach(entity);
                db.InternalCustomerDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { internalCustomer }.ToDataSourceResult(request, ModelState));
        }
        private InternalCustomerDetail GetMappedObject(InternalCustomerDetail internalCustomer)
        {
            var entity = new InternalCustomerDetail
            {
                InternalCustomerDetailId = internalCustomer.InternalCustomerDetailId,
                InternalCustomerId = internalCustomer.InternalCustomerId,
                TobeEvalutedEmployeeId = internalCustomer.TobeEvalutedEmployeeId,

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
