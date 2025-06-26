using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Models;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.InfoDesk.ChangeManagement;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class ChangeRequestsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
        public ActionResult Index()
        {
            PopulateData();
          
            return View();
        }

        private void PopulateData()
        {
                var employees = db.Person_Employee
                                    .Select(x =>new
                                    {
                                                x.EmployeeId,
                                                RequestorId = x.EmployeeId,
                                                ApproverId = x.EmployeeId,
                                                FullNameaEnglish = x.EmployeeCode + "-"+x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish,
                                                FullNameAmharic = x.FirstName + " " + x.MiddleName + " " + x.LastName                                               
                                    }).ToList();
            ViewData["Employees"] = employees;

            ViewData["orgStructure"] = db.MainCompanyStructures.ToList();

        }
       
         
       
        public ActionResult ChangeRequests_Read([DataSourceRequest]DataSourceRequest request)
        {
           
            var changeRequests = db.ChangeRequests.Where(x=>x.IsSendForApproval).ToList();
            DataSourceResult result = changeRequests.ToDataSourceResult(request, changeRequest => new ChangeRequest
            {
                ChangeRequestId = changeRequest.ChangeRequestId,
                ChangeType = changeRequest.ChangeType,
                ChangeTypeDetail = changeRequest.ChangeTypeDetail,
                Title = changeRequest.Title,
                RequestorId = changeRequest.RequestorId,
                ApproverId = changeRequest.ApproverId,
                Description = changeRequest.Description,
                ImpactAnalysis = changeRequest.ImpactAnalysis,
                ImplementationTeam = changeRequest.ImplementationTeam,
                PlannedEndDate = changeRequest.PlannedEndDate,
                PlannedStartDate = changeRequest.PlannedStartDate,
                RollbackPlan = changeRequest.RollbackPlan,
                RequestDate = changeRequest.RequestDate,
                RiskAssessment = changeRequest.RiskAssessment,
                Status = changeRequest.Status,
                ChangeStatus = changeRequest.ChangeStatus,
                Remark = changeRequest.Remark,
                CreatedBy = changeRequest.CreatedBy,
                DateCreated = changeRequest.DateCreated,
                IsOnlineApproved = changeRequest.IsOnlineApproved,
                IsOnlineTransferred = changeRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = changeRequest.IsOnlineTransferredBy,
                IsSendForApproval = changeRequest.IsSendForApproval,
                IsVoid = changeRequest.IsVoid,
                LastModified = changeRequest.LastModified,
                ModifiedBy = changeRequest.ModifiedBy,
                OnlineApprovedBy = changeRequest.OnlineApprovedBy,
                OnlineApprovedTime = changeRequest.OnlineApprovedTime,
                OnlineTransferredTime = changeRequest.OnlineTransferredTime,
                OrgBranchName = changeRequest.OrgBranchName,
                SendForApprovalBy = changeRequest.SendForApprovalBy,
                SendForApprovalTime = changeRequest.SendForApprovalTime,
                VoidBy = changeRequest.VoidBy,
                VoidTime = changeRequest.VoidTime,
               


            });

            return Json(result);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeRequests_Create([DataSourceRequest]DataSourceRequest request, ChangeRequest changeRequest)
        {
    
            changeRequest.DateCreated = DateTime.Now;
            changeRequest.CreatedBy = User.Identity.Name;
            changeRequest.SendForApprovalTime = DateTime.Now;
            changeRequest.SendForApprovalBy = User.Identity.Name;
            changeRequest.IsSendForApproval = true;
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(changeRequest);
                db.ChangeRequests.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { changeRequest}.ToDataSourceResult(request));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeRequests_Update([DataSourceRequest]DataSourceRequest request, ChangeRequest changeRequest)
        {
                     
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(changeRequest);
                db.ChangeRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { changeRequest }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeRequests_Destroy([DataSourceRequest]DataSourceRequest request, ChangeRequest changeRequest)
        {
            var change = db.ChangeRequests.Where(x => x.ChangeRequestId == changeRequest.ChangeRequestId).FirstOrDefault();
            if (change != null)
            {

                db.ChangeRequests.Attach(change);
                db.ChangeRequests.Remove(change);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted change Request " + changeRequest.ChangeRequestId);

               
            }

            return Json(new[] { changeRequest }.ToDataSourceResult(request));
        }
        
        private ChangeRequest GetMappedObject(ChangeRequest changeRequest)
        {
            var entity = new ChangeRequest
            {
                ChangeRequestId = changeRequest.ChangeRequestId,
                ChangeType = changeRequest.ChangeType,
                ChangeTypeDetail = changeRequest.ChangeTypeDetail,
                Title = changeRequest.Title,
                RequestorId = changeRequest.RequestorId,
                ApproverId = changeRequest.ApproverId,
                Description = changeRequest.Description,
                ImpactAnalysis = changeRequest.ImpactAnalysis,
                ImplementationTeam = changeRequest.ImplementationTeam,
                PlannedEndDate = changeRequest.PlannedEndDate,
                PlannedStartDate = changeRequest.PlannedStartDate,
                RollbackPlan = changeRequest.RollbackPlan,
                RequestDate = changeRequest.RequestDate,
                RiskAssessment = changeRequest.RiskAssessment,
                Status = changeRequest.Status,
                ChangeStatus = changeRequest.ChangeStatus,
                Remark = changeRequest.Remark,
                CreatedBy = changeRequest.CreatedBy,
                DateCreated = changeRequest.DateCreated,
                IsOnlineApproved = changeRequest.IsOnlineApproved,
                IsOnlineTransferred = changeRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = changeRequest.IsOnlineTransferredBy,
                IsSendForApproval = changeRequest.IsSendForApproval,
                IsVoid = changeRequest.IsVoid,
                LastModified = changeRequest.LastModified,
                ModifiedBy = changeRequest.ModifiedBy,
                OnlineApprovedBy = changeRequest.OnlineApprovedBy,
                OnlineApprovedTime = changeRequest.OnlineApprovedTime,
                OnlineTransferredTime = changeRequest.OnlineTransferredTime,
                OrgBranchName = changeRequest.OrgBranchName,
                SendForApprovalBy = changeRequest.SendForApprovalBy,
                SendForApprovalTime = changeRequest.SendForApprovalTime,
                VoidBy = changeRequest.VoidBy,
                VoidTime = changeRequest.VoidTime,
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
