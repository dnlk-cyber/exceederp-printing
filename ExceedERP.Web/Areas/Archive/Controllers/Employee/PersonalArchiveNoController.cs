using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using System.IO;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Areas.HRM.Controllers;

namespace ExceedERP.Web.Areas.Archive.Controllers
{

    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class PersonalArchiveNoController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private HRMInputs input = new HRMInputs();
        public ActionResult Index()
        {
            var employees = db.Person_Employee.ToList();
            
            ViewData["EmployeeList"] = employees.Select(q => new
            {
                q.EmployeeId,
                FullNameAmharic = q.EmployeeCode + "-" + q.FullNameAmharic
            });

            ViewData["Branch"] = db.Branch.ToList();

            return View();
        }
    
        public ActionResult PersonalArchiveNo_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PersonalArchiveNo> personalarchiveno = db.PersonalArchiveNo;
            DataSourceResult result = personalarchiveno.ToDataSourceResult(request, personalArchiveNo => new
            {
                PersonalArchiveNoId = personalArchiveNo.PersonalArchiveNoId,
                FileNo = personalArchiveNo.FileNo,
                EmployeeId = personalArchiveNo.EmployeeId,
                EmployeeCode = personalArchiveNo.EmployeeCode,
                ZoneId = personalArchiveNo.ZoneId,
                Shelve = personalArchiveNo.Shelve,
                Row = personalArchiveNo.Row,
                Folder = personalArchiveNo.Folder,
                BranchId = personalArchiveNo.BranchId,
                RoomNo = personalArchiveNo.RoomNo,
                Volume = personalArchiveNo.Volume,
                Remark = personalArchiveNo.Remark
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]       
        public ActionResult PersonalArchiveNo_Create([DataSourceRequest]DataSourceRequest request, PersonalArchiveNo personalArchiveNo)
        {
            //var checkExistance = db.PersonalArchiveNo.FirstOrDefault(x => x.EmployeeId == personalArchiveNo.EmployeeId);
            //if (checkExistance != null)
            //{
            //    ModelState.AddModelError("", "This employee has registered file number!!");
            //}


            var checkExistance = db.PersonalArchiveNo.FirstOrDefault(x => x.EmployeeId == personalArchiveNo.EmployeeId && x.Folder == personalArchiveNo.Folder);
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This folder no are taken!!");
            }
            if (ModelState.IsValid)
            {

                var entity = new PersonalArchiveNo
                {
                    FileNo = personalArchiveNo.FileNo,
                    EmployeeId = personalArchiveNo.EmployeeId,
                    EmployeeCode = input.GetEmployeeCode(personalArchiveNo.EmployeeId),
                    ZoneId = personalArchiveNo.ZoneId,
                    Shelve = personalArchiveNo.Shelve,
                    Row = personalArchiveNo.Row,
                    Folder = personalArchiveNo.Folder,
                    BranchId = personalArchiveNo.BranchId,
                    RoomNo = personalArchiveNo.RoomNo,
                    Volume = personalArchiveNo.Volume,
                    Remark = personalArchiveNo.Remark
                };

                db.PersonalArchiveNo.Add(entity);
                db.SaveChanges();
                personalArchiveNo.PersonalArchiveNoId = entity.PersonalArchiveNoId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Folder", "Success", User.Identity.Name, User.Identity.Name + " add new Employee Folder for " + User.Identity.Name + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { personalArchiveNo }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]      
        public ActionResult PersonalArchiveNo_Update([DataSourceRequest]DataSourceRequest request, PersonalArchiveNo personalArchiveNo)
        {
            if (ModelState.IsValid)
            {
                var entity = new PersonalArchiveNo
                {
                    PersonalArchiveNoId = personalArchiveNo.PersonalArchiveNoId,
                    EmployeeCode = input.GetEmployeeCode(personalArchiveNo.EmployeeId),
                    FileNo = personalArchiveNo.FileNo,
                    EmployeeId = personalArchiveNo.EmployeeId,
                    ZoneId = personalArchiveNo.ZoneId,
                    Shelve = personalArchiveNo.Shelve,
                    Row = personalArchiveNo.Row,
                    Folder = personalArchiveNo.Folder,
                    BranchId = personalArchiveNo.BranchId,
                    RoomNo = personalArchiveNo.RoomNo,
                    Volume = personalArchiveNo.Volume,
                    Remark = personalArchiveNo.Remark
                };

                db.PersonalArchiveNo.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Folder", "Success", User.Identity.Name, User.Identity.Name + " updated Employee Folder of " + User.Identity.Name + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { personalArchiveNo }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]      
        public ActionResult PersonalArchiveNo_Destroy([DataSourceRequest]DataSourceRequest request, PersonalArchiveNo personalArchiveNo)
        {
            if (ModelState.IsValid)
            {
                var entity = new PersonalArchiveNo
                {
                    PersonalArchiveNoId = personalArchiveNo.PersonalArchiveNoId,
                    FileNo = personalArchiveNo.FileNo,
                    EmployeeCode = personalArchiveNo.EmployeeCode,
                    EmployeeId = personalArchiveNo.EmployeeId,
                    ZoneId = personalArchiveNo.ZoneId,
                    Shelve = personalArchiveNo.Shelve,
                    Row = personalArchiveNo.Row,
                    Folder = personalArchiveNo.Folder,
                    BranchId = personalArchiveNo.BranchId,
                    RoomNo = personalArchiveNo.RoomNo,
                    Volume = personalArchiveNo.Volume,
                    Remark = personalArchiveNo.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted personal archive information of file no " + personalArchiveNo.FileNo);
                #endregion
                db.PersonalArchiveNo.Attach(entity);
                db.PersonalArchiveNo.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { personalArchiveNo }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin)]
        public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                    AttachmentId = attachment.AttachmentId,
                    ReferenceType = attachment.ReferenceType,
                    Reference = attachment.Reference,
                    Description = attachment.Description,
                    URL = attachment.URL,
                    BinaryFile = attachment.BinaryFile,
                    FileType = attachment.FileType,
                    Index = attachment.Index,
                    Remark = attachment.Remark,
                    FileName = attachment.FileName
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + attachment.ReferenceType.ToString() + " " + attachment.Description);
                #endregion
                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
                #region delete file from path
                //string fullPath = Request.MapPath(attachment.URL);
                if (System.IO.File.Exists(attachment.URL))
                {
                    System.IO.File.Delete(attachment.URL);
                }
                #endregion
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult UploadDocument(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                //if (file.ContentLength > 0)
                //{
                //    var fileName = Path.GetFileName(file.FileName);
                //    var path =
                //        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                //                           AttachmentType.DOCUMENT), txtValue);
                //    var mainPath =
                //        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                //                           AttachmentType.DOCUMENT));
                //    if (!Directory.Exists(mainPath))
                //    {
                //        Directory.CreateDirectory(mainPath);
                //    }
                //    if (!System.IO.File.Exists(path))
                //    {
                //        var attachment = db.AttachmentArchives.ToList().OrderByDescending(x => x.AttachmentArchiveId);
                //        string dec = "0001";
                //        if (attachment.Any())
                //        {
                //            dec = (Convert.ToInt32(attachment.FirstOrDefault().Description) + 1).ToString();
                //        }

                //        if (dec != "0001")
                //        {
                //            dec = "000" + dec;
                //        }


                //        file.SaveAs(path);

                //        var entity = new AttachmentArchive
                //        {

                //            Reference = HRMInputs.EmployeeId.ToString(),
                //            Description = dec,//file.FileName,
                //            FileName = txtValue,
                //            FileType = file.ContentType,
                //            ReferenceType = AttachmentType.DOCUMENT,
                //            Index = 0,
                //            URL = path,
                //            Remark = HRMInputs.Owner
                //        };
                //        db.AttachmentArchives.Add(entity);

                //        db.SaveChanges();
                //        ViewBag.Message = "Upload successful";
                //    }

                //}
                //else
                //{

                //    ViewBag.Message = "The file is existed";

                //}
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
         public ActionResult DownloadFile(string filename)
        {
            //if (filename == "")
            //{
            //    string messages = "Please select the row first!!";
            //    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            //}
            //HRMInputs.LetterId = filename;
            //HRMInputs.Owner = "Institution";
            //db = new ExceedDbContext();
            //try
            //{
            //    var attachment = db.LetterSender.FirstOrDefault(x => x.LetterSenderId.ToString() == filename);
            //    if (attachment != null)
            //    {
            //        WebClient req = new WebClient();
            //        HttpResponse response = System.Web.HttpContext.Current.Response;
            //        response.Clear();
            //        response.ClearContent();
            //        response.ClearHeaders();
            //        response.Buffer = true;
            //        response.AddHeader("Content-Disposition", "attachment;filename=\"" + attachment.FileName + "\"");
            //        byte[] datas = attachment.File;
            //        response.BinaryWrite(datas);
            //        response.End();
            //        HRMInputs.SelectedFile = 0;
            //        return File(attachment.FileName, attachment.FileType);
            //    }
            //}
            //catch (Exception)
            //{

            //    // throw;
            //}
            //HRMInputs.SelectedFile = 0;


            return RedirectToAction("Index");
            ////return new JavaScriptResult() { Script = "alert('" + "There is no attached File!!" + "');" };
        }
        public JsonResult GenerateEmployeeFolder()
        {
            var folders = db.PersonalArchiveNo.Select(x => x.EmployeeId).ToList();

            var employeeIds = db.Person_Employee.Where(x => !folders.Contains(x.EmployeeId)).Select(x=>x.EmployeeId).ToList();
            foreach (var empId in employeeIds)
            {
                var personal = new PersonalArchiveNo();

                personal.EmployeeId = empId;

                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
                if (placement != null)
                {
                    personal.BranchId = placement.BranchId;
                }
                else
                {
                    var place = db.Placements.Where(x => x.EmployeeId == empId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                    if(place != null)
                    {
                        personal.BranchId = place.BranchId;
                    }
                }

                db.PersonalArchiveNo.Add(personal);
                db.SaveChanges();
            }

            return Json(new
            {
                Message = "Successfully Generated"
            });
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
