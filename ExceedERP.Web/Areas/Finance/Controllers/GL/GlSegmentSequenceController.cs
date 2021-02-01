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
    public class GlSegmentSequenceController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
       

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
            ViewBag.Count = _db.GlSegmentSequences.Count();
            return View();
        }
        
       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult GlSegmentSequences_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlSegmentSequence> glsegmentsequences = _db.GlSegmentSequences;
            DataSourceResult result = glsegmentsequences.ToDataSourceResult(request, glSegmentSequence => new {
                GlSegmentSequenceId = glSegmentSequence.GlSegmentSequenceId,
                FirstSegment = glSegmentSequence.FirstSegment,
                SecondSegment = glSegmentSequence.SecondSegment,
                ThirdSegment = glSegmentSequence.ThirdSegment,
                FourthSegment = glSegmentSequence.FourthSegment,
                DateCreated = glSegmentSequence.DateCreated,
                LastModified = glSegmentSequence.LastModified,
                CreatedBy = glSegmentSequence.CreatedBy,
                ModifiedBy = glSegmentSequence.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentSequences_Create([DataSourceRequest]DataSourceRequest request, GlSegmentSequence glSegmentSequence)
        {
            if (ModelState.IsValid)
            {
                var valid = Validate(glSegmentSequence);

                if (!valid)
                {
                    ModelState.AddModelError("", "A segment cannot be repeated.");
                }
              
                var entity = new GlSegmentSequence
                {
                    FirstSegment = glSegmentSequence.FirstSegment,
                    SecondSegment = glSegmentSequence.SecondSegment,
                    ThirdSegment = glSegmentSequence.ThirdSegment,
                    FourthSegment = glSegmentSequence.FourthSegment,
                    DateCreated = DateTime.Now,
                    LastModified = glSegmentSequence.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = glSegmentSequence.ModifiedBy
                };

                if (valid)
                {
                    _db.GlSegmentSequences.Add(entity);
                    _db.SaveChanges();
                    glSegmentSequence.GlSegmentSequenceId = entity.GlSegmentSequenceId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL segment sequence setup", "Success", User.Identity.Name, $"created segment sequence setup {entity.GlSegmentSequenceId}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { glSegmentSequence }.ToDataSourceResult(request, ModelState));
        }
        
       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentSequences_Update([DataSourceRequest]DataSourceRequest request, GlSegmentSequence glSegmentSequence)
        {
            if (ModelState.IsValid)
            {
                var valid = Validate(glSegmentSequence);

                if (!valid)
                {
                    ModelState.AddModelError("", "A segment cannot be repeated.");
                }

                var entity = new GlSegmentSequence
                {
                    GlSegmentSequenceId = glSegmentSequence.GlSegmentSequenceId,
                    FirstSegment = glSegmentSequence.FirstSegment,
                    SecondSegment = glSegmentSequence.SecondSegment,
                    ThirdSegment = glSegmentSequence.ThirdSegment,
                    FourthSegment = glSegmentSequence.FourthSegment,
                    DateCreated = glSegmentSequence.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = glSegmentSequence.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                if (valid)
                {
                    _db.GlSegmentSequences.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL segment sequence setup", "Success", User.Identity.Name, $"updated segment sequence setup {entity.GlSegmentSequenceId}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { glSegmentSequence }.ToDataSourceResult(request, ModelState));
        }

        private bool Validate(GlSegmentSequence glSegmentSequence)
        {
            bool valid = !(glSegmentSequence.FirstSegment == glSegmentSequence.SecondSegment ||
                          glSegmentSequence.FirstSegment == glSegmentSequence.ThirdSegment ||
                          glSegmentSequence.FirstSegment == glSegmentSequence.FourthSegment ||
                          glSegmentSequence.SecondSegment == glSegmentSequence.ThirdSegment ||
                          glSegmentSequence.SecondSegment == glSegmentSequence.FourthSegment ||
                          glSegmentSequence.ThirdSegment == glSegmentSequence.FourthSegment);
            return valid;
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
