using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class AnnouncementDetailController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AnnouncmentDetails_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<AnnouncmentDetail> announcmentdetails = db.AnnouncmentDetails.Where (x=> x.AnnouncmentId == id);
            DataSourceResult result = announcmentdetails.ToDataSourceResult(request, announcmentDetail => new {
                AnnouncDetailId = announcmentDetail.AnnouncDetailId,
                Detail = announcmentDetail.Detail,
                Remark = announcmentDetail.Remark,
                OrgBranchName = announcmentDetail.OrgBranchName,
                DateCreated = announcmentDetail.DateCreated,
                LastModified = announcmentDetail.LastModified,
                CreatedBy = announcmentDetail.CreatedBy,
                ModifiedBy = announcmentDetail.ModifiedBy,
                AnnouncmentId = announcmentDetail.AnnouncmentId 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnouncmentDetails_Create([DataSourceRequest]DataSourceRequest request, AnnouncmentDetail announcmentDetail, int idTwo)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new AnnouncmentDetail
                {
                    Detail = announcmentDetail.Detail,
                    Remark = announcmentDetail.Remark,
                    OrgBranchName = announcmentDetail.OrgBranchName,
                    DateCreated =DateTime .Now ,
                    LastModified = announcmentDetail.LastModified,
                    CreatedBy = User .Identity .Name,
                    ModifiedBy = announcmentDetail.ModifiedBy,
                    AnnouncmentId = idTwo 
                };

                db.AnnouncmentDetails.Add(entity);
                db.SaveChanges();
                announcmentDetail.AnnouncDetailId = entity.AnnouncDetailId;
            }

            return Json(new[] { announcmentDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnouncmentDetails_Update([DataSourceRequest]DataSourceRequest request, AnnouncmentDetail announcmentDetail)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new AnnouncmentDetail
                {
                    AnnouncDetailId = announcmentDetail.AnnouncDetailId,
                    Detail = announcmentDetail.Detail,
                    Remark = announcmentDetail.Remark,
                    OrgBranchName = announcmentDetail.OrgBranchName,
                    DateCreated = announcmentDetail.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = announcmentDetail.CreatedBy,
                    ModifiedBy = User.Identity .Name ,
                    AnnouncmentId = announcmentDetail.AnnouncmentId 
                };

                db.AnnouncmentDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { announcmentDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnouncmentDetails_Destroy([DataSourceRequest]DataSourceRequest request, AnnouncmentDetail announcmentDetail)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new AnnouncmentDetail
                {
                    AnnouncDetailId = announcmentDetail.AnnouncDetailId,
                    Detail = announcmentDetail.Detail,
                    Remark = announcmentDetail.Remark,
                    OrgBranchName = announcmentDetail.OrgBranchName,
                    DateCreated = announcmentDetail.DateCreated,
                    LastModified = announcmentDetail.LastModified,
                    CreatedBy = announcmentDetail.CreatedBy,
                    ModifiedBy = announcmentDetail.ModifiedBy,
                    AnnouncmentId = announcmentDetail.AnnouncmentId 
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted detail announcment of announcment" + announcmentDetail.Announcment.Title );
                #endregion
                db.AnnouncmentDetails.Attach(entity);
                db.AnnouncmentDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { announcmentDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
