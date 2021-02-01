using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using System;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{
    
     [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
    public class DisciplineRecordController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
           
            GetMeasureDetail();
            GetMeasure();
            return View();
        }
        public void GetMeasureDetail()
        {
            var measureDetails = db.DisciplineMeasuresDetail;
            ViewData["MeasuresDetails"] = measureDetails;
           
        }
        public void GetMeasure()
        {
           
            var measurs = db.DisciplineMeasures;
            ViewData["Measures"] = measurs;
        }
        public ActionResult DisciplineRecord_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DisciplineRecord> disciplinerecord = db.DisciplineRecord;
            DataSourceResult result = disciplinerecord.ToDataSourceResult(request, disciplineRecord => new {
                RecordId = disciplineRecord.RecordId,
                
                DetailMeasuresId = disciplineRecord.DetailMeasuresId,
                MeasuresId = disciplineRecord.MeasuresId,
                Date = disciplineRecord.Date,
                Remark = disciplineRecord.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineRecord_Create([DataSourceRequest]DataSourceRequest request, DisciplineRecord disciplineRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new DisciplineRecord
                {
                   
                    DetailMeasuresId = disciplineRecord.DetailMeasuresId,
                    MeasuresId = disciplineRecord.MeasuresId,
                    Date = disciplineRecord.Date,
                    Remark = disciplineRecord.Remark
                };

                db.DisciplineRecord.Add(entity);
                db.SaveChanges();
                disciplineRecord.RecordId = entity.RecordId;

                #region operation log

                //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplineRecord", "Success", User.Identity.Name, User.Identity.Name + " add displine recored at "+DateTime.Today, Modules.HRM);

                #endregion
            }

            return Json(new[] { disciplineRecord }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineRecord_Update([DataSourceRequest]DataSourceRequest request, DisciplineRecord disciplineRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new DisciplineRecord
                {
                    RecordId = disciplineRecord.RecordId,
                   
                    DetailMeasuresId = disciplineRecord.DetailMeasuresId,
                    MeasuresId = disciplineRecord.MeasuresId,
                    Date = disciplineRecord.Date,
                    Remark = disciplineRecord.Remark
                };

                db.DisciplineRecord.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplineRecord", "Success", User.Identity.Name, User.Identity.Name + " update displine recored at " + DateTime.Today, Modules.HRM);

                #endregion
            }

            return Json(new[] { disciplineRecord }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineRecord_Destroy([DataSourceRequest]DataSourceRequest request, DisciplineRecord disciplineRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new DisciplineRecord
                {
                    RecordId = disciplineRecord.RecordId,
                  
                    DetailMeasuresId = disciplineRecord.DetailMeasuresId,
                    MeasuresId = disciplineRecord.MeasuresId,
                    Date = disciplineRecord.Date,
                    Remark = disciplineRecord.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted discipline record of measurement Id " + disciplineRecord.MeasuresId);
                #endregion
                db.DisciplineRecord.Attach(entity);
                db.DisciplineRecord.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { disciplineRecord }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
