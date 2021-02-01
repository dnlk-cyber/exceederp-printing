using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Linq;

namespace ExceedERP.Helpers.Construction
{
    public static class ConsSettingHelper
    {

        public static string GetUnitOfMearurnment(int id)
        {
            var db = new ExceedDbContext();
            var unit = new Core.Domain.Construction.UnitOfMeasurement();
            var temp = db.UnitOfMeasurement.Find(id);
            if (temp != null)
            {
                unit = temp;
            }
            return unit.Abbreviation;
        }
        public static decimal GetLabourRateIndexFactor()
        {
            var db = new ExceedDbContext();
            var indexFactor = new decimal();
            var setting =
                db.Configurations
                    .FirstOrDefault(x => x.Category == "Construction" && x.Reference == "INDEX_FaCTOR");
            if (setting != null)
            {
                indexFactor = Convert.ToDecimal(setting.CurrentValue);
            }
            return indexFactor;
        }
        public static decimal GetCurrentProfitRate()
        {
            var db = new ExceedDbContext();
            var profitRate = new decimal();
            var setting =
                db.Configurations
                    .FirstOrDefault(x => x.Category == "Construction" && x.Reference == "PROFIT_COST_RATE");
            if (setting != null)
            {
                profitRate = Convert.ToDecimal(setting.CurrentValue);
            }
            return profitRate;
        }
        public static decimal GetCurrentOverHeadRate()
        {
            var db = new ExceedDbContext();
            var overHeadRate = new decimal();
            var setting =
                db.Configurations
                    .FirstOrDefault(x => x.Category == "Construction" && x.Reference == "OVER_HEAD_COST_RATE");
            if (setting != null)
            {
                overHeadRate = Convert.ToDecimal(setting.CurrentValue);
            }
            return overHeadRate;
        }
        public static decimal GetNumberOfWorkingDaysPerYear()
        {
            var db = new ExceedDbContext();
            decimal value = new decimal();
            var configartion = db.Configurations.ToList();
            if (configartion.Any())
            {
                var workingDays = configartion.FirstOrDefault(x => x.Attribute == "WORKING_DAYS_PER_YEAR");
                if (workingDays != null)
                {
                    value = Convert.ToDecimal(workingDays.CurrentValue);
                }

            }
            return value;

        }
        public static decimal GetPEVCostRates(PEVCostRateSettingAttribute attribute, int periodId)
        {

            var amnt = new decimal();
            var db = new ExceedDbContext();
            var currentPeriod = db.PeriodMapping.Find(periodId);
            if (currentPeriod != null)
            {
                var currentComponent = db.PEVCostSetting
                    .FirstOrDefault(c => c.Attribute == attribute &&
                    c.ProjectInformationId == currentPeriod.ProjectInformationId);
                if (currentComponent != null)
                {
                    amnt = currentComponent.Value;
                }
            }

            return amnt;
        }
        /// <summary>
        /// this returns the percentage that is assigned for project progress
        /// in the performance evaluation setting
        /// </summary>
        /// <returns></returns>
        //public static decimal GetProjectProgressSetting()
        //{
        //    var db = new ExceedDbContext();
        //    decimal value = new decimal();
        //    var configartion = db.Configurations.ToList();
        //    if (configartion.Any())
        //    {
        //        var progressWeight = configartion.FirstOrDefault(x => x.Attribute == "PROGRESS");
        //        if (progressWeight != null)
        //        {
        //            value = Convert.ToDecimal(progressWeight.CurrentValue);
        //        }

        //    }
        //    return value;

        //}
        //public static decimal GetMonthlyProjectQualitySetting()
        //{
        //    var db = new ExceedDbContext();
        //    decimal value = new decimal();
        //    var configartion = db.Configurations.ToList();
        //    if (configartion.Any())
        //    {
        //        var qualityWeight = configartion.FirstOrDefault(x => x.Attribute == "QUALITY");
        //        if (qualityWeight != null)
        //        {
        //            value = Convert.ToDecimal(qualityWeight.CurrentValue);
        //        }

