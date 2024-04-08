using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class InstitutionIncomingAnswersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InstitutionIncomingAnswers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var institutionincominganswers = db.InstitutionIncomingAnswers.Where(x => x.InstitutionIncomingId == id).ToList();
            DataSourceResult result = institutionincominganswers.ToDataSourceResult(request, institutionIncomingAnswer => new
            {
                InstitutionIncomingAnswerId = institutionIncomingAnswer.InstitutionIncomingAnswerId,
                InstitutionIncomingId = institutionIncomingAnswer.InstitutionIncomingId,
                LetterSenderId = institutionIncomingAnswer.LetterSenderId,
                OrgStructureId = institutionIncomingAnswer.OrgStructureId,
                Answer = institutionIncomingAnswer.Answer,
                IsAnswered = institutionIncomingAnswer.IsAnswered,
                Date = institutionIncomingAnswer.Date,
                Time = institutionIncomingAnswer.Time
            });

            return Json(result);
        }
        public ActionResult InstitutionIncomingAnswers_Department_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            int orgId = 0;
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId);
            if (placement != null)
            {
                orgId = placement.OrgStructureId;
            }

            var institutionincominganswers = db.InstitutionIncomingAnswers.Where(x => x.InstitutionIncomingId == id).Where(x=>x.OrgStructureId == orgId || x.IsAnswered).ToList();

            DataSourceResult result = institutionincominganswers.ToDataSourceResult(request, institutionIncomingAnswer => new
            {
                InstitutionIncomingAnswerId = institutionIncomingAnswer.InstitutionIncomingAnswerId,
                InstitutionIncomingId = institutionIncomingAnswer.InstitutionIncomingId,
                LetterSenderId = institutionIncomingAnswer.LetterSenderId,
                OrgStructureId = institutionIncomingAnswer.OrgStructureId,
                Answer = institutionIncomingAnswer.Answer,
                IsAnswered = institutionIncomingAnswer.IsAnswered,
                Date = institutionIncomingAnswer.Date,
                Time = institutionIncomingAnswer.Time
            });

            return Json(result);
        }
        public ActionResult Answers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var institutionincominganswers = db.InstitutionIncomingAnswers.Where(x => x.InstitutionIncomingId == id && x.IsAnswered).ToList();

            DataSourceResult result = institutionincominganswers.ToDataSourceResult(request, institutionIncomingAnswer => new
            {
                InstitutionIncomingAnswerId = institutionIncomingAnswer.InstitutionIncomingAnswerId,
                InstitutionIncomingId = institutionIncomingAnswer.InstitutionIncomingId,
                LetterSenderId = institutionIncomingAnswer.LetterSenderId,
                OrgStructureId = institutionIncomingAnswer.OrgStructureId,
                Answer = institutionIncomingAnswer.Answer,
                IsAnswered = institutionIncomingAnswer.IsAnswered,
                Date = institutionIncomingAnswer.Date,
                Time = institutionIncomingAnswer.Time
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionIncomingAnswers_Create([DataSourceRequest]DataSourceRequest request, InstitutionIncomingAnswer institutionIncomingAnswer, int id)
        {
            string username = User.Identity.Name;
            var udb = new ApplicationDbContext();
            var user = udb.Users.FirstOrDefault(x => x.UserName == username);
            if (user != null)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == user.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    institutionIncomingAnswer.OrgStructureId = placement.OrgStructureId;
                }
                else
                {
                    ModelState.AddModelError("", "you don't have active placement");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid user");
            }

            if (ModelState.IsValid)
            {
                var entity = new InstitutionIncomingAnswer
                {
                    InstitutionIncomingId = id,
                    LetterSenderId = institutionIncomingAnswer.LetterSenderId,
                    OrgStructureId = institutionIncomingAnswer.OrgStructureId,
                    Answer = institutionIncomingAnswer.Answer,
                    Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    Time = DateTime.Now.ToShortTimeString()
                };

                db.InstitutionIncomingAnswers.Add(entity);
                db.SaveChanges();
                institutionIncomingAnswer.InstitutionIncomingAnswerId = entity.InstitutionIncomingAnswerId;
            }

            return Json(new[] { institutionIncomingAnswer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionIncomingAnswers_Update([DataSourceRequest]DataSourceRequest request, InstitutionIncomingAnswer institutionIncomingAnswer)
        {
            if (ModelState.IsValid)
            {
                var entity = new InstitutionIncomingAnswer
                {
                    InstitutionIncomingAnswerId = institutionIncomingAnswer.InstitutionIncomingAnswerId,
                    InstitutionIncomingId = institutionIncomingAnswer.InstitutionIncomingId,
                    LetterSenderId = institutionIncomingAnswer.LetterSenderId,
                    OrgStructureId = institutionIncomingAnswer.OrgStructureId,
                    Answer = institutionIncomingAnswer.Answer,
                    Date = institutionIncomingAnswer.Date,
                    Time = institutionIncomingAnswer.Time
                };

                db.InstitutionIncomingAnswers.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { institutionIncomingAnswer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionIncomingAnswers_Destroy([DataSourceRequest]DataSourceRequest request, InstitutionIncomingAnswer institutionIncomingAnswer)
        {
            if (ModelState.IsValid)
            {
                var entity = new InstitutionIncomingAnswer
                {
                    InstitutionIncomingAnswerId = institutionIncomingAnswer.InstitutionIncomingAnswerId,
                    InstitutionIncomingId = institutionIncomingAnswer.InstitutionIncomingId,
                    LetterSenderId = institutionIncomingAnswer.LetterSenderId,
                    OrgStructureId = institutionIncomingAnswer.OrgStructureId,
                    Answer = institutionIncomingAnswer.Answer,
                    Date = institutionIncomingAnswer.Date,
                    Time = institutionIncomingAnswer.Time
                };

                db.InstitutionIncomingAnswers.Attach(entity);
                db.InstitutionIncomingAnswers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { institutionIncomingAnswer }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
