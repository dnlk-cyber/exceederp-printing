using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Guarantee;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Guarantee
{
    public class GuaranteeFileController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser, ProcurementRoles.ProcurementBondApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser, ProcurementRoles.ProcurementBondApprover)]
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            var record = db.GuaranteeFiles.Find(id);

            if (record != null)
                return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
            else
                return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        public ActionResult GuaranteeFiles_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<GuaranteeFile> guaranteefiles = db.GuaranteeFiles.Where(x => x.GuaranteeId == id);
            DataSourceResult result = guaranteefiles.ToDataSourceResult(request, guaranteeFile => new
            {
                GuaranteeFileId = guaranteeFile.GuaranteeFileId,
                FileName = guaranteeFile.FileName,
                Description = guaranteeFile.Description,
                FilePath = guaranteeFile.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeFiles_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file, int id)
        {
            var timestamp = DateTime.Now.Year.ToString() +
                            DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                            DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                            DateTime.Now.Second.ToString();
            if (file != null)
            {

                string subPath = "~/App_Data/Uploads/Procurement/Guarantee/";

                string fileName = "";

                foreach (var temp in file)
                {
                    fileName = Path.GetFileName(temp.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + timestamp + "_" + fileName);
                    temp.SaveAs(physicalPath);
                    db.GuaranteeFiles.Add(new GuaranteeFile()
                    {
                        FileName = Path.GetFileName(temp.FileName),
                        FilePath = physicalPath,
                        GuaranteeId = id
                    });
                }

                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "guarantee attachment", "Success", User.Identity.Name, $"created attachement  {fileName} under record {id} ", Modules.Procurement);
                #endregion

            }
            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeFiles_Update([DataSourceRequest]DataSourceRequest request, GuaranteeFile guaranteeFile)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeFile
                {
                    GuaranteeFileId = guaranteeFile.GuaranteeFileId,
                    FileName = guaranteeFile.FileName,
                    Description = guaranteeFile.Description,
                    FilePath = guaranteeFile.FilePath
                };

                db.GuaranteeFiles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { guaranteeFile }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementBondUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeFiles_Destroy([DataSourceRequest]DataSourceRequest request, GuaranteeFile guaranteeFile)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeFile
                {
                    GuaranteeFileId = guaranteeFile.GuaranteeFileId,
                    FileName = guaranteeFile.FileName,
                    Description = guaranteeFile.Description,
                    FilePath = guaranteeFile.FilePath
                };

                db.GuaranteeFiles.Attach(entity);
                db.GuaranteeFiles.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { guaranteeFile }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
