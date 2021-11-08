using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
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

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class InterviewerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ExamTypeSetting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Interviewer_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<RecruitmetnInterviewerPerseon> recruitments = db.RecruitmetnInterviewerPerseons.Where(x=>x.VacancyId == id);
            DataSourceResult result = recruitments.ToDataSourceResult(request, recruit => new
            {
                EmployeeId = recruit.EmployeeId,
                RecruitmetnInterviewerPerseonId = recruit.RecruitmetnInterviewerPerseonId,
                VacancyId = recruit.VacancyId,
                Remark = recruit.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Interviewer_Create([DataSourceRequest]DataSourceRequest request, RecruitmetnInterviewerPerseon recruit, int id)
        {
            ModelState.Clear();
           
            if (ModelState.IsValid)
            {
               
                    var entity = new RecruitmetnInterviewerPerseon
                    {
                        EmployeeId = recruit.EmployeeId,
                        VacancyId = id,
                        Remark = recruit.Remark
                    };

                    db.RecruitmetnInterviewerPerseons.Add(entity);
                    db.SaveChanges();
                
            }
            return Json(new[] { recruit }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Interviewer_Update([DataSourceRequest]DataSourceRequest request, RecruitmetnInterviewerPerseon recruit)
        {
            ModelState.Clear();
        
            if (ModelState.IsValid)
            {
                var entity = new RecruitmetnInterviewerPerseon
                {
                    EmployeeId = recruit.EmployeeId,
                    RecruitmetnInterviewerPerseonId = recruit.RecruitmetnInterviewerPerseonId,
                    VacancyId = recruit.VacancyId,
                    Remark = recruit.Remark
                };

                db.RecruitmetnInterviewerPerseons.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { recruit }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Interviewer_Destroy([DataSourceRequest]DataSourceRequest request, RecruitmetnInterviewerPerseon recruit)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new RecruitmetnInterviewerPerseon
                {
                    EmployeeId = recruit.EmployeeId,
                    RecruitmetnInterviewerPerseonId = recruit.RecruitmetnInterviewerPerseonId,
                    VacancyId = recruit.VacancyId,
                    Remark = recruit.Remark
                };
                #region delete log
                //  UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Recruitment setting of " + configuration.Attribute);
                #endregion
                db.RecruitmetnInterviewerPerseons.Attach(entity);
                db.RecruitmetnInterviewerPerseons.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { recruit }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
