using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.PaymentRequests
{

    public class PaymentRequestAttachmentBudgetController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        
      

         [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            PaymentRequestAttachment record = _db.PaymentRequestAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

   
        public ActionResult PaymentRequestAttachment_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<PaymentRequestAttachment> tenderminuteuploads = _db.PaymentRequestAttachments.Where(x => x.PaymentRequestId == id);
            DataSourceResult result = tenderminuteuploads.ToDataSourceResult(request, PaymentRequestAttachment => new {
                PaymentRequestId = PaymentRequestAttachment.PaymentRequestId,
                PaymentRequestAttachmentId = PaymentRequestAttachment.PaymentRequestAttachmentId,
                FileName = PaymentRequestAttachment.FileName,
                FilePath = PaymentRequestAttachment.FilePath
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestAttachment_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 5 * 1024 * 1024;

                string subPath = "~/App_Data/ApPaymentRequest/" + "PaymentRequest"+ id.ToString();  

                bool exists = Directory.Exists(Server.MapPath(subPath));
               
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                foreach (var file in attachments)
                {
                    if (file != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                        }

                        if (file.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"File size exceeds the {MaxFileSize/(1024 * 1024)} MB limit.");
                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string physicalPath = Server.MapPath(subPath + "/" + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            file.SaveAs(physicalPath);
                            _db.PaymentRequestAttachments.Add(new PaymentRequestAttachment()
                            {
                                FileName = Path.GetFileName(file.FileName),
                                PaymentRequestId = id,
                                FilePath = physicalPath

                            });
                            _db.SaveChanges();
                        }
                    }
                }

              
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payment request", "Success", User.Identity.Name, $"created miattachmentnute under payment request {id} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestAttachment_Update([DataSourceRequest]DataSourceRequest request, PaymentRequestAttachment PaymentRequestAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new PaymentRequestAttachment
                {
                    PaymentRequestAttachmentId = PaymentRequestAttachment.PaymentRequestAttachmentId,
                    FileName = PaymentRequestAttachment.FileName,
                    FilePath = PaymentRequestAttachment.FilePath
                };

                _db.PaymentRequestAttachments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { PaymentRequestAttachment }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestAttachment_Destroy([DataSourceRequest]DataSourceRequest request, PaymentRequestAttachment PaymentRequestAttachment)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new PaymentRequestAttachment
                {

                    PaymentRequestAttachmentId = PaymentRequestAttachment.PaymentRequestAttachmentId,
                    PaymentRequestId = PaymentRequestAttachment.PaymentRequestId,
                    FileName = PaymentRequestAttachment.FileName,
                    FilePath = PaymentRequestAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.PaymentRequestAttachments.Attach(entity);
                _db.PaymentRequestAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { PaymentRequestAttachment }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
