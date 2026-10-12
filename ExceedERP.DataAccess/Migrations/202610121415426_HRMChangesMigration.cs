namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HRMChangesMigration : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "EXCEED.ApplicantScreenResult", newName: "ApplicantScreenResults");
            RenameTable(name: "EXCEED.ClearanceProviderDetail", newName: "ClearanceProviderDetails");
            RenameTable(name: "EXCEED.ClearanceRequestApprover", newName: "ClearanceRequestApprovers");
            RenameTable(name: "EXCEED.ClearanceRequest", newName: "ClearanceRequests");
            RenameTable(name: "EXCEED.CriteriaCategory", newName: "CriteriaCategories");
            RenameTable(name: "EXCEED.EmployeeProbationSetting", newName: "EmployeeProbationSettings");
            RenameTable(name: "EXCEED.EmployeeTransferRequest", newName: "EmployeeTransferRequests");
            RenameTable(name: "EXCEED.TransferRequestDepartmentValidation", newName: "TransferRequestDepartmentValidations");
            RenameTable(name: "EXCEED.TransferRequestDetail", newName: "TransferRequestDetails");
            RenameTable(name: "EXCEED.TransferRequestValidation", newName: "TransferRequestValidations");
            RenameTable(name: "EXCEED.ExperienceJobTitle", newName: "ExperienceJobTitles");
            RenameTable(name: "EXCEED.InternalApplicantExamResult", newName: "InternalApplicantExamResults");
            RenameTable(name: "EXCEED.InternalApplicantScreeningResult", newName: "InternalApplicantScreeningResults");
            RenameTable(name: "EXCEED.InternalApplicantScreening", newName: "InternalApplicantScreenings");
            RenameTable(name: "EXCEED.InternalCustomer", newName: "InternalCustomers");
            RenameTable(name: "EXCEED.InternalCustomerPerformanceEvaluation", newName: "InternalCustomerPerformanceEvaluations");
            RenameTable(name: "EXCEED.InternalCustomerEmployeeMeasurement", newName: "InternalCustomerEmployeeMeasurements");
            RenameTable(name: "EXCEED.InternalCustomerEmployeePerformance", newName: "InternalCustomerEmployeePerformances");
            RenameTable(name: "EXCEED.InternalCustomerEmployeePerformanceScore", newName: "InternalCustomerEmployeePerformanceScores");
            RenameTable(name: "EXCEED.LeaveRequestAttachement", newName: "LeaveRequestAttachements");
            RenameTable(name: "EXCEED.MainCompanyEvalutionSetting", newName: "MainCompanyEvalutionSettings");
            RenameTable(name: "EXCEED.NeedAssessmentSummaryDetail", newName: "NeedAssessmentSummaryDetails");
            RenameTable(name: "HRM.OtherBenefit", newName: "OtherBenefits");
            RenameTable(name: "EXCEED.PeerEmployeeMeasurement", newName: "PeerEmployeeMeasurements");
            RenameTable(name: "EXCEED.PeerToPeerEmployeePerformance", newName: "PeerToPeerEmployeePerformances");
            RenameTable(name: "EXCEED.PeerToPeerEmployeePerformanceScore", newName: "PeerToPeerEmployeePerformanceScores");
            RenameTable(name: "EXCEED.PeerToPeerPerformanceEvaluation", newName: "PeerToPeerPerformanceEvaluations");
            RenameTable(name: "EXCEED.PerformanceRequestPeriod", newName: "PerformanceRequestPeriods");
            RenameTable(name: "EXCEED.PerformanceSetting", newName: "PerformanceSettings");
            RenameTable(name: "EXCEED.ScreeningSetting", newName: "ScreeningSettings");
            RenameTable(name: "EXCEED.TrainingEvaluationAfter", newName: "TrainingEvaluationAfters");
            RenameTable(name: "EXCEED.TrainingNeedRequest", newName: "TrainingNeedRequests");
            RenameTable(name: "EXCEED.TrainingEvaluationBefore", newName: "TrainingEvaluationBefores");
            MoveTable(name: "Payroll.Addition", newSchema: "EXCEED");
            MoveTable(name: "Payroll.Advance", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.ApplicantScreenResults", newSchema: "HRM");
            MoveTable(name: "Payroll.Association", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.ClearanceProviderDetails", newSchema: "HRM");
            MoveTable(name: "EXCEED.ClearanceRequestApprovers", newSchema: "HRM");
            MoveTable(name: "EXCEED.ClearanceRequests", newSchema: "HRM");
            MoveTable(name: "EXCEED.ClearanceRequestStatus", newSchema: "HRM");
            MoveTable(name: "EXCEED.CriteriaCategories", newSchema: "HRM");
            MoveTable(name: "EXCEED.DatePreferenceSetting", newSchema: "Performance");
            MoveTable(name: "EXCEED.EmployeeProbationSettings", newSchema: "HRM");
            MoveTable(name: "EXCEED.EmployeeTransferRequests", newSchema: "HRM");
            MoveTable(name: "EXCEED.TransferRequestDepartmentValidations", newSchema: "HRM");
            MoveTable(name: "EXCEED.TransferRequestDetails", newSchema: "HRM");
            MoveTable(name: "EXCEED.TransferRequestValidations", newSchema: "HRM");
            MoveTable(name: "EXCEED.ExperienceJobTitles", newSchema: "HRM");
            MoveTable(name: "Payroll.FixedContribution", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.InternalApplicantExamResults", newSchema: "HRM");
            MoveTable(name: "EXCEED.InternalApplicants", newSchema: "HRM");
            MoveTable(name: "EXCEED.InternalApplicantScreeningResults", newSchema: "HRM");
            MoveTable(name: "EXCEED.InternalApplicantScreenings", newSchema: "HRM");
            MoveTable(name: "EXCEED.InternalCustomerDetail", newSchema: "Performance");
            MoveTable(name: "EXCEED.InternalCustomers", newSchema: "Performance");
            MoveTable(name: "EXCEED.InternalCustomerPerformanceEvaluations", newSchema: "Performance");
            MoveTable(name: "EXCEED.InternalCustomerEmployeeMeasurements", newSchema: "Performance");
            MoveTable(name: "EXCEED.InternalCustomerEmployeePerformances", newSchema: "Performance");
            MoveTable(name: "EXCEED.InternalCustomerEmployeePerformanceScores", newSchema: "Performance");
            MoveTable(name: "EXCEED.LeaveRequestAttachements", newSchema: "HRM");
            MoveTable(name: "HRM.LeaveTransfer", newSchema: "EXCEED");
            MoveTable(name: "Payroll.Loan", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.MainCompanyEvalutionSettings", newSchema: "Performance");
            MoveTable(name: "EXCEED.NeedAssessmentSummary", newSchema: "HRM");
            MoveTable(name: "EXCEED.NeedAssessmentSummaryDetails", newSchema: "HRM");
            MoveTable(name: "Payroll.OverTimePerDay", newSchema: "EXCEED");
            MoveTable(name: "Payroll.OverTime", newSchema: "EXCEED");
            MoveTable(name: "Payroll.OverTimeTransfer", newSchema: "EXCEED");
            MoveTable(name: "Payroll.OverTimeTrasferDetail", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.PeerEmployeeMeasurements", newSchema: "Performance");
            MoveTable(name: "EXCEED.PeerToPeerEmployeePerformances", newSchema: "Performance");
            MoveTable(name: "EXCEED.PeerToPeerEmployeePerformanceScores", newSchema: "Performance");
            MoveTable(name: "EXCEED.PeerToPeerPerformanceEvaluations", newSchema: "Performance");
            MoveTable(name: "EXCEED.PerformanceRequestPeriods", newSchema: "Performance");
            MoveTable(name: "EXCEED.PerformanceSettings", newSchema: "Performance");
            MoveTable(name: "Payroll.RegularContribution", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.ScreeningSettings", newSchema: "HRM");
            MoveTable(name: "EXCEED.TrainingEvaluationAfters", newSchema: "HRM");
            MoveTable(name: "EXCEED.TrainingNeedRequests", newSchema: "HRM");
            MoveTable(name: "EXCEED.TrainingEvaluationBefores", newSchema: "HRM");
            CreateTable(
                "HRM.ActingAllowances",
                c => new
                    {
                        ActingAllowanceId = c.Int(nullable: false, identity: true),
                        AllowanceDefinationId = c.Int(nullable: false),
                        Remark = c.String(),
                        ActingId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ActingAllowanceId)
                .ForeignKey("HRM.Actings", t => t.ActingId, cascadeDelete: true)
                .ForeignKey("HRM.AllowanceDefination", t => t.AllowanceDefinationId, cascadeDelete: true)
                .Index(t => t.AllowanceDefinationId)
                .Index(t => t.ActingId);
            
            CreateTable(
                "HRM.Actings",
                c => new
                    {
                        ActingId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        FromDateTime = c.DateTime(),
                        FromDateTimeAm = c.String(nullable: false),
                        ToDateTime = c.DateTime(),
                        ToDateTimeAm = c.String(),
                        Remark = c.String(),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsOpen = c.Boolean(nullable: false),
                        ActingAllowanceId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ActingId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.AllowanceGenerationSettings",
                c => new
                    {
                        AllowanceGenerationSettingId = c.Int(nullable: false, identity: true),
                        AllowGenerationForNewEmployee = c.Boolean(nullable: false),
                        AllowRegenerationForUpdatedEmployee = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AllowanceGenerationSettingId);
            
            CreateTable(
                "HRM.AllowanceSettings",
                c => new
                    {
                        AllowanceSettingId = c.Int(nullable: false, identity: true),
                        AllowanceType = c.Int(nullable: false),
                        IsOnlyForHead = c.Boolean(nullable: false),
                        LookupId = c.Int(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AllowanceSettingId);
            
            CreateTable(
                "HRM.AnnouncmentAttachments",
                c => new
                    {
                        AnnouncmentAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        AnnouncmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnnouncmentAttachmentId)
                .ForeignKey("HRM.Announcment", t => t.AnnouncmentId, cascadeDelete: true)
                .Index(t => t.AnnouncmentId);
            
            CreateTable(
                "HRM.VacancyLanguages",
                c => new
                    {
                        VacancyLanguageId = c.Int(nullable: false, identity: true),
                        LanguageName = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        ProficiencyLevel = c.String(maxLength: 50),
                        IsRequired = c.Boolean(nullable: false),
                        VacancyId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VacancyLanguageId)
                .ForeignKey("HRM.Vacancy", t => t.VacancyId, cascadeDelete: true)
                .Index(t => t.VacancyId);
            
            CreateTable(
                "HRM.ApplicantScreeningResultAttachments",
                c => new
                    {
                        ApplicantScreeningResultAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        IsInternal = c.Boolean(nullable: false),
                        InternalApplicantScreeningResultId = c.Int(),
                        ExternalApplicantScreeningResultId = c.Int(),
                    })
                .PrimaryKey(t => t.ApplicantScreeningResultAttachmentId);
            
            CreateTable(
                "HRM.AttendanceBio",
                c => new
                    {
                        AttendanceBioId = c.Int(nullable: false, identity: true),
                        EmpCode = c.String(),
                        Date = c.DateTime(),
                        PunchTime1 = c.DateTime(),
                        PunchTime2 = c.DateTime(),
                        PunchTime3 = c.DateTime(),
                        PunchTime4 = c.DateTime(),
                        PunchTime5 = c.DateTime(),
                        PunchTime6 = c.DateTime(),
                    })
                .PrimaryKey(t => t.AttendanceBioId);
            
            CreateTable(
                "HRM.AttendanceExceptions",
                c => new
                    {
                        AttendanceExceptionId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateAm = c.String(),
                        ToDate = c.DateTime(nullable: false),
                        ToDateAm = c.String(),
                        Description = c.String(),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AttendanceExceptionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.CompanyApprovalSettings",
                c => new
                    {
                        CompanyApprovalSettingId = c.Int(nullable: false, identity: true),
                        Module = c.Int(nullable: false),
                        Role = c.Int(nullable: false),
                        StepOrder = c.Int(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        AllowAutoPlacement = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CompanyApprovalSettingId);
            
            CreateTable(
                "HRM.CompanyLeaveWorkflowSettings",
                c => new
                    {
                        CompanyLeaveWorkflowSettingId = c.Int(nullable: false, identity: true),
                        UseProjectManagerApproval = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CompanyLeaveWorkflowSettingId);
            
            CreateTable(
                "HRM.ComplainAttachement",
                c => new
                    {
                        ComplainAttachementId = c.Int(nullable: false, identity: true),
                        ComplainId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ComplainAttachementId)
                .ForeignKey("HRM.Complain", t => t.ComplainId, cascadeDelete: true)
                .Index(t => t.ComplainId);
            
            CreateTable(
                "HRM.DailyAttendanceSettings",
                c => new
                    {
                        DailyAttendanceSettingId = c.Int(nullable: false, identity: true),
                        IsDailyAttendanceAndApprovalAllowed = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DailyAttendanceSettingId);
            
            CreateTable(
                "HRM.DelegationBenefitSettings",
                c => new
                    {
                        DelegationBenefitSettingId = c.Int(nullable: false, identity: true),
                        FromDays = c.Int(nullable: false),
                        ToDays = c.Int(nullable: false),
                        PayableSalaryInPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DelegationBenefitSettingId);
            
            CreateTable(
                "HRM.DelegationStepIncrementSettings",
                c => new
                    {
                        DelegationStepIncrementSettingId = c.Int(nullable: false, identity: true),
                        EnableAutoStepIncrement = c.Boolean(nullable: false),
                        InitialIncrementPeriodMonths = c.Int(nullable: false),
                        SubsequentIncrementPeriodMonths = c.Int(nullable: false),
                        MaxStepIncrements = c.Int(nullable: false),
                        ApplyOnlyIfSalaryGreaterOrEqual = c.Boolean(nullable: false),
                        EffectiveFromDate = c.DateTime(nullable: false),
                        EffectiveFromDateAm = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DelegationStepIncrementSettingId);
            
            CreateTable(
                "HRM.EducationFieldDetails",
                c => new
                    {
                        EducationFieldDetailId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                        ExperienceJobTitleId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EducationFieldDetailId)
                .ForeignKey("HRM.ExperienceJobTitles", t => t.ExperienceJobTitleId, cascadeDelete: true)
                .Index(t => t.ExperienceJobTitleId);
            
            CreateTable(
                "HRM.EducationLevelDetail",
                c => new
                    {
                        EducationLevelDetailId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                        Experience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExperienceJobTitleId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EducationLevelDetailId)
                .ForeignKey("HRM.ExperienceJobTitles", t => t.ExperienceJobTitleId, cascadeDelete: true)
                .Index(t => t.ExperienceJobTitleId);
            
            CreateTable(
                "HRM.EducationRequestAttachement",
                c => new
                    {
                        EducationRequestAttachementId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        EducationRequestId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EducationRequestAttachementId)
                .ForeignKey("HRM.EducationRequest", t => t.EducationRequestId, cascadeDelete: true)
                .Index(t => t.EducationRequestId);
            
            CreateTable(
                "HRM.EmployeeAttendanceSummary",
                c => new
                    {
                        EmployeeAttendanceSummaryId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        OrgStructureId = c.Int(),
                        BranchId = c.Int(),
                        EmployeeCode = c.String(),
                        Date = c.DateTime(nullable: false),
                        CheckInTime = c.DateTime(),
                        CheckOutTime = c.DateTime(),
                        Leave = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Absent = c.Decimal(nullable: false, precision: 8, scale: 6),
                        LateComing = c.Decimal(nullable: false, precision: 8, scale: 6),
                        TotalHrs = c.Decimal(precision: 18, scale: 2),
                        TotalDays = c.Decimal(precision: 18, scale: 2),
                        OvertimeHours = c.Decimal(nullable: false, precision: 8, scale: 6),
                        OvertimeDays = c.Decimal(nullable: false, precision: 8, scale: 6),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeAttendanceSummaryId);
            
            CreateTable(
                "HRM.EmployeeBenefitDetails",
                c => new
                    {
                        EmployeeBenefitDetailId = c.Int(nullable: false, identity: true),
                        BenefitTypeId = c.Int(nullable: false),
                        UnitofMeasurement = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizedQuantity = c.Decimal(precision: 18, scale: 2),
                        IsUtilized = c.Boolean(nullable: false),
                        Remark = c.String(maxLength: 500),
                        EmployeeBenefitId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeBenefitDetailId)
                .ForeignKey("HRM.EmployeeBenefits", t => t.EmployeeBenefitId, cascadeDelete: true)
                .ForeignKey("Common.Lookup", t => t.BenefitTypeId, cascadeDelete: true)
                .Index(t => t.BenefitTypeId)
                .Index(t => t.EmployeeBenefitId);
            
            CreateTable(
                "HRM.EmployeeBenefits",
                c => new
                    {
                        EmployeeBenefitId = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        EffectiveFrom = c.DateTime(nullable: false),
                        EffectiveTo = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeBenefitId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "HRM.EmployeeCards",
                c => new
                    {
                        EmployeeCardId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        ToEmployeeId = c.Int(),
                        OrgStructureId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        FromDate = c.DateTime(),
                        FromDateAm = c.String(nullable: false),
                        ToDate = c.DateTime(),
                        ToDateTimeAm = c.String(),
                        Remark = c.String(),
                        Salary = c.Decimal(precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        EachPrimaryId = c.Int(nullable: false),
                        CardType = c.Int(nullable: false),
                        ActionTaken = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeCardId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.EmployeeExternalFundings",
                c => new
                    {
                        EmployeeExternalFundingId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        FundingSource = c.String(nullable: false),
                        ProjectName = c.String(),
                        OutCome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LevelOfEffortPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeExternalFundingId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.EmployeeRewards",
                c => new
                    {
                        EmployeeRewardId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        SubBranch = c.String(),
                        Date = c.DateTime(nullable: false),
                        DateAm = c.String(),
                        RewardType = c.Int(nullable: false),
                        ReasonForReward = c.String(),
                        AwardedBy = c.String(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        Unit = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeRewardId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.EmployeeSalaryIncrementSettings",
                c => new
                    {
                        EmployeeSalaryIncrementSettingId = c.Int(nullable: false, identity: true),
                        ResponsibilityLevel = c.Int(nullable: false),
                        EligibilityServiceMonths = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeSalaryIncrementSettingId);
            
            CreateTable(
                "HRM.EmployeeTransferAttachments",
                c => new
                    {
                        EmployeeTransferAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        EmployeeTransferRequestId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeTransferAttachmentId)
                .ForeignKey("HRM.EmployeeTransferRequests", t => t.EmployeeTransferRequestId, cascadeDelete: true)
                .Index(t => t.EmployeeTransferRequestId);
            
            CreateTable(
                "HRM.TransferRequestDGMValidations",
                c => new
                    {
                        TransferRequestDGMValidationId = c.Int(nullable: false, identity: true),
                        EmployeeTransferRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TransferRequestDGMValidationId)
                .ForeignKey("HRM.EmployeeTransferRequests", t => t.EmployeeTransferRequestId, cascadeDelete: true)
                .Index(t => t.EmployeeTransferRequestId);
            
            CreateTable(
                "HRM.TransferRequestGMValidations",
                c => new
                    {
                        TransferRequestGMValidationId = c.Int(nullable: false, identity: true),
                        EmployeeTransferRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TransferRequestGMValidationId)
                .ForeignKey("HRM.EmployeeTransferRequests", t => t.EmployeeTransferRequestId, cascadeDelete: true)
                .Index(t => t.EmployeeTransferRequestId);
            
            CreateTable(
                "HRM.ExperienceJobTitleDetails",
                c => new
                    {
                        ExperienceJobTitleDetailId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        ExamType = c.Int(nullable: false),
                        Remark = c.String(),
                        ExperienceJobTitleId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExperienceJobTitleDetailId)
                .ForeignKey("HRM.ExperienceJobTitles", t => t.ExperienceJobTitleId, cascadeDelete: true)
                .Index(t => t.ExperienceJobTitleId);
            
            CreateTable(
                "HRM.ExternalSalaries",
                c => new
                    {
                        ExternalSalaryId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 5),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalSalaryId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.GuarantorOrSureties",
                c => new
                    {
                        GuarantorOrSuretyId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Remark = c.String(),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
                        ForwardedTime = c.DateTime(),
                        Status = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GuarantorOrSuretyId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.GuarantorOrSuretyAttachments",
                c => new
                    {
                        GuarantorOrSuretyAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        GuarantorOrSuretyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GuarantorOrSuretyAttachmentId)
                .ForeignKey("HRM.GuarantorOrSureties", t => t.GuarantorOrSuretyId, cascadeDelete: true)
                .Index(t => t.GuarantorOrSuretyId);
            
            CreateTable(
                "HRM.GuarantorOrSuretyDetails",
                c => new
                    {
                        GuarantorOrSuretyDetailId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Organiztion = c.String(nullable: false),
                        FromDate = c.DateTime(nullable: false),
                        FromDateAm = c.String(),
                        ToDate = c.DateTime(),
                        ToDateAm = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        GuarantorOrSuretyId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GuarantorOrSuretyDetailId)
                .ForeignKey("HRM.GuarantorOrSureties", t => t.GuarantorOrSuretyId, cascadeDelete: true)
                .Index(t => t.GuarantorOrSuretyId);
            
            CreateTable(
                "EXCEED.HolidayGiftEmployee",
                c => new
                    {
                        HolidayGiftEmployeeId = c.Int(nullable: false, identity: true),
                        PaymentRequestId = c.Int(),
                        EmployeeCode = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        EthiopianHoliday = c.Int(nullable: false),
                        OrgStructureId = c.Int(),
                        BranchId = c.Int(),
                        LookupId = c.Int(),
                        BusinessUnitId = c.Int(),
                        PreparedBy = c.String(),
                        PreparedTime = c.DateTime(),
                        PreparedById = c.Int(),
                        CheckedBy = c.String(),
                        CheckedTime = c.DateTime(),
                        CheckedById = c.Int(),
                        BasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HolidayGiftAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        HolidayGiftRateSettingId = c.Int(nullable: false),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
                        ForwardedTime = c.DateTime(),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.HolidayGiftEmployeeId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EXCEED.HolidayGiftRateSetting", t => t.HolidayGiftRateSettingId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.HolidayGiftRateSettingId);
            
            CreateTable(
                "EXCEED.HolidayGiftRateSetting",
                c => new
                    {
                        HolidayGiftRateSettingId = c.Int(nullable: false, identity: true),
                        MinGrossSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxGrossSalary = c.Decimal(precision: 18, scale: 2),
                        GiftAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.HolidayGiftRateSettingId);
            
            CreateTable(
                "HRM.InternalRecruitmentCriteriaTypeSettings",
                c => new
                    {
                        InternalRecruitmentCriteriaTypeSettingId = c.Int(nullable: false, identity: true),
                        ExamSource = c.Int(nullable: false),
                        ExamType = c.Int(nullable: false),
                        Squence = c.Int(nullable: false),
                        IsPercent = c.Boolean(nullable: false),
                        SettingVlaue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InternalRecruitmentCriteriaTypeSettingId);
            
            CreateTable(
                "HRM.InternalVacancyEligibilitySettings",
                c => new
                    {
                        InternalVacancyEligibilitySettingId = c.Int(nullable: false, identity: true),
                        EnforceMinimumServicePeriod = c.Boolean(nullable: false),
                        MinimumYearsOfService = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AllowContractEmployees = c.Boolean(nullable: false),
                        AllowStandbyTechnicians = c.Boolean(nullable: false),
                        AllowHROverride = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InternalVacancyEligibilitySettingId);
            
            CreateTable(
                "HRM.JobTitleAndRankMappings",
                c => new
                    {
                        JobTitleAndRankMappingId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        RankId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobTitleAndRankMappingId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.Rank", t => t.RankId, cascadeDelete: true)
                .Index(t => t.LookupId)
                .Index(t => t.RankId);
            
            CreateTable(
                "HRM.LeavePaidDeductionDetails",
                c => new
                    {
                        LeavePaidDeductionDetailId = c.Int(nullable: false, identity: true),
                        LeavePaidId = c.Int(nullable: false),
                        LeaveEntitleId = c.Int(),
                        LeaveEntitlementHistoryId = c.Int(),
                        SourceType = c.String(),
                        DeductedDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeavePaidDeductionDetailId)
                .ForeignKey("HRM.LeavePaid", t => t.LeavePaidId, cascadeDelete: true)
                .Index(t => t.LeavePaidId);
            
            CreateTable(
                "HRM.LeaveRequestHRManagerValidations",
                c => new
                    {
                        LeaveRequestHRManagerValidationId = c.Int(nullable: false, identity: true),
                        LeaveRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeaveRequestHRManagerValidationId)
                .ForeignKey("HRM.LeaveRequest", t => t.LeaveRequestId, cascadeDelete: true)
                .Index(t => t.LeaveRequestId);
            
            CreateTable(
                "HRM.LeaveRequestPMValidations",
                c => new
                    {
                        LeaveRequestPMValidationId = c.Int(nullable: false, identity: true),
                        LeaveRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeaveRequestPMValidationId)
                .ForeignKey("HRM.LeaveRequest", t => t.LeaveRequestId, cascadeDelete: true)
                .Index(t => t.LeaveRequestId);
            
            CreateTable(
                "HRM.LegacyEmployees",
                c => new
                    {
                        LegacyEmployeeId = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(),
                        FullName = c.String(),
                        Sex = c.String(),
                        Age = c.Int(),
                        Dept = c.String(),
                        Title = c.String(),
                        Grade = c.String(),
                        PresentSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Education = c.String(),
                        FieldOfStudy = c.String(),
                        WithInExperience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OutExperience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalExperience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LeaveBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RecruitmentDate = c.DateTime(nullable: false),
                        DateofTermination = c.DateTime(),
                        ReasonforTermination = c.String(),
                        Branch = c.String(),
                        SubBranch = c.String(),
                        IsProcessed = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LegacyEmployeeId);
            
            CreateTable(
                "HRM.LoanRequestAttachments",
                c => new
                    {
                        LoanRequestAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        LoanRequestId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LoanRequestAttachmentId)
                .ForeignKey("HRM.LoanRequest", t => t.LoanRequestId, cascadeDelete: true)
                .Index(t => t.LoanRequestId);
            
            CreateTable(
                "HRM.LoanRequest",
                c => new
                    {
                        LoanRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        RequestDateAm = c.String(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsForOther = c.Boolean(nullable: false),
                        IsFinalReviewCompleted = c.Boolean(nullable: false),
                        FinalReviewedBy = c.String(),
                        FinalReviewedOn = c.DateTime(),
                        IsSentToPayment = c.Boolean(nullable: false),
                        PaymentRequestId = c.Int(),
                        NextApproverEmployeeId = c.Int(),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LoanRequestId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.GLPeriodId);
            
            CreateTable(
                "HRM.LoanRequestDepartmentValidations",
                c => new
                    {
                        LoanRequestDepartmentValidationId = c.Int(nullable: false, identity: true),
                        LoanRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LoanRequestDepartmentValidationId)
                .ForeignKey("HRM.LoanRequest", t => t.LoanRequestId, cascadeDelete: true)
                .Index(t => t.LoanRequestId);
            
            CreateTable(
                "HRM.LoanRequestDetails",
                c => new
                    {
                        LoanRequestDetailId = c.Int(nullable: false, identity: true),
                        LoanRequestId = c.Int(nullable: false),
                        LoanType = c.Int(nullable: false),
                        Descrption = c.String(),
                        LoanAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoanRepaymentMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MonthlyDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LoanRequestDetailId)
                .ForeignKey("HRM.LoanRequest", t => t.LoanRequestId, cascadeDelete: true)
                .Index(t => t.LoanRequestId);
            
            CreateTable(
                "HRM.LoanRequestValidations",
                c => new
                    {
                        LoanRequestValidationId = c.Int(nullable: false, identity: true),
                        LoanRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LoanRequestValidationId)
                .ForeignKey("HRM.LoanRequest", t => t.LoanRequestId, cascadeDelete: true)
                .Index(t => t.LoanRequestId);
            
            CreateTable(
                "HRM.NeedAssessmentAttachments",
                c => new
                    {
                        NeedAssessmentAttachmentId = c.Int(nullable: false, identity: true),
                        NeedAssessmentId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        ContentType = c.String(),
                        FileSize = c.Long(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        NeedAssessment_AssesmentId = c.Int(),
                    })
                .PrimaryKey(t => t.NeedAssessmentAttachmentId)
                .ForeignKey("HRM.NeedAssessment", t => t.NeedAssessment_AssesmentId)
                .Index(t => t.NeedAssessment_AssesmentId);
            
            CreateTable(
                "HRM.PerformanceAllowedScores",
                c => new
                    {
                        PerformanceAllowedScoreId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        ScoreValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Order = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceAllowedScoreId);
            
            CreateTable(
                "HRM.PerformanceEvaluationSettings",
                c => new
                    {
                        PerformanceEvaluationSettingId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TotalWeight = c.Decimal(precision: 18, scale: 2),
                        MaxScorePerMeasurement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnforceTotalWeightValidation = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceEvaluationSettingId);
            
            CreateTable(
                "HRM.PlacementWorkflowSettings",
                c => new
                    {
                        PlacementWorkflowSettingId = c.Int(nullable: false, identity: true),
                        RequireChecker = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PlacementWorkflowSettingId);
            
            CreateTable(
                "HRM.ProbationPerformanceValidations",
                c => new
                    {
                        ProbationPerformanceValidationId = c.Int(nullable: false, identity: true),
                        ProbationPerformanceId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProbationPerformanceValidationId)
                .ForeignKey("HRM.ProbationPerformance", t => t.ProbationPerformanceId, cascadeDelete: true)
                .Index(t => t.ProbationPerformanceId);
            
            CreateTable(
                "HRM.ProbationsSettings",
                c => new
                    {
                        ProbationsSettingId = c.Int(nullable: false, identity: true),
                        ResponsibilityLevel = c.Int(),
                        ProbationDays = c.Int(nullable: false),
                        MinimumFitValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProbationsSettingId);
            
            CreateTable(
                "EXCEED.PromotionRequest",
                c => new
                    {
                        PromotionRequestId = c.Int(nullable: false, identity: true),
                        PlanType = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        NoOfPostion = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        EduLevel = c.Int(nullable: false),
                        RelevantExp = c.String(),
                        Experience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SpecialSkill = c.String(),
                        PlannedSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RankId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PromotionRequestId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .ForeignKey("HRM.Rank", t => t.RankId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.LookupId)
                .Index(t => t.RankId);
            
            CreateTable(
                "HRM.PromotionYearSettings",
                c => new
                    {
                        PromotionYearSettingId = c.Int(nullable: false, identity: true),
                        NumberOfPromotionYears = c.Int(nullable: false),
                        StepAmountsToPromote = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PromotionYearSettingId);
            
            CreateTable(
                "HRM.ProvidedTrainings",
                c => new
                    {
                        ProvidedTrainingId = c.Int(nullable: false, identity: true),
                        TrainingName = c.String(nullable: false),
                        Description = c.String(),
                        Duration = c.Int(),
                        DurationUnit = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        MainCompanyId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProvidedTrainingId)
                .ForeignKey("HRM.MainCompany", t => t.MainCompanyId, cascadeDelete: true)
                .Index(t => t.MainCompanyId);
            
            CreateTable(
                "HRM.RecruitmentRequestDepartmentValidation",
                c => new
                    {
                        RecruitmentRequestDepartmentValidationId = c.Int(nullable: false, identity: true),
                        RecruitmentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentRequestDepartmentValidationId)
                .ForeignKey("HRM.RecruitmentRequest", t => t.RecruitmentRequestId, cascadeDelete: true)
                .Index(t => t.RecruitmentRequestId);
            
            CreateTable(
                "HRM.RecruitmentRequestDGMValidation",
                c => new
                    {
                        RecruitmentRequestDGMValidationId = c.Int(nullable: false, identity: true),
                        RecruitmentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentRequestDGMValidationId)
                .ForeignKey("HRM.RecruitmentRequest", t => t.RecruitmentRequestId, cascadeDelete: true)
                .Index(t => t.RecruitmentRequestId);
            
            CreateTable(
                "HRM.RecruitmentRequestGMValidation",
                c => new
                    {
                        RecruitmentRequestGMValidationId = c.Int(nullable: false, identity: true),
                        RecruitmentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentRequestGMValidationId)
                .ForeignKey("HRM.RecruitmentRequest", t => t.RecruitmentRequestId, cascadeDelete: true)
                .Index(t => t.RecruitmentRequestId);
            
            CreateTable(
                "HRM.RecruitmentRequestHRValidation",
                c => new
                    {
                        RecruitmentRequestHRValidationId = c.Int(nullable: false, identity: true),
                        RecruitmentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentRequestHRValidationId)
                .ForeignKey("HRM.RecruitmentRequest", t => t.RecruitmentRequestId, cascadeDelete: true)
                .Index(t => t.RecruitmentRequestId);
            
            CreateTable(
                "HRM.RecruitmentRequestPMValidation",
                c => new
                    {
                        RecruitmentRequestPMValidationId = c.Int(nullable: false, identity: true),
                        RecruitmentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentRequestPMValidationId)
                .ForeignKey("HRM.RecruitmentRequest", t => t.RecruitmentRequestId, cascadeDelete: true)
                .Index(t => t.RecruitmentRequestId);
            
            CreateTable(
                "HRM.RecruitmentRequestProjectHRValidation",
                c => new
                    {
                        RecruitmentRequestProjectHRValidationId = c.Int(nullable: false, identity: true),
                        RecruitmentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentRequestProjectHRValidationId)
                .ForeignKey("HRM.RecruitmentRequest", t => t.RecruitmentRequestId, cascadeDelete: true)
                .Index(t => t.RecruitmentRequestId);
            
            CreateTable(
                "HRM.RecruitmentRequestSecondValidation",
                c => new
                    {
                        RecruitmentRequestSecondValidationId = c.Int(nullable: false, identity: true),
                        RecruitmentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentRequestSecondValidationId)
                .ForeignKey("HRM.RecruitmentRequest", t => t.RecruitmentRequestId, cascadeDelete: true)
                .Index(t => t.RecruitmentRequestId);
            
            CreateTable(
                "HRM.RelatedEducationFieldDetails",
                c => new
                    {
                        RelatedEducationFieldDetailId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                        ExperienceJobTitleId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RelatedEducationFieldDetailId)
                .ForeignKey("HRM.ExperienceJobTitles", t => t.ExperienceJobTitleId, cascadeDelete: true)
                .Index(t => t.ExperienceJobTitleId);
            
            CreateTable(
                "HRM.SickLeaveSalarySetting",
                c => new
                    {
                        SickLeaveSalarySettingId = c.Int(nullable: false, identity: true),
                        Type = c.Int(),
                        FromDays = c.Int(nullable: false),
                        ToDays = c.Int(nullable: false),
                        PayableSalaryInPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsTerminated = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SickLeaveSalarySettingId);
            
            CreateTable(
                "HRM.TerminationHRManagerValidations",
                c => new
                    {
                        TerminationHRManagerValidationId = c.Int(nullable: false, identity: true),
                        TerminationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TerminationHRManagerValidationId)
                .ForeignKey("HRM.Termination", t => t.TerminationId, cascadeDelete: true)
                .Index(t => t.TerminationId);
            
            CreateTable(
                "HRM.TerminationPMValidations",
                c => new
                    {
                        TerminationPMValidationId = c.Int(nullable: false, identity: true),
                        TerminationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TerminationPMValidationId)
                .ForeignKey("HRM.Termination", t => t.TerminationId, cascadeDelete: true)
                .Index(t => t.TerminationId);
            
            CreateTable(
                "HRM.TerminationSettings",
                c => new
                    {
                        TerminationSettingId = c.Int(nullable: false, identity: true),
                        ResponsibilityLevel = c.Int(),
                        NoticePeriodDays = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TerminationSettingId)
                .Index(t => t.ResponsibilityLevel, unique: true);
            
            CreateTable(
                "HRM.TrainingApplicantAttachments",
                c => new
                    {
                        TrainingApplicantAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        TrainingApplicantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TrainingApplicantAttachmentId)
                .ForeignKey("HRM.TrainingApplicant", t => t.TrainingApplicantId, cascadeDelete: true)
                .Index(t => t.TrainingApplicantId);
            
            CreateTable(
                "HRM.TrainingApplicantSecondValidations",
                c => new
                    {
                        TrainingApplicantSecondValidationId = c.Int(nullable: false, identity: true),
                        TrainingApplicantId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TrainingApplicantSecondValidationId)
                .ForeignKey("HRM.TrainingApplicant", t => t.TrainingApplicantId, cascadeDelete: true)
                .Index(t => t.TrainingApplicantId);
            
            CreateTable(
                "HRM.VacancyDetails",
                c => new
                    {
                        VacancyDetailId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        Experience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        VacancyId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VacancyDetailId)
                .ForeignKey("HRM.Vacancy", t => t.VacancyId, cascadeDelete: true)
                .Index(t => t.VacancyId);
            
            CreateTable(
                "HRM.OverTimeSettings",
                c => new
                    {
                        OverTimeSettingId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(),
                        NotAllowedForHead = c.Boolean(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.OverTimeSettingId)
                .ForeignKey("HRM.Branch", t => t.BranchId)
                .Index(t => t.BranchId);
            
            AddColumn("HRM.Employee", "IsBiometricRegistered", c => c.Boolean(nullable: false));
            AddColumn("HRM.Employee", "BiometricEmployeeId", c => c.Int());
            AddColumn("HRM.Employee", "ProhabitionState", c => c.Int());
            AddColumn("HRM.Employee", "IsProbationEnd", c => c.Boolean());
            AddColumn("HRM.Employee", "IsAttendanceExempt", c => c.Boolean(nullable: false));
            AddColumn("HRM.Employee", "ContratLeftDay", c => c.Int());
            AddColumn("HRM.Employee", "Age", c => c.Int());
            AddColumn("Insurance.EquipmentAccidentClaims", "ForwardedTime", c => c.DateTime());
            AddColumn("Insurance.EmployeeAccidentCreditServices", "ForwardedTime", c => c.DateTime());
            AddColumn("Insurance.EmployeeAccidentInsuranceClaims", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.Announcment", "IsTransfered", c => c.Boolean(nullable: false));
            AddColumn("HRM.Announcment", "IsForAllEmployee", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.ApPettyCash", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.Vacancy", "ReportsTo", c => c.String());
            AddColumn("HRM.Vacancy", "RankId", c => c.Int());
            AddColumn("HRM.Vacancy", "StepId", c => c.Int());
            AddColumn("HRM.Vacancy", "Isposted", c => c.Boolean(nullable: false));
            AddColumn("HRM.Vacancy", "CGPA", c => c.String());
            AddColumn("EXCEED.ApTPayment", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpenseClaim", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelOrder", "ForwardedTime", c => c.DateTime());
            AddColumn("Maintenance.Canibalizations", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.ClearanceProvider", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.ClearanceRequestApprovers", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.ClearanceRequests", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.ClearanceRequestStatus", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.Complain", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.Complain", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("HRM.Complain", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.Complain", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.Complain", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("HRM.Complain", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.Complain", "ForwardedFrom", c => c.String());
            AddColumn("HRM.Complain", "ForwardedTo", c => c.String());
            AddColumn("HRM.Complain", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.Complain", "CurrentApprovalStep", c => c.Int());
            AddColumn("HRM.Complain", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.Complain", "Status", c => c.String());
            AddColumn("HRM.Complain", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("HRM.Complain", "VoidBy", c => c.String());
            AddColumn("HRM.Complain", "VoidTime", c => c.DateTime());
            AddColumn("HRM.Complain", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.Complain", "OnlineApprovedBy", c => c.String());
            AddColumn("HRM.Complain", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.Complain", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("HRM.Complain", "IsOnlineTransferredBy", c => c.String());
            AddColumn("HRM.Complain", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("HRM.Complain", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.Complain", "OrgBranchName", c => c.String());
            AddColumn("HRM.Complain", "SendForApprovalBy", c => c.String());
            AddColumn("HRM.Complain", "SendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.Complain", "DateCreated", c => c.DateTime());
            AddColumn("HRM.Complain", "LastModified", c => c.DateTime());
            AddColumn("HRM.Complain", "CreatedBy", c => c.String());
            AddColumn("HRM.Complain", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.ContractManagement", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.ContractPayment", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.ContractRenewal", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.ContractTermination", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.EducationRequest", "Reason", c => c.String());
            AddColumn("HRM.EducationRequest", "DesiredTrainingDate", c => c.DateTime());
            AddColumn("HRM.EducationRequest", "DesiredTrainingDateAm", c => c.String());
            AddColumn("HRM.EducationRequest", "DurationofTraining", c => c.String());
            AddColumn("HRM.EducationRequest", "TrainingProvider", c => c.String());
            AddColumn("HRM.EducationRequest", "JobTitleId", c => c.Int(nullable: false));
            AddColumn("HRM.Quarter", "IsClosed", c => c.Boolean(nullable: false));
            AddColumn("HRM.Quarter", "DateFrom", c => c.DateTime());
            AddColumn("HRM.Quarter", "DateTo", c => c.DateTime());
            AddColumn("HRM.Quarter", "ClosedDate", c => c.DateTime());
            AddColumn("HRM.Quarter", "ClosedBy", c => c.String());
            AddColumn("HRM.Quarter", "DateCreated", c => c.DateTime());
            AddColumn("HRM.Quarter", "LastModified", c => c.DateTime());
            AddColumn("HRM.Quarter", "CreatedBy", c => c.String());
            AddColumn("HRM.Quarter", "ModifiedBy", c => c.String());
            AddColumn("HRM.QuarterLength", "IsClosed", c => c.Boolean(nullable: false));
            AddColumn("HRM.QuarterLength", "ClosedDate", c => c.DateTime());
            AddColumn("HRM.QuarterLength", "ClosedBy", c => c.String());
            AddColumn("HRM.QuarterLength", "DateCreated", c => c.DateTime());
            AddColumn("HRM.QuarterLength", "LastModified", c => c.DateTime());
            AddColumn("HRM.QuarterLength", "CreatedBy", c => c.String());
            AddColumn("HRM.QuarterLength", "ModifiedBy", c => c.String());
            AddColumn("Insurance.EmployeeMedicalExpenseRefunds", "ForwardedTime", c => c.DateTime());
            AddColumn("Performance.EmployeePerformance", "SecondLevelRejectionDate", c => c.DateTime());
            AddColumn("Performance.EmployeePerformance", "SecondLevelRejector", c => c.String());
            AddColumn("Performance.EmployeePerformance", "InteractionCount", c => c.Int());
            AddColumn("Performance.EmployeePerformance", "IsEscalatedToHR", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "EscalationDate", c => c.DateTime());
            AddColumn("Performance.EmployeePerformance", "SpecialAccomplishments", c => c.String());
            AddColumn("Performance.EmployeePerformance", "Strengths", c => c.String());
            AddColumn("Performance.EmployeePerformance", "AreasNeedingImprovement", c => c.String());
            AddColumn("HRM.EmployeeTransferRequests", "FromOrgStructureId", c => c.Int());
            AddColumn("HRM.EmployeeTransferRequests", "ToBranch1Id", c => c.Int());
            AddColumn("HRM.EmployeeTransferRequests", "ToBranchId", c => c.Int());
            AddColumn("HRM.EmployeeTransferRequests", "FromLookupId", c => c.Int());
            AddColumn("HRM.EmployeeTransferRequests", "LookupId", c => c.Int());
            AddColumn("HRM.EmployeeTransferRequests", "TotalExperience", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.EmployeeTransferRequests", "DesertAllowance", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.EmployeeTransferRequests", "DesertWorkExperienceYears", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.EmployeeTransferRequests", "Count", c => c.Int());
            AddColumn("HRM.EmployeeTransferRequests", "IsDGMApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.EmployeeTransferRequests", "DGMApprovedBy", c => c.String());
            AddColumn("HRM.EmployeeTransferRequests", "DGMApprovedTime", c => c.DateTime());
            AddColumn("HRM.EmployeeTransferRequests", "IsForOther", c => c.Boolean(nullable: false));
            AddColumn("HRM.EmployeeTransferRequests", "IsGMApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.EmployeeTransferRequests", "GMApprovedBy", c => c.String());
            AddColumn("HRM.EmployeeTransferRequests", "GMApprovedTime", c => c.DateTime());
            AddColumn("HRM.EmployeeTransferRequests", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.ExperienceJobTitles", "LookupId", c => c.Int(nullable: false));
            AddColumn("HRM.ExperienceJobTitles", "OrgBranchName", c => c.String());
            AddColumn("HRM.ExperienceJobTitles", "DateCreated", c => c.DateTime());
            AddColumn("HRM.ExperienceJobTitles", "LastModified", c => c.DateTime());
            AddColumn("HRM.ExperienceJobTitles", "CreatedBy", c => c.String());
            AddColumn("HRM.ExperienceJobTitles", "ModifiedBy", c => c.String());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "ForwardedTime", c => c.DateTime());
            AddColumn("Insurance.FireAndLightningInsurance", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetReturn", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.FundReceive", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.FundTransfer", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequest", "ForwardedTime", c => c.DateTime());
            AddColumn("Health.HealthRankAndValues", "ForwardedTime", c => c.DateTime());
            AddColumn("Insurance.InsurancePolicyRenewals", "ForwardedTime", c => c.DateTime());
            AddColumn("Performance.InternalCustomerDetail", "Remark", c => c.String());
            AddColumn("Performance.InternalCustomers", "Remark", c => c.String());
            AddColumn("Performance.InternalCustomerPerformanceEvaluations", "Remark", c => c.String());
            AddColumn("Performance.InternalCustomerEmployeePerformances", "EvaluatorEmployeeId", c => c.Int(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "CriteriaCategoryId", c => c.Int(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "DeadLine", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "Date", c => c.DateTime());
            AddColumn("Performance.InternalCustomerEmployeePerformances", "BranchId", c => c.Int(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "ResponsibilityLevel", c => c.Int(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "JobTitleId", c => c.Int(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "IsCorporateApplied", c => c.Boolean(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "IsSectorApplied", c => c.Boolean(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "IsImmediateSuppurvisorApproved", c => c.Boolean(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "IsAcceptedByEmployee", c => c.Boolean(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "IsRejectedByEmployee", c => c.Boolean(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "IsCurrent", c => c.Boolean(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "IsSummary", c => c.Boolean(nullable: false));
            AddColumn("Performance.InternalCustomerEmployeePerformances", "Comment", c => c.String());
            AddColumn("HRM.LeavePaid", "AccumulatedLeaveDays", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.LeavePaid", "TotalPayableDays", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePaid", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePaid", "DailyRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePaid", "GrossAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePaid", "AmountInBirr", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePaid", "TaxPayable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePaid", "NetPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePaid", "IsCalculated", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePaid", "IsSentToFinance", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePaid", "DateSent", c => c.DateTime());
            AddColumn("HRM.LeavePaid", "SentBy", c => c.String());
            AddColumn("HRM.LeavePaid", "PaymentRequestId", c => c.Int());
            AddColumn("HRM.LeavePayableDetails", "TaxableAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableDetails", "TotalTaxableSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableSettings", "EnableTaxableAllowance", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePlan", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.LeaveRequest", "IsProjectManagerApproved", c => c.Boolean());
            AddColumn("HRM.LeaveRequest", "ProjectManagerApprovedBy", c => c.String());
            AddColumn("HRM.LeaveRequest", "ProjectManagerApprovedDate", c => c.DateTime());
            AddColumn("HRM.LeaveRequest", "IsHRManagerApproved", c => c.Boolean());
            AddColumn("HRM.LeaveRequest", "HRManagerApprovedBy", c => c.String());
            AddColumn("HRM.LeaveRequest", "HRManagerApprovedDate", c => c.DateTime());
            AddColumn("HRM.LeaveReturn", "Reason", c => c.String());
            AddColumn("HRM.LeaveReturn", "IsForOther", c => c.Boolean(nullable: false));
            AddColumn("HRM.OverTimeRequest", "TotalHours", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("HRM.OverTimeRequest", "TotalDays", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("HRM.OverTimeRequest", "GrandTotalHours", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("HRM.OverTimeRequest", "ConvertedTotalDays", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("HRM.OverTimeRequest", "Status", c => c.String());
            AddColumn("HRM.OverTimeRequest", "Remark", c => c.String());
            AddColumn("HRM.OverTimeRequest", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("HRM.OverTimeRequest", "VoidBy", c => c.String());
            AddColumn("HRM.OverTimeRequest", "VoidTime", c => c.DateTime());
            AddColumn("HRM.OverTimeRequest", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.OverTimeRequest", "OnlineApprovedBy", c => c.String());
            AddColumn("HRM.OverTimeRequest", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.OverTimeRequest", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("HRM.OverTimeRequest", "IsOnlineTransferredBy", c => c.String());
            AddColumn("HRM.OverTimeRequest", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("HRM.OverTimeRequest", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.OverTimeRequest", "OrgBranchName", c => c.String());
            AddColumn("HRM.OverTimeRequest", "SendForApprovalBy", c => c.String());
            AddColumn("HRM.OverTimeRequest", "SendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.OverTimeRequest", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.OverTimeRequest", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("HRM.OverTimeRequest", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.OverTimeRequest", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.OverTimeRequest", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("HRM.OverTimeRequest", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.OverTimeRequest", "DateCreated", c => c.DateTime());
            AddColumn("HRM.OverTimeRequest", "LastModified", c => c.DateTime());
            AddColumn("HRM.OverTimeRequest", "CreatedBy", c => c.String());
            AddColumn("HRM.OverTimeRequest", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.ProformaPurchase", "ForwardedTime", c => c.DateTime());
            AddColumn("Performance.PeerToPeerPerformanceEvaluations", "Remark", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "Status", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "Remark", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Performance.PerformanceRequestPeriods", "VoidBy", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "VoidTime", c => c.DateTime());
            AddColumn("Performance.PerformanceRequestPeriods", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Performance.PerformanceRequestPeriods", "OnlineApprovedBy", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Performance.PerformanceRequestPeriods", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Performance.PerformanceRequestPeriods", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Performance.PerformanceRequestPeriods", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Performance.PerformanceRequestPeriods", "OrgBranchName", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "SendForApprovalBy", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "SendForApprovalTime", c => c.DateTime());
            AddColumn("Performance.PerformanceRequestPeriods", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Performance.PerformanceRequestPeriods", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("Performance.PerformanceRequestPeriods", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Performance.PerformanceRequestPeriods", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("Performance.PerformanceRequestPeriods", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.PlacementHistory", "BranchId", c => c.Int());
            AddColumn("HRM.Placement", "SalarChangeDate", c => c.DateTime());
            AddColumn("HRM.Placement", "SalarChangeDateAmharic", c => c.String());
            AddColumn("HRM.Placement", "NextSalaryChangeDate", c => c.DateTime());
            AddColumn("HRM.Placement", "NextSalaryChangeDateAmharic", c => c.String());
            AddColumn("HRM.Placement", "SalaryStructureVersionId", c => c.Int());
            AddColumn("HRM.Placement", "IsEditable", c => c.Boolean(nullable: false));
            AddColumn("HRM.Placement", "EmployeeTypeId", c => c.Int());
            AddColumn("HRM.Placement", "CheckCEO", c => c.Boolean(nullable: false));
            AddColumn("HRM.Placement", "Age", c => c.Double());
            AddColumn("HRM.Placement", "IsChecked", c => c.Boolean(nullable: false));
            AddColumn("HRM.Placement", "IsAttendanceExempt", c => c.Boolean(nullable: false));
            AddColumn("HRM.Placement", "CheckedBy", c => c.String());
            AddColumn("HRM.Placement", "CheckedDate", c => c.DateTime());
            AddColumn("EXCEED.PurchaseOrder", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.PoChangeOforder", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "EmployeeStatus", c => c.Int(nullable: false));
            AddColumn("HRM.ProbationPerformance", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.ProbationPerformance", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("HRM.ProbationPerformance", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.ProbationPerformance", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("HRM.ProbationPerformance", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "ForwardedFrom", c => c.String());
            AddColumn("HRM.ProbationPerformance", "ForwardedTo", c => c.String());
            AddColumn("HRM.ProbationPerformance", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.ProbationPerformance", "CurrentApprovalStep", c => c.Int());
            AddColumn("HRM.ProbationPerformance", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "Remark", c => c.String());
            AddColumn("HRM.ProbationPerformance", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("HRM.ProbationPerformance", "VoidBy", c => c.String());
            AddColumn("HRM.ProbationPerformance", "VoidTime", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.ProbationPerformance", "OnlineApprovedBy", c => c.String());
            AddColumn("HRM.ProbationPerformance", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("HRM.ProbationPerformance", "IsOnlineTransferredBy", c => c.String());
            AddColumn("HRM.ProbationPerformance", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.ProbationPerformance", "OrgBranchName", c => c.String());
            AddColumn("HRM.ProbationPerformance", "SendForApprovalBy", c => c.String());
            AddColumn("HRM.ProbationPerformance", "SendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "DateCreated", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "LastModified", c => c.DateTime());
            AddColumn("HRM.ProbationPerformance", "CreatedBy", c => c.String());
            AddColumn("HRM.ProbationPerformance", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.PurchasePayment", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "NextApproverEmpId", c => c.Int());
            AddColumn("HRM.RecruitmentRequest", "NextApprover", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "IsProjectHRApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "ProjectHRApprovedBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "ProjectHRApprovedDate", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "IsPMApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "PMApprovedBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "PMApprovedDate", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "IsDGMApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "DGMApprovedBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "DGMApprovedDate", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "IsGMApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "GMApprovedBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "GMApprovedDate", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "IsMiddleOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "MiddleOnlineApprovedBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "MiddleOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "IsMiddleSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "MiddleSendForApprovalBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "MiddleSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "SecondOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "SecondSendForApprovalBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "IsSecondVoid", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "SecondVoidBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "SecondVoidTime", c => c.DateTime());
            AddColumn("FleetManagement.RentalAdvancePaymentRequest", "ForwardedTime", c => c.DateTime());
            AddColumn("FleetManagement.RentalPaymentRequest", "ForwardedTime", c => c.DateTime());
            AddColumn("FleetManagement.RequestedEquipment", "ForwardedTime", c => c.DateTime());
            AddColumn("EXCEED.StoreReturn", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.Termination", "OrgStructureId", c => c.Int());
            AddColumn("HRM.Termination", "BusinessUnitId", c => c.Int());
            AddColumn("HRM.Termination", "BranchId", c => c.Int());
            AddColumn("HRM.Termination", "IsEarlyRetirement", c => c.Boolean(nullable: false));
            AddColumn("HRM.Termination", "IsProjectHRApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.Termination", "ProjectHRApprovedBy", c => c.String());
            AddColumn("HRM.Termination", "ProjectHRApprovedDate", c => c.DateTime());
            AddColumn("HRM.Termination", "IsPMApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.Termination", "PMApprovedBy", c => c.String());
            AddColumn("HRM.Termination", "PMApprovedDate", c => c.DateTime());
            AddColumn("HRM.Termination", "IsAdditionalApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.Termination", "AdditionalApprovedBy", c => c.String());
            AddColumn("HRM.Termination", "AdditionalApprovedDate", c => c.DateTime());
            AddColumn("HRM.Termination", "IsDepartmentHeadApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.Termination", "DepartmentHeadApprovedBy", c => c.String());
            AddColumn("HRM.Termination", "DepartmentHeadApprovedDate", c => c.DateTime());
            AddColumn("HRM.Termination", "IsHRManagerApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.Termination", "HRManagerApprovedBy", c => c.String());
            AddColumn("HRM.Termination", "HRManagerApprovedDate", c => c.DateTime());
            AddColumn("HRM.TrainingApplicant", "IsMiddleOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.TrainingApplicant", "MiddleOnlineApprovedBy", c => c.String());
            AddColumn("HRM.TrainingApplicant", "MiddleOnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.VehicleDemand", "ForwardedTime", c => c.DateTime());
            AlterColumn("HRM.Vacancy", "Department", c => c.Int());
            AlterColumn("HRM.Vacancy", "Location", c => c.String());
            AlterColumn("HRM.Vacancy", "Salary", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Payroll.EmpPayrollNoDaysWorked", "Days", c => c.Decimal(nullable: false, precision: 10, scale: 5));
            AlterColumn("HRM.LateComingSummary", "TotalDays", c => c.Decimal(nullable: false, precision: 10, scale: 5));
            AlterColumn("HRM.LeavePayableDetails", "PayableDays", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "Year1", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "Year2", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "AnnualLeavePerMonth", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "AgregateTaxableAmount", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "AgregateIncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "BasicSalaryIncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "TaxDifference", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.LeavePayableDetails", "NetPayment", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.OverTimeRequest", "FromTime", c => c.DateTime());
            AlterColumn("HRM.OverTimeRequest", "ToTime", c => c.DateTime());
            AlterColumn("HRM.Placement", "JobTitleName", c => c.String());
            AlterColumn("HRM.Placement", "MappingId", c => c.Int());
            AlterColumn("HRM.Placement", "ReportsToMappingId", c => c.Int());
            AlterColumn("HRM.Placement", "ApprovedByEmpId", c => c.Int());
            AlterColumn("HRM.Placement", "PreparedByEmpId", c => c.Int());
            AlterColumn("HRM.ProbationPerformance", "Status", c => c.String());
            AlterColumn("HRM.SeveranceCalculationSettings", "Description", c => c.String(maxLength: 500));
            AlterColumn("HRM.SeverancePayableDetail", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "TaxWithheldIncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "GrossComponsation", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "NoneTaxableTotal", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "TaxWithheldComTax", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "SpecialSalaryDeduction", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "TotalDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "Years", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "Months", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "Days", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "TotalServiceMonths", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "GrossCompensationPayment", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "CompensationPaymentT", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "TaxOnMonthlySalaryAndCompensation", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "TaxDifference", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "AccTaxDifference", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.SeverancePayableDetail", "NetPay", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("HRM.Termination", "TerminationDate", c => c.DateTime());
            DropColumn("HRM.Vacancy", "Language");
            DropColumn("HRM.ExperienceJobTitles", "JobTitle");
            DropColumn("HRM.RecruitmentRequest", "IsDepartmentOnlineApproved");
            DropColumn("HRM.RecruitmentRequest", "DepartmentOnlineApprovedBy");
            DropColumn("HRM.RecruitmentRequest", "DepartmentOnlineApprovedTime");
            DropColumn("HRM.RecruitmentRequest", "IsDepartmentSendForApproval");
            DropColumn("HRM.RecruitmentRequest", "DepartmentSendForApprovalBy");
            DropColumn("HRM.RecruitmentRequest", "DepartmentSendForApprovalTime");
        }
        
        public override void Down()
        {
            AddColumn("HRM.RecruitmentRequest", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.RecruitmentRequest", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("HRM.RecruitmentRequest", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.ExperienceJobTitles", "JobTitle", c => c.String());
            AddColumn("HRM.Vacancy", "Language", c => c.Int(nullable: false));
            DropForeignKey("HRM.OverTimeSettings", "BranchId", "HRM.Branch");
            DropForeignKey("HRM.VacancyDetails", "VacancyId", "HRM.Vacancy");
            DropForeignKey("HRM.TrainingApplicantSecondValidations", "TrainingApplicantId", "HRM.TrainingApplicant");
            DropForeignKey("HRM.TrainingApplicantAttachments", "TrainingApplicantId", "HRM.TrainingApplicant");
            DropForeignKey("HRM.TerminationPMValidations", "TerminationId", "HRM.Termination");
            DropForeignKey("HRM.TerminationHRManagerValidations", "TerminationId", "HRM.Termination");
            DropForeignKey("HRM.RelatedEducationFieldDetails", "ExperienceJobTitleId", "HRM.ExperienceJobTitles");
            DropForeignKey("HRM.RecruitmentRequestSecondValidation", "RecruitmentRequestId", "HRM.RecruitmentRequest");
            DropForeignKey("HRM.RecruitmentRequestProjectHRValidation", "RecruitmentRequestId", "HRM.RecruitmentRequest");
            DropForeignKey("HRM.RecruitmentRequestPMValidation", "RecruitmentRequestId", "HRM.RecruitmentRequest");
            DropForeignKey("HRM.RecruitmentRequestHRValidation", "RecruitmentRequestId", "HRM.RecruitmentRequest");
            DropForeignKey("HRM.RecruitmentRequestGMValidation", "RecruitmentRequestId", "HRM.RecruitmentRequest");
            DropForeignKey("HRM.RecruitmentRequestDGMValidation", "RecruitmentRequestId", "HRM.RecruitmentRequest");
            DropForeignKey("HRM.RecruitmentRequestDepartmentValidation", "RecruitmentRequestId", "HRM.RecruitmentRequest");
            DropForeignKey("HRM.ProvidedTrainings", "MainCompanyId", "HRM.MainCompany");
            DropForeignKey("EXCEED.PromotionRequest", "RankId", "HRM.Rank");
            DropForeignKey("EXCEED.PromotionRequest", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("EXCEED.PromotionRequest", "LookupId", "Common.Lookup");
            DropForeignKey("HRM.ProbationPerformanceValidations", "ProbationPerformanceId", "HRM.ProbationPerformance");
            DropForeignKey("HRM.NeedAssessmentAttachments", "NeedAssessment_AssesmentId", "HRM.NeedAssessment");
            DropForeignKey("HRM.LoanRequestValidations", "LoanRequestId", "HRM.LoanRequest");
            DropForeignKey("HRM.LoanRequestDetails", "LoanRequestId", "HRM.LoanRequest");
            DropForeignKey("HRM.LoanRequestDepartmentValidations", "LoanRequestId", "HRM.LoanRequest");
            DropForeignKey("HRM.LoanRequestAttachments", "LoanRequestId", "HRM.LoanRequest");
            DropForeignKey("HRM.LoanRequest", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("HRM.LoanRequest", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.LeaveRequestPMValidations", "LeaveRequestId", "HRM.LeaveRequest");
            DropForeignKey("HRM.LeaveRequestHRManagerValidations", "LeaveRequestId", "HRM.LeaveRequest");
            DropForeignKey("HRM.LeavePaidDeductionDetails", "LeavePaidId", "HRM.LeavePaid");
            DropForeignKey("HRM.JobTitleAndRankMappings", "RankId", "HRM.Rank");
            DropForeignKey("HRM.JobTitleAndRankMappings", "LookupId", "Common.Lookup");
            DropForeignKey("EXCEED.HolidayGiftEmployee", "HolidayGiftRateSettingId", "EXCEED.HolidayGiftRateSetting");
            DropForeignKey("EXCEED.HolidayGiftEmployee", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.GuarantorOrSuretyDetails", "GuarantorOrSuretyId", "HRM.GuarantorOrSureties");
            DropForeignKey("HRM.GuarantorOrSuretyAttachments", "GuarantorOrSuretyId", "HRM.GuarantorOrSureties");
            DropForeignKey("HRM.GuarantorOrSureties", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.ExternalSalaries", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.ExperienceJobTitleDetails", "ExperienceJobTitleId", "HRM.ExperienceJobTitles");
            DropForeignKey("HRM.EmployeeTransferAttachments", "EmployeeTransferRequestId", "HRM.EmployeeTransferRequests");
            DropForeignKey("HRM.TransferRequestGMValidations", "EmployeeTransferRequestId", "HRM.EmployeeTransferRequests");
            DropForeignKey("HRM.TransferRequestDGMValidations", "EmployeeTransferRequestId", "HRM.EmployeeTransferRequests");
            DropForeignKey("HRM.EmployeeRewards", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.EmployeeExternalFundings", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.EmployeeCards", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.EmployeeBenefitDetails", "BenefitTypeId", "Common.Lookup");
            DropForeignKey("HRM.EmployeeBenefitDetails", "EmployeeBenefitId", "HRM.EmployeeBenefits");
            DropForeignKey("HRM.EmployeeBenefits", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.EmployeeBenefits", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.EducationRequestAttachement", "EducationRequestId", "HRM.EducationRequest");
            DropForeignKey("HRM.EducationLevelDetail", "ExperienceJobTitleId", "HRM.ExperienceJobTitles");
            DropForeignKey("HRM.EducationFieldDetails", "ExperienceJobTitleId", "HRM.ExperienceJobTitles");
            DropForeignKey("HRM.ComplainAttachement", "ComplainId", "HRM.Complain");
            DropForeignKey("HRM.AttendanceExceptions", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.AttendanceExceptions", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.VacancyLanguages", "VacancyId", "HRM.Vacancy");
            DropForeignKey("HRM.AnnouncmentAttachments", "AnnouncmentId", "HRM.Announcment");
            DropForeignKey("HRM.ActingAllowances", "AllowanceDefinationId", "HRM.AllowanceDefination");
            DropForeignKey("HRM.Actings", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.ActingAllowances", "ActingId", "HRM.Actings");
            DropIndex("HRM.OverTimeSettings", new[] { "BranchId" });
            DropIndex("HRM.VacancyDetails", new[] { "VacancyId" });
            DropIndex("HRM.TrainingApplicantSecondValidations", new[] { "TrainingApplicantId" });
            DropIndex("HRM.TrainingApplicantAttachments", new[] { "TrainingApplicantId" });
            DropIndex("HRM.TerminationSettings", new[] { "ResponsibilityLevel" });
            DropIndex("HRM.TerminationPMValidations", new[] { "TerminationId" });
            DropIndex("HRM.TerminationHRManagerValidations", new[] { "TerminationId" });
            DropIndex("HRM.RelatedEducationFieldDetails", new[] { "ExperienceJobTitleId" });
            DropIndex("HRM.RecruitmentRequestSecondValidation", new[] { "RecruitmentRequestId" });
            DropIndex("HRM.RecruitmentRequestProjectHRValidation", new[] { "RecruitmentRequestId" });
            DropIndex("HRM.RecruitmentRequestPMValidation", new[] { "RecruitmentRequestId" });
            DropIndex("HRM.RecruitmentRequestHRValidation", new[] { "RecruitmentRequestId" });
            DropIndex("HRM.RecruitmentRequestGMValidation", new[] { "RecruitmentRequestId" });
            DropIndex("HRM.RecruitmentRequestDGMValidation", new[] { "RecruitmentRequestId" });
            DropIndex("HRM.RecruitmentRequestDepartmentValidation", new[] { "RecruitmentRequestId" });
            DropIndex("HRM.ProvidedTrainings", new[] { "MainCompanyId" });
            DropIndex("EXCEED.PromotionRequest", new[] { "RankId" });
            DropIndex("EXCEED.PromotionRequest", new[] { "LookupId" });
            DropIndex("EXCEED.PromotionRequest", new[] { "OrgStructureId" });
            DropIndex("HRM.ProbationPerformanceValidations", new[] { "ProbationPerformanceId" });
            DropIndex("HRM.NeedAssessmentAttachments", new[] { "NeedAssessment_AssesmentId" });
            DropIndex("HRM.LoanRequestValidations", new[] { "LoanRequestId" });
            DropIndex("HRM.LoanRequestDetails", new[] { "LoanRequestId" });
            DropIndex("HRM.LoanRequestDepartmentValidations", new[] { "LoanRequestId" });
            DropIndex("HRM.LoanRequest", new[] { "GLPeriodId" });
            DropIndex("HRM.LoanRequest", new[] { "EmployeeId" });
            DropIndex("HRM.LoanRequestAttachments", new[] { "LoanRequestId" });
            DropIndex("HRM.LeaveRequestPMValidations", new[] { "LeaveRequestId" });
            DropIndex("HRM.LeaveRequestHRManagerValidations", new[] { "LeaveRequestId" });
            DropIndex("HRM.LeavePaidDeductionDetails", new[] { "LeavePaidId" });
            DropIndex("HRM.JobTitleAndRankMappings", new[] { "RankId" });
            DropIndex("HRM.JobTitleAndRankMappings", new[] { "LookupId" });
            DropIndex("EXCEED.HolidayGiftEmployee", new[] { "HolidayGiftRateSettingId" });
            DropIndex("EXCEED.HolidayGiftEmployee", new[] { "EmployeeId" });
            DropIndex("HRM.GuarantorOrSuretyDetails", new[] { "GuarantorOrSuretyId" });
            DropIndex("HRM.GuarantorOrSuretyAttachments", new[] { "GuarantorOrSuretyId" });
            DropIndex("HRM.GuarantorOrSureties", new[] { "EmployeeId" });
            DropIndex("HRM.ExternalSalaries", new[] { "EmployeeId" });
            DropIndex("HRM.ExperienceJobTitleDetails", new[] { "ExperienceJobTitleId" });
            DropIndex("HRM.TransferRequestGMValidations", new[] { "EmployeeTransferRequestId" });
            DropIndex("HRM.TransferRequestDGMValidations", new[] { "EmployeeTransferRequestId" });
            DropIndex("HRM.EmployeeTransferAttachments", new[] { "EmployeeTransferRequestId" });
            DropIndex("HRM.EmployeeRewards", new[] { "EmployeeId" });
            DropIndex("HRM.EmployeeExternalFundings", new[] { "EmployeeId" });
            DropIndex("HRM.EmployeeCards", new[] { "EmployeeId" });
            DropIndex("HRM.EmployeeBenefits", new[] { "GlFiscalYearId" });
            DropIndex("HRM.EmployeeBenefits", new[] { "EmployeeId" });
            DropIndex("HRM.EmployeeBenefitDetails", new[] { "EmployeeBenefitId" });
            DropIndex("HRM.EmployeeBenefitDetails", new[] { "BenefitTypeId" });
            DropIndex("HRM.EducationRequestAttachement", new[] { "EducationRequestId" });
            DropIndex("HRM.EducationLevelDetail", new[] { "ExperienceJobTitleId" });
            DropIndex("HRM.EducationFieldDetails", new[] { "ExperienceJobTitleId" });
            DropIndex("HRM.ComplainAttachement", new[] { "ComplainId" });
            DropIndex("HRM.AttendanceExceptions", new[] { "EmployeeId" });
            DropIndex("HRM.AttendanceExceptions", new[] { "GlFiscalYearId" });
            DropIndex("HRM.VacancyLanguages", new[] { "VacancyId" });
            DropIndex("HRM.AnnouncmentAttachments", new[] { "AnnouncmentId" });
            DropIndex("HRM.Actings", new[] { "EmployeeId" });
            DropIndex("HRM.ActingAllowances", new[] { "ActingId" });
            DropIndex("HRM.ActingAllowances", new[] { "AllowanceDefinationId" });
            AlterColumn("HRM.Termination", "TerminationDate", c => c.DateTime(nullable: false));
            AlterColumn("HRM.SeverancePayableDetail", "NetPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "AccTaxDifference", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "TaxDifference", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "TaxOnMonthlySalaryAndCompensation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "CompensationPaymentT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "GrossCompensationPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "TotalServiceMonths", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "Days", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "Months", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "Years", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "TotalDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "SpecialSalaryDeduction", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "TaxWithheldComTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "NoneTaxableTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "GrossComponsation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "TaxWithheldIncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeverancePayableDetail", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.SeveranceCalculationSettings", "Description", c => c.String());
            AlterColumn("HRM.ProbationPerformance", "Status", c => c.Int(nullable: false));
            AlterColumn("HRM.Placement", "PreparedByEmpId", c => c.Int(nullable: false));
            AlterColumn("HRM.Placement", "ApprovedByEmpId", c => c.Int(nullable: false));
            AlterColumn("HRM.Placement", "ReportsToMappingId", c => c.Int(nullable: false));
            AlterColumn("HRM.Placement", "MappingId", c => c.Int(nullable: false));
            AlterColumn("HRM.Placement", "JobTitleName", c => c.String(nullable: false));
            AlterColumn("HRM.OverTimeRequest", "ToTime", c => c.DateTime(nullable: false));
            AlterColumn("HRM.OverTimeRequest", "FromTime", c => c.DateTime(nullable: false));
            AlterColumn("HRM.LeavePayableDetails", "NetPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "TaxDifference", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "BasicSalaryIncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "AgregateIncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "AgregateTaxableAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "AnnualLeavePerMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "Year2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "Year1", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LeavePayableDetails", "PayableDays", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.LateComingSummary", "TotalDays", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Payroll.EmpPayrollNoDaysWorked", "Days", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.Vacancy", "Salary", c => c.String());
            AlterColumn("HRM.Vacancy", "Location", c => c.Int(nullable: false));
            AlterColumn("HRM.Vacancy", "Department", c => c.Int(nullable: false));
            DropColumn("FleetManagement.VehicleDemand", "ForwardedTime");
            DropColumn("HRM.TrainingApplicant", "MiddleOnlineApprovedTime");
            DropColumn("HRM.TrainingApplicant", "MiddleOnlineApprovedBy");
            DropColumn("HRM.TrainingApplicant", "IsMiddleOnlineApproved");
            DropColumn("HRM.Termination", "HRManagerApprovedDate");
            DropColumn("HRM.Termination", "HRManagerApprovedBy");
            DropColumn("HRM.Termination", "IsHRManagerApproved");
            DropColumn("HRM.Termination", "DepartmentHeadApprovedDate");
            DropColumn("HRM.Termination", "DepartmentHeadApprovedBy");
            DropColumn("HRM.Termination", "IsDepartmentHeadApproved");
            DropColumn("HRM.Termination", "AdditionalApprovedDate");
            DropColumn("HRM.Termination", "AdditionalApprovedBy");
            DropColumn("HRM.Termination", "IsAdditionalApproved");
            DropColumn("HRM.Termination", "PMApprovedDate");
            DropColumn("HRM.Termination", "PMApprovedBy");
            DropColumn("HRM.Termination", "IsPMApproved");
            DropColumn("HRM.Termination", "ProjectHRApprovedDate");
            DropColumn("HRM.Termination", "ProjectHRApprovedBy");
            DropColumn("HRM.Termination", "IsProjectHRApproved");
            DropColumn("HRM.Termination", "IsEarlyRetirement");
            DropColumn("HRM.Termination", "BranchId");
            DropColumn("HRM.Termination", "BusinessUnitId");
            DropColumn("HRM.Termination", "OrgStructureId");
            DropColumn("EXCEED.StoreReturn", "ForwardedTime");
            DropColumn("FleetManagement.RequestedEquipment", "ForwardedTime");
            DropColumn("FleetManagement.RentalPaymentRequest", "ForwardedTime");
            DropColumn("FleetManagement.RentalAdvancePaymentRequest", "ForwardedTime");
            DropColumn("HRM.RecruitmentRequest", "SecondVoidTime");
            DropColumn("HRM.RecruitmentRequest", "SecondVoidBy");
            DropColumn("HRM.RecruitmentRequest", "IsSecondVoid");
            DropColumn("HRM.RecruitmentRequest", "SecondSendForApprovalTime");
            DropColumn("HRM.RecruitmentRequest", "SecondSendForApprovalBy");
            DropColumn("HRM.RecruitmentRequest", "IsSecondSendForApproval");
            DropColumn("HRM.RecruitmentRequest", "SecondOnlineApprovedTime");
            DropColumn("HRM.RecruitmentRequest", "SecondOnlineApprovedBy");
            DropColumn("HRM.RecruitmentRequest", "IsSecondOnlineApproved");
            DropColumn("HRM.RecruitmentRequest", "MiddleSendForApprovalTime");
            DropColumn("HRM.RecruitmentRequest", "MiddleSendForApprovalBy");
            DropColumn("HRM.RecruitmentRequest", "IsMiddleSendForApproval");
            DropColumn("HRM.RecruitmentRequest", "MiddleOnlineApprovedTime");
            DropColumn("HRM.RecruitmentRequest", "MiddleOnlineApprovedBy");
            DropColumn("HRM.RecruitmentRequest", "IsMiddleOnlineApproved");
            DropColumn("HRM.RecruitmentRequest", "GMApprovedDate");
            DropColumn("HRM.RecruitmentRequest", "GMApprovedBy");
            DropColumn("HRM.RecruitmentRequest", "IsGMApproved");
            DropColumn("HRM.RecruitmentRequest", "DGMApprovedDate");
            DropColumn("HRM.RecruitmentRequest", "DGMApprovedBy");
            DropColumn("HRM.RecruitmentRequest", "IsDGMApproved");
            DropColumn("HRM.RecruitmentRequest", "PMApprovedDate");
            DropColumn("HRM.RecruitmentRequest", "PMApprovedBy");
            DropColumn("HRM.RecruitmentRequest", "IsPMApproved");
            DropColumn("HRM.RecruitmentRequest", "ProjectHRApprovedDate");
            DropColumn("HRM.RecruitmentRequest", "ProjectHRApprovedBy");
            DropColumn("HRM.RecruitmentRequest", "IsProjectHRApproved");
            DropColumn("HRM.RecruitmentRequest", "NextApprover");
            DropColumn("HRM.RecruitmentRequest", "NextApproverEmpId");
            DropColumn("EXCEED.PurchasePayment", "ForwardedTime");
            DropColumn("HRM.ProbationPerformance", "ModifiedBy");
            DropColumn("HRM.ProbationPerformance", "CreatedBy");
            DropColumn("HRM.ProbationPerformance", "LastModified");
            DropColumn("HRM.ProbationPerformance", "DateCreated");
            DropColumn("HRM.ProbationPerformance", "SendForApprovalTime");
            DropColumn("HRM.ProbationPerformance", "SendForApprovalBy");
            DropColumn("HRM.ProbationPerformance", "OrgBranchName");
            DropColumn("HRM.ProbationPerformance", "IsSendForApproval");
            DropColumn("HRM.ProbationPerformance", "OnlineTransferredTime");
            DropColumn("HRM.ProbationPerformance", "IsOnlineTransferredBy");
            DropColumn("HRM.ProbationPerformance", "IsOnlineTransferred");
            DropColumn("HRM.ProbationPerformance", "OnlineApprovedTime");
            DropColumn("HRM.ProbationPerformance", "OnlineApprovedBy");
            DropColumn("HRM.ProbationPerformance", "IsOnlineApproved");
            DropColumn("HRM.ProbationPerformance", "VoidTime");
            DropColumn("HRM.ProbationPerformance", "VoidBy");
            DropColumn("HRM.ProbationPerformance", "IsVoid");
            DropColumn("HRM.ProbationPerformance", "Remark");
            DropColumn("HRM.ProbationPerformance", "ForwardedTime");
            DropColumn("HRM.ProbationPerformance", "CurrentApprovalStep");
            DropColumn("HRM.ProbationPerformance", "IsFinalApproval");
            DropColumn("HRM.ProbationPerformance", "ForwardedTo");
            DropColumn("HRM.ProbationPerformance", "ForwardedFrom");
            DropColumn("HRM.ProbationPerformance", "DepartmentSendForApprovalTime");
            DropColumn("HRM.ProbationPerformance", "DepartmentSendForApprovalBy");
            DropColumn("HRM.ProbationPerformance", "IsDepartmentSendForApproval");
            DropColumn("HRM.ProbationPerformance", "DepartmentOnlineApprovedTime");
            DropColumn("HRM.ProbationPerformance", "DepartmentOnlineApprovedBy");
            DropColumn("HRM.ProbationPerformance", "IsDepartmentOnlineApproved");
            DropColumn("HRM.ProbationPerformance", "EmployeeStatus");
            DropColumn("EXCEED.PoChangeOforder", "ForwardedTime");
            DropColumn("EXCEED.PurchaseOrder", "ForwardedTime");
            DropColumn("HRM.Placement", "CheckedDate");
            DropColumn("HRM.Placement", "CheckedBy");
            DropColumn("HRM.Placement", "IsAttendanceExempt");
            DropColumn("HRM.Placement", "IsChecked");
            DropColumn("HRM.Placement", "Age");
            DropColumn("HRM.Placement", "CheckCEO");
            DropColumn("HRM.Placement", "EmployeeTypeId");
            DropColumn("HRM.Placement", "IsEditable");
            DropColumn("HRM.Placement", "SalaryStructureVersionId");
            DropColumn("HRM.Placement", "NextSalaryChangeDateAmharic");
            DropColumn("HRM.Placement", "NextSalaryChangeDate");
            DropColumn("HRM.Placement", "SalarChangeDateAmharic");
            DropColumn("HRM.Placement", "SalarChangeDate");
            DropColumn("HRM.PlacementHistory", "BranchId");
            DropColumn("Performance.PerformanceRequestPeriods", "DepartmentSendForApprovalTime");
            DropColumn("Performance.PerformanceRequestPeriods", "DepartmentSendForApprovalBy");
            DropColumn("Performance.PerformanceRequestPeriods", "IsDepartmentSendForApproval");
            DropColumn("Performance.PerformanceRequestPeriods", "DepartmentOnlineApprovedTime");
            DropColumn("Performance.PerformanceRequestPeriods", "DepartmentOnlineApprovedBy");
            DropColumn("Performance.PerformanceRequestPeriods", "IsDepartmentOnlineApproved");
            DropColumn("Performance.PerformanceRequestPeriods", "SendForApprovalTime");
            DropColumn("Performance.PerformanceRequestPeriods", "SendForApprovalBy");
            DropColumn("Performance.PerformanceRequestPeriods", "OrgBranchName");
            DropColumn("Performance.PerformanceRequestPeriods", "IsSendForApproval");
            DropColumn("Performance.PerformanceRequestPeriods", "OnlineTransferredTime");
            DropColumn("Performance.PerformanceRequestPeriods", "IsOnlineTransferredBy");
            DropColumn("Performance.PerformanceRequestPeriods", "IsOnlineTransferred");
            DropColumn("Performance.PerformanceRequestPeriods", "OnlineApprovedTime");
            DropColumn("Performance.PerformanceRequestPeriods", "OnlineApprovedBy");
            DropColumn("Performance.PerformanceRequestPeriods", "IsOnlineApproved");
            DropColumn("Performance.PerformanceRequestPeriods", "VoidTime");
            DropColumn("Performance.PerformanceRequestPeriods", "VoidBy");
            DropColumn("Performance.PerformanceRequestPeriods", "IsVoid");
            DropColumn("Performance.PerformanceRequestPeriods", "Remark");
            DropColumn("Performance.PerformanceRequestPeriods", "Status");
            DropColumn("Performance.PeerToPeerPerformanceEvaluations", "Remark");
            DropColumn("EXCEED.ProformaPurchase", "ForwardedTime");
            DropColumn("HRM.OverTimeRequest", "ModifiedBy");
            DropColumn("HRM.OverTimeRequest", "CreatedBy");
            DropColumn("HRM.OverTimeRequest", "LastModified");
            DropColumn("HRM.OverTimeRequest", "DateCreated");
            DropColumn("HRM.OverTimeRequest", "DepartmentSendForApprovalTime");
            DropColumn("HRM.OverTimeRequest", "DepartmentSendForApprovalBy");
            DropColumn("HRM.OverTimeRequest", "IsDepartmentSendForApproval");
            DropColumn("HRM.OverTimeRequest", "DepartmentOnlineApprovedTime");
            DropColumn("HRM.OverTimeRequest", "DepartmentOnlineApprovedBy");
            DropColumn("HRM.OverTimeRequest", "IsDepartmentOnlineApproved");
            DropColumn("HRM.OverTimeRequest", "SendForApprovalTime");
            DropColumn("HRM.OverTimeRequest", "SendForApprovalBy");
            DropColumn("HRM.OverTimeRequest", "OrgBranchName");
            DropColumn("HRM.OverTimeRequest", "IsSendForApproval");
            DropColumn("HRM.OverTimeRequest", "OnlineTransferredTime");
            DropColumn("HRM.OverTimeRequest", "IsOnlineTransferredBy");
            DropColumn("HRM.OverTimeRequest", "IsOnlineTransferred");
            DropColumn("HRM.OverTimeRequest", "OnlineApprovedTime");
            DropColumn("HRM.OverTimeRequest", "OnlineApprovedBy");
            DropColumn("HRM.OverTimeRequest", "IsOnlineApproved");
            DropColumn("HRM.OverTimeRequest", "VoidTime");
            DropColumn("HRM.OverTimeRequest", "VoidBy");
            DropColumn("HRM.OverTimeRequest", "IsVoid");
            DropColumn("HRM.OverTimeRequest", "Remark");
            DropColumn("HRM.OverTimeRequest", "Status");
            DropColumn("HRM.OverTimeRequest", "ConvertedTotalDays");
            DropColumn("HRM.OverTimeRequest", "GrandTotalHours");
            DropColumn("HRM.OverTimeRequest", "TotalDays");
            DropColumn("HRM.OverTimeRequest", "TotalHours");
            DropColumn("HRM.LeaveReturn", "IsForOther");
            DropColumn("HRM.LeaveReturn", "Reason");
            DropColumn("HRM.LeaveRequest", "HRManagerApprovedDate");
            DropColumn("HRM.LeaveRequest", "HRManagerApprovedBy");
            DropColumn("HRM.LeaveRequest", "IsHRManagerApproved");
            DropColumn("HRM.LeaveRequest", "ProjectManagerApprovedDate");
            DropColumn("HRM.LeaveRequest", "ProjectManagerApprovedBy");
            DropColumn("HRM.LeaveRequest", "IsProjectManagerApproved");
            DropColumn("HRM.LeavePlan", "ForwardedTime");
            DropColumn("HRM.LeavePayableSettings", "EnableTaxableAllowance");
            DropColumn("HRM.LeavePayableDetails", "TotalTaxableSalary");
            DropColumn("HRM.LeavePayableDetails", "TaxableAllowance");
            DropColumn("HRM.LeavePaid", "PaymentRequestId");
            DropColumn("HRM.LeavePaid", "SentBy");
            DropColumn("HRM.LeavePaid", "DateSent");
            DropColumn("HRM.LeavePaid", "IsSentToFinance");
            DropColumn("HRM.LeavePaid", "IsCalculated");
            DropColumn("HRM.LeavePaid", "NetPay");
            DropColumn("HRM.LeavePaid", "TaxPayable");
            DropColumn("HRM.LeavePaid", "AmountInBirr");
            DropColumn("HRM.LeavePaid", "GrossAmount");
            DropColumn("HRM.LeavePaid", "DailyRate");
            DropColumn("HRM.LeavePaid", "BasicSalary");
            DropColumn("HRM.LeavePaid", "TotalPayableDays");
            DropColumn("HRM.LeavePaid", "AccumulatedLeaveDays");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "Comment");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "IsSummary");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "IsCurrent");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "IsRejectedByEmployee");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "IsAcceptedByEmployee");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "IsImmediateSuppurvisorApproved");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "IsSectorApplied");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "IsCorporateApplied");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "JobTitleId");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "ResponsibilityLevel");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "BusinessUnitId");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "BranchId");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "Date");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "DeadLine");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "OrgStructureId");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "CriteriaCategoryId");
            DropColumn("Performance.InternalCustomerEmployeePerformances", "EvaluatorEmployeeId");
            DropColumn("Performance.InternalCustomerPerformanceEvaluations", "Remark");
            DropColumn("Performance.InternalCustomers", "Remark");
            DropColumn("Performance.InternalCustomerDetail", "Remark");
            DropColumn("Insurance.InsurancePolicyRenewals", "ForwardedTime");
            DropColumn("Health.HealthRankAndValues", "ForwardedTime");
            DropColumn("EXCEED.PaymentRequest", "ForwardedTime");
            DropColumn("EXCEED.FundTransfer", "ForwardedTime");
            DropColumn("EXCEED.FundReceive", "ForwardedTime");
            DropColumn("EXCEED.FixedAssetReturn", "ForwardedTime");
            DropColumn("Insurance.FireAndLightningInsurance", "ForwardedTime");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "ForwardedTime");
            DropColumn("HRM.ExperienceJobTitles", "ModifiedBy");
            DropColumn("HRM.ExperienceJobTitles", "CreatedBy");
            DropColumn("HRM.ExperienceJobTitles", "LastModified");
            DropColumn("HRM.ExperienceJobTitles", "DateCreated");
            DropColumn("HRM.ExperienceJobTitles", "OrgBranchName");
            DropColumn("HRM.ExperienceJobTitles", "LookupId");
            DropColumn("HRM.EmployeeTransferRequests", "ForwardedTime");
            DropColumn("HRM.EmployeeTransferRequests", "GMApprovedTime");
            DropColumn("HRM.EmployeeTransferRequests", "GMApprovedBy");
            DropColumn("HRM.EmployeeTransferRequests", "IsGMApproved");
            DropColumn("HRM.EmployeeTransferRequests", "IsForOther");
            DropColumn("HRM.EmployeeTransferRequests", "DGMApprovedTime");
            DropColumn("HRM.EmployeeTransferRequests", "DGMApprovedBy");
            DropColumn("HRM.EmployeeTransferRequests", "IsDGMApproved");
            DropColumn("HRM.EmployeeTransferRequests", "Count");
            DropColumn("HRM.EmployeeTransferRequests", "DesertWorkExperienceYears");
            DropColumn("HRM.EmployeeTransferRequests", "DesertAllowance");
            DropColumn("HRM.EmployeeTransferRequests", "TotalExperience");
            DropColumn("HRM.EmployeeTransferRequests", "LookupId");
            DropColumn("HRM.EmployeeTransferRequests", "FromLookupId");
            DropColumn("HRM.EmployeeTransferRequests", "ToBranchId");
            DropColumn("HRM.EmployeeTransferRequests", "ToBranch1Id");
            DropColumn("HRM.EmployeeTransferRequests", "FromOrgStructureId");
            DropColumn("Performance.EmployeePerformance", "AreasNeedingImprovement");
            DropColumn("Performance.EmployeePerformance", "Strengths");
            DropColumn("Performance.EmployeePerformance", "SpecialAccomplishments");
            DropColumn("Performance.EmployeePerformance", "EscalationDate");
            DropColumn("Performance.EmployeePerformance", "IsEscalatedToHR");
            DropColumn("Performance.EmployeePerformance", "InteractionCount");
            DropColumn("Performance.EmployeePerformance", "SecondLevelRejector");
            DropColumn("Performance.EmployeePerformance", "SecondLevelRejectionDate");
            DropColumn("Insurance.EmployeeMedicalExpenseRefunds", "ForwardedTime");
            DropColumn("HRM.QuarterLength", "ModifiedBy");
            DropColumn("HRM.QuarterLength", "CreatedBy");
            DropColumn("HRM.QuarterLength", "LastModified");
            DropColumn("HRM.QuarterLength", "DateCreated");
            DropColumn("HRM.QuarterLength", "ClosedBy");
            DropColumn("HRM.QuarterLength", "ClosedDate");
            DropColumn("HRM.QuarterLength", "IsClosed");
            DropColumn("HRM.Quarter", "ModifiedBy");
            DropColumn("HRM.Quarter", "CreatedBy");
            DropColumn("HRM.Quarter", "LastModified");
            DropColumn("HRM.Quarter", "DateCreated");
            DropColumn("HRM.Quarter", "ClosedBy");
            DropColumn("HRM.Quarter", "ClosedDate");
            DropColumn("HRM.Quarter", "DateTo");
            DropColumn("HRM.Quarter", "DateFrom");
            DropColumn("HRM.Quarter", "IsClosed");
            DropColumn("HRM.EducationRequest", "JobTitleId");
            DropColumn("HRM.EducationRequest", "TrainingProvider");
            DropColumn("HRM.EducationRequest", "DurationofTraining");
            DropColumn("HRM.EducationRequest", "DesiredTrainingDateAm");
            DropColumn("HRM.EducationRequest", "DesiredTrainingDate");
            DropColumn("HRM.EducationRequest", "Reason");
            DropColumn("EXCEED.ContractTermination", "ForwardedTime");
            DropColumn("EXCEED.ContractRenewal", "ForwardedTime");
            DropColumn("EXCEED.ContractPayment", "ForwardedTime");
            DropColumn("EXCEED.ContractManagement", "ForwardedTime");
            DropColumn("HRM.Complain", "ModifiedBy");
            DropColumn("HRM.Complain", "CreatedBy");
            DropColumn("HRM.Complain", "LastModified");
            DropColumn("HRM.Complain", "DateCreated");
            DropColumn("HRM.Complain", "SendForApprovalTime");
            DropColumn("HRM.Complain", "SendForApprovalBy");
            DropColumn("HRM.Complain", "OrgBranchName");
            DropColumn("HRM.Complain", "IsSendForApproval");
            DropColumn("HRM.Complain", "OnlineTransferredTime");
            DropColumn("HRM.Complain", "IsOnlineTransferredBy");
            DropColumn("HRM.Complain", "IsOnlineTransferred");
            DropColumn("HRM.Complain", "OnlineApprovedTime");
            DropColumn("HRM.Complain", "OnlineApprovedBy");
            DropColumn("HRM.Complain", "IsOnlineApproved");
            DropColumn("HRM.Complain", "VoidTime");
            DropColumn("HRM.Complain", "VoidBy");
            DropColumn("HRM.Complain", "IsVoid");
            DropColumn("HRM.Complain", "Status");
            DropColumn("HRM.Complain", "ForwardedTime");
            DropColumn("HRM.Complain", "CurrentApprovalStep");
            DropColumn("HRM.Complain", "IsFinalApproval");
            DropColumn("HRM.Complain", "ForwardedTo");
            DropColumn("HRM.Complain", "ForwardedFrom");
            DropColumn("HRM.Complain", "DepartmentSendForApprovalTime");
            DropColumn("HRM.Complain", "DepartmentSendForApprovalBy");
            DropColumn("HRM.Complain", "IsDepartmentSendForApproval");
            DropColumn("HRM.Complain", "DepartmentOnlineApprovedTime");
            DropColumn("HRM.Complain", "DepartmentOnlineApprovedBy");
            DropColumn("HRM.Complain", "IsDepartmentOnlineApproved");
            DropColumn("HRM.ClearanceRequestStatus", "ForwardedTime");
            DropColumn("HRM.ClearanceRequests", "ForwardedTime");
            DropColumn("HRM.ClearanceRequestApprovers", "ForwardedTime");
            DropColumn("HRM.ClearanceProvider", "ForwardedTime");
            DropColumn("Maintenance.Canibalizations", "ForwardedTime");
            DropColumn("EXCEED.ApTravelOrder", "ForwardedTime");
            DropColumn("EXCEED.ApTravelExpenseClaim", "ForwardedTime");
            DropColumn("EXCEED.ApTPayment", "ForwardedTime");
            DropColumn("HRM.Vacancy", "CGPA");
            DropColumn("HRM.Vacancy", "Isposted");
            DropColumn("HRM.Vacancy", "StepId");
            DropColumn("HRM.Vacancy", "RankId");
            DropColumn("HRM.Vacancy", "ReportsTo");
            DropColumn("EXCEED.ApPettyCash", "ForwardedTime");
            DropColumn("EXCEED.ApPaymentSettlement", "ForwardedTime");
            DropColumn("HRM.Announcment", "IsForAllEmployee");
            DropColumn("HRM.Announcment", "IsTransfered");
            DropColumn("Insurance.EmployeeAccidentInsuranceClaims", "ForwardedTime");
            DropColumn("Insurance.EmployeeAccidentCreditServices", "ForwardedTime");
            DropColumn("Insurance.EquipmentAccidentClaims", "ForwardedTime");
            DropColumn("HRM.Employee", "Age");
            DropColumn("HRM.Employee", "ContratLeftDay");
            DropColumn("HRM.Employee", "IsAttendanceExempt");
            DropColumn("HRM.Employee", "IsProbationEnd");
            DropColumn("HRM.Employee", "ProhabitionState");
            DropColumn("HRM.Employee", "BiometricEmployeeId");
            DropColumn("HRM.Employee", "IsBiometricRegistered");
            DropTable("HRM.OverTimeSettings");
            DropTable("HRM.VacancyDetails");
            DropTable("HRM.TrainingApplicantSecondValidations");
            DropTable("HRM.TrainingApplicantAttachments");
            DropTable("HRM.TerminationSettings");
            DropTable("HRM.TerminationPMValidations");
            DropTable("HRM.TerminationHRManagerValidations");
            DropTable("HRM.SickLeaveSalarySetting");
            DropTable("HRM.RelatedEducationFieldDetails");
            DropTable("HRM.RecruitmentRequestSecondValidation");
            DropTable("HRM.RecruitmentRequestProjectHRValidation");
            DropTable("HRM.RecruitmentRequestPMValidation");
            DropTable("HRM.RecruitmentRequestHRValidation");
            DropTable("HRM.RecruitmentRequestGMValidation");
            DropTable("HRM.RecruitmentRequestDGMValidation");
            DropTable("HRM.RecruitmentRequestDepartmentValidation");
            DropTable("HRM.ProvidedTrainings");
            DropTable("HRM.PromotionYearSettings");
            DropTable("EXCEED.PromotionRequest");
            DropTable("HRM.ProbationsSettings");
            DropTable("HRM.ProbationPerformanceValidations");
            DropTable("HRM.PlacementWorkflowSettings");
            DropTable("HRM.PerformanceEvaluationSettings");
            DropTable("HRM.PerformanceAllowedScores");
            DropTable("HRM.NeedAssessmentAttachments");
            DropTable("HRM.LoanRequestValidations");
            DropTable("HRM.LoanRequestDetails");
            DropTable("HRM.LoanRequestDepartmentValidations");
            DropTable("HRM.LoanRequest");
            DropTable("HRM.LoanRequestAttachments");
            DropTable("HRM.LegacyEmployees");
            DropTable("HRM.LeaveRequestPMValidations");
            DropTable("HRM.LeaveRequestHRManagerValidations");
            DropTable("HRM.LeavePaidDeductionDetails");
            DropTable("HRM.JobTitleAndRankMappings");
            DropTable("HRM.InternalVacancyEligibilitySettings");
            DropTable("HRM.InternalRecruitmentCriteriaTypeSettings");
            DropTable("EXCEED.HolidayGiftRateSetting");
            DropTable("EXCEED.HolidayGiftEmployee");
            DropTable("HRM.GuarantorOrSuretyDetails");
            DropTable("HRM.GuarantorOrSuretyAttachments");
            DropTable("HRM.GuarantorOrSureties");
            DropTable("HRM.ExternalSalaries");
            DropTable("HRM.ExperienceJobTitleDetails");
            DropTable("HRM.TransferRequestGMValidations");
            DropTable("HRM.TransferRequestDGMValidations");
            DropTable("HRM.EmployeeTransferAttachments");
            DropTable("HRM.EmployeeSalaryIncrementSettings");
            DropTable("HRM.EmployeeRewards");
            DropTable("HRM.EmployeeExternalFundings");
            DropTable("HRM.EmployeeCards");
            DropTable("HRM.EmployeeBenefits");
            DropTable("HRM.EmployeeBenefitDetails");
            DropTable("HRM.EmployeeAttendanceSummary");
            DropTable("HRM.EducationRequestAttachement");
            DropTable("HRM.EducationLevelDetail");
            DropTable("HRM.EducationFieldDetails");
            DropTable("HRM.DelegationStepIncrementSettings");
            DropTable("HRM.DelegationBenefitSettings");
            DropTable("HRM.DailyAttendanceSettings");
            DropTable("HRM.ComplainAttachement");
            DropTable("HRM.CompanyLeaveWorkflowSettings");
            DropTable("HRM.CompanyApprovalSettings");
            DropTable("HRM.AttendanceExceptions");
            DropTable("HRM.AttendanceBio");
            DropTable("HRM.ApplicantScreeningResultAttachments");
            DropTable("HRM.VacancyLanguages");
            DropTable("HRM.AnnouncmentAttachments");
            DropTable("HRM.AllowanceSettings");
            DropTable("HRM.AllowanceGenerationSettings");
            DropTable("HRM.Actings");
            DropTable("HRM.ActingAllowances");
            MoveTable(name: "HRM.TrainingEvaluationBefores", newSchema: "EXCEED");
            MoveTable(name: "HRM.TrainingNeedRequests", newSchema: "EXCEED");
            MoveTable(name: "HRM.TrainingEvaluationAfters", newSchema: "EXCEED");
            MoveTable(name: "HRM.ScreeningSettings", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.RegularContribution", newSchema: "Payroll");
            MoveTable(name: "Performance.PerformanceSettings", newSchema: "EXCEED");
            MoveTable(name: "Performance.PerformanceRequestPeriods", newSchema: "EXCEED");
            MoveTable(name: "Performance.PeerToPeerPerformanceEvaluations", newSchema: "EXCEED");
            MoveTable(name: "Performance.PeerToPeerEmployeePerformanceScores", newSchema: "EXCEED");
            MoveTable(name: "Performance.PeerToPeerEmployeePerformances", newSchema: "EXCEED");
            MoveTable(name: "Performance.PeerEmployeeMeasurements", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.OverTimeTrasferDetail", newSchema: "Payroll");
            MoveTable(name: "EXCEED.OverTimeTransfer", newSchema: "Payroll");
            MoveTable(name: "EXCEED.OverTime", newSchema: "Payroll");
            MoveTable(name: "EXCEED.OverTimePerDay", newSchema: "Payroll");
            MoveTable(name: "HRM.NeedAssessmentSummaryDetails", newSchema: "EXCEED");
            MoveTable(name: "HRM.NeedAssessmentSummary", newSchema: "EXCEED");
            MoveTable(name: "Performance.MainCompanyEvalutionSettings", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.Loan", newSchema: "Payroll");
            MoveTable(name: "EXCEED.LeaveTransfer", newSchema: "HRM");
            MoveTable(name: "HRM.LeaveRequestAttachements", newSchema: "EXCEED");
            MoveTable(name: "Performance.InternalCustomerEmployeePerformanceScores", newSchema: "EXCEED");
            MoveTable(name: "Performance.InternalCustomerEmployeePerformances", newSchema: "EXCEED");
            MoveTable(name: "Performance.InternalCustomerEmployeeMeasurements", newSchema: "EXCEED");
            MoveTable(name: "Performance.InternalCustomerPerformanceEvaluations", newSchema: "EXCEED");
            MoveTable(name: "Performance.InternalCustomers", newSchema: "EXCEED");
            MoveTable(name: "Performance.InternalCustomerDetail", newSchema: "EXCEED");
            MoveTable(name: "HRM.InternalApplicantScreenings", newSchema: "EXCEED");
            MoveTable(name: "HRM.InternalApplicantScreeningResults", newSchema: "EXCEED");
            MoveTable(name: "HRM.InternalApplicants", newSchema: "EXCEED");
            MoveTable(name: "HRM.InternalApplicantExamResults", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.FixedContribution", newSchema: "Payroll");
            MoveTable(name: "HRM.ExperienceJobTitles", newSchema: "EXCEED");
            MoveTable(name: "HRM.TransferRequestValidations", newSchema: "EXCEED");
            MoveTable(name: "HRM.TransferRequestDetails", newSchema: "EXCEED");
            MoveTable(name: "HRM.TransferRequestDepartmentValidations", newSchema: "EXCEED");
            MoveTable(name: "HRM.EmployeeTransferRequests", newSchema: "EXCEED");
            MoveTable(name: "HRM.EmployeeProbationSettings", newSchema: "EXCEED");
            MoveTable(name: "Performance.DatePreferenceSetting", newSchema: "EXCEED");
            MoveTable(name: "HRM.CriteriaCategories", newSchema: "EXCEED");
            MoveTable(name: "HRM.ClearanceRequestStatus", newSchema: "EXCEED");
            MoveTable(name: "HRM.ClearanceRequests", newSchema: "EXCEED");
            MoveTable(name: "HRM.ClearanceRequestApprovers", newSchema: "EXCEED");
            MoveTable(name: "HRM.ClearanceProviderDetails", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.Association", newSchema: "Payroll");
            MoveTable(name: "HRM.ApplicantScreenResults", newSchema: "EXCEED");
            MoveTable(name: "EXCEED.Advance", newSchema: "Payroll");
            MoveTable(name: "EXCEED.Addition", newSchema: "Payroll");
            RenameTable(name: "EXCEED.TrainingEvaluationBefores", newName: "TrainingEvaluationBefore");
            RenameTable(name: "EXCEED.TrainingNeedRequests", newName: "TrainingNeedRequest");
            RenameTable(name: "EXCEED.TrainingEvaluationAfters", newName: "TrainingEvaluationAfter");
            RenameTable(name: "EXCEED.ScreeningSettings", newName: "ScreeningSetting");
            RenameTable(name: "EXCEED.PerformanceSettings", newName: "PerformanceSetting");
            RenameTable(name: "EXCEED.PerformanceRequestPeriods", newName: "PerformanceRequestPeriod");
            RenameTable(name: "EXCEED.PeerToPeerPerformanceEvaluations", newName: "PeerToPeerPerformanceEvaluation");
            RenameTable(name: "EXCEED.PeerToPeerEmployeePerformanceScores", newName: "PeerToPeerEmployeePerformanceScore");
            RenameTable(name: "EXCEED.PeerToPeerEmployeePerformances", newName: "PeerToPeerEmployeePerformance");
            RenameTable(name: "EXCEED.PeerEmployeeMeasurements", newName: "PeerEmployeeMeasurement");
            RenameTable(name: "HRM.OtherBenefits", newName: "OtherBenefit");
            RenameTable(name: "EXCEED.NeedAssessmentSummaryDetails", newName: "NeedAssessmentSummaryDetail");
            RenameTable(name: "EXCEED.MainCompanyEvalutionSettings", newName: "MainCompanyEvalutionSetting");
            RenameTable(name: "EXCEED.LeaveRequestAttachements", newName: "LeaveRequestAttachement");
            RenameTable(name: "EXCEED.InternalCustomerEmployeePerformanceScores", newName: "InternalCustomerEmployeePerformanceScore");
            RenameTable(name: "EXCEED.InternalCustomerEmployeePerformances", newName: "InternalCustomerEmployeePerformance");
            RenameTable(name: "EXCEED.InternalCustomerEmployeeMeasurements", newName: "InternalCustomerEmployeeMeasurement");
            RenameTable(name: "EXCEED.InternalCustomerPerformanceEvaluations", newName: "InternalCustomerPerformanceEvaluation");
            RenameTable(name: "EXCEED.InternalCustomers", newName: "InternalCustomer");
            RenameTable(name: "EXCEED.InternalApplicantScreenings", newName: "InternalApplicantScreening");
            RenameTable(name: "EXCEED.InternalApplicantScreeningResults", newName: "InternalApplicantScreeningResult");
            RenameTable(name: "EXCEED.InternalApplicantExamResults", newName: "InternalApplicantExamResult");
            RenameTable(name: "EXCEED.ExperienceJobTitles", newName: "ExperienceJobTitle");
            RenameTable(name: "EXCEED.TransferRequestValidations", newName: "TransferRequestValidation");
            RenameTable(name: "EXCEED.TransferRequestDetails", newName: "TransferRequestDetail");
            RenameTable(name: "EXCEED.TransferRequestDepartmentValidations", newName: "TransferRequestDepartmentValidation");
            RenameTable(name: "EXCEED.EmployeeTransferRequests", newName: "EmployeeTransferRequest");
            RenameTable(name: "EXCEED.EmployeeProbationSettings", newName: "EmployeeProbationSetting");
            RenameTable(name: "EXCEED.CriteriaCategories", newName: "CriteriaCategory");
            RenameTable(name: "EXCEED.ClearanceRequests", newName: "ClearanceRequest");
            RenameTable(name: "EXCEED.ClearanceRequestApprovers", newName: "ClearanceRequestApprover");
            RenameTable(name: "EXCEED.ClearanceProviderDetails", newName: "ClearanceProviderDetail");
            RenameTable(name: "EXCEED.ApplicantScreenResults", newName: "ApplicantScreenResult");
        }
    }
}
