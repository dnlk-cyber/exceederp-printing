using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    [AuthorizeRoles(ConstructionConstants.EstimationApprover, ConstructionConstants.ConstructionSettingAdmin)]
    //[AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin, ConstructionConstants.SchedulingUser, ConstructionConstants.ScheduleApprover)]
    public class ProjectInformationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private DataProviderController dataProvider;
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
         
            var periods = db.Periods.Where( x=> x.PeriodType ==PeriodType.FISCAL_YEAR ).AsQueryable();
            ViewData["PeriodList"] = periods;
            //ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();
            var branches = db.Branch.ToList();
            ViewData["Branch"] = branches;

            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            return View();
        }

        public JsonResult GetEmployees()
        {
            dataProvider = new DataProviderController();
            var employeeList = dataProvider.GetAllPlacmentEmployees(User.Identity.Name);
            return Json(employeeList.Select(p => new { PersonId = p.PersonId, FullName = p.FullName }), JsonRequestBehavior.AllowGet);
        }

        public string CheckStatus(int id)
        {
            var project = db.ProjectInformation.Where(x => x.ProjectInformationId == id).FirstOrDefault();
            if (project != null)
            {
                if (project.ProjectStatus == ProjectStatus.APPROVED)
                {
                    return "a";
                }
                if (project.ProjectStatus == ProjectStatus.STARTED)
                {
                    return "s";
                }
                if (project.ProjectStatus == ProjectStatus.CLOSED)
                {
                    return "c";
                }
            }
            return "";
        }
        public ActionResult ChangeTheState(int id, string state)
        {
            var projectInformation = db.ProjectInformation.Where(x => x.ProjectInformationId == id).FirstOrDefault();
            if (projectInformation != null)
            {
                var entity = new ProjectInformation
                {
                    ProjectInformationId = projectInformation.ProjectInformationId,
                    Name = projectInformation.Name,
                    Code = projectInformation.Code,
                    Category = projectInformation.Category,
                    BranchId = projectInformation.BranchId,
                    ContractAmount = projectInformation.ContractAmount,
                    ProjectScope = projectInformation.ProjectScope,
                    EmployeeId = projectInformation.EmployeeId,
                    ProjectType = projectInformation.ProjectType,
                    Client = projectInformation.Client,
                    Consultant = projectInformation.Consultant,
                    OrgBranchName = projectInformation.OrgBranchName,
                    DateCreated = projectInformation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectInformation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    ProjectStatus = projectInformation.ProjectStatus
                };
                if (state == "APPROVED")
                {
                    entity.ProjectStatus = ProjectStatus.APPROVED;

                }
                else if (state == "STARTED")
                {
                    entity.ProjectStatus = ProjectStatus.STARTED;

                }
                else if (state == "CLOSED")
                {
                    entity.ProjectStatus = ProjectStatus.CLOSED;

                }
                db = new ExceedDbContext();
                db.ProjectInformation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return new JavaScriptResult() { Script = "alert('" + "The Project state is changed to: " + state + "');" };
            }
            else
            {
                return new JavaScriptResult() { Script = "alert('" + "This Project is not found!!"+"');" };  
            }
           
        }

        [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin, ConstructionConstants.SchedulingUser, ConstructionConstants.ScheduleApprover)]
        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(p => !p.IsDeleted).AsQueryable();//.Where (x=> x.ProjectStatus == ProjectStatus .STARTED );
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new {
                ProjectInformationId = projectInformation.ProjectInformationId,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                ProjectType = projectInformation.ProjectType,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                ProjectStatus = projectInformation.ProjectStatus,

                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy, 
               });

            return Json(result);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectInformation_Create([DataSourceRequest]DataSourceRequest request, ProjectInformation projectInformation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectInformation
                {
                    ProjectType = projectInformation.ProjectType,
                    Name = projectInformation.Name,
                    Code = projectInformation.Code,
                    Category = projectInformation.Category,
                    BranchId = projectInformation.BranchId,
                    ContractAmount = projectInformation.ContractAmount,
                    ProjectScope = projectInformation.ProjectScope,
                    EmployeeId = projectInformation.EmployeeId,
                    Client = projectInformation.Client,
                    Consultant = projectInformation.Consultant,
                    OrgBranchName = projectInformation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectInformation.LastModified,
                    CreatedBy =User.Identity.Name ,
                    ModifiedBy = projectInformation.ModifiedBy,
                    ProjectStatus = projectInformation.ProjectStatus 
                    
                };

                db.ProjectInformation.Add(entity);
                db.SaveChanges();
                projectInformation.ProjectInformationId = entity.ProjectInformationId;
            }

            return Json(new[] { projectInformation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectInformation_Update([DataSourceRequest]DataSourceRequest request, ProjectInformation projectInformation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectInformation
                {
                    ProjectType = projectInformation.ProjectType,
                    ProjectInformationId = projectInformation.ProjectInformationId,
                    Name = projectInformation.Name,
                    Code = projectInformation.Code,
                    Category = projectInformation.Category,
                    BranchId = projectInformation.BranchId,
                    ContractAmount = projectInformation.ContractAmount,
                    ProjectScope = projectInformation.ProjectScope,
                    EmployeeId = projectInformation.EmployeeId,
                    Client = projectInformation.Client,
                    Consultant = projectInformation.Consultant,

                    OrgBranchName = projectInformation.OrgBranchName,
                    DateCreated = projectInformation.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectInformation.CreatedBy,
                    ModifiedBy = User .Identity .Name  ,
                    ProjectStatus = projectInformation.ProjectStatus 
                    
                };

                db.ProjectInformation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectInformation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectInformation_Destroy([DataSourceRequest]DataSourceRequest request, ProjectInformation projectInformation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectInformation
                {
                    ProjectType = projectInformation.ProjectType,
                    ProjectInformationId = projectInformation.ProjectInformationId,
                    Name = projectInformation.Name,
                    Code = projectInformation.Code,
                    Category = projectInformation.Category,
                    BranchId = projectInformation.BranchId,
                    ContractAmount = projectInformation.ContractAmount,
                    ProjectScope = projectInformation.ProjectScope,
                    EmployeeId = projectInformation.EmployeeId,
                    Client = projectInformation.Client,
                    Consultant = projectInformation.Consultant,
                    OrgBranchName = projectInformation.OrgBranchName,
                    DateCreated = projectInformation.DateCreated,
                    LastModified = projectInformation.LastModified,
                    CreatedBy = projectInformation.CreatedBy,
                    ModifiedBy = projectInformation.ModifiedBy ,
                    ProjectStatus = projectInformation.ProjectStatus ,

                    IsDeleted = true,
                    DeletedBy = User.Identity.Name,
                    DeletedOn = DateTime.Now
                   
                };

                db.ProjectInformation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectInformation }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectInformation_Destroy_Permanently([DataSourceRequest]DataSourceRequest request, ProjectInformation projectInformation)
        {
            if (ModelState.IsValid)
            {
                ProjectInformationHelper.CleanRemainingDataWhenProjectIsDeleted(projectInformation.ProjectInformationId);
                var entity = new ProjectInformation
                {
                    ProjectType = projectInformation.ProjectType,
                    ProjectInformationId = projectInformation.ProjectInformationId,
                    Name = projectInformation.Name,
                    Code = projectInformation.Code,
                    Category = projectInformation.Category,
                    BranchId = projectInformation.BranchId,
                    ContractAmount = projectInformation.ContractAmount,
                    ProjectScope = projectInformation.ProjectScope,
                    EmployeeId = projectInformation.EmployeeId,
                    Client = projectInformation.Client,
                    Consultant = projectInformation.Consultant,
                    OrgBranchName = projectInformation.OrgBranchName,
                    DateCreated = projectInformation.DateCreated,
                    LastModified = projectInformation.LastModified,
                    CreatedBy = projectInformation.CreatedBy,
                    ModifiedBy = projectInformation.ModifiedBy,
                    ProjectStatus = projectInformation.ProjectStatus

                };

                db.ProjectInformation.Attach(entity);
                db.ProjectInformation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectInformation }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName).Split('.')[1];
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" +  DataProviderController.EmployeeId.ToString() + "-" +
                                                    AttachmentType.PHOTO),
                             DataProviderController.EmployeeId.ToString() + "." + fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" +  DataProviderController.EmployeeId.ToString() + "-" +
                                                    AttachmentType.PHOTO));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (System.IO.File.Exists(path))
                    {

                        try
                        {
                            System.IO.File.Delete(path);
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                            //  return;
                        }
                    }
                    file.SaveAs(path);
                    var checkExistance =
                        db.Attachments.Where(
                            x =>
                                x.Reference == DataProviderController.EmployeeId.ToString() &&
                                x.Description == DataProviderController.EmployeeId.ToString() + "." + fileName &&
                                x.ReferenceType == AttachmentType.PHOTO).ToList();
                    if (checkExistance != null)
                    {
                        db.Attachments.RemoveRange(checkExistance);
                        db.SaveChanges();
                    }
                    db = new ExceedDbContext();
                    var entity = new Attachment
                    {

                        Reference =  DataProviderController.EmployeeId.ToString(),
                        Description =  DataProviderController.EmployeeId.ToString() + "." + fileName,
                        FileType = file.ContentType,
                        ReferenceType = AttachmentType.PHOTO,
                        Index = 0,
                        URL = path,
                        Remark =  DataProviderController.Owner
                    };
                    db.Attachments.Add(entity);

                    db.SaveChanges();
                    ViewBag.Message = "Upload successful";
                }

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
             DataProviderController.SelectedFile = id;
             
        }
        public ActionResult DownloadFile(string filename)
        {
            if ( DataProviderController.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.PROJECT_DOCUMENT && x.Remark ==  DataProviderController.Owner && x.AttachmentId ==  DataProviderController.SelectedFile)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" +  DataProviderController.EmployeeId.ToString() + "-" + AttachmentType.PROJECT_DOCUMENT + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                     DataProviderController.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (System.Exception)
            {

                //throw;
            }
             DataProviderController.SelectedFile = 0;
            return null;
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
        public ActionResult DownloadDoument(string filename)
        {
           
            if ( DataProviderController.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.PROJECT_DOCUMENT && x.Remark ==  DataProviderController.Owner && x.AttachmentId ==  DataProviderController.SelectedFile)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" +  DataProviderController.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                     DataProviderController.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
             DataProviderController.SelectedFile = 0;
            return null;
        }
      
        public ActionResult UploadDocument(HttpPostedFileBase file, string url, string txtValue, string txtDecsion, string id)
        {
            try
            {
               
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" +  DataProviderController.EmployeeId.ToString() + "-" +
                                           AttachmentType.PROJECT_DOCUMENT), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" +  DataProviderController.EmployeeId.ToString() + "-" +
                                           AttachmentType.PROJECT_DOCUMENT));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference =  DataProviderController.EmployeeId.ToString(),
                            Description = file.FileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.PROJECT_DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark =  DataProviderController.EmployeeId.ToString()
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
