using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.PurchaseRequisitions
{


    public class PurchaseRequisitionAttachmentController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            PurchaseRequisitionAttachment record = _db.PurchaseRequisitionAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult PurchaseRequisitionAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchaseRequisitionAttachment> purchaseRequisitionAttachments = _db.PurchaseRequisitionAttachments.Where(x => x.PurchaseRequisitionID == id);
            DataSourceResult result = purchaseRequisitionAttachments.ToDataSourceResult(request, purchaseRequisitionAttachment => new
            {
                PurchaseRequisitionAttachmentId = purchaseRequisitionAttachment.PurchaseRequisitionAttachmentId,
                PurchaseRequisitionID = purchaseRequisitionAttachment.PurchaseRequisitionID,
                FileName = purchaseRequisitionAttachment.FileName,
                FilePath = purchaseRequisitionAttachment.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionAttachments_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> PurchaseRequisitionAttachments, int id)
        {
            if (PurchaseRequisitionAttachments != null)
            {

                string subPath = "~/App_Data/Purchase Requisition/" + "PR" + id.ToString();

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                string fileName = "";
                foreach (var file in PurchaseRequisitionAttachments)
                {
                     fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);

                    _db.PurchaseRequisitionAttachments.Add(new PurchaseRequisitionAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        PurchaseRequisitionID = id,
                        FilePath = physicalPath

                    });

                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition atachment ", "Success", User.Identity.Name, $"created item {fileName} under record {id} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { PurchaseRequisitionAttachments }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionAttachments_Update([DataSourceRequest]DataSourceRequest request, TenderMinuteUploadDBModel tenderMinuteUploadDbModel)
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

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionAttachments_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionAttachment purchaseRequisitionAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionAttachment
                {
                    PurchaseRequisitionAttachmentId = purchaseRequisitionAttachment.PurchaseRequisitionAttachmentId,
                    PurchaseRequisitionID = purchaseRequisitionAttachment.PurchaseRequisitionID,
                    FileName = purchaseRequisitionAttachment.FileName,
                    FilePath = purchaseRequisitionAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.PurchaseRequisitionAttachments.Attach(entity);
                _db.PurchaseRequisitionAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { purchaseRequisitionAttachment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
