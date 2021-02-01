
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Letters;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Letter
{
  
    [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRLettersUser)]
    public class IncomingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime LetterDate { get; set; }
        public static DateTime RecivedDate { get; set; }
        public static string LetterDateAmahric { get; set; }
        public static string RecivedDateAmharic { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetLetterReferenceNo()
        {


            var categories = db.Incomming.OrderBy(e => e.LettersRefNo);


            return Json(categories.Select(s => new { Text = s.LettersRefNo, LettersRefNo = s.LettersRefNo }), JsonRequestBehavior.AllowGet);
        }
        public void SetDate(string name, string value)
        {
            if (name == "LetterDateAmmharic")
            {
                LetterDateAmahric = value;
                var splitDate = LetterDateAmahric.Split('/');
                LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "RecivedDateAmharic")
            {
                RecivedDateAmharic = value;
                var splitDate = RecivedDateAmharic.Split('/');
                RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
        public ActionResult DownloadFile(string filename)
        {

            db = new ExceedDbContext();
            try
            {
                var attachment =
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.INCOMING_LETTER && x.Remark == HRMInputs.Owner)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.INCOMING_LETTER + "/" + attachment.Description));
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
                        ReferenceType = AttachmentType.INCOMING_LETTER,
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

        public FileResult Download(string data, int id)
        {
            string contentType = GetContentType(data);
            // return File("~/App_Data/Images/" + data, System.Net.Mime.MediaTypeNames.Application.Octet);
            return File(Path.Combine(Server.MapPath("~/Content/Images/"), data), contentType, data);
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
        public ActionResult Incomming_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<Incomming> incommingList = db.Incomming.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = incommingList.ToDataSourceResult(request, incomming => new
            {
                IncommingId = incomming.IncommingId,
                LettersRefNo = incomming.LettersRefNo,
                Owner = incomming.Owner,
                LetterDate = incomming.LetterDate,
                RecivedDate = incomming.RecivedDate,
                Subject = incomming.Subject,
                Folder = incomming.Folder,
                Cluster = incomming.Cluster,
                NoOfAttachment = incomming.NoOfAttachment,
                Remark = incomming.Remark,
                LetterDateAmmharic = incomming.LetterDateAmmharic,
                RecivedDateAmharic = incomming.RecivedDateAmharic,
                DateCreated = incomming.DateCreated,
                LastModified = incomming.LastModified,
                CreatedBy = incomming.CreatedBy,
                ModifiedBy = incomming.ModifiedBy,
                OrgBranchName = incomming.OrgBranchName,
            });

            return Json(result);
        }
     
        [AuthorizeRoles(HrmConstants.HRLettersUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Incomming_Create([DataSourceRequest]DataSourceRequest request, Incomming incomming)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (LetterDate.Date <= RecivedDate.Date)
                {
                    ModelState.AddModelError("", "Please check the date range!!");
                }
                else if (LetterDateAmahric == null || RecivedDateAmharic == null)
                {
                    ModelState.AddModelError("", "Date values are mandatory!!");
                }
                else
                {
                    var entity = new Incomming
                    {
                        LettersRefNo = incomming.LettersRefNo,
                        Owner = incomming.Owner,
                        LetterDate = LetterDate,
                        RecivedDate = RecivedDate,
                        Subject = incomming.Subject,
                        Folder = incomming.Folder,
                        Cluster = incomming.Cluster,
                        NoOfAttachment = incomming.NoOfAttachment,
                        Remark = incomming.Remark,
                        LetterDateAmmharic = LetterDateAmahric,
                        RecivedDateAmharic = RecivedDateAmharic,
                        DateCreated = DateTime.Now,
                        LastModified = incomming.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = incomming.ModifiedBy,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.Incomming.Add(entity);
                    db.SaveChanges();
                    incomming.IncommingId = entity.IncommingId;

                    #region operation log
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Incoming", "Success", User.Identity.Name, User.Identity.Name+ " Add Incoming Letter  Owner " + entity.Owner, Modules.ArchiveAndLetters);
                    
                    #endregion
                }
            }

            return Json(new[] { incomming }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRLettersUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Incomming_Update([DataSourceRequest]DataSourceRequest request, Incomming incomming)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (LetterDateAmahric == null)
                {
                    LetterDateAmahric = incomming.LetterDateAmmharic;
                    var splitDate = LetterDateAmahric.Split('/');
                    LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (RecivedDateAmharic == null)
                {
                    RecivedDateAmharic = incomming.RecivedDateAmharic;
                    var splitDate = RecivedDateAmharic.Split('/');
                    RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (LetterDate.Date <= RecivedDate.Date)
                {
                    ModelState.AddModelError("", "Please check the date range!!");
                }
                else
                {
                    var entity = new Incomming
                    {
                        IncommingId = incomming.IncommingId,
                        LettersRefNo = incomming.LettersRefNo,
                        Owner = incomming.Owner,
                        LetterDate = LetterDate,
                        RecivedDate = RecivedDate,
                        Subject = incomming.Subject,
                        Folder = incomming.Folder,
                        Cluster = incomming.Cluster,
                        NoOfAttachment = incomming.NoOfAttachment,
                        Remark = incomming.Remark,
                        LetterDateAmmharic = LetterDateAmahric,
                        RecivedDateAmharic = RecivedDateAmharic,
                        DateCreated = incomming.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = incomming.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        OrgBranchName = incomming.OrgBranchName,
                    };

                    db.Incomming.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "Incoming", "Success", User.Identity.Name, User.Identity.Name + " update Incoming Letter  Owner " + entity.Owner, Modules.ArchiveAndLetters);

                    #endregion
                }
            }

            return Json(new[] { incomming }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Incomming_Destroy([DataSourceRequest]DataSourceRequest request, Incomming incomming)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Incomming
                {
                    IncommingId = incomming.IncommingId,
                    LettersRefNo = incomming.LettersRefNo,
                    Owner = incomming.Owner,
                    LetterDate = incomming.LetterDate,
                    RecivedDate = incomming.RecivedDate,
                    Subject = incomming.Subject,
                    Folder = incomming.Folder,
                    Cluster = incomming.Cluster,
                    NoOfAttachment = incomming.NoOfAttachment,
                    Remark = incomming.Remark,
                    LetterDateAmmharic = incomming.LetterDateAmmharic,
                    RecivedDateAmharic = incomming.RecivedDateAmharic,
                    DateCreated = incomming.DateCreated,
                    LastModified = incomming.LastModified,
                    CreatedBy = incomming.CreatedBy,
                    ModifiedBy = incomming.ModifiedBy,
                    OrgBranchName = incomming.OrgBranchName,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted incomming letter of reference no  " + incomming.LettersRefNo);
                #endregion
                db.Incomming.Attach(entity);
                db.Incomming.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { incomming }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
