namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class healthandinsuranceupg : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "EXCEED.HealthRankAndValue", newName: "HealthRankAndValues");
            RenameTable(name: "EXCEED.HealthRankAndValueSetting", newName: "HealthRankAndValueSettings");
            RenameTable(name: "APTS.LaboratoryCatagory", newName: "LaboratoryCatagoryies");
            RenameTable(name: "APTS.LaboratoryCatagoryPatientCard", newName: "LaboratoryCatagoryPatientCards");
            RenameTable(name: "APTS.LaboratoryTestPatientCard", newName: "LaboratoryTestPatientCards");
            RenameTable(name: "Health.MedicalRequest", newName: "MedicalRequests");
            MoveTable(name: "EXCEED.HealthRankAndValues", newSchema: "Health");
            MoveTable(name: "EXCEED.HealthRankAndValueSettings", newSchema: "Health");
            MoveTable(name: "APTS.LaboratoryCatagoryies", newSchema: "Health");
            MoveTable(name: "APTS.LaboratoryCatagoryPatientCards", newSchema: "Health");
            MoveTable(name: "APTS.LaboratoryTestPatientCards", newSchema: "Health");
            MoveTable(name: "APTS.LaboratoryTests", newSchema: "Health");
            DropForeignKey("EXCEED.EmployeeAccident", "EmployeeId", "HRM.Employee");
            DropIndex("EXCEED.EmployeeAccident", new[] { "EmployeeId" });
            CreateTable(
                "Insurance.AccidentClaimApprovals",
                c => new
                    {
                        AccidentClaimApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        EquipmentAccidentClaimId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccidentClaimApprovalId)
                .ForeignKey("Insurance.EquipmentAccidentClaims", t => t.EquipmentAccidentClaimId, cascadeDelete: true)
                .Index(t => t.EquipmentAccidentClaimId);
            
            CreateTable(
                "Insurance.EquipmentAccidentClaims",
                c => new
                    {
                        EquipmentAccidentClaimId = c.Int(nullable: false, identity: true),
                        AccidentID = c.String(),
                        AccidentDetail = c.String(nullable: false),
                        FleetsID = c.String(),
                        RequestRecorderEmployeeId = c.Int(nullable: false),
                        AccidentDate = c.DateTime(nullable: false),
                        AccidentTime = c.DateTime(nullable: false),
                        AccidentDateAm = c.String(),
                        Description = c.String(),
                        Machine = c.String(),
                        MachineOperatorName = c.String(),
                        Vehicle = c.String(),
                        PlateNumber = c.String(),
                        DriverName = c.String(),
                        IsCollissionWithAnotherVehicle = c.Boolean(nullable: false),
                        CollidedOwnerName = c.String(),
                        CollidedPlateNo = c.String(),
                        CollidedAddress = c.String(),
                        CollidedPhone = c.String(),
                        InsuranceBranch = c.String(),
                        PaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsVerified = c.Boolean(nullable: false),
                        IsTheDocumentComplete = c.Boolean(nullable: false),
                        IsTheDocumentCompleteByEIC = c.Boolean(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
                        EquipmentType = c.Int(nullable: false),
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
                .PrimaryKey(t => t.EquipmentAccidentClaimId);
            
            CreateTable(
                "Insurance.AccidentClaimDepartmentApprovals",
                c => new
                    {
                        AccidentClaimDepartmentApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        EquipmentAccidentClaimId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccidentClaimDepartmentApprovalId)
                .ForeignKey("Insurance.EquipmentAccidentClaims", t => t.EquipmentAccidentClaimId, cascadeDelete: true)
                .Index(t => t.EquipmentAccidentClaimId);
            
            CreateTable(
                "Insurance.EquipmentAccidentClaimAttachment",
                c => new
                    {
                        EquipmentAccidentClaimAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        EquipmentAccidentClaimId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EquipmentAccidentClaimAttachmentId)
                .ForeignKey("Insurance.EquipmentAccidentClaims", t => t.EquipmentAccidentClaimId, cascadeDelete: true)
                .Index(t => t.EquipmentAccidentClaimId);
            
            CreateTable(
                "Insurance.AccidentCreditServiceApprovals",
                c => new
                    {
                        AccidentCreditServiceApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        EmployeeAccidentCreditServiceId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccidentCreditServiceApprovalId)
                .ForeignKey("Insurance.EmployeeAccidentCreditServices", t => t.EmployeeAccidentCreditServiceId, cascadeDelete: true)
                .Index(t => t.EmployeeAccidentCreditServiceId);
            
            CreateTable(
                "Insurance.EmployeeAccidentCreditServices",
                c => new
                    {
                        EmployeeAccidentCreditServiceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        EmployementType = c.Int(nullable: false),
                        ResponsibilityLevel = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateAm = c.String(nullable: false),
                        Description = c.String(),
                        MedicalCenter = c.String(),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsVerified = c.Boolean(nullable: false),
                        IsTheDocumentComplete = c.Boolean(nullable: false),
                        IsTheDocumentCompleteByEIC = c.Boolean(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.EmployeeAccidentCreditServiceId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Insurance.AccidentCreditServiceAttachments",
                c => new
                    {
                        AccidentCreditServiceAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        EmployeeAccidentCreditServiceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccidentCreditServiceAttachmentId)
                .ForeignKey("Insurance.EmployeeAccidentCreditServices", t => t.EmployeeAccidentCreditServiceId, cascadeDelete: true)
                .Index(t => t.EmployeeAccidentCreditServiceId);
            
            CreateTable(
                "Insurance.AccidentCreditServiceDepartmentApprovals",
                c => new
                    {
                        AccidentCreditServiceDepartmentApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        EmployeeAccidentCreditServiceId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccidentCreditServiceDepartmentApprovalId)
                .ForeignKey("Insurance.EmployeeAccidentCreditServices", t => t.EmployeeAccidentCreditServiceId, cascadeDelete: true)
                .Index(t => t.EmployeeAccidentCreditServiceId);
            
            CreateTable(
                "Insurance.AccidentInsuranceClaimApprovals",
                c => new
                    {
                        AccidentInsuranceClaimApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        EmployeeAccidentInsuranceClaimId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccidentInsuranceClaimApprovalId)
                .ForeignKey("Insurance.EmployeeAccidentInsuranceClaims", t => t.EmployeeAccidentInsuranceClaimId, cascadeDelete: true)
                .Index(t => t.EmployeeAccidentInsuranceClaimId);
            
            CreateTable(
                "Insurance.EmployeeAccidentInsuranceClaims",
                c => new
                    {
                        EmployeeAccidentInsuranceClaimId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        EmployementType = c.Int(nullable: false),
                        ResponsibilityLevel = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateAm = c.String(nullable: false),
                        CauseOfTheAccident = c.String(),
                        TypeOfDamageCaussedByAccident = c.String(),
                        LocationOfAccident = c.String(),
                        OpinionOfImmediateSuperior = c.String(),
                        SalaryDailyOrmonthlyBeforeTtheAccident = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        IsVerifyied = c.Boolean(nullable: false),
                        IsTheDocumentComplete = c.Boolean(nullable: false),
                        IsTheDocumentCompleteByEIC = c.Boolean(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
                        InsuranceType = c.Int(nullable: false),
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
                .PrimaryKey(t => t.EmployeeAccidentInsuranceClaimId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Insurance.AccidentInsuranceClaimAttachments",
                c => new
                    {
                        AccidentInsuranceClaimAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        EmployeeAccidentInsuranceClaimId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccidentInsuranceClaimAttachmentId)
                .ForeignKey("Insurance.EmployeeAccidentInsuranceClaims", t => t.EmployeeAccidentInsuranceClaimId, cascadeDelete: true)
                .Index(t => t.EmployeeAccidentInsuranceClaimId);
            
            CreateTable(
                "Insurance.AccidentInsuranceClaimDepartmentApprovals",
                c => new
                    {
                        AccidentInsuranceClaimDepartmentApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        EmployeeAccidentInsuranceClaimId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccidentInsuranceClaimDepartmentApprovalId)
                .ForeignKey("Insurance.EmployeeAccidentInsuranceClaims", t => t.EmployeeAccidentInsuranceClaimId, cascadeDelete: true)
                .Index(t => t.EmployeeAccidentInsuranceClaimId);
            
            CreateTable(
                "Insurance.EmployeeMedicalExpenseRefunds",
                c => new
                    {
                        EmployeeMedicalExpenseRefundId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        EmployementType = c.Int(nullable: false),
                        ResponsibilityLevel = c.Int(nullable: false),
                        BenefitType = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateAm = c.String(nullable: false),
                        AmountInBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AllowedAmount = c.Decimal(precision: 18, scale: 2),
                        UsedAmount = c.Decimal(precision: 18, scale: 2),
                        RemainingAmount = c.Decimal(precision: 18, scale: 2),
                        TotalExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Deduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReasonForDeduction = c.String(),
                        NetPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfDocument = c.Int(nullable: false),
                        DepartmentAproverId = c.Int(),
                        IsVerifyied = c.Boolean(nullable: false),
                        IsTheDocumentComplete = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.EmployeeMedicalExpenseRefundId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Insurance.FireAndLightningInsuranceApprovals",
                c => new
                    {
                        FireAndLightningInsuranceApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        FireAndLightningInsuranceDetailId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FireAndLightningInsuranceApprovalId)
                .ForeignKey("Insurance.FireAndLightningInsuranceDetails", t => t.FireAndLightningInsuranceDetailId, cascadeDelete: true)
                .Index(t => t.FireAndLightningInsuranceDetailId);
            
            CreateTable(
                "Insurance.FireAndLightningInsuranceDetails",
                c => new
                    {
                        FireAndLightningInsuranceDetailId = c.Int(nullable: false, identity: true),
                        FleetsID = c.String(),
                        PackageValue = c.String(),
                        Remark = c.String(),
                        FireAndLightningInsuranceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FireAndLightningInsuranceDetailId)
                .ForeignKey("Insurance.FireAndLightningInsurance", t => t.FireAndLightningInsuranceId, cascadeDelete: true)
                .Index(t => t.FireAndLightningInsuranceId);
            
            CreateTable(
                "Insurance.FireAndLightningInsuranceDepartmentApprovals",
                c => new
                    {
                        FireAndLightningInsuranceDepartmentApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        FireAndLightningInsuranceDetailId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FireAndLightningInsuranceDepartmentApprovalId)
                .ForeignKey("Insurance.FireAndLightningInsuranceDetails", t => t.FireAndLightningInsuranceDetailId, cascadeDelete: true)
                .Index(t => t.FireAndLightningInsuranceDetailId);
            
            CreateTable(
                "Insurance.FireAndLightningInsurance",
                c => new
                    {
                        FireAndLightningInsuranceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        AccidentDetail = c.String(nullable: false),
                        AccidentDate = c.DateTime(nullable: false),
                        AccidentTime = c.DateTime(nullable: false),
                        AccidentDateAm = c.String(),
                        IsVerified = c.Boolean(nullable: false),
                        IsTheDocumentComplete = c.Boolean(nullable: false),
                        IsTheDocumentCompleteByEIC = c.Boolean(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.FireAndLightningInsuranceId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "Insurance.FireAndLightningInsuranceAttachment",
                c => new
                    {
                        FireAndLightningInsuranceAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        FireAndLightningInsuranceDetailId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FireAndLightningInsuranceAttachmentId)
                .ForeignKey("Insurance.FireAndLightningInsuranceDetails", t => t.FireAndLightningInsuranceDetailId, cascadeDelete: true)
                .Index(t => t.FireAndLightningInsuranceDetailId);
            
            CreateTable(
                "EXCEED.FixedAssetAccidentRecord",
                c => new
                    {
                        FixedAssetAccidentRecordId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        PlateNumber = c.String(),
                        Amount = c.Double(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Remark = c.String(maxLength: 200),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetAccidentRecordId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId)
                .Index(t => t.BranchId);
            
            CreateTable(
                "Health.HealthAttachments",
                c => new
                    {
                        HealthAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        HealthRankAndValueId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HealthAttachmentId)
                .ForeignKey("Health.HealthRankAndValues", t => t.HealthRankAndValueId, cascadeDelete: true)
                .Index(t => t.HealthRankAndValueId);
            
            CreateTable(
                "Health.HealthBenefitSettings",
                c => new
                    {
                        HealthBenefitSettingId = c.Int(nullable: false, identity: true),
                        BenefitType = c.Int(nullable: false),
                        BenfitValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.HealthBenefitSettingId);
            
            CreateTable(
                "Health.HealthExpenseAndTaxSettings",
                c => new
                    {
                        HealthExpenseAndTaxSettingId = c.Int(nullable: false, identity: true),
                        ApSetupItemId = c.Int(nullable: false),
                        GLTaxRateID = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.HealthExpenseAndTaxSettingId)
                .ForeignKey("EXCEED.ApSetupItem", t => t.ApSetupItemId, cascadeDelete: true)
                .ForeignKey("EXCEED.GLTaxRate", t => t.GLTaxRateID, cascadeDelete: true)
                .Index(t => t.ApSetupItemId)
                .Index(t => t.GLTaxRateID);
            
            CreateTable(
                "Health.HealthRankAndValueValidation",
                c => new
                    {
                        HealthRankAndValueValidationId = c.Int(nullable: false, identity: true),
                        HealthRankAndValueId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.HealthRankAndValueValidationId)
                .ForeignKey("Health.HealthRankAndValues", t => t.HealthRankAndValueId, cascadeDelete: true)
                .Index(t => t.HealthRankAndValueId);
            
            CreateTable(
                "Health.HealthResponsiblityLevelValueSettings",
                c => new
                    {
                        HealthResponsiblityLevelValueSettingId = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        CurrentValue = c.String(),
                    })
                .PrimaryKey(t => t.HealthResponsiblityLevelValueSettingId);
            
            CreateTable(
                "Insurance.InsurancePaymentSettings",
                c => new
                    {
                        InsurancePaymentSettingId = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        EmployeeType = c.Int(nullable: false),
                        BenefitType = c.Int(nullable: false),
                        PaymentAmount = c.Int(nullable: false),
                        CurrentValue = c.Decimal(precision: 18, scale: 2),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InsurancePaymentSettingId);
            
            CreateTable(
                "Insurance.InsurancePolicyRenewals",
                c => new
                    {
                        InsurancePolicyRenewalId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        EmployementType = c.Int(nullable: false),
                        ResponsibilityLevel = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateAm = c.String(nullable: false),
                        Description = c.String(),
                        Machine = c.String(),
                        Vehicle = c.String(),
                        PaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsVerified = c.Boolean(nullable: false),
                        IsTheDocumentComplete = c.Boolean(nullable: false),
                        IsTheDocumentCompleteByEIC = c.Boolean(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.InsurancePolicyRenewalId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Insurance.PolicyRenewalDepartmentApprovals",
                c => new
                    {
                        PolicyRenewalDepartmentApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        InsurancePolicyRenewalId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PolicyRenewalDepartmentApprovalId)
                .ForeignKey("Insurance.InsurancePolicyRenewals", t => t.InsurancePolicyRenewalId, cascadeDelete: true)
                .Index(t => t.InsurancePolicyRenewalId);
            
            CreateTable(
                "Insurance.PolicyRenewalApprovals",
                c => new
                    {
                        PolicyRenewalApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        InsurancePolicyRenewalId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PolicyRenewalApprovalId)
                .ForeignKey("Insurance.InsurancePolicyRenewals", t => t.InsurancePolicyRenewalId, cascadeDelete: true)
                .Index(t => t.InsurancePolicyRenewalId);
            
            CreateTable(
                "Insurance.PolicyRenewalAttachments",
                c => new
                    {
                        PolicyRenewalAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        InsurancePolicyRenewalId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PolicyRenewalAttachmentId)
                .ForeignKey("Insurance.InsurancePolicyRenewals", t => t.InsurancePolicyRenewalId, cascadeDelete: true)
                .Index(t => t.InsurancePolicyRenewalId);
            
            CreateTable(
                "Health.MedicalAttachments",
                c => new
                    {
                        MedicalAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        MedicalRequestId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MedicalAttachmentId)
                .ForeignKey("Health.MedicalRequests", t => t.MedicalRequestId, cascadeDelete: true)
                .Index(t => t.MedicalRequestId);
            
            CreateTable(
                "Insurance.MedicalExpenseRefundApprovals",
                c => new
                    {
                        MedicalExpenseRefundApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        AproverId = c.Int(nullable: false),
                        Remark = c.String(),
                        EmployeeMedicalExpenseRefundId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MedicalExpenseRefundApprovalId)
                .ForeignKey("Insurance.EmployeeMedicalExpenseRefunds", t => t.EmployeeMedicalExpenseRefundId, cascadeDelete: true)
                .Index(t => t.EmployeeMedicalExpenseRefundId);
            
            CreateTable(
                "Insurance.MedicalExpenseRefundAttachments",
                c => new
                    {
                        MedicalExpenseRefundAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        EmployeeMedicalExpenseRefundId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MedicalExpenseRefundAttachmentId)
                .ForeignKey("Insurance.EmployeeMedicalExpenseRefunds", t => t.EmployeeMedicalExpenseRefundId, cascadeDelete: true)
                .Index(t => t.EmployeeMedicalExpenseRefundId);
            
            CreateTable(
                "Insurance.MedicalExpenseRefundDepartmentApprovals",
                c => new
                    {
                        MedicalExpenseRefundDepartmentApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        DepartmentAproverId = c.Int(nullable: false),
                        Remark = c.String(),
                        EmployeeMedicalExpenseRefundId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MedicalExpenseRefundDepartmentApprovalId)
                .ForeignKey("Insurance.EmployeeMedicalExpenseRefunds", t => t.EmployeeMedicalExpenseRefundId, cascadeDelete: true)
                .Index(t => t.EmployeeMedicalExpenseRefundId);
            
            CreateTable(
                "Health.PaymentCoverageSettings",
                c => new
                    {
                        PaymentCoverageSettingId = c.Int(nullable: false, identity: true),
                        Payment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.PaymentCoverageSettingId);
            
            CreateTable(
                "EXCEED.VehicleAccidentDetail",
                c => new
                    {
                        VehicleAccidentDetailId = c.Int(nullable: false, identity: true),
                        AccidentType = c.Int(nullable: false),
                        IsThirdPartAccident = c.Boolean(nullable: false),
                        AccidentLevel = c.Int(nullable: false),
                        FixedAssetAccidentRecordId = c.Int(nullable: false),
                        Remark = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.VehicleAccidentDetailId)
                .ForeignKey("EXCEED.FixedAssetAccidentRecord", t => t.FixedAssetAccidentRecordId, cascadeDelete: true)
                .Index(t => t.FixedAssetAccidentRecordId);
            
            AddColumn("EXCEED.EmployeeAccident", "IsOutOFSystem", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.EmployeeAccident", "DailyLabour", c => c.String());
            AddColumn("Health.HealthRankAndValues", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Health.HealthRankAndValues", "PaymentCoverage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Health.HealthRankAndValues", "TotalPaid", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Health.HealthRankAndValues", "StaffContribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Health.HealthRankAndValues", "IsPartnerOrGov", c => c.Boolean(nullable: false));
            AddColumn("Health.HealthRankAndValues", "BenefitType", c => c.Int(nullable: false));
            AddColumn("Health.HealthRankAndValues", "PaymentType", c => c.Int(nullable: false));
            AddColumn("Health.HealthRankAndValues", "IsOtherBenifit", c => c.Boolean(nullable: false));
            AddColumn("Health.HealthRankAndValues", "PaymentRequestId", c => c.Int());
            AddColumn("Health.HealthRankAndValues", "Status", c => c.String());
            AddColumn("Health.HealthRankAndValues", "Remark", c => c.String());
            AddColumn("Health.HealthRankAndValues", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Health.HealthRankAndValues", "VoidBy", c => c.String());
            AddColumn("Health.HealthRankAndValues", "VoidTime", c => c.DateTime());
            AddColumn("Health.HealthRankAndValues", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Health.HealthRankAndValues", "OnlineApprovedBy", c => c.String());
            AddColumn("Health.HealthRankAndValues", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Health.HealthRankAndValues", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Health.HealthRankAndValues", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Health.HealthRankAndValues", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Health.HealthRankAndValues", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Health.HealthRankAndValues", "OrgBranchName", c => c.String());
            AddColumn("Health.HealthRankAndValues", "SendForApprovalBy", c => c.String());
            AddColumn("Health.HealthRankAndValues", "SendForApprovalTime", c => c.DateTime());
            AddColumn("Health.HealthRankAndValues", "DateCreated", c => c.DateTime());
            AddColumn("Health.HealthRankAndValues", "LastModified", c => c.DateTime());
            AddColumn("Health.HealthRankAndValues", "CreatedBy", c => c.String());
            AddColumn("Health.HealthRankAndValues", "ModifiedBy", c => c.String());
            AddColumn("Insurance.Insurance", "EmployeeId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.EmployeeAccident", "OrgStructureId", c => c.Int());
            AlterColumn("EXCEED.EmployeeAccident", "LookupId", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.VehicleAccidentDetail", "FixedAssetAccidentRecordId", "EXCEED.FixedAssetAccidentRecord");
            DropForeignKey("Insurance.MedicalExpenseRefundDepartmentApprovals", "EmployeeMedicalExpenseRefundId", "Insurance.EmployeeMedicalExpenseRefunds");
            DropForeignKey("Insurance.MedicalExpenseRefundAttachments", "EmployeeMedicalExpenseRefundId", "Insurance.EmployeeMedicalExpenseRefunds");
            DropForeignKey("Insurance.MedicalExpenseRefundApprovals", "EmployeeMedicalExpenseRefundId", "Insurance.EmployeeMedicalExpenseRefunds");
            DropForeignKey("Health.MedicalAttachments", "MedicalRequestId", "Health.MedicalRequests");
            DropForeignKey("Insurance.PolicyRenewalAttachments", "InsurancePolicyRenewalId", "Insurance.InsurancePolicyRenewals");
            DropForeignKey("Insurance.PolicyRenewalApprovals", "InsurancePolicyRenewalId", "Insurance.InsurancePolicyRenewals");
            DropForeignKey("Insurance.InsurancePolicyRenewals", "EmployeeId", "HRM.Employee");
            DropForeignKey("Insurance.PolicyRenewalDepartmentApprovals", "InsurancePolicyRenewalId", "Insurance.InsurancePolicyRenewals");
            DropForeignKey("Health.HealthRankAndValueValidation", "HealthRankAndValueId", "Health.HealthRankAndValues");
            DropForeignKey("Health.HealthExpenseAndTaxSettings", "GLTaxRateID", "EXCEED.GLTaxRate");
            DropForeignKey("Health.HealthExpenseAndTaxSettings", "ApSetupItemId", "EXCEED.ApSetupItem");
            DropForeignKey("Health.HealthAttachments", "HealthRankAndValueId", "Health.HealthRankAndValues");
            DropForeignKey("EXCEED.FixedAssetAccidentRecord", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("EXCEED.FixedAssetAccidentRecord", "BranchId", "HRM.Branch");
            DropForeignKey("Insurance.FireAndLightningInsuranceAttachment", "FireAndLightningInsuranceDetailId", "Insurance.FireAndLightningInsuranceDetails");
            DropForeignKey("Insurance.FireAndLightningInsuranceApprovals", "FireAndLightningInsuranceDetailId", "Insurance.FireAndLightningInsuranceDetails");
            DropForeignKey("Insurance.FireAndLightningInsurance", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Insurance.FireAndLightningInsuranceDetails", "FireAndLightningInsuranceId", "Insurance.FireAndLightningInsurance");
            DropForeignKey("Insurance.FireAndLightningInsuranceDepartmentApprovals", "FireAndLightningInsuranceDetailId", "Insurance.FireAndLightningInsuranceDetails");
            DropForeignKey("Insurance.EmployeeMedicalExpenseRefunds", "EmployeeId", "HRM.Employee");
            DropForeignKey("Insurance.EmployeeAccidentInsuranceClaims", "EmployeeId", "HRM.Employee");
            DropForeignKey("Insurance.AccidentInsuranceClaimDepartmentApprovals", "EmployeeAccidentInsuranceClaimId", "Insurance.EmployeeAccidentInsuranceClaims");
            DropForeignKey("Insurance.AccidentInsuranceClaimAttachments", "EmployeeAccidentInsuranceClaimId", "Insurance.EmployeeAccidentInsuranceClaims");
            DropForeignKey("Insurance.AccidentInsuranceClaimApprovals", "EmployeeAccidentInsuranceClaimId", "Insurance.EmployeeAccidentInsuranceClaims");
            DropForeignKey("Insurance.AccidentCreditServiceDepartmentApprovals", "EmployeeAccidentCreditServiceId", "Insurance.EmployeeAccidentCreditServices");
            DropForeignKey("Insurance.AccidentCreditServiceAttachments", "EmployeeAccidentCreditServiceId", "Insurance.EmployeeAccidentCreditServices");
            DropForeignKey("Insurance.AccidentCreditServiceApprovals", "EmployeeAccidentCreditServiceId", "Insurance.EmployeeAccidentCreditServices");
            DropForeignKey("Insurance.EmployeeAccidentCreditServices", "EmployeeId", "HRM.Employee");
            DropForeignKey("Insurance.EquipmentAccidentClaimAttachment", "EquipmentAccidentClaimId", "Insurance.EquipmentAccidentClaims");
            DropForeignKey("Insurance.AccidentClaimDepartmentApprovals", "EquipmentAccidentClaimId", "Insurance.EquipmentAccidentClaims");
            DropForeignKey("Insurance.AccidentClaimApprovals", "EquipmentAccidentClaimId", "Insurance.EquipmentAccidentClaims");
            DropIndex("EXCEED.VehicleAccidentDetail", new[] { "FixedAssetAccidentRecordId" });
            DropIndex("Insurance.MedicalExpenseRefundDepartmentApprovals", new[] { "EmployeeMedicalExpenseRefundId" });
            DropIndex("Insurance.MedicalExpenseRefundAttachments", new[] { "EmployeeMedicalExpenseRefundId" });
            DropIndex("Insurance.MedicalExpenseRefundApprovals", new[] { "EmployeeMedicalExpenseRefundId" });
            DropIndex("Health.MedicalAttachments", new[] { "MedicalRequestId" });
            DropIndex("Insurance.PolicyRenewalAttachments", new[] { "InsurancePolicyRenewalId" });
            DropIndex("Insurance.PolicyRenewalApprovals", new[] { "InsurancePolicyRenewalId" });
            DropIndex("Insurance.PolicyRenewalDepartmentApprovals", new[] { "InsurancePolicyRenewalId" });
            DropIndex("Insurance.InsurancePolicyRenewals", new[] { "EmployeeId" });
            DropIndex("Health.HealthRankAndValueValidation", new[] { "HealthRankAndValueId" });
            DropIndex("Health.HealthExpenseAndTaxSettings", new[] { "GLTaxRateID" });
            DropIndex("Health.HealthExpenseAndTaxSettings", new[] { "ApSetupItemId" });
            DropIndex("Health.HealthAttachments", new[] { "HealthRankAndValueId" });
            DropIndex("EXCEED.FixedAssetAccidentRecord", new[] { "BranchId" });
            DropIndex("EXCEED.FixedAssetAccidentRecord", new[] { "GlFiscalYearId" });
            DropIndex("Insurance.FireAndLightningInsuranceAttachment", new[] { "FireAndLightningInsuranceDetailId" });
            DropIndex("Insurance.FireAndLightningInsurance", new[] { "GlFiscalYearId" });
            DropIndex("Insurance.FireAndLightningInsuranceDepartmentApprovals", new[] { "FireAndLightningInsuranceDetailId" });
            DropIndex("Insurance.FireAndLightningInsuranceDetails", new[] { "FireAndLightningInsuranceId" });
            DropIndex("Insurance.FireAndLightningInsuranceApprovals", new[] { "FireAndLightningInsuranceDetailId" });
            DropIndex("Insurance.EmployeeMedicalExpenseRefunds", new[] { "EmployeeId" });
            DropIndex("Insurance.AccidentInsuranceClaimDepartmentApprovals", new[] { "EmployeeAccidentInsuranceClaimId" });
            DropIndex("Insurance.AccidentInsuranceClaimAttachments", new[] { "EmployeeAccidentInsuranceClaimId" });
            DropIndex("Insurance.EmployeeAccidentInsuranceClaims", new[] { "EmployeeId" });
            DropIndex("Insurance.AccidentInsuranceClaimApprovals", new[] { "EmployeeAccidentInsuranceClaimId" });
            DropIndex("Insurance.AccidentCreditServiceDepartmentApprovals", new[] { "EmployeeAccidentCreditServiceId" });
            DropIndex("Insurance.AccidentCreditServiceAttachments", new[] { "EmployeeAccidentCreditServiceId" });
            DropIndex("Insurance.EmployeeAccidentCreditServices", new[] { "EmployeeId" });
            DropIndex("Insurance.AccidentCreditServiceApprovals", new[] { "EmployeeAccidentCreditServiceId" });
            DropIndex("Insurance.EquipmentAccidentClaimAttachment", new[] { "EquipmentAccidentClaimId" });
            DropIndex("Insurance.AccidentClaimDepartmentApprovals", new[] { "EquipmentAccidentClaimId" });
            DropIndex("Insurance.AccidentClaimApprovals", new[] { "EquipmentAccidentClaimId" });
            AlterColumn("EXCEED.EmployeeAccident", "LookupId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.EmployeeAccident", "OrgStructureId", c => c.Int(nullable: false));
            DropColumn("Insurance.Insurance", "EmployeeId");
            DropColumn("Health.HealthRankAndValues", "ModifiedBy");
            DropColumn("Health.HealthRankAndValues", "CreatedBy");
            DropColumn("Health.HealthRankAndValues", "LastModified");
            DropColumn("Health.HealthRankAndValues", "DateCreated");
            DropColumn("Health.HealthRankAndValues", "SendForApprovalTime");
            DropColumn("Health.HealthRankAndValues", "SendForApprovalBy");
            DropColumn("Health.HealthRankAndValues", "OrgBranchName");
            DropColumn("Health.HealthRankAndValues", "IsSendForApproval");
            DropColumn("Health.HealthRankAndValues", "OnlineTransferredTime");
            DropColumn("Health.HealthRankAndValues", "IsOnlineTransferredBy");
            DropColumn("Health.HealthRankAndValues", "IsOnlineTransferred");
            DropColumn("Health.HealthRankAndValues", "OnlineApprovedTime");
            DropColumn("Health.HealthRankAndValues", "OnlineApprovedBy");
            DropColumn("Health.HealthRankAndValues", "IsOnlineApproved");
            DropColumn("Health.HealthRankAndValues", "VoidTime");
            DropColumn("Health.HealthRankAndValues", "VoidBy");
            DropColumn("Health.HealthRankAndValues", "IsVoid");
            DropColumn("Health.HealthRankAndValues", "Remark");
            DropColumn("Health.HealthRankAndValues", "Status");
            DropColumn("Health.HealthRankAndValues", "PaymentRequestId");
            DropColumn("Health.HealthRankAndValues", "IsOtherBenifit");
            DropColumn("Health.HealthRankAndValues", "PaymentType");
            DropColumn("Health.HealthRankAndValues", "BenefitType");
            DropColumn("Health.HealthRankAndValues", "IsPartnerOrGov");
            DropColumn("Health.HealthRankAndValues", "StaffContribution");
            DropColumn("Health.HealthRankAndValues", "TotalPaid");
            DropColumn("Health.HealthRankAndValues", "PaymentCoverage");
            DropColumn("Health.HealthRankAndValues", "BusinessUnitId");
            DropColumn("EXCEED.EmployeeAccident", "DailyLabour");
            DropColumn("EXCEED.EmployeeAccident", "IsOutOFSystem");
            DropTable("EXCEED.VehicleAccidentDetail");
            DropTable("Health.PaymentCoverageSettings");
            DropTable("Insurance.MedicalExpenseRefundDepartmentApprovals");
            DropTable("Insurance.MedicalExpenseRefundAttachments");
            DropTable("Insurance.MedicalExpenseRefundApprovals");
            DropTable("Health.MedicalAttachments");
            DropTable("Insurance.PolicyRenewalAttachments");
            DropTable("Insurance.PolicyRenewalApprovals");
            DropTable("Insurance.PolicyRenewalDepartmentApprovals");
            DropTable("Insurance.InsurancePolicyRenewals");
            DropTable("Insurance.InsurancePaymentSettings");
            DropTable("Health.HealthResponsiblityLevelValueSettings");
            DropTable("Health.HealthRankAndValueValidation");
            DropTable("Health.HealthExpenseAndTaxSettings");
            DropTable("Health.HealthBenefitSettings");
            DropTable("Health.HealthAttachments");
            DropTable("EXCEED.FixedAssetAccidentRecord");
            DropTable("Insurance.FireAndLightningInsuranceAttachment");
            DropTable("Insurance.FireAndLightningInsurance");
            DropTable("Insurance.FireAndLightningInsuranceDepartmentApprovals");
            DropTable("Insurance.FireAndLightningInsuranceDetails");
            DropTable("Insurance.FireAndLightningInsuranceApprovals");
            DropTable("Insurance.EmployeeMedicalExpenseRefunds");
            DropTable("Insurance.AccidentInsuranceClaimDepartmentApprovals");
            DropTable("Insurance.AccidentInsuranceClaimAttachments");
            DropTable("Insurance.EmployeeAccidentInsuranceClaims");
            DropTable("Insurance.AccidentInsuranceClaimApprovals");
            DropTable("Insurance.AccidentCreditServiceDepartmentApprovals");
            DropTable("Insurance.AccidentCreditServiceAttachments");
            DropTable("Insurance.EmployeeAccidentCreditServices");
            DropTable("Insurance.AccidentCreditServiceApprovals");
            DropTable("Insurance.EquipmentAccidentClaimAttachment");
            DropTable("Insurance.AccidentClaimDepartmentApprovals");
            DropTable("Insurance.EquipmentAccidentClaims");
            DropTable("Insurance.AccidentClaimApprovals");
            CreateIndex("EXCEED.EmployeeAccident", "EmployeeId");
            AddForeignKey("EXCEED.EmployeeAccident", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            MoveTable(name: "Health.LaboratoryTests", newSchema: "APTS");
            MoveTable(name: "Health.LaboratoryTestPatientCards", newSchema: "APTS");
            MoveTable(name: "Health.LaboratoryCatagoryPatientCards", newSchema: "APTS");
            MoveTable(name: "Health.LaboratoryCatagoryies", newSchema: "APTS");
            MoveTable(name: "Health.HealthRankAndValueSettings", newSchema: "EXCEED");
            MoveTable(name: "Health.HealthRankAndValues", newSchema: "EXCEED");
            RenameTable(name: "Health.MedicalRequests", newName: "MedicalRequest");
            RenameTable(name: "APTS.LaboratoryTestPatientCards", newName: "LaboratoryTestPatientCard");
            RenameTable(name: "APTS.LaboratoryCatagoryPatientCards", newName: "LaboratoryCatagoryPatientCard");
            RenameTable(name: "APTS.LaboratoryCatagoryies", newName: "LaboratoryCatagory");
            RenameTable(name: "EXCEED.HealthRankAndValueSettings", newName: "HealthRankAndValueSetting");
            RenameTable(name: "EXCEED.HealthRankAndValues", newName: "HealthRankAndValue");
        }
    }
}
