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
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class ChangeRequestSelfCaresController : BaseController
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

            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var changeRequests = db.ChangeRequests.Where(x=>x.RequestorId==employeeId).ToList();
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
            changeRequest.RequestorId = changeRequest.RequestorId?? UserInformationHelper.GetEmployeeId(User.Identity.Name);
            changeRequest.ApproverId = changeRequest.ApproverId?? EmployeeHelper.GetHeadId( UserInformationHelper.GetEmployeeId(User.Identity.Name));
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
        public JsonResult SendForApproval(int id)
        {
            var changeRequest = db.ChangeRequests.Find(id);
            if (changeRequest == null)
            {
                return Json(new { Message = "Record not found.!", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
            if (changeRequest.IsSendForApproval)
            {
                return Json(new { Message = "Already Send for approval.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);

            }
            changeRequest.IsSendForApproval = true;
            changeRequest.SendForApprovalTime = DateTime.Now;
            changeRequest.SendForApprovalBy = User.Identity.Name;
            db.Entry(changeRequest).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Message = "You have successfully sent for approval.", Status = "Ok" }, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
