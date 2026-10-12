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
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead,
        HrmConstants.HRPerformanceUser, 
        HrmConstants.HRPerformanceAdmin, 
        HrmConstants.HRDepartmentHead, 
        HrmConstants.HREmployeeSelfService)]

    public class InternalCustomerPerformanceEvaluationsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult InternalCustomerResponses_Read([DataSourceRequest]DataSourceRequest request, int periodId)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var orgstructure = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            int currentUserDepId = orgstructure != null ? orgstructure.OrgStructureId : 0;
            IQueryable<InternalCustomerPerformanceEvaluation> internalcustomerperformanceevaluations = db.InternalCustomerPerformanceEvaluations.Where(x => x.PerformanceRequestPeriodId == periodId && x.OrgStructureId == currentUserDepId);
            DataSourceResult result = internalcustomerperformanceevaluations.ToDataSourceResult(request, internalCustomerPerformanceEvaluation => new {
                InternalCustomerPerformanceEvaluationId = internalCustomerPerformanceEvaluation.InternalCustomerPerformanceEvaluationId,
                NumberOfEmployee = internalCustomerPerformanceEvaluation.NumberOfEmployee,
                OrgStructureId = internalCustomerPerformanceEvaluation.OrgStructureId,
                PerformanceRequestPeriodId = internalCustomerPerformanceEvaluation.PerformanceRequestPeriodId,
                IsConfirmed = internalCustomerPerformanceEvaluation.IsConfirmed,
                Remark = internalCustomerPerformanceEvaluation.Remark,

            });

            return Json(result);
        }
        public ActionResult InternalCustomerPerformanceEvaluations_Read([DataSourceRequest]DataSourceRequest request, int pId)
        {
            IQueryable<InternalCustomerPerformanceEvaluation> internalCustomerPerformances = db.InternalCustomerPerformanceEvaluations.Where(x=>x.PerformanceRequestPeriodId == pId);
            DataSourceResult result = internalCustomerPerformances.ToDataSourceResult(request, internalCustomerPerformance => new {

                InternalCustomerPerformanceEvaluationId = internalCustomerPerformance.InternalCustomerPerformanceEvaluationId,
                PerformanceRequestPeriodId = internalCustomerPerformance.PerformanceRequestPeriodId,
                NumberOfEmployee = internalCustomerPerformance.NumberOfEmployee,
                OrgStructureId = internalCustomerPerformance.OrgStructureId,
                IsConfirmed = internalCustomerPerformance.IsConfirmed,
                Remark = internalCustomerPerformance.Remark,


            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerPerformanceEvaluations_Create([DataSourceRequest]DataSourceRequest request, InternalCustomerPerformanceEvaluation internalCustomerPerformance,int pId)
        {
            if (ModelState.IsValid)
            {
                internalCustomerPerformance.PerformanceRequestPeriodId = pId;
                var entity = GetMappedObject(internalCustomerPerformance);
                db.InternalCustomerPerformanceEvaluations.Add(entity);
                db.SaveChanges();
                internalCustomerPerformance.InternalCustomerPerformanceEvaluationId = entity.InternalCustomerPerformanceEvaluationId;
            }

            return Json(new[] { internalCustomerPerformance }.ToDataSourceResult(request, ModelState),JsonRequestBehavior.AllowGet);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerPerformanceEvaluations_Update([DataSourceRequest]DataSourceRequest request, InternalCustomerPerformanceEvaluation internalCustomerPerformance)
        {
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(internalCustomerPerformance);
                db.InternalCustomerPerformanceEvaluations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { internalCustomerPerformance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalCustomerPerformanceEvaluations_Destroy([DataSourceRequest]DataSourceRequest request, InternalCustomerPerformanceEvaluation internalCustomerPerformance)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(internalCustomerPerformance);
                db.InternalCustomerPerformanceEvaluations.Attach(entity);
                db.InternalCustomerPerformanceEvaluations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { internalCustomerPerformance }.ToDataSourceResult(request, ModelState));
        }
        private InternalCustomerPerformanceEvaluation GetMappedObject(InternalCustomerPerformanceEvaluation internalCustomerPerformance)
        {
            var entity = new InternalCustomerPerformanceEvaluation
            {
                InternalCustomerPerformanceEvaluationId = internalCustomerPerformance.InternalCustomerPerformanceEvaluationId,
                PerformanceRequestPeriodId = internalCustomerPerformance.PerformanceRequestPeriodId,
                NumberOfEmployee = internalCustomerPerformance.NumberOfEmployee,
                IsConfirmed = internalCustomerPerformance.IsConfirmed,
                OrgStructureId = internalCustomerPerformance.OrgStructureId,
                Remark = internalCustomerPerformance.Remark,

            };
            return entity;
        }
        public JsonResult InternalCustomerEvaluatorConfirmation(int id)
        {
            // Find the evaluator by ID or return error if not found
            var evaluator = db.InternalCustomerPerformanceEvaluations.Find(id);

            if (evaluator == null)
            {
                // Return error if no evaluation record found
                return Json(new { Message = "The internal customer evaluation with the provided ID could not be found. Please check the ID and try again.", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }

            // Check if the evaluation has already been confirmed
            if (evaluator.IsConfirmed)
            {
                // If already confirmed, notify the user that no further action is needed
                return Json(new { Message = "This evaluation has already been confirmed. No further approval is necessary.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Approve the evaluation and save changes
                evaluator.IsConfirmed = true;
                db.Entry(evaluator).State = EntityState.Modified;
                db.SaveChanges();
                var head = db.Placements
                   .FirstOrDefault(x => x.OrgStructureId == evaluator.OrgStructureId
                   && x.isCurrent
                   && x.IsHead);
                // Prepare and send the notification
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(head?.HeadId??0), // Send notification to the evaluator
                    Title = "Evaluation Approval Notification",
                    Message = $"The evaluation for internal customer {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)} has been successfully approved. You may now proceed with the next steps.",
                    URL = "/HRM/AssignEvaluators" // You can customize the URL
                };

                Notify.NotifyUser(notif);

                // Return success message after successful approval
                return Json(new { Message = "Evaluation successfully approved and notification sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Return a detailed error message if something goes wrong
                return Json(new { Message = $"An error occurred while processing the request: {ex.Message}. Please try again later or contact support if the issue persists.", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult InternalCustomerEvaluatorConfirmationCCC(int id)
        {
            var evaluator = db.InternalCustomerPerformanceEvaluations.Find(id);
            if (evaluator != null)
            {
                if (evaluator.IsConfirmed)
                {
                    var formattedData = new
                    {
                        Message = "You can't approve again, it is already approved.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Approve the assignment here
                    evaluator.IsConfirmed = true;
                    db.Entry(evaluator).State = EntityState.Modified;
                    db.SaveChanges();  // Make sure to save the changes in the database

                    var formattedData = new
                    {
                        Message = "You successfully approved .",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var formattedData = new
                {
                    Message = "No evaluation found for the provided ID.",
                    Status = "Error"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
