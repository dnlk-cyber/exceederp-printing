using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;

using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApPaymentSettlementGrnController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
      
       [AuthorizeRoles(FinanceConstants.AccountPayableSettlement , FinanceConstants.AccountPayableApprover)]
        public ActionResult Index()
        {
            return View();
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableSettlement , FinanceConstants.AccountPayableApprover)]
        public ActionResult ApPaymentSettlementGrns_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPaymentSettlementGrn> ApPaymentSettlementGrns = _db.ApPaymentSettlementGrns.Where(x => x.ApPaymentSettlementId == id);
            DataSourceResult result = ApPaymentSettlementGrns.ToDataSourceResult(request, ApPaymentSettlementGrn => new
            {
                ApPaymentSettlementGrnId = ApPaymentSettlementGrn.ApPaymentSettlementGrnId,
                ApPaymentSettlementId = id,
                ItemType = ApPaymentSettlementGrn.ItemType,
                GrnNo = ApPaymentSettlementGrn.GrnNo,
                OrgBranchName = ApPaymentSettlementGrn.OrgBranchName,
                DateCreated = ApPaymentSettlementGrn.DateCreated,
                LastModified = ApPaymentSettlementGrn.LastModified,
                CreatedBy = ApPaymentSettlementGrn.CreatedBy,
                ModifiedBy = ApPaymentSettlementGrn.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableSettlement)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementGrns_Create(int id, [DataSourceRequest]DataSourceRequest request, ApPaymentSettlementGrn ApPaymentSettlementGrn)
        {
           
           
            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlementGrn
                {
                    ApPaymentSettlementId = id,
                    ItemType= ApPaymentSettlementGrn.ItemType,
                    GrnNo = ApPaymentSettlementGrn.GrnNo,
                    OrgBranchName = ApPaymentSettlementGrn.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = ApPaymentSettlementGrn.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = ApPaymentSettlementGrn.ModifiedBy
                };

                _db.ApPaymentSettlementGrns.Add(entity);
                _db.SaveChanges();
                ApPaymentSettlementGrn.ApPaymentSettlementGrnId = entity.ApPaymentSettlementGrnId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment GRN", "Success", User.Identity.Name, $"created Purchase item {entity.GrnNo} under record {entity.ApPaymentSettlementGrnId} ", Modules.Procurement);
                #endregion
            }
            return Json(new[] { ApPaymentSettlementGrn }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableSettlement)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementGrns_Update([DataSourceRequest]DataSourceRequest request, ApPaymentSettlementGrn ApPaymentSettlementGrn)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlementGrn
                {                               
                    ApPaymentSettlementGrnId = ApPaymentSettlementGrn.ApPaymentSettlementGrnId,
                    ApPaymentSettlementId = ApPaymentSettlementGrn.ApPaymentSettlementId,
                    GrnNo = ApPaymentSettlementGrn.GrnNo,
                    ItemType = ApPaymentSettlementGrn.ItemType,
                    OrgBranchName = ApPaymentSettlementGrn.OrgBranchName,
                    DateCreated = ApPaymentSettlementGrn.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = ApPaymentSettlementGrn.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ApPaymentSettlementGrns.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment GRN", "Success", User.Identity.Name, $"updated Purchase item {entity.GrnNo} under record {entity.ApPaymentSettlementGrnId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { ApPaymentSettlementGrn }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableSettlement)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementGrns_Destroy([DataSourceRequest]DataSourceRequest request, ApPaymentSettlementGrn ApPaymentSettlementGrn)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlementGrn
                {
                    ApPaymentSettlementGrnId = ApPaymentSettlementGrn.ApPaymentSettlementGrnId,
                    ApPaymentSettlementId = ApPaymentSettlementGrn.ApPaymentSettlementId,
                    GrnNo = ApPaymentSettlementGrn.GrnNo,
                    ItemType = ApPaymentSettlementGrn.ItemType,
                    OrgBranchName = ApPaymentSettlementGrn.OrgBranchName,
                    DateCreated = ApPaymentSettlementGrn.DateCreated,
                    LastModified = ApPaymentSettlementGrn.LastModified,
                    CreatedBy = ApPaymentSettlementGrn.CreatedBy,
                    ModifiedBy = ApPaymentSettlementGrn.ModifiedBy
                };

                _db.ApPaymentSettlementGrns.Attach(entity);
                _db.ApPaymentSettlementGrns.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { ApPaymentSettlementGrn }.ToDataSourceResult(request, ModelState));
        }

      

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
