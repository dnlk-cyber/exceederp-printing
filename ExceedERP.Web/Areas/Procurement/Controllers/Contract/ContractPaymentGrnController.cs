using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{
    public class ContractPaymentGrnController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
      
       [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser , ProcurementRoles.ProcurementForeignPurchaseApprover)]
        public ActionResult Index()
        {
            return View();
        }

       [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser , ProcurementRoles.ProcurementForeignPurchaseApprover)]
        public ActionResult ContractPaymentGrns_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ContractPaymentGrn> contractPaymentGrns = _db.ContractPaymentGrns.Where(x => x.ContractPaymentId == id);
            DataSourceResult result = contractPaymentGrns.ToDataSourceResult(request, contractPaymentGrn => new
            {
                ContractPaymentGrnId = contractPaymentGrn.ContractPaymentGrnId,
                ContractPaymentId = id,
                GrnNo = contractPaymentGrn.GrnNo,
                OrgBranchName = contractPaymentGrn.OrgBranchName,
                DateCreated = contractPaymentGrn.DateCreated,
                LastModified = contractPaymentGrn.LastModified,
                CreatedBy = contractPaymentGrn.CreatedBy,
                ModifiedBy = contractPaymentGrn.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentGrns_Create(int id, [DataSourceRequest]DataSourceRequest request, ContractPaymentGrn contractPaymentGrn)
        {
           
           
            if (ModelState.IsValid)
            {
                var entity = new ContractPaymentGrn
                {
                    ContractPaymentId = id,
                    GrnNo = contractPaymentGrn.GrnNo,
                    OrgBranchName = contractPaymentGrn.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = contractPaymentGrn.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = contractPaymentGrn.ModifiedBy
                };

                _db.ContractPaymentGrns.Add(entity);
                _db.SaveChanges();
                contractPaymentGrn.ContractPaymentGrnId = entity.ContractPaymentGrnId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract payment GRN", "Success", User.Identity.Name, $"created contract item {entity.GrnNo} under record {entity.ContractPaymentGrnId} ", Modules.Procurement);
                #endregion
            }
            return Json(new[] { contractPaymentGrn }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentGrns_Update(int id, [DataSourceRequest]DataSourceRequest request, ContractPaymentGrn contractPaymentGrn)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractPaymentGrn
                {
                    ContractPaymentId = id,
                    ContractPaymentGrnId = contractPaymentGrn.ContractPaymentGrnId,
                    GrnNo = contractPaymentGrn.GrnNo,
                    OrgBranchName = contractPaymentGrn.OrgBranchName,
                    DateCreated = contractPaymentGrn.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = contractPaymentGrn.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ContractPaymentGrns.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract payment GRN", "Success", User.Identity.Name, $"updated contract item {entity.GrnNo} under record {entity.ContractPaymentGrnId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractPaymentGrn }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentGrns_Destroy([DataSourceRequest]DataSourceRequest request, ContractPaymentGrn contractPaymentGrn)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new ContractPaymentGrn
                {
                    ContractPaymentGrnId = contractPaymentGrn.ContractPaymentGrnId,
                    GrnNo = contractPaymentGrn.GrnNo,
                    OrgBranchName = contractPaymentGrn.OrgBranchName,
                    DateCreated = contractPaymentGrn.DateCreated,
                    LastModified = contractPaymentGrn.LastModified,
                    CreatedBy = contractPaymentGrn.CreatedBy,
                    ModifiedBy = contractPaymentGrn.ModifiedBy
                };

                _db.ContractPaymentGrns.Attach(entity);
                _db.ContractPaymentGrns.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { contractPaymentGrn }.ToDataSourceResult(request, ModelState));
        }

      

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
