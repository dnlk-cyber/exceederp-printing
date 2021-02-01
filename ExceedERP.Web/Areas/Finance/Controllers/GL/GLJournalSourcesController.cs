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
    public class GlJournalSourcesController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GLJournalSources_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLJournalSources> gljournalsources = _db.GLJournalSources;
            DataSourceResult result = gljournalsources.ToDataSourceResult(request, gLJournalSources => new {
                GLJournalSourcesID = gLJournalSources.GLJournalSourcesID,
                Sources = gLJournalSources.Sources,
                Description = gLJournalSources.Description,
                JournalRefernces = gLJournalSources.JournalRefernces,
                JournalApproval = gLJournalSources.JournalApproval,
                FreezJournal = gLJournalSources.FreezJournal,
                OrgBranchName = gLJournalSources.OrgBranchName,
                DateCreated = gLJournalSources.DateCreated,
                LastModified = gLJournalSources.LastModified,
                CreatedBy = gLJournalSources.CreatedBy,
                ModifiedBy = gLJournalSources.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLJournalSources_Create([DataSourceRequest]DataSourceRequest request, GLJournalSources gLJournalSources)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLJournalSources
                {
                    Sources = gLJournalSources.Sources,
                    Description = gLJournalSources.Description,
                    JournalRefernces = gLJournalSources.JournalRefernces,
                    JournalApproval = gLJournalSources.JournalApproval,
                    FreezJournal = gLJournalSources.FreezJournal,
                    OrgBranchName = gLJournalSources.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = gLJournalSources.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gLJournalSources.ModifiedBy
                };

                _db.GLJournalSources.Add(entity);
                _db.SaveChanges();
                gLJournalSources.GLJournalSourcesID = entity.GLJournalSourcesID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL journal source", "Success", User.Identity.Name, $"created source {entity.Sources}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLJournalSources }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLJournalSources_Update([DataSourceRequest]DataSourceRequest request, GLJournalSources gLJournalSources)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLJournalSources
                {
                    GLJournalSourcesID = gLJournalSources.GLJournalSourcesID,
                    Sources = gLJournalSources.Sources,
                    Description = gLJournalSources.Description,
                    JournalRefernces = gLJournalSources.JournalRefernces,
                    JournalApproval = gLJournalSources.JournalApproval,
                    FreezJournal = gLJournalSources.FreezJournal,
                    OrgBranchName = gLJournalSources.OrgBranchName,
                    DateCreated = gLJournalSources.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = gLJournalSources.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.GLJournalSources.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL journal source", "Success", User.Identity.Name, $"updated source {entity.Sources}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLJournalSources }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLJournalSources_Destroy([DataSourceRequest]DataSourceRequest request, GLJournalSources gLJournalSources)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLJournalSources
                {
                    GLJournalSourcesID = gLJournalSources.GLJournalSourcesID,
                    Sources = gLJournalSources.Sources,
                    Description = gLJournalSources.Description,
                    JournalRefernces = gLJournalSources.JournalRefernces,
                    JournalApproval = gLJournalSources.JournalApproval,
                    FreezJournal = gLJournalSources.FreezJournal,
                    OrgBranchName = gLJournalSources.OrgBranchName,
                    DateCreated = gLJournalSources.DateCreated,
                    LastModified = gLJournalSources.LastModified,
                    CreatedBy = gLJournalSources.CreatedBy,
                    ModifiedBy = gLJournalSources.ModifiedBy
                };

                _db.GLJournalSources.Attach(entity);
                _db.GLJournalSources.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLJournalSources }.ToDataSourceResult(request, ModelState));
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
