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
    public class GlDynamicSegmentSetupController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GlSegmentSetups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLSegmentSetup> glsegmentsetups = _db.GlSegmentSetups;
            DataSourceResult result = glsegmentsetups.ToDataSourceResult(request, gLSegmentSetup => new {
                GLSegmentSetupId = gLSegmentSetup.GLSegmentSetupId,
                Name = gLSegmentSetup.Name,
                Description = gLSegmentSetup.Description,
                SegmentLength = gLSegmentSetup.SegmentLength,
                Sequence = gLSegmentSetup.Sequence,
                Displayed = gLSegmentSetup.Displayed,
                Enabled = gLSegmentSetup.Enabled,
                SegmentType = gLSegmentSetup.SegmentType,
                DateCreated = gLSegmentSetup.DateCreated,
                LastModified = gLSegmentSetup.LastModified,
                CreatedBy = gLSegmentSetup.CreatedBy,
                ModifiedBy = gLSegmentSetup.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentSetups_Create([DataSourceRequest]DataSourceRequest request, GLSegmentSetup gLSegmentSetup)
        {
            #region validation
            var exists = _db.GlSegmentSetups.Where(x => x.SegmentType == gLSegmentSetup.SegmentType && gLSegmentSetup.SegmentType != DynamicSegmentTypes.Other)
                .ToList();
            if (exists.Any())
                ModelState.AddModelError("Segment Type", gLSegmentSetup.SegmentType + " already exists in the system.");

            var nameexists = _db.GlSegmentSetups.Where(x => x.Name == gLSegmentSetup.Name)
                .ToList();
            if (nameexists.Any())
                ModelState.AddModelError("Segment Type", gLSegmentSetup.Name + " already exists in the system.");
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new GLSegmentSetup
                {
                    Name = gLSegmentSetup.Name,
                    Description = gLSegmentSetup.Description,
                    SegmentLength = gLSegmentSetup.SegmentLength,
                    Sequence = gLSegmentSetup.Sequence,
                    Displayed = gLSegmentSetup.Displayed,
                    Enabled = gLSegmentSetup.Enabled,
                    SegmentType = gLSegmentSetup.SegmentType,
                    DateCreated = gLSegmentSetup.DateCreated,
                    LastModified = gLSegmentSetup.LastModified,
                    CreatedBy = gLSegmentSetup.CreatedBy,
                    ModifiedBy = gLSegmentSetup.ModifiedBy
                };

                _db.GlSegmentSetups.Add(entity);
                _db.SaveChanges();
                gLSegmentSetup.GLSegmentSetupId = entity.GLSegmentSetupId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL dynamic chart of account segment type", "Success", User.Identity.Name, $"created segment {entity.Name} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLSegmentSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentSetups_Update([DataSourceRequest]DataSourceRequest request, GLSegmentSetup gLSegmentSetup)
        {

            #region validation
            var exists = _db.GlSegmentSetups.Where(x => x.GLSegmentSetupId != gLSegmentSetup.GLSegmentSetupId).Where(x => x.SegmentType == gLSegmentSetup.SegmentType && gLSegmentSetup.SegmentType != DynamicSegmentTypes.Other)
                .ToList();
            if (exists.Any())
                ModelState.AddModelError("Segment Type", gLSegmentSetup.SegmentType + " already exists in the system.");

            var nameexists = _db.GlSegmentSetups.Where(x => x.GLSegmentSetupId != gLSegmentSetup.GLSegmentSetupId).Where(x => x.Name == gLSegmentSetup.Name)
                .ToList();
            if (nameexists.Any())
                ModelState.AddModelError("Segment Type", gLSegmentSetup.Name + " already exists in the system.");
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new GLSegmentSetup
                {
                    GLSegmentSetupId = gLSegmentSetup.GLSegmentSetupId,
                    Name = gLSegmentSetup.Name,
                    Description = gLSegmentSetup.Description,
                    SegmentLength = gLSegmentSetup.SegmentLength,
                    Sequence = gLSegmentSetup.Sequence,
                    Displayed = gLSegmentSetup.Displayed,
                    Enabled = gLSegmentSetup.Enabled,
                    SegmentType = gLSegmentSetup.SegmentType,
                    DateCreated = gLSegmentSetup.DateCreated,
                    LastModified = gLSegmentSetup.LastModified,
                    CreatedBy = gLSegmentSetup.CreatedBy,
                    ModifiedBy = gLSegmentSetup.ModifiedBy
                };

                _db.GlSegmentSetups.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL dynamic chart of account segment type", "Success", User.Identity.Name, $"updated segment {entity.Name}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLSegmentSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentSetups_Destroy([DataSourceRequest]DataSourceRequest request, GLSegmentSetup gLSegmentSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLSegmentSetup
                {
                    GLSegmentSetupId = gLSegmentSetup.GLSegmentSetupId,
                    Name = gLSegmentSetup.Name,
                    Description = gLSegmentSetup.Description,
                    SegmentLength = gLSegmentSetup.SegmentLength,
                    Sequence = gLSegmentSetup.Sequence,
                    Displayed = gLSegmentSetup.Displayed,
                    Enabled = gLSegmentSetup.Enabled,
                    SegmentType = gLSegmentSetup.SegmentType,
                    DateCreated = gLSegmentSetup.DateCreated,
                    LastModified = gLSegmentSetup.LastModified,
                    CreatedBy = gLSegmentSetup.CreatedBy,
                    ModifiedBy = gLSegmentSetup.ModifiedBy
                };

                _db.GlSegmentSetups.Attach(entity);
                _db.GlSegmentSetups.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLSegmentSetup }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult Enable_Disable(string id)
        {
            if (id != null)
            {
                int did = int.Parse(id);
                var account = _db.GlSegmentSetups.FirstOrDefault(p => p.GLSegmentSetupId == did);
                if (account != null && account.Enabled)
                {
                    account.Enabled = false;
                }
                else
                {
                    if (account != null) account.Enabled = true;
                }

                if (account != null)
                {
                    _db.GlSegmentSetups.Attach(account);
                    _db.Entry(account).State = EntityState.Modified;
                }

                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Updated.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            string errMsg = "Record not found";
            var formattedData1 = new
            {
                Message = errMsg,
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
