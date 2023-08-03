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
    public class GLAccountConfigController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GLAccountConfigs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLAccountConfig> GLAccountConfigs = _db.GLAccountConfigs;
            DataSourceResult result = GLAccountConfigs.ToDataSourceResult(request, GLAccountConfig => new
            {
                GLAccountConfigId = GLAccountConfig.GLAccountConfigId,
                Category = GLAccountConfig.Category,
                AccountCode = GLAccountConfig.AccountCode,
                Description = GLAccountConfig.Description,
                OrgBranchName = GLAccountConfig.OrgBranchName,
                DateCreated = GLAccountConfig.DateCreated,
                LastModified = GLAccountConfig.LastModified,
                CreatedBy = GLAccountConfig.CreatedBy,
                ModifiedBy = GLAccountConfig.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLAccountConfigs_Create([DataSourceRequest]DataSourceRequest request, GLAccountConfig GLAccountConfig)
        {
            var exists = _db.GLAccountConfigs.Where(x => x.GLAccountConfigId != GLAccountConfig.GLAccountConfigId).Where(x => x.Category==GLAccountConfig.Category).ToList();

            if (exists.Any())
                ModelState.AddModelError("", GLAccountConfig.Category + " already exists in the system.");

            if (ModelState.IsValid)
            {
               
                    var entity = new GLAccountConfig
                    {
                        Category = GLAccountConfig.Category,
                        AccountCode = GLAccountConfig.AccountCode,
                        Description= GLAccountConfig.Description,
                        DateCreated = DateTime.Now,
                        LastModified = GLAccountConfig.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = GLAccountConfig.ModifiedBy
                    };

                    _db.GLAccountConfigs.Add(entity);
                    _db.SaveChanges();
                    GLAccountConfig.GLAccountConfigId = entity.GLAccountConfigId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL account config", "Success", User.Identity.Name, $"created config {entity.AccountCode} {entity.Category}", Modules.Finance);
                    #endregion
                
            }

            return Json(new[] { GLAccountConfig }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLAccountConfigs_Update([DataSourceRequest]DataSourceRequest request, GLAccountConfig GLAccountConfig)
        {
            var exists = _db.GLAccountConfigs.Where(x => x.GLAccountConfigId != GLAccountConfig.GLAccountConfigId).Where(x => x.Category==GLAccountConfig.Category).ToList();

            if (exists.Any())
                ModelState.AddModelError("", GLAccountConfig.Category + " already exists in the system.");

            if (ModelState.IsValid)
            {
               
                    var entity = new GLAccountConfig
                    {
                        GLAccountConfigId = GLAccountConfig.GLAccountConfigId,
                        Category = GLAccountConfig.Category,
                        AccountCode = GLAccountConfig.AccountCode,
                        Description = GLAccountConfig.Description,
                        OrgBranchName = GLAccountConfig.OrgBranchName,
                        DateCreated = GLAccountConfig.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = GLAccountConfig.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.GLAccountConfigs.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL account config", "Success", User.Identity.Name, $"updated account config {entity.GLAccountConfigId} {entity.AccountCode} {entity.Category}", Modules.Finance);
                    #endregion
                
            }

            return Json(new[] { GLAccountConfig }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLAccountConfigs_Destroy([DataSourceRequest]DataSourceRequest request, GLAccountConfig GLAccountConfig)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLAccountConfig
                {
                    GLAccountConfigId = GLAccountConfig.GLAccountConfigId,
                    Category = GLAccountConfig.Category,
                    AccountCode = GLAccountConfig.AccountCode,
                    Description = GLAccountConfig.Description,                  
                    OrgBranchName = GLAccountConfig.OrgBranchName,
                    DateCreated = GLAccountConfig.DateCreated,
                    LastModified = GLAccountConfig.LastModified,
                    CreatedBy = GLAccountConfig.CreatedBy,
                    ModifiedBy = GLAccountConfig.ModifiedBy
                };

                _db.GLAccountConfigs.Attach(entity);
                _db.GLAccountConfigs.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { GLAccountConfig }.ToDataSourceResult(request, ModelState));
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
