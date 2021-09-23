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
    public class GlCrossValidationRuleElementController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GlCrossValidationRuleElements_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<GLCrossValidationRuleElement> glcrossvalidationruleelements = _db.GlCrossValidationRuleElements.Where(x=>x.GLSegmentCrossValidationId==id);
            DataSourceResult result = glcrossvalidationruleelements.ToDataSourceResult(request, gLCrossValidationRuleElement => new {
                GLSegmentCrossValidationId = gLCrossValidationRuleElement.GLSegmentCrossValidationId,
                GLCrossValidationRuleElementId = gLCrossValidationRuleElement.GLCrossValidationRuleElementId,
                SegmentType = gLCrossValidationRuleElement.SegmentType,
                From = gLCrossValidationRuleElement.From,
                To = gLCrossValidationRuleElement.To,
                DateCreated = gLCrossValidationRuleElement.DateCreated,
                LastModified = gLCrossValidationRuleElement.LastModified,
                CreatedBy = gLCrossValidationRuleElement.CreatedBy,
                ModifiedBy = gLCrossValidationRuleElement.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlCrossValidationRuleElements_Create([DataSourceRequest]DataSourceRequest request, GLCrossValidationRuleElement gLCrossValidationRuleElement,int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLCrossValidationRuleElement
                {
                    GLSegmentCrossValidationId = id,
                    SegmentType = gLCrossValidationRuleElement.SegmentType,
                    From = gLCrossValidationRuleElement.From,
                    To = gLCrossValidationRuleElement.To,
                    DateCreated = gLCrossValidationRuleElement.DateCreated,
                    LastModified = gLCrossValidationRuleElement.LastModified,
                    CreatedBy = gLCrossValidationRuleElement.CreatedBy,
                    ModifiedBy = gLCrossValidationRuleElement.ModifiedBy
                };

                _db.GlCrossValidationRuleElements.Add(entity);
                _db.SaveChanges();
                gLCrossValidationRuleElement.GLCrossValidationRuleElementId = entity.GLCrossValidationRuleElementId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL cross validation element ", "Success", User.Identity.Name, $"created rule element {entity.SegmentType} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLCrossValidationRuleElement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlCrossValidationRuleElements_Update([DataSourceRequest]DataSourceRequest request, GLCrossValidationRuleElement gLCrossValidationRuleElement)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLCrossValidationRuleElement
                {
                    GLSegmentCrossValidationId = gLCrossValidationRuleElement.GLSegmentCrossValidationId,
                    GLCrossValidationRuleElementId = gLCrossValidationRuleElement.GLCrossValidationRuleElementId,
                    SegmentType = gLCrossValidationRuleElement.SegmentType,
                    From = gLCrossValidationRuleElement.From,
                    To = gLCrossValidationRuleElement.To,
                    DateCreated = gLCrossValidationRuleElement.DateCreated,
                    LastModified = gLCrossValidationRuleElement.LastModified,
                    CreatedBy = gLCrossValidationRuleElement.CreatedBy,
                    ModifiedBy = gLCrossValidationRuleElement.ModifiedBy
                };

                _db.GlCrossValidationRuleElements.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL cross validation element ", "Success", User.Identity.Name, $"updated rule element {entity.SegmentType} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLCrossValidationRuleElement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
