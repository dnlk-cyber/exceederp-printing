using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class CommunicationSourceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CommunicationSources_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CommunicationSource> communicationsources = db.CommunicationSources;
            DataSourceResult result = communicationsources.ToDataSourceResult(request, communicationSource => new {
                CommunicationSourceID = communicationSource.CommunicationSourceID,
                Reference = communicationSource.Reference,
                CommunicationType = communicationSource.CommunicationType,
                CommunicationCode = communicationSource.CommunicationCode,
                Remark = communicationSource.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationSources_Create([DataSourceRequest]DataSourceRequest request, CommunicationSource communicationSource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CommunicationSource
                {
                    Reference = communicationSource.Reference,
                    CommunicationType = communicationSource.CommunicationType,
                    CommunicationCode = communicationSource.CommunicationCode,
               ////     CreatedBy = User.Identity.Name,
                //    DateCreated = DateTime.Now,
                    Remark = communicationSource.Remark
                };

                db.CommunicationSources.Add(entity);
                db.SaveChanges();
                communicationSource.CommunicationSourceID = entity.CommunicationSourceID;
            }

            return Json(new[] { communicationSource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationSources_Update([DataSourceRequest]DataSourceRequest request, CommunicationSource communicationSource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CommunicationSource
                {
                    CommunicationSourceID = communicationSource.CommunicationSourceID,
                    Reference = communicationSource.Reference,
                    CommunicationType = communicationSource.CommunicationType,
                    CommunicationCode = communicationSource.CommunicationCode,
                    Remark = communicationSource.Remark
                };

                db.CommunicationSources.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { communicationSource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationSources_Destroy([DataSourceRequest]DataSourceRequest request, CommunicationSource communicationSource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CommunicationSource
                {
                    CommunicationSourceID = communicationSource.CommunicationSourceID,
                    Reference = communicationSource.Reference,
                    CommunicationType = communicationSource.CommunicationType,
                    CommunicationCode = communicationSource.CommunicationCode,
                    Remark = communicationSource.Remark
                };

                db.CommunicationSources.Attach(entity);
                db.CommunicationSources.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Communication Source number " + communicationSource.CommunicationSourceID);
		
            }

            return Json(new[] { communicationSource }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
