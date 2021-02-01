using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{


    public class ContractPaymentDocumentController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            ContractPaymentAttachment record = _db.ContractPaymentAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult ContractPaymentAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ContractPaymentAttachment> contractPaymentAttachments = _db.ContractPaymentAttachments.Where(x => x.ContractPaymentId == id);
            DataSourceResult result = contractPaymentAttachments.ToDataSourceResult(request, contractPaymentAttachement => new
            {
                ContractPaymentAttachmentId = contractPaymentAttachement.ContractPaymentAttachmentId,
                ContractPaymentId = contractPaymentAttachement.ContractPaymentId,
                FileName = contractPaymentAttachement.FileName,
                FilePath = contractPaymentAttachement.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentAttachments_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> contractPaymentDocuments, int id)
        {
            if (contractPaymentDocuments != null)
            {

                string subPath = "~/App_Data/Contract Payment Document/" + "Contract" + id.ToString();

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                string fileName = "";
                foreach (var file in contractPaymentDocuments)
                {
                    fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);

                    _db.ContractPaymentAttachments.Add(new ContractPaymentAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        ContractPaymentId = id,
                        FilePath = physicalPath

                    });

                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract payment attachment", "Success", User.Identity.Name, $"created attachement  {fileName} under record {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractPaymentDocuments }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentAttachments_Update([DataSourceRequest]DataSourceRequest request, TenderMinuteUploadDBModel tenderMinuteUploadDbModel)
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

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentAttachments_Destroy([DataSourceRequest]DataSourceRequest request, ContractPaymentAttachment contractPaymentAttachement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractPaymentAttachment
                {
                    ContractPaymentAttachmentId = contractPaymentAttachement.ContractPaymentAttachmentId,
                    ContractPaymentId = contractPaymentAttachement.ContractPaymentId,
                    FileName = contractPaymentAttachement.FileName,
                    FilePath = contractPaymentAttachement.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.ContractPaymentAttachments.Attach(entity);
                _db.ContractPaymentAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { contractPaymentAttachement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
