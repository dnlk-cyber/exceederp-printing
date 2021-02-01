using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderContractController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FileDwonload(int id)
        {
            ArSalesOrderContract record = _db.ArSalesOrderContracts.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        public ActionResult ArSalesOrderContracts_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderContract> arsalesordercontracts = _db.ArSalesOrderContracts.Where(x=>x.ArSalesOrderId==id);
            DataSourceResult result = arsalesordercontracts.ToDataSourceResult(request, arSalesOrderContract => new {
                ArSalesOrderContractId = arSalesOrderContract.ArSalesOrderContractId,
                ArSalesOrderId = arSalesOrderContract.ArSalesOrderId,
                FileName = arSalesOrderContract.FileName,
                FilePath = arSalesOrderContract.FilePath,
                OrgBranchName = arSalesOrderContract.OrgBranchName,
                DateCreated = arSalesOrderContract.DateCreated,
                LastModified = arSalesOrderContract.LastModified,
                CreatedBy = arSalesOrderContract.CreatedBy,
                ModifiedBy = arSalesOrderContract.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderContracts_Create(int id,[DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> contractupload)
        {
            if (contractupload != null)
            {
                string subPath = "~/App_Data/SalesOrder/" + "SalesOrder" + id.ToString();

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                foreach (var file in contractupload)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    file.SaveAs(physicalPath);
                    _db.ArSalesOrderContracts.Add(new ArSalesOrderContract()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        ArSalesOrderId = id,
                        FilePath = physicalPath,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    });

                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AR sales order contract item", "Success", User.Identity.Name, $"created broadcast item {fileName} under record {id} ", Modules.Finance);
                    #endregion
                }


            }

            return Json(new[] { contractupload }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderContracts_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderContract arSalesOrderContract)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderContract
                {
                    ArSalesOrderId=arSalesOrderContract.ArSalesOrderId,
                    ArSalesOrderContractId = arSalesOrderContract.ArSalesOrderContractId,
                    FileName = arSalesOrderContract.FileName,
                    FilePath = arSalesOrderContract.FilePath,
                    OrgBranchName = arSalesOrderContract.OrgBranchName,
                    DateCreated = arSalesOrderContract.DateCreated,
                    LastModified = arSalesOrderContract.LastModified,
                    CreatedBy = arSalesOrderContract.CreatedBy,
                    ModifiedBy = arSalesOrderContract.ModifiedBy
                };

                _db.ArSalesOrderContracts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderContract }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderContracts_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderContract arSalesOrderContract)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderContract
                {
                    ArSalesOrderContractId = arSalesOrderContract.ArSalesOrderContractId,
                    ArSalesOrderId = arSalesOrderContract.ArSalesOrderId,
                    FileName = arSalesOrderContract.FileName,
                    FilePath = arSalesOrderContract.FilePath,
                    OrgBranchName = arSalesOrderContract.OrgBranchName,
                    DateCreated = arSalesOrderContract.DateCreated,
                    LastModified = arSalesOrderContract.LastModified,
                    CreatedBy = arSalesOrderContract.CreatedBy,
                    ModifiedBy = arSalesOrderContract.ModifiedBy
                };

                _db.ArSalesOrderContracts.Attach(entity);
                _db.ArSalesOrderContracts.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderContract }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
