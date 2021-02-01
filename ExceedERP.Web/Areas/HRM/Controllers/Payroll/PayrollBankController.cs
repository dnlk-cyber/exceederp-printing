using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    public class PayrollBankController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
           
            var BusinessTypes = Enum.GetValues(typeof(BusinessType ))
                          .Cast<BusinessType>()
                         
                          .ToList();
            var businessList = BusinessTypes.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["BusinessTypes"] = businessList;
            return View();
        }

        public ActionResult Companies_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MainCompany> companies = db.Companies.Where(c => c.Category == "0" && c.IsActive);
            DataSourceResult result = companies.ToDataSourceResult(request, mainCompany => new {
                MainCompanyId = mainCompany.MainCompanyId,
                TradeName = mainCompany.TradeName,
                BrandName = mainCompany.BrandName,
                BusinessType = mainCompany.BusinessType,
                Category = mainCompany.Category,
                IsActive = mainCompany.IsActive,
                DateCreated = mainCompany.DateCreated,
                LastModified = mainCompany.LastModified,
                CreatedBy = mainCompany.CreatedBy,
                ModifiedBy = mainCompany.ModifiedBy
            });

            return Json(result);
        }



       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
