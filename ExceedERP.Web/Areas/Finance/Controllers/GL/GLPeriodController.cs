using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlPeriodController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GLPeriods_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<GLPeriod> glperiods = _db.GLPeriods.Where(x => x.GlFiscalYearId == id);
            DataSourceResult result = glperiods.ToDataSourceResult(request, gLPeriod => new
            {
                GlFiscalYearId = id,
                GLPeriodId = gLPeriod.GLPeriodId,
                Index = gLPeriod.Index,
                Name = gLPeriod.Name,
                DateFrom = gLPeriod.DateFrom,
                DateTo = gLPeriod.DateTo,
                Status = gLPeriod.Status,
                OrgBranchName = gLPeriod.OrgBranchName,
                DateCreated = gLPeriod.DateCreated,
                LastModified = gLPeriod.LastModified,
                CreatedBy = gLPeriod.CreatedBy,
                ModifiedBy = gLPeriod.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLPeriods_Create(int id, [DataSourceRequest]DataSourceRequest request, GLPeriod gLPeriod)
        {
            #region validation

            bool valid = true;
            var exists = _db.GLPeriods.Where(x => x.GLPeriodId != gLPeriod.GLPeriodId).Where(x => x.Name.ToUpper().Equals(gLPeriod.Name.ToUpper())).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("", gLPeriod.Name + " already exists in the system.");
                valid = false;
            }

            if (gLPeriod.DateFrom > gLPeriod.DateTo)
            {
                ModelState.AddModelError("", "Start Date should be before End Date.");
                valid = false;
            }

            var overlaps = _db.GLPeriods.Where(x => x.Name != gLPeriod.Name)
                                                .Where(x => x.DateFrom <= gLPeriod.DateTo)
                                                .Where(x => x.DateTo >= gLPeriod.DateFrom).ToList();
            if (overlaps.Any())
            {
                ModelState.AddModelError("", "The Period's range overlaps with one or more previous Periods.");
                valid = false;
            }
            #endregion
            if (ModelState.IsValid)
            {               
                var entity = new GLPeriod
                {
                    GlFiscalYearId = id,
                    Name = gLPeriod.Name,
                    DateFrom = gLPeriod.DateFrom.Date,
                    DateTo = gLPeriod.DateTo.Date,
                    Status = gLPeriod.Status,
                    OrgBranchName = gLPeriod.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = gLPeriod.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gLPeriod.ModifiedBy
                };
                _db.GLPeriods.Add(entity);
                _db.SaveChanges();
                gLPeriod.GLPeriodId = entity.GLPeriodId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL period", "Success", User.Identity.Name, $"created period {entity.Name}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLPeriod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLPeriods_Update([DataSourceRequest]DataSourceRequest request, GLPeriod gLPeriod, int id)
        {
            #region validation

            bool valid = true;
            var exists = _db.GLPeriods.Where(x => x.GLPeriodId != gLPeriod.GLPeriodId).Where(x => x.Name.ToUpper().Equals(gLPeriod.Name.ToUpper())).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("", gLPeriod.Name + " already exists in the system.");
                valid = false;
            }

            if (gLPeriod.DateFrom > gLPeriod.DateTo)
            {
                ModelState.AddModelError("", "Start Date should be before End Date.");
                valid = false;
            }

            var overlaps = _db.GLPeriods.Where(x => x.Name != gLPeriod.Name)
                                                .Where(x => x.DateFrom <= gLPeriod.DateTo)
                                                .Where(x => x.DateTo >= gLPeriod.DateFrom).ToList();
            if (overlaps.Any())
            {
                ModelState.AddModelError("", "The Period's range overlaps with one or more previous Periods.");
                valid = false;
            }
            #endregion
            if (ModelState.IsValid)
            {
                
             
                    var entity = new GLPeriod
                    {
                        GlFiscalYearId = id,
                        GLPeriodId = gLPeriod.GLPeriodId,
                        Name = gLPeriod.Name,
                        DateFrom = gLPeriod.DateFrom,
                        DateTo = gLPeriod.DateTo,
                        Status = gLPeriod.Status,
                        OrgBranchName = gLPeriod.OrgBranchName,
                        DateCreated = gLPeriod.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = gLPeriod.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.GLPeriods.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL period", "Success", User.Identity.Name, $"updated period {entity.Name}", Modules.Finance);
                    #endregion
                
            }

            return Json(new[] { gLPeriod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLPeriods_Destroy([DataSourceRequest]DataSourceRequest request, GLPeriod gLPeriod)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLPeriod
                {
                    GLPeriodId = gLPeriod.GLPeriodId,
                    Name = gLPeriod.Name,
                    DateFrom = gLPeriod.DateFrom,
                    DateTo = gLPeriod.DateTo,
                    Status = gLPeriod.Status,
                    OrgBranchName = gLPeriod.OrgBranchName,
                    DateCreated = gLPeriod.DateCreated,
                    LastModified = gLPeriod.LastModified,
                    CreatedBy = gLPeriod.CreatedBy,
                    ModifiedBy = gLPeriod.ModifiedBy
                };

                _db.GLPeriods.Attach(entity);
                _db.GLPeriods.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLPeriod }.ToDataSourceResult(request, ModelState));
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
