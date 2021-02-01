using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlSegmentCrossValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GlSegmentCrossValidations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLSegmentCrossValidation> glsegmentcrossvalidations = _db.GlSegmentCrossValidations;
            DataSourceResult result = glsegmentcrossvalidations.ToDataSourceResult(request, gLSegmentCrossValidation => new {
                GLSegmentCrossValidationId = gLSegmentCrossValidation.GLSegmentCrossValidationId,
                Name = gLSegmentCrossValidation.Name,
                Description = gLSegmentCrossValidation.Description,
                Enabled = gLSegmentCrossValidation.Enabled,
                ErrMsg = gLSegmentCrossValidation.ErrMsg,
                ErrorSegment = gLSegmentCrossValidation.ErrorSegment,
                ErrorSegmentFrom = gLSegmentCrossValidation.ErrorSegmentFrom,
                ErrorSegmentTo = gLSegmentCrossValidation.ErrorSegmentTo,
                DateCreated = gLSegmentCrossValidation.DateCreated,
                LastModified = gLSegmentCrossValidation.LastModified,
                CreatedBy = gLSegmentCrossValidation.CreatedBy,
                ModifiedBy = gLSegmentCrossValidation.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentCrossValidations_Create([DataSourceRequest]DataSourceRequest request, GLSegmentCrossValidation gLSegmentCrossValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLSegmentCrossValidation
                {
                    Name = gLSegmentCrossValidation.Name,
                    Description = gLSegmentCrossValidation.Description,
                    Enabled = gLSegmentCrossValidation.Enabled,
                    ErrMsg = gLSegmentCrossValidation.ErrMsg,
                    ErrorSegment = gLSegmentCrossValidation.ErrorSegment,
                    ErrorSegmentFrom = gLSegmentCrossValidation.ErrorSegmentFrom,
                    ErrorSegmentTo = gLSegmentCrossValidation.ErrorSegmentTo,
                    DateCreated = gLSegmentCrossValidation.DateCreated,
                    LastModified = gLSegmentCrossValidation.LastModified,
                    CreatedBy = gLSegmentCrossValidation.CreatedBy,
                    ModifiedBy = gLSegmentCrossValidation.ModifiedBy
                };

                _db.GlSegmentCrossValidations.Add(entity);
                _db.SaveChanges();
                gLSegmentCrossValidation.GLSegmentCrossValidationId = entity.GLSegmentCrossValidationId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL cross validation setup", "Success", User.Identity.Name, $"created cross validation setup {entity.GLSegmentCrossValidationId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLSegmentCrossValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentCrossValidations_Update([DataSourceRequest]DataSourceRequest request, GLSegmentCrossValidation gLSegmentCrossValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLSegmentCrossValidation
                {
                    GLSegmentCrossValidationId = gLSegmentCrossValidation.GLSegmentCrossValidationId,
                    Name = gLSegmentCrossValidation.Name,
                    Description = gLSegmentCrossValidation.Description,
                    Enabled = gLSegmentCrossValidation.Enabled,
                    ErrMsg = gLSegmentCrossValidation.ErrMsg,
                    ErrorSegment = gLSegmentCrossValidation.ErrorSegment,
                    ErrorSegmentFrom = gLSegmentCrossValidation.ErrorSegmentFrom,
                    ErrorSegmentTo = gLSegmentCrossValidation.ErrorSegmentTo,
                    DateCreated = gLSegmentCrossValidation.DateCreated,
                    LastModified = gLSegmentCrossValidation.LastModified,
                    CreatedBy = gLSegmentCrossValidation.CreatedBy,
                    ModifiedBy = gLSegmentCrossValidation.ModifiedBy
                };

                _db.GlSegmentCrossValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL cross validation setup", "Success", User.Identity.Name, $"updated cross validation setup {entity.GLSegmentCrossValidationId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLSegmentCrossValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
