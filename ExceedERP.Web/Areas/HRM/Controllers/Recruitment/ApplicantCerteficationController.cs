using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
    public class ApplicantCerteficationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantCertefication_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ApplicantCertefication> applicantcertefication = db.ApplicantCertefications.Where(x => x.ApplicantDetailId == id);
            DataSourceResult result = applicantcertefication.ToDataSourceResult(request, certeficate => new
            {

                ApplicantCerteficationId = certeficate.ApplicantCerteficationId,
                ApplicantDetailId = certeficate.ApplicantDetailId,
                ApplicantRegistrationId = certeficate.ApplicantRegistrationId,
                CerteficationName = certeficate.CerteficationName,
                CerteficationType = certeficate.CerteficationType,
                Remark = certeficate.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantCertefication_Create([DataSourceRequest]DataSourceRequest request, ApplicantCertefication certeficate, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new ApplicantCertefication
                {
                    ApplicantCerteficationId = certeficate.ApplicantCerteficationId,
                    ApplicantDetailId = id,
                    ApplicantRegistrationId = certeficate.ApplicantRegistrationId,
                    CerteficationName = certeficate.CerteficationName,
                    CerteficationType = certeficate.CerteficationType,
                    Remark = certeficate.Remark,

                };

                db.ApplicantCertefications.Add(entity);
                db.SaveChanges();
                certeficate.ApplicantCerteficationId = entity.ApplicantCerteficationId;
            }

            return Json(new[] { certeficate }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantCertefication_Update([DataSourceRequest]DataSourceRequest request, ApplicantCertefication certeficate)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new ApplicantCertefication
                {
                    ApplicantCerteficationId = certeficate.ApplicantCerteficationId,
                    ApplicantDetailId = certeficate.ApplicantDetailId,
                    ApplicantRegistrationId = certeficate.ApplicantRegistrationId,
                    CerteficationName = certeficate.CerteficationName,
                    CerteficationType = certeficate.CerteficationType,
                    Remark = certeficate.Remark,

                };

                db.ApplicantCertefications.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { certeficate }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantCertefication_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantCertefication certeficate)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ApplicantCertefication
                {
                    ApplicantCerteficationId = certeficate.ApplicantCerteficationId,
                    ApplicantDetailId = certeficate.ApplicantDetailId,
                    ApplicantRegistrationId = certeficate.ApplicantRegistrationId,
                    CerteficationName = certeficate.CerteficationName,
                    CerteficationType = certeficate.CerteficationType,
                    Remark = certeficate.Remark,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant experience  information of applicant" + certeficate.ApplicantCerteficationId);
                #endregion
                db.ApplicantCertefications.Attach(entity);
                db.ApplicantCertefications.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { certeficate }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
