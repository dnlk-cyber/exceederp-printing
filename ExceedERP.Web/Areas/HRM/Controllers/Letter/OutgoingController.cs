
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using System.Collections.Generic;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Letters;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Letter
{

    [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRLettersUser)]
    public class OutgoingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public static string DateAmahric { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmmharic")
            {
                DateAmahric = value;
                var splitDate = DateAmahric.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult DownloadFile(string filename)
        {

            db = new ExceedDbContext();
            try
            {
                var attachment =
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.OUTGOING_LETTER && x.Remark == HRMInputs.Owner)
                           .FirstOrDefault();
                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.OUTGOING_LETTER + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    return File(filename, attachment.FileType);
                }
            }
            catch (System.Exception)
            {

                //throw;
            }
            return null;
        }
        public ActionResult GetLetterCardSerialNo()
        {


            var categories = db.Outgoing.OrderBy(e => e.CardSerialNo);


            return Json(categories.Select(s => new { Text = s.CardSerialNo, CardSerialNo = s.CardSerialNo }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.EXPERIENCE), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.EXPERIENCE));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    file.SaveAs(path);

                    var entity = new Attachment
                    {

                        Reference = HRMInputs.EmployeeId.ToString(),
                        Description = file.FileName,
                        FileType = file.ContentType,
                        ReferenceType = AttachmentType.OUTGOING_LETTER,
                        Index = 0,
                        URL = path,
                        Remark = HRMInputs.Owner

                    };
                    db.Attachments.Add(entity);

                    db.SaveChanges();
                    ViewBag.Message = "Upload successful";
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }

        public ActionResult Download(string data, int id)
        {
            var image = db.Attachments.Where(x => x.AttachmentId == id).FirstOrDefault();
            byte[] file = new byte[] { };
            if (image != null)
            {
                file = GetFilesBytes(image.URL);
            }
            return File(file.ToArray(), "application/octet-stream", data);

        }

        public static byte[] GetFilesBytes(string url)
        {
            FileStream fileStream = new FileStream(url, FileMode.Open);
            MemoryStream target = new MemoryStream();

            fileStream.CopyTo(target);

            return target.ToArray();
        }

        public string GetContentType(string FileName)
        {
            string path = "~/Content/Images/" + FileName;
            string ext = Path.GetExtension(path); //get file extension
            string type = "";

            // set known types based on file extension
            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".htm":
                    case ".html":
                        type = "text/HTML";
                        break;

                    case ".txt":
                        type = "text/plain";
                        break;

                    case ".GIF":
                        type = "image/GIF";
                        break;

                    case ".pdf":
                        type = "Application/pdf";
                        break;

                    case ".doc":
                    case ".docx":
                    case ".rtf":

                        type = "Application/msword";
                        break;
                    case ".xlsx":
                        type = "Application/x-msexcel";
                        break;
                }
            }
            return type;
        }
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
                    Remark = attachment.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + attachment.ReferenceType.ToString() + " " + attachment.Description);
                #endregion
                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
       
        [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRLettersUser)]
        public ActionResult Outgoing_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<Outgoing> outgoingList = db.Outgoing.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = outgoingList.ToDataSourceResult(request, outgoing => new
            {
                OutgoingId = outgoing.OutgoingId,
                Name = outgoing.Name,
                Date = outgoing.Date,
                FolderNo = outgoing.FolderNo,
                CardSerialNo = outgoing.CardSerialNo,
                Owner = outgoing.Owner,
                DateAmharic = outgoing.DateAmharic,
                DateCreated = outgoing.DateCreated,
                LastModified = outgoing.LastModified,
                CreatedBy = outgoing.CreatedBy,
                ModifiedBy = outgoing.ModifiedBy,
                OrgBranchName = outgoing.OrgBranchName,
            });

            return Json(result);
        }
      
        [AuthorizeRoles(HrmConstants.HRLettersUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Outgoing_Create([DataSourceRequest]DataSourceRequest request, Outgoing outgoing)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmahric == null)
                {
                    ModelState.AddModelError("", "Date value is mandatory!!");
                }
                else
                {
                    var entity = new Outgoing
                    {
                        Name = outgoing.Name,
                        Date = outgoing.Date,
                        FolderNo = outgoing.FolderNo,
                        CardSerialNo = outgoing.CardSerialNo,
                        Owner = outgoing.Owner,
                        DateAmharic = DateAmahric,
                        DateCreated = DateTime.Now,
                        LastModified = outgoing.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = outgoing.ModifiedBy,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.Outgoing.Add(entity);
                    db.SaveChanges();
                    outgoing.OutgoingId = entity.OutgoingId;

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "Outgoing", "Success", User.Identity.Name, User.Identity.Name + " Add outgoing Letter  Owner " + entity.Owner, Modules.ArchiveAndLetters);

                    #endregion
                }
            }

            return Json(new[] { outgoing }.ToDataSourceResult(request, ModelState));
        }
       
        [AuthorizeRoles(HrmConstants.HRLettersUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Outgoing_Update([DataSourceRequest]DataSourceRequest request, Outgoing outgoing)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmahric == null)
                {
                    DateAmahric = outgoing.DateAmharic;
                    var splitDate = DateAmahric.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }

                var entity = new Outgoing
                {
                    OutgoingId = outgoing.OutgoingId,
                    Name = outgoing.Name,
                    Date = Date,
                    FolderNo = outgoing.FolderNo,
                    CardSerialNo = outgoing.CardSerialNo,
                    Owner = outgoing.Owner,
                    DateAmharic = DateAmahric,
                    DateCreated = outgoing.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = outgoing.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = outgoing.OrgBranchName,
                };

                db.Outgoing.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Outgoing", "Success", User.Identity.Name, User.Identity.Name + " update outgoing Letter  Owner " + entity.Owner, Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { outgoing }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Outgoing_Destroy([DataSourceRequest]DataSourceRequest request, Outgoing outgoing)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Outgoing
                {
                    OutgoingId = outgoing.OutgoingId,
                    Name = outgoing.Name,
                    Date = outgoing.Date,
                    FolderNo = outgoing.FolderNo,
                    CardSerialNo = outgoing.CardSerialNo,
                    Owner = outgoing.Owner,
                    DateAmharic = outgoing.DateAmharic,
                    DateCreated = outgoing.DateCreated,
                    LastModified = outgoing.LastModified,
                    CreatedBy = outgoing.CreatedBy,
                    ModifiedBy = outgoing.ModifiedBy,
                    OrgBranchName = outgoing.OrgBranchName,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted outgoing letter of name" + outgoing.Name);
                #endregion
                db.Outgoing.Attach(entity);
                db.Outgoing.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { outgoing }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
