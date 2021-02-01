//using ExceedERP.Core.Domain.Construction;
//using ExceedERP.DataAccess.Context;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
//{
//    public class ResourceController : Controller
//    {
//        private ExceedDbContext db = new ExceedDbContext();
//        public ActionResult Index()
//        {
//            return View();
//        }

//        public virtual JsonResult Resources_Read([DataSourceRequest]DataSourceRequest request)
//        {
//            //IQueryable<Resource> resources = db.Resources;
//            //DataSourceResult result = resources.ToDataSourceResult(request, resource => new ResourceViewModel {
//            //    ID = resource.ID,
//            //    ResourceTypeId = resource.ResourceTypeId,
//            //    Name = resource.Name,
//            //    Color = resource.Color,
//            //    UnitofMeasure = resource.UnitofMeasure,
//            //    PrimaryRole = resource.PrimaryRole
//            //});

//            return Json(resourceService.GetAll().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Resources_Create([DataSourceRequest]DataSourceRequest request, ResourceViewModel resourceViewModel)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = resourceViewModel.ToEntity();

//                db.Resources.Add(entity);
//                db.SaveChanges();
//                resourceViewModel.ID = entity.ID;
//            }

//            return Json(new[] { resourceViewModel }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Resources_Update([DataSourceRequest]DataSourceRequest request, ResourceViewModel resourceVM)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = resourceVM.ToEntity();

//                db.Resources.Attach(entity);
//                db.Entry(entity).State = EntityState.Modified;
//                db.SaveChanges();
//            }

//            return Json(new[] { resourceVM }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Resources_Destroy([DataSourceRequest]DataSourceRequest request, ResourceViewModel resourceVM)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = resourceVM.ToEntity();

//                db.Resources.Attach(entity);
//                db.Resources.Remove(entity);
//                db.SaveChanges();
//            }

//            return Json(new[] { resourceVM }.ToDataSourceResult(request, ModelState));
//        }

//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}