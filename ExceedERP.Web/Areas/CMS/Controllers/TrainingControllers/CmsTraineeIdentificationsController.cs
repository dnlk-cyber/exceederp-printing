using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CMS;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CMS.Controllers.TrainingControllers
{
    public class CmsTraineeIdentificationsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CmsTraineeIdentifications_Read([DataSourceRequest]DataSourceRequest request, int traineeId)
        {
            IQueryable<CmsTraineeIdentification> cmstraineeidentifications = db.CmsTraineeIdentifications.Where(i => i.CmsTraineeId == traineeId);
            DataSourceResult result = cmstraineeidentifications.ToDataSourceResult(request, cmsTraineeIdentification => new
            {
                CmsTraineeIdentificationId = cmsTraineeIdentification.CmsTraineeIdentificationId,
                CmsTraineeId = cmsTraineeIdentification.CmsTraineeId,
                IdentificationType = cmsTraineeIdentification.IdentificationType,
                IdNumber = cmsTraineeIdentification.IdNumber,
                Remark = cmsTraineeIdentification.Remark,
                DateOfIssue = cmsTraineeIdentification.DateOfIssue,
                DateOfExpiry = cmsTraineeIdentification.DateOfExpiry,

                DateCreated = cmsTraineeIdentification.DateCreated,
                LastModified = cmsTraineeIdentification.LastModified,
                CreatedBy = cmsTraineeIdentification.CreatedBy,
                ModifiedBy = cmsTraineeIdentification.ModifiedBy
            });

            return Json(result);
        }
        private bool IsDuplicateRecord(CmsTraineeIdentification cmsTraineeIdentification, int id)
        {
            return db.CmsTraineeIdentifications.Any(i => i.CmsTraineeId == id && i.IdentificationType == cmsTraineeIdentification.IdentificationType && i.CmsTraineeIdentificationId != cmsTraineeIdentification.CmsTraineeIdentificationId);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CmsTraineeIdentifications_Create([DataSourceRequest]DataSourceRequest request, CmsTraineeIdentification cmsTraineeIdentification, int traineeId)
        {
            if (IsDuplicateRecord(cmsTraineeIdentification, traineeId))
            {
                ModelState.AddModelError("", "This Method of Identification is registered already");
            }
            if (ModelState.IsValid)
            {
                var entity = new CmsTraineeIdentification
                {
                    CmsTraineeId = traineeId,
                    IdentificationType = cmsTraineeIdentification.IdentificationType,
                    IdNumber = cmsTraineeIdentification.IdNumber,
                    Remark = cmsTraineeIdentification.Remark,
                    DateOfIssue = cmsTraineeIdentification.DateOfIssue,
                    DateOfExpiry = cmsTraineeIdentification.DateOfExpiry,

                    DateCreated = DateTime.Now,
                    LastModified = cmsTraineeIdentification.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = cmsTraineeIdentification.ModifiedBy
                };

                db.CmsTraineeIdentifications.Add(entity);
                db.SaveChanges();
                cmsTraineeIdentification.CmsTraineeIdentificationId = entity.CmsTraineeIdentificationId;
            }

            return Json(new[] { cmsTraineeIdentification }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CmsTraineeIdentifications_Update([DataSourceRequest]DataSourceRequest request, CmsTraineeIdentification cmsTraineeIdentification)
        {
            if (IsDuplicateRecord(cmsTraineeIdentification, cmsTraineeIdentification.CmsTraineeId))
            {
                ModelState.AddModelError("", "This Method of Identification is registered already");
            }
            if (ModelState.IsValid)
            {
                var entity = new CmsTraineeIdentification
                {
                    CmsTraineeIdentificationId = cmsTraineeIdentification.CmsTraineeIdentificationId,
                    CmsTraineeId = cmsTraineeIdentification.CmsTraineeId,
                    IdentificationType = cmsTraineeIdentification.IdentificationType,
                    IdNumber = cmsTraineeIdentification.IdNumber,
                    Remark = cmsTraineeIdentification.Remark,
                    DateOfIssue = cmsTraineeIdentification.DateOfIssue,
                    DateOfExpiry = cmsTraineeIdentification.DateOfExpiry,

                    DateCreated = cmsTraineeIdentification.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = cmsTraineeIdentification.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.CmsTraineeIdentifications.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cmsTraineeIdentification }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CmsTraineeIdentifications_Destroy([DataSourceRequest]DataSourceRequest request, CmsTraineeIdentification cmsTraineeIdentification)
        {
            if (ModelState.IsValid)
            {
                var entity = new CmsTraineeIdentification
                {
                    CmsTraineeIdentificationId = cmsTraineeIdentification.CmsTraineeIdentificationId,
                    CmsTraineeId = cmsTraineeIdentification.CmsTraineeId,
                    IdentificationType = cmsTraineeIdentification.IdentificationType,
                    IdNumber = cmsTraineeIdentification.IdNumber,
                    Remark = cmsTraineeIdentification.Remark,
                    DateOfIssue = cmsTraineeIdentification.DateOfIssue,
                    DateOfExpiry = cmsTraineeIdentification.DateOfExpiry,

                    DateCreated = cmsTraineeIdentification.DateCreated,
                    LastModified = cmsTraineeIdentification.LastModified,
                    CreatedBy = cmsTraineeIdentification.CreatedBy,
                    ModifiedBy = cmsTraineeIdentification.ModifiedBy
                };

                db.CmsTraineeIdentifications.Attach(entity);
                db.CmsTraineeIdentifications.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cmsTraineeIdentification }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
