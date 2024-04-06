namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmupgrademig : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.CommitteMinute", "DisciplinaryCaseId", "HRM.DisciplinaryCase");
            DropForeignKey("HRM.Delegation", "GLPeriodId", "EXCEED.GLPeriod");
            DropIndex("HRM.CommitteMinute", new[] { "DisciplinaryCaseId" });
            DropIndex("HRM.Delegation", new[] { "GLPeriodId" });
            CreateTable(
                "EXCEED.CriteriaCategory",
                c => new
                    {
                        CriteriaCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CriteriaCategoryId);
            
            CreateTable(
                "EXCEED.EmployeeFolderAttachment",
                c => new
                    {
                        EmployeeFolderAttachmentId = c.Int(nullable: false, identity: true),
                        PersonalArchiveNoId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeFolderAttachmentId);
            
            CreateTable(
                "EXCEED.EmployeeProbationSetting",
                c => new
                    {
                        EmployeeProbationSettingId = c.Int(nullable: false, identity: true),
                        IncludeWeekend = c.Boolean(nullable: false),
                        IncludeHoliday = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeProbationSettingId);
            
            CreateTable(
                "EXCEED.EmployeeTransferRequest",
                c => new
                    {
                        EmployeeTransferRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        Reason = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                        BranchId = c.Int(nullable: false),
                        FromBusinessUnitId = c.Int(),
                        FromBranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(),
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
                .PrimaryKey(t => t.EmployeeTransferRequestId);
            
            CreateTable(
                "EXCEED.TransferRequestDepartmentValidation",
                c => new
                    {
                        TransferRequestDepartmentValidationId = c.Int(nullable: false, identity: true),
                        EmployeeTransferRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TransferRequestDepartmentValidationId)
                .ForeignKey("EXCEED.EmployeeTransferRequest", t => t.EmployeeTransferRequestId, cascadeDelete: true)
                .Index(t => t.EmployeeTransferRequestId);
            
            CreateTable(
                "EXCEED.TransferRequestDetail",
                c => new
                    {
                        TransferRequestDetailId = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        JobTitleId = c.Int(nullable: false),
                        EmployeeId = c.Int(),
                        Description = c.String(),
                        EmployeeTransferRequestId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TransferRequestDetailId)
                .ForeignKey("EXCEED.EmployeeTransferRequest", t => t.EmployeeTransferRequestId, cascadeDelete: true)
                .Index(t => t.EmployeeTransferRequestId);
            
            CreateTable(
                "EXCEED.TransferRequestValidation",
                c => new
                    {
                        TransferRequestValidationId = c.Int(nullable: false, identity: true),
                        EmployeeTransferRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TransferRequestValidationId)
                .ForeignKey("EXCEED.EmployeeTransferRequest", t => t.EmployeeTransferRequestId, cascadeDelete: true)
                .Index(t => t.EmployeeTransferRequestId);
            
            CreateTable(
                "EXCEED.InternalCustomerDetail",
                c => new
                    {
                        InternalCustomerDetailId = c.Int(nullable: false, identity: true),
                        TobeEvalutedEmployeeId = c.Int(nullable: false),
                        InternalCustomerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InternalCustomerDetailId)
                .ForeignKey("EXCEED.InternalCustomer", t => t.InternalCustomerId, cascadeDelete: true)
                .Index(t => t.InternalCustomerId);
            
            CreateTable(
                "EXCEED.InternalCustomer",
                c => new
                    {
                        InternalCustomerId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        TobeEvalutedEmployeeId = c.Int(),
                        InternalCustomerPerformanceEvaluationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InternalCustomerId)
                .ForeignKey("EXCEED.InternalCustomerPerformanceEvaluation", t => t.InternalCustomerPerformanceEvaluationId, cascadeDelete: true)
                .Index(t => t.InternalCustomerPerformanceEvaluationId);
            
            CreateTable(
                "EXCEED.InternalCustomerPerformanceEvaluation",
                c => new
                    {
                        InternalCustomerPerformanceEvaluationId = c.Int(nullable: false, identity: true),
                        NumberOfEmployee = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        PerformanceRequestPeriodId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InternalCustomerPerformanceEvaluationId);
            
            CreateTable(
                "EXCEED.InternalCustomerEmployeeMeasurement",
                c => new
                    {
                        InternalCustomerEmployeeMeasurementId = c.Int(nullable: false, identity: true),
                        InternalCustomerEmployeePerformanceId = c.Int(nullable: false),
                        MeasurmentId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InternalCustomerEmployeeMeasurementId);
            
            CreateTable(
                "EXCEED.InternalCustomerEmployeePerformance",
                c => new
                    {
                        InternalCustomerEmployeePerformanceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GlPeriodId = c.Int(nullable: false),
                        EvaluationCriteriaTypeId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApproved = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InternalCustomerEmployeePerformanceId);
            
            CreateTable(
                "EXCEED.InternalCustomerEmployeePerformanceScore",
                c => new
                    {
                        InternalCustomerEmployeePerformanceScoreId = c.Int(nullable: false, identity: true),
                        InternalCustomerEmployeeMeasurementId = c.Int(nullable: false),
                        PerformanceActionId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InternalCustomerEmployeePerformanceScoreId);
            
            CreateTable(
                "EXCEED.LeavePayableDetail",
                c => new
                    {
                        LeavePayableDetailId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(),
                        OrgStructureId = c.Int(nullable: false),
                        EmployeeCode = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        PayableDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Year1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Year2 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LeavePayableId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeavePayableDetailId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EXCEED.LeavePayable", t => t.LeavePayableId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LeavePayableId);
            
            CreateTable(
                "EXCEED.LeavePayable",
                c => new
                    {
                        LeavePayableId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeavePayableId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "EXCEED.LeavePayableSetting",
                c => new
                    {
                        LeavePayableSettingId = c.Int(nullable: false, identity: true),
                        CurrentValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.LeavePayableSettingId);
            
            CreateTable(
                "EXCEED.MainCompanyEvalutionSetting",
                c => new
                    {
                        MainCompanyEvalutionSettingId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(),
                        LookUpId = c.Int(),
                        MeasurementDefinitionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MainCompanyEvalutionSettingId)
                .ForeignKey("Performance.MeasurementDefinition", t => t.MeasurementDefinitionId, cascadeDelete: true)
                .Index(t => t.MeasurementDefinitionId);
            
            CreateTable(
                "EXCEED.NeedAssessmentSummary",
                c => new
                    {
                        NeedAssessmentSummaryId = c.Int(nullable: false, identity: true),
                        NoOfStaff = c.Int(nullable: false),
                        RequiredTraining = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Description = c.String(),
                        Type = c.String(),
                        MainCompanyId = c.Int(),
                        DevelopmentRequired = c.String(),
                        DevelopmentBy = c.String(),
                        Remark = c.String(),
                        Check = c.Boolean(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        CostPerPerson = c.Double(nullable: false),
                        TotalCost = c.Double(nullable: false),
                        IsTransfered = c.Boolean(nullable: false),
                        IsSendForApproval = c.Boolean(nullable: false),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        IsThirdSendForApproval = c.Boolean(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        IsRejected = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.NeedAssessmentSummaryId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompany", t => t.MainCompanyId)
                .Index(t => t.LookupId)
                .Index(t => t.MainCompanyId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "EXCEED.NeedAssessmentSummaryDetail",
                c => new
                    {
                        NeedAssessmentSummaryDetailId = c.Int(nullable: false, identity: true),
                        AssesmentId = c.Int(nullable: false),
                        AssesmentSummaryId = c.Int(nullable: false),
                        Summary_NeedAssessmentSummaryId = c.Int(),
                    })
                .PrimaryKey(t => t.NeedAssessmentSummaryDetailId)
                .ForeignKey("EXCEED.NeedAssessmentSummary", t => t.Summary_NeedAssessmentSummaryId)
                .Index(t => t.Summary_NeedAssessmentSummaryId);
            
            CreateTable(
                "EXCEED.PeerEmployeeMeasurement",
                c => new
                    {
                        PeerEmployeeMeasurementId = c.Int(nullable: false, identity: true),
                        PeerToPeerEmployeePerformanceId = c.Int(nullable: false),
                        MeasurmentId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PeerEmployeeMeasurementId);
            
            CreateTable(
                "EXCEED.PeerToPeerEmployeePerformance",
                c => new
                    {
                        PeerToPeerEmployeePerformanceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GlPeriodId = c.Int(nullable: false),
                        EvaluationCriteriaTypeId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApproved = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PeerToPeerEmployeePerformanceId);
            
            CreateTable(
                "EXCEED.PeerToPeerEmployeePerformanceScore",
                c => new
                    {
                        PeerToPeerEmployeePerformanceScoreId = c.Int(nullable: false, identity: true),
                        PeerEmployeeMeasurementId = c.Int(nullable: false),
                        PerformanceActionId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PeerToPeerEmployeePerformanceScoreId);
            
            CreateTable(
                "EXCEED.PeerToPeerPerformanceEvaluation",
                c => new
                    {
                        PeerToPeerPerformanceEvaluationId = c.Int(nullable: false, identity: true),
                        EvaluatorId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        PerformanceRequestPeriodId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PeerToPeerPerformanceEvaluationId)
                .ForeignKey("EXCEED.PerformanceRequestPeriod", t => t.PerformanceRequestPeriodId, cascadeDelete: true)
                .Index(t => t.PerformanceRequestPeriodId);
            
            CreateTable(
                "EXCEED.PerformanceRequestPeriod",
                c => new
                    {
                        PerformanceRequestPeriodId = c.Int(nullable: false, identity: true),
                        FiscalYearId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceRequestPeriodId);
            
            CreateTable(
                "EXCEED.PerformanceSetting",
                c => new
                    {
                        PerformanceSettingId = c.Int(nullable: false, identity: true),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceSettingId);
            
            CreateTable(
                "EXCEED.SeverancePayableDetail",
                c => new
                    {
                        SeverancePayableDetailId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(),
                        OrgStructureId = c.Int(nullable: false),
                        SeverancePayableId = c.Int(nullable: false),
                        EmployeeCode = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        BasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOFYears = c.String(),
                        GrossComponsation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxWithheldIncomeTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoneTaxableTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxWithheldComTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SpecialSalaryDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDeductions = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeverancePayableDetailId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EXCEED.SeverancePayable", t => t.SeverancePayableId, cascadeDelete: true)
                .Index(t => t.SeverancePayableId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EXCEED.SeverancePayable",
                c => new
                    {
                        SeverancePayableId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeverancePayableId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "EXCEED.TrainingEvaluationAfter",
                c => new
                    {
                        TrainingEvaluationAfterId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TrainingEvaluationCriteriaId = c.Int(nullable: false),
                        TrainingEvaluationSettingCategoryId = c.Int(nullable: false),
                        Criteria = c.String(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.String(),
                        TrainingNeedRequestId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrainingEvaluationAfterId)
                .ForeignKey("EXCEED.TrainingNeedRequest", t => t.TrainingNeedRequestId, cascadeDelete: true)
                .Index(t => t.TrainingNeedRequestId);
            
            CreateTable(
                "EXCEED.TrainingNeedRequest",
                c => new
                    {
                        TrainingNeedRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Prerequest = c.String(nullable: false),
                        EmployeePerformanceGap = c.String(nullable: false),
                        TrainingNeedForEmployeePerformanceGap = c.String(nullable: false),
                        TheNeedForTrainingRelatedToInistitution = c.String(),
                        ExpectedResultFromTraining = c.String(),
                        OrgStructureId = c.Int(nullable: false),
                        RequiredTraining = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        AssesmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrainingNeedRequestId)
                .ForeignKey("HRM.NeedAssessment", t => t.AssesmentId, cascadeDelete: true)
                .Index(t => t.AssesmentId);
            
            CreateTable(
                "EXCEED.TrainingEvaluationBefore",
                c => new
                    {
                        TrainingEvaluationBeforeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TrainingEvaluationCriteriaId = c.Int(nullable: false),
                        TrainingEvaluationSettingCategoryId = c.Int(nullable: false),
                        Criteria = c.String(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.String(),
                        TrainingNeedRequestId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrainingEvaluationBeforeId)
                .ForeignKey("EXCEED.TrainingNeedRequest", t => t.TrainingNeedRequestId, cascadeDelete: true)
                .Index(t => t.TrainingNeedRequestId);
            
            AddColumn("Common.Lookup", "EvalutionType", c => c.Int(nullable: false));
            AddColumn("Common.Lookup", "MainFunction", c => c.String());
            AddColumn("Common.Lookup", "InformationOne", c => c.String());
            AddColumn("Common.Lookup", "InformationTwo", c => c.String());
            AddColumn("Common.Lookup", "TaxTresholdAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Common.Lookup", "Percentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.AttendanceNew", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("HRM.AttendanceNew", "MorningAttendance", c => c.Int(nullable: false));
            AddColumn("HRM.AttendanceNew", "AfternoonAttendance", c => c.Int(nullable: false));
            AddColumn("HRM.MainCompanyStructure", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("HRM.ClearanceProvider", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("HRM.ClearanceProvider", "LookupId", c => c.Int(nullable: false));
            AddColumn("HRM.Committee", "LookupId", c => c.Int(nullable: false));
            //AddColumn("HRM.Competency", "Organization", c => c.String());
            AddColumn("Performance.MeasurementDefinition", "EvalutionType", c => c.Int(nullable: false));
            AddColumn("Performance.MeasurementDefinition", "CriteriaCategoryId", c => c.Int(nullable: false));
            AddColumn("Performance.MeasurementDefinition", "IsMainCompanyLevel", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "DirectBossId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "CriteriaCategoryId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "DeadLine", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Performance.EmployeePerformance", "Date", c => c.DateTime());
            AddColumn("Performance.EmployeePerformance", "BranchId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "ResponsibilityLevel", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "JobTitleId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "IsCorporateApplied", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "IsSectorApplied", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "IsImmediateSuppurvisorApproved", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "IsAcceptedByEmployee", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "IsRejectedByEmployee", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "EmployeeRemark", c => c.String());
            AddColumn("Performance.EmployeePerformance", "IsSecondLevelApproved", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "IsHRApproved", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "IsHRLevelEvalution", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "Comment", c => c.String());
            AddColumn("Performance.EmployeePerformance", "DateCreated", c => c.DateTime());
            AddColumn("Performance.EmployeePerformance", "LastModified", c => c.DateTime());
            AddColumn("Performance.EmployeePerformance", "CreatedBy", c => c.String());
            AddColumn("Performance.EmployeePerformance", "ModifiedBy", c => c.String());
            AddColumn("Performance.EmployeePerformanceScore", "Comment", c => c.String());
            AddColumn("HRM.JobTitleMapping", "BusinessUnitId", c => c.Int());
            AddColumn("HRM.JobTitleMapping", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("HRM.JobTitleMappingTemp", "BusinessUnitId", c => c.Int());
            AddColumn("HRM.JobTitleMappingTemp", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeaveRequest", "BranchId", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveRequest", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("HRM.NeedAssessment", "IsSummarized", c => c.Boolean(nullable: false));
            AddColumn("HRM.NeedAssessment", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.NeedAssessment", "IsThirdSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.Placement", "ParentOrgStructureId", c => c.Int());
            AddColumn("HRM.Placement", "IsTemporary", c => c.Boolean(nullable: false));
            AddColumn("HRM.Placement", "HeadId", c => c.Int());
            AddColumn("HRM.Termination", "IsForciblyTerminated", c => c.Boolean(nullable: false));
            AddColumn("HRM.TrainingEvaluationCriteria", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.TrainingEvaluationCriteria", "Result", c => c.String());
            AlterColumn("HRM.Delegation", "FromDateTimeAm", c => c.String(nullable: false));
            AlterColumn("HRM.DisciplineMeasuresDetail", "Level", c => c.String(nullable: false));
            DropColumn("HRM.ClearanceProvider", "WorkUnit");
            DropColumn("HRM.Committee", "CommitteeType");
            DropColumn("HRM.CommitteMinute", "DisciplinaryCaseId");
            DropColumn("HRM.Cosign", "BirthPlace");
            DropColumn("HRM.Delegation", "GLPeriodId");
        }
        
        public override void Down()
        {
            AddColumn("HRM.Delegation", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("HRM.Cosign", "BirthPlace", c => c.String(nullable: false));
            AddColumn("HRM.CommitteMinute", "DisciplinaryCaseId", c => c.Int(nullable: false));
            AddColumn("HRM.Committee", "CommitteeType", c => c.Int(nullable: false));
            AddColumn("HRM.ClearanceProvider", "WorkUnit", c => c.String());
            DropForeignKey("EXCEED.TrainingEvaluationBefore", "TrainingNeedRequestId", "EXCEED.TrainingNeedRequest");
            DropForeignKey("EXCEED.TrainingEvaluationAfter", "TrainingNeedRequestId", "EXCEED.TrainingNeedRequest");
            DropForeignKey("EXCEED.TrainingNeedRequest", "AssesmentId", "HRM.NeedAssessment");
            DropForeignKey("EXCEED.SeverancePayableDetail", "SeverancePayableId", "EXCEED.SeverancePayable");
            DropForeignKey("EXCEED.SeverancePayable", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("EXCEED.SeverancePayableDetail", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.PeerToPeerPerformanceEvaluation", "PerformanceRequestPeriodId", "EXCEED.PerformanceRequestPeriod");
            DropForeignKey("EXCEED.NeedAssessmentSummaryDetail", "Summary_NeedAssessmentSummaryId", "EXCEED.NeedAssessmentSummary");
            DropForeignKey("EXCEED.NeedAssessmentSummary", "MainCompanyId", "HRM.MainCompany");
            DropForeignKey("EXCEED.NeedAssessmentSummary", "LookupId", "Common.Lookup");
            DropForeignKey("EXCEED.NeedAssessmentSummary", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("EXCEED.MainCompanyEvalutionSetting", "MeasurementDefinitionId", "Performance.MeasurementDefinition");
            DropForeignKey("EXCEED.LeavePayableDetail", "LeavePayableId", "EXCEED.LeavePayable");
            DropForeignKey("EXCEED.LeavePayable", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("EXCEED.LeavePayableDetail", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.InternalCustomerDetail", "InternalCustomerId", "EXCEED.InternalCustomer");
            DropForeignKey("EXCEED.InternalCustomer", "InternalCustomerPerformanceEvaluationId", "EXCEED.InternalCustomerPerformanceEvaluation");
            DropForeignKey("EXCEED.TransferRequestValidation", "EmployeeTransferRequestId", "EXCEED.EmployeeTransferRequest");
            DropForeignKey("EXCEED.TransferRequestDetail", "EmployeeTransferRequestId", "EXCEED.EmployeeTransferRequest");
            DropForeignKey("EXCEED.TransferRequestDepartmentValidation", "EmployeeTransferRequestId", "EXCEED.EmployeeTransferRequest");
            DropIndex("EXCEED.TrainingEvaluationBefore", new[] { "TrainingNeedRequestId" });
            DropIndex("EXCEED.TrainingNeedRequest", new[] { "AssesmentId" });
            DropIndex("EXCEED.TrainingEvaluationAfter", new[] { "TrainingNeedRequestId" });
            DropIndex("EXCEED.SeverancePayable", new[] { "GlFiscalYearId" });
            DropIndex("EXCEED.SeverancePayableDetail", new[] { "EmployeeId" });
            DropIndex("EXCEED.SeverancePayableDetail", new[] { "SeverancePayableId" });
            DropIndex("EXCEED.PeerToPeerPerformanceEvaluation", new[] { "PerformanceRequestPeriodId" });
            DropIndex("EXCEED.NeedAssessmentSummaryDetail", new[] { "Summary_NeedAssessmentSummaryId" });
            DropIndex("EXCEED.NeedAssessmentSummary", new[] { "GlFiscalYearId" });
            DropIndex("EXCEED.NeedAssessmentSummary", new[] { "MainCompanyId" });
            DropIndex("EXCEED.NeedAssessmentSummary", new[] { "LookupId" });
            DropIndex("EXCEED.MainCompanyEvalutionSetting", new[] { "MeasurementDefinitionId" });
            DropIndex("EXCEED.LeavePayable", new[] { "GlFiscalYearId" });
            DropIndex("EXCEED.LeavePayableDetail", new[] { "LeavePayableId" });
            DropIndex("EXCEED.LeavePayableDetail", new[] { "EmployeeId" });
            DropIndex("EXCEED.InternalCustomer", new[] { "InternalCustomerPerformanceEvaluationId" });
            DropIndex("EXCEED.InternalCustomerDetail", new[] { "InternalCustomerId" });
            DropIndex("EXCEED.TransferRequestValidation", new[] { "EmployeeTransferRequestId" });
            DropIndex("EXCEED.TransferRequestDetail", new[] { "EmployeeTransferRequestId" });
            DropIndex("EXCEED.TransferRequestDepartmentValidation", new[] { "EmployeeTransferRequestId" });
            AlterColumn("HRM.DisciplineMeasuresDetail", "Level", c => c.Int(nullable: false));
            AlterColumn("HRM.Delegation", "FromDateTimeAm", c => c.String());
            DropColumn("HRM.TrainingEvaluationCriteria", "Result");
            DropColumn("HRM.TrainingEvaluationCriteria", "Weight");
            DropColumn("HRM.Termination", "IsForciblyTerminated");
            DropColumn("HRM.Placement", "HeadId");
            DropColumn("HRM.Placement", "IsTemporary");
            DropColumn("HRM.Placement", "ParentOrgStructureId");
            DropColumn("HRM.NeedAssessment", "IsThirdSendForApproval");
            DropColumn("HRM.NeedAssessment", "IsSecondSendForApproval");
            DropColumn("HRM.NeedAssessment", "IsSummarized");
            DropColumn("HRM.LeaveRequest", "BusinessUnitId");
            DropColumn("HRM.LeaveRequest", "BranchId");
            DropColumn("HRM.JobTitleMappingTemp", "IsActive");
            DropColumn("HRM.JobTitleMappingTemp", "BusinessUnitId");
            DropColumn("HRM.JobTitleMapping", "IsActive");
            DropColumn("HRM.JobTitleMapping", "BusinessUnitId");
            DropColumn("Performance.EmployeePerformanceScore", "Comment");
            DropColumn("Performance.EmployeePerformance", "ModifiedBy");
            DropColumn("Performance.EmployeePerformance", "CreatedBy");
            DropColumn("Performance.EmployeePerformance", "LastModified");
            DropColumn("Performance.EmployeePerformance", "DateCreated");
            DropColumn("Performance.EmployeePerformance", "Comment");
            DropColumn("Performance.EmployeePerformance", "IsHRLevelEvalution");
            DropColumn("Performance.EmployeePerformance", "IsHRApproved");
            DropColumn("Performance.EmployeePerformance", "IsSecondLevelApproved");
            DropColumn("Performance.EmployeePerformance", "EmployeeRemark");
            DropColumn("Performance.EmployeePerformance", "IsRejectedByEmployee");
            DropColumn("Performance.EmployeePerformance", "IsAcceptedByEmployee");
            DropColumn("Performance.EmployeePerformance", "IsImmediateSuppurvisorApproved");
            DropColumn("Performance.EmployeePerformance", "IsSectorApplied");
            DropColumn("Performance.EmployeePerformance", "IsCorporateApplied");
            DropColumn("Performance.EmployeePerformance", "JobTitleId");
            DropColumn("Performance.EmployeePerformance", "ResponsibilityLevel");
            DropColumn("Performance.EmployeePerformance", "BusinessUnitId");
            DropColumn("Performance.EmployeePerformance", "BranchId");
            DropColumn("Performance.EmployeePerformance", "Date");
            DropColumn("Performance.EmployeePerformance", "DeadLine");
            DropColumn("Performance.EmployeePerformance", "OrgStructureId");
            DropColumn("Performance.EmployeePerformance", "CriteriaCategoryId");
            DropColumn("Performance.EmployeePerformance", "DirectBossId");
            DropColumn("Performance.MeasurementDefinition", "IsMainCompanyLevel");
            DropColumn("Performance.MeasurementDefinition", "CriteriaCategoryId");
            DropColumn("Performance.MeasurementDefinition", "EvalutionType");
            DropColumn("HRM.Competency", "Organization");
            DropColumn("HRM.Committee", "LookupId");
            DropColumn("HRM.ClearanceProvider", "LookupId");
            DropColumn("HRM.ClearanceProvider", "OrgStructureId");
            DropColumn("HRM.MainCompanyStructure", "IsActive");
            DropColumn("HRM.AttendanceNew", "AfternoonAttendance");
            DropColumn("HRM.AttendanceNew", "MorningAttendance");
            DropColumn("HRM.AttendanceNew", "OrgStructureId");
            DropColumn("Common.Lookup", "Percentage");
            DropColumn("Common.Lookup", "TaxTresholdAmount");
            DropColumn("Common.Lookup", "InformationTwo");
            DropColumn("Common.Lookup", "InformationOne");
            DropColumn("Common.Lookup", "MainFunction");
            DropColumn("Common.Lookup", "EvalutionType");
            DropTable("EXCEED.TrainingEvaluationBefore");
            DropTable("EXCEED.TrainingNeedRequest");
            DropTable("EXCEED.TrainingEvaluationAfter");
            DropTable("EXCEED.SeverancePayable");
            DropTable("EXCEED.SeverancePayableDetail");
            DropTable("EXCEED.PerformanceSetting");
            DropTable("EXCEED.PerformanceRequestPeriod");
            DropTable("EXCEED.PeerToPeerPerformanceEvaluation");
            DropTable("EXCEED.PeerToPeerEmployeePerformanceScore");
            DropTable("EXCEED.PeerToPeerEmployeePerformance");
            DropTable("EXCEED.PeerEmployeeMeasurement");
            DropTable("EXCEED.NeedAssessmentSummaryDetail");
            DropTable("EXCEED.NeedAssessmentSummary");
            DropTable("EXCEED.MainCompanyEvalutionSetting");
            DropTable("EXCEED.LeavePayableSetting");
            DropTable("EXCEED.LeavePayable");
            DropTable("EXCEED.LeavePayableDetail");
            DropTable("EXCEED.InternalCustomerEmployeePerformanceScore");
            DropTable("EXCEED.InternalCustomerEmployeePerformance");
            DropTable("EXCEED.InternalCustomerEmployeeMeasurement");
            DropTable("EXCEED.InternalCustomerPerformanceEvaluation");
            DropTable("EXCEED.InternalCustomer");
            DropTable("EXCEED.InternalCustomerDetail");
            DropTable("EXCEED.TransferRequestValidation");
            DropTable("EXCEED.TransferRequestDetail");
            DropTable("EXCEED.TransferRequestDepartmentValidation");
            DropTable("EXCEED.EmployeeTransferRequest");
            DropTable("EXCEED.EmployeeProbationSetting");
            DropTable("EXCEED.EmployeeFolderAttachment");
            DropTable("EXCEED.CriteriaCategory");
            CreateIndex("HRM.Delegation", "GLPeriodId");
            CreateIndex("HRM.CommitteMinute", "DisciplinaryCaseId");
            AddForeignKey("HRM.Delegation", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("HRM.CommitteMinute", "DisciplinaryCaseId", "HRM.DisciplinaryCase", "DisciplinaryCaseId", cascadeDelete: true);
        }
    }
}
