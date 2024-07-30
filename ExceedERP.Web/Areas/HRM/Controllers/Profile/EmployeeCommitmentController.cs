using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Hubs;



namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class EmployeeCommitmentController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/EmployeeCommitment
        public static DateTime StartDate { get; set; }
        public static DateTime EndDate { get; set; }
        public static string AmharicStartDate { get; set; }
        public static string AmharicEndDate { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var commtTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COMMITMENT_TYPE);
            ViewData["commitment"] = commtTypes;
            var cosignTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COSIGN_TYPE);
            ViewData["CosignType"] = cosignTypes;

            ViewData["EmployeeList"] = db.Person_Employee.AsQueryable();
        }

        public void SetDate(string name, string value)
        {
            if (name == "FromCometAm")
            {
                AmharicStartDate = value;
                var splitDate = AmharicStartDate.Split('/');
                StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToCometAm")
            {
                AmharicEndDate = value;
                var splitDate = AmharicEndDate.Split('/');
                EndDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin, HrmConstants.HRProfileUser)]
        public ActionResult Commitments_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<Commitment> commitments = db.Commitments;//.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = commitments.ToDataSourceResult(request, commitment => new
            {
                EmployeeId = commitment.EmployeeId,
                EmployeeCode = commitment.EmployeeCode,
                CommitmentId = commitment.CommitmentId,
                LookupId = commitment.LookupId,
                Reason = commitment.Reason,
                From = commitment.From,
                To = commitment.To,
                Value = commitment.Value,
                Remark = commitment.Remark,
                FromCometAm = commitment.FromCometAm,
                ToCometAm = commitment.ToCometAm,
                DateCreated = commitment.DateCreated,
                LastModified = commitment.LastModified,
                CreatedBy = commitment.CreatedBy,
                ModifiedBy = commitment.ModifiedBy,
                OrgBranchName = commitment.OrgBranchName,
                IsActive = commitment.IsActive

            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Commitments_Create([DataSourceRequest]DataSourceRequest request, Commitment commitment)
        {
            ModelState.Clear();

            if (commitment.FromCometAm != null)
            {
                var splitDate = commitment.FromCometAm.Split('/');
                commitment.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Please Enter Start Date");
            }
            if (commitment.ToCometAm != null)
            {
                var splitDate = commitment.ToCometAm.Split('/');
                commitment.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Please Enter End Date");
            }

            if (commitment.From > commitment.To)
            {
                ModelState.AddModelError("", "Start Date should be lessthan or Equal to the End Date");
            }
            if (commitment.From.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
            }

            if (ModelState.IsValid)
            {
                var commitmentTable = db.Commitments.Where(x => x.CommitmentId == commitment.CommitmentId).ToList();
                
               
                    var entity = new Commitment
                    {
                        CommitmentId = commitment.CommitmentId,
                        EmployeeId = commitment.EmployeeId,
                         Employee = commitment.Employee,
                        Reason = commitment.Reason,
                        LookupId = commitment.LookupId,
                        From = StartDate,
                        To = EndDate,
                        Value = commitment.Value,
                        Remark = commitment.Remark,
                        FromCometAm = AmharicStartDate,
                        ToCometAm = AmharicEndDate,
                        EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == commitment.EmployeeId).FirstOrDefault().EmployeeCode,
                        DateCreated = DateTime.Now,
                        LastModified = commitment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = commitment.ModifiedBy,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        IsActive = true
                    };

                    db.Commitments.Add(entity);
                    db.SaveChanges();
                    commitment.CommitmentId = entity.CommitmentId;

                    #region Notification
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == commitment.EmployeeId);
                    if (person != null)
                    {
                        var superviorId = EmployeeHelper.GetSupervisor(commitment.EmployeeId);

                        NotifModels notif = new NotifModels
                        {

                            UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                            Title = "Commitment Notification",
                            Message = $" New Commitment is Added for {person.FullNameaEnglish}  ",
                            URL = "~/HRM/EmployeeCommitment"
                        };

                        Notify.NotifyUser(notif);
                    }
                    #endregion
                    //}
                    #region operation log

                    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (profile != null)
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Commitment", "Success", User.Identity.Name, "Commitment of " + profile.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                    }
                    #endregion
                }
            

            return Json(new[] { commitment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Commitments_Update([DataSourceRequest]DataSourceRequest request, Commitment commitment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AmharicStartDate == null)
                {
                    AmharicStartDate = commitment.FromCometAm;
                    var splitDate = AmharicStartDate.Split('/');
                    StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                else if (AmharicEndDate == null)
                {
                    AmharicEndDate = commitment.ToCometAm;
                    var splitDate = AmharicEndDate.Split('/');
                    EndDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (StartDate.Date > EndDate.Date)
                {
                    ModelState.AddModelError("", "Please Check the date range!!");
                }
                else if (StartDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Commitment
                    {
                        CommitmentId = commitment.CommitmentId,
                        LookupId = commitment.LookupId,
                        EmployeeId = commitment.EmployeeId,
                        Employee = commitment.Employee,
                        Reason = commitment.Reason,
                        From = StartDate,
                        To = EndDate,
                        Value = commitment.Value,
                        Remark = commitment.Remark,
                        FromCometAm = AmharicStartDate,
                        ToCometAm = AmharicEndDate,
                        EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == commitment.EmployeeId).FirstOrDefault().EmployeeCode,
                        DateCreated = commitment.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = commitment.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        OrgBranchName = commitment.OrgBranchName,
                        IsActive = commitment.IsActive
                    };

                    db.Commitments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Commitment", "Success", User.Identity.Name, "Commitment of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);

                    }
                    #endregion
                }
            }

            return Json(new[] { commitment }.ToDataSourceResult(request, ModelState));
        }
        
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Commitments_Destroy([DataSourceRequest]DataSourceRequest request, Commitment commitment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Commitment
                {
                    CommitmentId = commitment.CommitmentId,
                    LookupId = commitment.LookupId,
                    EmployeeId = commitment.EmployeeId,
                    Employee = commitment.Employee,
                    Reason = commitment.Reason,
                    From = commitment.From,
                    To = commitment.To,
                    Value = commitment.Value,
                    Remark = commitment.Remark,
                    FromCometAm = commitment.FromCometAm,
                    ToCometAm = commitment.ToCometAm,
                    EmployeeCode = commitment.EmployeeCode,
                    DateCreated = commitment.DateCreated,
                    LastModified = commitment.LastModified,
                    CreatedBy = commitment.CreatedBy,
                    ModifiedBy = commitment.ModifiedBy,
                    OrgBranchName = commitment.OrgBranchName,
                    IsActive = commitment.IsActive
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Commitment information of employee Id" + commitment.EmployeeId);
                #endregion
                db.Commitments.Attach(entity);
                db.Commitments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { commitment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.COMMITMENT), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.COMMITMENT));
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
                        ReferenceType = AttachmentType.COMMITMENT,
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
        public ActionResult DownloadFile(string filename)
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
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.COMMITMENT && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.COMMITMENT + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (System.Exception)
            {

                //throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public string Close(int id)
        {
            var empCommit = db.Commitments.FirstOrDefault(x => x.CommitmentId == id);
            if (empCommit != null)
            {
                empCommit.IsActive = false;

                db.Entry(empCommit).State = EntityState.Modified;
                db.SaveChanges();

                return "true";
            }
            return "false";
        }
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
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

                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult EmployeeCommitment_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<Commitment> commitments = db.Commitments.Where(x => x.EmployeeId == id);
            DataSourceResult result = commitments.ToDataSourceResult(request, commitment => new
            {
                EmployeeId = commitment.EmployeeId,
                EmployeeCode = commitment.EmployeeCode,
                CommitmentId = commitment.CommitmentId,
                LookupId = commitment.LookupId,
                Reason = commitment.Reason,
                From = commitment.From,
                To = commitment.To,
                Value = commitment.Value,
                Remark = commitment.Remark,
                FromCometAm = commitment.FromCometAm,
                ToCometAm = commitment.ToCometAm,
                DateCreated = commitment.DateCreated,
                LastModified = commitment.LastModified,
                CreatedBy = commitment.CreatedBy,
                ModifiedBy = commitment.ModifiedBy,
                OrgBranchName = commitment.OrgBranchName,

            });

            return Json(result);
        }
    }
}