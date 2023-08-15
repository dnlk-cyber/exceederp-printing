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
    public class GlTaxRateController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
           
            return View();
        }

        public ActionResult GLTaxRates_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLTaxRate> gltaxrates = _db.GLTaxRates;
            DataSourceResult result = gltaxrates.ToDataSourceResult(request, gLTaxRate => new
            {
                GLTaxRateID = gLTaxRate.GLTaxRateID,
                TaxName = gLTaxRate.TaxName,
                Rate = gLTaxRate.Rate,
                CalculatedRate = gLTaxRate.CalculatedRate,
                ApplyToAr = gLTaxRate.ApplyToAr,
                ApplyToAp = gLTaxRate.ApplyToAp,
                ApplyToWithholding = gLTaxRate.ApplyToWithholding,
                ArAccountCode = gLTaxRate.ArAccountCode,
                ApAccountCode = gLTaxRate.ApAccountCode,
                WithholdingAccountCode = gLTaxRate.WithholdingAccountCode,
                Enable = gLTaxRate.Enable,
                OrgBranchName = gLTaxRate.OrgBranchName,
                DateCreated = gLTaxRate.DateCreated,
                LastModified = gLTaxRate.LastModified,
                CreatedBy = gLTaxRate.CreatedBy,
                ModifiedBy = gLTaxRate.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLTaxRates_Create([DataSourceRequest]DataSourceRequest request, GLTaxRate gLTaxRate)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.GLTaxRates.Where(x => x.GLTaxRateID != gLTaxRate.GLTaxRateID).Where(x => x.TaxName.ToUpper().Equals(gLTaxRate.TaxName.ToUpper())).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", gLTaxRate.TaxName + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new GLTaxRate
                    {
                        TaxName = gLTaxRate.TaxName,
                        Rate = gLTaxRate.Rate,
                        CalculatedRate= gLTaxRate.Rate/100,
                        ApplyToAr = gLTaxRate.ApplyToAr,
                        ApplyToAp = gLTaxRate.ApplyToAp,
                        ApplyToWithholding = gLTaxRate.ApplyToWithholding,
                        ArAccountCode = gLTaxRate.ArAccountCode,
                        ApAccountCode = gLTaxRate.ApAccountCode,
                        WithholdingAccountCode = gLTaxRate.WithholdingAccountCode,
                        Enable = gLTaxRate.Enable,
                        OrgBranchName = gLTaxRate.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = gLTaxRate.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = gLTaxRate.ModifiedBy
                    };

                    _db.GLTaxRates.Add(entity);
                    _db.SaveChanges();
                    gLTaxRate.GLTaxRateID = entity.GLTaxRateID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL tax type setup", "Success", User.Identity.Name, $"created tax type setup {entity.GLTaxRateID}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { gLTaxRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLTaxRates_Update([DataSourceRequest]DataSourceRequest request, GLTaxRate gLTaxRate)
        {
            if (ModelState.IsValid)
            {
                  #region validation

                bool valid = true;
                var exists = _db.GLTaxRates.Where(x => x.GLTaxRateID != gLTaxRate.GLTaxRateID).Where(x => x.TaxName.ToUpper().Equals(gLTaxRate.TaxName.ToUpper())).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", gLTaxRate.TaxName + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new GLTaxRate
                    {
                        GLTaxRateID = gLTaxRate.GLTaxRateID,
                        TaxName = gLTaxRate.TaxName,
                        Rate = gLTaxRate.Rate,
                        CalculatedRate = gLTaxRate.Rate / 100,
                        ApplyToAr = gLTaxRate.ApplyToAr,
                        ApplyToAp = gLTaxRate.ApplyToAp,
                        ApplyToWithholding = gLTaxRate.ApplyToWithholding,
                        ArAccountCode = gLTaxRate.ArAccountCode,
                        ApAccountCode = gLTaxRate.ApAccountCode,
                        WithholdingAccountCode = gLTaxRate.WithholdingAccountCode,
                        Enable = gLTaxRate.Enable,
                        OrgBranchName = gLTaxRate.OrgBranchName,
                        DateCreated = gLTaxRate.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = gLTaxRate.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.GLTaxRates.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL tax type setup", "Success", User.Identity.Name, $"updated tax type setup {entity.GLTaxRateID}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { gLTaxRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLTaxRates_Destroy([DataSourceRequest]DataSourceRequest request, GLTaxRate gLTaxRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLTaxRate
                {
                    GLTaxRateID = gLTaxRate.GLTaxRateID,
                    TaxName = gLTaxRate.TaxName,
                    Rate = gLTaxRate.Rate,
                    CalculatedRate = gLTaxRate.CalculatedRate,
                    ApplyToAr = gLTaxRate.ApplyToAr,
                    ApplyToAp = gLTaxRate.ApplyToAp,
                    ApplyToWithholding = gLTaxRate.ApplyToWithholding,
                    ArAccountCode = gLTaxRate.ArAccountCode,
                    ApAccountCode = gLTaxRate.ApAccountCode,
                    WithholdingAccountCode = gLTaxRate.WithholdingAccountCode,
                    Enable = gLTaxRate.Enable,
                    OrgBranchName = gLTaxRate.OrgBranchName,
                    DateCreated = gLTaxRate.DateCreated,
                    LastModified = gLTaxRate.LastModified,
                    CreatedBy = gLTaxRate.CreatedBy,
                    ModifiedBy = gLTaxRate.ModifiedBy
                };

                _db.GLTaxRates.Attach(entity);
                _db.GLTaxRates.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLTaxRate }.ToDataSourceResult(request, ModelState));
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
