
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.Templates;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Template
{
    public class TemplateController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/Template
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Template_Read([DataSourceRequest]DataSourceRequest request)
        {
            var templates = db.Templates.ToList();
            DataSourceResult result = templates.ToDataSourceResult(request, template => new
            {
                TemplateId = template.TemplateId,
                TempOne = template.TempOne,
                TemplateType = template.TemplateType
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Template_Create([DataSourceRequest]DataSourceRequest request, ExceedERP.Core.Domain.HRM.Templates.Template template)
        {

            if (ModelState.IsValid)
            {
                var entity = new ExceedERP.Core.Domain.HRM.Templates.Template
                {
                    TempOne = template.TempOne,
                    TemplateType = template.TemplateType
                };

                db.Templates.Add(entity);
                db.SaveChanges();
                template.TemplateId = entity.TemplateId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Template", "Success", User.Identity.Name, User.Identity.Name + " add new Template called " + entity.TempOne, Modules.HRM);

                #endregion

            }

            return Json(new[] { template }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Template_Update([DataSourceRequest]DataSourceRequest request, ExceedERP.Core.Domain.HRM.Templates.Template template)
        {
            if (ModelState.IsValid)
            {


                var entity = new ExceedERP.Core.Domain.HRM.Templates.Template
                {
                    TemplateId = template.TemplateId,
                    TempOne = template.TempOne,
                    TemplateType = template.TemplateType
                };


                db.Templates.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Template", "Success", User.Identity.Name, User.Identity.Name + " update the Template " + entity.TempOne, Modules.HRM);

                #endregion
            }

            return Json(new[] { template }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Template_Destroy([DataSourceRequest]DataSourceRequest request, ExceedERP.Core.Domain.HRM.Templates.Template template)
        {
            if (ModelState.IsValid)
            {

                var entity = new ExceedERP.Core.Domain.HRM.Templates.Template
                {
                    TemplateId = template.TemplateId,
                    TempOne = template.TempOne,
                    TemplateType = template.TemplateType
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + template.TemplateType);
                #endregion

                db.Templates.Attach(entity);
                db.Templates.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { template }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}