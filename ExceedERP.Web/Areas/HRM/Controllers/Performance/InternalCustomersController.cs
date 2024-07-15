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
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead, 
        HrmConstants.HRPerformanceUser, 
        HrmConstants.HRPerformanceAdmin, 
        HrmConstants.HREmployeeSelfService)]

    public class InternalCustomersController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult InternalCustomers_Read([DataSourceRequest]DataSourceRequest request, int iCId)
        {
            IQueryable<InternalCustomer> internalCustomers = db.InternalCustomers.Where(x=>x.InternalCustomerPerformanceEvaluationId == iCId);
            DataSourceResult result = internalCustomers.ToDataSourceResult(request, internalCustomer => new {

                InternalCustomerId = internalCustomer.InternalCustomerId,
                EmployeeId = internalCustomer.EmployeeId,
                TobeEvalutedEmployeeId = internalCustomer.TobeEvalutedEmployeeId,
                InternalCustomerPerformanceEvaluationId = internalCustomer.InternalCustomerPerformanceEvaluationId


            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomers_Create([DataSourceRequest]DataSourceRequest request, InternalCustomer internalCustomer,int iCId)
        {
            var nuOfEvaluater = db.InternalCustomerPerformanceEvaluations.Find(iCId);
            var employee = db.InternalCustomers.Where(x => x.InternalCustomerPerformanceEvaluationId == iCId).ToList();
            if (nuOfEvaluater != null&& employee.Count() >= nuOfEvaluater.NumberOfEmployee)
            {
                ModelState.AddModelError("", $"The Resquested Evaluators can't exceed {nuOfEvaluater.NumberOfEmployee}!!");

            
            }

            if (employee.Where(x => x.EmployeeId == internalCustomer.EmployeeId&&x.TobeEvalutedEmployeeId == internalCustomer.TobeEvalutedEmployeeId&& x.InternalCustomerPerformanceEvaluationId == iCId).Any())
            {
                ModelState.AddModelError("", $"The Employee already assigned select another!!");

            }
            if (ModelState.IsValid)
            {
                internalCustomer.InternalCustomerPerformanceEvaluationId = iCId;
                var entity = GetMappedObject(internalCustomer);
                db.InternalCustomers.Add(entity);
                db.SaveChanges();
                internalCustomer.InternalCustomerId = entity.InternalCustomerId;
            }

            return Json(new[] { internalCustomer }.ToDataSourceResult(request, ModelState),JsonRequestBehavior.AllowGet);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomers_Update([DataSourceRequest]DataSourceRequest request, InternalCustomer internalCustomer)
        {
            if (db.InternalCustomers.Where(x => x.EmployeeId == internalCustomer.EmployeeId && x.TobeEvalutedEmployeeId == internalCustomer.TobeEvalutedEmployeeId && x.InternalCustomerPerformanceEvaluationId == internalCustomer.InternalCustomerPerformanceEvaluationId).Any())
            {
                ModelState.AddModelError("", $"The Employee already assigned select another!!");

            }
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(internalCustomer);
                db.InternalCustomers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { internalCustomer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomers_Destroy([DataSourceRequest]DataSourceRequest request, InternalCustomer internalCustomer)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(internalCustomer);
                db.InternalCustomers.Attach(entity);
                db.InternalCustomers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { internalCustomer }.ToDataSourceResult(request, ModelState));
        }
        private InternalCustomer GetMappedObject(InternalCustomer internalCustomer)
        {
            var entity = new InternalCustomer
            {
                InternalCustomerId = internalCustomer.InternalCustomerId,
                EmployeeId = internalCustomer.EmployeeId,
                TobeEvalutedEmployeeId = internalCustomer.TobeEvalutedEmployeeId,
                InternalCustomerPerformanceEvaluationId = internalCustomer.InternalCustomerPerformanceEvaluationId

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
