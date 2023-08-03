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
    public class SupplierBlackFileController : Controller
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
            var record = db.SupplierBlackFiles.Find(id);

            if (record != null)
                return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
            else
                return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult SupplierBlackFiles_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<SupplierBlackFile> supplierBlackFiles = db.SupplierBlackFiles.Where(x=>x.SupplierBlackFileId==id);
            DataSourceResult result = supplierBlackFiles.ToDataSourceResult(request, supplierBlackFile => new {
                SupplierBlackFileId = supplierBlackFile.SupplierBlackFileId,
                FileName = supplierBlackFile.FileName,
                Description = supplierBlackFile.Description,
                FilePath = supplierBlackFile.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierBlackFiles_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file, int id)
        {
            var timestamp = DateTime.Now.Year.ToString() +
                            DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                            DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                            DateTime.Now.Second.ToString();
            if (file != null)
            {

                string subPath = "~/App_Data/Uploads/Procurement/SupplierBlackList/";


                string fileName = "";
                foreach (var temp in file)
                {
                    fileName = Path.GetFileName(temp.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + timestamp + "_" + fileName);
                    temp.SaveAs(physicalPath);
                    db.SupplierBlackFiles.Add(new SupplierBlackFile()
                    {
                        FileName = Path.GetFileName(temp.FileName),
                        FilePath = physicalPath,
                        SupplierBlackListId= id
                    });
                }

                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "supplier black list attachment", "Success", User.Identity.Name, $"created attachement  {fileName} under record {id} ", Modules.Procurement);
                #endregion

            }
            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierBlackFiles_Update([DataSourceRequest]DataSourceRequest request, SupplierBlackFile supplierBlackFile)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierBlackFile
                {
                    SupplierBlackListId = supplierBlackFile.SupplierBlackListId,
                    SupplierBlackFileId = supplierBlackFile.SupplierBlackFileId,
                    FileName = supplierBlackFile.FileName,
                    Description = supplierBlackFile.Description,
                    FilePath = supplierBlackFile.FilePath
                };

                db.SupplierBlackFiles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { supplierBlackFile }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierBlackFiles_Destroy([DataSourceRequest]DataSourceRequest request, SupplierBlackFile supplierBlackFile)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierBlackFile
                {
                    SupplierBlackListId = supplierBlackFile.SupplierBlackListId,
                    SupplierBlackFileId = supplierBlackFile.SupplierBlackFileId,
                    FileName = supplierBlackFile.FileName,
                    Description = supplierBlackFile.Description,
                    FilePath = supplierBlackFile.FilePath
                };

                db.SupplierBlackFiles.Attach(entity);
                db.SupplierBlackFiles.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplierBlackFile }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
