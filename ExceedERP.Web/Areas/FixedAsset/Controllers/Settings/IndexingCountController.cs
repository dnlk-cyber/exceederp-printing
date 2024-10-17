using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FixedAsset.Setting;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers
{
    public class IndexingCountController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var assetSubCategories = db.FixedAssetSubCategories.Select(
              i => new
              {
                  Name = i.SubCategoryCode + " " + i.SubCategoryName ,
                  Code = i.SubCategoryCode
              }).AsQueryable();

            ViewData["SubCategory"] = assetSubCategories;
            return View();
        }

        public ActionResult IndexingCount_Read([DataSourceRequest]DataSourceRequest request)
        {
            
            IQueryable<IndexingCount> indexingCounts  = db.IndexingCounts;
            DataSourceResult result = indexingCounts.ToDataSourceResult(request, indexingCount => new IndexingCount
            {
                IndexCode = indexingCount.IndexCode,
                SubCategoryCode = indexingCount.SubCategoryCode,
                CurrentValue = indexingCount.CurrentValue,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IndexingCount_Create([DataSourceRequest]DataSourceRequest request, IndexingCount indexingCount)
        {
            
            if (ModelState.IsValid)
            {
                var entity = new IndexingCount
                {
                    IndexCode = indexingCount.IndexCode.ToUpper(),
                    SubCategoryCode = indexingCount.SubCategoryCode,
                    CurrentValue = indexingCount.CurrentValue,
                };

                db.IndexingCounts.Add(entity);
                db.SaveChanges();
                indexingCount.IndexCode = entity.IndexCode;
            }

            return Json(new[] { indexingCount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IndexingCount_Update([DataSourceRequest]DataSourceRequest request, IndexingCount indexingCount)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new IndexingCount
                {
                    IndexCode = indexingCount.IndexCode.ToUpper(),
                    SubCategoryCode = indexingCount.SubCategoryCode,
                    CurrentValue = indexingCount.CurrentValue,
                };

                db.IndexingCounts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { indexingCount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IndexingCount_Destroy([DataSourceRequest]DataSourceRequest request, IndexingCount indexingCount)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new IndexingCount
                {
                    IndexCode = indexingCount.IndexCode,
                    SubCategoryCode = indexingCount.SubCategoryCode,
                    CurrentValue = indexingCount.CurrentValue,
                };

                db.IndexingCounts.Attach(entity);
                db.IndexingCounts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { indexingCount }.ToDataSourceResult(request, ModelState));
        }
      
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }       


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
