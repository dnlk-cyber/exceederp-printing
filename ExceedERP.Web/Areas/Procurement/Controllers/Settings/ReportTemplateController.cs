using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Settings
{
   
    [AuthorizeRoles(ProcurementRoles.ProcurementAdmin )]
    public class ReportTemplateController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index() 
        {
            return View();
        }
     
        public ActionResult ReportTemplates_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ReportTemplate> reporttemplates = _db.ReportTemplates;
            DataSourceResult result = reporttemplates.ToDataSourceResult(request, reportTemplate => new {
                ReportTemplateId = reportTemplate.ReportTemplateId,
                Temp1 = reportTemplate.Temp1,
                Temp2 = reportTemplate.Temp2,
                Temp3 = reportTemplate.Temp3,
                Temp4 = reportTemplate.Temp4,
                Temp5 = reportTemplate.Temp5,
                Temp6 = reportTemplate.Temp6,
                Temp7 = reportTemplate.Temp7,
                Type = reportTemplate.Type,
                Status = reportTemplate.Status
            });

            return Json(result);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReportTemplates_Create([DataSourceRequest]DataSourceRequest request, ReportTemplate reportTemplate)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReportTemplate
                {
                    Temp1 = reportTemplate.Temp1,
                    Temp2 = reportTemplate.Temp2,
                    Temp3 = reportTemplate.Temp3,
                    Temp4 = reportTemplate.Temp4,
                    Temp5 = reportTemplate.Temp5,
                    Temp6 = reportTemplate.Temp6,
                    Temp7 = reportTemplate.Temp7,
                    Type = reportTemplate.Type,
                    Status = reportTemplate.Status
                };

                _db.ReportTemplates.Add(entity);
                _db.SaveChanges();
                reportTemplate.ReportTemplateId = entity.ReportTemplateId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Report Template setup", "Success", User.Identity.Name, $"created template {entity.Type}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { reportTemplate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReportTemplates_Update([DataSourceRequest]DataSourceRequest request, ReportTemplate reportTemplate)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReportTemplate
                {
                    ReportTemplateId = reportTemplate.ReportTemplateId,
                    Temp1 = reportTemplate.Temp1,
                    Temp2 = reportTemplate.Temp2,
                    Temp3 = reportTemplate.Temp3,
                    Temp4 = reportTemplate.Temp4,
                    Temp5 = reportTemplate.Temp5,
                    Temp6 = reportTemplate.Temp6,
                    Temp7 = reportTemplate.Temp7,
                    Type = reportTemplate.Type,
                    Status = reportTemplate.Status
                };

                _db.ReportTemplates.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Report Template setup", "Success", User.Identity.Name, $"updated template {entity.Type}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { reportTemplate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReportTemplates_Destroy([DataSourceRequest]DataSourceRequest request, ReportTemplate reportTemplate)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReportTemplate
                {
                    ReportTemplateId = reportTemplate.ReportTemplateId,
                    Temp1 = reportTemplate.Temp1,
                    Temp2 = reportTemplate.Temp2,
                    Temp3 = reportTemplate.Temp3,
                    Temp4 = reportTemplate.Temp4,
                    Temp5 = reportTemplate.Temp5,
                    Temp6 = reportTemplate.Temp6,
                    Temp7 = reportTemplate.Temp7,
                    Type = reportTemplate.Type,
                    Status = reportTemplate.Status
                };

                _db.ReportTemplates.Attach(entity);
                _db.ReportTemplates.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { reportTemplate }.ToDataSourceResult(request, ModelState));
        }

        
      
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
