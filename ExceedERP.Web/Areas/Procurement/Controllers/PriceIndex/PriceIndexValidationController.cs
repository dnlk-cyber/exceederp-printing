using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]
    public class PriceIndexValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        public ActionResult PriceIndexValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PriceIndexValidation> PriceIndexValidations = db.PriceIndexValidations.Where(ii => ii.ItemPriceIndexID == id);
            DataSourceResult result = PriceIndexValidations.ToDataSourceResult(request, PriceIndexValidation => new
            {
                PriceIndexValidationId = PriceIndexValidation.PriceIndexValidationId,
                Status = PriceIndexValidation.Status,
                Remark = PriceIndexValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriceIndexValidations_Create([DataSourceRequest] DataSourceRequest request, PriceIndexValidation PriceIndexValidation, int id)
        {

            var record = db.ItemPriceIndexes.Find(id);           
            #region validation 

            if (record == null)
            {                
                ModelState.AddModelError("No Found", "Issue record not found");
            }

            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (PriceIndexValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.ItemPriceIndexID = id;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.Entry(record).State = EntityState.Modified;
                       // db.SaveChanges();
                    }
                    if (PriceIndexValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                       // db.SaveChanges();
                    }
                    var entity = new PriceIndexValidation
                    {
                        ItemPriceIndexID = id,
                        Status = PriceIndexValidation.Status,
                        Remark = PriceIndexValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = PriceIndexValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = PriceIndexValidation.ModifiedBy
                    };

                    db.PriceIndexValidations.Add(entity);
                    db.SaveChanges();
                    PriceIndexValidation.PriceIndexValidationId = entity.PriceIndexValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "price index approval", "Success", User.Identity.Name, $"approved item price index {entity.ItemPriceIndexID} ", Modules.Procurement);
                    #endregion
                }

            }

            return Json(new[] { PriceIndexValidation }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriceIndexValidations_Update([DataSourceRequest]DataSourceRequest request, PriceIndexValidation PriceIndexValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceIndexValidation
                {
                    PriceIndexValidationId = PriceIndexValidation.PriceIndexValidationId,
                    Status = PriceIndexValidation.Status,
                    Remark = PriceIndexValidation.Remark,
                };

                db.PriceIndexValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PriceIndexValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriceIndexValidations_Destroy([DataSourceRequest]DataSourceRequest request, PriceIndexValidation PriceIndexValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceIndexValidation
                {
                    PriceIndexValidationId = PriceIndexValidation.PriceIndexValidationId,
                    Status = PriceIndexValidation.Status,
                    Remark = PriceIndexValidation.Remark,
                };

                db.PriceIndexValidations.Attach(entity);
                db.PriceIndexValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { PriceIndexValidation }.ToDataSourceResult(request, ModelState));
        }

       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
