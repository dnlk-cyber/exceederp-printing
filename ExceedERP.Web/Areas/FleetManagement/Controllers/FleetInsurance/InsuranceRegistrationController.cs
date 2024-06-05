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
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    //[AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class InsuranceRegistrationController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["fleet"] = db.Fleets.Select(x => new { Id = x.FleetsID, Name = x.PlateNumber }).ToList();
            ViewData["InsuranceCompany"] = db.InsuranceCompany.Select(x => new { x.InsuranceCompanyId, x.InsuranceCompanyName }).ToList();
            return View();
        }


        private bool IsExpired(DateTime? expireDate)
        {
            DateTime currentDate = DateTime.Now;
            TimeSpan timeDifference = (DateTime)expireDate - currentDate;
            int daysRemaining = timeDifference.Days;
            if(daysRemaining <= 15)
            {
                return true;
            }
            return false;
        }


        public ActionResult FleetInsurances_Read([DataSourceRequest] DataSourceRequest request)
        {
            var fleetInsurances = db.FleetInsurance;

            DataSourceResult result = fleetInsurances.ToDataSourceResult(request, fleetsInsurance => new FleetInsurance
            {
                FleetInsuranceId = fleetsInsurance.FleetInsuranceId,
                FleetsID = fleetsInsurance.FleetsID,
                InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId,
                PremiumCost = fleetsInsurance.PremiumCost,
                DueDate = fleetsInsurance.DueDate,
                ExpireDate = fleetsInsurance.ExpireDate,
                
                HasExpired = IsExpired(fleetsInsurance.DueDate),
                Attachment = fleetsInsurance.Attachment,
                Description = fleetsInsurance.Description,
                PolicyNo = fleetsInsurance.PolicyNo,
                ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
                ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance,
                InsuranceType = fleetsInsurance.InsuranceType,
                CreatedBy = fleetsInsurance.CreatedBy,
                DateCreated = fleetsInsurance.DateCreated,
               
            });

            return Json(result);
        }



        public ActionResult FleetInsurances_Create([DataSourceRequest] DataSourceRequest request, FleetInsurance fleetsInsurance)
        {
            if(fleetsInsurance.DueDate == null || fleetsInsurance.ExpireDate == null)
            {
                ModelState.AddModelError(string.Empty, "Due Date and Expire Date needs to have value");
            }
            var exists = db.FleetInsurance.Where(x => x.FleetsID == fleetsInsurance.FleetsID).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError(string.Empty, "This Item is already registred!");
            }
            if (ModelState.IsValid)
            {
                var entity = new FleetInsurance
                {
                    
                    FleetsID= fleetsInsurance.FleetsID,
                    InsuranceCompanyId= fleetsInsurance.InsuranceCompanyId,
                    PremiumCost= fleetsInsurance.PremiumCost,
                    DueDate= fleetsInsurance.DueDate,
                    ExpireDate= fleetsInsurance.ExpireDate,
                    Attachment= fleetsInsurance.Attachment,
                    Description= fleetsInsurance.Description,
                    PolicyNo = fleetsInsurance.PolicyNo,
                    ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
                    ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance,
                    InsuranceType = fleetsInsurance.InsuranceType,
                };

                db.FleetInsurance.Add(entity);
                db.SaveChanges();
                fleetsInsurance.FleetInsuranceId = entity.FleetInsuranceId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance registration", "Success", User.Identity.Name, $"created insurance registration {entity.FleetInsuranceId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { fleetsInsurance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetInsurances_Update([DataSourceRequest] DataSourceRequest request, FleetInsurance fleetsInsurance)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetInsurance
                {
                    FleetInsuranceId = fleetsInsurance.FleetInsuranceId,
                    FleetsID = fleetsInsurance.FleetsID,
                    InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId,
                    PremiumCost = fleetsInsurance.PremiumCost,
                    DueDate = fleetsInsurance.DueDate,
                    ExpireDate = fleetsInsurance.ExpireDate,
                    Attachment = fleetsInsurance.Attachment,
                    Description = fleetsInsurance.Description,
                    PolicyNo = fleetsInsurance.PolicyNo,
                    ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
                    ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance,
                    InsuranceType = fleetsInsurance.InsuranceType,
                };

                db.FleetInsurance.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance update", "Success", User.Identity.Name, $"updated insurance registration number {entity.FleetInsuranceId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { fleetsInsurance }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult FleetInsurances_Destroy([DataSourceRequest] DataSourceRequest request, FleetInsurance fleetsInsurance)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetInsurance
                {
                    FleetInsuranceId = fleetsInsurance.FleetInsuranceId,
                    FleetsID = fleetsInsurance.FleetsID,
                    InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId,
                    PremiumCost = fleetsInsurance.PremiumCost,
                    DueDate = fleetsInsurance.DueDate,
                    ExpireDate = fleetsInsurance.ExpireDate,
                    Attachment = fleetsInsurance.Attachment,
                    Description = fleetsInsurance.Description,
                    ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
                    ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance,
                    InsuranceType = fleetsInsurance.InsuranceType,

                    PolicyNo = fleetsInsurance.PolicyNo
                };

                db.FleetInsurance.Attach(entity);
                db.FleetInsurance.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance registration number {entity.FleetInsuranceId }");
                #endregion
            }

            return Json(new[] { fleetsInsurance }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}