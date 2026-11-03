using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class VacancyDetailsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Recruitment_Admin = "HR_Recruitment_Admin";
     
       
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult VacancyDetails_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<VacancyDetail> vacancyDetails = db.VacancyDetails.Where(x => x.VacancyId==id);

            DataSourceResult result = vacancyDetails.ToList().ToDataSourceResult(request, vacancyDetail => new
            {
                VacancyDetailId = vacancyDetail.VacancyDetailId,
                VacancyId = vacancyDetail.VacancyId,
                LookupId = vacancyDetail.LookupId,
                Experience = vacancyDetail.Experience,
                Decription = vacancyDetail.Decription,
                DateCreated = vacancyDetail.DateCreated,
                LastModified = vacancyDetail.LastModified,
                CreatedBy = vacancyDetail.CreatedBy,
                ModifiedBy = vacancyDetail.ModifiedBy,
            });

            return Json(result);
        }
        
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult VacancyDetails_Create([DataSourceRequest]DataSourceRequest request, VacancyDetail vacancyDetail,int id)
        {

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                vacancyDetail.VacancyId = id;
                vacancyDetail.CreatedBy = User.Identity.Name;
                vacancyDetail.DateCreated = DateTime.Now; ;
                var entity = GetMappedObject(vacancyDetail);
                db.VacancyDetails.Add(entity);
                db.SaveChanges();
                vacancyDetail.VacancyDetailId = entity.VacancyDetailId;

               
            }


            return Json(new[] { vacancyDetail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VacancyDetails_Update([DataSourceRequest]DataSourceRequest request, VacancyDetail vacancyDetail)
        {
            ModelState.Clear();

          
            if (ModelState.IsValid)
            {
                vacancyDetail.ModifiedBy = User.Identity.Name;
                vacancyDetail.LastModified = DateTime.Now; ;
                var entity = GetMappedObject(vacancyDetail);
                db.VacancyDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { vacancyDetail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VacancyDetails_Destroy([DataSourceRequest]DataSourceRequest request, VacancyDetail vaccancyPlan)
        {
            ModelState.Clear();
          
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(vaccancyPlan);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted vacancy of vacancy code " + vaccancyPlan.Vacancy.VaccancyCode);
                #endregion
                db.VacancyDetails.Attach(entity);
                db.VacancyDetails.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
        }
       private VacancyDetail GetMappedObject(VacancyDetail vacancyDetail)
        {
            var entity = new VacancyDetail
            {
                VacancyDetailId = vacancyDetail.VacancyDetailId,
                VacancyId = vacancyDetail.VacancyId,
                LookupId = vacancyDetail.LookupId,
                Experience = vacancyDetail.Experience,
                Decription = vacancyDetail.Decription,
                DateCreated = vacancyDetail.DateCreated,
                LastModified = vacancyDetail.LastModified,
                CreatedBy = vacancyDetail.CreatedBy,
                ModifiedBy = vacancyDetail.ModifiedBy,

            };
            return entity;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

     
    }
    
}
