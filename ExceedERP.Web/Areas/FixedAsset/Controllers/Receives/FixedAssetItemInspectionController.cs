using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Receives
{

    public class FixedAssetItemInspectionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetInspector)]
        public ActionResult Index()
        {
            return View();
        }

        

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetInspector)]
        public ActionResult FixedAssetItemInspections_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetInspection> iteminspections = _db.FixedAssetInspections.Where(i => i.FixedAssetReceiveId == id);
            DataSourceResult result = iteminspections.ToDataSourceResult(request, fixedAssetInspection => new
            {
                FixedAssetInspectionId = fixedAssetInspection.FixedAssetInspectionId,
                FixedAssetReceiveId = fixedAssetInspection.FixedAssetReceiveId,
                InspectionDate = fixedAssetInspection.InspectionDate,
                EmployeeId = fixedAssetInspection.EmployeeId,
                Accepted = fixedAssetInspection.Accepted,
                Remark = fixedAssetInspection.Remark,

                CreatedBy = fixedAssetInspection.CreatedBy,
                DateCreated = fixedAssetInspection.DateCreated,
                ModifiedBy = fixedAssetInspection.ModifiedBy,
                LastModified = fixedAssetInspection.LastModified,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetInspector)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetItemInspections_Create([DataSourceRequest]DataSourceRequest request, FixedAssetInspection itemInspection, int id)
        {
           
          
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetInspection
                {
                    InspectionDate = itemInspection.InspectionDate,
                    FixedAssetReceiveId = id,
                    EmployeeId = itemInspection.EmployeeId,
                    Accepted = itemInspection.Accepted,
                    Remark = itemInspection.Remark,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = itemInspection.ModifiedBy,
                    LastModified = itemInspection.LastModified,
                    

                };

                _db.FixedAssetInspections.Add(entity);
                _db.SaveChanges();
                itemInspection.FixedAssetInspectionId = entity.FixedAssetInspectionId;
            }

            return Json(new[] { itemInspection }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetInspector)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetItemInspections_Update([DataSourceRequest]DataSourceRequest request, FixedAssetInspection itemInspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetInspection
                {
                    FixedAssetInspectionId = itemInspection.FixedAssetInspectionId,
                    InspectionDate = itemInspection.InspectionDate,
                    EmployeeId = itemInspection.EmployeeId,
                    Accepted = itemInspection.Accepted,
                    Remark = itemInspection.Remark,

                    CreatedBy=itemInspection.CreatedBy,
                    DateCreated=itemInspection.DateCreated,
                    ModifiedBy = User.Identity.Name,
                    LastModified=DateTime.Now

                };

                _db.FixedAssetInspections.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { itemInspection }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetInspector)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetItemInspections_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetInspection itemInspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetInspection
                {
                    FixedAssetInspectionId = itemInspection.FixedAssetInspectionId,
                    InspectionDate = itemInspection.InspectionDate,
                    EmployeeId = itemInspection.EmployeeId,
                    Accepted = itemInspection.Accepted,
                    Remark = itemInspection.Remark
                };

                _db.FixedAssetInspections.Attach(entity);
                _db.FixedAssetInspections.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { itemInspection }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
