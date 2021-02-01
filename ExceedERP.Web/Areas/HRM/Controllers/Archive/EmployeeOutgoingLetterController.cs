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
using ExceedERP.Core.Domain.HRM.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class EmployeeOutgoingLetterController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime OutgoingLetterDate { get; set; }
        public static string OutgoingLetterDateAm { get; set; }
        public ActionResult Index()
        {
            ViewData["Employees"] = db.Person_Employee.AsQueryable();
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();
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
        public ActionResult NewOutgoingLetter_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OutgoingLetter> newoutgoingletter = db.NewOutgoingLetter.Where(x => x.RefernceType == "Employee");
            DataSourceResult result = newoutgoingletter.ToDataSourceResult(request, newOutgoingLetter => new
            {
                NewOutgoingLetterLetterId = newOutgoingLetter.NewOutgoingLetterLetterId,
                FolderNo = newOutgoingLetter.FolderNo,
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
                BookNo = newOutgoingLetter.BookNo,
                OutgoingLetterDate = newOutgoingLetter.OutgoingLetterDate,
                OutgoingLetterDateAm = newOutgoingLetter.OutgoingLetterDateAm,
                OutgoingTime = newOutgoingLetter.OutgoingTime,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NewOutgoingLetter_Create([DataSourceRequest]DataSourceRequest request, OutgoingLetter outgoingLetter)
        {
            ModelState.Clear();
            var fileNo = "";
            var row = "";
            var shelve = "";
            var folder = "";
            var institutionArchive = db.PersonalArchiveNo.Where(x => x.EmployeeId.ToString() == outgoingLetter.OwnerName).ToList().OrderByDescending(x=>x.PersonalArchiveNoId).FirstOrDefault();
            if(institutionArchive != null)
            {
                fileNo = institutionArchive.FileNo;
                row = institutionArchive.Row;
                shelve = institutionArchive.Shelve;
                folder = institutionArchive.Folder;
            }
            var checkexistance = db.NewOutgoingLetter.Where(x => x.BookNo == outgoingLetter.BookNo).FirstOrDefault();
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This Book No is already exist!!");
            }
            if (OutgoingLetterDateAm == null)
            {

                ModelState.AddModelError("", "Outgoing Letter date value is mandatory!!");

            }
            if (OutgoingLetterDateAm != null)
            {
                if (OutgoingLetterDate.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("", "Outgoing Letter date sholud be less or equal to current date!!");
                }
            }
            if (ModelState.IsValid)
            {

                {
                    var entity = new OutgoingLetter
                    {
                        BookNo = outgoingLetter.BookNo,
                        FolderNo = folder,//outgoingLetter.FolderNo,
                        LetterReferenceNo = outgoingLetter.LetterReferenceNo,
                        OwnerName = outgoingLetter.OwnerName,
                        InstitutionTypeId = outgoingLetter.InstitutionTypeId,
                        Subject = outgoingLetter.Subject,
                        Shelve = shelve,//outgoingLetter.Shelve,
                        Row = row,//outgoingLetter.Row,
                        FileNo = fileNo,//outgoingLetter.FileNo,
                        NoOfAttachment = outgoingLetter.NoOfAttachment,
                        SerialNo = outgoingLetter.SerialNo,
                        RefernceType = "Employee",
                        Remark = outgoingLetter.Remark,
                        OutgoingLetterDate = OutgoingLetterDate,
                        OutgoingLetterDateAm = OutgoingLetterDateAm,
                        OutgoingTime = outgoingLetter.OutgoingTime,
                    };

                    db.NewOutgoingLetter.Add(entity);
                    db.SaveChanges();
                    outgoingLetter.NewOutgoingLetterLetterId = entity.NewOutgoingLetterLetterId;


                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Outgoing Letter", "Success", User.Identity.Name, User.Identity.Name + " add new outgoing letter for " + entity.OwnerName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                    
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


            if (ModelState.IsValid)
            {


                {
                    var entity = new OutgoingLetter
                    {
                        NewOutgoingLetterLetterId = outgoingLetter.NewOutgoingLetterLetterId,
                        FolderNo = outgoingLetter.FolderNo,
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
                        RefernceType = "Employee",
                        BookNo = outgoingLetter.BookNo,
                        OutgoingLetterDate = OutgoingLetterDate,
                        OutgoingLetterDateAm = OutgoingLetterDateAm,
                        OutgoingTime = outgoingLetter.OutgoingTime,
                    };

                    db.NewOutgoingLetter.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Outgoing Letter", "Success", User.Identity.Name, User.Identity.Name + " updated outgoing letter of " + entity.OwnerName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                    
                    #endregion
                }
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
                    BookNo = outgoingLetter.BookNo,
                    OutgoingLetterDate = outgoingLetter.OutgoingLetterDate,
                    OutgoingLetterDateAm = outgoingLetter.OutgoingLetterDateAm,
                    OutgoingTime = outgoingLetter.OutgoingTime,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted outgoing letter of folder no" + outgoingLetter.FolderNo);
                #endregion
                db.NewOutgoingLetter.Attach(entity);
                db.NewOutgoingLetter.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { outgoingLetter }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetData(int empId)
        {
            List<PersonalArchiveNo> myData = new List<PersonalArchiveNo>();
            var archives = db.PersonalArchiveNo.Where(x => x.EmployeeId == empId).OrderByDescending(x => x.PersonalArchiveNoId).ToList();
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
        public JsonResult GetShelve()
        {
            var personalArchiveNos = db.PersonalArchiveNo.ToList();
            return Json(personalArchiveNos.Select(p => new
            {
                Shelve = p.Shelve
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRows()
        {
            var personalArchiveNos = db.PersonalArchiveNo.ToList();
            return Json(personalArchiveNos.Select(p => new
            {
                Row = p.Row
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFolder()
        {
            var personalArchiveNos = db.PersonalArchiveNo.ToList();
            return Json(personalArchiveNos.Select(p => new
            {
                Folder = p.Folder
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFile()
        {
            var personalArchiveNos = db.PersonalArchiveNo.ToList();
            return Json(personalArchiveNos.Select(p => new
            {
                FileNo = p.FileNo
            }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
