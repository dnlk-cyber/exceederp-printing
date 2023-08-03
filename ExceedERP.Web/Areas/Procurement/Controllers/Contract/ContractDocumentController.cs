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


    public class ContractDocumentController : Controller
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
            ContractAttachment record = _db.ContractAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult ContractAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ContractAttachment> contractAttachments = _db.ContractAttachments.Where(x => x.ContractManagementID == id);
            DataSourceResult result = contractAttachments.ToDataSourceResult(request, contractAttachment => new
            {
                ContractAttachmentId = contractAttachment.ContractAttachmentId,
                ContractManagementID = contractAttachment.ContractManagementID,
                FileName = contractAttachment.FileName,
                FilePath = contractAttachment.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractAttachments_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> contractDocuments, int id)
        {
            if (contractDocuments != null)
            {

                string subPath = "~/App_Data/Contract Document/" + "Contract" + id.ToString();

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                string fileName = ""
;                foreach (var file in contractDocuments)
                {
                    fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);

                    _db.ContractAttachments.Add(new ContractAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        ContractManagementID = id,
                        FilePath = physicalPath

                    });

                }
              
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract document attachment", "Success", User.Identity.Name, $"created attachement  {fileName} under record {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractDocuments }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractAttachments_Update([DataSourceRequest]DataSourceRequest request, TenderMinuteUploadDBModel tenderMinuteUploadDbModel)
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
        public ActionResult ContractAttachments_Destroy([DataSourceRequest]DataSourceRequest request, ContractAttachment contractAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractAttachment
                {
                    ContractAttachmentId = contractAttachment.ContractAttachmentId,
                    ContractManagementID = contractAttachment.ContractManagementID,
                    FileName = contractAttachment.FileName,
                    FilePath = contractAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.ContractAttachments.Attach(entity);
                _db.ContractAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { contractAttachment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
