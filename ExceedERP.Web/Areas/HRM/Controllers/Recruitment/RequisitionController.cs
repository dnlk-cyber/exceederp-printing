using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class RequisitionController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static List<MainCompanyStructure> AllJobCodes { get; set; }
        public static int JobCode { get; set; }
        public ActionResult Index()
        {

            PopulateViewData();
            GetOrganizations();
            return View();
        }
        private void PopulateViewData()
        {
            var Nodes = db.MainCompanyStructures.ToList();
        }
        public ActionResult Recruitments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Core.Domain.HRM.RecruitmentManagment.Recruitment> recruitments = db.Recruitments;
            DataSourceResult result = recruitments.ToDataSourceResult(request, recruitment => new
            {
                RecruitmentId = recruitment.RecruitmentId,
                IsForContrat = recruitment.IsForContrat,
                LookupId = recruitment.LookupId,
                NoOfVaccantPostion = recruitment.NoOfVaccantPostion,
                NoOfCurrentStaff = recruitment.NoOfCurrentStaff,
                NoOfShouldBeStaff = recruitment.NoOfShouldBeStaff
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Recruitments_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.RecruitmentManagment.Recruitment recruitment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.RecruitmentManagment.Recruitment
                {
                    IsForContrat = recruitment.IsForContrat,
                    LookupId = JobCode,
                    NoOfVaccantPostion = recruitment.NoOfVaccantPostion,
                    NoOfCurrentStaff = recruitment.NoOfCurrentStaff,
                    NoOfShouldBeStaff = recruitment.NoOfShouldBeStaff
                };

                db.Recruitments.Add(entity);
                db.SaveChanges();
                recruitment.RecruitmentId = entity.RecruitmentId;
            }
            RedirectToAction("Index");
            return Json(new[] { recruitment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Recruitments_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.RecruitmentManagment.Recruitment recruitment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.RecruitmentManagment.Recruitment
                {
                    RecruitmentId = recruitment.RecruitmentId,
                    IsForContrat = recruitment.IsForContrat,
                    LookupId = recruitment.LookupId,
                    NoOfVaccantPostion = recruitment.NoOfVaccantPostion,
                    NoOfCurrentStaff = recruitment.NoOfCurrentStaff,
                    NoOfShouldBeStaff = recruitment.NoOfShouldBeStaff
                };

                db.Recruitments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { recruitment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Recruitments_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.RecruitmentManagment.Recruitment recruitment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.RecruitmentManagment.Recruitment
                {
                    RecruitmentId = recruitment.RecruitmentId,
                    IsForContrat = recruitment.IsForContrat,
                    LookupId = recruitment.LookupId,
                    NoOfVaccantPostion = recruitment.NoOfVaccantPostion,
                    NoOfCurrentStaff = recruitment.NoOfCurrentStaff,
                    NoOfShouldBeStaff = recruitment.NoOfShouldBeStaff
                };
                #region delete log
                var desc = db.Lookups.FirstOrDefault(x=>x.LookupId == recruitment.LookupId)?.Description;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + desc);
                #endregion
                db.Recruitments.Attach(entity);
                db.Recruitments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { recruitment }.ToDataSourceResult(request, ModelState));
        }

        //public void GetJobCode(string jobName)
        //{

        //    if (AllJobCodes== null || AllJobCodes.Count <= 0) return;
        //    var mainCompanyStructure = AllJobCodes.FirstOrDefault(j => j.Name == jobName);
        //    if (mainCompanyStructure != null)
        //        JobCode = mainCompanyStructure.OrgStructureId;
        //}
        public JsonResult GetOrganizations()
        {

            var organizationUnits = db.MainCompanyStructures.ToList();

            ViewData["jobTitle"] = organizationUnits;
            return Json(organizationUnits.Select(s => new { OrgStructureId = s.OrgStructureId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
