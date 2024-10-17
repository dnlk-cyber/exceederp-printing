using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class FixedAssetRequisitionAttachmentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();        
      

         [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            FixedAssetRequisitionAttachment record = _db.FixedAssetRequisitionAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

   
        public ActionResult FixedAssetRequisitionAttachment_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<FixedAssetRequisitionAttachment> attachements = _db.FixedAssetRequisitionAttachments.Where(x => x.FixedAssetRequisitionId == id);
            DataSourceResult result = attachements.ToDataSourceResult(request, FixedAssetRequisitionAttachment => new {
                FixedAssetRequisitionId = FixedAssetRequisitionAttachment.FixedAssetRequisitionId,
                FixedAssetRequisitionAttachmentId = FixedAssetRequisitionAttachment.FixedAssetRequisitionAttachmentId,
                FileName = FixedAssetRequisitionAttachment.FileName,
                FilePath = FixedAssetRequisitionAttachment.FilePath
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionAttachment_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {

                string subPath = "~/App_Data/FixedAssetRequisition/" + "SR"+ id.ToString();  

                bool exists = Directory.Exists(Server.MapPath(subPath));
               
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
              
                 foreach (var file in attachments)
                 {
                      string fileName = Path.GetFileName(file.FileName);
                     string physicalPath = Server.MapPath(subPath +"/"+ fileName);
                  //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);
                    _db.FixedAssetRequisitionAttachments.Add(new FixedAssetRequisitionAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        FixedAssetRequisitionId = id,
                        FilePath = physicalPath
                        
                    });
                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender minutes of meeting", "Success", User.Identity.Name, $"created minute under tender {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionAttachment_Update([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionAttachment FixedAssetRequisitionAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionAttachment
                {
                    FixedAssetRequisitionAttachmentId = FixedAssetRequisitionAttachment.FixedAssetRequisitionAttachmentId,
                    FileName = FixedAssetRequisitionAttachment.FileName,
                    FilePath = FixedAssetRequisitionAttachment.FilePath
                };

                _db.FixedAssetRequisitionAttachments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { FixedAssetRequisitionAttachment }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionAttachment_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionAttachment FixedAssetRequisitionAttachment)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionAttachment
                {

                    FixedAssetRequisitionAttachmentId = FixedAssetRequisitionAttachment.FixedAssetRequisitionAttachmentId,
                    FixedAssetRequisitionId = FixedAssetRequisitionAttachment.FixedAssetRequisitionId,
                    FileName = FixedAssetRequisitionAttachment.FileName,
                    FilePath = FixedAssetRequisitionAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.FixedAssetRequisitionAttachments.Attach(entity);
                _db.FixedAssetRequisitionAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { FixedAssetRequisitionAttachment }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
