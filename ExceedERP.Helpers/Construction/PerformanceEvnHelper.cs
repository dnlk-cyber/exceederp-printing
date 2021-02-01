using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Construction
{
    public static class PerformanceEvnHelper
    {
        /// <summary>
        /// Get the planned amount for a given month  from the project progress
        /// </summary>
        /// <param name="periodId"></param>
        /// <returns></returns>
        public static decimal GetPlannedAmountOfMonth(int periodId)
        {
            var db = new ExceedDbContext();
            decimal plannedAmount = new decimal();
            var currentPeriod = db.PeriodMapping.Find(periodId);
            if (currentPeriod != null)
            {
                var progress = db.MonthlyProjectProgress.Where(p => p.PeriodMappingId == currentPeriod.PeriodMappingId).FirstOrDefault();
                if (progress != null)
                {
                    plannedAmount = progress.PlannedAmount;
                }
            }
            return plannedAmount;
        }
        public static decimal GetPlannedAmountToDate(int periodId)
        {
            var db = new ExceedDbContext();
            decimal plannedAmountToDate = new decimal();
            var currentPeriod = db.PeriodMapping.Find(periodId);
            if (currentPeriod != null)
            {
                var progress = db.MonthlyProjectProgress.Where(p => p.PeriodMappingId == currentPeriod.PeriodMappingId).FirstOrDefault();
                if (progress != null)
                {
                    plannedAmountToDate = progress.PlannedToDate;
                }
            }
            return plannedAmountToDate;
        }
        public static decimal GetExecutedAmountOfMonth(int periodId)
        {
            var db = new ExceedDbContext();
            decimal executedAmount = new decimal();
            var currentPeriod = db.PeriodMapping.Find(periodId);
            if (currentPeriod != null)
            {
                var progress = db.MonthlyProjectProgress.Where(p => p.PeriodMappingId == currentPeriod.PeriodMappingId).FirstOrDefault();
                if (progress != null)
                {
                    executedAmount = progress.ExecutedAmount;
                }
            }
            return executedAmount;
        }
        public static decimal GetExecutedAmountToDate(int periodId)
        {
            var db = new ExceedDbContext();
            decimal executedAmountToDate = new decimal();
            var currentPeriod = db.PeriodMapping.Find(periodId);
            if (currentPeriod != null)
            {
                var progress = db.MonthlyProjectProgress.Where(p => p.PeriodMappingId == currentPeriod.PeriodMappingId).FirstOrDefault();
                if (progress != null)
                {
                    executedAmountToDate = progress.ExecutedAmountToDate;
                }
            }
            return executedAmountToDate;
        }
        public static decimal GetExecutedAmountTillPrevMonth(int periodId)
        {
            var db = new ExceedDbContext();
            decimal executedAmountTillPrevMonth = new decimal();
            var currentPeriod = db.PeriodMapping.Find(periodId);
            if (currentPeriod != null)
            {
                var progress = db.MonthlyProjectProgress.Where(p => p.PeriodMappingId == currentPeriod.PeriodMappingId).FirstOrDefault();
                if (progress != null)
                {
                    executedAmountTillPrevMonth = progress.ExecutedAmountTillPreviousMonth;
                }
            }
            return executedAmountTillPrevMonth;
        }
        public static string GetConcatnatedPaymentApprovalTime(MonthlyProjectPayment model)
        {
            var approvalTime = model.ApprovalTime;
            var concatnatedValue = "";
            if (approvalTime == MonthlyDueDate.On_Due_Date)
            {
                concatnatedValue = (MonthlyDueDate.On_Due_Date.ToString()).Replace('_', ' ');
            }
            else if (approvalTime == MonthlyDueDate.Before_Due_Date)
            {
                concatnatedValue = model.Days + " days " + MonthlyDueDate.Before_Due_Date.ToString().Replace('_', ' ') + "( " + model.Points + "pts )";
            }
            else if (approvalTime == MonthlyDueDate.After_Due_Date)
            {
                concatnatedValue = model.Days + " days " + MonthlyDueDate.After_Due_Date.ToString().Replace('_', ' ') + "( " + model.Points + "pts )";
            }
            return concatnatedValue;
        }
        public static string GetConcatnatedDueDateValue(MonthlyDueDate submissionTime, decimal? days)
        {
            var val = "";
            if (submissionTime == MonthlyDueDate.On_Due_Date)
            {
                val = MonthlyDueDate.On_Due_Date.ToString().Replace('_', ' ');
            }
            else if (submissionTime == MonthlyDueDate.After_Due_Date)
            {
                val = days.ToString() + " days " + MonthlyDueDate.After_Due_Date.ToString().Replace('_', ' ');
            }
            else if (submissionTime == MonthlyDueDate.Before_Due_Date)
            {
                val = days.ToString() + " days " + MonthlyDueDate.Before_Due_Date.ToString().Replace('_', ' ');
            }
            return val;
        }
        /// <summary>
        /// this returns the correction factor for project progress in a given month
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static decimal GetMonthlyProgressCorrectionFactor(MonthlyProjectProgress model)
        {
            var correctionFactor = new decimal();
            var isScheduleWeeklyBase = model.ScheduleBrokenDownToWeeklyBase;
            var areFMAware = model.AreFormenAwareOfIt;
            if (isScheduleWeeklyBase && areFMAware)
            {
                correctionFactor = (decimal)(model.NumberOfEvaluationMeething / 4.0);
            }
            else if (!isScheduleWeeklyBase && !areFMAware)
            {
                correctionFactor = (decimal)(0.8 * model.NumberOfEvaluationMeething / 4.0);
            }
            else if ((!isScheduleWeeklyBase && areFMAware) || (isScheduleWeeklyBase && !areFMAware))
            {
                correctionFactor = (decimal)(0.9 * model.NumberOfEvaluationMeething / 4.0);
            }
            return correctionFactor;
        }

        public static decimal CalculateScoredPoints(MonthlyProjectCommunication model)
        {
            var comn = model.ComnLevel;
            var pt = new decimal();
            if (comn == MonthlyProjectCommunicationLevel.Very_Good)
            {
                pt = 2;
            }
            else if (comn == MonthlyProjectCommunicationLevel.Good)
            {
                pt = 1;
            }
            else if (comn == MonthlyProjectCommunicationLevel.Not_Good)
            {
                pt = 0;
            }
            else if (comn == MonthlyProjectCommunicationLevel.Poor)
            {
                pt = -1;
            }
            return pt;
        }

        public static decimal GetHealthAndSafetyScore(PerformanceModifierHealthAndSafety model)
        {
            decimal score = new decimal();
            var injury = model.InjuryLevel;
            if (injury == PerformanceModifierInjuryLevel.Heavy)
            {
                score = -5 * model.InjuryCount;
            }
            else if (injury == PerformanceModifierInjuryLevel.Medium)
            {
                score = -2 * model.InjuryCount;

            }
            else if (injury == PerformanceModifierInjuryLevel.Light)
            {
                score = -1 * model.InjuryCount;

            }
            return score;
        }

        /// <summary>
        /// update monthly quality when one of the four sub-categories is changed
        /// 
        /// </summary>
        /// <param name="modelId"></param>
        public static void UpdateMonthlyProjectQualityScore(int modelId)
        {
            var db = new ExceedDbContext();

            var id = modelId;
            var qualityPlanningList = db.QualityPlanning.Where(qp => qp.MonthlyProjectQualityId == id).ToList();
            var qualityAssuranceList = db.QualityAssurance.Where(qa => qa.MonthlyProjectQualityId == id).ToList();
            var qualityControlList = db.QualityControl.Where(qc => qc.MonthlyProjectQualityId == id).ToList();
            var qualityComplaintList = db.MonthlyProjectQualityComplaints.Where(complaint => complaint.MonthlyProjectQualityId == id);

            var qualityPlanningScore = qualityPlanningList.Sum(m => m.Points);
            var qualityAssuranceScore = qualityAssuranceList.Sum(m => m.Points);
            var qualityControlScore = qualityControlList.Sum(m => m.Points);
            var qualityComplaints = qualityComplaintList.Sum(m => m.NumOfComplaints);

            var totalScore = qualityPlanningScore + qualityAssuranceScore + qualityControlScore - qualityComplaints;

            var allowablePercentage = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.QUALITY);
            var contextEntity = db.MonthlyProjectQuality.Find(id);
            if (contextEntity != null)
            {
                contextEntity.QualityPlanning = qualityPlanningScore;
                contextEntity.QualityAssurance = qualityAssuranceScore;
                contextEntity.QualityControl = qualityControlScore;
                contextEntity.QualityComplaint = 0 - qualityComplaints; // make the complaint score a -ve value
                contextEntity.TotalPoints = totalScore;
                contextEntity.MaxAllowablePoints = allowablePercentage;

                db.MonthlyProjectQuality.Attach(contextEntity);
                db.Entry(contextEntity).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        public static void UpdateMonthlyProjectPlanningScore(int planningId)
        {
            var db = new ExceedDbContext();
            var subAcToManualList = db.ScheduleSubmissionAcToOperationManual.Where(m => m.MonthlyProjectPlanningId == planningId).ToList();
            var subAcToContractList = db.RScheduleSubmissionAcToContract.Where(m => m.MonthlyProjectPlanningId == planningId).ToList();
            var completnessList = db.ProjectPnanningCompleteness.Where(m => m.MonthlyProjectPlanningId == planningId).ToList();
            var acceptanceList = db.ProjectPlanningQualityAndAcceptance.Where(m => m.MonthlyProjectPlanningId == planningId).ToList();

            var totalPoint = subAcToContractList.Sum(m => m.Points) +
                subAcToManualList.Sum(m => m.Points) +
                completnessList.Sum(m => m.Points) +
                acceptanceList.Sum(m => m.points);

            var allowablePercentage = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PLANNING);
            var entityContext = db.MonthlyProjectPlanning.Find(planningId);
            if (entityContext != null)
            {
                entityContext.RevScheduleSubAcToManual = subAcToManualList.Sum(m => m.Points);
                entityContext.RevScheduleSubAcToContract = subAcToContractList.Sum(m => m.Points);
                entityContext.Completness = completnessList.Sum(m => m.Points);
                entityContext.QualityAndAcceptablity = acceptanceList.Sum(m => m.points);
                entityContext.TotalPoints = totalPoint;
                entityContext.MaxAllowablePoints = allowablePercentage;

                db.MonthlyProjectPlanning.Attach(entityContext);
                db.Entry(entityContext).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void UpdateMonthlyProjectClaimsScore(int modelId)
        {
            var db = new ExceedDbContext();

            var id = modelId;
            var l1 = db.ContempraryRecord.Where(qp => qp.MonthlyProjectClaimId == id).ToList();
            var l2 = db.RequirementForMonthlyClaims.Where(qa => qa.MonthlyProjectClaimId == id).ToList();
            var l3 = db.AnalysisTechniqueApplicationForMonthlyClaims.Where(qc => qc.MonthlyProjectClaimId == id).ToList();
            var l4 = db.ApprovalStatusForClaimsResponded.Where(complaint => complaint.MonthlyProjectClaimId == id);

            var s1 = l1.Sum(m => m.Points);
            var s2 = l2.Sum(m => m.Points);
            var s3 = l3.Sum(m => m.Points);
            var s4 = l4.Sum(m => m.Points);

            var totalScore = s1 + s2 + s3 + s4;

            var allowablePercentage = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.CLAIM);
            var contextEntity = db.MonthlyProjectClaim.Find(id);
            if (contextEntity != null)
            {
                contextEntity.ContempraryRecordKeeping = s1;
                contextEntity.FullFillingRequirementForClaims = s2;
                contextEntity.AnalysisTechnique = s3;
                contextEntity.ApprovalStatusOfClaims = s4;
                contextEntity.Points = totalScore;
                contextEntity.MaxAllowablePoints = allowablePercentage;

                db.MonthlyProjectClaim.Attach(contextEntity);
                db.Entry(contextEntity).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        public static void UpdateMonthlyReport(int modelId)
        {
            var db = new ExceedDbContext();

            var id = modelId;
            var l1 = db.MonthlyProjectReportCompletnessComponent.Where(qp => qp.MonthlyProjectReportId == id).ToList();
            var l2 = db.MonthlyProjectReportNarrative.Where(qa => qa.MonthlyProjectReportId == id).ToList();

            var s1 = l1.Sum(m => m.Points);
            var s2 = l2.Sum(m => m.Points);
            var subTotal = s1 + s2;

            var allowablePercentage = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.REPORT);
            var contextEntity = db.MonthlyProjectReport.Find(id);
            if (contextEntity != null)
            {
                contextEntity.Completeness = s1;
                contextEntity.Narrative = s2;

                contextEntity.MaxAllowablePoints = allowablePercentage;
                contextEntity.TotalPoints = contextEntity.Points + subTotal;

                db.MonthlyProjectReport.Attach(contextEntity);
                db.Entry(contextEntity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void UpdateMonthlyProjectComn(int id)
        {
            var db = new ExceedDbContext();
            var l1 = db.MonthlyProjectCommunicationWithinProject.Where(m => m.MonthlyProjectCommunicationId == id).ToList();
            var l2 = db.MonthlyProjectCommunicationWithHeadOffice.Where(m => m.MonthlyProjectCommunicationId == id).ToList();
            var l3 = db.MonthlyProjectCommunicationWithConsultant.Where(m => m.MonthlyProjectCommunicationId == id).ToList();

            var s1 = l1.Sum(m => m.Points);
            var s2 = l2.Sum(m => m.Points);
            var s3 = l3.Sum(m => m.Points);

            var allowablePercentage = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.COMMUNICATION);
            var contextEntity = db.MonthlyProjectCommunication.Find(id);
            if (contextEntity != null)
            {
                contextEntity.ComnWithinProject = s1;
                contextEntity.ComnWithHeadOffice = s2;
                contextEntity.ComnWithConsultant = s3;

                contextEntity.MaxAllowablePoints = allowablePercentage;
                contextEntity.TotalPoints = s1 + s2 + s3 + contextEntity.GoodGovernanceComplaints;

                db.MonthlyProjectCommunication.Attach(contextEntity);
                db.Entry(contextEntity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private static decimal GetMaterialUtilizationScore(int monthlyModifierId)
        {
            var id = monthlyModifierId;
            var db = new ExceedDbContext();
            var materialList = db.PerformanceModifierMaterialUtilization.Where(mu => mu.MonthlyProjectPerformanceModifierId == id).ToList();
            var avgUtilization = materialList.Sum(mu => mu.UtilizedAmount) / materialList.Count();
            var averageWastage = 100 - avgUtilization;

            var totalScore = new decimal();
            if (averageWastage > 2)
            {
                //while (averageWastage > 0)
                //{
                //    totalScore = totalScore - 1;
                //    averageWastage = averageWastage - 2;

                //}
                totalScore = (int)averageWastage / 2 * (-1);
            }
            return totalScore;
        }

        public static void UpdateMonthlyPerformanceModifier(int id)
        {
            var db = new ExceedDbContext();
            var hsList = db.PerformanceModifierHealthAndSafety.Where(m => m.MonthlyProjectPerformanceModifierId == id).ToList();
            var bReqList = db.PerformanceModifierBudgetRequest.Where(m => m.MonthlyProjectPerformanceModifierId == id).ToList();
            var bUtlList = db.PerformanceModifierBudgetUtilization.Where(m => m.MonthlyProjectPerformanceModifierId == id).ToList();
            var mUtlList = db.PerformanceModifierMaterialUtilization.Where(m => m.MonthlyProjectPerformanceModifierId == id).ToList();
            var scComplaints = db.PerformanceModifierSubContractAdministration.Where(m => m.MonthlyProjectPerformanceModifierId == id).ToList();

            var allowablePercentage = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIERS);

            var entityFromContext = db.MonthlyProjectPerformanceModifier.Find(id);
            if (entityFromContext != null)
            {
                entityFromContext.HealthAndSafety = hsList.Sum(m => m.Points);
                entityFromContext.BudgetRequestAndUtilization =
                    bReqList.Sum(m => m.Points) + bUtlList.Sum(m => m.Points);
                entityFromContext.MaterialUtilization = GetMaterialUtilizationScore(entityFromContext.MonthlyProjectPerformanceModifierId);
                entityFromContext.SubContractAdministration = (decimal)scComplaints.Sum(m => m.Points);

                entityFromContext.MaxAllowablePoints = (-1) * allowablePercentage;
                entityFromContext.TotalPoints = entityFromContext.HealthAndSafety +
                                                entityFromContext.BudgetRequestAndUtilization +
                                                entityFromContext.MaterialUtilization +
                                                entityFromContext.SubContractAdministration;

                db.MonthlyProjectPerformanceModifier.Attach(entityFromContext);
                db.Entry(entityFromContext).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public static void UpdateMonthlyPerformanceModifierAfterModifyingMaterialUtilization(int id)
        {
            var db = new ExceedDbContext();
            var mUtlList = db.PerformanceModifierMaterialUtilization.Where(m => m.MonthlyProjectPerformanceModifierId == id).ToList();

            var allowablePercentage = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIERS);
            var matUtilWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_MATERIAL_UTILIZATION);
            var entityFromContext = db.MonthlyProjectPerformanceModifier.Find(id);
            if (entityFromContext != null)
            {
                
                entityFromContext.MaterialUtilization = GetMaterialUtilizationScore(entityFromContext.MonthlyProjectPerformanceModifierId);
                if(entityFromContext.MaterialUtilization < -1 * matUtilWeight)
                {
                    entityFromContext.MaterialUtilization = -1 * matUtilWeight;
                }
                entityFromContext.TotalPoints = entityFromContext.HealthAndSafety +
                    entityFromContext.BudgetRequestAndUtilization + entityFromContext.MaterialUtilization +
                    entityFromContext.SubContractAdministration;
                if(entityFromContext.TotalPoints < -1 * allowablePercentage)
                {
                    entityFromContext.TotalPoints = -1 * allowablePercentage;
                }
                db.MonthlyProjectPerformanceModifier.Attach(entityFromContext);
                db.Entry(entityFromContext).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //
        // cost
        public static decimal GetPlannedProjectDirectCostForMonth(int periodId)
        {
            var db = new ExceedDbContext();
            
            //var currentOverHeadRate = ConsSettingHelper.GetCurrentOverHeadRate();
            //var currentProfitRate = ConsSettingHelper.GetCurrentProfitRate();
            //var directCostRate = 100 - (currentOverHeadRate + currentProfitRate);
            var directCostRate = ConsSettingHelper.GetPEVCostRates(PEVCostRateSettingAttribute.PlannedDirectCostRate, periodId);
            
            var plannedCostForTheMonth = GetPlannedAmountOfMonth(periodId);

            return plannedCostForTheMonth * directCostRate;
        }
        public static decimal GetPlannedProjectOverHeadCost(int periodId)
        {
            var db = new ExceedDbContext();
            //var currentOverHeadRate = ConsSettingHelper.GetCurrentOverHeadRate();
            var currentOverHeadRate = ConsSettingHelper.GetPEVCostRates(PEVCostRateSettingAttribute.PlannedProjectOHCostRate, periodId);
            var plannedCostForTheMonth = GetPlannedAmountOfMonth(periodId);

            return plannedCostForTheMonth * currentOverHeadRate;
        }
        public static decimal GetPlannedProjectProfitAndOHCost(int periodId)
        {
            var db = new ExceedDbContext();
            //var currentOverHeadRate = ConsSettingHelper.GetCurrentOverHeadRate();
            //var currentProfitRate = ConsSettingHelper.GetCurrentProfitRate();
            var currentProfitAndHeadOfficeOHRate = ConsSettingHelper.GetPEVCostRates(PEVCostRateSettingAttribute.PlannedProfitAndHeadOfficeOHCostRate, periodId);

            var plannedCostForTheMonth = GetPlannedAmountOfMonth(periodId);

            return plannedCostForTheMonth * currentProfitAndHeadOfficeOHRate ;
        }
        /// <summary>
        /// calculate the actual profit and head office over head cost for a given month
        /// </summary>
        /// <param name="model"></param>
        /// <param name="periodId"></param>
        /// <returns></returns>
        public static decimal GetActualProfitAndHeadOfficeOH(MonthlyProjectCost model, int periodId)
        {
            //var db = new ExceedDbContext();
            var executedAmountInTheMonth = GetExecutedAmountOfMonth(periodId);
            return executedAmountInTheMonth - (model.ActualDirectCost + model.ActualProjectOHCost);
        }
        public static decimal GetPlannedDirectCostTodate(int periodId)
        {
            var db = new ExceedDbContext();
            //var currentOverHeadRate = ConsSettingHelper.GetCurrentOverHeadRate();
            //var currentProfitRate = ConsSettingHelper.GetCurrentProfitRate();
            var directCostRate = ConsSettingHelper.GetPEVCostRates(PEVCostRateSettingAttribute.PlannedDirectCostRate, periodId);
            //var totalProjectAmount = ContractMgmtHelper.GetProjectTotalAmount(periodId);
            var plannedAmntToDate = GetPlannedAmountToDate(periodId);
            return plannedAmntToDate * directCostRate;
        }
        public static decimal GetPlannedOHcostToDate(int periodId)
        {
            var db = new ExceedDbContext();
            //var currentOverHeadRate = ConsSettingHelper.GetCurrentOverHeadRate();
            var currentOverHeadRate = ConsSettingHelper.GetPEVCostRates(PEVCostRateSettingAttribute.PlannedProjectOHCostRate, periodId);

            var plannedAmntToDate = GetPlannedAmountToDate(periodId);
            return currentOverHeadRate * plannedAmntToDate;
        }
        public static decimal GetPlannedProfitAndHeadOfficeOHToDate(int periodId)
        {
            var db = new ExceedDbContext();
            //var currentOverHeadRate = ConsSettingHelper.GetCurrentOverHeadRate();
            //var currentProfitRate = ConsSettingHelper.GetCurrentProfitRate();
            var currentProfitAndHeadOfficeOHRate = ConsSettingHelper.GetPEVCostRates(PEVCostRateSettingAttribute.PlannedProfitAndHeadOfficeOHCostRate, periodId);

            var plannedAmntToDate = GetPlannedAmountToDate(periodId);

            return plannedAmntToDate * currentProfitAndHeadOfficeOHRate ;
        }
        public static decimal GetActualProfitAndOHCostToDate(MonthlyProjectCost model, int periodId)
        {
            decimal result = new decimal();

            var executedAmountToDate = GetExecutedAmountToDate(periodId);
            result = executedAmountToDate - model.ActualDirectCostToDate - model.ActualProjectOHCostToDate;

            return result;
        }

        //

        //
        // payment 
        public static decimal GetExpectedToBeApprovedThisMonth(MonthlyProjectPayment model, int periodId)
        {
            var executedToDate = GetExecutedAmountToDate(periodId);
            var approveToDate = model.ApprovedTillPreMonth + model.AmountApprovedThisMonth;

            return executedToDate - approveToDate;

        }
        public static decimal GetAmountNotApprovedThisMonth(MonthlyProjectPayment model, int periodId)
        {
            //var executedTillPrevMonth = GetExecutedAmountTillPrevMonth(periodId);
            var expectedTobeApprovedThisMonth = GetExpectedToBeApprovedThisMonth(model, periodId);
            //decimal amntNotApproved = new decimal();
            return expectedTobeApprovedThisMonth - model.AmountApprovedThisMonth;
            //{
            //    amntNotApproved = executedTillPrevMonth - model.AmountApprovedThisMonth;
            //}
            //return amntNotApproved;
        }
        public static decimal GetApprovedAmountToDate(MonthlyProjectPayment model)
        {
            return model.AmountApprovedThisMonth + model.ApprovedTillPreMonth;
        }
        public static decimal GetAmountNotApprovedToDate(MonthlyProjectPayment model, int periodId)
        {
            var approvedToDate = GetApprovedAmountToDate(model);
            var executedToDate = GetExecutedAmountToDate(periodId);
            var notApprovedToDate = new decimal();
            if (executedToDate - approvedToDate > 0)
            {
                notApprovedToDate = executedToDate - approvedToDate;
            }
            return notApprovedToDate;
        }
        public static decimal GetMatOnSiteNotApprovedThisMonth(MonthlyProjectPayment model)
        {
            //var matNotApproved = new decimal();
            if (model.MatOnSiteThisMonth == null)
            {
                model.MatOnSiteThisMonth = 0;
            }
            if (model.MatOnSiteApprovedThisMonth == null)
            {
                model.MatOnSiteApprovedThisMonth = 0;
            }
            return (decimal)0.5 * (decimal)model.MatOnSiteThisMonth - (decimal)model.MatOnSiteApprovedThisMonth;
        }

        #region Planning
        public static decimal GetScoreOfScheduleRevAccToManual(MonthlyProjectPlanning projectPlanning)
        {
            decimal score = new decimal();
            var submissionTime = projectPlanning.SubmssinTimeAcctoManual;
            if (projectPlanning.IsRevisionTimeAccManual)
            {
                if (submissionTime == MonthlyDueDate.On_Due_Date)
                {
                    score = 3;
                }
                else if (submissionTime == MonthlyDueDate.Before_Due_Date)
                {
                    score = projectPlanning.DaysAfterOrBeforeDueDateAccToManual + 3;
                }
                else if (submissionTime == MonthlyDueDate.After_Due_Date)
                {
                    score = 3 - projectPlanning.DaysAfterOrBeforeDueDateAccToManual;
                }

            }

            return score;
        }

        public static decimal GetScoreOfScheduleRevAccToContract(MonthlyProjectPlanning projectPlanning)
        {
            decimal score = new decimal();
            var submissionTime = projectPlanning.SubmssinTimeAcctoContract;
            if (projectPlanning.IsRevisionTimeAccContract)
            {
                if (submissionTime == MonthlyDueDate.On_Due_Date)
                {
                    score = 3;
                }
                else if (submissionTime == MonthlyDueDate.Before_Due_Date)
                {
                    score = projectPlanning.DaysAfterOrBeforeDueDateAccToContract + 3;
                }
                else if (submissionTime == MonthlyDueDate.After_Due_Date)
                {
                    score = 3 - projectPlanning.DaysAfterOrBeforeDueDateAccToContract;
                }

            }

            return score;

        }

        public static decimal GetQualityAndAcceptablityScore(MonthlyProjectPlanning projectPlanning)
        {
            decimal score = new decimal();
            var approval = projectPlanning.ApprovalType;
            if (projectPlanning.IsApprovalMadeInTheMonth)
            {
                if (approval == RevisedScheduleApprovalStatus.Approved_Without_Correction)
                {
                    score = 2;
                }
                else if (approval == RevisedScheduleApprovalStatus.Approved_After_One_Correction)
                {
                    score = 1;
                }
                else if (approval == RevisedScheduleApprovalStatus.Approved_After_Two_Correction)
                {
                    score = 0;
                }

            }
            return score;

        }
        #endregion

        public static decimal GetScoreOfAnalysisTechniqueAppliedForClaims(MonthlyProjectClaim claim)
        {
            decimal score = new decimal();
            var analysis = claim.AnalysisType;
            if (claim.IsClaimPreparedAndIssuedInTheMonth)
            {
                if (analysis == AnalysisTechniqueForMonthlyClaims.Appropriate_Delay_Analysis_Technics)
                {
                    score = 2;
                }
                else if (analysis == AnalysisTechniqueForMonthlyClaims.Delay_Analysis_Technic_Applied)
                {
                    score = 1;
                }
                else if (analysis == AnalysisTechniqueForMonthlyClaims.No_Delay_Analysis_Technic_Applied)
                {
                    score = 0;
                }
            }
            return score;
        }

        public static decimal GetScoereOfForClaimsApprovalStatus(MonthlyProjectClaim claim)
        {
            decimal score = new decimal();

            var approvalType = claim.ClaimApprovalStatus;
            if (claim.IsApprovalMadInTheMonth)
            {
                if (approvalType == MonthlyClaimRequestApprovalStatus.Above_80Percent_Approved)
                {
                    score = 3;
                }
                else if (approvalType == MonthlyClaimRequestApprovalStatus.Above_60Percent_Approved)
                {
                    score = 2;
                }
                else if (approvalType == MonthlyClaimRequestApprovalStatus.Above_50Percent_Approved)
                {
                    score = 1;
                }
            }
            return score;
        }

        //public static decimal CalculateMaterialUtilizationScore(MonthlyProjectPerformanceModifier modifier)
        //{
        //    decimal averageUtilization = (modifier.Cement + modifier.ReadyMixConcrete + modifier.ReinforcementBar +
        //                                 modifier.Sand + modifier.FormWork + modifier.Ceramic) / 6;
        //    decimal averageWastage = 100 - averageUtilization;
        //    decimal score = ((int)averageWastage / 2) * -1;
        //    return score;
        //}
        //{

        //}
    }
}
