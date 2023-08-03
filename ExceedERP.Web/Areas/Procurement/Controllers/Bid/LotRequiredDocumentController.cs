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
    public class LotRequiredDocumentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult LotRequiredDocuments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LotRequiredDocument> lotrequireddocuments = _db.LotRequiredDocuments;
            DataSourceResult result = lotrequireddocuments.ToDataSourceResult(request, lotRequiredDocument => new
            {
                LotRequiredDocumentId = lotRequiredDocument.LotRequiredDocumentId,
                LotID = lotRequiredDocument.LotID,
                Document = lotRequiredDocument.Document,
                Mandatory = lotRequiredDocument.Mandatory,
                Remark = lotRequiredDocument.Remark,
                OrgBranchName = lotRequiredDocument.OrgBranchName,
                DateCreated = lotRequiredDocument.DateCreated,
                LastModified = lotRequiredDocument.LastModified,
                CreatedBy = lotRequiredDocument.CreatedBy,
                ModifiedBy = lotRequiredDocument.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotRequiredDocuments_Create([DataSourceRequest]DataSourceRequest request, LotRequiredDocument lotRequiredDocument)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotRequiredDocument
                {
                    LotID = lotRequiredDocument.LotID,
                    Document = lotRequiredDocument.Document,
                    Mandatory = lotRequiredDocument.Mandatory,
                    Remark = lotRequiredDocument.Remark,
                    OrgBranchName = lotRequiredDocument.OrgBranchName,
                    DateCreated = lotRequiredDocument.DateCreated,
                    LastModified = lotRequiredDocument.LastModified,
                    CreatedBy = lotRequiredDocument.CreatedBy,
                    ModifiedBy = lotRequiredDocument.ModifiedBy
                };

                _db.LotRequiredDocuments.Add(entity);
                _db.SaveChanges();
                lotRequiredDocument.LotRequiredDocumentId = entity.LotRequiredDocumentId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot required document", "Success", User.Identity.Name, $"created lot required document {entity.Document} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotRequiredDocument }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotRequiredDocuments_Update([DataSourceRequest]DataSourceRequest request, LotRequiredDocument lotRequiredDocument)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotRequiredDocument
                {
                    LotRequiredDocumentId = lotRequiredDocument.LotRequiredDocumentId,
                    LotID = lotRequiredDocument.LotID,
                    Document = lotRequiredDocument.Document,
                    Mandatory = lotRequiredDocument.Mandatory,
                    Remark = lotRequiredDocument.Remark,
                    OrgBranchName = lotRequiredDocument.OrgBranchName,
                    DateCreated = lotRequiredDocument.DateCreated,
                    LastModified = lotRequiredDocument.LastModified,
                    CreatedBy = lotRequiredDocument.CreatedBy,
                    ModifiedBy = lotRequiredDocument.ModifiedBy
                };

                _db.LotRequiredDocuments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot required document", "Success", User.Identity.Name, $"updated lot required document {entity.Document} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotRequiredDocument }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotRequiredDocuments_Destroy([DataSourceRequest]DataSourceRequest request, LotRequiredDocument lotRequiredDocument)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotRequiredDocument
                {
                    LotRequiredDocumentId = lotRequiredDocument.LotRequiredDocumentId,
                    LotID = lotRequiredDocument.LotID,
                    Document = lotRequiredDocument.Document,
                    Mandatory = lotRequiredDocument.Mandatory,
                    Remark = lotRequiredDocument.Remark,
                    OrgBranchName = lotRequiredDocument.OrgBranchName,
                    DateCreated = lotRequiredDocument.DateCreated,
                    LastModified = lotRequiredDocument.LastModified,
                    CreatedBy = lotRequiredDocument.CreatedBy,
                    ModifiedBy = lotRequiredDocument.ModifiedBy
                };

                _db.LotRequiredDocuments.Attach(entity);
                _db.LotRequiredDocuments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { lotRequiredDocument }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
