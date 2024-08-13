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
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Insurance;
using ExceedERP.Web.Areas.HRM.Controllers;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{

    [AuthorizeRoles(HrmConstants.HRHealthUser, HrmConstants.HRHealthAdmin)]
    public class AccidentRecordController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        string ErrorMessage = "";
        public ActionResult Index()
        {
            ViewData["AllEmployeeList"] = db.Person_Employee.ToList();
            return View();
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
        private bool CheckAccidentRecordValidation(int id)
        {
            ErrorMessage = "";


            #region Restitutions
            var Restitutions = db.Restitutions.Where(x => x.AccidentRecordId == id).FirstOrDefault();
            if (Restitutions != null)
            {

                ErrorMessage = "You cannot delete this Accident Record.Please delete Restitution information first.";
                return false;


            }
            #endregion
            #region AccidentMedicalRecords
            var AccidentMedicalRecords = db.AccidentMedicalRecords.Where(x => x.AccidentRecordId == id).FirstOrDefault();
            if (AccidentMedicalRecords != null)
            {

                ErrorMessage = "You cannot delete this Accident Record.Please delete Accident Medical Record information first.";
                return false;


            }
            #endregion
            return true;

        }
        [AuthorizeRoles(HrmConstants.HRHealthUser, HrmConstants.HRHealthAdmin)]
        public ActionResult AccidentRecords_Read([DataSourceRequest]DataSourceRequest request)
        {
            var accidentrecords = db.AccidentRecords.ToList();
            DataSourceResult result = accidentrecords.ToDataSourceResult(request, accidentRecord => new
            {
                AccidentRecordId = accidentRecord.AccidentRecordId,
                Date = accidentRecord.Date,
                DetailDescription = accidentRecord.DetailDescription,
                AccidentPlace = accidentRecord.AccidentPlace,
                Witness = accidentRecord.Witness,
                IsInWorkPlace = accidentRecord.IsInWorkPlace,
                IsInOnJob = accidentRecord.IsInOnJob,
                EmployeeId = accidentRecord.EmployeeId,
                Remark = accidentRecord.Remark,
                DateAmharic = accidentRecord.DateAmharic
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]

        [AuthorizeRoles(HrmConstants.HRHealthUser)]
        public ActionResult AccidentRecords_Create([DataSourceRequest]DataSourceRequest request, AccidentRecord accidentRecord)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date value is mandatory!!");
                }
                else
                {
                    var entity = new AccidentRecord
                    {
                        Date = Date,
                        DetailDescription = accidentRecord.DetailDescription,
                        AccidentPlace = accidentRecord.AccidentPlace,
                        Witness = accidentRecord.Witness,
                        IsInWorkPlace = accidentRecord.IsInWorkPlace,
                        IsInOnJob = accidentRecord.IsInOnJob,
                        Remark = accidentRecord.Remark,
                        EmployeeId = accidentRecord.EmployeeId,
                        DateAmharic = DateAmharic
                    };

                    db.AccidentRecords.Add(entity);
                    db.SaveChanges();
                    accidentRecord.AccidentRecordId = entity.AccidentRecordId;

                    #region operation log                

                    UserHelper.OperationLog(Request.UserHostAddress, "AccidentRecord", "Success", User.Identity.Name, User.Identity.Name + " add accident recored at Date " + DateTime.Today + " accident place "  + entity.AccidentPlace ,Modules.Health);

                    #endregion
                }
            }
            return Json(new[] { accidentRecord }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]

        [AuthorizeRoles(HrmConstants.HRHealthUser)]
        public ActionResult AccidentRecords_Update([DataSourceRequest]DataSourceRequest request, AccidentRecord accidentRecord)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = accidentRecord.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var entity = new AccidentRecord
                {
                    AccidentRecordId = accidentRecord.AccidentRecordId,
                    Date = Date,
                    DetailDescription = accidentRecord.DetailDescription,
                    AccidentPlace = accidentRecord.AccidentPlace,
                    Witness = accidentRecord.Witness,
                    IsInWorkPlace = accidentRecord.IsInWorkPlace,
                    IsInOnJob = accidentRecord.IsInOnJob,
                    Remark = accidentRecord.Remark,
                    EmployeeId = accidentRecord.EmployeeId,
                    DateAmharic = DateAmharic
                };

                db.AccidentRecords.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "AccidentRecord", "Success", User.Identity.Name, User.Identity.Name + " update accident recored at Date " + DateTime.Today + " accident place " + entity.AccidentPlace, Modules.Health);

                #endregion
            }

            return Json(new[] { accidentRecord }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]

        [AuthorizeRoles(HrmConstants.HRHealthAdmin)]
        public ActionResult AccidentRecords_Destroy([DataSourceRequest]DataSourceRequest request, AccidentRecord accidentRecord)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckAccidentRecordValidation(accidentRecord.AccidentRecordId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new AccidentRecord
                    {
                        AccidentRecordId = accidentRecord.AccidentRecordId,
                        Date = accidentRecord.Date,
                        DetailDescription = accidentRecord.DetailDescription,
                        AccidentPlace = accidentRecord.AccidentPlace,
                        Witness = accidentRecord.Witness,
                        IsInWorkPlace = accidentRecord.IsInWorkPlace,
                        IsInOnJob = accidentRecord.IsInOnJob,
                        Remark = accidentRecord.Remark,
                        EmployeeId = accidentRecord.EmployeeId,
                        DateAmharic = accidentRecord.DateAmharic,
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted accident record  information of record id " + accidentRecord.AccidentRecordId);
                    #endregion
                    db.AccidentRecords.Attach(entity);
                    db.AccidentRecords.Remove(entity);
                    db.SaveChanges();
                }
            }
            return Json(new[] { accidentRecord }.ToDataSourceResult(request, ModelState));
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public ActionResult DownloadDoument(string filename)
        {
            if (HRMInputs.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.DOCUMENT && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }

        [AuthorizeRoles(HrmConstants.HRHealthUser, HrmConstants.HRHealthAdmin)]
        public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        {
            ModelState.Clear();
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
                    FileName = attachment.FileName,

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
        public ActionResult UploadDocument(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {



                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.DOCUMENT), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.DOCUMENT));
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
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
