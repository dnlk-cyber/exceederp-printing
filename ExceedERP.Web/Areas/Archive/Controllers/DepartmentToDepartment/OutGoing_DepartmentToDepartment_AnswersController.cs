using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using System.IO;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToDepartment
{
    public class OutGoing_DepartmentToDepartment_AnswersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OutGoing_DepartmentToDepartment_Answers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            int orgId = 0;
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId);
            if (placement != null)
            {
                orgId = placement.OrgStructureId;
            }

            var answers = db.OutGoing_DepartmentToDepartment_Answers.Where(x => x.OrgStructureId == orgId || x.IsAnswered).Where(x => x.OutGoing_DepartmentToDepartmentId == id).ToList();

            DataSourceResult result = answers.ToDataSourceResult(request, outGoing_DepartmentToDepartment_Answer => new
            {
                OutGoing_DepartmentToDepartment_AnswerId = outGoing_DepartmentToDepartment_Answer.OutGoing_DepartmentToDepartment_AnswerId,
                OutGoing_DepartmentToDepartmentId = outGoing_DepartmentToDepartment_Answer.OutGoing_DepartmentToDepartmentId,
                Answer = outGoing_DepartmentToDepartment_Answer.Answer,
                OrgStructureId = outGoing_DepartmentToDepartment_Answer.OrgStructureId,
                IsAnswered = outGoing_DepartmentToDepartment_Answer.IsAnswered
            });

            return Json(result);
        }
        public ActionResult OutGoing_DepartmentToDepartment_Answers_Finished_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var outgoing_departmenttodepartment_answers = db.OutGoing_DepartmentToDepartment_Answers.Where(x => x.OutGoing_DepartmentToDepartmentId == id && x.IsAnswered).ToList();

            DataSourceResult result = outgoing_departmenttodepartment_answers.ToDataSourceResult(request, outGoing_DepartmentToDepartment_Answer => new
            {
                OutGoing_DepartmentToDepartment_AnswerId = outGoing_DepartmentToDepartment_Answer.OutGoing_DepartmentToDepartment_AnswerId,
                OutGoing_DepartmentToDepartmentId = outGoing_DepartmentToDepartment_Answer.OutGoing_DepartmentToDepartmentId,
                Answer = outGoing_DepartmentToDepartment_Answer.Answer,
                OrgStructureId = outGoing_DepartmentToDepartment_Answer.OrgStructureId,
                IsAnswered = outGoing_DepartmentToDepartment_Answer.IsAnswered
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutGoing_DepartmentToDepartment_Answers_Create([DataSourceRequest]DataSourceRequest request, OutGoing_DepartmentToDepartment_Answer outGoing_DepartmentToDepartment_Answer, int id)
        {
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);

            int orgId = EmployeeHelper.GetEmployeeDepartmentIdByEmpId(empId);
            outGoing_DepartmentToDepartment_Answer.OrgStructureId = EmployeeHelper.GetEmployeeDepartmentIdByEmpId(empId);

            if (ModelState.IsValid)
            {
                var entity = new OutGoing_DepartmentToDepartment_Answer
                {
                    OutGoing_DepartmentToDepartment_AnswerId = outGoing_DepartmentToDepartment_Answer.OutGoing_DepartmentToDepartment_AnswerId,
                    OutGoing_DepartmentToDepartmentId = id,
                    Answer = outGoing_DepartmentToDepartment_Answer.Answer,
                    OrgStructureId = outGoing_DepartmentToDepartment_Answer.OrgStructureId,
                    IsAnswered = outGoing_DepartmentToDepartment_Answer.IsAnswered
                };

                db.OutGoing_DepartmentToDepartment_Answers.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { outGoing_DepartmentToDepartment_Answer }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Answer_Department(OutGoing_DepartmentToDepartment_Answer depAnswer)
        {
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);

            int orgId = EmployeeHelper.GetEmployeeDepartmentIdByEmpId(empId);
            depAnswer.OrgStructureId = EmployeeHelper.GetEmployeeDepartmentIdByEmpId(empId);

           // int.TryParse(depAnswer.MasterId, out int outGoingDepartmentToDepartmentId);
            if (ModelState.IsValid)
            {
                //if (depAnswer.AnswerFile != null && depAnswer.AnswerFile.ContentLength > 0)
                //{
                //    depAnswer.FilePath = HttpContext.Server.MapPath("~/Content/Files/Archive/");
                //    depAnswer.ShortPath = "/Content/Files/Archive/";
                //    if (!Directory.Exists(depAnswer.FilePath))
                //    {
                //        Directory.CreateDirectory(depAnswer.FilePath);
                //    }
                //    depAnswer.FileName = Path.GetFileNameWithoutExtension(depAnswer.AnswerFile.FileName);
                //    depAnswer.FileExtension = System.IO.Path.GetExtension(depAnswer.AnswerFile.FileName);
                //    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                //    depAnswer.FileName = depAnswer.FileName + name + depAnswer.FileExtension;
                //    depAnswer.FilePath = depAnswer.FilePath + depAnswer.FileName;

                //    depAnswer.AnswerFile.SaveAs(depAnswer.FilePath);

                //    var entity = new OutGoing_DepartmentToDepartment_Answer
                //    {
                //        OutGoing_DepartmentToDepartmentId = outGoingDepartmentToDepartmentId,
                //        Answer = depAnswer.Answer,
                //        OrgStructureId = depAnswer.OrgStructureId,
                //        FilePath = depAnswer.FilePath,
                //        ShortPath = depAnswer.ShortPath,
                //        FileName = depAnswer.FileName,
                //        FileExtension = depAnswer.FileExtension,
                //        IsAnswered = depAnswer.IsAnswered
                //    };

                //    db.OutGoing_DepartmentToDepartment_Answers.Add(entity);
                //    db.SaveChanges();
                //}

            }

            return RedirectToAction("Index_Receiver", "OutGoing_DepartmentToDepartments");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutGoing_DepartmentToDepartment_Answers_Update([DataSourceRequest]DataSourceRequest request, OutGoing_DepartmentToDepartment_Answer outGoing_DepartmentToDepartment_Answer)
        {
            if (ModelState.IsValid)
            {
                var entity = new OutGoing_DepartmentToDepartment_Answer
                {
                    OutGoing_DepartmentToDepartment_AnswerId = outGoing_DepartmentToDepartment_Answer.OutGoing_DepartmentToDepartment_AnswerId,
                    OutGoing_DepartmentToDepartmentId = outGoing_DepartmentToDepartment_Answer.OutGoing_DepartmentToDepartmentId,
                    Answer = outGoing_DepartmentToDepartment_Answer.Answer,
                    OrgStructureId = outGoing_DepartmentToDepartment_Answer.OrgStructureId,
                    IsAnswered = outGoing_DepartmentToDepartment_Answer.IsAnswered
                };

                db.OutGoing_DepartmentToDepartment_Answers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { outGoing_DepartmentToDepartment_Answer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutGoing_DepartmentToDepartment_Answers_Destroy([DataSourceRequest]DataSourceRequest request, OutGoing_DepartmentToDepartment_Answer outGoing_DepartmentToDepartment_Answer)
        {
            if (ModelState.IsValid)
            {
                var entity = new OutGoing_DepartmentToDepartment_Answer
                {
                    OutGoing_DepartmentToDepartment_AnswerId = outGoing_DepartmentToDepartment_Answer.OutGoing_DepartmentToDepartment_AnswerId,
                    OutGoing_DepartmentToDepartmentId = outGoing_DepartmentToDepartment_Answer.OutGoing_DepartmentToDepartmentId,
                    Answer = outGoing_DepartmentToDepartment_Answer.Answer,
                    OrgStructureId = outGoing_DepartmentToDepartment_Answer.OrgStructureId,
                    IsAnswered = outGoing_DepartmentToDepartment_Answer.IsAnswered
                };

                db.OutGoing_DepartmentToDepartment_Answers.Attach(entity);
                db.OutGoing_DepartmentToDepartment_Answers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { outGoing_DepartmentToDepartment_Answer }.ToDataSourceResult(request, ModelState));
        }
        public string FinishAnswering(int id)
        {
            // you can finish answering only your answer 
            int orgId = 0;
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId);

            if (placement != null)
            {
                orgId = placement.OrgStructureId;
            }

            var answers = db.OutGoing_DepartmentToDepartment_Answers.Where(x => x.OutGoing_DepartmentToDepartmentId == id && x.OrgStructureId == orgId).ToList();
            foreach (var answer in answers)
            {
                answer.IsAnswered = true;

                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
            }

            return "Successfull";
        }
        //public ActionResult DownloadAnswer(int attachmentId)
        //{
        //    var record = db.OutGoing_DepartmentToDepartment_Answers.Find(attachmentId);

        //    return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
