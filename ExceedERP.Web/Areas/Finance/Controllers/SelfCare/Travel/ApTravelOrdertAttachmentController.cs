using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{

    public class ApTravelOrdertAttachmentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        
      

         [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            ApTravelOrdertAttachment record = _db.ApTravelOrdertAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

   
        public ActionResult ApTravelOrdertAttachment_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<ApTravelOrdertAttachment> tenderminuteuploads = _db.ApTravelOrdertAttachments.Where(x => x.ApTravelOrderId == id);
            DataSourceResult result = tenderminuteuploads.ToDataSourceResult(request, ApTravelOrdertAttachment => new {
                ApTravelOrderId = ApTravelOrdertAttachment.ApTravelOrderId,
                ApTravelOrdertAttachmentId = ApTravelOrdertAttachment.ApTravelOrdertAttachmentId,
                FileName = ApTravelOrdertAttachment.FileName,
                FilePath = ApTravelOrdertAttachment.FilePath
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrdertAttachment_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {

                string subPath = "~/App_Data/ApTravelOrder/" + "TravelOrder" + id.ToString();  

                bool exists = Directory.Exists(Server.MapPath(subPath));
               
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
              
                 foreach (var file in attachments)
                 {
                      string fileName = Path.GetFileName(file.FileName);
                     string physicalPath = Server.MapPath(subPath +"/"+ fileName);
                  //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);
                    _db.ApTravelOrdertAttachments.Add(new ApTravelOrdertAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        ApTravelOrderId = id,
                        FilePath = physicalPath
                        
                    });
                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payment request", "Success", User.Identity.Name, $"created miattachmentnute under payment request {id} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrdertAttachment_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrdertAttachment ApTravelOrdertAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrdertAttachment
                {
                    ApTravelOrdertAttachmentId = ApTravelOrdertAttachment.ApTravelOrdertAttachmentId,
                    FileName = ApTravelOrdertAttachment.FileName,
                    FilePath = ApTravelOrdertAttachment.FilePath
                };

                _db.ApTravelOrdertAttachments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { ApTravelOrdertAttachment }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrdertAttachment_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrdertAttachment ApTravelOrdertAttachment)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrdertAttachment
                {

                    ApTravelOrdertAttachmentId = ApTravelOrdertAttachment.ApTravelOrdertAttachmentId,
                    ApTravelOrderId = ApTravelOrdertAttachment.ApTravelOrderId,
                    FileName = ApTravelOrdertAttachment.FileName,
                    FilePath = ApTravelOrdertAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.ApTravelOrdertAttachments.Attach(entity);
                _db.ApTravelOrdertAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { ApTravelOrdertAttachment }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
