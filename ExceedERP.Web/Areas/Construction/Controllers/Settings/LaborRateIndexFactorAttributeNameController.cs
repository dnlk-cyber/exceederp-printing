using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class LaborRateIndexFactorAttributeNameController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/LaborRateIndexFactorAttributeName
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AttributeName_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LaborRateIndexFactorAttributeName> attributeNames = db.LaborRateIndexFactorAttributeNames;
            DataSourceResult result = attributeNames.ToDataSourceResult(request, attributeName => new
            {
                LaborRateIndexFactorAttributeNameId = attributeName.LaborRateIndexFactorAttributeNameId,
                Name = attributeName.Name,
                Description = attributeName.Description,
            });

            return Json(result);
        }

        private bool CheckExistance(string name)
        {
            var attributeName = db.LaborRateIndexFactorAttributeNames.Where(x => x.Name == name).FirstOrDefault();
            if (attributeName != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttributeName_Create([DataSourceRequest]DataSourceRequest request, LaborRateIndexFactorAttributeName attributeName)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(attributeName.Name))
                {
                    ModelState.AddModelError("", "attribute is already registered!!");
                }
                else
                {
                    var entity = new LaborRateIndexFactorAttributeName
                    {
                        LaborRateIndexFactorAttributeNameId = attributeName.LaborRateIndexFactorAttributeNameId,
                        Name = attributeName.Name,
                        Description = attributeName.Description,
                    };

                    db.LaborRateIndexFactorAttributeNames.Add(entity);
                    db.SaveChanges();
                    attributeName.LaborRateIndexFactorAttributeNameId = entity.LaborRateIndexFactorAttributeNameId;

                }
            }

            return Json(new[] { attributeName }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttributeName_Update([DataSourceRequest]DataSourceRequest request, LaborRateIndexFactorAttributeName attributeName)
        {
            if (ModelState.IsValid)
            {
                var entity = new LaborRateIndexFactorAttributeName
                {
                    LaborRateIndexFactorAttributeNameId = attributeName.LaborRateIndexFactorAttributeNameId,
                    Name = attributeName.Name,
                    Description = attributeName.Description,
                };

                db.LaborRateIndexFactorAttributeNames.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attributeName }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttributeName_Destroy([DataSourceRequest]DataSourceRequest request, LaborRateIndexFactorAttributeName attributeName)
        {
            if (ModelState.IsValid)
            {
                var entity = new LaborRateIndexFactorAttributeName
                {
                    LaborRateIndexFactorAttributeNameId = attributeName.LaborRateIndexFactorAttributeNameId,
                    Name = attributeName.Name,
                    Description = attributeName.Description,
                };

                db.LaborRateIndexFactorAttributeNames.Attach(entity);
                db.LaborRateIndexFactorAttributeNames.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attributeName }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}