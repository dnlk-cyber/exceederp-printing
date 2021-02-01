using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{
   
    public class ProformaUploadsController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            ProformaUploadDbModel record = _db.ProformaUploadDbModels.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult ProformaUploadDbModels_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<ProformaUploadDbModel> proformaUploadDbModels = _db.ProformaUploadDbModels.Where(x => x.ProformaPurchaseID == id);
            DataSourceResult result = proformaUploadDbModels.ToDataSourceResult(request, proformaUploadDbModel => new {
                ProformaPurchaseID = proformaUploadDbModel.ProformaPurchaseID,
                ProformaUploadDbModelId = proformaUploadDbModel.ProformaUploadDbModelId,
                FileName = proformaUploadDbModel.FileName,
                FilePath = proformaUploadDbModel.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaUploadDbModels_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> proformaUploadDbModel , int id)
        {
            if (proformaUploadDbModel != null)
            {

                string subPath = "~/App_Data/Proforma/" + "Proforma" + id.ToString();  

                bool exists = Directory.Exists(Server.MapPath(subPath));
               
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                string fileName = "";
                 foreach (var file in proformaUploadDbModel)
                 {
                      fileName = Path.GetFileName(file.FileName);
                     string physicalPath = Server.MapPath(subPath +"/"+ fileName);
                  //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);
                    _db.ProformaUploadDbModels.Add(new ProformaUploadDbModel()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        ProformaPurchaseID = id,
                        FilePath = physicalPath
                        
                    });
                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma attachment", "Success", User.Identity.Name, $"created attachement  {fileName} under record {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaUploadDbModel }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaUploadDbModels_Update([DataSourceRequest]DataSourceRequest request, ProformaUploadDbModel proformaUploadDbModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaUploadDbModel
                {
                    ProformaUploadDbModelId = proformaUploadDbModel.ProformaUploadDbModelId,
                    FileName = proformaUploadDbModel.FileName,
                    FilePath = proformaUploadDbModel.FilePath
                };

                _db.ProformaUploadDbModels.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { proformaUploadDbModel }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaUploadDbModels_Destroy([DataSourceRequest]DataSourceRequest request, ProformaUploadDbModel proformaUploadDbModel)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new ProformaUploadDbModel
                {

                    ProformaUploadDbModelId = proformaUploadDbModel.ProformaUploadDbModelId,
                    ProformaPurchaseID = proformaUploadDbModel.ProformaPurchaseID,
                    FileName = proformaUploadDbModel.FileName,
                    FilePath = proformaUploadDbModel.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.ProformaUploadDbModels.Attach(entity);
                _db.ProformaUploadDbModels.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { proformaUploadDbModel }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
