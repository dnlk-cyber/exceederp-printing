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
using ExceedERP.Core.Domain.Common.HRM;
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
    public class PeerToPeerPerformanceEvaluationsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult PeerToPeerPerformanceEvaluations_Read([DataSourceRequest]DataSourceRequest request,int pPId)
        {
            IQueryable<PeerToPeerPerformanceEvaluation> peerToPeerPerformances = db.PeerToPeerPerformanceEvaluations.Where(x=>x.PerformanceRequestPeriodId ==pPId);
            DataSourceResult result = peerToPeerPerformances.ToDataSourceResult(request, peerToPeerPerformance => new {
                PeerToPeerPerformanceEvaluationId = peerToPeerPerformance.PeerToPeerPerformanceEvaluationId,
                PerformanceRequestPeriodId = peerToPeerPerformance.PerformanceRequestPeriodId,
                EvaluatorId = peerToPeerPerformance.EvaluatorId,
                IsAsignmentApproved = peerToPeerPerformance.IsAsignmentApproved,
                EmployeeId = peerToPeerPerformance.EmployeeId

            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PeerToPeerPerformanceEvaluations_Create([DataSourceRequest]DataSourceRequest request, PeerToPeerPerformanceEvaluation peerToPeerPerformance, int pPId)
        {
            if (peerToPeerPerformance.EmployeeId == peerToPeerPerformance.EvaluatorId)
            {
                ModelState.AddModelError("exists", "the evaluator and to be Evaluated can't be the some");

            }
            var employees = db.PeerToPeerPerformanceEvaluations.Where(x => (x.EmployeeId == peerToPeerPerformance.EmployeeId && x.EvaluatorId == peerToPeerPerformance.EvaluatorId)&&x.PerformanceRequestPeriodId==pPId).ToList();
            if (employees.Any())
            {
                ModelState.AddModelError("exists", "the employees are registered Already");
            }
            if (ModelState.IsValid)
            {
                peerToPeerPerformance.PerformanceRequestPeriodId = pPId;
                var entity = GetMappedObject(peerToPeerPerformance);
                db.PeerToPeerPerformanceEvaluations.Add(entity);
                db.SaveChanges();
                peerToPeerPerformance.PeerToPeerPerformanceEvaluationId = entity.PeerToPeerPerformanceEvaluationId;
            }

            return Json(new[] { peerToPeerPerformance }.ToDataSourceResult(request, ModelState),JsonRequestBehavior.AllowGet);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PeerToPeerPerformanceEvaluations_Update([DataSourceRequest]DataSourceRequest request, PeerToPeerPerformanceEvaluation peerToPeerPerformance)
        {
            if (peerToPeerPerformance.EmployeeId == peerToPeerPerformance.EvaluatorId)
            {
                ModelState.AddModelError("exists", "the evaluator and to be Evaluated can't be the same");

            }
            var employees = db.PeerToPeerPerformanceEvaluations.Where(x => (x.EmployeeId == peerToPeerPerformance.EmployeeId&& x.EvaluatorId == peerToPeerPerformance.EvaluatorId)
                                                                       && x.PerformanceRequestPeriodId == peerToPeerPerformance.PerformanceRequestPeriodId
                                                                       &&x.PeerToPeerPerformanceEvaluationId !=peerToPeerPerformance.PeerToPeerPerformanceEvaluationId).ToList();
            if (employees.Any())
            {
                ModelState.AddModelError("exists", "the employees are registered Already");
            }
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(peerToPeerPerformance);
                db.PeerToPeerPerformanceEvaluations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { peerToPeerPerformance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PeerToPeerPerformanceEvaluations_Destroy([DataSourceRequest]DataSourceRequest request, PeerToPeerPerformanceEvaluation peerToPeerPerformance)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(peerToPeerPerformance);
                db.PeerToPeerPerformanceEvaluations.Attach(entity);
                db.PeerToPeerPerformanceEvaluations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { peerToPeerPerformance }.ToDataSourceResult(request, ModelState));
        }
        private PeerToPeerPerformanceEvaluation GetMappedObject(PeerToPeerPerformanceEvaluation peerToPeerPerformance)
        {
            var entity = new PeerToPeerPerformanceEvaluation
            {
                PeerToPeerPerformanceEvaluationId = peerToPeerPerformance.PeerToPeerPerformanceEvaluationId,
                PerformanceRequestPeriodId = peerToPeerPerformance.PerformanceRequestPeriodId,
                EvaluatorId = peerToPeerPerformance.EvaluatorId,
                IsAsignmentApproved = peerToPeerPerformance.IsAsignmentApproved,
                EmployeeId = peerToPeerPerformance.EmployeeId
               
            };
            return entity;
        }
        public JsonResult PeerToPeerEvaluatorConfirmation(int id)
        {
            // Find the evaluator with the provided ID or return an error if not found
            var evaluator = db.PeerToPeerPerformanceEvaluations.Find(id);

            if (evaluator == null)
            {
                return Json(new { Message = "The evaluation record with the specified ID was not found. Please check the ID and try again.", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }

            // Check if the assignment is already approved
            if (evaluator.IsAsignmentApproved)
            {
                return Json(new { Message = "This evaluation has already been approved. You cannot approve it again.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Approve the assignment and save the changes
                evaluator.IsAsignmentApproved = true;
                db.Entry(evaluator).State = EntityState.Modified;
                db.SaveChanges();

                // Prepare and send the notification
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(evaluator.EvaluatorId),
                    Title = "New Evaluation Assigned",
                    Message = $"You have been assigned to evaluate the performance of {UserInformationHelper.GetEmployeeFullNameById(evaluator.EmployeeId)}. Please complete the evaluation within the specified timeframe.",
                    URL = "/HRM/PeerToPeerEmployeePerformance"
                };

                Notify.NotifyUser(notif);

                return Json(new { Message = "Evaluation successfully approved and notification sent to the evaluator.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // General error handling
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
