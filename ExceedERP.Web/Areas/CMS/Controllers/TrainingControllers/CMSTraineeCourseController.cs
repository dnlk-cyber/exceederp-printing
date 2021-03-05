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

namespace ExceedERP.Web.Areas.CMS.Controllers
{
    public class CMSTraineeCourseController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Data_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<CMSTraineeCourse> courses = db.CMSTraineeCourses.Where(c => c.CmsTraineeId == id).ToList();
            if (!courses.Any())
            {
                var currentTrainee = db.CmsTrainees.Find(id);
                if (currentTrainee != null && currentTrainee.Type == TraineeType.Individual)
                {
                    var course = db.CMSCourses.Find(currentTrainee?.CMSCourseId);
                    if (course != null)
                    {
                        var entity = new CMSTraineeCourse
                        {
                            CmsTraineeId = id,
                            CMSCourseId = course.CMSCourseId,
                            TrainingStatus = TrainingProgramStatus.Registered,
                            StartOfTraining = DateTime.Now,
                            EndOfTraining = DateTime.Now,

                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = null,
                            LastModified = null,
                        };
                        db.CMSTraineeCourses.Add(entity);
                        db.SaveChanges();

                        courses.Add(entity);
                    }
                }

            }
            DataSourceResult result = courses.AsQueryable().ToDataSourceResult(request, traineeCourse => new CMSTraineeCourse
            {
                CMSTraineeCourseId = traineeCourse.CMSTraineeCourseId,
                CmsTraineeId = traineeCourse.CmsTraineeId,
                CMSCourseId = traineeCourse.CMSCourseId,
                TheoreticalExamResult = traineeCourse.TheoreticalExamResult,
                PracticalExamResult = traineeCourse.PracticalExamResult,
                TrainingStatus = traineeCourse.TrainingStatus,
                EndOfTraining = traineeCourse.EndOfTraining,
                StartOfTraining = traineeCourse.StartOfTraining,
                PlaceOfTrainingOrExamination = traineeCourse.PlaceOfTrainingOrExamination,
                TargetAmount = traineeCourse.TargetAmount,



                DateCreated = traineeCourse.DateCreated,
                LastModified = traineeCourse.LastModified,
                CreatedBy = traineeCourse.CreatedBy,
                ModifiedBy = traineeCourse.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Create([DataSourceRequest]DataSourceRequest request, CMSTraineeCourse traineeCourse, int id)
        {
            if (DuplicateTrainingProgramExists(traineeCourse, id))
            {
                ModelState.AddModelError("", "Duplicate Program Assignment not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new CMSTraineeCourse
                {
                    CmsTraineeId = id,

                    CMSCourseId = traineeCourse.CMSCourseId,
                    TheoreticalExamResult = traineeCourse.TheoreticalExamResult,
                    PracticalExamResult = traineeCourse.PracticalExamResult,
                    TrainingStatus = traineeCourse.TrainingStatus,
                    EndOfTraining = DateTime.Now,
                    StartOfTraining = DateTime.Now,
                    PlaceOfTrainingOrExamination = traineeCourse.PlaceOfTrainingOrExamination,
                    TargetAmount = traineeCourse.TargetAmount,



                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.CMSTraineeCourses.Add(entity);
                db.SaveChanges();
                traineeCourse.CMSTraineeCourseId = entity.CMSTraineeCourseId;
            }

            return Json(new[] { traineeCourse }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Update([DataSourceRequest]DataSourceRequest request, CMSTraineeCourse traineeCourse)
        {
            if (DuplicateTrainingProgramExists(traineeCourse, traineeCourse.CmsTraineeId))
            {
                ModelState.AddModelError("", "Duplicate Course Assignment not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new CMSTraineeCourse
                {
                    CMSTraineeCourseId = traineeCourse.CMSTraineeCourseId,
                    CmsTraineeId = traineeCourse.CmsTraineeId,
                    CMSCourseId = traineeCourse.CMSCourseId,
                    TheoreticalExamResult = traineeCourse.TheoreticalExamResult,
                    PracticalExamResult = traineeCourse.PracticalExamResult,
                    TrainingStatus = traineeCourse.TrainingStatus,
                    EndOfTraining = traineeCourse.EndOfTraining,
                    StartOfTraining = traineeCourse.StartOfTraining,
                    PlaceOfTrainingOrExamination = traineeCourse.PlaceOfTrainingOrExamination,
                    TargetAmount = traineeCourse.TargetAmount,


                    DateCreated = traineeCourse.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = traineeCourse.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.CMSTraineeCourses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { traineeCourse }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Destroy([DataSourceRequest]DataSourceRequest request, CMSTraineeCourse traineeCourse)
        {
            if (ModelState.IsValid)
            {
                var entity = new CMSTraineeCourse
                {
                    CMSTraineeCourseId = traineeCourse.CMSTraineeCourseId,
                    CmsTraineeId = traineeCourse.CmsTraineeId,
                    CMSCourseId = traineeCourse.CMSCourseId,
                    TheoreticalExamResult = traineeCourse.TheoreticalExamResult,
                    PracticalExamResult = traineeCourse.PracticalExamResult,
                    TrainingStatus = traineeCourse.TrainingStatus,
                    EndOfTraining = traineeCourse.EndOfTraining,
                    StartOfTraining = traineeCourse.StartOfTraining,
                    PlaceOfTrainingOrExamination = traineeCourse.PlaceOfTrainingOrExamination,
                    TargetAmount = traineeCourse.TargetAmount,



                    DateCreated = traineeCourse.DateCreated,
                    LastModified = traineeCourse.LastModified,
                    CreatedBy = traineeCourse.CreatedBy,
                    ModifiedBy = traineeCourse.ModifiedBy
                };

                db.CMSTraineeCourses.Attach(entity);
                db.CMSTraineeCourses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { traineeCourse }.ToDataSourceResult(request, ModelState));
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

        private bool DuplicateTrainingProgramExists(CMSTraineeCourse course, int id)
        {
            return db.CMSTraineeCourses.Any(p => p.CmsTraineeId == id && p.CMSCourseId == course.CMSCourseId && p.CMSTraineeCourseId != course.CMSTraineeCourseId);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
