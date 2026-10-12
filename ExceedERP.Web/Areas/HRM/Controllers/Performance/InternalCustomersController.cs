﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Hubs;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead, 
        HrmConstants.HRPerformanceUser, 
        HrmConstants.HRPerformanceAdmin, 
        HrmConstants.HRDepartmentHead, 
        HrmConstants.HREmployeeSelfService)]

    public class InternalCustomersController : BaseController
    {

        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult InternalCustomers_Read([DataSourceRequest]DataSourceRequest request, int iCId)
        {
            IQueryable<InternalCustomer> internalCustomers = db.InternalCustomers.Where(x=>x.InternalCustomerPerformanceEvaluationId == iCId);
            DataSourceResult result = internalCustomers.ToDataSourceResult(request, internalCustomer => new {

                InternalCustomerId = internalCustomer.InternalCustomerId,
                EmployeeId = internalCustomer.EmployeeId,
                IsConfirmed = internalCustomer.IsConfirmed,
                TobeEvalutedEmployeeId = internalCustomer.TobeEvalutedEmployeeId,
                Remark = internalCustomer.Remark,
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
                return Json(new[] { internalCustomer }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);

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
                return Json(new[] { internalCustomer }.ToDataSourceResult(request, ModelState));

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
                IsConfirmed = internalCustomer.IsConfirmed,
                Remark = internalCustomer.Remark,
                InternalCustomerPerformanceEvaluationId = internalCustomer.InternalCustomerPerformanceEvaluationId

            };
            return entity;
        }
        public JsonResult InternalCustomerConfirmation(int id)
        {
            // Find the evaluator with the provided ID
            var evaluator = db.InternalCustomers.Find(id);

            if (evaluator == null)
            {
                // Return error if no evaluation record found
                return Json(new { Message = "The internal customer record with the provided ID was not found. Please check the ID and try again.", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }

            // Check if the assignment has already been confirmed
            if (evaluator.IsConfirmed)
            {
                // If already confirmed, return message indicating that no further approval is needed
                return Json(new { Message = "This evaluation has already been confirmed. No further approval is required.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Approve the assignment and save the changes
                evaluator.IsConfirmed = true;
                db.Entry(evaluator).State = EntityState.Modified;
                db.SaveChanges();
                // Fetch the employees who need to be evaluated
                var employeesToBeEvaluated = db.InternalCustomerDetails
                    .Where(x => x.InternalCustomerId == evaluator.InternalCustomerId);

                foreach (var employee in employeesToBeEvaluated)
                {
                    // Prepare and send notifications for each employee to be evaluated
                    var notif = new NotifModels
                    {
                        UserId = UserInformationHelper.GetEmployeeUserId(evaluator.EmployeeId),
                        Title = "New Evaluation Assigned",
                        Message = $"You have been assigned to evaluate the performance of {UserInformationHelper.GetEmployeeFullNameById(employee.TobeEvalutedEmployeeId)}. Please complete the evaluation within the specified timeframe.",
                        URL = "/HRM/InternalCustomerEmployeePerformance"
                    };

                    Notify.NotifyUser(notif);
                }

                // Return success message after approval and notification
                return Json(new { Message = "Evaluation Assignation is  successfully approved and notifications sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Return a detailed error message if something goes wrong
                return Json(new { Message = $"An error occurred while processing the request: {ex.Message}. Please try again later or contact support if the issue persists.", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
