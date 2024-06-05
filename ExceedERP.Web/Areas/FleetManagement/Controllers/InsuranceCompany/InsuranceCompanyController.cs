using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    //[AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class InsuranceCompanyController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Index()
        {
            
            //ViewData["InsuranceCompany"] = db.InsuranceCompany.Select(x => new { x.InsuranceCompanyId, x.InsuranceCompanyName }).ToList();


            return View();
        }



        public ActionResult InsuranceCompanies_Read([DataSourceRequest] DataSourceRequest request)
        {
            var insuranceCompanies = db.InsuranceCompany;

            DataSourceResult result = insuranceCompanies.ToDataSourceResult(request, insuranceCompany => new
            {
                InsuranceCompanyId= insuranceCompany.InsuranceCompanyId,
                InsuranceCompanyName= insuranceCompany.InsuranceCompanyName,
                Address= insuranceCompany.Address,
                Phone= insuranceCompany.Phone,
                Email= insuranceCompany.Email,
            });

            return Json(result);
        }



        public ActionResult InsuranceCompanies_Create([DataSourceRequest] DataSourceRequest request, InsuranceCompany insuranceCompany)
        {
            var exists = db.InsuranceCompany.Where(x => x.InsuranceCompanyName.Trim().ToLower() == insuranceCompany.InsuranceCompanyName.Trim().ToLower()).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Company name is already registered");
            }
            if (ModelState.IsValid)
            {
                var entity = new InsuranceCompany
                {
                    InsuranceCompanyId = insuranceCompany.InsuranceCompanyId,
                    InsuranceCompanyName = insuranceCompany.InsuranceCompanyName,
                    Address = insuranceCompany.Address,
                    Phone = insuranceCompany.Phone,
                    Email = insuranceCompany.Email
                };

                db.InsuranceCompany.Add(entity);
                db.SaveChanges();
                insuranceCompany.InsuranceCompanyId = entity.InsuranceCompanyId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Insurance company registration", "Success", User.Identity.Name, $"created insurance company {entity.InsuranceCompanyId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { insuranceCompany }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceCompanies_Update([DataSourceRequest] DataSourceRequest request, InsuranceCompany InsuranceCompany)
        {
          if (ModelState.IsValid)
            {
                var entity = new InsuranceCompany
                {
                    InsuranceCompanyId = InsuranceCompany.InsuranceCompanyId,
                    InsuranceCompanyName = InsuranceCompany.InsuranceCompanyName,
                    Address = InsuranceCompany.Address,
                    Phone = InsuranceCompany.Phone,
                    Email = InsuranceCompany.Email


                };

                db.InsuranceCompany.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Company registration", "Success", User.Identity.Name, $"updated insurance company {entity.InsuranceCompanyId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { InsuranceCompany }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult InsuranceCompanies_Destroy([DataSourceRequest] DataSourceRequest request, InsuranceCompany InsuranceCompany)
        {
            if (ModelState.IsValid)
            {
                var entity = new InsuranceCompany
                {
                    InsuranceCompanyId = InsuranceCompany.InsuranceCompanyId,
                    InsuranceCompanyName = InsuranceCompany.InsuranceCompanyName,
                    Address = InsuranceCompany.Address,
                    Phone = InsuranceCompany.Phone,
                    Email = InsuranceCompany.Email


                };

                db.InsuranceCompany.Attach(entity);
                db.InsuranceCompany.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance company {entity.InsuranceCompanyId}");
                #endregion
            }

            return Json(new[] { InsuranceCompany }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}