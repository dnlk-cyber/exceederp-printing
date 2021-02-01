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
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjectUploadController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            //var periods = db.Periods.Where( x=> x.PeriodType ==PeriodType.FISCAL_YEAR ).AsQueryable();
            //ViewData["PeriodList"] = periods;
           
            return View();
        }

        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation;
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new {
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
                    Name = projectInformation.Name,
                    Code = projectInformation.Code,
                    Category = projectInformation.Category,
                    BranchId = projectInformation.BranchId,
                    ContractAmount = projectInformation.ContractAmount,
                    ProjectScope = projectInformation.ProjectScope,
                    EmployeeId = projectInformation.EmployeeId,
                    Client = projectInformation.Client,
                    Consultant = projectInformation.Consultant,
                   
                  
                    
                   
                   
                   
                   
                   
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
                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + DataProviderController.EmployeeId.ToString() + "-" +
                                           AttachmentType.PROJECT_DOCUMENT), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + DataProviderController.EmployeeId.ToString() + "-" +
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

                            Reference = DataProviderController.EmployeeId.ToString(),
                            Description = file.FileName,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.PROJECT_DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = DataProviderController.Owner
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

        public void Download(int id)
        {
            DataProviderController.SelectedFile = id;

        }
        public ActionResult DownloadFile(string filename)
        {
            if (DataProviderController.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.PROJECT_DOCUMENT && x.Remark == DataProviderController.Owner && x.AttachmentId == DataProviderController.SelectedFile)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + DataProviderController.EmployeeId.ToString() + "-" + AttachmentType.PROJECT_DOCUMENT + "/" + attachment.Description));
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

}
