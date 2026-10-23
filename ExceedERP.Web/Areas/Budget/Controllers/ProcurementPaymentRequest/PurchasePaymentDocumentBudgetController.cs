using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.ProcurementPaymentRequest
{


    public class PurchasePaymentDocumentBudgetController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            PurchasePaymentAttachment record = _db.PurchasePaymentAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult PurchasePaymentAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchasePaymentAttachment> PurchasePaymentAttachments = _db.PurchasePaymentAttachments.Where(x => x.PurchasePaymentId == id);
            DataSourceResult result = PurchasePaymentAttachments.ToDataSourceResult(request, PurchasePaymentAttachement => new
            {
                PurchasePaymentAttachmentId = PurchasePaymentAttachement.PurchasePaymentAttachmentId,
                PurchasePaymentId = PurchasePaymentAttachement.PurchasePaymentId,
                FileName = PurchasePaymentAttachement.FileName,
                FilePath = PurchasePaymentAttachement.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentAttachments_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> PurchasePaymentDocuments, int id)
        {
            if (PurchasePaymentDocuments != null)
            {

                string subPath = "~/App_Data/Purchase Payment Document/" + "Purchase" + id.ToString();

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                string fileName = "";
                foreach (var file in PurchasePaymentDocuments)
                {
                    fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);

                    _db.PurchasePaymentAttachments.Add(new PurchasePaymentAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        PurchasePaymentId = id,
                        FilePath = physicalPath

                    });

                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment attachment", "Success", User.Identity.Name, $"created attachement  {fileName} under record {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { PurchasePaymentDocuments }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentAttachments_Update([DataSourceRequest]DataSourceRequest request, TenderMinuteUploadDBModel tenderMinuteUploadDbModel)
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

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentAttachments_Destroy([DataSourceRequest]DataSourceRequest request, PurchasePaymentAttachment PurchasePaymentAttachement)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentAttachment
                {
                    PurchasePaymentAttachmentId = PurchasePaymentAttachement.PurchasePaymentAttachmentId,
                    PurchasePaymentId = PurchasePaymentAttachement.PurchasePaymentId,
                    FileName = PurchasePaymentAttachement.FileName,
                    FilePath = PurchasePaymentAttachement.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.PurchasePaymentAttachments.Attach(entity);
                _db.PurchasePaymentAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { PurchasePaymentAttachement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
