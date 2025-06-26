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
    public class ChangeApprovalsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
       
      
       
        public ActionResult ChangeApprovals_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
           
            var changeApprovals = db.ChangeApprovals.Where(x=>x.ChangeRequestId==Id).ToList();
            DataSourceResult result = changeApprovals.ToDataSourceResult(request, changeApproval => new ChangeApproval
            {
                ChangeApprovalId = changeApproval.ChangeApprovalId,
                ChangeRequestId = changeApproval.ChangeRequestId,
                Status = changeApproval.Status,
                ApproverId = changeApproval.ApproverId,
                CreatedBy = changeApproval.CreatedBy,
                IsApproved = changeApproval.IsApproved,
                Remark = changeApproval.Remark,
                DateCreated = changeApproval.DateCreated,
                LastModified = changeApproval.LastModified,
                ModifiedBy = changeApproval.ModifiedBy
            });

            return Json(result);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeApprovals_Create([DataSourceRequest]DataSourceRequest request, ChangeApproval changeRequest,int Id)
        {

            changeRequest.DateCreated = DateTime.Now;
            changeRequest.CreatedBy = User.Identity.Name;
            changeRequest.ChangeRequestId = Id;
            changeRequest.ApproverId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
           
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(changeRequest);
                db.ChangeApprovals.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { changeRequest}.ToDataSourceResult(request));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeApprovals_Update([DataSourceRequest]DataSourceRequest request, ChangeApproval changeRequest)
        {

            if (ModelState.IsValid)
            {
                changeRequest.LastModified = DateTime.Now;
                changeRequest.ModifiedBy = User.Identity.Name;
                changeRequest.ApproverId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var entity = GetMappedObject(changeRequest);
                db.ChangeApprovals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { changeRequest }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeApprovals_Destroy([DataSourceRequest]DataSourceRequest request, ChangeApproval changeTask)
        {
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(changeTask);
                db.ChangeApprovals.Attach(entity);
                db.ChangeApprovals.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Change Approval " + changeTask.ChangeApprovalId);

               
            }

            return Json(new[] { changeTask }.ToDataSourceResult(request));
        }
        
        private ChangeApproval GetMappedObject(ChangeApproval changeApproval)
        {
            var entity = new ChangeApproval
            {
                ChangeApprovalId = changeApproval.ChangeApprovalId,
                ChangeRequestId = changeApproval.ChangeRequestId,
                ApproverId = changeApproval.ApproverId,
                Status = changeApproval.Status,
                CreatedBy = changeApproval.CreatedBy,
                IsApproved = changeApproval.IsApproved,
                Remark = changeApproval.Remark,
                DateCreated = changeApproval.DateCreated,
                LastModified = changeApproval.LastModified,
                ModifiedBy = changeApproval.ModifiedBy,

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
