﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.Core.Domain.HRM.ArchiveManagement;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
    public class EmployeeIncomingLetterController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();
            return View();
        }

        public ActionResult NewIncommingLetter_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IncomingLetterRegistration> newincommingletter = db.NewIncommingLetter;
            DataSourceResult result = newincommingletter.ToDataSourceResult(request, newIncommingLetter => new {

                BookNo = newIncommingLetter.BookNo,
                FolderNo = newIncommingLetter.FolderNo,
                LetterDate = newIncommingLetter.LetterDate,
                LetterReferenceNo = newIncommingLetter.LetterReferenceNo,
                RecivedDate = newIncommingLetter.RecivedDate,
                OwnerName = newIncommingLetter.OwnerName,
                InstitutionTypeId = newIncommingLetter.InstitutionTypeId,
                Subject = newIncommingLetter.Subject,
                Shelve = newIncommingLetter.Shelve,
                Row = newIncommingLetter.Row,
                FileNo = newIncommingLetter.FileNo,
                NoOfAttachment = newIncommingLetter.NoOfAttachment,
                OrgStructureId = newIncommingLetter.OrgStructureId,
                Remark = newIncommingLetter.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NewIncommingLetter_Create([DataSourceRequest]DataSourceRequest request, IncomingLetterRegistration incommingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new IncomingLetterRegistration
                {
                    FolderNo = incommingLetter.FolderNo,
                    LetterDate = incommingLetter.LetterDate,
                    LetterReferenceNo = incommingLetter.LetterReferenceNo,
                    RecivedDate = incommingLetter.RecivedDate,
                    OwnerName = incommingLetter.OwnerName,
                    InstitutionTypeId = incommingLetter.InstitutionTypeId,
                    Subject = incommingLetter.Subject,
                    Shelve = incommingLetter.Shelve,
                    Row = incommingLetter.Row,
                    FileNo = incommingLetter.FileNo,
                    NoOfAttachment = incommingLetter.NoOfAttachment,
                    OrgStructureId = incommingLetter.OrgStructureId,
                    Remark = incommingLetter.Remark
                };

                db.NewIncommingLetter.Add(entity);
                db.SaveChanges();
                incommingLetter.BookNo = entity.BookNo;
                //db = new ExceedDbContext();
                //var letterSender = new LetterSender();
                //var entityr = new LetterSender
                //{
                //    Date = DateTime.Now,
                //    Receiver = incommingLetter.SerialNo,
                //    Description = incommingLetter.Subject,
                //    BookNo = incommingLetter.BookNo,
                //};

                //db.LetterSender.Add(entityr);
                //db.SaveChanges();
                //letterSender.LetterSenderId = entityr.LetterSenderId;
                db = new ExceedDbContext();
                var recivedLetters = new RecivedLetters();
                var entityy = new RecivedLetters
                {
                    //InstnPerson = incommingLetter.InstitutionType.InstitutionName,
                    Receiver = incommingLetter.OrgStructureId,
                    BookNo = incommingLetter.BookNo,
                    Sender = incommingLetter.OwnerName,
                    Description = incommingLetter.Subject,
                    FileName = null,
                    LetterStatus = LettersStatus.NEW,
                    RecivedDate = incommingLetter.RecivedDate
                };

                db.RecivedLetters.Add(entityy);
                db.SaveChanges();
                recivedLetters.RecivedLetterId = entityy.RecivedLetterId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Incoming Letter", "Success", User.Identity.Name, User.Identity.Name + " add new employee incoming letter of book no " + entity.BookNo + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                
                #endregion
            }


            return Json(new[] { incommingLetter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NewIncommingLetter_Update([DataSourceRequest]DataSourceRequest request, IncomingLetterRegistration incommingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new IncomingLetterRegistration
                {
                    BookNo = incommingLetter.BookNo,
                    FolderNo = incommingLetter.FolderNo,
                    LetterDate = incommingLetter.LetterDate,
                    LetterReferenceNo = incommingLetter.LetterReferenceNo,
                    RecivedDate = incommingLetter.RecivedDate,
                    OwnerName = incommingLetter.OwnerName,
                    InstitutionTypeId = incommingLetter.InstitutionTypeId,
                    Subject = incommingLetter.Subject,
                    Shelve = incommingLetter.Shelve,
                    Row = incommingLetter.Row,
                    FileNo = incommingLetter.FileNo,
                    NoOfAttachment = incommingLetter.NoOfAttachment,
                    OrgStructureId = incommingLetter.OrgStructureId,
                    Remark = incommingLetter.Remark
                };

                db.NewIncommingLetter.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
              
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Incoming Letter", "Success", User.Identity.Name, User.Identity.Name + " updated employee incoming letter of book no " + entity.BookNo + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                
                #endregion
            }

            return Json(new[] { incommingLetter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NewIncommingLetter_Destroy([DataSourceRequest]DataSourceRequest request, IncomingLetterRegistration incommingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new IncomingLetterRegistration
                {
                    BookNo = incommingLetter.BookNo,
                    FolderNo = incommingLetter.FolderNo,
                    LetterDate = incommingLetter.LetterDate,
                    LetterReferenceNo = incommingLetter.LetterReferenceNo,
                    RecivedDate = incommingLetter.RecivedDate,
                    OwnerName = incommingLetter.OwnerName,
                    InstitutionTypeId = incommingLetter.InstitutionTypeId,
                    Subject = incommingLetter.Subject,
                    Shelve = incommingLetter.Shelve,
                    Row = incommingLetter.Row,
                    FileNo = incommingLetter.FileNo,
                    NoOfAttachment = incommingLetter.NoOfAttachment,
                    OrgStructureId = incommingLetter.OrgStructureId,
                    Remark = incommingLetter.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted incomming letter of book no" + incommingLetter.BookNo);
                #endregion
                db.NewIncommingLetter.Attach(entity);
                db.NewIncommingLetter.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { incommingLetter }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
