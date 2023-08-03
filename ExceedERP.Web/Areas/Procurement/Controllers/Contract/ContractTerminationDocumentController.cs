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


    public class ContractTerminationDocumentController : Controller
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
            ContractTerminationAttachment record = _db.ContractTerminationAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult ContractTerminationAttachments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ContractTerminationAttachment> contractTerminationAttachments = _db.ContractTerminationAttachments.Where(x => x.ContractTerminationId == id);
            DataSourceResult result = contractTerminationAttachments.ToDataSourceResult(request, contractTerminationAttachement => new
            {
                ContractTerminationAttachmentId = contractTerminationAttachement.ContractTerminationAttachmentId,
                ContractTerminationId = contractTerminationAttachement.ContractTerminationId,
                FileName = contractTerminationAttachement.FileName,
                FilePath = contractTerminationAttachement.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractTerminationAttachments_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> contractTerminationDocuments, int id)
        {
            if (contractTerminationDocuments != null)
            {

                string subPath = "~/App_Data/Contract Termination Document/" + "Contract" + id.ToString();

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                string fileName = "";
                foreach (var file in contractTerminationDocuments)
                {
                     fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);

                    _db.ContractTerminationAttachments.Add(new ContractTerminationAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        ContractTerminationId = id,
                        FilePath = physicalPath

                    });

                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract termination attachment", "Success", User.Identity.Name, $"created attachement  {fileName} under record {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractTerminationDocuments }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractTerminationAttachments_Update([DataSourceRequest]DataSourceRequest request, TenderMinuteUploadDBModel tenderMinuteUploadDbModel)
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
        public ActionResult ContractTerminationAttachments_Destroy([DataSourceRequest]DataSourceRequest request, ContractTerminationAttachment contractTerminationAttachement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractTerminationAttachment
                {
                    ContractTerminationAttachmentId = contractTerminationAttachement.ContractTerminationAttachmentId,
                    ContractTerminationId = contractTerminationAttachement.ContractTerminationId,
                    FileName = contractTerminationAttachement.FileName,
                    FilePath = contractTerminationAttachement.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.ContractTerminationAttachments.Attach(entity);
                _db.ContractTerminationAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { contractTerminationAttachement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
