using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class EmployeeCodeSequenceController : Controller
    {
        // GET: HRM/EmployeeCodeSequence
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmployeeCodeSequence_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeeCodeSequenceSetting> employeeCodeSequenceSettings = db.EmployeeCodeSequenceSettings;
            DataSourceResult result = employeeCodeSequenceSettings.ToDataSourceResult(request, employeeCodeSequenceSetting => new
            {
                EmployeeCodeSequenceSettingId = employeeCodeSequenceSetting.EmployeeCodeSequenceSettingId,
                Name = employeeCodeSequenceSetting.Name,
                IndexCount = employeeCodeSequenceSetting.IndexCount,
                Status = employeeCodeSequenceSetting.Status,
                ItemCodeTrim = employeeCodeSequenceSetting.ItemCodeTrim
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCodeSequence_Create([DataSourceRequest]DataSourceRequest request, EmployeeCodeSequenceSetting employeeCodeSequenceSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new EmployeeCodeSequenceSetting
                {
                    Name = employeeCodeSequenceSetting.Name,
                    IndexCount = employeeCodeSequenceSetting.IndexCount,
                    Status = employeeCodeSequenceSetting.Status,
                    ItemCodeTrim = employeeCodeSequenceSetting.ItemCodeTrim
                };

                db.EmployeeCodeSequenceSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Employee Code Sequence Settings", "Success", User.Identity.Name, "Create Employee Code Sequence Setting " + entity.EmployeeCodeSequenceSettingId, Modules.HRM);

                #endregion



            }

            return Json(new[] { employeeCodeSequenceSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCodeSequence_Update([DataSourceRequest]DataSourceRequest request, EmployeeCodeSequenceSetting employeeCodeSequenceSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new EmployeeCodeSequenceSetting
                {
                    EmployeeCodeSequenceSettingId = employeeCodeSequenceSetting.EmployeeCodeSequenceSettingId,
                    Name = employeeCodeSequenceSetting.Name,
                    IndexCount = employeeCodeSequenceSetting.IndexCount,
                    Status = employeeCodeSequenceSetting.Status,
                    ItemCodeTrim = employeeCodeSequenceSetting.ItemCodeTrim
                };

                db.EmployeeCodeSequenceSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Employee Code Sequence Settings", "Success", User.Identity.Name, "Update Employee Code Sequence Setting " + entity.EmployeeCodeSequenceSettingId, Modules.HRM);

                #endregion

            }
            return Json(new[] { employeeCodeSequenceSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCodeSequence_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeCodeSequenceSetting employeeCodeSequenceSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeCodeSequenceSetting
                {
                    EmployeeCodeSequenceSettingId = employeeCodeSequenceSetting.EmployeeCodeSequenceSettingId,
                    Name = employeeCodeSequenceSetting.Name,
                    IndexCount = employeeCodeSequenceSetting.IndexCount,
                    Status = employeeCodeSequenceSetting.Status,
                    ItemCodeTrim = employeeCodeSequenceSetting.ItemCodeTrim
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Employee Code Sequence Setting " + entity.EmployeeCodeSequenceSettingId);
                #endregion
                db.EmployeeCodeSequenceSettings.Attach(entity);
                db.EmployeeCodeSequenceSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeCodeSequenceSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}