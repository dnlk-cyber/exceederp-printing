using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead, InventoryConstants.PropertyRequisitionSecondApprover)]
    public class StoreRequisitionSecondSelfCareValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult StoreRequisitionSecondValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<StoreRequisitionSecondValidation> StoreRequisitionSecondValidations = _db.StoreRequisitionSecondValidations.Where(x=>x.StoreRequisitionId==id);
            DataSourceResult result = StoreRequisitionSecondValidations.ToDataSourceResult(request, StoreRequisitionSecondValidation => new {
                StoreRequisitionSecondValidationId = StoreRequisitionSecondValidation.StoreRequisitionSecondValidationId,
                Status = StoreRequisitionSecondValidation.Status,
                Remark = StoreRequisitionSecondValidation.Remark,
                OrgBranchName = StoreRequisitionSecondValidation.OrgBranchName,
                DateCreated = StoreRequisitionSecondValidation.DateCreated,
                LastModified = StoreRequisitionSecondValidation.LastModified,
                CreatedBy = StoreRequisitionSecondValidation.CreatedBy,
                ModifiedBy = StoreRequisitionSecondValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyRequisitionSecondApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequisitionSecondValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, StoreRequisitionSecondValidation StoreRequisitionSecondValidation)
        {
            var storeRequisition = _db.StoreRequisitions.Find(id);
            if (storeRequisition != null /*&& storeRequisition.IsMiddleOnlineApproved*/)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate this Record: It is Already Approved");
            }

            if (ModelState.IsValid)
            {
                if (storeRequisition != null)
                {
                    if (StoreRequisitionSecondValidation.Status == ApprovalStatuses.Approve)
                    {

                        storeRequisition.Status = "Second Approved";
                        //storeRequisition.IsMiddleOnlineApproved = true;
                        //storeRequisition.MiddleOnlineApprovedBy = User.Identity.Name;
                        //storeRequisition.MiddleOnlineApprovedTime = DateTime.Now;
                        _db.Entry(storeRequisition).State = EntityState.Modified;
                        _db.SaveChanges();

                    }
                    else if (StoreRequisitionSecondValidation.Status == ApprovalStatuses.Reject)
                    {
                        //storeRequisition.MiddleOnlineApprovedBy = "";
                        //storeRequisition.IsMiddleSendForApproval = false;
                        storeRequisition.Status = "Second Approval Rejected";
                        _db.Entry(storeRequisition).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                   

                    var entity = new StoreRequisitionSecondValidation
                    {
                        StoreRequisitionId = id,
                        Status = StoreRequisitionSecondValidation.Status,
                        Remark = StoreRequisitionSecondValidation.Remark,
                        OrgBranchName = StoreRequisitionSecondValidation.OrgBranchName,
                        DateCreated = StoreRequisitionSecondValidation.DateCreated,
                        LastModified = StoreRequisitionSecondValidation.LastModified,
                        CreatedBy = StoreRequisitionSecondValidation.CreatedBy,
                        ModifiedBy = StoreRequisitionSecondValidation.ModifiedBy
                    };

                    _db.StoreRequisitionSecondValidations.Add(entity);
                    _db.SaveChanges();
                    StoreRequisitionSecondValidation.StoreRequisitionSecondValidationId =
                        entity.StoreRequisitionSecondValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "StoreRequisitionSecondValidation", "Success", User.Identity.Name, $"created StoreRequisitionSecondValidation  {entity.StoreRequisitionSecondValidationId} Tag {entity.StoreRequisitionId}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { StoreRequisitionSecondValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
