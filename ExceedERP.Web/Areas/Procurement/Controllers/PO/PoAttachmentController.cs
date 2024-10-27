using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{
    [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
    public class PoAttachmentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();        
      

         [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            PoAttachment record = _db.PoAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

   
        public ActionResult PoAttachment_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<PoAttachment> attachements = _db.PoAttachments.Where(x => x.PurchaseOrderID == id);
            DataSourceResult result = attachements.ToDataSourceResult(request, PoAttachment => new {
                PurchaseOrderID = PoAttachment.PurchaseOrderID,
                PoAttachmentId = PoAttachment.PoAttachmentId,
                FileName = PoAttachment.FileName,
                FilePath = PoAttachment.FilePath
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoAttachment_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {

                string subPath = "~/App_Data/PO/" + "PO"+ id.ToString();  

                bool exists = Directory.Exists(Server.MapPath(subPath));
               
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
              
                 foreach (var file in attachments)
                 {
                      string fileName = Path.GetFileName(file.FileName);
                     string physicalPath = Server.MapPath(subPath +"/"+ fileName);
                  //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);
                    _db.PoAttachments.Add(new PoAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        PurchaseOrderID = id,
                        FilePath = physicalPath
                        
                    });
                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "PO attachment", "Success", User.Identity.Name, $"created attachement under po {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoAttachment_Update([DataSourceRequest]DataSourceRequest request, PoAttachment PoAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new PoAttachment
                {
                    PoAttachmentId = PoAttachment.PoAttachmentId,
                    FileName = PoAttachment.FileName,
                    FilePath = PoAttachment.FilePath
                };

                _db.PoAttachments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { PoAttachment }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoAttachment_Destroy([DataSourceRequest]DataSourceRequest request, PoAttachment PoAttachment)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new PoAttachment
                {

                    PoAttachmentId = PoAttachment.PoAttachmentId,
                    PurchaseOrderID = PoAttachment.PurchaseOrderID,
                    FileName = PoAttachment.FileName,
                    FilePath = PoAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.PoAttachments.Attach(entity);
                _db.PoAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { PoAttachment }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