        //    }
        //    return value;

        //}
        //public static decimal GetMonthlyProjectPlanningSetting()
        //{
        //    var db = new ExceedDbContext();
        //    decimal value = new decimal();
        //    var configartion = db.Configurations.ToList();
        //    if (configartion.Any())
        //    {
        //        var planningWeight = configartion.FirstOrDefault(x => x.Attribute == "PLANNING");
        //        if (planningWeight != null)
        //        {
        //            value = Convert.ToDecimal(planningWeight.CurrentValue);
        //        }

        //    }
        //    return value;

        //}
        //public static decimal GetMonthlyProjectClaimsSetting()
        //{
        //    var db = new ExceedDbContext();
        //    decimal value = new decimal();
        //    var configartion = db.Configurations.ToList();
        //    if (configartion.Any())
        //    {
        //        var claimWeight = configartion.FirstOrDefault(x => x.Attribute == "CLAIM");
        //        if (claimWeight != null)
        //        {
        //            value = Convert.ToDecimal(claimWeight.CurrentValue);
        //        }

        //    }
        //    return value;

        //}
        //public static decimal GetMonthlyProjectReportSetting()
        //{
        //    var db = new ExceedDbContext();
        //    decimal value = new decimal();
        //    var configartion = db.Configurations.ToList();
        //    if (configartion.Any())
        //    {
        //        var reportWeight = configartion.FirstOrDefault(x => x.Attribute == "REPORT");
        //        if (reportWeight != null)
        //        {
        //            value = Convert.ToDecimal(reportWeight.CurrentValue);
        //        }

        //    }
        //    return value;

        //}
        //public static decimal GetMonthlyProjectCommunicationSetting()
        //{
        //    var db = new ExceedDbContext();
        //    decimal value = new decimal();
        //    var configartion = db.Configurations.ToList();
        //    if (configartion.Any())
        //    {
        //        var comnWeight = configartion.FirstOrDefault(x => x.Attribute == "COMMUNICATION");
        //        if (comnWeight != null)
        //        {
        //            value = Convert.ToDecimal(comnWeight.CurrentValue);
        //        }

        //    }
        //    return value;

        //}
        //public static decimal GetMonthlyProjectPerformanceModifierSetting()
        //{
        //    var db = new ExceedDbContext();
        //    decimal value = new decimal();
        //    var component = db.PerformanceEvaluationComponentWeight.FirstOrDefault(c => c.ComponentName == ProjectPerformanceEvaluationComponents.MODIFIERS);
        //    if (component != null)
        //    {
        //        value = component.Value;
        //    }
        //    return value;

        //}

