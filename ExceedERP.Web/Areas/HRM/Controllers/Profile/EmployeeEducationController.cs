using System.Collections.Generic;
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
using System;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class EmployeeEducationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/EmployeeEducation
        public ActionResult Index()
        {
          
        
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
        }
        public ActionResult DownloadFile(string filename)
        {

            db = new ExceedDbContext();
            try
            {
                var attachment =
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.EDUCATION && x.Remark == HRMInputs.Owner)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.EDUCATION + "/" + attachment.Description));
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

        public ActionResult EmpTrainingEducations_Read([DataSourceRequest]DataSourceRequest request, int idEd)
        {
            IQueryable<Training> emptrainingeducations = db.EmpTrainingEducations.Where(t => t.IsTraining == false && t.EmployeeId == idEd);
            DataSourceResult result = emptrainingeducations.ToDataSourceResult(request, empTrainingEducation => new
            {
                EmpEduTrainId = empTrainingEducation.EmpEduTrainId,
                EmployeeId = empTrainingEducation.EmployeeId,
                OrganizationName = empTrainingEducation.OrganizationName,
                OrgAddress = empTrainingEducation.OrgAddress,
                IsTraining = false,
                From = empTrainingEducation.From,
                To = empTrainingEducation.To,
                Score = empTrainingEducation.Score,
                TotalCost = empTrainingEducation.TotalCost,
                PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                Remark = empTrainingEducation.Remark,
                DateCreated = empTrainingEducation.DateCreated,
                LastModified = empTrainingEducation.LastModified,
                CreatedBy = empTrainingEducation.CreatedBy,
                ModifiedBy = empTrainingEducation.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpTrainingEducations_Create([DataSourceRequest]DataSourceRequest request, Training empTrainingEducation, int idEdTwo)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
               
                
                    var entity = new Training
                    {
                        OrganizationName = empTrainingEducation.OrganizationName,
                        EmployeeId = idEdTwo,
                        OrgAddress = empTrainingEducation.OrgAddress,
                        IsTraining = false,
                        From = empTrainingEducation.From,
                        To = empTrainingEducation.To,
                        Score = empTrainingEducation.Score,
                        TotalCost = empTrainingEducation.TotalCost,
                        PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                        Remark = empTrainingEducation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = empTrainingEducation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = empTrainingEducation.ModifiedBy
                    };

                    db.EmpTrainingEducations.Add(entity);
                    db.SaveChanges();
                    empTrainingEducation.EmpEduTrainId = entity.EmpEduTrainId;

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Education", "Success", User.Identity.Name, "Employee Education for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                    
                }
                #endregion
            }

            return Json(new[] { empTrainingEducation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpTrainingEducations_Update([DataSourceRequest]DataSourceRequest request, Training empTrainingEducation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Training
                {
                    EmpEduTrainId = empTrainingEducation.EmpEduTrainId,
                    OrganizationName = empTrainingEducation.OrganizationName,
                    OrgAddress = empTrainingEducation.OrgAddress,
                    IsTraining = false,
                    From = empTrainingEducation.From,
                    To = empTrainingEducation.To,
                    Score = empTrainingEducation.Score,
                    TotalCost = empTrainingEducation.TotalCost,
                    PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                    Remark = empTrainingEducation.Remark,
                    DateCreated = empTrainingEducation.DateCreated,
                    LastModified = DateTime.Now ,
                    CreatedBy = empTrainingEducation.CreatedBy,
                    ModifiedBy =User.Identity .Name 
                };

                db.EmpTrainingEducations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Education", "Success", User.Identity.Name, "Employee Education of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                    
                }
                #endregion
            }

            return Json(new[] { empTrainingEducation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpTrainingEducations_Destroy([DataSourceRequest]DataSourceRequest request, Training empTrainingEducation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Training
                {
                    EmpEduTrainId = empTrainingEducation.EmpEduTrainId,
                    OrganizationName = empTrainingEducation.OrganizationName,
                    OrgAddress = empTrainingEducation.OrgAddress,
                    IsTraining = false,
                    From = empTrainingEducation.From,
                    To = empTrainingEducation.To,
                    Score = empTrainingEducation.Score,
                    TotalCost = empTrainingEducation.TotalCost,
                    PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                    Remark = empTrainingEducation.Remark,
                    DateCreated = empTrainingEducation.DateCreated,
                    LastModified = empTrainingEducation.LastModified,
                    CreatedBy = empTrainingEducation.CreatedBy,
                    ModifiedBy = empTrainingEducation.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted eductaion information of employee Id" + empTrainingEducation.EmployeeId );
                #endregion
                db.EmpTrainingEducations.Attach(entity);
                db.EmpTrainingEducations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { empTrainingEducation }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.EDUCATION.ToString()), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.EDUCATION.ToString()));
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
                        ReferenceType = AttachmentType.EDUCATION,
                        Index = 0,
                        URL = path

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
            return File("~/Content/Images/" + data, System.Net.Mime.MediaTypeNames.Application.Octet);

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
