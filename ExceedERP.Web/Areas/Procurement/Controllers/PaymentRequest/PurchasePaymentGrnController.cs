using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Procurement;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PaymentRequest
{
    public class PurchasePaymentGrnController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
      
       [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser , ProcurementRoles.ProcurementPaymentApprover)]
        public ActionResult Index()
        {
            return View();
        }

       [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser , ProcurementRoles.ProcurementPaymentApprover)]
        public ActionResult PurchasePaymentGrns_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchasePaymentGrn> PurchasePaymentGrns = _db.PurchasePaymentGrns.Where(x => x.PurchasePaymentId == id);
            DataSourceResult result = PurchasePaymentGrns.ToDataSourceResult(request, PurchasePaymentGrn => new
            {
                PurchasePaymentGrnId = PurchasePaymentGrn.PurchasePaymentGrnId,
                PurchasePaymentId = id,
                ItemType = PurchasePaymentGrn.ItemType,
                GrnNo = PurchasePaymentGrn.GrnNo,
                OrgBranchName = PurchasePaymentGrn.OrgBranchName,
                DateCreated = PurchasePaymentGrn.DateCreated,
                LastModified = PurchasePaymentGrn.LastModified,
                CreatedBy = PurchasePaymentGrn.CreatedBy,
                ModifiedBy = PurchasePaymentGrn.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentGrns_Create(int id, [DataSourceRequest]DataSourceRequest request, PurchasePaymentGrn purchasePaymentGrn)
        {
            #region validation
            var exist=_db.PurchasePaymentGrns                
                .Where(x=>x.ItemType==purchasePaymentGrn.ItemType)
                .Where(x => x.GrnNo == purchasePaymentGrn.GrnNo)
                .FirstOrDefault();

            if (exist != null)
                ModelState.AddModelError("exist",$" Grn No of  {purchasePaymentGrn.GrnNo} type {purchasePaymentGrn.ItemType} is alreay added in purchase payment {exist.PurchasePaymentId}.");
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentGrn
                {
                    PurchasePaymentId = id,
                    ItemType= purchasePaymentGrn.ItemType,
                    GrnNo = purchasePaymentGrn.GrnNo,
                    OrgBranchName = purchasePaymentGrn.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = purchasePaymentGrn.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = purchasePaymentGrn.ModifiedBy
                };

                _db.PurchasePaymentGrns.Add(entity);
                _db.SaveChanges();
                purchasePaymentGrn.PurchasePaymentGrnId = entity.PurchasePaymentGrnId;

                PaymentHelper.UpdatePurchasePaymentTotals(entity.PurchasePaymentId);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment GRN", "Success", User.Identity.Name, $"created Purchase item {entity.GrnNo} under record {entity.PurchasePaymentGrnId} ", Modules.Procurement);
                #endregion
            }
            return Json(new[] { purchasePaymentGrn }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentGrns_Update([DataSourceRequest]DataSourceRequest request, PurchasePaymentGrn purchasePaymentGrn)
        {
            #region validation
            var exist = _db.PurchasePaymentGrns
                .Where(x => x.PurchasePaymentGrnId != purchasePaymentGrn.PurchasePaymentGrnId)
                .Where(x => x.ItemType == purchasePaymentGrn.ItemType)
                .Where(x => x.GrnNo == purchasePaymentGrn.GrnNo)
                .FirstOrDefault();

            if (exist != null)
                ModelState.AddModelError("exist", $" Grn No of  {purchasePaymentGrn.GrnNo} type {purchasePaymentGrn.ItemType} is alreay added in purchase payment {exist.PurchasePaymentId}.");
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentGrn
                {                               
                    PurchasePaymentGrnId = purchasePaymentGrn.PurchasePaymentGrnId,
                    PurchasePaymentId = purchasePaymentGrn.PurchasePaymentId,
                    GrnNo = purchasePaymentGrn.GrnNo,
                    ItemType = purchasePaymentGrn.ItemType,
                    OrgBranchName = purchasePaymentGrn.OrgBranchName,
                    DateCreated = purchasePaymentGrn.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = purchasePaymentGrn.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.PurchasePaymentGrns.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                PaymentHelper.UpdatePurchasePaymentTotals(entity.PurchasePaymentId);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment GRN", "Success", User.Identity.Name, $"updated Purchase item {entity.GrnNo} under record {entity.PurchasePaymentGrnId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchasePaymentGrn }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentGrns_Destroy([DataSourceRequest]DataSourceRequest request, PurchasePaymentGrn PurchasePaymentGrn)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentGrn
                {
                    PurchasePaymentGrnId = PurchasePaymentGrn.PurchasePaymentGrnId,
                    PurchasePaymentId = PurchasePaymentGrn.PurchasePaymentId,
                    GrnNo = PurchasePaymentGrn.GrnNo,
                    ItemType = PurchasePaymentGrn.ItemType,
                    OrgBranchName = PurchasePaymentGrn.OrgBranchName,
                    DateCreated = PurchasePaymentGrn.DateCreated,
                    LastModified = PurchasePaymentGrn.LastModified,
                    CreatedBy = PurchasePaymentGrn.CreatedBy,
                    ModifiedBy = PurchasePaymentGrn.ModifiedBy
                };

                _db.PurchasePaymentGrns.Attach(entity);
                _db.PurchasePaymentGrns.Remove(entity);
                _db.SaveChanges();

                PaymentHelper.UpdatePurchasePaymentTotals(PurchasePaymentGrn.PurchasePaymentId);

                #region operation log
                UserHelper.DeleteLog(Request.UserHostAddress, "Purchase payment GRN", "Success", User.Identity.Name, $"deleted Purchase item {PurchasePaymentGrn.GrnNo} under record {entity.PurchasePaymentGrnId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { PurchasePaymentGrn }.ToDataSourceResult(request, ModelState));
        }

      

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
