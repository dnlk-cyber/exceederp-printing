using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.ArchiveManagement;
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class LetterReceiversController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LetterReceivers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var letterreceivers = db.LetterReceivers.Where(x => x.LetterForMultipleStaffId == id).ToList();

            DataSourceResult result = letterreceivers.ToDataSourceResult(request, letterReceiver => new
            {
                LetterReceiverId = letterReceiver.LetterReceiverId,
                LetterForMultipleStaffId = letterReceiver.LetterForMultipleStaffId,
                EmployeeId = letterReceiver.EmployeeId,
                IsForAllEmployee = letterReceiver.IsForAllEmployee,
                IsTransfered = letterReceiver.IsTransfered
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterReceivers_Create([DataSourceRequest]DataSourceRequest request, LetterReceiver letterReceiver, int id)
        {
            var checkIfForAll = db.LetterFolMultipleStaffs.Find(id);
            if (checkIfForAll != null && checkIfForAll.IsForAllEmployee)
            {
                ModelState.AddModelError("","This letter is prepared for all employees you can't select employee");
            }
            if (ModelState.IsValid)
            {                
                if (letterReceiver.EmployeesSelected.Any())
                {
                    var empList = letterReceiver.EmployeesSelected.ToList();
                    for (int i = 0; i < letterReceiver.EmployeesSelected.Count(); i++)
                    {
                        int empId = letterReceiver.EmployeesSelected[i];
                        var check = db.LetterReceivers.FirstOrDefault(x => x.LetterForMultipleStaffId == id && x.EmployeeId == empId);
                        if (check != null)
                        {
                            continue;
                        }
                        var entity = new LetterReceiver
                        {
                            LetterForMultipleStaffId = id,
                            EmployeeId = empId,
                            IsForAllEmployee = true,
                            IsTransfered = false
                        };

                        db.LetterReceivers.Add(entity);
                        db.SaveChanges();
                    }
                }
                //if (letterReceiver.IsForAllEmployee)
                //{
                //    var receivers = db.LetterReceivers.Where(x => x.LetterForMultipleStaffId == id).ToList();
                //    foreach (var rec in receivers)
                //    {
                //        db.Entry(rec).State = EntityState.Deleted;
                //    }

                //    var entity = new LetterReceiver
                //    {
                //        LetterForMultipleStaffId = id,
                //        EmployeeId = 0,
                //        IsForAllEmployee = true,
                //        IsTransfered = false
                //    };

                //    db.LetterReceivers.Add(entity);
                //    db.SaveChanges();
                //}
                //else
                //{
                //    var receiver = db.LetterReceivers.Where(x => x.IsForAllEmployee).ToList();
                //    foreach(var rec in receiver)
                //    {
                //        db.Entry(rec).State = EntityState.Deleted;
                //        db.SaveChanges();
                //    }
                //    var entity = new LetterReceiver
                //    {
                //        LetterForMultipleStaffId = id,
                //        EmployeeId = letterReceiver.EmployeeId,
                //        IsForAllEmployee = false,
                //        IsTransfered = false
                //    };

                //    db.LetterReceivers.Add(entity);
                //    db.SaveChanges();
                //}               
            }

            return Json(new[] { letterReceiver }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterReceivers_Update([DataSourceRequest]DataSourceRequest request, LetterReceiver letterReceiver)
        {
            if (ModelState.IsValid)
            {
                if (letterReceiver.IsForAllEmployee)
                {
                    var receivers = db.LetterReceivers.Where(x => x.LetterForMultipleStaffId == letterReceiver.LetterForMultipleStaffId).ToList();
                    foreach (var rec in receivers)
                    {
                        db.Entry(rec).State = EntityState.Deleted;
                        db.SaveChanges();
                    }

                    var entity = new LetterReceiver
                    {
                        EmployeeId = 0,
                        LetterForMultipleStaffId = letterReceiver.LetterForMultipleStaffId,
                        IsForAllEmployee = true
                    };

                    db.LetterReceivers.Add(entity);
                    db.SaveChanges();
                }
                else
                {
                    var receiver = db.LetterReceivers.Find(letterReceiver.LetterReceiverId);

                    receiver.IsForAllEmployee = false;
                    receiver.EmployeeId = letterReceiver.EmployeeId;

                    db.Entry(receiver).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { letterReceiver }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterReceivers_Destroy([DataSourceRequest]DataSourceRequest request, LetterReceiver letterReceiver)
        {
            if (ModelState.IsValid)
            {
                var entity = new LetterReceiver
                {
                    LetterReceiverId = letterReceiver.LetterReceiverId,
                    LetterForMultipleStaffId = letterReceiver.LetterForMultipleStaffId,
                    EmployeeId = letterReceiver.EmployeeId,
                    IsForAllEmployee = letterReceiver.IsForAllEmployee
                };

                db.LetterReceivers.Attach(entity);
                db.LetterReceivers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { letterReceiver }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FinishTransfer(int masterIdd)
        {
            var letters = db.LetterReceivers.Where(x => x.LetterForMultipleStaffId == masterIdd).ToList();
            foreach (var let in letters)
            {
                let.IsTransfered = true;

                db.Entry(let).State = EntityState.Modified;
                db.SaveChanges();
            }

            var letter = db.LetterFolMultipleStaffs.Find(masterIdd);
            if (letter != null)
            {
                letter.IsTransfered = true;

                db.Entry(letter).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "LetterForMultipleStaffs");
        }
        public JsonResult UploadEmployeeLetters(int masterId, HttpPostedFileBase[] file, string description)
        {
            for(int i=0;i<file.Count();i++)
            {
                HttpPostedFileBase fileUpload = file[i];
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Files/Archive/");
                    string shortPath = "/Content/Files/Archive/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string newFileName = Path.GetFileNameWithoutExtension(fileUpload.FileName);
                    var fileExtension = System.IO.Path.GetExtension(fileUpload.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name + fileExtension;
                    string filePath = path + newFileName;

                    fileUpload.SaveAs(filePath);

                    var entity = new AttachedLettersForStaff
                    {
                        LetterForMultipleStaffId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.AttachedLettersForStaffs.Add(entity);
                    db.SaveChanges();
                }
            }            

            return Json("Successfull");
        }
        public ActionResult DownloadEmployeeLetters(int AttachedLettersForStaffId)
        {
            var record = db.AttachedLettersForStaffs.Find(AttachedLettersForStaffId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
