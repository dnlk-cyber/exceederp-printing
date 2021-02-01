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
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using System.IO;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;
using System.Data.Entity.Validation;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin, HrmConstants.HRDisciplineUser)]
    public class CommitteMinuteDetailController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string DateAmharic { get; set; }
        public static DateTime Date { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var committees = db.Committees;

            ViewData["Committees"] = committees;
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

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin, HrmConstants.HRDisciplineUser)]
        public ActionResult CommitteMinute_Read([DataSourceRequest]DataSourceRequest request, string committeeMinut, int id)
        {
            IQueryable<CommitteMinute> committeminutes = db.CommitteMinutes.Where(x => x.CommitteeId == id).AsQueryable();

            DataSourceResult result = committeminutes.ToDataSourceResult(request, committeMinute => new
            {
                CommitteeId = committeMinute.CommitteeId,
                CommitteMinuteId = committeMinute.CommitteMinuteId,
                FileNo = committeMinute.FileNo,
                Agenda = committeMinute.Agenda,
                PlaceOfMetting = committeMinute.PlaceOfMetting,
                Remark = committeMinute.Remark,
                Date = Date,
                OrgBranchName = committeMinute.OrgBranchName,
                DateCreated = committeMinute.DateCreated,
                LastModified = committeMinute.LastModified,
                CreatedBy = committeMinute.CreatedBy,
                ModifiedBy = committeMinute.ModifiedBy,
                DateAmharic = committeMinute.DateAmharic
            });

            return Json(result);
        }
        //[Authorize(Roles = HR_Company_User + "," + HR_Discipline_User)]
        //[AuthorizeRoles(HrmConstants.HRCompanyUser, HrmConstants.HRDisciplineUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommitteMinute_Create([DataSourceRequest]DataSourceRequest request, CommitteMinute committeMinute, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new CommitteMinute
                {
                    CommitteeId = id,
                    FileNo = committeMinute.FileNo,
                    Agenda = committeMinute.Agenda,
                    PlaceOfMetting = committeMinute.PlaceOfMetting,
                    Remark = committeMinute.Remark,
                    Date = Date,
                    OrgBranchName = committeMinute.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = committeMinute.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = committeMinute.ModifiedBy,
                    DateAmharic = DateAmharic
                };



                db.CommitteMinutes.Add(entity);
                db.SaveChanges();
                committeMinute.CommitteMinuteId = entity.CommitteMinuteId;


            }

            return Json(new[] { committeMinute }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser, HrmConstants.HRDisciplineUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommitteMinute_Update([DataSourceRequest]DataSourceRequest request, CommitteMinute committeMinute)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = committeMinute.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var entity = new CommitteMinute
                {
                    CommitteeId = committeMinute.CommitteeId,
                    CommitteMinuteId = committeMinute.CommitteMinuteId,
                    FileNo = committeMinute.FileNo,
                    Agenda = committeMinute.Agenda,
                    PlaceOfMetting = committeMinute.PlaceOfMetting,
                    Remark = committeMinute.Remark,
                    Date = Date,
                    OrgBranchName = committeMinute.OrgBranchName,
                    DateCreated = committeMinute.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = committeMinute.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    DateAmharic = DateAmharic
                };

                db.CommitteMinutes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { committeMinute }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommitteMinute_Destroy([DataSourceRequest]DataSourceRequest request, CommitteMinute committeMinute)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new CommitteMinute
                {
                    CommitteeId = committeMinute.CommitteeId,
                    CommitteMinuteId = committeMinute.CommitteMinuteId,
                    FileNo = committeMinute.FileNo,
                    Agenda = committeMinute.Agenda,
                    PlaceOfMetting = committeMinute.PlaceOfMetting,
                    Remark = committeMinute.Remark,
                    Date = Date,
                    OrgBranchName = committeMinute.OrgBranchName,
                    DateCreated = committeMinute.DateCreated,
                    LastModified = committeMinute.LastModified,
                    CreatedBy = committeMinute.CreatedBy,
                    ModifiedBy = committeMinute.ModifiedBy,
                    DateAmharic = committeMinute.DateAmharic

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Commitee Minute of file no " + committeMinute.FileNo.ToString() + " agenda " + committeMinute.Agenda);
                #endregion
                db.CommitteMinutes.Attach(entity);
                db.CommitteMinutes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { committeMinute }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadFile(string filename)
        {

            db = new ExceedDbContext();
            try
            {
                var attachment =
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.MINUTE)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + "0" + "-" + AttachmentType.MINUTE + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {


            }
            return null;
        }
        //[Authorize(Roles = HR_Company_User + "," + HR_Discipline_User)]
        [AuthorizeRoles(HrmConstants.HRCompanyUser, HrmConstants.HRDisciplineUser)]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.MINUTE), fileName);
                    var mainPath =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.MINUTE));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = HRMInputs.EmployeeId.ToString(),
                            Description = file.FileName,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.MINUTE,
                            Index = 0,
                            URL = path,
                            Remark = HRMInputs.Owner

                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

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
            return File("~/Content/Images/" + data, System.Net.Mime.MediaTypeNames.Application.Octet);

        }
        //[Authorize(Roles = HR_Company_Admin + "," + HR_Company_User + "," + HR_Admin + "," + HR_Discipline_User)]
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin, HrmConstants.HRDisciplineUser)]
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
