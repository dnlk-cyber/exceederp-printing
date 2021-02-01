using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class LaborRateIndexFactorAttributeController : Controller
    {
        public static int NumOfSundaysPerYear = 52;
        public static int NumOfpublicHolidaysPeryear;
        public static int NumOfWorkingDaysPerYear;
        public void SetWorkingDays()
        {
            NumOfWorkingDaysPerYear = 365 - (this.GetPublicHolidays() + NumOfSundaysPerYear);
        }
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/LaborRateIndexFactorAttribute
        public ActionResult Index()
        {
            var attributteNames = db.LaborRateIndexFactorAttributeNames.Select(a => new { a.Name, a.LaborRateIndexFactorAttributeNameId}).ToList();
            ViewData["AttributeNames"] = attributteNames;

            return View();
        }

        public ActionResult Attributes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LaborRateIndexFactorAttribute> attributes = db.LaborRateIndexFactorAttributes;
            DataSourceResult result = attributes.ToDataSourceResult(request, attribute => new
            {
                LaborRateIndexFactorAttributeId = attribute.LaborRateIndexFactorAttributeId,
                LaborRateIndexFactorAttributeNameId = attribute.LaborRateIndexFactorAttributeNameId,
                CalculationDescription = attribute.CalculationDescription,
                NumberOfDays = attribute.NumberOfDays,
                Index = attribute.Index,
                ApprovedIndex = attribute.ApprovedIndex
            });

            return Json(result);
        }

        private bool CheckExistance(int nameId)
        {
            var attribute = db.LaborRateIndexFactorAttributes.Where(x => x.LaborRateIndexFactorAttributeNameId == nameId).FirstOrDefault();
            if (attribute != null)
            {
                return true;
            }
            return false;
        }
        private decimal CalculateIndexValue(LaborRateIndexFactorAttribute model) {
            decimal result = new decimal();
            var yearlyWorkingDays = ConsSettingHelper.GetNumberOfWorkingDaysPerYear();
            if(yearlyWorkingDays > 0 && model.NumberOfDays != null)
            {
                result = (decimal)model.NumberOfDays / yearlyWorkingDays;
            }
            return result;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attribute_Create([DataSourceRequest]DataSourceRequest request, LaborRateIndexFactorAttribute attribute)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(attribute.LaborRateIndexFactorAttributeNameId))
                {
                    ModelState.AddModelError("", "attribute is already registered!!");
                }
                else
                {
                    var entity = new LaborRateIndexFactorAttribute
                    {
                        LaborRateIndexFactorAttributeId = attribute.LaborRateIndexFactorAttributeId,
                        LaborRateIndexFactorAttributeNameId = attribute.LaborRateIndexFactorAttributeNameId,
                        CalculationDescription = attribute.CalculationDescription,
                        NumberOfDays = attribute.NumberOfDays,
                        Index = Math.Round(this.CalculateIndexValue(attribute), 4),
                        ApprovedIndex = attribute.ApprovedIndex

                    };
                    if (entity.Index <= 0) { entity.Index = null; }
                    entity.ApprovedIndex = entity.Index;
                    attribute.Index = entity.Index;
                    attribute.ApprovedIndex = entity.Index;

                    db.LaborRateIndexFactorAttributes.Add(entity);
                    db.SaveChanges();
                    attribute.LaborRateIndexFactorAttributeId = entity.LaborRateIndexFactorAttributeId;

                }
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attribute_Update([DataSourceRequest]DataSourceRequest request, LaborRateIndexFactorAttribute attribute)
        {
            if (ModelState.IsValid)
            {
                var entity = new LaborRateIndexFactorAttribute
                {
                    LaborRateIndexFactorAttributeId = attribute.LaborRateIndexFactorAttributeId,
                    LaborRateIndexFactorAttributeNameId = attribute.LaborRateIndexFactorAttributeNameId,
                    CalculationDescription = attribute.CalculationDescription,
                    NumberOfDays = attribute.NumberOfDays,
                    Index = Math.Round(this.CalculateIndexValue(attribute), 4),
                    ApprovedIndex = attribute.ApprovedIndex

                };
                if (entity.Index <= 0) { entity.Index = null; }
                attribute.Index = entity.Index;
                if(entity.ApprovedIndex == null) { entity.ApprovedIndex = entity.Index; }
                attribute.ApprovedIndex = entity.ApprovedIndex;

                db.LaborRateIndexFactorAttributes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attribute_Destroy([DataSourceRequest]DataSourceRequest request, LaborRateIndexFactorAttribute attribute)
        {
            if (ModelState.IsValid)
            {
                var entity = new LaborRateIndexFactorAttribute
                {
                    LaborRateIndexFactorAttributeId = attribute.LaborRateIndexFactorAttributeId,
                    LaborRateIndexFactorAttributeNameId = attribute.LaborRateIndexFactorAttributeNameId,
                    CalculationDescription = attribute.CalculationDescription,
                    NumberOfDays = attribute.NumberOfDays,
                    Index = attribute.Index,
                    ApprovedIndex = attribute.ApprovedIndex
                };

                db.LaborRateIndexFactorAttributes.Attach(entity);
                db.LaborRateIndexFactorAttributes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public int GetPublicHolidays()
        {
            int result = 0;
            var values = db.LaborRateIndexFactorAttributes.ToList();
            var targetValue = new LaborRateIndexFactorAttribute();
            var targetName = new LaborRateIndexFactorAttributeName();
            foreach (var item in values) {
                var currenName = db.LaborRateIndexFactorAttributeNames.Where(n => n.Name == "Public Holidays").FirstOrDefault();
                if(currenName != null)
                {
                    targetName = currenName;
                }
            }
            if(targetName != null)
            {
                targetValue = db.LaborRateIndexFactorAttributes.Where(v => v.LaborRateIndexFactorAttributeNameId == targetName.LaborRateIndexFactorAttributeNameId).FirstOrDefault();
            }
            if(targetValue != null)
            {
                result = (int)targetValue.NumberOfDays;
            }
            return result;
        }
       
    }
}