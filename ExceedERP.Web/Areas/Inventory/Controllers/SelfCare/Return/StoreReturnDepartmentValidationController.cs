using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.SelfCare.Return
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class StoreReturnDepartmentValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

       
        public ActionResult Index()
        {
            return View();
        }

      
        public ActionResult StoreReturnDepartmentValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<StoreReturnDepartmentValidation> storereturnvalidations = db.StoreReturnDepartmentValidations.Where(sr => sr.StoreReturnID == id);
            DataSourceResult result = storereturnvalidations.ToDataSourceResult(request, storeReturnValidation => new
            {
                StoreReturnDepartmentValidationID = storeReturnValidation.StoreReturnDepartmentValidationID,
                Status = storeReturnValidation.Status,
                Remark = storeReturnValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnDepartmentValidations_Create([DataSourceRequest]DataSourceRequest request, StoreReturnDepartmentValidation storeReturnValidation, int id)
        {
            var record = db.StoreReturns.Find(id);
            var items = db.StoreReturnItemss.Where(x => x.StoreReturnID == id).AsNoTracking().ToList();
            var storeItemAssignment = db.StoreItemAssignments.Where(st => st.StoreCode == record.StoreCode);
           
            if (record != null && record.IsDepartmentOnlineApproved)
            {
                string message = "You cannot manipulate this record,it  is already approved";
                ModelState.AddModelError("approved", message);
            }
            if (record != null && record.StoreReturnItems.Count == 0)
            {
                string message = "The Record Does not Contain Any Item";
                ModelState.AddModelError(String.Empty, message);
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (storeReturnValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Department Approved";
                        record.IsDepartmentOnlineApproved = true;
                        record.DepartmentOnlineApprovedBy = User.Identity.Name;
                        record.DepartmentOnlineApprovedTime = DateTime.Now;
                        db.StoreReturns.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                       
                        
                    }
                    else if (storeReturnValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsDepartmentSendForApproval = false;
                        record.Status = "Department Rejected";
                        db.StoreReturns.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                       // db.SaveChanges();
                    }
                   

                    var entity = new StoreReturnDepartmentValidation
                    {
                        StoreReturnID = id,
                        Status = storeReturnValidation.Status,
                        Remark = storeReturnValidation.Remark,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };
                    db.StoreReturnDepartmentValidations.Add(entity);
                    db.SaveChanges();
                    storeReturnValidation.StoreReturnDepartmentValidationID = entity.StoreReturnDepartmentValidationID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store return approval", "Success", User.Identity.Name, $"approved store return {entity.StoreReturnID} ", Modules.Inventory);
                    #endregion
                }


            }
            return Json(new[] { storeReturnValidation }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnDepartmentValidations_Update([DataSourceRequest]DataSourceRequest request, StoreReturnDepartmentValidation storeReturnValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreReturnDepartmentValidation
                {
                    StoreReturnDepartmentValidationID = storeReturnValidation.StoreReturnDepartmentValidationID,
                    Status = storeReturnValidation.Status,
                    Remark = storeReturnValidation.Remark,
                };

                db.StoreReturnDepartmentValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { storeReturnValidation }.ToDataSourceResult(request, ModelState));
        }

   
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnDepartmentValidations_Destroy([DataSourceRequest]DataSourceRequest request, StoreReturnDepartmentValidation storeReturnValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreReturnDepartmentValidation
                {
                    StoreReturnDepartmentValidationID = storeReturnValidation.StoreReturnDepartmentValidationID,
                    Status = storeReturnValidation.Status,
                    Remark = storeReturnValidation.Remark,
                };

                db.StoreReturnDepartmentValidations.Attach(entity);
                db.StoreReturnDepartmentValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { storeReturnValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
