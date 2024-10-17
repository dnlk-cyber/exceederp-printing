using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.StoreRequisitions
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]

    public class FixedAssetRequisitionSelfCareValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FixedAssetRequisitionValidations_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<FixedAssetRequisitionValidation> storerequisitionvalidations = _db.FixedAssetRequisitionValidations.Where(sr => sr.FixedAssetRequisitionId == id);
            DataSourceResult result = storerequisitionvalidations.ToDataSourceResult(request, storeRequisitionValidation => new {
                FixedAssetRequisitionValidationId = storeRequisitionValidation.FixedAssetRequisitionValidationId,
                Status = storeRequisitionValidation.Status,
                Remark = storeRequisitionValidation.Remark,
            });

            return Json(result);
        }


        [AuthorizeRoles( HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionValidations_Create([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionValidation storeRequisitionValidation, int id)
        {
            var storeRequisition = _db.FixedAssetRequisitions.Find(id);
            if (storeRequisition != null && storeRequisition.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate this Record: It is Already Approved");
            }
           
                
            if (ModelState.IsValid)
            {
                if (storeRequisition != null)
                {
                    if (storeRequisitionValidation.Status == ApprovalStatuses.Approve)
                    {
                        storeRequisition.Status = "User Approved";
                        storeRequisition.IsOnlineApproved = true;
                        storeRequisition.OnlineApprovedBy = User.Identity.Name;
                        storeRequisition.OnlineApprovedTime = DateTime.Now.Date;
                    }
                    else if (storeRequisitionValidation.Status == ApprovalStatuses.Reject)
                    {
                        storeRequisition.IsSendForApproval = false;
                        storeRequisition.Status = "User Rejected";
                    }
                    _db.Entry(storeRequisition).State = EntityState.Modified;
                    _db.SaveChanges();
                    var entity = new FixedAssetRequisitionValidation
                    {
                        FixedAssetRequisitionId = id,
                        Status = storeRequisitionValidation.Status,
                        Remark = storeRequisitionValidation.Remark,
                    };

                    _db.FixedAssetRequisitionValidations.Add(entity);
                    _db.SaveChanges();
                    storeRequisitionValidation.FixedAssetRequisitionValidationId = entity.FixedAssetRequisitionValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisitionValidation", "Success", User.Identity.Name, $"updated FixedAssetRequisitionValidation  {entity.FixedAssetRequisitionValidationId} FixedAssetRequisition {entity.FixedAssetRequisitionId}", Modules.FixedAsset);
                    #endregion
                }
            }
              
            
            return Json(new[] { storeRequisitionValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
         public ActionResult FixedAssetRequisitionValidations_Update([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionValidation storeRequisitionValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionValidation
                {
                    FixedAssetRequisitionValidationId = storeRequisitionValidation.FixedAssetRequisitionValidationId,
                    Status = storeRequisitionValidation.Status,
                    Remark = storeRequisitionValidation.Remark,
                };

                _db.FixedAssetRequisitionValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete FixedAssetRequisitionValidation  {storeRequisitionValidation.FixedAssetRequisitionValidationId} FixedAssetRequisition {storeRequisitionValidation.FixedAssetRequisitionId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { storeRequisitionValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
       public ActionResult FixedAssetRequisitionValidations_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionValidation storeRequisitionValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionValidation
                {
                    FixedAssetRequisitionValidationId = storeRequisitionValidation.FixedAssetRequisitionValidationId,
                    Status = storeRequisitionValidation.Status,
                    Remark = storeRequisitionValidation.Remark,
                };

                _db.FixedAssetRequisitionValidations.Attach(entity);
                _db.FixedAssetRequisitionValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { storeRequisitionValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
