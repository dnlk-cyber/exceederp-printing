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
    public class PerformanceEvaluationOds : List<PeriodVeiwModel>
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
                                        NumberOfEvaluationMeething = prog.NumberOfEvaluationMeething,
                                        CorrectionFactor = prog.CorrectionFactor
                                    };
                                    generalProjInfo.ExecutedToDatePercentage = progressRecord.ExecutedAmountToDatePercentageFromContract;
                                    generalProjInfo.ExecutedToDate = progressRecord.ExecutedAmountToDate;
                                    record.ProgressList.Add(progressRecord);
                                    record.GeneralInfoList.Add(generalProjInfo);

                                }

                            }
                            #endregion
                            #region Quality
                            // quality
                            var qualityList = db.MonthlyProjectQuality.Where(q => q.PeriodMappingId == periodMapping.PeriodMappingId).ToList();
                            if (qualityList.Any())
                            {
                                foreach (MonthlyProjectQuality quality in qualityList)
                                {
                                    var qualityRecord = new MonthlyQualityViewModel
                                    {
                                        MaxAllowablePoint = quality.MaxAllowablePoints,
                                        TotalPoints = quality.TotalPoints,

                                        DoPersonnelKnowHowToMeetQualityRequirements = quality.DoPersonnelKnowHowToMeetQualityRequirements,
                                        DoPersonnelKnowTheQualityRequirements = quality.DoPersonnelKnowTheQualityRequirements,
                                        MaterialTestsPerformed = quality.MaterialTestsPerformed,
                                        EvaluationOfMethod = quality.EvaluationOfMethod,
                                        OverAllEvaluationOfLabour = quality.OverAllEvaluationOfLabour,
                                        NumberOfTestsPerformedOnExecutedWorks = quality.NumberOfTestsPerformedOnExecutedWorks,
                                        ExtentOfInspectionPerformedOnExecutedWorks = quality.ExtentOfInspectionPerformedOnExecutedWorks,
                                        FromClient = quality.FromClient,
                                        FromEngineers = quality.FromEngineers,
                                        FromHeadOffice = quality.FromHeadOffice,
                                        FromQualityEngineer = quality.FromQualityEngineer,

                                        QualityPlanning = quality.QualityPlanning,
                                        QualityAssurance = quality.QualityAssurance,
                                        QualityControl = quality.QualityControl,
                                        QualityComplaint = quality.QualityComplaint,

                                    };
                                    #region previous version code 
                                    // quality planning
                                    //var qualityPlanning = db.QualityPlanning.Where(qpl => qpl.MonthlyProjectQualityId == quality.MonthlyProjectQualityId).ToList();
                                    //if (qualityPlanning.Any())
                                    //{
                                    //    foreach (QualityPlanning qPlanning in qualityPlanning)
                                    //    {
                                    //        var qualityPlanningRecord = new QualityPlanningViewModel
                                    //        {
                                    //            Description = qPlanning.Description,
                                    //            MaxPoints = qPlanning.MaxPoints,
                                    //            Points = qPlanning.Points,
                                    //        };
                                    //        qualityRecord.QPlanningList.Add(qualityPlanningRecord);
                                    //    }
                                    //}
                                    //// quality assurance
                                    //var qualityAssurance = db.QualityAssurance.Where(qa => qa.MonthlyProjectQualityId == quality.MonthlyProjectQualityId).ToList();
                                    //if (qualityAssurance.Any())
                                    //{
                                    //    foreach (QualityAssurance qAssurance in qualityAssurance)
                                    //    {
                                    //        var qAssuranceRecord = new QualityAssuranceViewModel
                                    //        {
                                    //            Description = qAssurance.Description,
                                    //            MaxPoints = qAssurance.MaxPoints,
                                    //            Points = qAssurance.Points
                                    //        };
                                    //        qualityRecord.QAssuranceList.Add(qAssuranceRecord);
                                    //    }
                                    //}
                                    //// quality control
                                    //var qualityControl = db.QualityControl.Where(qcn => qcn.MonthlyProjectQualityId == quality.MonthlyProjectQualityId).ToList();
                                    //if (qualityControl.Any())
                                    //{
                                    //    foreach (QualityControl qControl in qualityControl)
                                    //    {
                                    //        var qualityControlRecord = new QualityControlViewModel
                                    //        {
                                    //            Description = qControl.Description,
                                    //            MaxPoints = qControl.MaxPoints,
                                    //            Points = qControl.Points
                                    //        };
                                    //        qualityRecord.QControlList.Add(qualityControlRecord);
                                    //    }
                                    //}
                                    //// quality complaints
                                    //var qualityComplaints = db.MonthlyProjectQualityComplaints.Where(qcomplaint => qcomplaint.MonthlyProjectQualityId == quality.MonthlyProjectQualityId).ToList();
                                    //if (qualityComplaints.Any())
                                    //{
                                    //    foreach (MonthlyProjectQualityComplaint complaint in qualityComplaints)
                                    //    {
                                    //        var complaintRecord = new QualityComplaintsViewModel
                                    //        {
                                    //            Source = complaint.Source.ToString(),
                                    //            Complaint = (-1) * complaint.NumOfComplaints
                                    //        };
                                    //        qualityRecord.QComplaintList.Add(complaintRecord);
                                    //    }
                                    //} 
                                    #endregion
                                    record.QualityList.Add(qualityRecord);
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


                                    };
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
                                        WrittenComn = communication.WrittenComn,
                                        EmailComn = communication.EmailComn,
                                        TelephoneComn = communication.TelephoneComn,
                                        ComnLevel = communication.ComnLevel.ToString().Replace('_', ' ')
                                    };
                                    #region previous code
                                    //// comn within project
                                    //var comnWithinProjectList = db.MonthlyProjectCommunicationWithinProject.Where(cmn => cmn.MonthlyProjectCommunicationId == communication.MonthlyProjectCommunicationId).ToList();
                                    //if (comnWithinProjectList.Any())
                                    //{
                                    //    foreach (MonthlyProjectCommunicationWithinProject comnInProject in comnWithinProjectList)
                                    //    {
                                    //        var comnWithinProjRecord = new ComnWithinProjectViewModel
                                    //        {
                                    //            Description = comnInProject.Description,
                                    //            Points = comnInProject.Points
                                    //        };
                                    //        comnRecord.ComnWithinProjectList.Add(comnWithinProjRecord);
                                    //    }
                                    //}
                                    //// comn with head office
                                    //var comnWithHO = db.MonthlyProjectCommunicationWithHeadOffice.Where(cm => cm.MonthlyProjectCommunicationId == communication.MonthlyProjectCommunicationId).ToList();
                                    //if (comnWithHO.Any())
                                    //{
                                    //    foreach (MonthlyProjectCommunicationWithHeadOffice comnWithHeadOffice in comnWithHO)
                                    //    {
                                    //        var comnWithHORecord = new ComnWithHeadOfficeViewModel
                                    //        {
                                    //            CommunicationMethod = comnWithHeadOffice.CommunicationMethod.ToString(),
                                    //            Points = comnWithHeadOffice.Points
                                    //        };
                                    //        comnRecord.ComnWithHeadOfficeList.Add(comnWithHORecord);
                                    //    }
                                    //}
                                    //// conm with consultant
                                    //var comnWithConsultantList = db.MonthlyProjectCommunicationWithConsultant.Where(cmn => cmn.MonthlyProjectCommunicationId == communication.MonthlyProjectCommunicationId).ToList();
                                    //if (comnWithConsultantList.Any())
                                    //{
                                    //    foreach (MonthlyProjectCommunicationWithConsultant comnWithConsultant in comnWithConsultantList)
                                    //    {
                                    //        var comnWithConsultantRecord = new ComnWithConsultantViewModel
                                    //        {
                                    //            CommunicationLevel = comnWithConsultant.CommunicationLevel.ToString(),
                                    //            Points = comnWithConsultant.Points
                                    //        };
                                    //        comnRecord.ComnWithConsultantList.Add(comnWithConsultantRecord);
                                    //    }
                                    //} 
                                    #endregion
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

                                        SiteAccess = modifier.SiteAccess,
                                        DelayedPayment = modifier.DelayedPayment,
                                        LackOfGoodGovernance = modifier.LackOfGoodGovernance,
                                        InactionForSubContractor = modifier.InactionForSubContractor
                                    };
                                    #region existing code
                                    #region MyRegion
                                    // health and safety
                                    //var healthAndSafetyList = db.PerformanceModifierHealthAndSafety.Where(hs => hs.MonthlyProjectPerformanceModifierId == modifier.MonthlyProjectPerformanceModifierId).ToList();
                                    //if (healthAndSafetyList.Any())
                                    //{
                                    //    foreach (PerformanceModifierHealthAndSafety healthAdSafety in healthAndSafetyList)
                                    //    {
                                    //        var healthAndSafetyRecord = new ModifierHelthAndSafetyVM
                                    //        {
                                    //            InjuryLevel = healthAdSafety.InjuryLevel.ToString() + " injury --- " + healthAdSafety.InjuryCount,
                                    //            Points = healthAdSafety.Points
                                    //        };
                                    //        modifierRecord.ModifierHelthAndSafetyList.Add(healthAndSafetyRecord);
                                    //    }
                                    //}
                                    // budget req and utilization
                                    var reqList = db.PerformanceModifierBudgetRequest.Where(r => r.MonthlyProjectPerformanceModifierId == modifier.MonthlyProjectPerformanceModifierId).ToList();
                                    //var utlList = db.PerformanceModifierBudgetUtilization.Where(utl => utl.MonthlyProjectPerformanceModifierId == modifier.MonthlyProjectPerformanceModifierId).ToList();
                                    //if (reqList.Any() || utlList.Any())
                                    //{
                                    //    var reqAndUtlRecord = new ModifierBudgetRequestAndUtilizationVM
                                    //    {
                                    //        TotalPoints = reqList.Sum(x => x.Points) + utlList.Sum(u => u.Points)
                                    //    };
                                    //    foreach (PerformanceModifierBudgetRequest budgetReq in reqList)
                                    //    {
                                    //        var budgetReqRecord = new ModifierBudgetReqVM
                                    //        {
                                    //            ForecastedBudget = budgetReq.ForecastedBudget,
                                    //            RequestedBudget = budgetReq.RequestedBudget,
                                    //            Deviation = budgetReq.Deviation,
                                    //            Points = budgetReq.Points
                                    //        };
                                    //        reqAndUtlRecord.BudgetRequestList.Add(budgetReqRecord);
                                    //    }
                                    //    foreach (PerformanceModifierBudgetUtilization budgetUtilization in utlList)
                                    //    {
                                    //        var budgetUtlRecord = new ModifierBudgetUtlVM
                                    //        {
                                    //            RequestedBudget = budgetUtilization.RequestedBudget,
                                    //            UtilizedBudget = budgetUtilization.UtilizedBudget,
                                    //            Deviation = budgetUtilization.Deviation,
                                    //            Points = budgetUtilization.Points
                                    //        };
                                    //        reqAndUtlRecord.BudgetUtilizationList.Add(budgetUtlRecord);
                                    //    }
                                    //    modifierRecord.ModifierBudgetReqAndUtilList.Add(reqAndUtlRecord);
                                    //}
                                    // material utilization 
                                    #endregion
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
                                    #region MyRegion
                                    // subcontract complaints
                                    //var subContractComplainList = db.PerformanceModifierSubContractAdministration.Where(sc => sc.MonthlyProjectPerformanceModifierId == modifier.MonthlyProjectPerformanceModifierId).ToList();
                                    //if (subContractComplainList.Any())
                                    //{
                                    //    foreach (PerformanceModifierSubContractAdministration subcontract in subContractComplainList)
                                    //    {
                                    //        var subContractComplaintRecord = new ModifierSubContractAdmVM
                                    //        {
                                    //            Description = subcontract.Description + " ( " + subcontract.MaxAllowablePoints + " pts )",
                                    //            Points = subcontract.Points
                                    //        };
                                    //        modifierRecord.ModifierSubContractAdmnList.Add(subContractComplaintRecord);
                                    //    }
                                    //}  
                                    #endregion
                                    #endregion
                                    record.ModifierList.Add(modifierRecord);
                                }
                            }
                            #endregion

                            #region evalluation summary
                            var progSummary = record.ProgressList.FirstOrDefault();
                            var generalInfoSummary = record.GeneralInfoList.FirstOrDefault();
                            if (progSummary != null && generalInfoSummary != null)
                            {
                                #region add progress summary
                                var progSummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                                {
                                    EvaluationComponent = "Progress",
                                    Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PROGRESS) / 100,
                                };

                                var progMonthlySummaryRecord = new PerformanceEvaluationSubComponentsSummaryViewModel
                                {
                                    EvaluationComponent = "monthly",
                                    Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PROGRESS_MONTHLY) / 100,
                                    RowData = progSummary.ExecutedAmountPercentageFromPlan * progSummary.CorrectionFactor,
                                };
                                progMonthlySummaryRecord.Points = Math.Min(125, progMonthlySummaryRecord.RowData * 100);
                                progMonthlySummaryRecord.Result = progMonthlySummaryRecord.Points * progMonthlySummaryRecord.Weight;


                                var progToDateSummaryRecord = new PerformanceEvaluationSubComponentsSummaryViewModel
                                {
                                    EvaluationComponent = "to date",
                                    Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PROGRESS_TODATE) / 100,
                                    RowData = generalInfoSummary.TimeElapsedTodatePercentage > 0 ? generalInfoSummary.ExecutedToDatePercentage / generalInfoSummary.TimeElapsedTodatePercentage : 0,
                                };
                                progToDateSummaryRecord.Points = Math.Min(125, progToDateSummaryRecord.RowData * 100);
                                progToDateSummaryRecord.Result = progToDateSummaryRecord.Points * progToDateSummaryRecord.Weight;

                                progSummaryRecord.SubComponentList.Add(progMonthlySummaryRecord);
                                progSummaryRecord.SubComponentList.Add(progToDateSummaryRecord);
                                progSummaryRecord.Result = progMonthlySummaryRecord.Result + progToDateSummaryRecord.Result;
                                record.PEvComponentsSummaryList.Add(progSummaryRecord);

                                #endregion
                            }
                            #region add quality to summary
                            var qualitySum = record.QualityList.FirstOrDefault();
                            if (qualitySum != null)
                            {
                                var qualitySummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                                {
                                    EvaluationComponent = "Quality",
                                    Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.QUALITY) / 100,
                                    RowData = qualitySum.QualityPlanning + qualitySum.QualityAssurance + qualitySum.QualityControl + qualitySum.QualityComplaint,
                                    HasNoSub = true
                                };
                                qualitySummaryRecord.Points = qualitySummaryRecord.RowData * 5;
                                qualitySummaryRecord.Result = qualitySummaryRecord.Points * qualitySummaryRecord.Weight;
                                record.PEvComponentsSummaryList.Add(qualitySummaryRecord);
                            }
                            #endregion
                            #region add cost to summary
                            var costSumm = record.CostList.FirstOrDefault();
                            if (costSumm != null && progSummary != null)
                            {
                                var costSummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                                {
                                    EvaluationComponent = "Cost",
                                    Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.COST) / 100,
                                    RowData = this.CalculateCostSummaryRowData(progSummary, costSumm),
                                    HasNoSub = true
                                };
                                costSummaryRecord.Points = costSummaryRecord.RowData > 125 ? 125 :
                                    Math.Max(costSummaryRecord.RowData, -50) * 100;
                                costSummaryRecord.Result = costSummaryRecord.Weight * costSummaryRecord.Points;

                                record.PEvComponentsSummaryList.Add(costSummaryRecord);
                            }
                            #endregion
                            #region add payment summary
                            var paymentSumm = record.PaymentList.FirstOrDefault();
                            if (paymentSumm != null)
                            {
                                var paymentSummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                                {
                                    EvaluationComponent = "Payment",
                                    Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PAYMENT) / 100,
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
                            }
                            #endregion
                            #region add planning to summary
                            var planningSum = record.PlanningList.FirstOrDefault();
                            if (planningSum != null)
                            {
                                var planningSummaryRecord = new PerformanceEvaluationComponentsSummaryViewModel
                                {
                                    EvaluationComponent = "Planning",
                                    Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PLANNING) / 100,
                                    RowData = this.CalculatePlanningSummaryResult(planningSum.Id),
                                    HasNoSub = true,
                                };
                                planningSummaryRecord.Points = planningSummaryRecord.RowData * 10;
                                planningSummaryRecord.Result = planningSummaryRecord.Weight * planningSummaryRecord.Points;

                                record.PEvComponentsSummaryList.Add(planningSummaryRecord);
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
                            }
                            #endregion
                            #endregion

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
                                    Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_MATERIAL_UTILIZATION)/100,
                                    RowData = modifierSumm.MaterialUtilization,
                                };
                                matUtilSummaryRecord.Points = matUtilSummaryRecord.RowData * 20;
                                matUtilSummaryRecord.Result = matUtilSummaryRecord.Weight * matUtilSummaryRecord.Points;

                                var subContractSummaryRecord = new PerformanceEvaluationModifiersSummaryViewModel
                                {
                                    EvaluationComponent = "Sub contract administration",
                                    Weight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_SUBCONTRACT_ADMINISTRATION)/100,
                                    RowData = modifierSumm.SubContractAdministration,
                                };
                                subContractSummaryRecord.Points = subContractSummaryRecord.RowData * 20;
                                subContractSummaryRecord.Result = subContractSummaryRecord.Weight * subContractSummaryRecord.Points;

                                record.PEvModifiersSummaryList.Add(healthSummayRecord);
                                record.PEvModifiersSummaryList.Add(budgutSummaryRecord);
                                record.PEvModifiersSummaryList.Add(matUtilSummaryRecord);
                                record.PEvModifiersSummaryList.Add(subContractSummaryRecord);
                            }
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
    }
}
