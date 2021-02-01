using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = HR_Admin + "," + HR_Benefit_User + "," + HR_Benefit_Admin)]
    public class OtherBenefitController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Benefit_Admin = "HR_Benefit_Admin";
        private const string HR_Benefit_User = "HR_Benefit_User";
        private const string HR_Admin = "HR_Admin";
        public static List<MainCompanyStructure> AllJobCodes { get; set; }
       
        public ActionResult Index()
        {
            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;
           
            GetOrganizations();
            GetLookups();
            return View();
        }
        public void GetLookups()
        {
            var benefits = db.Lookups.Where(l => l.LookupType == LookUpTypes.OTHER_BENEFIT).ToList();
            ViewData["Benefits"] = benefits;

            var uoms = db.Lookups.Where(l => l.LookupType == LookUpTypes.UOM).ToList();
            ViewData["UOM"] = uoms;
        }
   
    

      
        public JsonResult GetOrganizations()
        {

            var organizationUnits = db.Lookups.Where( x=> x.LookupType == LookUpTypes.JOB_TITLE).ToList();

            ViewData["jobTitle"] = organizationUnits;
            return Json(organizationUnits.Select(s => new { LookupId = s.LookupId, Description = s.Description }), JsonRequestBehavior.AllowGet);
        }
         [Authorize(Roles = HR_Admin + "," + HR_Benefit_User + "," + HR_Benefit_Admin)]
        public ActionResult OtherBenefits_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OtherBenefit> otherbenefits = db.OtherBenefits;
            DataSourceResult result = otherbenefits.ToDataSourceResult(request, otherBenefit => new {
                OtherBenefitId = otherBenefit.OtherBenefitId,
                LookupId = otherBenefit.LookupId,
                BenefitName = otherBenefit.BenefitName,
                UnitofMeasurement = otherBenefit.UnitofMeasurement,
                Quantity = otherBenefit.Quantity,
                TimeInterval = otherBenefit.TimeInterval,
                Remark = otherBenefit.Remark,
                DateCreated = otherBenefit.DateCreated,
                LastModified = otherBenefit.LastModified,
                CreatedBy = otherBenefit.CreatedBy,
                ModifiedBy = otherBenefit.ModifiedBy
            });

            return Json(result);
        }
         [Authorize(Roles = HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OtherBenefits_Create([DataSourceRequest]DataSourceRequest request, OtherBenefit otherBenefit)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new OtherBenefit
                {
                    LookupId = otherBenefit.LookupId,
                    BenefitName = otherBenefit.BenefitName,
                    UnitofMeasurement = otherBenefit.UnitofMeasurement,
                    Quantity = otherBenefit.Quantity,
                    TimeInterval = otherBenefit.TimeInterval,

                    Remark = otherBenefit.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = otherBenefit.LastModified,
                    CreatedBy = User .Identity .Name ,
                    ModifiedBy = otherBenefit.ModifiedBy

                };

                db.OtherBenefits.Add(entity);
                db.SaveChanges();
                otherBenefit.OtherBenefitId = entity.OtherBenefitId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll other benefit", "Success", User.Identity.Name, $"added other benefit  {entity.BenefitName} unit {entity.UnitofMeasurement} quantity {entity.Quantity} interval {entity.TimeInterval} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { otherBenefit }.ToDataSourceResult(request, ModelState));
        }
         [Authorize(Roles = HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OtherBenefits_Update([DataSourceRequest]DataSourceRequest request, OtherBenefit otherBenefit)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new OtherBenefit
                {
                    OtherBenefitId = otherBenefit.OtherBenefitId,
                    LookupId = otherBenefit.LookupId,
                    BenefitName = otherBenefit.BenefitName,
                    UnitofMeasurement = otherBenefit.UnitofMeasurement,
                    Quantity = otherBenefit.Quantity,
                    TimeInterval = otherBenefit.TimeInterval,
                    Remark = otherBenefit.Remark,
                    DateCreated = otherBenefit.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = otherBenefit.CreatedBy,
                    ModifiedBy = User .Identity .Name 
                };

                db.OtherBenefits.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll other benefit", "Success", User.Identity.Name, $"updated other benefit  {entity.BenefitName} unit {entity.UnitofMeasurement} quantity {entity.Quantity} interval {entity.TimeInterval} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { otherBenefit }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Admin + "," + HR_Benefit_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OtherBenefits_Destroy([DataSourceRequest]DataSourceRequest request, OtherBenefit otherBenefit)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new OtherBenefit
                {
                    OtherBenefitId = otherBenefit.OtherBenefitId,
                    LookupId = otherBenefit.LookupId,
                    BenefitName = otherBenefit.BenefitName,
                    UnitofMeasurement = otherBenefit.UnitofMeasurement,
                    Quantity = otherBenefit.Quantity,
                    TimeInterval = otherBenefit.TimeInterval,
                    Remark = otherBenefit.Remark,
                    DateCreated = otherBenefit.DateCreated,
                    LastModified = otherBenefit.LastModified,
                    CreatedBy = otherBenefit.CreatedBy,
                    ModifiedBy = otherBenefit.ModifiedBy
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted other benefit information of employee " + otherBenefit.LookupId,Modules.Payroll);
                #endregion
                db.OtherBenefits.Attach(entity);
                db.OtherBenefits.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { otherBenefit }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