        public static decimal GetComponentWeight(ProjectPerformanceEvaluationComponents coponent)
        {
            var db = new ExceedDbContext();
            var wgt = new decimal();
            var currentComponent =
                db.PerformanceEvaluationComponentWeight.FirstOrDefault(c => c.ComponentName == coponent);
            if (currentComponent != null)
            {
                wgt = currentComponent.Value;
            }
            return wgt;
        }
        public static decimal GetComponentWeight(ProjectPerformanceEvaluationComponents coponent, AvailableProjectTypes projectType)
        {
            var db = new ExceedDbContext();
            var wgt = new decimal();
            var currentComponent =
                db.PerformanceEvaluationComponentWeight.FirstOrDefault(c => c.ComponentName == coponent);
            if (currentComponent != null)
            {
                if (projectType == AvailableProjectTypes.Road)
                {
                    wgt = currentComponent.ValueForRoadProjects;
                }
                else
                    wgt = currentComponent.Value;
            }
            return wgt;
        }
        /// <summary>
        /// Gets the componenet weight based on the project type;
        /// </summary>
        /// <param name="coponent"></param>
        /// <param name="periodId"></param>
        /// <returns></returns>
        public static decimal GetComponentWeight(ProjectPerformanceEvaluationComponents coponent, int periodId)
        {
            var db = new ExceedDbContext();
            var performancePeriod = db.PeriodMapping.Find(periodId);
            var wgt = new decimal();
            var currentComponent =
                db.PerformanceEvaluationComponentWeight.FirstOrDefault(c => c.ComponentName == coponent);
            if (performancePeriod != null && currentComponent != null)
            {
                var type = performancePeriod.ProjectInformation.ProjectType;
                if (type == AvailableProjectTypes.Building)
                {
                    wgt = currentComponent.Value;
                }
                else if (type == AvailableProjectTypes.Road)
                {
                    wgt = currentComponent.ValueForRoadProjects;
                }
            }

            return wgt;
        }
        public static decimal GetProjectSpecificSettingValue(int projectId, ProjectSettingAttributes attributeName)
        {
            var db = new ExceedDbContext();
            var value = new decimal();
            var comp = db.ProjectSpecificSettingAttributes.FirstOrDefault(s => s.ProjectInformationId == projectId && s.Attribute == attributeName);
            if (comp != null)
            {
                value = comp.Value;
            }
            return value;
        }
        /// <summary>
        /// Get the retention rate from the payment setting
        /// </summary>
        /// <returns></returns>
        public static decimal GetCurrentRetensionRate(int projectId)
        {
            var db = new ExceedDbContext();
            decimal currentRate = new decimal();
            // general setting
            var currentSetting = db.ProjectPaymentAttributes.FirstOrDefault(at => at.AttributeName == ProjectPaymentAttributeName.Retention);
            //
            var projectSpecificSetting = db.ProjectPaymentTerms.FirstOrDefault(atr => atr.Contract.ProjectInformationId == projectId && atr.AttributeName == ProjectPaymentAttributeName.Retention);
            if (projectSpecificSetting != null)
            {
                currentRate = projectSpecificSetting.Value;
            }
            else
            {
                if (currentSetting != null)
                {
                    currentRate = currentSetting.Value;
                }
            }

            return currentRate;
        }
        /// <summary>
        /// Get the VAT rate from the payment setting
        /// </summary>
        /// <returns></returns>
        //public static decimal GetCurrentVatRate(int projectId)
        //{
        //    var db = new ExceedDbContext();
        //    decimal currentRate = new decimal();
        //    var setting = db.ProjectPaymentTerms.FirstOrDefault(at => at.AttributeName == ProjectPaymentAttributeName.VAT);
        //    if (setting != null)
        //    {
        //        currentRate = setting.Value;
        //    }
        //    return currentRate;
        //}
        public static decimal GetCurrentAdvancedPaymentRate(int projectId)
        {
            var db = new ExceedDbContext();
            decimal adPaymentRate = new decimal();
            //
            // General payment 
            var adPayRateSetting = db.ProjectPaymentAttributes.FirstOrDefault(at => at.AttributeName == ProjectPaymentAttributeName.Advance_Payment_Rate);
            //
            // Project specific payment terms 
            var projAdPaySetting =
                db.ProjectPaymentTerms.FirstOrDefault(
                    atr => atr.Contract.ProjectInformationId == projectId && atr.AttributeName == ProjectPaymentAttributeName.Advance_Payment_Rate);
            if (projAdPaySetting != null)
            {
                adPaymentRate = projAdPaySetting.Value;
            }
            else

            {
                if (adPayRateSetting != null)
                {
                    adPaymentRate = adPayRateSetting.Value;
                }

            }
            return adPaymentRate;
        }
        /// <summary>
        /// Calculate advance re-payment rate from advance payment rate and 
        /// project comletion rate for complete advance repayment
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static decimal GetCalculatedAdvanceRepaymentRate(int projectId)
        {
            var projectSpecificPaymentTermsExist = false;

            var db = new ExceedDbContext();
            decimal adPaymentRate = new decimal();
            //
            // General payment 
            var compAdRepayRateSetting = db.ProjectPaymentAttributes.FirstOrDefault(at => at.AttributeName == ProjectPaymentAttributeName.Project_Percentage_for_Complete_Advance_Repayment);
            var adPayRateSetting = db.ProjectPaymentAttributes.FirstOrDefault(at => at.AttributeName == ProjectPaymentAttributeName.Advance_Payment_Rate);
            //
            // Project specific payment terms 
            var projAdPaySetting =
                db.ProjectPaymentTerms.FirstOrDefault(
                    atr => atr.Contract.ProjectInformationId == projectId && atr.AttributeName == ProjectPaymentAttributeName.Advance_Payment_Rate);
            var projCompAdRepayRateSetting =
                db.ProjectPaymentTerms.FirstOrDefault(
                    atr => atr.Contract.ProjectInformationId == projectId && atr.AttributeName == ProjectPaymentAttributeName
                               .Project_Percentage_for_Complete_Advance_Repayment);

            if (projAdPaySetting != null && projCompAdRepayRateSetting != null)
            {
                if (projAdPaySetting.Value > 0 && projCompAdRepayRateSetting.Value > 0)
                {
                    projectSpecificPaymentTermsExist = true;
                }
            }
            if (projectSpecificPaymentTermsExist)
            {
                if (projCompAdRepayRateSetting.Value > 100)
                {
                    adPaymentRate = projAdPaySetting.Value;
                }
                else
                {
                    adPaymentRate = projAdPaySetting.Value / (projCompAdRepayRateSetting.Value / 100);
                }
            }
            else
            {
                if (compAdRepayRateSetting != null && adPayRateSetting != null)
                {
                    if (compAdRepayRateSetting.Value == 0 || compAdRepayRateSetting.Value > 100)
                    {
                        adPaymentRate = adPayRateSetting.Value;
                    }
                    adPaymentRate = adPayRateSetting.Value / (compAdRepayRateSetting.Value / 100);
                }
            }

            return adPaymentRate;
        }
        public static decimal GetCurrentCompleteAdvancedRePaymentRate()
        {
            var db = new ExceedDbContext();
            decimal currentRate = new decimal();
            var setting = db.ProjectPaymentAttributes.FirstOrDefault(at => at.AttributeName == ProjectPaymentAttributeName.Project_Percentage_for_Complete_Advance_Repayment);
            if (setting != null)
            {
                currentRate = setting.Value;
            }
            return currentRate;
        }
        //
        //  summary:
        //      Update project when project specific settings are added
        public static void UpdateProjectAfterSettingChanges(int projectId)
        {
            var db = new ExceedDbContext();
            var currentProject = db.ProjectInformation.Find(projectId);
            if (currentProject != null)
            {
                var overHeadRateAttribute = db.ProjectSpecificSettingAttributes.FirstOrDefault(a => a.ProjectInformationId == currentProject.ProjectInformationId && a.Attribute == ProjectSettingAttributes.Overhead_Cost_Rate);
                //var profitRate = db.ProjectSpecificSettingAttributes.FirstOrDefault(a => a.ProjectInformationId == currentProject.ProjectInformationId && a.Attribute == ProjectSettingAttributes.Profit_Cost_Rate);
                var projectWorkItems = db.WorkItemPerProject.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId && !w.IsSubCategory).ToList();
                // 
                // update work items' over head costs
                if (projectWorkItems.Any() && overHeadRateAttribute != null)
                {
                    foreach (WorkItemPerProject workItem in projectWorkItems)
                    {
                        workItem.OverheadRate = overHeadRateAttribute.Value;
                        workItem.OverHeadCost = workItem.DirectCost * overHeadRateAttribute.Value / 100;
                        workItem.TotalUnitCost = workItem.LabourHourlyOutPut > 0 ?
                                     (workItem.DirectCost + workItem.OverHeadCost + workItem.ProfitCost) / workItem.LabourHourlyOutPut :
                                     workItem.DirectCost + workItem.OverHeadCost + workItem.ProfitCost;

                        db.WorkItemPerProject.Attach(workItem);
                        db.Entry(workItem).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var pieceRates = db.AreaWorkItemes.Where(pr => pr.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                if (pieceRates.Any() && overHeadRateAttribute != null)
                {
                    foreach (AreaWorkItem pieceRate in pieceRates)
                    {
                        pieceRate.OverHeadProfitCost = (pieceRate.DirectCost * overHeadRateAttribute.Value / 100) + (pieceRate.DirectCost * GetCurrentProfitRate() / 100);

                        db.AreaWorkItemes.Attach(pieceRate);
                        db.Entry(pieceRate).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var subContractWorkItems = db.SubContractWorkItem.Where(scw => scw.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                if (subContractWorkItems.Any() && overHeadRateAttribute != null)
                {
                    foreach (SubContractWorkItem workItem in subContractWorkItems)
                    {
                        workItem.OverHeadProfit = (workItem.SelectedCost * overHeadRateAttribute.Value / 100) + (workItem.SelectedCost * GetCurrentProfitRate() / 100);

                        db.SubContractWorkItem.Attach(workItem);
                        db.Entry(workItem).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var genericWorkItems = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId && !w.IsSubCategory).ToList();
                if (genericWorkItems.Any() && overHeadRateAttribute != null)
                {
                    foreach (GenericWorkItemPerProject workItem in genericWorkItems)
                    {
                        workItem.OverheadRate = overHeadRateAttribute.Value;
                        workItem.OverHeadCost = workItem.DirectCost * overHeadRateAttribute.Value / 100;
                        //workItem.TotalUnitCost = workItem.DirectCost + workItem.OverHeadCost + workItem.ProfitCost;
                        workItem.TotalUnitCost = workItem.LabourHourlyOutPut > 0 ?
                                    (workItem.DirectCost + workItem.OverHeadCost + workItem.ProfitCost) / workItem.LabourHourlyOutPut :
                                    workItem.DirectCost + workItem.OverHeadCost + workItem.ProfitCost;

                        workItem.TotalUnitCostIncludingTransport = workItem.TotalUnitCost + workItem.CalculatedTransportCost;

                        db.GenericWorkItemPerProject.Attach(workItem);
                        db.Entry(workItem).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        public static string GetEVMPeriodFiscalYear(int evmPeriodId)
        {
            var year = "";
            var db = new ExceedDbContext();
            var evmPeriod = db.EarnedValueManagementPeriods.Find(evmPeriodId);
            if (evmPeriod != null)
            {
                var consPeroid = db.ConstructionPeriod.Find(evmPeriod.ConstructionPeriodId);
                if (consPeroid != null)
                {
                    var glPeriod = db.GLPeriods.FirstOrDefault(p => p.Name == consPeroid.Name);
                    if (glPeriod != null)
                    {
                        year = glPeriod.GlFiscalYear.Name;
                    }
                }
            }

            return year;
        }

        public static string GetConstructionPeriodFiscalYear(int constPeriodId)
        {
            var year = "";
            var db = new ExceedDbContext();

            var consPeroid = db.ConstructionPeriod.Find(constPeriodId);
            if (consPeroid != null)
            {
                var glPeriod = db.GLPeriods.FirstOrDefault(p => p.Name == consPeroid.Name);
                if (glPeriod != null)
                {
                    year = glPeriod.GlFiscalYear.Name;
                }
            }

            return year;
        }
        public static int? GetStructureTypeCode(int configId, AvailableProjectTypes type)
        {
            int? code = null;
            var db = new ExceedDbContext();
            var mapping = db.ProjectTypeMappings.FirstOrDefault(pm => pm.ConfigurationId == configId && pm.ProjectType == type);
            if (mapping != null)
            {
                code = mapping.CodeOrSeries;
            }
            return code;
        }
    }
}
