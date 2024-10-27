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

    public class ApTravelExpenseClaimAttachmentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        
      

         [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            ApTravelExpenseClaimAttachment record = _db.ApTravelExpenseClaimAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

   
        public ActionResult ApTravelExpenseClaimAttachment_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<ApTravelExpenseClaimAttachment> tenderminuteuploads = _db.ApTravelExpenseClaimAttachments.Where(x => x.ApTravelExpenseClaimId == id);
            DataSourceResult result = tenderminuteuploads.ToDataSourceResult(request, ApTravelExpenseClaimAttachment => new {
                ApTravelExpenseClaimId = ApTravelExpenseClaimAttachment.ApTravelExpenseClaimId,
                ApTravelExpenseClaimAttachmentId = ApTravelExpenseClaimAttachment.ApTravelExpenseClaimAttachmentId,
                FileName = ApTravelExpenseClaimAttachment.FileName,
                FilePath = ApTravelExpenseClaimAttachment.FilePath
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseClaimAttachment_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {

                string subPath = "~/App_Data/ApTravelOrderClaim/" + "Claim" + id.ToString();  

                bool exists = Directory.Exists(Server.MapPath(subPath));
               
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
              
                 foreach (var file in attachments)
                 {
                      string fileName = Path.GetFileName(file.FileName);
                     string physicalPath = Server.MapPath(subPath +"/"+ fileName);
                  //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);
                    _db.ApTravelExpenseClaimAttachments.Add(new ApTravelExpenseClaimAttachment()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        ApTravelExpenseClaimId = id,
                        FilePath = physicalPath
                        
                    });
                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "travel claim", "Success", User.Identity.Name, $"created miattachmentnute under payment request {id} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseClaimAttachment_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaimAttachment ApTravelExpenseClaimAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseClaimAttachment
                {
                    ApTravelExpenseClaimAttachmentId = ApTravelExpenseClaimAttachment.ApTravelExpenseClaimAttachmentId,
                    FileName = ApTravelExpenseClaimAttachment.FileName,
                    FilePath = ApTravelExpenseClaimAttachment.FilePath
                };

                _db.ApTravelExpenseClaimAttachments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { ApTravelExpenseClaimAttachment }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseClaimAttachment_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaimAttachment ApTravelExpenseClaimAttachment)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseClaimAttachment
                {

                    ApTravelExpenseClaimAttachmentId = ApTravelExpenseClaimAttachment.ApTravelExpenseClaimAttachmentId,
                    ApTravelExpenseClaimId = ApTravelExpenseClaimAttachment.ApTravelExpenseClaimId,
                    FileName = ApTravelExpenseClaimAttachment.FileName,
                    FilePath = ApTravelExpenseClaimAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.ApTravelExpenseClaimAttachments.Attach(entity);
                _db.ApTravelExpenseClaimAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { ApTravelExpenseClaimAttachment }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
