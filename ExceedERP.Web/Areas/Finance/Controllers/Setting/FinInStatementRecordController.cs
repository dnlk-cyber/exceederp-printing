using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    public class FinInStatementRecordController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FinInStatementRecords_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FinInStatementRecord> fininstatementrecords = _db.FinInStatementRecords;
            DataSourceResult result = fininstatementrecords.ToDataSourceResult(request, finInStatementRecord => new
            {
                FinInStatementRecordID = finInStatementRecord.FinInStatementRecordID,
                FYear = finInStatementRecord.FYear,
                Status = finInStatementRecord.Status,
                OrgBranchName = finInStatementRecord.OrgBranchName,
                DateCreated = finInStatementRecord.DateCreated,
                LastModified = finInStatementRecord.LastModified,
                CreatedBy = finInStatementRecord.CreatedBy,
                ModifiedBy = finInStatementRecord.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinInStatementRecords_Create([DataSourceRequest]DataSourceRequest request, FinInStatementRecord finInStatementRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinInStatementRecord
                {
                    FYear = finInStatementRecord.FYear,
                    Status = finInStatementRecord.Status,
                    OrgBranchName = finInStatementRecord.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = finInStatementRecord.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = finInStatementRecord.ModifiedBy
                };

                _db.FinInStatementRecords.Add(entity);
                _db.SaveChanges();
                finInStatementRecord.FinInStatementRecordID = entity.FinInStatementRecordID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance income statement", "Success", User.Identity.Name, $"created income statement year {entity.FYear}", Modules.Finance);
                #endregion
            }

            return Json(new[] { finInStatementRecord }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinInStatementRecords_Update([DataSourceRequest]DataSourceRequest request, FinInStatementRecord finInStatementRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinInStatementRecord
                {
                    FinInStatementRecordID = finInStatementRecord.FinInStatementRecordID,
                    FYear = finInStatementRecord.FYear,
                    Status = finInStatementRecord.Status,
                    OrgBranchName = finInStatementRecord.OrgBranchName,
                    DateCreated = finInStatementRecord.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = finInStatementRecord.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.FinInStatementRecords.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance income statement", "Success", User.Identity.Name, $"updated income statement year {entity.FYear}", Modules.Finance);
                #endregion
            }

            return Json(new[] { finInStatementRecord }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinInStatementRecords_Destroy([DataSourceRequest]DataSourceRequest request, FinInStatementRecord finInStatementRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinInStatementRecord
                {
                    FinInStatementRecordID = finInStatementRecord.FinInStatementRecordID,
                    FYear = finInStatementRecord.FYear,
                    Status = finInStatementRecord.Status,
                    OrgBranchName = finInStatementRecord.OrgBranchName,
                    DateCreated = finInStatementRecord.DateCreated,
                    LastModified = finInStatementRecord.LastModified,
                    CreatedBy = finInStatementRecord.CreatedBy,
                    ModifiedBy = finInStatementRecord.ModifiedBy
                };

                _db.FinInStatementRecords.Attach(entity);
                _db.FinInStatementRecords.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { finInStatementRecord }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
