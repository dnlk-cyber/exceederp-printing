using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Helpers.Inventory;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class RoadPerformanceEvaluationSummaryOds : List<PerformanceEvaluationSummaryVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PerformanceEvaluationSummaryVM> GetPerformanceEvaluationSummaryData(string periodId)
        {
            var list = new List<PerformanceEvaluationSummaryVM>();
            //int id = Convert.ToInt32(projectId);
            int pId = Convert.ToInt32(periodId);
            var evaluationPeriodMappings = db.PeriodMapping.Where(p => p.ConstructionPeriodId == pId && p.ProjectInformation.ProjectType == AvailableProjectTypes.Road).ToList();
            var projIds = new HashSet<int>(evaluationPeriodMappings.Select(p => p.ProjectInformationId));
            var projectsWithEvaluationInCurrentPeriod = db.ProjectInformation.Where(proj => projIds.Contains(proj.ProjectInformationId)).ToList();

            var totalAmount = projectsWithEvaluationInCurrentPeriod.Sum(p => p.ContractAmount);
            foreach (ProjectInformation project in projectsWithEvaluationInCurrentPeriod)
            {
                var mainContract = ContractMgmtHelper.GetProjectMainContract(project.ProjectInformationId);
                #region genal information
                var generalProjInfo = new PEvProjectGeneralInfoVM
                {
                    ContractPeriod = mainContract.ContractPeriod,
                    CommencementDate = mainContract.CommencementDate.ToShortDateString(),
                    ApprovedTimeExtension = mainContract.ApprovedTimeExtension,
                    TotalProjectPeriod = mainContract.TotalProjectPeriod,
                    OriginalcompletionTime = mainContract.ContractTime.ToShortDateString(),
                    RevisedCompletionDate = mainContract.RevisedCompletionDate == null ? " " : ((DateTime)mainContract.RevisedCompletionDate).ToShortDateString(),
                    TimeElapsedToDate = mainContract.TimeElapsedToDate,
                    ContractAmount = mainContract.ContractAmount,
                    Rebate = mainContract.Rebate,
                    Supplemetary = mainContract.Supplemetary,
                    Variation = mainContract.Variation,
                    TotalProjectAmount = mainContract.TotalProjectAmount,
                    NetGrand = mainContract.NetGrand,
                    ExecutedToDate = mainContract.ExecutedToDate
                };
                decimal totalProjectPeriod = generalProjInfo.TotalProjectPeriod ?? 0;
                decimal timeElapsedToDate = generalProjInfo.TimeElapsedToDate ?? 0;

                generalProjInfo.TimeElapsedTodatePercentage = totalProjectPeriod > 0 ?
                    timeElapsedToDate / totalProjectPeriod : 0;

                #endregion
                var evaluationPeriod = evaluationPeriodMappings.FirstOrDefault(pm => pm.ProjectInformationId == project.ProjectInformationId);
                if (evaluationPeriod != null)
                {
                    
                    var record = new PeriodVeiwModel
                    {
                        Name = evaluationPeriod.PeriodMappingId.ToString(),
                        //FromPeriod = currentConstructionPeriod.DateFrom.ToString("MMMM dd, yy"),
                        //ToPeriod = currentConstructionPeriod.DateTo.ToString("MMMM dd, yy"),
                    };

                    // get the progress valeus for a given period
                    #region Progress
                    var totalProjectAmount = ContractMgmtHelper.GetProjectTotalAmount(evaluationPeriod.PeriodMappingId);
                    var progresses = db.MonthlyProjectProgress.Where(pr => pr.PeriodMappingId == evaluationPeriod.PeriodMappingId).ToList();
                    if (progresses.Any())
                    {
                        foreach (MonthlyProjectProgress prog in progresses)
                        {
                            var progressRecord = new MonthlyProgressViewModel
                            {
                                PlannedAmount = prog.PlannedAmount,
                                PlannedAmountPercentage = totalProjectAmount == 0 ? 0 : prog.PlannedAmount / totalProjectAmount,

                                PlannedAmountTillPreviousMonth = prog.PlannedAmountTillPreviousMonth,
                                PlannedAmountTillPreviousMonthPercentage = totalProjectAmount == 0 ? 0 : prog.PlannedAmountTillPreviousMonth / totalProjectAmount,

                                PlannedToDate = prog.PlannedToDate,
                                PlannedToDatePercentage = totalProjectAmount == 0 ? 0 : prog.PlannedToDate / totalProjectAmount,

                                ExecutedAmount = prog.ExecutedAmount,
                                ExecutedAmountPercentageFromContract = totalProjectAmount == 0 ? 0 : prog.ExecutedAmount / totalProjectAmount,
                                ExecutedAmountPercentageFromPlan = prog.PlannedAmount == 0 ? 0 : prog.ExecutedAmount / prog.PlannedAmount,

                                ExecutedAmountTillPreviousMonth = prog.ExecutedAmountTillPreviousMonth,
                                ExecutedAmountTillPreviousMonthPercentageFromContract = totalProjectAmount == 0 ? 0 : prog.ExecutedAmountTillPreviousMonth / totalProjectAmount,
                                ExecutedAmountTillPreviousMonthPercentageFromPlan = prog.PlannedAmountTillPreviousMonth == 0 ? 0 : prog.ExecutedAmountTillPreviousMonth / prog.PlannedAmountTillPreviousMonth,

                                ExecutedAmountToDate = prog.ExecutedAmountToDate,
                                ExecutedAmountToDatePercentageFromContract = totalProjectAmount == 0 ? 0 : prog.ExecutedAmountToDate / totalProjectAmount,
                                ExecutedAmountToDatePercentageFromPlan = prog.PlannedToDate == 0 ? 0 : prog.ExecutedAmountToDate / prog.PlannedToDate,

                                ScheduleBrokenDownToWeeklyBase = prog.ScheduleBrokenDownToWeeklyBase ? "Yes" : "No",
                                AreFormenAwareOfIt = prog.AreFormenAwareOfIt ? "Yes" : "No",
                                ActionPlanIsPreparedWithAmountAndProblemsAreAddressed = prog.ActionPlanIsPreparedWithAmountAndProblemsAreAddressed ? "Yes" : "No",
                                WellDefinedTeamStructureExists = prog.WellDefinedTeamStructureExists ? "Yes" : "No",
                                NumberOfEvaluationMeething = prog.NumberOfEvaluationMeething,
                                CorrectionFactor = prog.CorrectionFactor
                            };
                            progressRecord.MonthlyExecutedVsPlanPerformance = this.CalculateMonthlyExecutedVsPlanPerformance(progressRecord.ExecutedAmountPercentageFromPlan);
                            progressRecord.TodateExecutedVsPlanPerformance = this.CalculateExecutedTodateVsPlanPerformance(progressRecord.ExecutedAmountToDatePercentageFromPlan);

                            record.ProgressList.Add(progressRecord);
                            record.GeneralInfoList.Add(generalProjInfo);

                        }

                    }
                    #endregion
                    #region Cost
                    var costList = db.MonthlyProjectCost.Where(mc => mc.PeriodMappingId == evaluationPeriod.PeriodMappingId).ToList();
                    if (costList.Any())
                    {
                        // used for percentage calculations
                        var projectAmount = ContractMgmtHelper.GetProjectContractAmount(evaluationPeriod.PeriodMappingId);
                        var plannedAmountForTheCurrentMonth = PerformanceEvnHelper.GetPlannedAmountOfMonth(evaluationPeriod.PeriodMappingId);
                        var excutedAmountInThisMonth = PerformanceEvnHelper.GetExecutedAmountOfMonth(evaluationPeriod.PeriodMappingId);
                        var excutedAmountToDate = PerformanceEvnHelper.GetExecutedAmountToDate(evaluationPeriod.PeriodMappingId);
                        foreach (MonthlyProjectCost cost in costList)
                        {
                            var costRecord = new MonthlyCostViewModel
                            {
                                PlannedDirectCost = cost.PlannedDirectCost,
                                PlannedProjectOHCost = cost.PlannedProjectOHCost,
                                PlannedProfitAndOHCost = cost.PlannedProfitAndOHCost,
                                ActualDirectCost = cost.ActualDirectCost,
                                ActualProjectOHCost = cost.ActualProjectOHCost,
                                ActualProfitAndOHCost = cost.ActualProfitAndOHCost,
                                PlannedDirectCostToDate = cost.PlannedDirectCostToDate,
                                PlannedProjectOHCostToDate = cost.PlannedProjectOHCostToDate,
                                PlannedProfitAndOHToDate = cost.PlannedProfitAndOHToDate,
                                ActualDirectCostToDate = cost.ActualDirectCostToDate,
                                ActualProfitAndOHToDate = cost.ActualProfitAndOHToDate,
                                ActualProjectOHCostToDate = cost.ActualProjectOHCostToDate,
                                CostTracking = cost.CostTracking,

                                EquipmentUtilizationEfficiency = cost.EquipmentUtilizationEfficiency,
                                EquipmentUtilizationPerformance = cost.EquipmentUtilizationEfficiency,
                                IsFuelUtilizedBasedOnStandard = cost.FuelUtilizationBasedOnStandard > 0 ? "Yes" : "No",
                                FuelUtilizationBasedOnStandardMarks = cost.FuelUtilizationBasedOnStandard,
                                IsProperAndAccurateEarnedValueAnalysisUsed = cost.ProperAndAccurateEarnedValueAnalysis > 0 ? "Yes" : "No",
                                ProperAndAccurateEarnedValueAnalysisMarks = cost.ProperAndAccurateEarnedValueAnalysis,
                                IsProductivitiyOfLaborAndEquipmentRecorded = cost.ProductivitiyOfLaborAndEquipmentMeaure > 0 ? "Yes" : "No",
                                ProductivityOfLaborAndEquipmentMarks = cost.ProductivitiyOfLaborAndEquipmentMeaure,
                                IsReusingOfConstructionMaterialsApplied = cost.ReusingOfConstructionMaterials > 0 ? "Yes" : "No",
                                ReusingOfConstructionMaterialsMark = cost.ReusingOfConstructionMaterials,
                                CostFromExpectationMarks = cost.CostsFromExpectation,

                            };
                            //costRecord.EquipmentUtilizationPerformance = this.CalculateEquipmentUtilizationPerformance(costRecord.EquipmentUtilizationEfficiency);

                            if (plannedAmountForTheCurrentMonth > 0)
                            {
                                costRecord.PlannedDirectCostPercentage = cost.PlannedDirectCost / plannedAmountForTheCurrentMonth;
                                costRecord.PlannedProjectOHCostPercentage = cost.PlannedProjectOHCost / plannedAmountForTheCurrentMonth;
                                costRecord.PlannedProfitAndOHCostPercentage = cost.PlannedProfitAndOHCost / plannedAmountForTheCurrentMonth;
                            }
                            if (excutedAmountInThisMonth > 0)
                            {
                                costRecord.ActualDirectCostPercentage = cost.ActualDirectCost / excutedAmountInThisMonth;
                                costRecord.ActualProjectOHCostPercentage = cost.ActualProjectOHCost / excutedAmountInThisMonth;
                                costRecord.ActualProfitAndOHCostPercentage = cost.ActualProfitAndOHCost / excutedAmountInThisMonth;
                            }
                            if (projectAmount > 0)
                            {
                                costRecord.PlannedDirectCostToDatePercentage = cost.PlannedDirectCostToDate / projectAmount;
                                costRecord.PlannedProjectOHCostToDatePercentage = cost.PlannedProjectOHCostToDate / projectAmount;
                                costRecord.PlannedProfitAndOHToDatePercentage = cost.PlannedProfitAndOHToDate / projectAmount;
                            }
                            if (excutedAmountToDate > 0)
                            {
                                costRecord.ActualDirectCostToDatePercentage = cost.ActualDirectCostToDate / excutedAmountToDate;
                                costRecord.ActualProjectOHCostToDatePercentage = cost.ActualProjectOHCostToDate / excutedAmountToDate;
                                costRecord.ActualProfitAndOHToDatePercentage = cost.ActualProfitAndOHToDate / excutedAmountToDate;
                            }
                            record.CostList.Add(costRecord);
                        }
                    }
                    #endregion
                    #region Payment
                    var paymentList = db.MonthlyProjectPayment.Where(mp => mp.PeriodMappingId == evaluationPeriod.PeriodMappingId).ToList();
                    if (paymentList.Any())
                    {
                        foreach (MonthlyProjectPayment payment in paymentList)
                        {
                            //var approvalTimeRecord = new PaymentApprovalTimeViewMoedl
                            //{
                            //    ApprovalTimeConcatnated = PerformanceEvnHelper.GetConcatnatedPaymentApprovalTime(payment),
                            //    Points = payment.Points
                            //};
                            var paymentRecord = new MonthlyPaymentViewModel
                            {
                                ApprovedTillPreMonth = payment.ApprovedTillPreMonth,
                                ExpectedToBeApprovedThisMonth = payment.ExpectedToBeApprovedThisMonth,
                                AmountApprovedThisMonth = payment.AmountApprovedThisMonth,
                                AmounNotApprovedThisMonth = payment.AmounNotApprovedThisMonth,
                                ApprovedToDate = payment.ApprovedToDate,
                                AmountNotApprovedToDate = payment.AmountNotApprovedToDate,
                                PriceAdjustmentTillPreMonth = payment.PriceAdjustmentTillPreMonth,
                                PriceAdjustmentThisMonth = payment.PriceAdjustmentThisMonth,
                                PriceAdjustmentApprovedToDate = payment.PriceAdjustmentApprovedToDate,
                                MatOnSiteThisMonth = payment.MatOnSiteThisMonth,
                                MatOnSiteApprovedThisMonth = payment.MatOnSiteApprovedThisMonth,
                                MatOnSiteNotAppprovedThiMonth = payment.MatOnSiteNotAppprovedThiMonth,
                                ApprovalTime = PerformanceEvnHelper.GetConcatnatedPaymentApprovalTime(payment),
                                Points = payment.Points
                            };

                            //paymentRecord.ApprovalTimeList.Add(approvalTimeRecord);
                            record.PaymentList.Add(paymentRecord);
                        }
                    }
                    #endregion
                    #region Planning
                    var planningList = db.MonthlyProjectPlanning.Where(p => p.PeriodMappingId == evaluationPeriod.PeriodMappingId).ToList();
                    if (planningList.Any())
                    {
                        foreach (MonthlyProjectPlanning planning in planningList)
                        {
                            var planningRecord = new MonthlyPlanningViewModel
                            {
                                Id = planning.MonthlyProjectPlanningId,
                                MaxAllowablePoints = planning.MaxAllowablePoints,
                                TotalPoints = planning.TotalPoints,

                                RevScheduleSubAcToManual = planning.RevScheduleSubAcToManual,
                                RevScheduleSubAcToContract = planning.RevScheduleSubAcToContract,
                                Completness = planning.Completness,
                                QualityAndAcceptablity = planning.QualityAndAcceptablity,

                                IsRevisionTimeAccManual = planning.IsRevisionTimeAccManual ? "Yes" : "No",
                                RevisedSdlSubmitedAccordinglyAccToManual = planning.RevisedSdlSubmitedAccordinglyAccToManual ? "Yes" : "No",
                                SubmssinTimeAcctoManual = PerformanceEvnHelper.GetConcatnatedDueDateValue(planning.SubmssinTimeAcctoManual, planning.DaysAfterOrBeforeDueDateAccToManual),
                                IsRevisionTimeAccContract = planning.IsRevisionTimeAccContract ? "Yes" : "No",
                                RevisedSdlSubmitedAccordinglyAccToContract = planning.RevisedSdlSubmitedAccordinglyAccToContract ? "Yes" : "No",
                                SubmssinTimeAcctoContract = PerformanceEvnHelper.GetConcatnatedDueDateValue(planning.SubmssinTimeAcctoContract, planning.DaysAfterOrBeforeDueDateAccToContract),
                                WorkSchedule = planning.WorkSchedule,
                                ResourceSchedule = planning.ResourceSchedule,
                                CashFlowSchedule = planning.CashFlowSchedule,
                                BudgetRequirementSchedule = planning.BudgetRequirementSchedule,
                                IsApprovalMadeInTheMonth = planning.IsApprovalMadeInTheMonth ? "Yes" : "No",
                                ApprovalType = planning.ApprovalType.ToString().Replace('_', ' '),

                            };

                            record.PlanningList.Add(planningRecord);
                        }
                    }
                    #endregion
                    #region Claim
                    var claimList = db.MonthlyProjectClaim.Where(mc => mc.PeriodMappingId == evaluationPeriod.PeriodMappingId).ToList();
                    if (claimList.Any())
                    {
                        foreach (MonthlyProjectClaim claim in claimList)
                        {
                            var claimRecord = new MonthlyClaimViewModel
                            {
                                Id = claim.MonthlyProjectClaimId,
                                MaxAllowablePoints = claim.MaxAllowablePoints,
                                Points = claim.Points,

                                ContempraryRecordKeeping = claim.ContempraryRecordKeeping,
                                FullFillingRequirementForClaims = claim.FullFillingRequirementForClaims,
                                AnalysisTechnique = claim.AnalysisTechnique,
                                ApprovalStatusOfClaims = claim.ApprovalStatusOfClaims,

                                DailylyRecords = claim.DailylyRecords,
                                IsClaimPreparedAndIssuedInTheMonth = claim.IsClaimPreparedAndIssuedInTheMonth ? "Yes" : "No",
                                SubstantiveRequirementRefClauses = claim.SubstantiveRequirementRefClauses,
                                ProceduralRequirementDocumentation = claim.ProceduralRequirementDocumentation,
                                ProofRequirementDocumentation = claim.ProofRequirementDocumentation,
                                WasThereissuetoClaim = claim.WasThereissuetoClaim ? "Yes" : "No",
                                AnalysisType = claim.AnalysisType.ToString().Replace('_', ' '),
                                IsApprovalMadInTheMonth = claim.IsApprovalMadInTheMonth ? "Yes" : "No",
                                ClaimApprovalStatus = claim.ClaimApprovalStatus.ToString().Replace('_', ' '),

                            };

                            record.ClaimList.Add(claimRecord);
                        }
                    }
                    #endregion
                    #region Report
                    var reportList = db.MonthlyProjectReport.Where(rpt => rpt.PeriodMappingId == evaluationPeriod.PeriodMappingId).ToList();
                    if (reportList.Any())
                    {
                        foreach (MonthlyProjectReport report in reportList)
                        {
                            var reportRecord = new MonthlyReportViewModel
                            {
                                ReportingTime = PerformanceEvnHelper.GetConcatnatedDueDateValue(report.ReportingTime, report.Days),
                                Points = report.Points,
                                Completeness = report.Completeness,
                                Narrative = report.Narrative,

                                Progress = report.Progress,
                                QualityControl = report.QualityControl,
                                HealthAndSafety = report.HealthAndSafety,
                                EarnedValueAnalysis = report.EarnedValueAnalysis,
                                MaterialUtilization = report.MaterialUtilization,
                                LabourUtilization = report.LabourUtilixation,
                                EquipmentUtilization = report.EquipmentUtulization,
                                FuelUtilization = report.FuelUtilization,
                                ProperIdentificationOfProblems = report.ProperIdentificationOfProblems
                            };
                            #region previous code
                            //// completness components
                            //var completnessList = db.MonthlyProjectReportCompletnessComponent.Where(cp => cp.MonthlyProjectReportId == report.MonthlyProjectReportId).ToList();
                            //if (completnessList.Any())
                            //{
                            //    foreach (MonthlyProjectReportCompletnessComponent component in completnessList)
                            //    {
                            //        var completnessRecord = new ReportCompletnessViewModel
                            //        {
                            //            Attribute = component.Attribute + " ( " + component.MaxAllowablePoints + " pts)",
                            //            Points = component.Points
                            //        };
                            //        reportRecord.ReportCompletnessList.Add(completnessRecord);
                            //    }
                            //}
                            //// Narratives
                            //var narrativeList = db.MonthlyProjectReportNarrative.Where(cp => cp.MonthlyProjectReportId == report.MonthlyProjectReportId).ToList();
                            //if (narrativeList.Any())
                            //{
                            //    foreach (MonthlyProjectReportNarrative narrative in narrativeList)
                            //    {
                            //        var narrativeRecord = new ReportNarrativeViewModel
                            //        {
                            //            Attribute = narrative.Attribute + " ( " + narrative.MaxAllowablePoints + " pts)",
                            //            Points = narrative.Points
                            //        };
                            //        reportRecord.ReportNarrativeList.Add(narrativeRecord);
                            //    }
                            //} 
                            #endregion
                            record.ReportList.Add(reportRecord);
                        }
                    }
                    #endregion
                    #region Communication
                    var comnList = db.MonthlyProjectCommunication.Where(cm => cm.PeriodMappingId == evaluationPeriod.PeriodMappingId).ToList();
                    if (comnList.Any())
                    {
                        foreach (MonthlyProjectCommunication communication in comnList)
                        {
                            var comnRecord = new MonthlyCommunicationViewModel
                            {
                                MaxAllowablePoints = communication.MaxAllowablePoints,
                                TotalPoints = communication.TotalPoints,

                                ComnWithinProject = communication.ComnWithinProject,
                                ComnWithHeadOffice = communication.ComnWithHeadOffice,
                                ComnWithConsultant = communication.ComnWithConsultant,
                                NumberOfComplaints = communication.NumberOfComplaints,
                                GovernancePoints = communication.GoodGovernanceComplaints,

                                CloseDiscussionWithinProject = communication.CloseDiscussionWithinProject,
                                MobilizationAndDisscusstionWthinWorkers = communication.MobilizationAndDisscusstionWthinWorkers,
                                WrittenComn = communication.WrittenComn,
                                EmailComn = communication.EmailComn,
                                TelephoneComn = communication.TelephoneComn,
                                ComnLevel = communication.ComnLevel.ToString().Replace('_', ' ')
                            };

                            record.CommuncationList.Add(comnRecord);
                        }
                    }
                    #endregion
                    #region Modifiers
                    var modifierList = db.MonthlyProjectPerformanceModifier.Where(md => md.PeriodMappingId == evaluationPeriod.PeriodMappingId).ToList();
                    if (modifierList.Any())
                    {
                        foreach (MonthlyProjectPerformanceModifier modifier in modifierList)
                        {
                            var modifierRecord = new MonthlyPerformanceModifierVM
                            {
                                HealthAndSafety = modifier.HealthAndSafety,
                                BudgetRequestAndUtilization = modifier.BudgetRequestAndUtilization,
                                MaterialUtilization = modifier.MaterialUtilization,
                                ConductQualityAssurance = modifier.ConductQualityAssurance,
                                SubContractAdministration = modifier.SubContractAdministration,
                                TotalPoints = modifier.TotalPoints,

                                HeavyInjuriesCount = modifier.HeavyInjuriesCount,
                                LightInjuriesCount = modifier.LightInjuriesCount,

                                ForcastedBudget = modifier.ForcastedBudget,
                                RequestedBudget = modifier.RequestedBudget,
                                RequestDeviation = modifier.RequestDeviation,
                                PointFromReqDeviation = modifier.PointFromReqDeviation,
                                UtilizedBudget = modifier.UtilizedBudget,
                                UtilizationDeviation = modifier.UtilizationDeviation,
                                PointFromUtlDeviation = modifier.PointFromUtlDeviation,

                                SiteAccess = modifier.SiteAccess,
                                DelayedPayment = modifier.DelayedPayment,
                                LackOfGoodGovernance = modifier.LackOfGoodGovernance,
                                InactionForSubContractor = modifier.InactionForSubContractor
                            };
                            #region existing code

                            //var reqList = db.PerformanceModifierBudgetRequest.Where(r => r.MonthlyProjectPerformanceModifierId == modifier.MonthlyProjectPerformanceModifierId).ToList();

                            var materialUtlList = db.PerformanceModifierMaterialUtilization.Where(mu => mu.MonthlyProjectPerformanceModifierId == modifier.MonthlyProjectPerformanceModifierId).ToList();
                            if (materialUtlList.Any())
                            {
                                foreach (PerformanceModifierMaterialUtilization materialUtl in materialUtlList)
                                {
                                    var matUtlRecord = new ModifierMaterialUtlVM
                                    {
                                        Material = InventoryHelper.GetItemName(materialUtl.ItemCode),
                                        UtilizedAmount = materialUtl.UtilizedAmount,
                                    };
                                    modifierRecord.ModifierMaterialUtlList.Add(matUtlRecord);
                                }
                            }

                            #endregion
                            record.ModifierList.Add(modifierRecord);
                        }
                    }
                    #endregion

                    #region evalluation summary
                    var progressToSummarize = record.ProgressList.FirstOrDefault();
                    var generalInfoSummary = record.GeneralInfoList.FirstOrDefault();
                    if (progressToSummarize != null && generalInfoSummary != null)
                    {
                        var progRecord = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ContractAmount = project.ContractAmount,
                            AttentionWeight = totalAmount > 0 ? project.ContractAmount / totalAmount : 0,
                            ComponentName = "work progress and action plan preparation " + Math.Round(ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PROGRESS, AvailableProjectTypes.Road), 0) + "%",
                            ValueOrder = 2,
                            Value = progressToSummarize.MonthlyExecutedVsPlanPerformance + progressToSummarize.TodateExecutedVsPlanPerformance
                        };
                        list.Add(progRecord);
                        //
                    }
                   
                    #region add cost to summary
                    var costToSummarize = record.CostList.FirstOrDefault();
                    if (costToSummarize != null && progressToSummarize != null)
                    {

                        var costWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.COST, AvailableProjectTypes.Road);
                        var costSumRecord = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Resource utilization and cost " + Math.Round(costWeight , 0) + "%",
                            Value = costToSummarize.EquipmentUtilizationPerformance +
                                    costToSummarize.FuelUtilizationBasedOnStandardMarks +
                                    costToSummarize.ProperAndAccurateEarnedValueAnalysisMarks +
                                    costToSummarize.ProductivityOfLaborAndEquipmentMarks +
                                    costToSummarize.CostFromExpectationMarks + 
                                    costToSummarize.ReusingOfConstructionMaterialsMark,
                            ValueOrder = 4
                        };
                        if (costSumRecord.Value > costWeight)
                            costSumRecord.Value = costWeight;
                        list.Add(costSumRecord);
                    }
                    #endregion
                    #region add payment summary
                    var paymentSumm = record.PaymentList.FirstOrDefault();
                    if (paymentSumm != null)
                    {
                        var paymentSummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                        {
                            EvaluationComponent = "Payment",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PAYMENT, AvailableProjectTypes.Road) / 100,
                            HasNoSub = false,
                        };

                        var paymentAccRecord = new PerformanceEvaluationSubComponentsSummaryViewModel
                        {
                            EvaluationComponent = "accomplishement",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PAYMENT_ACCOMPLISHMENT) / 100,
                            RowData = paymentSumm.ExpectedToBeApprovedThisMonth <= 0 ? 1 : paymentSumm.AmountApprovedThisMonth / paymentSumm.ExpectedToBeApprovedThisMonth,

                        };
                        paymentAccRecord.Points = 100 * paymentAccRecord.RowData;
                        paymentAccRecord.Result = paymentAccRecord.Points * paymentAccRecord.Weight;
                        // add accomplishement to the list
                        paymentSummaryRecord.SubComponentList.Add(paymentAccRecord);
                        var priceAdjustment = paymentSumm.PriceAdjustmentThisMonth ?? 0;
                        var paymentPriceAdjustmentRec = new PerformanceEvaluationSubComponentsSummaryViewModel
                        {
                            EvaluationComponent = "price adjustment",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PAYMENT_PRICE_ADJUSTMENT) / 100,
                            RowData = priceAdjustment == 0 ? 0 : 100,

                        };
                        paymentPriceAdjustmentRec.Points = paymentPriceAdjustmentRec.RowData;
                        paymentPriceAdjustmentRec.Result = paymentPriceAdjustmentRec.Points * paymentPriceAdjustmentRec.Weight;
                        //add price adjustment summary to the list
                        paymentSummaryRecord.SubComponentList.Add(paymentPriceAdjustmentRec);

                        var approvedMatOnSiteThisMonth = paymentSumm.MatOnSiteApprovedThisMonth ?? 0;
                        var matOnSiteThisMonth = paymentSumm.MatOnSiteThisMonth ?? 0;
                        var paymentMatOnSiteRecord = new PerformanceEvaluationSubComponentsSummaryViewModel
                        {
                            EvaluationComponent = "material on site",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PAYMENT_MATERIAL_ON_SITE) / 100,
                            RowData = matOnSiteThisMonth <= 0 ? 100 : (approvedMatOnSiteThisMonth / ((decimal)0.5 * matOnSiteThisMonth)) * 100,
                        };
                        paymentMatOnSiteRecord.Points = paymentMatOnSiteRecord.RowData;
                        paymentMatOnSiteRecord.Result = paymentMatOnSiteRecord.Weight * paymentMatOnSiteRecord.Points;
                        //add mat on site summary to the lsit
                        paymentSummaryRecord.SubComponentList.Add(paymentMatOnSiteRecord);

                        var paymentApprovalTimeRec = new PerformanceEvaluationSubComponentsSummaryViewModel
                        {
                            EvaluationComponent = "approval time",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PAYMENT_APPROVAL_TIME) / 100,
                            RowData = paymentSumm.Points
                        };
                        if (paymentApprovalTimeRec.RowData == 0)
                        {
                            paymentApprovalTimeRec.Points = 100;
                        }
                        else if (paymentApprovalTimeRec.RowData > 0)
                        {
                            paymentApprovalTimeRec.Points = 100 + paymentSumm.Points * 20;

                        }
                        else
                        {
                            paymentApprovalTimeRec.Points = 20 * paymentApprovalTimeRec.RowData;
                        }
                        paymentApprovalTimeRec.Result = paymentApprovalTimeRec.Weight * paymentApprovalTimeRec.Points;
                        // add payment appral time to the list
                        paymentSummaryRecord.SubComponentList.Add(paymentApprovalTimeRec);
                        paymentSummaryRecord.Result = paymentApprovalTimeRec.Result +
                                                      paymentPriceAdjustmentRec.Result +
                                                      paymentMatOnSiteRecord.Result +
                                                      paymentAccRecord.Result;
                        record.PEvComponentsSummaryList.Add(paymentSummaryRecord);

                        var paymntSumRecord = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Payment and related issues " + Math.Round(paymentSummaryRecord.Weight * 100, 0) + "%",
                            Value = paymentSummaryRecord.Result,
                            ValueOrder = 5
                        };
                        list.Add(paymntSumRecord);
                    }

                    #endregion
                    #region add planning to summary
                    var planningSum = record.PlanningList.FirstOrDefault();
                    if (planningSum != null)
                    {
                        var planningSummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                        {
                            EvaluationComponent = "Planning",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PLANNING, AvailableProjectTypes.Road) / 100,
                            RowData = this.CalculatePlanningSummaryResult(planningSum.Id),
                            HasNoSub = true,
                        };
                        planningSummaryRecord.Points = planningSummaryRecord.RowData * 10;
                        planningSummaryRecord.Result = planningSummaryRecord.Weight * planningSummaryRecord.Points;

                        record.PEvComponentsSummaryList.Add(planningSummaryRecord);

                        var planningSummaryOfSummaryRecord = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Planning" + Math.Round(planningSummaryRecord.Weight * 100, 0) + "%",
                            Value = planningSummaryRecord.Result,
                            ValueOrder = 6
                        };
                        list.Add(planningSummaryOfSummaryRecord);
                    }
                    #endregion
                    #region add claim to summary
                    var claimSumm = record.ClaimList.FirstOrDefault();
                    if (claimSumm != null)
                    {
                        var claimSummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                        {
                            EvaluationComponent = "Claim",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.CLAIM) / 100,
                            RowData = this.CalculateClaimSummaryResult(claimSumm.Id),
                            HasNoSub = true,
                        };
                        claimSummaryRecord.Points = claimSummaryRecord.RowData * 10;
                        claimSummaryRecord.Result = claimSummaryRecord.Weight * claimSummaryRecord.Points;

                        record.PEvComponentsSummaryList.Add(claimSummaryRecord);

                        var claimSummaryOfSummaryRecord = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Claim Organizing & Requesting on time and risk management " + Math.Round(claimSummaryRecord.Weight * 100, 0) + "%",
                            Value = claimSummaryRecord.Result,
                            ValueOrder = 7
                        };
                        list.Add(claimSummaryOfSummaryRecord);
                    }
                    #endregion
                    #region add report to summary
                    var reportSumm = record.ReportList.FirstOrDefault();
                    if (reportSumm != null)
                    {
                        var reportSummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                        {
                            EvaluationComponent = "Report",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.REPORT) / 100,
                            RowData = reportSumm.Points + reportSumm.Completeness + reportSumm.Narrative,
                            HasNoSub = true
                        };
                        reportSummaryRecord.Points = reportSummaryRecord.RowData * 20;
                        reportSummaryRecord.Result = reportSummaryRecord.Weight * reportSummaryRecord.Points;

                        record.PEvComponentsSummaryList.Add(reportSummaryRecord);

                        var reportSummaryOfSummaryRecord = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Monthly and semi monthly Report Preparation" + Math.Round(reportSummaryRecord.Weight * 100, 0) + "%",
                            Value = reportSummaryRecord.Result,
                            ValueOrder = 8
                        };
                        list.Add(reportSummaryOfSummaryRecord);
                    }
                    #endregion
                    #region add communiction to summary
                    var commnSumm = record.CommuncationList.FirstOrDefault();
                    if (commnSumm != null)
                    {
                        var comnSummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                        {
                            EvaluationComponent = "Communication",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.COMMUNICATION) / 100,
                            RowData = commnSumm.ComnWithinProject + commnSumm.GovernancePoints + commnSumm.ComnWithHeadOffice + commnSumm.ComnWithConsultant,
                            HasNoSub = true,
                        };
                        comnSummaryRecord.Points = comnSummaryRecord.RowData * 20;
                        comnSummaryRecord.Result = comnSummaryRecord.Weight * comnSummaryRecord.Points;

                        record.PEvComponentsSummaryList.Add(comnSummaryRecord);

                        var communicationSummaryOfSummaryRecord = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Communication " + Math.Round(comnSummaryRecord.Weight * 100, 0) + "%",
                            Value = comnSummaryRecord.Result,
                            ValueOrder = 9
                        };
                        list.Add(communicationSummaryOfSummaryRecord);
                    }
                    #endregion
                    #endregion

                    #region modifiers summary
                    var modifierSumm = record.ModifierList.FirstOrDefault();
                    if (modifierSumm != null)
                    {
                        var healthSummayRecord = new PerformanceEvaluationModifiersSummaryViewModel
                        {
                            EvaluationComponent = "health and safety",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_HEALTH_AND_SAFETY) / 100,
                            RowData = modifierSumm.HealthAndSafety,
                        };
                        healthSummayRecord.Points = healthSummayRecord.RowData * 20;
                        healthSummayRecord.Result = healthSummayRecord.Weight * healthSummayRecord.Points;

                        var budgutSummaryRecord = new PerformanceEvaluationModifiersSummaryViewModel
                        {
                            EvaluationComponent = "budget utilization",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_BUDGET_UTILIZATION) / 100,
                            RowData = modifierSumm.BudgetRequestAndUtilization,
                        };
                        budgutSummaryRecord.Points = budgutSummaryRecord.RowData * 20;
                        budgutSummaryRecord.Result = budgutSummaryRecord.Weight * budgutSummaryRecord.Points;

                        var matUtilSummaryRecord = new PerformanceEvaluationModifiersSummaryViewModel
                        {
                            EvaluationComponent = "material utilization",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_MATERIAL_UTILIZATION) / 100,
                            RowData = modifierSumm.MaterialUtilization,
                        };
                        matUtilSummaryRecord.Points = matUtilSummaryRecord.RowData * 20;
                        matUtilSummaryRecord.Result = matUtilSummaryRecord.Weight * matUtilSummaryRecord.Points;

                        var subContractSummaryRecord = new PerformanceEvaluationModifiersSummaryViewModel
                        {
                            EvaluationComponent = "Sub contract administration",
                            Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_SUBCONTRACT_ADMINISTRATION) / 100,
                            RowData = modifierSumm.SubContractAdministration,
                        };
                        subContractSummaryRecord.Points = subContractSummaryRecord.RowData * 20;
                        subContractSummaryRecord.Result = subContractSummaryRecord.Weight * subContractSummaryRecord.Points;

                        record.PEvModifiersSummaryList.Add(healthSummayRecord);
                        record.PEvModifiersSummaryList.Add(budgutSummaryRecord);
                        record.PEvModifiersSummaryList.Add(matUtilSummaryRecord);
                        record.PEvModifiersSummaryList.Add(subContractSummaryRecord);

                        #region summary of summary
                        var healthSummaryOfSummaryRecord = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Health & Safety " + Math.Round(healthSummayRecord.Weight * 100, 0) + "%",
                            Value = healthSummayRecord.Result,
                            ValueOrder = 10
                        };
                        list.Add(healthSummaryOfSummaryRecord);
                        var budgetUtlSummaryOfSummaryRecord = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Budget Utilization " + Math.Round(budgutSummaryRecord.Weight * 100, 0) + "%",
                            Value = budgutSummaryRecord.Result,
                            ValueOrder = 11
                        };
                        list.Add(budgetUtlSummaryOfSummaryRecord);
                        var materialUtlSummaryOfSummary = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "PRP implementation " + Math.Round(matUtilSummaryRecord.Weight * 100, 0) + "%",
                            Value = matUtilSummaryRecord.Result,
                            ValueOrder = 12
                        };
                        list.Add(materialUtlSummaryOfSummary);
                        var subContractSummaryOfSummary = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Sub Contract Administration " + Math.Round(subContractSummaryRecord.Weight * 100, 0) + "%",
                            Value = subContractSummaryRecord.Result,
                            ValueOrder = 13
                        };
                        list.Add(subContractSummaryOfSummary);
                        var conductQualityAssuranceSummaryofSummary = new PerformanceEvaluationSummaryVM
                        {
                            ProjectName = project.Name,
                            ComponentName = "Conduct Quality Assurance ",
                            Value = modifierSumm.ConductQualityAssurance,
                            ValueOrder = 14
                        };
                        list.Add(conductQualityAssuranceSummaryofSummary);
                        #endregion
                    }
                    #endregion
                }
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public EvaluationSummaryHeaderVM GettHeaderData(string periodId)
        {
            int id = Convert.ToInt32(periodId);
            var item = db.ConstructionPeriod.Find(id);
            EvaluationSummaryHeaderVM headerDs = new EvaluationSummaryHeaderVM();
            var organization = db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.EvaluationPeriod = item.Name;

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return headerDs;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PerformanceEvaluationPeriodViewModel> GetEValuationPeriods()
        {
            List<PerformanceEvaluationPeriodViewModel> list = new List<PerformanceEvaluationPeriodViewModel>();
            var evaluationPeriods = db.PeriodMapping.Where(m => m.ProjectInformation.ProjectType == AvailableProjectTypes.Road).ToList();
            var ids = new HashSet<int>(evaluationPeriods.Select(p => p.ConstructionPeriodId));
            var constructionPeriods = db.ConstructionPeriod.Where(p => ids.Contains(p.ConstructionPeriodId)).ToList();
            if (constructionPeriods.Any())
            {
                foreach (ConstructionPeriod period in constructionPeriods)
                {
                    var periodRecord = new PerformanceEvaluationPeriodViewModel
                    {
                        Id = period.ConstructionPeriodId,
                        Name = period.Name
                    };
                    list.Add(periodRecord);
                }
            }
            return list;
        }

        private decimal CalculateCostSummaryRowData(MonthlyProgressViewModel progress, MonthlyCostViewModel cost)
        {
            var result = new decimal();
            if (progress.ExecutedAmount < cost.ActualProjectOHCost)
            {
                result = 0;
            }
            else if (cost.CostTracking == 0)
            {
                var planned = cost.PlannedProfitAndOHCostPercentage;
                var actual = cost.ActualProfitAndOHCostPercentage;
                if (planned != 0)
                {
                    result = 1 + (actual - planned) / planned;
                }
            }
            else
            {
                var planned = cost.PlannedProfitAndOHCostPercentage;
                var actual = cost.ActualProfitAndOHCostPercentage;
                var tracking = cost.CostTracking;
                result = (1 + (actual - planned) / planned) * ((20 + tracking) / 20);
            }

            return result;
        }
        private decimal CalculatePlanningSummaryResult(int id)
        {
            var result = new decimal();
            var planning = db.MonthlyProjectPlanning.Find(id);
            if (planning != null)
            {
                if ((planning.IsRevisionTimeAccManual && !planning.RevisedSdlSubmitedAccordinglyAccToManual) ||
                planning.IsRevisionTimeAccContract && !planning.RevisedSdlSubmitedAccordinglyAccToContract)
                {
                    result = 0;
                }
                else if ((planning.IsRevisionTimeAccManual && planning.IsRevisionTimeAccContract) && planning.IsApprovalMadeInTheMonth)
                {
                    result = planning.RevScheduleSubAcToManual + planning.RevScheduleSubAcToContract +
                        planning.Completness + planning.QualityAndAcceptablity;
                }
                else if ((planning.IsRevisionTimeAccManual || planning.IsRevisionTimeAccContract) && !planning.IsApprovalMadeInTheMonth)
                {
                    result = (planning.RevScheduleSubAcToManual + planning.RevScheduleSubAcToContract +
                        planning.Completness) * 10 / 8;
                }
                else if ((!planning.IsRevisionTimeAccManual || !planning.IsRevisionTimeAccContract) && !planning.IsApprovalMadeInTheMonth)
                {
                    result = (planning.RevScheduleSubAcToManual + planning.RevScheduleSubAcToContract +
                        planning.Completness + planning.QualityAndAcceptablity) * 10 / 5;
                }
                else
                {
                    result = planning.QualityAndAcceptablity * 5;
                }
            }
            return result;
        }
        private decimal CalculateClaimSummaryResult(int id)
        {
            var result = new decimal();
            var claim = db.MonthlyProjectClaim.Find(id);
            if (claim != null)
            {
                if (!claim.IsClaimPreparedAndIssuedInTheMonth && claim.WasThereissuetoClaim)
                {
                    result = 0;
                }
                else if (claim.IsClaimPreparedAndIssuedInTheMonth && claim.IsApprovalMadInTheMonth)
                {
                    result = claim.ContempraryRecordKeeping +
                             claim.FullFillingRequirementForClaims +
                             claim.AnalysisTechnique +
                             claim.ApprovalStatusOfClaims;
                }
                else if (claim.IsClaimPreparedAndIssuedInTheMonth && !claim.IsApprovalMadInTheMonth)
                {
                    result = (claim.ContempraryRecordKeeping +
                             claim.FullFillingRequirementForClaims +
                             claim.AnalysisTechnique) * 10 / 7;
                }
                else if (!claim.IsClaimPreparedAndIssuedInTheMonth && claim.IsApprovalMadInTheMonth)
                {
                    result = (claim.ContempraryRecordKeeping +
                             claim.ApprovalStatusOfClaims) * 10 / 5;
                }
                else
                {
                    result = claim.ContempraryRecordKeeping * 5;
                }

            }
            return result;
        }
        private decimal CalculateMonthlyExecutedVsPlanPerformance(decimal percentageOfExecutedThisMonthVsPlan)
        {
            var mark = new decimal();
            var r = percentageOfExecutedThisMonthVsPlan * 100;
            if (r >= 90)
            {
                mark = 6;
            }
            else if (r < 90 && r >= 80)
            {
                mark = 4;
            }
            else if (r < 80 && r >= 70)
            {
                mark = 3;
            }
            else if (r < 70 && r >= 60)
            {
                mark = 1;
            }
            else { mark = 0; }
            return mark;
        }
        private decimal CalculateExecutedTodateVsPlanPerformance(decimal percentageOfExecutedTodateVsPlan)
        {
            var mark = new decimal();
            var r = percentageOfExecutedTodateVsPlan * 100;
            if (r >= 90)
            {
                mark = 7;
            }
            else if (r < 90 && r >= 80)
            {
                mark = 5;
            }
            else if (r < 80 && r >= 70)
            {
                mark = 3;
            }
            else if (r < 70 && r >= 60)
            {
                mark = 1;
            }
            else { mark = 0; }
            return mark;
        }
        private decimal CalculateEquipmentUtilizationPerformance(decimal utilizationEfficiency)
        {
            var result = new decimal();
            if (utilizationEfficiency >= 75)
            {
                result = 4;
            }
            else if (utilizationEfficiency < 75 && utilizationEfficiency >= 65)
            {
                result = 3;
            }
            else if (utilizationEfficiency < 65 && utilizationEfficiency >= 50)
            {
                result = 2;
            }
            else
            {
                result = 0;
            }
            return result;
        }
    }
}
