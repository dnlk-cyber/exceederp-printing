using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.FundTransfers
{
   
    public class ApFundTransferRequestAttachmentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        
      

         [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            FundTransferAttachment record = _db.FundTransferAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

   
        public ActionResult FundTransferAttachment_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<FundTransferAttachment> tenderminuteuploads = _db.FundTransferAttachments.Where(x => x.FundTransferId == id);
            DataSourceResult result = tenderminuteuploads.ToDataSourceResult(request, FundTransferAttachment => new {
                FundTransferId = FundTransferAttachment.FundTransferId,
                FundTransferAttachmentId = FundTransferAttachment.FundTransferAttachmentId,
                FileName = FundTransferAttachment.FileName,
                FilePath = FundTransferAttachment.FilePath
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransferAttachment_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {

                string subPath = "~/App_Data/ApFundTransferRequest/" + "FundTransfer"+ id.ToString();  

                bool exists = Directory.Exists(Server.MapPath(subPath));
               
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
              
                 foreach (var file in attachments)
                 {
                      string fileName = Path.GetFileName(file.FileName);
                     string physicalPath = Server.MapPath(subPath +"/"+ fileName);
                  //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);
                    _db.FundTransferAttachments.Add(new FundTransferAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        FundTransferId = id,
                        FilePath = physicalPath
                        
                    });
                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "fund transfer", "Success", User.Identity.Name, $"created attachment under fund transfer {id} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransferAttachment_Update([DataSourceRequest]DataSourceRequest request, FundTransferAttachment FundTransferAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new FundTransferAttachment
                {
                    FundTransferAttachmentId = FundTransferAttachment.FundTransferAttachmentId,
                    FileName = FundTransferAttachment.FileName,
                    FilePath = FundTransferAttachment.FilePath
                };

                _db.FundTransferAttachments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { FundTransferAttachment }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransferAttachment_Destroy([DataSourceRequest]DataSourceRequest request, FundTransferAttachment FundTransferAttachment)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new FundTransferAttachment
                {

                    FundTransferAttachmentId = FundTransferAttachment.FundTransferAttachmentId,
                    FundTransferId = FundTransferAttachment.FundTransferId,
                    FileName = FundTransferAttachment.FileName,
                    FilePath = FundTransferAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.FundTransferAttachments.Attach(entity);
                _db.FundTransferAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { FundTransferAttachment }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
