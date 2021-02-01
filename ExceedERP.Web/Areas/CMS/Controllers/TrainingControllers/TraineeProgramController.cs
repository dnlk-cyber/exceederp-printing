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
    public class TraineeProgramController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Data_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<TraineeProgram> programs = db.TraineePrograms.Where(c => c.CmsTraineeId == id).ToList();
            if (!programs.Any())
            {
                var currentTrainee = db.CmsTrainees.Find(id);
                if(currentTrainee != null && currentTrainee.Type == TraineeType.Individual)
                {
                    var program = db.CMSPrograms.Find(currentTrainee?.CMSProgramId);
                    if (program != null)
                    {
                        var entity = new TraineeProgram
                        {
                            CmsTraineeId = id,
                            ProgramId = program.CMSProgramId,
                            TrainingStatus = TrainingProgramStatus.Registered,

                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = null,
                            LastModified = null,
                        };
                        db.TraineePrograms.Add(entity);
                        db.SaveChanges();

                        programs.Add(entity);
                    }
                }
                
            }
            DataSourceResult result = programs.AsQueryable().ToDataSourceResult(request, traineeProgram => new TraineeProgram
            {
                TraineeProgramId = traineeProgram.TraineeProgramId,
                CmsTraineeId = traineeProgram.CmsTraineeId,
                ProgramId = traineeProgram.ProgramId,
                TheoreticalExamResult = traineeProgram.TheoreticalExamResult,
                PracticalExamResult = traineeProgram.PracticalExamResult,
                TrainingStatus = traineeProgram.TrainingStatus,


                DateCreated = traineeProgram.DateCreated,
                LastModified = traineeProgram.LastModified,
                CreatedBy = traineeProgram.CreatedBy,
                ModifiedBy = traineeProgram.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Create([DataSourceRequest]DataSourceRequest request, TraineeProgram traineeProgram, int id)
        {
            if(DuplicateTrainingProgramExists(traineeProgram, id))
            {
                ModelState.AddModelError("", "Duplicate Program Assignment not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new TraineeProgram
                {
                    CmsTraineeId = id,

                    ProgramId = traineeProgram.ProgramId,
                    TheoreticalExamResult = traineeProgram.TheoreticalExamResult,
                    PracticalExamResult = traineeProgram.PracticalExamResult,
                    TrainingStatus = traineeProgram.TrainingStatus,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.TraineePrograms.Add(entity);
                db.SaveChanges();
                traineeProgram.TraineeProgramId = entity.TraineeProgramId;
            }

            return Json(new[] { traineeProgram }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Update([DataSourceRequest]DataSourceRequest request, TraineeProgram traineeProgram)
        {
            if (DuplicateTrainingProgramExists(traineeProgram, traineeProgram.CmsTraineeId))
            {
                ModelState.AddModelError("", "Duplicate Program Assignment not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new TraineeProgram
                {
                    TraineeProgramId = traineeProgram.TraineeProgramId,
                    CmsTraineeId = traineeProgram.CmsTraineeId,
                    ProgramId = traineeProgram.ProgramId,
                    TheoreticalExamResult = traineeProgram.TheoreticalExamResult,
                    PracticalExamResult = traineeProgram.PracticalExamResult,
                    TrainingStatus = traineeProgram.TrainingStatus,

                    DateCreated = traineeProgram.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = traineeProgram.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.TraineePrograms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { traineeProgram }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Destroy([DataSourceRequest]DataSourceRequest request, TraineeProgram traineeProgram)
        {
            if (ModelState.IsValid)
            {
                var entity = new TraineeProgram
                {
                    TraineeProgramId = traineeProgram.TraineeProgramId,
                    CmsTraineeId = traineeProgram.CmsTraineeId,
                    ProgramId = traineeProgram.ProgramId,
                    TheoreticalExamResult = traineeProgram.TheoreticalExamResult,
                    PracticalExamResult = traineeProgram.PracticalExamResult,
                    TrainingStatus = traineeProgram.TrainingStatus,

                    DateCreated = traineeProgram.DateCreated,
                    LastModified = traineeProgram.LastModified,
                    CreatedBy = traineeProgram.CreatedBy,
                    ModifiedBy = traineeProgram.ModifiedBy
                };

                db.TraineePrograms.Attach(entity);
                db.TraineePrograms.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { traineeProgram }.ToDataSourceResult(request, ModelState));
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

        private bool DuplicateTrainingProgramExists(TraineeProgram program, int id)
        {
            return db.TraineePrograms.Any(p => p.CmsTraineeId == id && p.ProgramId == program.ProgramId && p.TraineeProgramId != program.TraineeProgramId);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
