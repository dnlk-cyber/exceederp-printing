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
   
    public class TenderTemplateController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }
       
        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            TenderTemplateFiles record = _db.TenderTemplateFiles.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult TenderTemplateFiles_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TenderTemplateFiles> tendertemplatefiles = _db.TenderTemplateFiles;
            DataSourceResult result = tendertemplatefiles.ToDataSourceResult(request, tenderTemplateFiles => new {
                TenderTemplateFilesId = tenderTemplateFiles.TenderTemplateFilesId,
                FileName = tenderTemplateFiles.FileName,
                Description = tenderTemplateFiles.Description,
                FilePath = tenderTemplateFiles.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderTemplateFiles_Create([DataSourceRequest] DataSourceRequest request,IEnumerable<HttpPostedFileBase> file)
            {
            if (file != null)
                {

                string subPath = "~/App_Data/Tender Template/" ;
                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                string fileName = "";

                    foreach (var temp in file)
                        {
                            fileName = Path.GetFileName(temp.FileName);
                            string physicalPath = Server.MapPath(subPath + "/" + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            temp.SaveAs(physicalPath);
                            _db.TenderTemplateFiles.Add(new TenderTemplateFiles()
                                {
                                    FileName = Path.GetFileName(temp.FileName),

                                    FilePath = physicalPath

                                });
                        }

                    _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender template", "Success", User.Identity.Name, $"uploaded tender template {fileName} ", Modules.Procurement);
                #endregion

            }
            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

            }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderTemplateFiles_Update([DataSourceRequest]DataSourceRequest request, TenderTemplateFiles tenderTemplateFiles)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderTemplateFiles
                {
                    TenderTemplateFilesId = tenderTemplateFiles.TenderTemplateFilesId,
                    FileName = tenderTemplateFiles.FileName,
                    Description = tenderTemplateFiles.Description,
                    FilePath = tenderTemplateFiles.FilePath
                };

                _db.TenderTemplateFiles.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { tenderTemplateFiles }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderTemplateFiles_Destroy([DataSourceRequest]DataSourceRequest request, TenderTemplateFiles tenderTemplateFiles)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderTemplateFiles
                {
                    TenderTemplateFilesId = tenderTemplateFiles.TenderTemplateFilesId,
                    FileName = tenderTemplateFiles.FileName,
                    Description = tenderTemplateFiles.Description,
                    FilePath = tenderTemplateFiles.FilePath
                };

                _db.TenderTemplateFiles.Attach(entity);
                _db.TenderTemplateFiles.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderTemplateFiles }.ToDataSourceResult(request, ModelState));
        }
         [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderTemplatedDownload(string id )
            {
                return File(id.ToString(), "application/force-download", Path.GetFileName(id.ToString()));

            }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
