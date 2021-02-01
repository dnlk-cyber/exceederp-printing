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
    public class GlJournalCategoriersController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
       

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult GLJournalCategoriers_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLJournalCategoriers> gljournalcategoriers = _db.GLJournalCategoriers;
            DataSourceResult result = gljournalcategoriers.ToDataSourceResult(request, gLJournalCategoriers => new {
                GLJournalCategoriersID = gLJournalCategoriers.GLJournalCategoriersID,
                Category = gLJournalCategoriers.Category,
                Description = gLJournalCategoriers.Description,
                Enable = gLJournalCategoriers.Enable,
                DateCreated = gLJournalCategoriers.DateCreated,
                LastModified = gLJournalCategoriers.LastModified,
                CreatedBy = gLJournalCategoriers.CreatedBy,
                ModifiedBy = gLJournalCategoriers.ModifiedBy
            });

            return Json(result);
        }

         [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLJournalCategoriers_Create([DataSourceRequest]DataSourceRequest request, GLJournalCategoriers gLJournalCategoriers)
        {
            #region validation

            var exists = _db.GLJournalCategoriers.Where(x => x.Category == gLJournalCategoriers.Category).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The category already exists.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GLJournalCategoriers
                {
                    Category = gLJournalCategoriers.Category,
                    Description = gLJournalCategoriers.Description,
                    Enable = gLJournalCategoriers.Enable,
                    DateCreated = DateTime.Now,
                    LastModified = gLJournalCategoriers.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gLJournalCategoriers.ModifiedBy
                };

                _db.GLJournalCategoriers.Add(entity);
                _db.SaveChanges();
                gLJournalCategoriers.GLJournalCategoriersID = entity.GLJournalCategoriersID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL journal category", "Success", User.Identity.Name, $"created category {entity.Category}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLJournalCategoriers }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLJournalCategoriers_Update([DataSourceRequest]DataSourceRequest request, GLJournalCategoriers gLJournalCategoriers)
        {
            #region validation
            var exists = _db.GLJournalCategoriers.Where(x => x.GLJournalCategoriersID != gLJournalCategoriers.GLJournalCategoriersID)
                                                 .Where(x => x.Category == gLJournalCategoriers.Category).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The category already exists.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GLJournalCategoriers
                {
                    GLJournalCategoriersID = gLJournalCategoriers.GLJournalCategoriersID,
                    Category = gLJournalCategoriers.Category,
                    Description = gLJournalCategoriers.Description,
                    Enable = gLJournalCategoriers.Enable,
                    DateCreated = gLJournalCategoriers.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = gLJournalCategoriers.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.GLJournalCategoriers.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL journal category", "Success", User.Identity.Name, $"updated category {entity.Category}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLJournalCategoriers }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLJournalCategoriers_Destroy([DataSourceRequest]DataSourceRequest request, GLJournalCategoriers gLJournalCategoriers)
        {
            //if (ModelState.IsValid)
            //{
            //    var entity = new GLJournalCategoriers
            //    {
            //        GLJournalCategoriersID = gLJournalCategoriers.GLJournalCategoriersID,
            //        Category = gLJournalCategoriers.Category,
            //        Description = gLJournalCategoriers.Description,
            //        Enable = gLJournalCategoriers.Enable,
            //        DateCreated = gLJournalCategoriers.DateCreated,
            //        LastModified = gLJournalCategoriers.LastModified,
            //        CreatedBy = gLJournalCategoriers.CreatedBy,
            //        ModifiedBy = gLJournalCategoriers.ModifiedBy
            //    };

            //    _db.GLJournalCategoriers.Attach(entity);
            //    _db.GLJournalCategoriers.Remove(entity);
            //    _db.SaveChanges();
            //}

            return Json(new[] { gLJournalCategoriers }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var categoriers = _db.GLJournalCategoriers.Find(id);
                if (categoriers != null && categoriers.Enable)
                {
                    categoriers.Enable = false;
                }
                else
                {
                    if (categoriers != null) categoriers.Enable = true;
                }
                if (categoriers != null)
                {
                    _db.GLJournalCategoriers.Attach(categoriers);
                    _db.Entry(categoriers).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
