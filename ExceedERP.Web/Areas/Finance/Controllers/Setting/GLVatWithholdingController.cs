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

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class GlVatWithholdingController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GLVatWithholdings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLVatWithholding> glvatwithholdings = _db.GLVatWithholdings;
            DataSourceResult result = glvatwithholdings.ToDataSourceResult(request, gLVatWithholding => new {
                GLVatWithholdingID = gLVatWithholding.GLVatWithholdingID,
                WithholdingAccountCode = gLVatWithholding.WithholdingAccountCode,
                WithholdPercent = gLVatWithholding.WithholdPercent,
                Branch = gLVatWithholding.Branch,
                DateCreated = gLVatWithholding.DateCreated,
                LastModified = gLVatWithholding.LastModified,
                CreatedBy = gLVatWithholding.CreatedBy,
                ModifiedBy = gLVatWithholding.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLVatWithholdings_Create([DataSourceRequest]DataSourceRequest request, GLVatWithholding gLVatWithholding)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.GLVatWithholdings.Where(x => x.GLVatWithholdingID != gLVatWithholding.GLVatWithholdingID).Where(x => x.WithholdingAccountCode.ToUpper().Equals(gLVatWithholding.WithholdingAccountCode.ToUpper())).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", gLVatWithholding.WithholdingAccountCode + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new GLVatWithholding
                    {
                        WithholdingAccountCode = gLVatWithholding.WithholdingAccountCode,
                        Branch = gLVatWithholding.Branch,
                        WithholdPercent = gLVatWithholding.WithholdPercent,
                        DateCreated = DateTime.Now,
                        LastModified = gLVatWithholding.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = gLVatWithholding.ModifiedBy
                    };

                    _db.GLVatWithholdings.Add(entity);
                    _db.SaveChanges();
                    gLVatWithholding.GLVatWithholdingID = entity.GLVatWithholdingID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Finance VAT withholding", "Success", User.Identity.Name, $"created withholding {entity.GLVatWithholdingID}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { gLVatWithholding }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLVatWithholdings_Update([DataSourceRequest]DataSourceRequest request, GLVatWithholding gLVatWithholding)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.GLVatWithholdings.Where(x => x.GLVatWithholdingID != gLVatWithholding.GLVatWithholdingID).Where(x => x.WithholdingAccountCode.ToUpper().Equals(gLVatWithholding.WithholdingAccountCode.ToUpper())).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", gLVatWithholding.WithholdingAccountCode + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new GLVatWithholding
                    {
                        GLVatWithholdingID = gLVatWithholding.GLVatWithholdingID,
                        WithholdingAccountCode = gLVatWithholding.WithholdingAccountCode,
                        WithholdPercent = gLVatWithholding.WithholdPercent,
                        Branch = gLVatWithholding.Branch,
                        DateCreated = gLVatWithholding.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = gLVatWithholding.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.GLVatWithholdings.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Finance VAT withholding", "Success", User.Identity.Name, $"updated withholding {entity.GLVatWithholdingID}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { gLVatWithholding }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLVatWithholdings_Destroy([DataSourceRequest]DataSourceRequest request, GLVatWithholding gLVatWithholding)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLVatWithholding
                {
                    GLVatWithholdingID = gLVatWithholding.GLVatWithholdingID,
                    WithholdingAccountCode = gLVatWithholding.WithholdingAccountCode,
                    WithholdPercent = gLVatWithholding.WithholdPercent,
                    Branch = gLVatWithholding.Branch,
                    DateCreated = gLVatWithholding.DateCreated,
                    LastModified = gLVatWithholding.LastModified,
                    CreatedBy = gLVatWithholding.CreatedBy,
                    ModifiedBy = gLVatWithholding.ModifiedBy
                };

                _db.GLVatWithholdings.Attach(entity);
                _db.GLVatWithholdings.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLVatWithholding }.ToDataSourceResult(request, ModelState));
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
