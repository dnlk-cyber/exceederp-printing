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


    public class BidCancellationDocumentUploadController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            TenderCancellationUploadDbModel record = _db.TenderCancellationUploadDbModels.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult TenderCancellationUploadDbModels_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TenderCancellationUploadDbModel> tenderCancellationUploadDbModels = _db.TenderCancellationUploadDbModels.Where(x => x.TenderCancellationId == id);
            DataSourceResult result = tenderCancellationUploadDbModels.ToDataSourceResult(request, tenderCancellationUploadDbModel => new
            {
                TenderCancellationUploadDbModelId = tenderCancellationUploadDbModel.TenderCancellationUploadDbModelId,
                TenderCancellationId = tenderCancellationUploadDbModel.TenderCancellationId,
                FileName = tenderCancellationUploadDbModel.FileName,
                FilePath = tenderCancellationUploadDbModel.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderCancellationUploadDbModels_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> bidCancellationDocuments, int id)
        {
            if (bidCancellationDocuments != null)
            {

                string subPath = "~/App_Data/Bid Cancellation/" + "Cancellation" + id.ToString();

                bool exists = Directory.Exists(Server.MapPath(subPath));
                string fileName = "";
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                foreach (var file in bidCancellationDocuments)
                {
                    fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);

                    _db.TenderCancellationUploadDbModels.Add(new TenderCancellationUploadDbModel()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        TenderCancellationId = id,
                        FilePath = physicalPath

                    });

                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender bid document", "Success", User.Identity.Name, $"created bid document attachment {fileName} under record {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { bidCancellationDocuments }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderCancellationUploadDbModels_Update([DataSourceRequest]DataSourceRequest request, TenderMinuteUploadDBModel tenderMinuteUploadDbModel)
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

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderCancellationUploadDbModels_Destroy([DataSourceRequest]DataSourceRequest request, TenderCancellationUploadDbModel tenderCancellationUploadDbModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderCancellationUploadDbModel
                {
                    TenderCancellationUploadDbModelId = tenderCancellationUploadDbModel.TenderCancellationUploadDbModelId,
                    TenderCancellationId = tenderCancellationUploadDbModel.TenderCancellationId,
                    FileName = tenderCancellationUploadDbModel.FileName,
                    FilePath = tenderCancellationUploadDbModel.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.TenderCancellationUploadDbModels.Attach(entity);
                _db.TenderCancellationUploadDbModels.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderCancellationUploadDbModel }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
