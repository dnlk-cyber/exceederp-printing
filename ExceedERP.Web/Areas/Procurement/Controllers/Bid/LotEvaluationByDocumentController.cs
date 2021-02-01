using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
    public class LotEvaluationByDocumentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult LotEvaluationByDocuments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<LotEvaluationByDocument> lotevaluationbydocuments = _db.LotEvaluationByDocuments.Where(x => x.LotID == id);
            DataSourceResult result = lotevaluationbydocuments.ToDataSourceResult(request, lotEvaluationByDocument => new
            {
                LotEvaluationByDocumentId = lotEvaluationByDocument.LotEvaluationByDocumentId,
                LotID = lotEvaluationByDocument.LotID,
                SupplierId = lotEvaluationByDocument.SupplierId,
                IsNewSupplier = lotEvaluationByDocument.IsNewSupplier,
                SupplierName = lotEvaluationByDocument.SupplierName,
                Document = lotEvaluationByDocument.Document,
                IsAvailable = lotEvaluationByDocument.IsAvailable,
                Remark = lotEvaluationByDocument.Remark,
                OrgBranchName = lotEvaluationByDocument.OrgBranchName,
                DateCreated = lotEvaluationByDocument.DateCreated,
                LastModified = lotEvaluationByDocument.LastModified,
                CreatedBy = lotEvaluationByDocument.CreatedBy,
                ModifiedBy = lotEvaluationByDocument.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotEvaluationByDocuments_Create(int id, [DataSourceRequest]DataSourceRequest request, LotEvaluationByDocument lotEvaluationByDocument)
        {

            var pParticipatingSuppliers = _db.BidPParticipatingSupplierses.Find(int.Parse(lotEvaluationByDocument.SupplierName));//name has purchase id value
            if (pParticipatingSuppliers != null)
            {
                lotEvaluationByDocument.SupplierId = pParticipatingSuppliers.SupplierId;
                lotEvaluationByDocument.IsNewSupplier = pParticipatingSuppliers.IsNewSupplier;
                lotEvaluationByDocument.SupplierName = pParticipatingSuppliers.SupplierName;
            }

            if (ModelState.IsValid)
            {
                var entity = new LotEvaluationByDocument
                {
                    LotID = id,
                    SupplierId = lotEvaluationByDocument.SupplierId,
                    IsNewSupplier = lotEvaluationByDocument.IsNewSupplier,
                    SupplierName = lotEvaluationByDocument.SupplierName,
                    Document = lotEvaluationByDocument.Document,
                    IsAvailable = lotEvaluationByDocument.IsAvailable,
                    Remark = lotEvaluationByDocument.Remark,
                    OrgBranchName = lotEvaluationByDocument.OrgBranchName,
                    DateCreated = lotEvaluationByDocument.DateCreated,
                    LastModified = lotEvaluationByDocument.LastModified,
                    CreatedBy = lotEvaluationByDocument.CreatedBy,
                    ModifiedBy = lotEvaluationByDocument.ModifiedBy
                };

                _db.LotEvaluationByDocuments.Add(entity);
                _db.SaveChanges();
                lotEvaluationByDocument.LotEvaluationByDocumentId = entity.LotEvaluationByDocumentId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot evaluation by document", "Success", User.Identity.Name, $"created evaluation document {entity.Document} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotEvaluationByDocument }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotEvaluationByDocuments_Update([DataSourceRequest]DataSourceRequest request, LotEvaluationByDocument lotEvaluationByDocument)
        {
            var pParticipatingSuppliers = _db.BidPParticipatingSupplierses.Find(int.Parse(lotEvaluationByDocument.SupplierName));//name has purchase id value
            if (pParticipatingSuppliers != null)
            {
                lotEvaluationByDocument.SupplierId = pParticipatingSuppliers.SupplierId;
                lotEvaluationByDocument.IsNewSupplier = pParticipatingSuppliers.IsNewSupplier;
                lotEvaluationByDocument.SupplierName = pParticipatingSuppliers.SupplierName;
            }
            if (ModelState.IsValid)
            {
                var entity = new LotEvaluationByDocument
                {
                    LotID = lotEvaluationByDocument.LotID,
                    LotEvaluationByDocumentId = lotEvaluationByDocument.LotEvaluationByDocumentId,
                    SupplierId = lotEvaluationByDocument.SupplierId,
                    IsNewSupplier = lotEvaluationByDocument.IsNewSupplier,
                    SupplierName = lotEvaluationByDocument.SupplierName,
                    Document = lotEvaluationByDocument.Document,
                    IsAvailable = lotEvaluationByDocument.IsAvailable,
                    Remark = lotEvaluationByDocument.Remark,
                    OrgBranchName = lotEvaluationByDocument.OrgBranchName,
                    DateCreated = lotEvaluationByDocument.DateCreated,
                    LastModified = lotEvaluationByDocument.LastModified,
                    CreatedBy = lotEvaluationByDocument.CreatedBy,
                    ModifiedBy = lotEvaluationByDocument.ModifiedBy
                };

                _db.LotEvaluationByDocuments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot evaluation by document", "Success", User.Identity.Name, $"updated evaluation document {entity.Document} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotEvaluationByDocument }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotEvaluationByDocuments_Destroy([DataSourceRequest]DataSourceRequest request, LotEvaluationByDocument lotEvaluationByDocument)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotEvaluationByDocument
                {
                    LotID = lotEvaluationByDocument.LotID,
                    LotEvaluationByDocumentId = lotEvaluationByDocument.LotEvaluationByDocumentId,
                    SupplierId = lotEvaluationByDocument.SupplierId,
                    IsNewSupplier = lotEvaluationByDocument.IsNewSupplier,
                    SupplierName = lotEvaluationByDocument.SupplierName,
                    Document = lotEvaluationByDocument.Document,
                    IsAvailable = lotEvaluationByDocument.IsAvailable,
                    Remark = lotEvaluationByDocument.Remark,
                    OrgBranchName = lotEvaluationByDocument.OrgBranchName,
                    DateCreated = lotEvaluationByDocument.DateCreated,
                    LastModified = lotEvaluationByDocument.LastModified,
                    CreatedBy = lotEvaluationByDocument.CreatedBy,
                    ModifiedBy = lotEvaluationByDocument.ModifiedBy
                };

                _db.LotEvaluationByDocuments.Attach(entity);
                _db.LotEvaluationByDocuments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { lotEvaluationByDocument }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
