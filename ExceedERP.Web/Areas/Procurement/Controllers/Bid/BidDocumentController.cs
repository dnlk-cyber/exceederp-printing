using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{


    public class BidDocumentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            TenderBidDocument record = _db.TenderBidDocuments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult TenderBidDocuments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TenderBidDocument> tenderbiddocuments = _db.TenderBidDocuments.Where(x => x.TenderID == id);
            DataSourceResult result = tenderbiddocuments.ToDataSourceResult(request, tenderBidDocument => new
            {
                TenderBidDocumentId = tenderBidDocument.TenderBidDocumentId,
                TenderID = tenderBidDocument.TenderID,
                FileName = tenderBidDocument.FileName,
                FilePath = tenderBidDocument.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderBidDocuments_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> bidDocuments, int id)
        {
            if (bidDocuments != null)
            {

                string subPath = "~/App_Data/Bid Document/" + "Tender" + id.ToString();
                string fileName = "";

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                foreach (var file in bidDocuments)
                {
                    fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);

                    _db.TenderBidDocuments.Add(new TenderBidDocument()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        TenderID = id,
                        FilePath = physicalPath

                    });

                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender bid document", "Success", User.Identity.Name, $"created bid document attachment {fileName} under record {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { bidDocuments }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMinuteUploads_Update([DataSourceRequest]DataSourceRequest request, TenderMinuteUploadDBModel tenderMinuteUploadDbModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderMinuteUploadDBModel
                {
                    TenderMinuteUploadDBModelId = tenderMinuteUploadDbModel.TenderMinuteUploadDBModelId,
                    FileName = tenderMinuteUploadDbModel.FileName,
                    FilePath = tenderMinuteUploadDbModel.FilePath
                };

                _db.TenderMinuteUploads.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { tenderMinuteUploadDbModel }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderBidDocuments_Destroy([DataSourceRequest]DataSourceRequest request, TenderBidDocument tenderBidDocument)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderBidDocument
                {
                    TenderBidDocumentId = tenderBidDocument.TenderBidDocumentId,
                    TenderID = tenderBidDocument.TenderID,
                    FileName = tenderBidDocument.FileName,
                    FilePath = tenderBidDocument.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.TenderBidDocuments.Attach(entity);
                _db.TenderBidDocuments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderBidDocument }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
