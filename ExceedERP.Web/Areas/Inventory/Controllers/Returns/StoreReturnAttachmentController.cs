using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Returns
{


    public class StoreReturnAttachmentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            StoreReturnAttachment record = _db.StoreReturnAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult StoreReturnAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<StoreReturnAttachment> StoreReturnAttachments = _db.StoreReturnAttachments.Where(x => x.StoreReturnID == id);
            DataSourceResult result = StoreReturnAttachments.ToDataSourceResult(request, StoreReturnAttachment => new
            {
                StoreReturnAttachmentId = StoreReturnAttachment.StoreReturnAttachmentId,
                StoreReturnID = StoreReturnAttachment.StoreReturnID,
                FileName = StoreReturnAttachment.FileName,
                FilePath = StoreReturnAttachment.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnAttachments_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> StoreReturnAttachments, int id)
        {
            if (StoreReturnAttachments != null)
            {

                string subPath = "~/App_Data/Inventory/" + "StoreRetun" + id.ToString();

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                foreach (var file in StoreReturnAttachments)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);

                    _db.StoreReturnAttachments.Add(new StoreReturnAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        StoreReturnID = id,
                        FilePath = physicalPath

                    });

                }

                _db.SaveChanges();
            }

            return Json(new[] { StoreReturnAttachments }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
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

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturnAttachments_Destroy([DataSourceRequest]DataSourceRequest request, StoreReturnAttachment StoreReturnAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreReturnAttachment
                {
                    StoreReturnAttachmentId = StoreReturnAttachment.StoreReturnAttachmentId,
                    StoreReturnID = StoreReturnAttachment.StoreReturnID,
                    FileName = StoreReturnAttachment.FileName,
                    FilePath = StoreReturnAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.StoreReturnAttachments.Attach(entity);
                _db.StoreReturnAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { StoreReturnAttachment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
