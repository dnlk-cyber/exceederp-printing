using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class MaintenanceCompanyDocumentTypesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MaintenanceCompanyDocumentTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MaintenanceCompanyDocumentType> companydocumenttypes = db.MaintenanceCompanyDocumentTypes;
            DataSourceResult result = companydocumenttypes.ToDataSourceResult(request, companyDocumentType => new {
                MaintenanceCompanyDocumentTypeId = companyDocumentType.MaintenanceCompanyDocumentTypeId,
                DocumentType = companyDocumentType.DocumentType
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaintenanceCompanyDocumentTypes_Create([DataSourceRequest]DataSourceRequest request, MaintenanceCompanyDocumentType companyDocumentType)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaintenanceCompanyDocumentType
                {
                    DocumentType = companyDocumentType.DocumentType
                };

                db.MaintenanceCompanyDocumentTypes.Add(entity);
                db.SaveChanges();
                companyDocumentType.MaintenanceCompanyDocumentTypeId = entity.MaintenanceCompanyDocumentTypeId;
            }

            return Json(new[] { companyDocumentType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaintenanceCompanyDocumentTypes_Update([DataSourceRequest]DataSourceRequest request, MaintenanceCompanyDocumentType companyDocumentType)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaintenanceCompanyDocumentType
                {
                    MaintenanceCompanyDocumentTypeId = companyDocumentType.MaintenanceCompanyDocumentTypeId,
                    DocumentType = companyDocumentType.DocumentType
                };

                db.MaintenanceCompanyDocumentTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { companyDocumentType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaintenanceCompanyDocumentTypes_Destroy([DataSourceRequest]DataSourceRequest request, MaintenanceCompanyDocumentType companyDocumentType)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaintenanceCompanyDocumentType
                {
                    MaintenanceCompanyDocumentTypeId = companyDocumentType.MaintenanceCompanyDocumentTypeId,
                    DocumentType = companyDocumentType.DocumentType
                };

                db.MaintenanceCompanyDocumentTypes.Attach(entity);
                db.MaintenanceCompanyDocumentTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { companyDocumentType }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
