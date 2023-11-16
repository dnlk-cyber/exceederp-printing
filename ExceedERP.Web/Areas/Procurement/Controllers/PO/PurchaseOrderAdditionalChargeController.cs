using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{
    
    public class PurchaseOrderAdditionalChargeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PoAdditionalCharges_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PoAdditionalCharge> PoAdditionalCharges = db.PoAdditionalCharges.Where(x => x.PurchaseOrderID == id);
            DataSourceResult result = PoAdditionalCharges.ToDataSourceResult(request, PoAdditionalCharge => new
            {
                PoAdditionalChargeId = PoAdditionalCharge.PoAdditionalChargeId,
                AmountType = PoAdditionalCharge.AmountType,
                Amount = PoAdditionalCharge.Amount,
                GLTaxRateID = PoAdditionalCharge.GLTaxRateID,
                PurchaseOrderID = PoAdditionalCharge.PurchaseOrderID,
                Remark = PoAdditionalCharge.Remark,
                OrgBranchName = PoAdditionalCharge.OrgBranchName,
                DateCreated = PoAdditionalCharge.DateCreated,
                LastModified = PoAdditionalCharge.LastModified,
                CreatedBy = PoAdditionalCharge.CreatedBy,
                ModifiedBy = PoAdditionalCharge.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoAdditionalCharges_Create(int id, [DataSourceRequest]DataSourceRequest request, PoAdditionalCharge PoAdditionalCharge)
        {

            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Purchase Order First");
            }

            var tax = db.GLTaxRates.Find(PoAdditionalCharge.GLTaxRateID);

            if (ModelState.IsValid)
            {
                var entity = new PoAdditionalCharge
                {
                    PurchaseOrderID = id,
                    AmountType = PoAdditionalCharge.AmountType,
                    Amount = PoAdditionalCharge.Amount,
                    GLTaxRateID = PoAdditionalCharge.GLTaxRateID,
                };
               
                entity.Remark = PoAdditionalCharge.Remark;
                entity.OrgBranchName = PoAdditionalCharge.OrgBranchName;
                entity.DateCreated = PoAdditionalCharge.DateCreated;
                entity.LastModified = DateTime.Today;
                entity.CreatedBy = PoAdditionalCharge.CreatedBy;
                entity.ModifiedBy = PoAdditionalCharge.ModifiedBy;

                db.PoAdditionalCharges.Add(entity);
                db.SaveChanges();
                PoAdditionalCharge.PoAdditionalChargeId = entity.PoAdditionalChargeId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order additional charge", "Success", User.Identity.Name, $"created item {entity.Remark} under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { PoAdditionalCharge }.ToDataSourceResult(request, ModelState));
        }
        

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoAdditionalCharges_Update([DataSourceRequest]DataSourceRequest request, PoAdditionalCharge PoAdditionalCharge)
        {

            var tax = db.GLTaxRates.Find(PoAdditionalCharge.GLTaxRateID);
            if (ModelState.IsValid)
            {
                PoAdditionalCharge entity =
                    new PoAdditionalCharge
                    {
                        PoAdditionalChargeId = PoAdditionalCharge.PoAdditionalChargeId,
                        PurchaseOrderID = PoAdditionalCharge.PurchaseOrderID,
                        AmountType = PoAdditionalCharge.AmountType,
                        Amount = PoAdditionalCharge.Amount,
                        GLTaxRateID = PoAdditionalCharge.GLTaxRateID,
                    };

             
                entity.Remark = PoAdditionalCharge.Remark;
                entity.OrgBranchName = PoAdditionalCharge.OrgBranchName;
                entity.DateCreated = PoAdditionalCharge.DateCreated;
                entity.LastModified = DateTime.Today;
                entity.CreatedBy = PoAdditionalCharge.CreatedBy;
                entity.ModifiedBy = User.Identity.Name;


                db.PoAdditionalCharges.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase additional charge", "Success", User.Identity.Name, $"updated item {entity.Remark} under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { PoAdditionalCharge }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoAdditionalCharges_Destroy([DataSourceRequest]DataSourceRequest request, PoAdditionalCharge PoAdditionalCharge)
        {
            if (ModelState.IsValid)
            {
                var entity = new PoAdditionalCharge
                {
                    PoAdditionalChargeId = PoAdditionalCharge.PoAdditionalChargeId,
                    AmountType = PoAdditionalCharge.AmountType,
                    Amount = PoAdditionalCharge.Amount,
                    GLTaxRateID = PoAdditionalCharge.GLTaxRateID,
                    Remark = PoAdditionalCharge.Remark,
                    OrgBranchName = PoAdditionalCharge.OrgBranchName,
                    DateCreated = PoAdditionalCharge.DateCreated,
                    LastModified = PoAdditionalCharge.LastModified,
                    CreatedBy = PoAdditionalCharge.CreatedBy,
                    ModifiedBy = PoAdditionalCharge.ModifiedBy
                };

                db.PoAdditionalCharges.Attach(entity);
                db.PoAdditionalCharges.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { PoAdditionalCharge }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(ProcurementRoles.ProcurementAdmin, ProcurementRoles.ProcurementTenderApprover, ProcurementRoles.ProcurementTenderUser)]

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        [AuthorizeRoles(ProcurementRoles.ProcurementAdmin, ProcurementRoles.ProcurementTenderApprover, ProcurementRoles.ProcurementTenderUser)]

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
