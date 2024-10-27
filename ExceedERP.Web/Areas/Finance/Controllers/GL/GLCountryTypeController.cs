using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class GlCountryTypeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GLCountryTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLCountryType> glcountrytypes = _db.GLCountryTypes;
            DataSourceResult result = glcountrytypes.ToDataSourceResult(request, gLCountryType => new {
                GLCountryTypeID = gLCountryType.GLCountryTypeID,
                CountryName = gLCountryType.CountryName,
                Description = gLCountryType.Description,
                DateCreated = gLCountryType.DateCreated,
                LastModified = gLCountryType.LastModified,
                CreatedBy = gLCountryType.CreatedBy,
                ModifiedBy = gLCountryType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLCountryTypes_Create([DataSourceRequest]DataSourceRequest request, GLCountryType gLCountryType)
        {
			#region validation

			var exists = _db.GLCountryTypes.Where(x => x.GLCountryTypeID != gLCountryType.GLCountryTypeID).Where(x => x.CountryName.ToUpper().Equals(gLCountryType.CountryName.ToUpper())).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("", gLCountryType.CountryName + " already exists in the system.");
			}

			#endregion
			if (ModelState.IsValid)
            {
                var entity = new GLCountryType
                {
                    CountryName = gLCountryType.CountryName,
                    Description = gLCountryType.Description,
                    DateCreated = DateTime.Now,
                    LastModified = gLCountryType.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gLCountryType.ModifiedBy
                };

                _db.GLCountryTypes.Add(entity);
                _db.SaveChanges();
                gLCountryType.GLCountryTypeID = entity.GLCountryTypeID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL country setup", "Success", User.Identity.Name, $"created country {entity.CountryName}  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLCountryType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLCountryTypes_Update([DataSourceRequest]DataSourceRequest request, GLCountryType gLCountryType)
        {
			#region validation

			var exists = _db.GLCountryTypes.Where(x => x.GLCountryTypeID != gLCountryType.GLCountryTypeID).Where(x => x.CountryName.ToUpper().Equals(gLCountryType.CountryName.ToUpper())).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", gLCountryType.CountryName + " already exists in the system.");
			}

			#endregion
			if (ModelState.IsValid)
            {
                var entity = new GLCountryType
                {
                    GLCountryTypeID = gLCountryType.GLCountryTypeID,
                    CountryName = gLCountryType.CountryName,
                    Description = gLCountryType.Description,
                    DateCreated = gLCountryType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = gLCountryType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.GLCountryTypes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL country setup", "Success", User.Identity.Name, $"updated country {entity.CountryName}  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLCountryType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLCountryTypes_Destroy([DataSourceRequest]DataSourceRequest request, GLCountryType gLCountryType)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLCountryType
                {
                    GLCountryTypeID = gLCountryType.GLCountryTypeID,
                    CountryName = gLCountryType.CountryName,
                    Description = gLCountryType.Description,
                    DateCreated = gLCountryType.DateCreated,
                    LastModified = gLCountryType.LastModified,
                    CreatedBy = gLCountryType.CreatedBy,
                    ModifiedBy = gLCountryType.ModifiedBy
                };

                _db.GLCountryTypes.Attach(entity);
                _db.GLCountryTypes.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLCountryType }.ToDataSourceResult(request, ModelState));
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
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
