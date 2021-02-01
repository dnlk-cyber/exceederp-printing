﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.Core.Domain.HRM.ArchiveManagement;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
   
    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class NewOutgoingLetterController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime OutgoingLetterDate { get; set; }
        public static string OutgoingLetterDateAm { get; set; }
        public ActionResult Index()
        {
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();
            ViewData["SubjectType"] = db.SubjectTypes.AsQueryable();
            return View();
        }
        public void SetDate(string name, string value)
        {

            if (name == "OutgoingLetterDateAm")
            {
                OutgoingLetterDateAm = value;
                var splitDate = OutgoingLetterDateAm.Split('/');
                OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }




        }
        public JsonResult GetShelve()
        {
            var vacancies = db.InstitutionArchiveNo.ToList();
            return Json(vacancies.Select(p => new
            {
                Shelve = p.Shelve
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRows()
        {
            var vacancies = db.InstitutionArchiveNo.ToList();
            return Json(vacancies.Select(p => new
            {
                Row = p.Row
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFolder()
        {
            var vacancies = db.InstitutionArchiveNo.ToList();
            return Json(vacancies.Select(p => new
            {
                Folder = p.Folder
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFile()
        {
            var vacancies = db.InstitutionArchiveNo.ToList();
            return Json(vacancies.Select(p => new
            {
                FileNo = p.FileNo
            }), JsonRequestBehavior.AllowGet);
        }
    
        public ActionResult NewOutgoingLetter_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OutgoingLetter> newoutgoingletter = db.NewOutgoingLetter.Where(x => x.RefernceType == "Institution");
            DataSourceResult result = newoutgoingletter.ToDataSourceResult(request, newOutgoingLetter => new {
                NewOutgoingLetterLetterId = newOutgoingLetter.NewOutgoingLetterLetterId,
                FolderNo = newOutgoingLetter.FolderNo,
                BookNo = newOutgoingLetter.BookNo,
                LetterReferenceNo = newOutgoingLetter.LetterReferenceNo,
                OwnerName = newOutgoingLetter.OwnerName,
                InstitutionTypeId = newOutgoingLetter.InstitutionTypeId,
                Subject = newOutgoingLetter.Subject,
                Shelve = newOutgoingLetter.Shelve,
                Row = newOutgoingLetter.Row,
                FileNo = newOutgoingLetter.FileNo,
                NoOfAttachment = newOutgoingLetter.NoOfAttachment,
                SerialNo = newOutgoingLetter.SerialNo,
                Remark = newOutgoingLetter.Remark,
                OutgoingLetterDate = newOutgoingLetter.OutgoingLetterDate,
                OutgoingLetterDateAm = newOutgoingLetter.OutgoingLetterDateAm,
                OutgoingTime = newOutgoingLetter.OutgoingTime,
                RefernceType = newOutgoingLetter.RefernceType
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
   
        public ActionResult NewOutgoingLetter_Create([DataSourceRequest]DataSourceRequest request, OutgoingLetter outgoingLetter)
        {

            ModelState.Clear();
            var checkexistance = db.NewOutgoingLetter.Where(x => x.BookNo == outgoingLetter.BookNo).FirstOrDefault();
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This Book No is already exist!!");
            }
            if (OutgoingLetterDateAm != null)
            {
                if (OutgoingLetterDate.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("", "Outgoing Letter date sholud be less or equal to current date!!");
                }
            }
            if (OutgoingLetterDateAm == null)
            {
              
                    ModelState.AddModelError("", "Outgoing Letter date value is mandatory!!");
               
            }
            if(outgoingLetter.OutgoingLetterDateAm != null)
            {
                var splitDate = outgoingLetter.OutgoingLetterDateAm.Split('/');
                OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            var file = "";
            var row = "";
            var shelv = "";
            var folder = "";
            var recored = "";
            var archive = db.InstitutionArchiveNo.Where(x => x.InstitutionName == outgoingLetter.OutgoingInstitutionName).OrderByDescending(x => x.InstiArchiveNoId).ToList().FirstOrDefault();
            if (archive != null)
            {
                file = archive.FileNo;
                row = archive.Row;
                shelv = archive.Shelve;
                folder = archive.Folder;
            }

            if (ModelState.IsValid)
            {
               
                {
                    var entity = new OutgoingLetter
                    {
                        FolderNo = file,// outgoingLetter.FolderNo,
                        BookNo = outgoingLetter.BookNo,
                        LetterReferenceNo = outgoingLetter.LetterReferenceNo,
                        OwnerName = outgoingLetter.OwnerName,
                        InstitutionTypeId = outgoingLetter.InstitutionTypeId,
                        Subject = outgoingLetter.Subject,
                        Shelve = shelv,// outgoingLetter.Shelve,
                        Row = row,// outgoingLetter.Row,
                        FileNo = file,// outgoingLetter.FileNo,
                        NoOfAttachment = outgoingLetter.NoOfAttachment,
                        SerialNo = outgoingLetter.SerialNo,
                        Remark = outgoingLetter.Remark,
                        OutgoingLetterDate = OutgoingLetterDate,
                        OutgoingLetterDateAm = OutgoingLetterDateAm,
                        OutgoingTime = outgoingLetter.OutgoingTime,
                        RefernceType = "Institution"
                    };

                    db.NewOutgoingLetter.Add(entity);
                    db.SaveChanges();
                    outgoingLetter.NewOutgoingLetterLetterId = entity.NewOutgoingLetterLetterId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    
                        UserHelper.OperationLog(Request.UserHostAddress, "Institution Outgoing Letter", "Success", User.Identity.Name, User.Identity.Name + " add new Institution Outgoing Letter for " + entity.BookNo + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                    
                    #endregion
                }
            }

            return Json(new[] { outgoingLetter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
      
        public ActionResult NewOutgoingLetter_Update([DataSourceRequest]DataSourceRequest request, OutgoingLetter outgoingLetter)
        {
            ModelState.Clear();
            if (OutgoingLetterDateAm == null)
            {
                if (outgoingLetter.OutgoingLetterDateAm != null)
                {
                    OutgoingLetterDateAm = outgoingLetter.OutgoingLetterDateAm;
                    var splitDate = OutgoingLetterDateAm.Split('/');
                    OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]),
                        Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                else

                {


                    ModelState.AddModelError("", "Outgoing Letter date value is mandatory!!");


                }
            }
            if(outgoingLetter.OutgoingLetterDateAm != null)
            {
                var splitDate = outgoingLetter.OutgoingLetterDateAm.Split('/');
                OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            //var checkexistance = db.NewOutgoingLetter.Where(x => x.BookNo == outgoingLetter.BookNo).FirstOrDefault();
            //if (checkexistance != null)
            //{
            //    ModelState.AddModelError("", "This Book No is already exist!!");
            //}

            var file = "";
            var row = "";
            var shelv = "";
            var folder = "";
            var recored = "";
            var archive = db.InstitutionArchiveNo.Where(x => x.InstitutionName == outgoingLetter.OutgoingInstitutionName).OrderByDescending(x => x.InstiArchiveNoId).ToList().FirstOrDefault();
            if (archive != null)
            {
                file = archive.FileNo;
                row = archive.Row;
                shelv = archive.Shelve;
                folder = archive.Folder;
            }
            if (ModelState.IsValid)
            {
                //var checkexistance = db.NewOutgoingLetter.Where(x => x.BookNo == outgoingLetter.BookNo).FirstOrDefault();
                //if (checkexistance != null)
                //{
                //    ModelState.AddModelError("", "This Book No is already exist!!");
                //}
                //else
                //{
                    var entity = new OutgoingLetter
                    {
                        NewOutgoingLetterLetterId = outgoingLetter.NewOutgoingLetterLetterId,
                        FolderNo = folder,// outgoingLetter.FolderNo,
                        BookNo = outgoingLetter.BookNo,
                        LetterReferenceNo = outgoingLetter.LetterReferenceNo,
                        OwnerName = outgoingLetter.OwnerName,
                        InstitutionTypeId = outgoingLetter.InstitutionTypeId,
                        Subject = outgoingLetter.Subject,
                        Shelve = shelv,// outgoingLetter.Shelve,
                        Row =row,// outgoingLetter.Row,
                        FileNo = file,// outgoingLetter.FileNo,
                        NoOfAttachment = outgoingLetter.NoOfAttachment,
                        SerialNo = outgoingLetter.SerialNo,
                        Remark = outgoingLetter.Remark,
                        OutgoingLetterDate = OutgoingLetterDate,
                        OutgoingLetterDateAm = OutgoingLetterDateAm,
                        OutgoingTime = outgoingLetter.OutgoingTime,
                        RefernceType = outgoingLetter.RefernceType
                    };

                    db.NewOutgoingLetter.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Institution Outgoing Letter", "Success", User.Identity.Name, User.Identity.Name + " Updated Institution Outgoing Letter of " + entity.BookNo + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                
                #endregion
                //}
            }

            return Json(new[] { outgoingLetter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
  
        public ActionResult NewOutgoingLetter_Destroy([DataSourceRequest]DataSourceRequest request, OutgoingLetter outgoingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new OutgoingLetter
                {
                    NewOutgoingLetterLetterId = outgoingLetter.NewOutgoingLetterLetterId,
                    FolderNo = outgoingLetter.FolderNo,
                    BookNo = outgoingLetter.BookNo,
                    LetterReferenceNo = outgoingLetter.LetterReferenceNo,
                    OwnerName = outgoingLetter.OwnerName,
                    InstitutionTypeId = outgoingLetter.InstitutionTypeId,
                    Subject = outgoingLetter.Subject,
                    Shelve = outgoingLetter.Shelve,
                    Row = outgoingLetter.Row,
                    FileNo = outgoingLetter.FileNo,
                    NoOfAttachment = outgoingLetter.NoOfAttachment,
                    SerialNo = outgoingLetter.SerialNo,
                    Remark = outgoingLetter.Remark,
                    OutgoingLetterDate = outgoingLetter.OutgoingLetterDate,
                    OutgoingLetterDateAm = outgoingLetter.OutgoingLetterDateAm,
                    OutgoingTime = outgoingLetter.OutgoingTime,
                    RefernceType = outgoingLetter.RefernceType
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted outgoing letter information of folder no " + outgoingLetter.FolderNo);
                #endregion
                db.NewOutgoingLetter.Attach(entity);
                db.NewOutgoingLetter.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { outgoingLetter }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetData(int subType)
        {
            List<InstitutionArchiveNo> myData = new List<InstitutionArchiveNo>();
            var archives = db.InstitutionArchiveNo.Where(x => x.InstitutionName == subType.ToString()).OrderByDescending(x => x.InstiArchiveNoId).ToList();
            if (archives.Any())
            {
                myData.Add(archives.FirstOrDefault());
                return Json(myData.Select(i => new
                {
                    Shelve = i.Shelve,
                    Row = i.Row,
                    FileNo = i.FileNo,
                    Folder = i.Folder
                }), JsonRequestBehavior.AllowGet);

            }

            return null;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
