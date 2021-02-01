using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
   
    public class TenderMinutesOfMeetingController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            TenderMinuteUploadDBModel record = _db.TenderMinuteUploads.Find(id);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult TenderMinuteUploads_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<TenderMinuteUploadDBModel> tenderminuteuploads = _db.TenderMinuteUploads.Where(x => x.TenderID == id);
            DataSourceResult result = tenderminuteuploads.ToDataSourceResult(request, tenderMinuteUploadDbModel => new {
                TenderID = tenderMinuteUploadDbModel.TenderID,
                TenderMinuteUploadDBModelId = tenderMinuteUploadDbModel.TenderMinuteUploadDBModelId,
                FileName = tenderMinuteUploadDbModel.FileName,
                FilePath = tenderMinuteUploadDbModel.FilePath
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMinuteUploads_Create([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> tenderMinuteUploadDbModel , int id)
        {
            if (tenderMinuteUploadDbModel != null)
            {

                string subPath = "~/App_Data/Tender/" + "Tender"+ id.ToString();  

                bool exists = Directory.Exists(Server.MapPath(subPath));
               
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
              
                 foreach (var file in tenderMinuteUploadDbModel)
                 {
                      string fileName = Path.GetFileName(file.FileName);
                     string physicalPath = Server.MapPath(subPath +"/"+ fileName);
                  //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                    file.SaveAs(physicalPath);
                    _db.TenderMinuteUploads.Add(new TenderMinuteUploadDBModel()
                    {
                        FileName = Path.GetFileName(file.FileName),
                        TenderID = id,
                        FilePath = physicalPath
                        
                    });
                }

                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender minutes of meeting", "Success", User.Identity.Name, $"created minute under tender {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderMinuteUploadDbModel }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMinuteUploads_Update([DataSourceRequest]DataSourceRequest request, TenderMinuteUploadDBModel tenderMinuteUploadDbModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderMinuteUploadDBModel
                {
                    TenderMinuteUploadDBModelId = tenderMinuteUploadDbModel.TenderMinuteUploadDBModelId,
                    FileName = tenderMinuteUploadDbModel.FileName,
                    FilePath = tenderMinuteUploadDbModel.FilePath
                };

                _db.TenderMinuteUploads.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { tenderMinuteUploadDbModel }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMinuteUploads_Destroy([DataSourceRequest]DataSourceRequest request, TenderMinuteUploadDBModel tenderMinuteUploadDbModel)
        {
          

           
            if (ModelState.IsValid)
            {
                var entity = new TenderMinuteUploadDBModel
                {

                    TenderMinuteUploadDBModelId = tenderMinuteUploadDbModel.TenderMinuteUploadDBModelId,
                    TenderID = tenderMinuteUploadDbModel.TenderID,
                    FileName = tenderMinuteUploadDbModel.FileName,
                    FilePath = tenderMinuteUploadDbModel.FilePath
                };
                if (System.IO.File.Exists(entity.FilePath))
                {
                    System.IO.File.Delete(entity.FilePath);
                }
                _db.TenderMinuteUploads.Attach(entity);
                _db.TenderMinuteUploads.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderMinuteUploadDbModel }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
