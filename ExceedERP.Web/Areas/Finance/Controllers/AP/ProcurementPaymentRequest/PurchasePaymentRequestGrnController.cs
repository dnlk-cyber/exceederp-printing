using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.ProcurementPaymentRequest
{
    public class PurchasePaymentRequestGrnController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
      
       [AuthorizeRoles(FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableApprover)]
        public ActionResult Index()
        {
            return View();
        }

        private void LoadGrns(int id)
        {
            var purchasePayment = _db.PurchasePayments.Find(id);
            if (purchasePayment != null)
            {
                var po = _db.PurchaseOrders.Find(int.Parse(purchasePayment.PoOrContractNumber));
                var receiveType = _db.ReceiveTypes.Where(x => x.Type == GoodReceiveType.PurchaseOrder).FirstOrDefault();
                if(receiveType!=null && po !=null)
                {
                    #region stock grn
                    var stockGrns = _db.GoodReceives
                        .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId && x.ReceiveRef == po.PurchaseOrderID.ToString())
                        .Where(x=>x.IsOnlineApproved && x.IsVoid==false)
                        .ToList();
                    foreach(var g in stockGrns)
                    {
                        var exisit = _db.PurchasePaymentGrns
                            .Where(x => x.PurchasePaymentId == id)
                             .Where(x => x.ItemType == PurchaseItemType.Stock)
                            .Where(x => x.GrnNo == g.GoodReceiveId).FirstOrDefault();

                        if(exisit==null)
                        {
                            var entity = new PurchasePaymentGrn
                            {
                                PurchasePaymentId = id,
                                ItemType = PurchaseItemType.Stock,
                                GrnNo =g.GoodReceiveId,
                                OrgBranchName = "NA",
                                DateCreated = DateTime.Now,
                                CreatedBy = "System",
                            };

                            _db.PurchasePaymentGrns.Add(entity);
                            _db.SaveChanges();

                        }

                    }
                    #endregion
                    #region fixed asset grn
                    var fixedGrns = _db.FixedAssetReceives
                        .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId && x.ReceiveRef == po.PurchaseOrderID.ToString())
                        .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                        .ToList();
                    foreach (var g in fixedGrns)
                    {
                        var exisit = _db.PurchasePaymentGrns
                            .Where(x => x.PurchasePaymentId == id)
                             .Where(x => x.ItemType == PurchaseItemType.FixedAsset)
                            .Where(x => x.GrnNo == g.FixedAssetReceiveId).FirstOrDefault();

                        if (exisit == null)
                        {
                            var entity = new PurchasePaymentGrn
                            {
                                PurchasePaymentId = id,
                                ItemType = PurchaseItemType.FixedAsset,
                                GrnNo = g.FixedAssetReceiveId,
                                OrgBranchName = "NA",
                                DateCreated = DateTime.Now,
                                CreatedBy = "System",
                            };

                            _db.PurchasePaymentGrns.Add(entity);
                            _db.SaveChanges();

                        }

                    }
                    #endregion
                }
            }
        }

       [AuthorizeRoles(FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableApprover)]
        public ActionResult PurchasePaymentGrns_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            LoadGrns(id);

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

       [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentGrns_Create(int id, [DataSourceRequest]DataSourceRequest request, PurchasePaymentGrn PurchasePaymentGrn)
        {
           
           
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentGrn
                {
                    PurchasePaymentId = id,
                    ItemType= PurchasePaymentGrn.ItemType,
                    GrnNo = PurchasePaymentGrn.GrnNo,
                    OrgBranchName = PurchasePaymentGrn.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = PurchasePaymentGrn.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = PurchasePaymentGrn.ModifiedBy
                };

                _db.PurchasePaymentGrns.Add(entity);
                _db.SaveChanges();
                PurchasePaymentGrn.PurchasePaymentGrnId = entity.PurchasePaymentGrnId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment GRN", "Success", User.Identity.Name, $"created Purchase item {entity.GrnNo} under record {entity.PurchasePaymentGrnId} ", Modules.Procurement);
                #endregion
            }
            return Json(new[] { PurchasePaymentGrn }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentGrns_Update([DataSourceRequest]DataSourceRequest request, PurchasePaymentGrn PurchasePaymentGrn)
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
                    LastModified = DateTime.Now,
                    CreatedBy = PurchasePaymentGrn.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.PurchasePaymentGrns.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment GRN", "Success", User.Identity.Name, $"updated Purchase item {entity.GrnNo} under record {entity.PurchasePaymentGrnId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { PurchasePaymentGrn }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
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
