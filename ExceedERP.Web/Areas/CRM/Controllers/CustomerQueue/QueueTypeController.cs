using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMQueueSetting)]
    public class QueueTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QueueTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<QueueType> queuetypes = db.QueueTypes;
            DataSourceResult result = queuetypes.ToDataSourceResult(request, queueType => new {
                QueueTypeId = queueType.QueueTypeId,
                QueueName = queueType.QueueName,
                Description = queueType.Description,
                OrgBranchName = queueType.OrgBranchName,
                DateCreated = queueType.DateCreated,
                LastModified = queueType.LastModified,
                CreatedBy = queueType.CreatedBy,
                ModifiedBy = queueType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QueueTypes_Create([DataSourceRequest]DataSourceRequest request, QueueType queueType)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueType
                {
                    QueueName = queueType.QueueName,
                    Description = queueType.Description,
                    OrgBranchName = queueType.OrgBranchName,
                    DateCreated = queueType.DateCreated,
                    LastModified = queueType.LastModified,
                    CreatedBy = queueType.CreatedBy,
                    ModifiedBy = queueType.ModifiedBy
                };

                db.QueueTypes.Add(entity);
                db.SaveChanges();
                queueType.QueueTypeId = entity.QueueTypeId;
            }

            return Json(new[] { queueType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QueueTypes_Update([DataSourceRequest]DataSourceRequest request, QueueType queueType)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueType
                {
                    QueueTypeId = queueType.QueueTypeId,
                    QueueName = queueType.QueueName,
                    Description = queueType.Description,
                    OrgBranchName = queueType.OrgBranchName,
                    DateCreated = queueType.DateCreated,
                    LastModified = queueType.LastModified,
                    CreatedBy = queueType.CreatedBy,
                    ModifiedBy = queueType.ModifiedBy
                };

                db.QueueTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { queueType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QueueTypes_Destroy([DataSourceRequest]DataSourceRequest request, QueueType queueType)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueType
                {
                    QueueTypeId = queueType.QueueTypeId,
                    QueueName = queueType.QueueName,
                    Description = queueType.Description,
                    OrgBranchName = queueType.OrgBranchName,
                    DateCreated = queueType.DateCreated,
                    LastModified = queueType.LastModified,
                    CreatedBy = queueType.CreatedBy,
                    ModifiedBy = queueType.ModifiedBy
                };

                db.QueueTypes.Attach(entity);
                db.QueueTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { queueType }.ToDataSourceResult(request, ModelState));
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
