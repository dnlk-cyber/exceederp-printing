using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class InspectionChecklistController : Controller
    {
        // GET: FleetManagement/InspectionChecklist
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult Status_Read([DataSourceRequest]DataSourceRequest request)
        {
            var FetchStatus = db.InspectionStatusKeys.AsQueryable();
            DataSourceResult Result = FetchStatus.ToDataSourceResult(request, x => new
            {
                InspectionStatusKeyId = x.InspectionStatusKeyId,
                Remark = x.Remark,
                StatusKey = x.StatusKey,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                LastModified = x.LastModified,
                ModifiedBy = x.ModifiedBy
                
            });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Status_Create([DataSourceRequest]DataSourceRequest request,InspectionStatusKey statusKey)

        public ActionResult Status_Create([DataSourceRequest]DataSourceRequest request, InspectionStatusKey inspectionStatusKey)
        {

            inspectionStatusKey.CreatedBy = User.Identity.Name;
            inspectionStatusKey.DateCreated = DateTime.Now;
            if(ModelState.IsValid)
            {
                var entity = (InspectionStatusKey)this.GetMappedStatus(inspectionStatusKey);
                db.InspectionStatusKeys.Add(entity);
                db.SaveChanges();
            }
          return Json(new[] { inspectionStatusKey }.ToDataSourceResult(request,ModelState));
        }
        private Object GetMappedStatus(InspectionStatusKey x)
            {
            var entity = new InspectionStatusKey
            {
                InspectionStatusKeyId = x.InspectionStatusKeyId,
                Remark = x.Remark,
                StatusKey = x.StatusKey,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                LastModified = x.LastModified,
                ModifiedBy = x.ModifiedBy
            };
            return entity;
        }

        public ActionResult Status_Destroy([DataSourceRequest]DataSourceRequest request, InspectionStatusKey inspectionStatusKey)
        {
            var FetchStatus = db.InspectionStatusKeys.FirstOrDefault(x => x.InspectionStatusKeyId == inspectionStatusKey.InspectionStatusKeyId);
            if(FetchStatus!=null)
            {
                db.InspectionStatusKeys.Remove(FetchStatus);
                db.SaveChanges();

            }
            return Json(new[] { inspectionStatusKey }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult Status_Update([DataSourceRequest]DataSourceRequest request, InspectionStatusKey inspectionStatusKey)
        {
            var FetchStatus = db.InspectionStatusKeys.FirstOrDefault(x => x.InspectionStatusKeyId == inspectionStatusKey.InspectionStatusKeyId);
            if(FetchStatus!=null)
            {
                FetchStatus.Remark = inspectionStatusKey.Remark;
                FetchStatus.StatusKey = inspectionStatusKey.StatusKey;
                FetchStatus.ModifiedBy = User.Identity.Name;
                FetchStatus.LastModified = DateTime.Now;
                // db.InspectionStatusKeys.Attach(FetchStatus);
                db.Entry(FetchStatus).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            return Json(new[] { inspectionStatusKey }.ToDataSourceResult(request, ModelState));

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}