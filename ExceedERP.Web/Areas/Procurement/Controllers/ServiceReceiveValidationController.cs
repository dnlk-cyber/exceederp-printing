using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{

    public class ServiceReceiveValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover)]
        public ActionResult ServiceReceiveValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ServiceReceiveValidation> serviceReceiveValidations = _db.ServiceReceiveValidations.Where(gi => gi.ServiceReceiveId == id);
            DataSourceResult result = serviceReceiveValidations.ToDataSourceResult(request, serviceReceiveValidation => new
            {
                ServiceReceiveValidationId = serviceReceiveValidation.ServiceReceiveValidationId,
                Status = serviceReceiveValidation.Status,
                Remark = serviceReceiveValidation.Remark
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceiveValidations_Create([DataSourceRequest] DataSourceRequest request, ServiceReceiveValidation serviceReceiveValidation, int id)
        {
            var record = _db.ServiceReceives.Find(id);
            var pricesetting = _db.AveragePriceSettings.FirstOrDefault();
            #region Checking the grn

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (record.ServiceReceiveItems.Count == 0)
                {
                    string message = "The Record Does not Contain Any Item";
                    ModelState.AddModelError(String.Empty, message);
                }
               
                if (serviceReceiveValidation.Status == ApprovalStatuses.Reject)
                {
                    record.IsSendForApproval = false;
                    record.Status = "Rejected";
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "GRN record not found");
            }


            #endregion
           
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (serviceReceiveValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.ServiceReceiveId = id;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }
                    if (serviceReceiveValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new ServiceReceiveValidation
                    {
                        ServiceReceiveId = id,
                        Status = serviceReceiveValidation.Status,
                        Remark = serviceReceiveValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = serviceReceiveValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = serviceReceiveValidation.ModifiedBy
                    };

                    _db.ServiceReceiveValidations.Add(entity);
                    _db.SaveChanges();
                    serviceReceiveValidation.ServiceReceiveValidationId = entity.ServiceReceiveValidationId;
                }
               
            }
            

        return Json(new[] { serviceReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceiveValidations_Update([DataSourceRequest]DataSourceRequest request, ServiceReceiveValidation serviceReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ServiceReceiveValidation
                {
                    ServiceReceiveValidationId = serviceReceiveValidation.ServiceReceiveValidationId,
                    Status = serviceReceiveValidation.Status,
                    Remark = serviceReceiveValidation.Remark
                };

                _db.ServiceReceiveValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { serviceReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceiveValidations_Destroy([DataSourceRequest]DataSourceRequest request, ServiceReceiveValidation serviceReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ServiceReceiveValidation
                {
                    ServiceReceiveValidationId = serviceReceiveValidation.ServiceReceiveValidationId,
                    Status = serviceReceiveValidation.Status,
                    Remark = serviceReceiveValidation.Remark
                };

                _db.ServiceReceiveValidations.Attach(entity);
                _db.ServiceReceiveValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { serviceReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
