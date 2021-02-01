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
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class RoadProjectsPerformanceEvaluationOds : List<PeriodVeiwModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PeriodVeiwModel> GetPerformanceEvaluationData(string projectId, string periodId)
        {
            var list = new List<PeriodVeiwModel>();
            int id = Convert.ToInt32(projectId);
            int pId = Convert.ToInt32(periodId);
            var project = db.ProjectInformation.FirstOrDefault(p => p.ProjectInformationId == id);
            var evaluationPeriod = db.PeriodMapping.FirstOrDefault(p => p.PeriodMappingId == pId);

            if (project != null)
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
                var periodsUnderProject = db.PeriodMapping.Where(pm => pm.ProjectInformationId == id).ToList();
                if (evaluationPeriod != null)
                {
                    periodsUnderProject = new List<PeriodMapping> { evaluationPeriod };
                }
                if (periodsUnderProject.Any())
                {
                    foreach (PeriodMapping periodMapping in periodsUnderProject)
                    {
                        var currentConstructionPeriod = db.ConstructionPeriod.Find(periodMapping.ConstructionPeriodId);
                        var record = new PeriodVeiwModel();
                        if (currentConstructionPeriod != null)
                        {
                            record = new PeriodVeiwModel
                            {
                                Name = currentConstructionPeriod.Name,
                                FromPeriod = currentConstructionPeriod.DateFrom.ToString("MMMM dd, yy"),
                                ToPeriod = currentConstructionPeriod.DateTo.ToString("MMMM dd, yy"),
                            };

                            // get the progress valeus for a given period
                            #region Progress
                            var totalProjectAmount = ContractMgmtHelper.GetProjectTotalAmount(periodMapping.PeriodMappingId);
                            var progresses = db.MonthlyProjectProgress.Where(pr => pr.PeriodMappingId == periodMapping.PeriodMappingId).ToList();
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
                                        ActionPlanIsPreparedWithAmountAndProblemsAreAddressed = prog.ActionPlanIsPreparedWithAmountAndProblemsAreAddressed? "Yes" : "No",
                                        WellDefinedTeamStructureExists = prog.WellDefinedTeamStructureExists ? "Yes" : "No",
                                        NumberOfEvaluationMeething = prog.NumberOfEvaluationMeething,
                                        CorrectionFactor = prog.CorrectionFactor
                                    };
                                    progressRecord.MonthlyExecutedVsPlanPerformance = this.CalculateMonthlyExecutedVsPlanPerformance(progressRecord.ExecutedAmountPercentageFromPlan);
                                    progressRecord.TodateExecutedVsPlanPerformance = this.CalculateExecutedTodateVsPlanPerformance(progressRecord.ExecutedAmountToDatePercentageFromPlan);

                                    generalProjInfo.ExecutedToDatePercentage = progressRecord.ExecutedAmountToDatePercentageFromContract;
                                    generalProjInfo.ExecutedToDate = progressRecord.ExecutedAmountToDate;
                                    record.ProgressList.Add(progressRecord);
                                    record.GeneralInfoList.Add(generalProjInfo);

                                }

                            }
                            #endregion
                            #region Cost
                            var costList = db.MonthlyProjectCost.Where(mc => mc.PeriodMappingId == periodMapping.PeriodMappingId).ToList();
                            if (costList.Any())
                            {
                                // used for percentage calculations
                                var projectAmount = ContractMgmtHelper.GetProjectContractAmount(periodMapping.PeriodMappingId);
                                var plannedAmountForTheCurrentMonth = PerformanceEvnHelper.GetPlannedAmountOfMonth(periodMapping.PeriodMappingId);
                                var excutedAmountInThisMonth = PerformanceEvnHelper.GetExecutedAmountOfMonth(periodMapping.PeriodMappingId);
                                var excutedAmountToDate = PerformanceEvnHelper.GetExecutedAmountToDate(periodMapping.PeriodMappingId);
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
                                        IsFuelUtilizedBasedOnStandard = cost.FuelUtilizationBasedOnStandard> 0 ? "Yes" : "No",
                                        FuelUtilizationBasedOnStandardMarks = cost.FuelUtilizationBasedOnStandard,
                                        IsProperAndAccurateEarnedValueAnalysisUsed = cost.ProperAndAccurateEarnedValueAnalysis > 0? "Yes" : "No",
                                        ProperAndAccurateEarnedValueAnalysisMarks = cost.ProperAndAccurateEarnedValueAnalysis,
                                        IsProductivitiyOfLaborAndEquipmentRecorded = cost.ProductivitiyOfLaborAndEquipmentMeaure > 0 ? "Yes" : "No",
                                        ProductivityOfLaborAndEquipmentMarks = cost.ProductivitiyOfLaborAndEquipmentMeaure,
                                        IsReusingOfConstructionMaterialsApplied = cost.ReusingOfConstructionMaterials >0 ? "Yes" : "No",
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
                            var paymentList = db.MonthlyProjectPayment.Where(mp => mp.PeriodMappingId == periodMapping.PeriodMappingId).ToList();
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
                            var planningList = db.MonthlyProjectPlanning.Where(p => p.PeriodMappingId == periodMapping.PeriodMappingId).ToList();
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
                                    #region previous code
                                    // revised schedule submission according to operational manula
                                    var rScheduleAcToManual = db.ScheduleSubmissionAcToOperationManual.Where(rs => rs.MonthlyProjectPlanningId == planning.MonthlyProjectPlanningId).ToList();
                                    if (rScheduleAcToManual.Any())
                                    {
                                        foreach (RScheduleSubmissionAcToOperationManual scheduleSubmission in rScheduleAcToManual)
                                        {
                                            var rAcToManualRecord = new RevisedScheduleAcToManualViewModel
                                            {
                                                IsRevisionTime = scheduleSubmission.IsRevisionTime ? "Yes" : "No",
                                                SubmissionTime = PerformanceEvnHelper.GetConcatnatedDueDateValue(scheduleSubmission.SubmissionTime, scheduleSubmission.Days),
                                                Points = scheduleSubmission.Points
                                            };
                                            planningRecord.RvAcToManualList.Add(rAcToManualRecord);
                                        }
                                    }
                                    // revised schedule according to contract
                                    var rScheduleActoContractList = db.RScheduleSubmissionAcToContract.Where(rs => rs.MonthlyProjectPlanningId == planning.MonthlyProjectPlanningId).ToList();
                                    if (rScheduleActoContractList.Any())
                                    {
                                        foreach (RScheduleSubmissionAcToContract rScheduleSubmission in rScheduleActoContractList)
                                        {
                                            var rScAcToContractRecord = new RevisedScheduleAcToContractViewMode
                                            {
                                                IsRevisionTime = rScheduleSubmission.IsRevisionTime ? "Yes" : "No",
                                                SubmissionTime = PerformanceEvnHelper.GetConcatnatedDueDateValue(rScheduleSubmission.SubmissionTime, rScheduleSubmission.Days),
                                                Points = rScheduleSubmission.Points

                                            };
                                            planningRecord.RvAcToContractList.Add(rScAcToContractRecord);
                                        }
                                    }
                                    // completness
                                    var completnessComponenetList = db.ProjectPnanningCompleteness.Where(pln => pln.MonthlyProjectPlanningId == planning.MonthlyProjectPlanningId).ToList();
                                    if (completnessComponenetList.Any())
                                    {
                                        foreach (ProjectPnanningCompleteness completnessComponenet in completnessComponenetList)
                                        {
                                            var comletnessRecord = new PlanningCompletnessViewModel
                                            {
                                                ScheduleType = completnessComponenet.ScheduleType.ToString().Replace('_', ' '),
                                                MaxAllowablePoints = completnessComponenet.MaxAllowablePoints,
                                                Points = completnessComponenet.Points
                                            };
                                            planningRecord.CompletnessList.Add(comletnessRecord);
                                        }
                                    }
                                    // quality and acceptability
                                    var acceptablityList = db.ProjectPlanningQualityAndAcceptance.Where(qa => qa.MonthlyProjectPlanningId == planning.MonthlyProjectPlanningId).ToList();
                                    if (acceptablityList.Any())
                                    {
                                        foreach (ProjectPlanningQualityAndAcceptance qAndAcceptance in acceptablityList)
                                        {
                                            var acceptanceRecord = new PlanningQualityAndAcceptablityViewModel
                                            {
                                                ApprovalType = qAndAcceptance.ApprovalType.ToString().Replace('_', ' '),
                                                IsApprovalMadeInTheMonth = qAndAcceptance.IsApprovalMadeInTheMonth ? "Yes" : "No",
                                                points = qAndAcceptance.points
                                            };
                                            planningRecord.QualityAndAcceptabilityList.Add(acceptanceRecord);
                                        }
                                    }
                                    #endregion
                                    record.PlanningList.Add(planningRecord);
                                }
                            }
                            #endregion
                            #region Claim
                            var claimList = db.MonthlyProjectClaim.Where(mc => mc.PeriodMappingId == periodMapping.PeriodMappingId).ToList();
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
                                    #region previous code
                                    // contemprary record keeping
                                    //var contempraryRecordList = db.ContempraryRecord.Where(cr => cr.MonthlyProjectClaimId == claim.MonthlyProjectClaimId).ToList();
                                    //if (contempraryRecordList.Any())
                                    //{
                                    //    foreach (ContempraryRecord contemprary in contempraryRecordList)
                                    //    {
                                    //        var contempraryRecordToInsert = new ClaimContempraryRecordViewModel
                                    //        {
                                    //            Description = contemprary.Description + " ( " + contemprary.MaxAllowablePoints + "pts) ",
                                    //            Points = contemprary.Points
                                    //        };
                                    //        claimRecord.ClaimContempraryRecordList.Add(contempraryRecordToInsert);
                                    //    }
                                    //}
                                    //// claim requirements
                                    //var requirementList = db.RequirementForMonthlyClaims.Where(rq => rq.MonthlyProjectClaimId == claim.MonthlyProjectClaimId).ToList();
                                    //if (requirementList.Any())
                                    //{
                                    //    foreach (RequirementForMonthlyClaims requirement in requirementList)
                                    //    {
                                    //        var requirementRecord = new ClaimRequirementViewModel
                                    //        {
                                    //            IsClaimPreparedAndIssuedInThisMonth = claim.IsClaimPreparedAndIssuedInThisMonth ? "Yes" : "No",
                                    //            Description = requirement.Description,
                                    //            Points = requirement.Points
                                    //        };
                                    //        claimRecord.ClaimRequirementList.Add(requirementRecord);
                                    //    }
                                    //}
                                    //// claim analysis techniques
                                    //var analysisTechniqueList = db.AnalysisTechniqueApplicationForMonthlyClaims.Where(ana => ana.MonthlyProjectClaimId == claim.MonthlyProjectClaimId).ToList();
                                    //if (analysisTechniqueList.Any())
                                    //{
                                    //    foreach (AnalysisTechniqueApplicationForMonthlyClaims analysis in analysisTechniqueList)
                                    //    {
                                    //        var analysisRecord = new ClaimAnalysisTechniqueViewModel
                                    //        {
                                    //            WasThereIssueToClaim = analysis.WasThereIssueToClaim ? "Yes" : "No",
                                    //            AnalysisTechnique = analysis.AnalysisTechnique.ToString().Replace('_', ' '),
                                    //            Points = analysis.Points
                                    //        };
                                    //        claimRecord.ClaimAnalysisTechniqueList.Add(analysisRecord);
                                    //    }
                                    //}
                                    //// claim approval status
                                    //var approvalList = db.ApprovalStatusForClaimsResponded.Where(aps => aps.MonthlyProjectClaimId == claim.MonthlyProjectClaimId).ToList();
                                    //if (approvalList.Any())
                                    //{
                                    //    foreach (ApprovalStatusForClaimsResponded approval in approvalList)
                                    //    {
                                    //        var approvalRecord = new ClaimApprovalStatusViewModel
                                    //        {
                                    //            IsApprovalMadeInTheMonth = approval.IsApprovalMadeInTheMonth ? "Yes" : "No",
                                    //            ApprovalStatus = approval.ApprovalStatus.ToString().Replace('_', ' '),
                                    //            Points = approval.Points
                                    //        };
                                    //        claimRecord.ClaimApprovalStatusList.Add(approvalRecord);
                                    //    }
                                    //} 
                                    #endregion
                                    record.ClaimList.Add(claimRecord);
                                }
                            }
                            #endregion
                            #region Report
                            var reportList = db.MonthlyProjectReport.Where(rpt => rpt.PeriodMappingId == periodMapping.PeriodMappingId).ToList();
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
                            var comnList = db.MonthlyProjectCommunication.Where(cm => cm.PeriodMappingId == periodMapping.PeriodMappingId).ToList();
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
                                        CommunicationAndSupportBetweenProjects = communication.CommunicationAndSupportBetweenProjects,
                                        WrittenComn = communication.WrittenComn,
                                        EmailComn = communication.EmailComn,
                                        TelephoneComn = communication.TelephoneComn,
                                        ResponseFromHeadOffice = communication.ResponseFromHeadOffice,
                                        ComnLevel = communication.ComnLevel.ToString().Replace('_', ' ')
                                    };
                                    
                                    record.CommuncationList.Add(comnRecord);
                                }
                            }
                            #endregion
                            #region Modifiers
                            var modifierList = db.MonthlyProjectPerformanceModifier.Where(md => md.PeriodMappingId == periodMapping.PeriodMappingId).ToList();
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
                                        SumOfPRPAndQualityAssurance = modifier.ConductQualityAssurance + modifier.MaterialUtilization,
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

                                        //Cement = modifier.Cement,
                                        //ReadyMixConcrete = modifier.ReadyMixConcrete,
                                        //ReinforcementBar = modifier.ReinforcementBar,
                                        //FormWork = modifier.FormWork,
                                        //Sand = modifier.Sand,
                                        //Ceramic = modifier.Ceramic,

                                        PRPImplementationLevel = modifier.PRPImplementationLevel.ToString(),
                                        ReworkHappened = modifier.ReworkHappened,
                                        ConsultantComplaint = modifier.ConsultantComplaint,
                                        DefectOccured = modifier.DefectOccured,

                                        SiteAccess = modifier.SiteAccess,
                                        DelayedPayment = modifier.DelayedPayment,
                                        LackOfGoodGovernance = modifier.LackOfGoodGovernance,
                                        InactionForSubContractor = modifier.InactionForSubContractor
                                    };
                                    record.ModifierList.Add(modifierRecord);
                                }
                            }
                            #endregion
                        }
                        list.Add(record);
                    }
                }
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectHeaderVM GetProjectHeader(string projectId)
        {
            int id = Convert.ToInt32(projectId);
            var item = db.ProjectInformation.Find(id);
            ProjectHeaderVM headerDs = new ProjectHeaderVM();
            var organization = db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;

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
        public List<PerformanceEvaluationPeriodViewModel> GetEValuationPeriods(string projectId)
        {
            List<PerformanceEvaluationPeriodViewModel> list = new List<PerformanceEvaluationPeriodViewModel>();
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.FirstOrDefault(p => p.ProjectInformationId == id);

            if (project != null)
            {
                var evaluationPeriods = db.PeriodMapping.Where(p => p.ProjectInformationId == id).ToList();
                if (evaluationPeriods.Any())
                {
                    foreach (PeriodMapping period in evaluationPeriods)
                    {

                        var record = new PerformanceEvaluationPeriodViewModel
                        {
                            Id = period.PeriodMappingId,
                            Name = period.ConstructionPeriod.Name
                        };
                        list.Add(record);
                    }
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
            if(r >= 90)
            {
                mark = 6;
            }
            else if(r < 90 && r >= 80)
            {
                mark = 4;
            }
            else if(r < 80 && r >= 70)
            {
                mark = 3;
            }
            else if(r < 70 && r >= 60)
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
            if(utilizationEfficiency >= 75)
            {
                result = 4;
            }
            else if(utilizationEfficiency < 75 && utilizationEfficiency >= 65)
            {
                result = 3;
            }
            else if(utilizationEfficiency <65 && utilizationEfficiency >= 50)
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
