using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class PerformanceComponentWeightController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult PerformanceComponentWeight_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PerformanceEvaluationComponentWeight> attributes = db.PerformanceEvaluationComponentWeight;
            DataSourceResult result = attributes.ToDataSourceResult(request, attribute => new
            {
                PerformanceEvaluationComponentWeightId = attribute.PerformanceEvaluationComponentWeightId,
                ComponentName = attribute.ComponentName,
                Value = attribute.Value,
                ValueForRoadProjects = attribute.ValueForRoadProjects,

                OrgBranchName = attribute.OrgBranchName,
                DateCreated = attribute.DateCreated,
                LastModified = attribute.LastModified,
                CreatedBy = attribute.CreatedBy,
                ModifiedBy = attribute.ModifiedBy
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerformanceComponentWeight_Create([DataSourceRequest]DataSourceRequest request, PerformanceEvaluationComponentWeight attribute)
        {
            var errorExists = false;
            if (ModelState.IsValid)
            {
                if (CheckExistance(attribute))
                {
                    ModelState.AddModelError("", "Value Exists!!");
                    errorExists = true;
                }
                if (IsInvalidProgressWeightSetting(attribute))
                {
                    ModelState.AddModelError("", "Sum  of the weights of Monthly progress and todate progress shouldn't exceed the weight of total progress!!!");
                    errorExists = true;
                }
                if (TotalWeightHasExceededLimit(attribute))
                {
                    ModelState.AddModelError("", "Sum of weight of the components shoudn't exceed 100%");
                    errorExists = true;
                }
                if (IsInvalidModifiersSetting(attribute))
                {
                    ModelState.AddModelError("", "Sum of weight of sub-components shouldn't exceed weight of Modifier");
                    errorExists = true;
                }
                if (IsInvalidPaymentSetting(attribute))
                {
                    ModelState.AddModelError("", "Sum of weight of sub-components shouldn't exceed weight of Payment");
                    errorExists = true;
                }
                if(!errorExists)
                {
                    var entity = new PerformanceEvaluationComponentWeight
                    {
                        PerformanceEvaluationComponentWeightId = attribute.PerformanceEvaluationComponentWeightId,
                        ComponentName = attribute.ComponentName,
                        Value = attribute.Value,
                        ValueForRoadProjects = attribute.ValueForRoadProjects,

                        OrgBranchName = attribute.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = attribute.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = attribute.ModifiedBy
                    };

                    db.PerformanceEvaluationComponentWeight.Add(entity);
                    db.SaveChanges();
                    attribute.PerformanceEvaluationComponentWeightId = entity.PerformanceEvaluationComponentWeightId;

                }
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerformanceComponentWeight_Update([DataSourceRequest]DataSourceRequest request, PerformanceEvaluationComponentWeight attribute)
        {
            var errorExists = false;
            if (ModelState.IsValid)
            {
                if (IsInvalidProgressWeightSetting(attribute))
                {
                    ModelState.AddModelError("", "Sum  of the weights of Monthly progress and todate progress shouldn't exceed the weight of total progress!!!");
                    errorExists = true;
                }
                if (TotalWeightHasExceededLimit(attribute))
                {
                    ModelState.AddModelError("", "Sum of weight of the components shoudn't exceed 100%");
                    errorExists = true;
                }
                if (IsInvalidModifiersSetting(attribute))
                {
                    ModelState.AddModelError("", "Sum of weight of sub-components shouldn't exceed weight of Modifier");
                    errorExists = true;
                }
                if (IsInvalidPaymentSetting(attribute))
                {
                    ModelState.AddModelError("", "Sum of weight of sub-components shouldn't exceed weight of Payment");
                    errorExists = true;
                }
                if (!errorExists)
                {
                    var entity = new PerformanceEvaluationComponentWeight
                    {
                        PerformanceEvaluationComponentWeightId = attribute.PerformanceEvaluationComponentWeightId,
                        ComponentName = attribute.ComponentName,
                        Value = attribute.Value,
                        ValueForRoadProjects = attribute.ValueForRoadProjects,

                        OrgBranchName = attribute.OrgBranchName,
                        DateCreated = attribute.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = attribute.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    db.PerformanceEvaluationComponentWeight.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerformanceComponentWeight_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceEvaluationComponentWeight attribute)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceEvaluationComponentWeight
                {
                    PerformanceEvaluationComponentWeightId = attribute.PerformanceEvaluationComponentWeightId,
                    ComponentName = attribute.ComponentName,
                    Value = attribute.Value,
                    ValueForRoadProjects = attribute.ValueForRoadProjects,

                    OrgBranchName = attribute.OrgBranchName,
                    DateCreated = attribute.DateCreated,
                    LastModified = attribute.LastModified,
                    CreatedBy = attribute.CreatedBy,
                    ModifiedBy = attribute.ModifiedBy
                };

                db.PerformanceEvaluationComponentWeight.Attach(entity);
                db.PerformanceEvaluationComponentWeight.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region methods
        private bool CheckExistance(PerformanceEvaluationComponentWeight model)
        {
            bool attributeExists = false;
            var attribute = db.PerformanceEvaluationComponentWeight.FirstOrDefault(at => at.ComponentName == model.ComponentName);
            if (attribute != null)
            {
                attributeExists = true;
            }
            return attributeExists;
        }
        private bool IsInvalidProgressWeightSetting(PerformanceEvaluationComponentWeight model)
        {
            bool isInValid = false;
            bool isProgressComponent = model.ComponentName == ProjectPerformanceEvaluationComponents.PROGRESS ||
                model.ComponentName == ProjectPerformanceEvaluationComponents.PROGRESS_MONTHLY ||
                model.ComponentName == ProjectPerformanceEvaluationComponents.PROGRESS_TODATE;

            decimal totalValue = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PROGRESS);
            var monthlyValue = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PROGRESS_TODATE);
            var toDateValue = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PROGRESS_MONTHLY);
            if (isProgressComponent)
            {
                var currentComponent = model.ComponentName;
                if (currentComponent == ProjectPerformanceEvaluationComponents.PROGRESS)
                {
                    totalValue = model.Value;
                }
                else if (currentComponent == ProjectPerformanceEvaluationComponents.PROGRESS_MONTHLY)
                {
                    monthlyValue = model.Value;
                }
                else if (currentComponent == ProjectPerformanceEvaluationComponents.PROGRESS_TODATE)
                {
                    toDateValue = model.Value;
                }

                if (monthlyValue + toDateValue > totalValue)
                {
                    isInValid = true;
                }

            }
            return isInValid;
        }

        private bool TotalWeightHasExceededLimit(PerformanceEvaluationComponentWeight component)
        {
            var sumIsBeyondLimit = false;
            var totalWeight = new decimal();
            var progress = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PROGRESS);
            var quality = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.QUALITY);
            var cost = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.COST);
            var payment = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PAYMENT);
            var planning = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PLANNING);
            var claim = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.CLAIM);
            var report = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.REPORT);
            var comn = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.COMMUNICATION);
            if(component.ComponentName == ProjectPerformanceEvaluationComponents.PROGRESS)
            {
                progress = component.Value;
            }
            else if(component.ComponentName == ProjectPerformanceEvaluationComponents.QUALITY)
            {
                quality = component.Value;
            }
            else if(component.ComponentName == ProjectPerformanceEvaluationComponents.COST)
            {
                cost = component.Value;
            }
            else if(component.ComponentName == ProjectPerformanceEvaluationComponents.PAYMENT)
            {
                payment = component.Value;
            }
            else if(component.ComponentName == ProjectPerformanceEvaluationComponents.PLANNING)
            {
                planning = component.Value;
            }
            else if(component.ComponentName == ProjectPerformanceEvaluationComponents.CLAIM)
            {
                claim = component.Value;
            }
            else if(component.ComponentName == ProjectPerformanceEvaluationComponents.REPORT)
            {
                report = component.Value;
            }
            else if(component.ComponentName == ProjectPerformanceEvaluationComponents.COMMUNICATION)
            {
                comn = component.Value;
            }
            if (IsSubCoponent(component.ComponentName) || component.ComponentName == ProjectPerformanceEvaluationComponents.MODIFIERS)
            {
                totalWeight = progress + quality + cost + payment + planning + claim + report + comn;
            }
            else
            {
                totalWeight =  progress + quality + cost + payment + planning + claim + report + comn;
            }
            if (totalWeight > 100)
            {
                sumIsBeyondLimit = true;
            }
            return sumIsBeyondLimit;
        }



        private bool IsInvalidModifiersSetting(PerformanceEvaluationComponentWeight component)
        {
            var isInvalid = false;
            if (component.ComponentName == ProjectPerformanceEvaluationComponents.MODIFIERS ||
                IsModifierSubCoponent(component.ComponentName))
            {
                var total = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIERS);
                var healthAndSafety =
                    ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents
                        .MODIFIER_HEALTH_AND_SAFETY);
                var budgetUtl =
                    ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents
                        .MODIFIER_BUDGET_UTILIZATION);
                var materialUtilization =
                    ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents
                        .MODIFIER_MATERIAL_UTILIZATION);
                var subContractAdmn = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents
                    .MODIFIER_SUBCONTRACT_ADMINISTRATION);

                if (component.ComponentName == ProjectPerformanceEvaluationComponents.MODIFIERS)
                {
                    total = component.Value;
                }
                else if (component.ComponentName == ProjectPerformanceEvaluationComponents.MODIFIER_HEALTH_AND_SAFETY)
                {
                    healthAndSafety = component.Value;
                }
                else if (component.ComponentName == ProjectPerformanceEvaluationComponents.MODIFIER_BUDGET_UTILIZATION)
                {
                    budgetUtl = component.Value;
                }
                else if (component.ComponentName == ProjectPerformanceEvaluationComponents
                             .MODIFIER_MATERIAL_UTILIZATION)
                {
                    materialUtilization = component.Value;
                }
                else if (component.ComponentName == ProjectPerformanceEvaluationComponents.MODIFIER_SUBCONTRACT_ADMINISTRATION)
                {
                    subContractAdmn = component.Value;
                }

                if (total < healthAndSafety + budgetUtl + materialUtilization + subContractAdmn)
                {
                    isInvalid = true;
                }
            }
            return isInvalid;
        }
        private bool IsInvalidPaymentSetting(PerformanceEvaluationComponentWeight component)
        {
            var isInvalid = false;
            if (component.ComponentName == ProjectPerformanceEvaluationComponents.PAYMENT ||
                IsPaymentSubCoponent(component.ComponentName))
            {
                var total = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PAYMENT);
                var priceAdjustmentWeight =
                    ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents
                        .PAYMENT_PRICE_ADJUSTMENT);
                var accomplishementWeight =
                    ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents
                        .PAYMENT_ACCOMPLISHMENT);
                var approvalTimeWeight =
                    ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents
                        .PAYMENT_APPROVAL_TIME);
                var materialOnSiteWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents
                    .PAYMENT_MATERIAL_ON_SITE);

                if (component.ComponentName == ProjectPerformanceEvaluationComponents.PAYMENT)
                {
                    total = component.Value;
                }
                else if (component.ComponentName == ProjectPerformanceEvaluationComponents.PAYMENT_PRICE_ADJUSTMENT)
                {
                    priceAdjustmentWeight = component.Value;
                }
                else if (component.ComponentName == ProjectPerformanceEvaluationComponents.PAYMENT_ACCOMPLISHMENT)
                {
                    accomplishementWeight = component.Value;
                }
                else if (component.ComponentName == ProjectPerformanceEvaluationComponents
                             .PAYMENT_APPROVAL_TIME)
                {
                    approvalTimeWeight = component.Value;
                }
                else if (component.ComponentName == ProjectPerformanceEvaluationComponents.PAYMENT_MATERIAL_ON_SITE)
                {
                    materialOnSiteWeight = component.Value;
                }

                if (total < priceAdjustmentWeight + accomplishementWeight + approvalTimeWeight + materialOnSiteWeight)
                {
                    isInvalid = true;
                }
            }
            return isInvalid;
        }
        private bool IsSubCoponent(ProjectPerformanceEvaluationComponents compoent)
        {
            return compoent == ProjectPerformanceEvaluationComponents.PROGRESS_TODATE ||
                   compoent == ProjectPerformanceEvaluationComponents.PROGRESS_MONTHLY ||
                   compoent == ProjectPerformanceEvaluationComponents.PAYMENT_ACCOMPLISHMENT ||
                   compoent == ProjectPerformanceEvaluationComponents.PAYMENT_APPROVAL_TIME ||
                   compoent == ProjectPerformanceEvaluationComponents.PAYMENT_MATERIAL_ON_SITE ||
                   compoent == ProjectPerformanceEvaluationComponents.PAYMENT_PRICE_ADJUSTMENT ||
                   compoent == ProjectPerformanceEvaluationComponents.MODIFIER_BUDGET_UTILIZATION ||
                   compoent == ProjectPerformanceEvaluationComponents.MODIFIER_HEALTH_AND_SAFETY ||
                   compoent == ProjectPerformanceEvaluationComponents.MODIFIER_MATERIAL_UTILIZATION ||
                   compoent == ProjectPerformanceEvaluationComponents.MODIFIER_SUBCONTRACT_ADMINISTRATION;
        }
        private bool IsModifierSubCoponent(ProjectPerformanceEvaluationComponents compoent)
        {
            return
                   compoent == ProjectPerformanceEvaluationComponents.MODIFIER_BUDGET_UTILIZATION ||
                   compoent == ProjectPerformanceEvaluationComponents.MODIFIER_HEALTH_AND_SAFETY ||
                   compoent == ProjectPerformanceEvaluationComponents.MODIFIER_MATERIAL_UTILIZATION ||
                   compoent == ProjectPerformanceEvaluationComponents.MODIFIER_SUBCONTRACT_ADMINISTRATION;
        }
        private bool IsPaymentSubCoponent(ProjectPerformanceEvaluationComponents compoent)
        {
            return
                compoent == ProjectPerformanceEvaluationComponents.PAYMENT_ACCOMPLISHMENT ||
                compoent == ProjectPerformanceEvaluationComponents.PAYMENT_APPROVAL_TIME ||
                compoent == ProjectPerformanceEvaluationComponents.PAYMENT_MATERIAL_ON_SITE ||
                compoent == ProjectPerformanceEvaluationComponents.PAYMENT_PRICE_ADJUSTMENT;
        }
        #endregion
    }
}