using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.ArchiveManagement;
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers
{

    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class InstitutionOutgoingLetterController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();
            ViewData["SubjectType"] = db.SubjectTypes.AsQueryable();
            return View();
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
        public ActionResult InstitutionOutgoingLetter_Read([DataSourceRequest]DataSourceRequest request)
        {
            var newoutgoingletter = db.InstitutionOutgoingLetters.Where(x => x.RefernceType == ReferenceType.Institution.ToString()).ToList();
            DataSourceResult result = newoutgoingletter.ToDataSourceResult(request, newOutgoingLetter => new
            {
                InstitutionOutgoingLetterId = newOutgoingLetter.InstitutionOutgoingLetterId,
                SubjectTypeId = newOutgoingLetter.SubjectTypeId,
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
        public ActionResult InstitutionOutgoingLetter_Create([DataSourceRequest]DataSourceRequest request, InstitutionOutgoingLetter outgoingLetter)
        {
            ModelState.Clear();

            var checkexistance = db.InstitutionOutgoingLetters.FirstOrDefault(x => x.BookNo == outgoingLetter.BookNo);
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This Book No is already exist!!");
            }
            if (outgoingLetter.OutgoingLetterDateAm != null)
            {
                var splitDate = outgoingLetter.OutgoingLetterDateAm.Split('/');
                outgoingLetter.OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                if (outgoingLetter.OutgoingLetterDate.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("", "Outgoing Letter date sholud be less or equal to current date!!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Outgoing Letter date is mandatory");
            }

            var archive = db.InstitutionArchiveNo.Where(x => x.SubjectTypeId == outgoingLetter.SubjectTypeId).OrderByDescending(x => x.InstiArchiveNoId).FirstOrDefault();
            if (archive != null)
            {
                outgoingLetter.FileNo = archive.FileNo;
                outgoingLetter.Row = archive.Row;
                outgoingLetter.Shelve = archive.Shelve;
                outgoingLetter.FolderNo = archive.Folder;
            }

            if (ModelState.IsValid)
            {
                var entity = new InstitutionOutgoingLetter
                {
                    FolderNo = outgoingLetter.FolderNo,
                    BookNo = outgoingLetter.BookNo,
                    LetterReferenceNo = outgoingLetter.LetterReferenceNo,
                    OwnerName = outgoingLetter.OwnerName,
                    InstitutionTypeId = outgoingLetter.InstitutionTypeId,
                    SubjectTypeId = outgoingLetter.SubjectTypeId,
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
                    RefernceType = ReferenceType.Institution.ToString()
                };

                db.InstitutionOutgoingLetters.Add(entity);
                db.SaveChanges();
                outgoingLetter.InstitutionOutgoingLetterId = entity.InstitutionOutgoingLetterId;

                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Institution Outgoing Letter", "Success", User.Identity.Name, User.Identity.Name + " add new Institution Outgoing Letter for " + entity.BookNo + " at date " + DateTime.Today, Modules.ArchiveAndLetters);
                #endregion
            }

            return Json(new[] { outgoingLetter }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionOutgoingLetter_Update([DataSourceRequest]DataSourceRequest request, InstitutionOutgoingLetter outgoingLetter)
        {
            ModelState.Clear();
            if (outgoingLetter.OutgoingLetterDateAm != null)
            {
                var splitDate = outgoingLetter.OutgoingLetterDateAm.Split('/');
                outgoingLetter.OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Outgoing Letter date value is mandatory!!");
            }           

            var archive = db.InstitutionArchiveNo.Where(x => x.SubjectTypeId == outgoingLetter.SubjectTypeId).OrderByDescending(x => x.InstiArchiveNoId).FirstOrDefault();
            if (archive != null)
            {
                outgoingLetter.FileNo = archive.FileNo;
                outgoingLetter.Row = archive.Row;
                outgoingLetter.Shelve = archive.Shelve;
                outgoingLetter.FolderNo = archive.Folder;
            }
            if (ModelState.IsValid)
            {
                var entity = new InstitutionOutgoingLetter
                {
                    InstitutionOutgoingLetterId = outgoingLetter.InstitutionOutgoingLetterId,
                    SubjectTypeId = outgoingLetter.SubjectTypeId,
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

                db.InstitutionOutgoingLetters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Institution Outgoing Letter", "Success", User.Identity.Name, User.Identity.Name + " Updated Institution Outgoing Letter of " + entity.BookNo + " at date " + DateTime.Today, Modules.ArchiveAndLetters);
                #endregion
            }

            return Json(new[] { outgoingLetter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionOutgoingLetter_Destroy([DataSourceRequest]DataSourceRequest request, InstitutionOutgoingLetter outgoingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new InstitutionOutgoingLetter
                {
                    InstitutionOutgoingLetterId = outgoingLetter.InstitutionOutgoingLetterId,
                    SubjectTypeId = outgoingLetter.SubjectTypeId,
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
                db.InstitutionOutgoingLetters.Attach(entity);
                db.InstitutionOutgoingLetters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { outgoingLetter }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetData(int subType)
        {
            List<InstitutionArchiveNo> myData = new List<InstitutionArchiveNo>();
            var archives = db.InstitutionArchiveNo.Where(x => x.SubjectTypeId == subType).OrderByDescending(x => x.InstiArchiveNoId).ToList();
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
        public ActionResult DownloadArchiveAttachment(int ArchiveAttachementId)
        {
            var record = db.ArchiveAttachements.Find(ArchiveAttachementId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
