using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;


namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    [AuthorizeRoles(ProcurementRoles.ProcurementAdmin)]

    public class ProcurementPoTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
   
        public ActionResult ProcurementPOTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProcurementPOType> procurementpotypes = db.ProcurementPOTypes;
            DataSourceResult result = procurementpotypes.ToDataSourceResult(request, procurementPOType => new {
                ProcurementPOTypeID = procurementPOType.ProcurementPOTypeID,
                POType = procurementPOType.POType,
                AccountCode = procurementPOType.AccountCode,
                
                OrgBranchName = procurementPOType.OrgBranchName,
                DateCreated = procurementPOType.DateCreated,
                LastModified = procurementPOType.LastModified,
                CreatedBy = procurementPOType.CreatedBy,
                ModifiedBy = procurementPOType.ModifiedBy
            });

            return Json(result);
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcurementPOTypes_Create([DataSourceRequest]DataSourceRequest request, ProcurementPOType procurementPOType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcurementPOType
                {
                    POType = procurementPOType.POType,
                    AccountCode = procurementPOType.AccountCode,
                    OrgBranchName = procurementPOType.OrgBranchName,
                    DateCreated = procurementPOType.DateCreated,
                    LastModified = procurementPOType.LastModified,
                    CreatedBy = procurementPOType.CreatedBy,
                    ModifiedBy = procurementPOType.ModifiedBy
                };

                db.ProcurementPOTypes.Add(entity);
                db.SaveChanges();
                procurementPOType.ProcurementPOTypeID = entity.ProcurementPOTypeID;
            }

            return Json(new[] { procurementPOType }.ToDataSourceResult(request, ModelState));
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcurementPOTypes_Update([DataSourceRequest]DataSourceRequest request, ProcurementPOType procurementPOType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcurementPOType
                {
                    ProcurementPOTypeID = procurementPOType.ProcurementPOTypeID,
                    POType = procurementPOType.POType,
                    AccountCode = procurementPOType.AccountCode,
                    
                    OrgBranchName = procurementPOType.OrgBranchName,
                    DateCreated = procurementPOType.DateCreated,
                    LastModified = procurementPOType.LastModified,
                    CreatedBy = procurementPOType.CreatedBy,
                    ModifiedBy = procurementPOType.ModifiedBy
                };

                db.ProcurementPOTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { procurementPOType }.ToDataSourceResult(request, ModelState));
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcurementPOTypes_Destroy([DataSourceRequest]DataSourceRequest request, ProcurementPOType procurementPOType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcurementPOType
                {
                    ProcurementPOTypeID = procurementPOType.ProcurementPOTypeID,
                    POType = procurementPOType.POType,
                    AccountCode = procurementPOType.AccountCode,
                    
                    OrgBranchName = procurementPOType.OrgBranchName,
                    DateCreated = procurementPOType.DateCreated,
                    LastModified = procurementPOType.LastModified,
                    CreatedBy = procurementPOType.CreatedBy,
                    ModifiedBy = procurementPOType.ModifiedBy
                };

                db.ProcurementPOTypes.Attach(entity);
                db.ProcurementPOTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { procurementPOType }.ToDataSourceResult(request, ModelState));
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
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
