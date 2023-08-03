using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{
    
    public class ProformaEvaluationResultController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }


         [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult ProformaEvaluationResults_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<ProformaEvaluationResult> proformaevaluationresults = _db.ProformaEvaluationResults.Where(p => p.ProformaPurchaseID == id);
            DataSourceResult result = proformaevaluationresults.ToDataSourceResult(request, proformaEvaluationResult => new {
                ProformaEvaluationResultID = proformaEvaluationResult.ProformaEvaluationResultID,
                ProformaPurchaseID = proformaEvaluationResult.ProformaPurchaseID,
                SupplierId = proformaEvaluationResult.SupplierId,
                IsNewSupplier = proformaEvaluationResult.IsNewSupplier,
                SupplierName = proformaEvaluationResult.SupplierName,
                CatCode = proformaEvaluationResult.CatCode,
                CategoryName = proformaEvaluationResult.CategoryName,
                ItmCode = proformaEvaluationResult.ItmCode,
                ItemName = proformaEvaluationResult.ItemName,
                Name = proformaEvaluationResult.Name,
                ModelNo = proformaEvaluationResult.ModelNo,
                ProformaItemID = proformaEvaluationResult.ProformaItemID,
                Financial = proformaEvaluationResult.Financial,

                Technical = proformaEvaluationResult.Technical,
                Pass = proformaEvaluationResult.Pass,
                Total = proformaEvaluationResult.Total,

                OrgBranchName = proformaEvaluationResult.OrgBranchName,
                DateCreated = proformaEvaluationResult.DateCreated,
                LastModified = proformaEvaluationResult.LastModified,
                CreatedBy = proformaEvaluationResult.CreatedBy,
                ModifiedBy = proformaEvaluationResult.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaEvaluationResults_Create([DataSourceRequest]DataSourceRequest request, ProformaEvaluationResult proformaEvaluationResult , int id)
            {
           
            if (ModelState.IsValid)
            {
                var entity = new ProformaEvaluationResult
                {
                    ProformaPurchaseID = id,
                    SupplierId = proformaEvaluationResult.SupplierId,
                    IsNewSupplier = proformaEvaluationResult.IsNewSupplier,
                    SupplierName = proformaEvaluationResult.SupplierName,
                    Financial = proformaEvaluationResult.Financial,
                    ProformaItemID = proformaEvaluationResult.ProformaItemID,
                    CatCode = proformaEvaluationResult.CatCode,
                    CategoryName = proformaEvaluationResult.CategoryName,
                    ItmCode = proformaEvaluationResult.ItmCode,
                    ItemName = proformaEvaluationResult.ItemName,
                    Name = proformaEvaluationResult.Name,
                    ModelNo = proformaEvaluationResult.ModelNo,
                    Technical = proformaEvaluationResult.Technical,
                    Pass = proformaEvaluationResult.Pass,
                    Total = proformaEvaluationResult.Total,
                    OrgBranchName = proformaEvaluationResult.OrgBranchName,
                    DateCreated = proformaEvaluationResult.DateCreated,
                    LastModified = proformaEvaluationResult.LastModified,
                    CreatedBy = proformaEvaluationResult.CreatedBy,
                    ModifiedBy = proformaEvaluationResult.ModifiedBy
                };

                _db.ProformaEvaluationResults.Add(entity);
                _db.SaveChanges();
                proformaEvaluationResult.ProformaEvaluationResultID = entity.ProformaEvaluationResultID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma evaluation result", "Success", User.Identity.Name, $"created evaluation result {entity.SupplierName} under record {entity.ProformaEvaluationResultID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaEvaluationResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaEvaluationResults_Update([DataSourceRequest]DataSourceRequest request, ProformaEvaluationResult proformaEvaluationResult)
        {
          
            if (ModelState.IsValid)
            {
                var entity = new ProformaEvaluationResult
                {
                    ProformaEvaluationResultID = proformaEvaluationResult.ProformaEvaluationResultID,
                    SupplierId = proformaEvaluationResult.SupplierId,
                    IsNewSupplier = proformaEvaluationResult.IsNewSupplier,
                    SupplierName = proformaEvaluationResult.SupplierName,
                    ProformaPurchaseID = proformaEvaluationResult.ProformaPurchaseID,
                    Financial = proformaEvaluationResult.Financial,
                    Technical = proformaEvaluationResult.Technical,
                    Pass = proformaEvaluationResult.Pass,
                    Total = proformaEvaluationResult.Total,
                    ProformaItemID = proformaEvaluationResult.ProformaItemID,
                    CatCode = proformaEvaluationResult.CatCode,
                    CategoryName = proformaEvaluationResult.CategoryName,
                    ItmCode = proformaEvaluationResult.ItmCode,
                    ItemName = proformaEvaluationResult.ItemName,
                    Name = proformaEvaluationResult.Name,
                    ModelNo = proformaEvaluationResult.ModelNo,
                    OrgBranchName = proformaEvaluationResult.OrgBranchName,
                    DateCreated = proformaEvaluationResult.DateCreated,
                    LastModified = proformaEvaluationResult.LastModified,
                    CreatedBy = proformaEvaluationResult.CreatedBy,
                    ModifiedBy = proformaEvaluationResult.ModifiedBy
                };

                _db.ProformaEvaluationResults.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma evaluation result", "Success", User.Identity.Name, $"updated evaluation result {entity.SupplierName} under record {entity.ProformaEvaluationResultID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaEvaluationResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public ActionResult ProformaEvaluationResults_Destroy([DataSourceRequest]DataSourceRequest request, ProformaEvaluationResult proformaEvaluationResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaEvaluationResult
                {
                    ProformaPurchaseID = proformaEvaluationResult.ProformaPurchaseID,
                    ProformaEvaluationResultID = proformaEvaluationResult.ProformaEvaluationResultID,
                    SupplierId = proformaEvaluationResult.SupplierId,
                    IsNewSupplier = proformaEvaluationResult.IsNewSupplier,
                    SupplierName = proformaEvaluationResult.SupplierName,
                    ProformaItemID = proformaEvaluationResult.ProformaItemID,
                    ItmCode = proformaEvaluationResult.ItmCode,
                   
                    Financial = proformaEvaluationResult.Financial,
                    Technical = proformaEvaluationResult.Technical,
                    Pass = proformaEvaluationResult.Pass,
                    Total = proformaEvaluationResult.Total,

                    OrgBranchName = proformaEvaluationResult.OrgBranchName,
                    DateCreated = proformaEvaluationResult.DateCreated,
                    LastModified = proformaEvaluationResult.LastModified,
                    CreatedBy = proformaEvaluationResult.CreatedBy,
                    ModifiedBy = proformaEvaluationResult.ModifiedBy
                };

                _db.ProformaEvaluationResults.Attach(entity);
                _db.ProformaEvaluationResults.Remove(entity);
               
                _db.SaveChanges();
            }

            return Json(new[] { proformaEvaluationResult }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
