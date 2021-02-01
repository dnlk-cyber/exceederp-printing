using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ExceedERP.Core.Domain.Global;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class DownloadDocumentController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
        
            return View();
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        

        public ActionResult DownloadFile(string filename)
        {
           
                db = new ExceedDbContext();
                var attachment =
                    db.Attachments.Where(x => x.Description == filename && x.ReferenceType == AttachmentType.DOCUMENT && x.AttachmentId == HRMInputs.SelectedFile)
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
                    "attachment;filename=\"" +filename + "\"");
                byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/0-DOCUMENT/" + filename));
                response.BinaryWrite(datas);
                response.End();
                return File(filename, attachment.FileType);
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
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
                RemoveFiles(attachment.Description);
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {
               
                    var physicalPath = Path.Combine(Server.MapPath("~/Content/Images/0-DOCUMENT"), fileName);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // Deleting the File from the Location
                        System.IO.File.Delete(physicalPath);
                    }
                }
            
            return Content("");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
