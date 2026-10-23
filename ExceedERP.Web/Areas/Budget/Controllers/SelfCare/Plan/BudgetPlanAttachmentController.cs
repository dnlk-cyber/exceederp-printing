using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.SelfCare
{

    public class BudgetPlanAttachmentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        
      

         [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            BudgetPlanAttachment record = _db.BudgetPlanAttachments.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

   
        public ActionResult BudgetPlanAttachment_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<BudgetPlanAttachment> tenderminuteuploads = _db.BudgetPlanAttachments.Where(x => x.PlanCategoryID == id);
            DataSourceResult result = tenderminuteuploads.ToDataSourceResult(request, BudgetPlanAttachment => new {
                PlanCategoryID = BudgetPlanAttachment.PlanCategoryID,
                BudgetPlanAttachmentId = BudgetPlanAttachment.BudgetPlanAttachmentId,
                FileName = BudgetPlanAttachment.FileName,
                FilePath = BudgetPlanAttachment.FilePath
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanAttachment_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 2 * 1024 * 1024;

                string subPath = "~/App_Data/Budget/" + "Budget Plan" + id.ToString();  

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
                            _db.BudgetPlanAttachments.Add(new BudgetPlanAttachment()
                            {
                                FileName = Path.GetFileName(file.FileName),
                                PlanCategoryID = id,
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
        public ActionResult BudgetPlanAttachment_Update([DataSourceRequest]DataSourceRequest request, BudgetPlanAttachment BudgetPlanAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanAttachment
                {
                    BudgetPlanAttachmentId = BudgetPlanAttachment.BudgetPlanAttachmentId,
                    FileName = BudgetPlanAttachment.FileName,
                    FilePath = BudgetPlanAttachment.FilePath
                };

                _db.BudgetPlanAttachments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { BudgetPlanAttachment }.ToDataSourceResult(request, ModelState));
        }

     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanAttachment_Destroy([DataSourceRequest]DataSourceRequest request, BudgetPlanAttachment BudgetPlanAttachment)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanAttachment
                {

                    BudgetPlanAttachmentId = BudgetPlanAttachment.BudgetPlanAttachmentId,
                    PlanCategoryID = BudgetPlanAttachment.PlanCategoryID,
                    FileName = BudgetPlanAttachment.FileName,
                    FilePath = BudgetPlanAttachment.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.BudgetPlanAttachments.Attach(entity);
                _db.BudgetPlanAttachments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { BudgetPlanAttachment }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
