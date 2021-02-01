﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.HRM.ArchiveManagement;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
    
     [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class NewIncommingLetterController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
           PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            db = new ExceedDbContext();
            ViewData["InstuType"] = db.InstitutionType.ToList();
        }
        [HttpPost]
        public ActionResult Index(IncomingLetterRegistration incommingLetter)
        {
            
            if (ModelState.IsValid)
            {
                var entity = new IncomingLetterRegistration
                {
                    FolderNo = incommingLetter.FolderNo,
                    BookNo = incommingLetter.BookNo,
                    LetterDate = Convert.ToDateTime(incommingLetter.LetterDate),
                    LetterReferenceNo = incommingLetter.LetterReferenceNo,
                    RecivedDate =Convert.ToDateTime(  incommingLetter.RecivedDate),
                    OwnerName = incommingLetter.OwnerName,
                    InstitutionTypeId = incommingLetter.InstitutionTypeId,
                    Subject = incommingLetter.Subject,
                    Shelve = incommingLetter.Shelve,
                    Row = incommingLetter.Row,
                    FileNo = incommingLetter.FileNo,
                    NoOfAttachment = incommingLetter.NoOfAttachment,
                    OrgStructureId = incommingLetter.OrgStructureId,
                    RefernceType = "Institution",
                    Remark = incommingLetter.Remark,
                    OutgoingLetterReferenceNo = incommingLetter.OutgoingLetterReferenceNo,
                    OutgoingOwnerName = incommingLetter.OutgoingOwnerName,
                    OutgoingInstitutionTypeId = incommingLetter.OutgoingInstitutionTypeId,
                    OutgoingSubject = incommingLetter.OutgoingSubject,
                    OutgoingShelve = incommingLetter.OutgoingShelve,
                    OutgoingRow = incommingLetter.OutgoingRow,
                    OutgoingFileNo = incommingLetter.OutgoingFileNo,
                    OutgoingNoOfAttachment = incommingLetter.OutgoingNoOfAttachment,
                    OutgoingSerialNo = "Institution",
                    OutgoingLetterDate =incommingLetter .OutgoingLetterDate ,
                    OutgoingRefernceType = incommingLetter.OutgoingRefernceType,
                    OutgoingRemark = incommingLetter.OutgoingRemark
                };
                db= new ExceedDbContext();
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
                //    Sender = incommingLetter.OwnerName,
                //    StatusName = "NEW"
                    
                //};

                //db.LetterSender.Add(entityr);
                //db.SaveChanges();
                //letterSender.LetterSenderId = entityr.LetterSenderId;
                db = new ExceedDbContext();
                var recivedLetters = new RecivedLetters();
                var entityy = new RecivedLetters
                {
                    //InstnPerson = recivedLetters.InstnPerson,
                    Receiver = incommingLetter.OrgStructureId,
                    BookNo = incommingLetter.BookNo,
                    Sender = incommingLetter.OwnerName,
                    Description = incommingLetter.Subject,
                    InstnPerson = incommingLetter.OwnerName,//.InstitutionName,
                    FileName = null,
                    LetterStatus = LettersStatus.NEW,
                    RecivedDate = incommingLetter.RecivedDate
                };

                db.RecivedLetters.Add(entityy);
                db.SaveChanges();
                recivedLetters.RecivedLetterId = entityy.RecivedLetterId;
            }
            return RedirectToAction( "Index");
        }

        //public ActionResult NewIncommingLetter_Read( )
        //{
        //    IQueryable<IncomingLetterRegistration> newincommingletter = db.NewIncommingLetter;
        //    DataSourceResult result = newincommingletter.ToDataSourceResult(request, incommingLetter => new {
              
        //        FolderNo = incommingLetter.FolderNo,
        //        BookNo = incommingLetter.BookNo,
        //        LetterDate = incommingLetter.LetterDate,
        //        LetterReferenceNo = incommingLetter.LetterReferenceNo,
        //        RecivedDate = incommingLetter.RecivedDate,
        //        OwnerName = incommingLetter.OwnerName,
        //        InstitutionTypeId = incommingLetter.InstitutionTypeId,
        //        Subject = incommingLetter.Subject,
        //        Shelve = incommingLetter.Shelve,
        //        Row = incommingLetter.Row,
        //        FileNo = incommingLetter.FileNo,
        //        NoOfAttachment = incommingLetter.NoOfAttachment,
        //        SerialNo = incommingLetter.SerialNo,
        //        RefernceType = incommingLetter.RefernceType,
        //        Remark = incommingLetter.Remark,
        //        OutgoingLetterReferenceNo = incommingLetter.OutgoingLetterReferenceNo,
        //        OutgoingOwnerName = incommingLetter.OutgoingOwnerName,
        //        OutgoingInstitutionTypeId = incommingLetter.OutgoingInstitutionTypeId,
        //        OutgoingSubject = incommingLetter.OutgoingSubject,
        //        OutgoingShelve = incommingLetter.OutgoingShelve,
        //        OutgoingRow = incommingLetter.OutgoingRow,
        //        OutgoingFileNo = incommingLetter.OutgoingFileNo,
        //        OutgoingNoOfAttachment = incommingLetter.OutgoingNoOfAttachment,
        //        OutgoingSerialNo = incommingLetter.OutgoingSerialNo,
        //        OutgoingRefernceType = incommingLetter.OutgoingRefernceType,
        //        OutgoingRemark = incommingLetter.OutgoingRemark
        //    });

        //    return Json(result);
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NewIncommingLetter_Create(IncomingLetterRegistration incommingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new IncomingLetterRegistration
                {
                    FolderNo = incommingLetter.FolderNo,
                    BookNo = incommingLetter.BookNo,
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
                    RefernceType = incommingLetter.RefernceType,
                    Remark = incommingLetter.Remark,
                    OutgoingLetterReferenceNo = incommingLetter.OutgoingLetterReferenceNo,
                    OutgoingOwnerName = incommingLetter.OutgoingOwnerName,
                    OutgoingInstitutionTypeId = incommingLetter.OutgoingInstitutionTypeId,
                    OutgoingSubject = incommingLetter.OutgoingSubject,
                    OutgoingShelve = incommingLetter.OutgoingShelve,
                    OutgoingRow = incommingLetter.OutgoingRow,
                    OutgoingFileNo = incommingLetter.OutgoingFileNo,
                    OutgoingNoOfAttachment = incommingLetter.OutgoingNoOfAttachment,
                    OutgoingSerialNo = incommingLetter.OutgoingSerialNo,
                    OutgoingRefernceType = incommingLetter.OutgoingRefernceType,
                    OutgoingRemark = incommingLetter.OutgoingRemark
                };

                db.NewIncommingLetter.Add(entity);
                db.SaveChanges();
                incommingLetter.BookNo = entity.BookNo;
            }

            return RedirectToAction( "Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NewIncommingLetter_Update([DataSourceRequest]DataSourceRequest request, IncomingLetterRegistration incommingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new IncomingLetterRegistration
                {
                   
                    FolderNo = incommingLetter.FolderNo,
                    BookNo = incommingLetter.BookNo,
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
                    RefernceType = incommingLetter.RefernceType,
                    Remark = incommingLetter.Remark,
                    OutgoingLetterReferenceNo = incommingLetter.OutgoingLetterReferenceNo,
                    OutgoingOwnerName = incommingLetter.OutgoingOwnerName,
                    OutgoingInstitutionTypeId = incommingLetter.OutgoingInstitutionTypeId,
                    OutgoingSubject = incommingLetter.OutgoingSubject,
                    OutgoingShelve = incommingLetter.OutgoingShelve,
                    OutgoingRow = incommingLetter.OutgoingRow,
                    OutgoingFileNo = incommingLetter.OutgoingFileNo,
                    OutgoingNoOfAttachment = incommingLetter.OutgoingNoOfAttachment,
                    OutgoingSerialNo = incommingLetter.OutgoingSerialNo,
                    OutgoingRefernceType = incommingLetter.OutgoingRefernceType,
                    OutgoingRemark = incommingLetter.OutgoingRemark
                };

                db.NewIncommingLetter.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
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
                   
                    FolderNo = incommingLetter.FolderNo,
                    BookNo = incommingLetter.BookNo,
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
                    RefernceType = incommingLetter.RefernceType,
                    Remark = incommingLetter.Remark,
                    OutgoingLetterReferenceNo = incommingLetter.OutgoingLetterReferenceNo,
                    OutgoingOwnerName = incommingLetter.OutgoingOwnerName,
                    OutgoingInstitutionTypeId = incommingLetter.OutgoingInstitutionTypeId,
                    OutgoingSubject = incommingLetter.OutgoingSubject,
                    OutgoingShelve = incommingLetter.OutgoingShelve,
                    OutgoingRow = incommingLetter.OutgoingRow,
                    OutgoingFileNo = incommingLetter.OutgoingFileNo,
                    OutgoingNoOfAttachment = incommingLetter.OutgoingNoOfAttachment,
                    OutgoingSerialNo = incommingLetter.OutgoingSerialNo,
                    OutgoingRefernceType = incommingLetter.OutgoingRefernceType,
                    OutgoingRemark = incommingLetter.OutgoingRemark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted incomming letter information of folder no " + incommingLetter.FolderNo);
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
