﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using ExceedERP.Core.Domain.HRM.ArchiveManagement;
﻿using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
    public class LetterSenderController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string DateAmharic { get; set; }
        public static DateTime Date { get; set; }
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();

            var allUsers = context.Users.ToList();
            ViewData["Users"] = allUsers;
            GetDepartments();
            return View();
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.Where(x => x.Parent == 0).FirstOrDefault();
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo = db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }




        }
        public ActionResult LetterSender_Read([DataSourceRequest]DataSourceRequest request, string bookId)
        {
            IQueryable<LetterSender> LetterSender = db.LetterSender.Where (x=> x.BookNo ==bookId ).OrderBy (x=> x.Date );
            DataSourceResult result = LetterSender.ToDataSourceResult(request, letterSender => new {
                LetterSenderId = letterSender.LetterSenderId,
                Date = letterSender.Date,
                Receiver = letterSender.Receiver,
                Description = letterSender.Description,
                Sender = letterSender.Sender,
                BookNo = letterSender.BookNo ,
                OwnerName = GetOwener(letterSender.BookNo),
                Status = letterSender.Status,
                 DateAmharic = letterSender.DateAmharic,
             
                FileName = letterSender.FileName,
                FileType = letterSender.FileType,
                File = letterSender.File,
            });

            return Json(result);
        }
        private string GetOwener(string bookno)
        {
            var book = db.NewIncommingLetter.Where(x => x.BookNo == bookno).FirstOrDefault();
            if (book!=null)
            {
                return book.OwnerName;
            }
            return "";

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterSender_Create([DataSourceRequest]DataSourceRequest request, LetterSender letterSender, string bookTwo)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LetterSender
                {
                    Date = DateTime .Now ,
                    Receiver = letterSender.Receiver,
                    Description = letterSender.Description,
                    Sender = User.Identity.Name,                  
                    BookNo = bookTwo,
                  OwnerName = GetOwener(bookTwo),
                    Status = LettersStatus.NEW,
                    DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    FileName = letterSender.FileName,
                    FileType = letterSender.FileType,
                    File = letterSender.File,
                    
                };

                db.LetterSender.Add(entity);
                db.SaveChanges();
                letterSender.LetterSenderId = entity.LetterSenderId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Letter Sender", "Success", User.Identity.Name, User.Identity.Name + " add new Letter Sender of book no " + entity.BookNo + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                
                #endregion
            }

            return Json(new[] { letterSender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterSender_Update([DataSourceRequest]DataSourceRequest request, LetterSender letterSender)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LetterSender
                {
                    LetterSenderId = letterSender.LetterSenderId,
                    Date = letterSender.Date,
                    Receiver = letterSender.Receiver,
                    Description = letterSender.Description,
                    Sender = User.Identity.Name,
                    BookNo = letterSender.BookNo ,
                    OwnerName = letterSender.OwnerName,
                    Status = letterSender.Status,
                    DateAmharic = letterSender.DateAmharic,
                    FileName = letterSender.FileName,
                    FileType = letterSender.FileType,
                    File = letterSender.File,
                };

                db.LetterSender.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Letter Sender", "Success", User.Identity.Name, User.Identity.Name + " updated Letter Sender of book no " + entity.BookNo + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { letterSender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterSender_Destroy([DataSourceRequest]DataSourceRequest request, LetterSender letterSender)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LetterSender
                {
                    LetterSenderId = letterSender.LetterSenderId,
                    Date = letterSender.Date,
                    Receiver = letterSender.Receiver,
                    Description = letterSender.Description,
                    Sender = letterSender.Sender,
                    BookNo = letterSender.BookNo ,
                    OwnerName = letterSender.OwnerName,
                    Status = letterSender.Status,
                    DateAmharic = letterSender.DateAmharic,
                    FileName = letterSender.FileName,
                    FileType = letterSender.FileType,
                    File = letterSender.File,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted letter sender information of name " + letterSender.Description);
                #endregion
                db.LetterSender.Attach(entity);
                db.LetterSender.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { letterSender }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
