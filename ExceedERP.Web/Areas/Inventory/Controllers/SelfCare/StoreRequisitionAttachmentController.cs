using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.SelfCare
{
   
    public class StoreRequisitionAttachmentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();        
      

         [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            StoreRequisitionAttachment record = _db.StoreRequisitionAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

   
        public ActionResult StoreRequisitionAttachment_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<StoreRequisitionAttachment> attachements = _db.StoreRequisitionAttachments.Where(x => x.StoreRequisitionID == id);
            DataSourceResult result = attachements.ToDataSourceResult(request, StoreRequisitionAttachment => new {
                StoreRequisitionID = StoreRequisitionAttachment.StoreRequisitionID,
                StoreRequisitionAttachmentId = StoreRequisitionAttachment.StoreRequisitionAttachmentId,
                FileName = StoreRequisitionAttachment.FileName,
                FilePath = StoreRequisitionAttachment.FilePath
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequisitionAttachment_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {

                string subPath = "~/App_Data/StoreRequisition/" + "PaymentRequest"+ id.ToString();  

                bool exists = Directory.Exists(Server.MapPath(subPath));
               
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
              
                 foreach (var file in attachments)
                 {
                      string fileName = Path.GetFileName(file.FileName);
                     string physicalPath = Server.MapPath(subPath +"/"+ fileName);
                  //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);
                    _db.StoreRequisitionAttachments.Add(new StoreRequisitionAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        StoreRequisitionID = id,
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
        public ActionResult StoreRequisitionAttachment_Update([DataSourceRequest]DataSourceRequest request, StoreRequisitionAttachment StoreRequisitionAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionAttachment
                {
                    StoreRequisitionAttachmentId = StoreRequisitionAttachment.StoreRequisitionAttachmentId,
                    FileName = StoreRequisitionAttachment.FileName,
                    FilePath = StoreRequisitionAttachment.FilePath
                };

                _db.StoreRequisitionAttachments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { StoreRequisitionAttachment }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequisitionAttachment_Destroy([DataSourceRequest]DataSourceRequest request, StoreRequisitionAttachment StoreRequisitionAttachment)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionAttachment
                {

                    StoreRequisitionAttachmentId = StoreRequisitionAttachment.StoreRequisitionAttachmentId,
                    StoreRequisitionID = StoreRequisitionAttachment.StoreRequisitionID,
                    FileName = StoreRequisitionAttachment.FileName,
                    FilePath = StoreRequisitionAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.StoreRequisitionAttachments.Attach(entity);
                _db.StoreRequisitionAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { StoreRequisitionAttachment }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
