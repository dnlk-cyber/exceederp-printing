using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Suppliers
{
    public class SupplierListMinuteFileController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            var record = db.SupplierListMinuteFiles.Find(id);

            if (record != null)
                return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
            else
                return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult SupplierListMinuteFiles_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SupplierListMinuteFile> supplierlistminutefiles = db.SupplierListMinuteFiles;
            DataSourceResult result = supplierlistminutefiles.ToDataSourceResult(request, supplierListMinuteFile => new {
                SupplierListMinuteFileId = supplierListMinuteFile.SupplierListMinuteFileId,
                FileName = supplierListMinuteFile.FileName,
                Description = supplierListMinuteFile.Description,
                FilePath = supplierListMinuteFile.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierListMinuteFiles_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {
            var timestamp = DateTime.Now.Year.ToString() +
                            DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                            DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                            DateTime.Now.Second.ToString();
            if (file != null)
            {

                string subPath = "~/App_Data/Uploads/Procurement/SupplierListMoM/";

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                string fileName = "";
                foreach (var temp in file)
                {
                    fileName = Path.GetFileName(temp.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" +timestamp+"_" + fileName);
                    //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    temp.SaveAs(physicalPath);
                    db.SupplierListMinuteFiles.Add(new SupplierListMinuteFile()
                    {
                        FileName = Path.GetFileName(temp.FileName),
                        FilePath = physicalPath
                    });
                }

                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "supplier list attachment", "Success", User.Identity.Name, $"created attachement  {fileName}  ", Modules.Procurement);
                #endregion

            }
            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierListMinuteFiles_Update([DataSourceRequest]DataSourceRequest request, SupplierListMinuteFile supplierListMinuteFile)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierListMinuteFile
                {
                    SupplierListMinuteFileId = supplierListMinuteFile.SupplierListMinuteFileId,
                    FileName = supplierListMinuteFile.FileName,
                    Description = supplierListMinuteFile.Description,
                    FilePath = supplierListMinuteFile.FilePath
                };

                db.SupplierListMinuteFiles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { supplierListMinuteFile }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierListMinuteFiles_Destroy([DataSourceRequest]DataSourceRequest request, SupplierListMinuteFile supplierListMinuteFile)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierListMinuteFile
                {
                    SupplierListMinuteFileId = supplierListMinuteFile.SupplierListMinuteFileId,
                    FileName = supplierListMinuteFile.FileName,
                    Description = supplierListMinuteFile.Description,
                    FilePath = supplierListMinuteFile.FilePath
                };

                db.SupplierListMinuteFiles.Attach(entity);
                db.SupplierListMinuteFiles.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplierListMinuteFile }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
