using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.StoreRequisitions
{
    [Authorize]

    public class FixedAssetRequisitionValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion, FixedAssetConstants.FixedAssetRequistionApprover, FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion, FixedAssetConstants.FixedAssetRequistionApprover,FixedAssetConstants.FixedAssetRequistionSecondApprover, FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
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


        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistionSecondApprover)]
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
                        storeRequisitionValidation.FixedAssetRequisitionValidationId =
                            entity.FixedAssetRequisitionValidationId;
                    }
                }
              
            
            return Json(new[] { storeRequisitionValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistionApprover)]
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
            }

            return Json(new[] { storeRequisitionValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistionApprover)]
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
