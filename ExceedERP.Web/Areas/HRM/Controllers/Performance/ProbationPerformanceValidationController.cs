using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRDepartmentHead, HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin)]
    public class ProbationPerformanceValidationsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
     
        public ActionResult ProbationPerformanceValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProbationPerformanceValidation> requestValidations = db.ProbationPerformanceValidations.Where(x=>x.ProbationPerformanceId == id);
            DataSourceResult result = requestValidations.ToDataSourceResult(request, requestValidation => new
            {

                ProbationPerformanceValidationId = requestValidation.ProbationPerformanceValidationId,
                ProbationPerformanceId = requestValidation.ProbationPerformanceId,
                Status = requestValidation.Status,
                Remark = requestValidation.Remark,
                CreatedBy = requestValidation.CreatedBy,
                DateCreated = requestValidation.DateCreated,
                LastModified = requestValidation.LastModified,
                ModifiedBy = requestValidation.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceValidations_Create( [DataSourceRequest] DataSourceRequest request, ProbationPerformanceValidation requestValidation,int id)
        {
            ModelState.Clear();

            var record = db.ProbationPerformances.FirstOrDefault(x=>x.ProbationPerformanceId == id);
            if (record == null)
            {
                ModelState.AddModelError("approved", "Record not found");
                return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
            }
            bool exists = db.ProbationPerformanceValidations
             .Any(x => x.ProbationPerformanceId == id
           && x.Status == requestValidation.Status);
            if (exists)
            {
                ModelState.AddModelError("validations", "Record already exists for this status");
                return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
            }
            if (record.IsOnlineApproved)
            {
                ModelState.AddModelError("approved", "You can't manipulate this record, it is already approved");
                return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
            }
            
           

            if (!ModelState.IsValid)
                return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));

            var now = DateTime.Now;
            var user = User.Identity.Name;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    requestValidation.ProbationPerformanceId = id;
                    requestValidation.DateCreated = now;
                    requestValidation.CreatedBy = user;

                    // ----- STATUS HANDLING -----
                    switch (requestValidation.Status)
                    {
                        case ApprovalStatuses.Reject:
                            record.Status = "HR Rejected";
                            break;

                        case ApprovalStatuses.Pending:
                            record.Status = "HR pending";
                            break;

                        case ApprovalStatuses.Approve:
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.OnlineApprovedBy = user;
                            record.OnlineApprovedTime = now;

                            break;
                    }


                    db.Entry(record).State = EntityState.Modified;

                    var entity = GetMappedObject(requestValidation);
                    db.ProbationPerformanceValidations.Add(entity);
                    db.SaveChanges();
                    transaction.Commit();
                    requestValidation.ProbationPerformanceValidationId = entity.ProbationPerformanceValidationId;
                    UserHelper.OperationLog(Request.UserHostAddress,"Employee Probation Performance approval","Success",user,$"approved ProbationPerformance request {entity.ProbationPerformanceId}",
                        Modules.HRM);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceValidations_Update([DataSourceRequest]DataSourceRequest request, ProbationPerformanceValidation requestValidation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                requestValidation.LastModified = DateTime.Now;
                requestValidation.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestValidation);
                db.ProbationPerformanceValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceValidations_Destroy([DataSourceRequest]DataSourceRequest request, ProbationPerformanceValidation requestValidation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(requestValidation);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee Probation Performance Validation   information of applicant" + requestValidation.ProbationPerformanceValidationId);
                #endregion
                db.ProbationPerformanceValidations.Attach(entity);
                db.ProbationPerformanceValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }
        private ProbationPerformanceValidation GetMappedObject(ProbationPerformanceValidation requestValidation)
        {
            var entity = new ProbationPerformanceValidation
            {
                ProbationPerformanceValidationId = requestValidation.ProbationPerformanceValidationId,
                ProbationPerformanceId = requestValidation.ProbationPerformanceId,
                Status = requestValidation.Status,
                Remark = requestValidation.Remark,
                CreatedBy= requestValidation.CreatedBy,
                DateCreated= requestValidation.DateCreated,
                LastModified= requestValidation.LastModified,
                ModifiedBy= requestValidation.ModifiedBy,
                

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
