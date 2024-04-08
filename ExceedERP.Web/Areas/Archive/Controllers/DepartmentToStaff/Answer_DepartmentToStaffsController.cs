using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToStaff
{
    public class Answer_DepartmentToStaffsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Answer_DepartmentToStaffs_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var answers = db.Answer_DepartmentToStaffs.Where(x => x.EmployeeId == empId && x.OutGoing_DepartmentToStaffId == id).ToList();

            DataSourceResult result = answers.ToDataSourceResult(request, answer_DepartmentToStaff => new
            {
                Answer_DepartmentToStaffId = answer_DepartmentToStaff.Answer_DepartmentToStaffId,
                OutGoing_DepartmentToStaffId = answer_DepartmentToStaff.OutGoing_DepartmentToStaffId,
                Answer = answer_DepartmentToStaff.Answer,
                EmployeeId = answer_DepartmentToStaff.EmployeeId,
                IsAnswered = answer_DepartmentToStaff.IsAnswered,
                Date = answer_DepartmentToStaff.Date,
                DateEt = answer_DepartmentToStaff.DateEt,
                Time = answer_DepartmentToStaff.Time,
            });

            return Json(result);
        }
        public ActionResult Answer_DepartmentToStaffs_Sent_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var answers = db.Answer_DepartmentToStaffs.Where(x => x.OutGoing_DepartmentToStaffId == id).Where(x => x.IsAnswered).ToList();

            DataSourceResult result = answers.ToDataSourceResult(request, answer_DepartmentToStaff => new
            {
                Answer_DepartmentToStaffId = answer_DepartmentToStaff.Answer_DepartmentToStaffId,
                OutGoing_DepartmentToStaffId = answer_DepartmentToStaff.OutGoing_DepartmentToStaffId,
                Answer = answer_DepartmentToStaff.Answer,
                EmployeeId = answer_DepartmentToStaff.EmployeeId,
                IsAnswered = answer_DepartmentToStaff.IsAnswered,
                Date = answer_DepartmentToStaff.Date,
                DateEt = answer_DepartmentToStaff.DateEt,
                Time = answer_DepartmentToStaff.Time,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Answer_DepartmentToStaffs_Create([DataSourceRequest]DataSourceRequest request, Answer_DepartmentToStaff answer_DepartmentToStaff, int id)
        {
            string username = User.Identity.Name;
            
            answer_DepartmentToStaff.EmployeeId = UserInformationHelper.GetEmployeeId(username);

            string time = DateTime.Now.ToShortTimeString();

            if (ModelState.IsValid)
            {
                var entity = new Answer_DepartmentToStaff
                {
                    OutGoing_DepartmentToStaffId = id,
                    Answer = answer_DepartmentToStaff.Answer,
                    EmployeeId = answer_DepartmentToStaff.EmployeeId,
                    IsAnswered = answer_DepartmentToStaff.IsAnswered,
                    Date = DateTime.Now,
                    DateEt = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day,DateTime.Now.Month,DateTime.Now.Year),
                    Time = time,
                };

                db.Answer_DepartmentToStaffs.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { answer_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Answer_Employee(Answer_DepartmentToStaff answerEmp)
        {
            string username = User.Identity.Name;
            answerEmp.EmployeeId = UserInformationHelper.GetEmployeeId(username);

            //int.TryParse(answerEmp.MasterId, out int outGoingDepartmentToStaffId);
            //if (ModelState.IsValid)
            //{
            //    if (answerEmp.AnswerFile != null && answerEmp.AnswerFile.ContentLength > 0)
            //    {
            //        answerEmp.FilePath = HttpContext.Server.MapPath("~/Content/Files/Archive/");
            //        answerEmp.ShortPath = "/Content/Files/Archive/";
            //        if (!Directory.Exists(answerEmp.FilePath))
            //        {
            //            Directory.CreateDirectory(answerEmp.FilePath);
            //        }
            //        answerEmp.FileName = Path.GetFileNameWithoutExtension(answerEmp.AnswerFile.FileName);
            //        answerEmp.FileExtension = System.IO.Path.GetExtension(answerEmp.AnswerFile.FileName);
            //        string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            //        answerEmp.FileName = answerEmp.FileName + name + answerEmp.FileExtension;
            //        answerEmp.FilePath = answerEmp.FilePath + answerEmp.FileName;

            //        answerEmp.AnswerFile.SaveAs(answerEmp.FilePath);

            //        var entity = new Answer_DepartmentToStaff
            //        {
            //            OutGoing_DepartmentToStaffId = outGoingDepartmentToStaffId,
            //            Answer = answerEmp.Answer,
            //            EmployeeId = answerEmp.EmployeeId,
            //            FilePath = answerEmp.FilePath,
            //            ShortPath = answerEmp.ShortPath,
            //            FileName = answerEmp.FileName,
            //            FileExtension = answerEmp.FileExtension,
            //            IsAnswered = answerEmp.IsAnswered
            //        };

            //        db.Answer_DepartmentToStaffs.Add(entity);
            //        db.SaveChanges();
            //    }

            //}

            return RedirectToAction("Index_Receiver", "OutGoing_DepartmentToStaff");

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Answer_DepartmentToStaffs_Update([DataSourceRequest]DataSourceRequest request, Answer_DepartmentToStaff answer_DepartmentToStaff)
        {
            if (ModelState.IsValid)
            {
                var entity = new Answer_DepartmentToStaff
                {
                    Answer_DepartmentToStaffId = answer_DepartmentToStaff.Answer_DepartmentToStaffId,
                    OutGoing_DepartmentToStaffId = answer_DepartmentToStaff.OutGoing_DepartmentToStaffId,
                    Answer = answer_DepartmentToStaff.Answer,
                    EmployeeId = answer_DepartmentToStaff.EmployeeId,
                    IsAnswered = answer_DepartmentToStaff.IsAnswered,
                    Date = answer_DepartmentToStaff.Date,
                    DateEt = answer_DepartmentToStaff.DateEt,
                    Time = answer_DepartmentToStaff.Time,
                };

                db.Answer_DepartmentToStaffs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { answer_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Answer_DepartmentToStaffs_Destroy([DataSourceRequest]DataSourceRequest request, Answer_DepartmentToStaff answer_DepartmentToStaff)
        {
            if (ModelState.IsValid)
            {
                var answer = db.Answer_DepartmentToStaffs.Find(answer_DepartmentToStaff.Answer_DepartmentToStaffId);

                if(answer != null )
                {
                    db.Entry(answer).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }

            return Json(new[] { answer_DepartmentToStaff }.ToDataSourceResult(request, ModelState));
        }
        public string FinishAnswering(int id)
        {
            // you can finish answering only your answer 
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);


            var answers = db.Answer_DepartmentToStaffs.Where(x => x.OutGoing_DepartmentToStaffId == id && x.EmployeeId == empId).ToList();
            foreach (var answer in answers)
            {
                answer.IsAnswered = true;

                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
            }

            return "Successfull";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
