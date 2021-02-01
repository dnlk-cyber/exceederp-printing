namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialrevolution : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.Absence",
                c => new
                    {
                        AbsenceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        NoOfWorkingDays = c.Int(nullable: false),
                        Reason = c.String(),
                        MeasureTaken = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AbsenceId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.Employee",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(nullable: false),
                        Title = c.Int(nullable: false),
                        FirstName = c.String(nullable: false),
                        MiddleName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Nationality = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Gender = c.Int(nullable: false),
                        CategoryID = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        MothersFullName = c.String(),
                        BirthPlace = c.String(nullable: false),
                        Nation = c.String(),
                        Region = c.Int(nullable: false),
                        Religion = c.Int(nullable: false),
                        MartialStatus = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        HiredDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "EXCEED.UserCardItems",
                c => new
                    {
                        UserCardItemsID = c.Int(nullable: false, identity: true),
                        PersonID = c.String(),
                        EmployeeFullName = c.String(),
                        ItemDesc = c.String(),
                        UOM = c.String(),
                        Quanity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TagNumber = c.String(),
                        SerialPartNo = c.String(),
                        IssueDate = c.DateTime(nullable: false),
                        SIVNo = c.String(),
                        ReturnDate = c.DateTime(),
                        SRNNo = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        UserCard_EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.UserCardItemsID)
                .ForeignKey("HRM.Employee", t => t.UserCard_EmployeeId)
                .Index(t => t.UserCard_EmployeeId);
            
            CreateTable(
                "Insurance.AccidentMedicalRecord",
                c => new
                    {
                        AccidentMedicalRecordId = c.Int(nullable: false, identity: true),
                        AccidentRecordId = c.Int(nullable: false),
                        MedicalTreatment = c.String(),
                        InstitutionName = c.String(),
                        MedicalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ramrk = c.String(),
                    })
                .PrimaryKey(t => t.AccidentMedicalRecordId)
                .ForeignKey("Insurance.AccidentRecord", t => t.AccidentRecordId, cascadeDelete: true)
                .Index(t => t.AccidentRecordId);
            
            CreateTable(
                "Insurance.AccidentRecord",
                c => new
                    {
                        AccidentRecordId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        DetailDescription = c.String(),
                        AccidentPlace = c.String(),
                        Witness = c.String(),
                        IsInWorkPlace = c.Boolean(nullable: false),
                        IsInOnJob = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AccidentRecordId);
            
            CreateTable(
                "Payroll.Addition",
                c => new
                    {
                        AdditionId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LookupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdditionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "common.Lookup",
                c => new
                    {
                        LookupId = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Category = c.String(),
                        Index = c.Int(nullable: false),
                        LookupType = c.Int(nullable: false),
                        Description = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LookupId);
            
            CreateTable(
                "common.Period",
                c => new
                    {
                        PeriodId = c.Int(nullable: false, identity: true),
                        PeriodName = c.String(nullable: false),
                        PeriodType = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        ParentId = c.Int(),
                        Index = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PeriodId);
            
            CreateTable(
                "common.Address",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        ReferenceType = c.String(),
                        Reference = c.String(),
                        Category = c.String(),
                        Value = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AddressId);
            
            CreateTable(
                "Payroll.Advance",
                c => new
                    {
                        AdvanceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        AdvanceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Months = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        JumpHoliday = c.Boolean(nullable: false),
                        Description = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AdvanceId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "HRM.AllowanceDefination",
                c => new
                    {
                        AllowanceDefinationId = c.Int(nullable: false, identity: true),
                        AllowanceName = c.String(nullable: false),
                        AllowanceType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AllowanceDefinationId);
            
            CreateTable(
                "HRM.AllowancePerEmployee",
                c => new
                    {
                        AllowanceId = c.Int(nullable: false, identity: true),
                        AllowanceDefinationId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AllowanceId)
                .ForeignKey("HRM.AllowanceDefination", t => t.AllowanceDefinationId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.AllowanceDefinationId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.AllowanceTypeRule",
                c => new
                    {
                        AllowanceTypeRuleId = c.Int(nullable: false, identity: true),
                        AllowanceType = c.Int(nullable: false),
                        TaxTrasholdAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Percentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AllowanceTypeRuleId);
            
            CreateTable(
                "EXCEED.AnnualNeedAssesmentItem",
                c => new
                    {
                        AnnualNeedAssesmentItemID = c.Int(nullable: false, identity: true),
                        AnnualNeedAssesmentID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        Function = c.String(),
                        PerformanceRequirement = c.String(),
                        PhysicalCharacteristics = c.String(),
                        QuantityRequired = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinimumQualityRequired = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BudgetSource = c.String(),
                        ProcurementCategory = c.Int(nullable: false),
                        EstimatedBudget = c.Decimal(precision: 18, scale: 2),
                        EstimatedTimeTheItemIsNeeded = c.DateTime(nullable: false),
                        Uom = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualNeedAssesmentItemID)
                .ForeignKey("EXCEED.AnnualNeedAssesment", t => t.AnnualNeedAssesmentID, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .Index(t => t.AnnualNeedAssesmentID)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "EXCEED.AnnualNeedAssesment",
                c => new
                    {
                        AnnualNeedAssesmentID = c.Int(nullable: false, identity: true),
                        Branch = c.String(),
                        RequestingDepartment = c.String(),
                        RequestingName = c.String(),
                        Remark = c.String(),
                        GLPeriodId = c.Int(nullable: false),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualNeedAssesmentID)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.GLPeriodId);
            
            CreateTable(
                "EXCEED.AnnualNeedAssesmentValidation",
                c => new
                    {
                        AnnualNeedAssesmentValidationID = c.Int(nullable: false, identity: true),
                        AnnualNeedAssesmentID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualNeedAssesmentValidationID)
                .ForeignKey("EXCEED.AnnualNeedAssesment", t => t.AnnualNeedAssesmentID, cascadeDelete: true)
                .Index(t => t.AnnualNeedAssesmentID);
            
            CreateTable(
                "EXCEED.GLPeriod",
                c => new
                    {
                        GLPeriodId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLPeriodId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "EXCEED.GlFiscalYear",
                c => new
                    {
                        GlFiscalYearId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlFiscalYearId);
            
            CreateTable(
                "EXCEED.InventoryItem",
                c => new
                    {
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        ItemCategoryCode = c.String(maxLength: 128),
                        Name = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        UoMName = c.String(maxLength: 128),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvgPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .ForeignKey("EXCEED.UoM", t => t.UoMName)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.UoMName);
            
            CreateTable(
                "EXCEED.ItemCategory",
                c => new
                    {
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        AccountCode = c.String(nullable: false),
                        IssueAccountCode = c.String(nullable: false),
                        HaveParent = c.Boolean(nullable: false),
                        IsFixedAsset = c.Boolean(nullable: false),
                        ParentName = c.String(),
                    })
                .PrimaryKey(t => t.ItemCategoryCode);
            
            CreateTable(
                "EXCEED.StoreItemAssignment",
                c => new
                    {
                        StoreItemAssignmentID = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(nullable: false, maxLength: 128),
                        ItemCategoryCode = c.String(nullable: false),
                        ReOrderPoint = c.Single(nullable: false),
                        MinPoint = c.Single(nullable: false),
                        MaxPoint = c.Single(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemCode = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.StoreItemAssignmentID)
                .ForeignKey("EXCEED.InventoryStore", t => t.StoreCode, cascadeDelete: true)
                .Index(t => t.StoreCode);
            
            CreateTable(
                "EXCEED.BinCardLine",
                c => new
                    {
                        BinCardLineID = c.Int(nullable: false, identity: true),
                        StoreItemAssignmentID = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        ReferenceDoc = c.String(),
                        ReceivedQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedUPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedTPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedUPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedTPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceUPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceTPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.BinCardLineID)
                .ForeignKey("EXCEED.StoreItemAssignment", t => t.StoreItemAssignmentID, cascadeDelete: true)
                .Index(t => t.StoreItemAssignmentID);
            
            CreateTable(
                "EXCEED.InventoryStore",
                c => new
                    {
                        StoreCode = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Branch = c.String(nullable: false),
                        Location = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreCode);
            
            CreateTable(
                "EXCEED.StoreKeeperAssignment",
                c => new
                    {
                        StoreKeeperAssignmentID = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(nullable: false, maxLength: 128),
                        StoreKeeper = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.StoreKeeperAssignmentID)
                .ForeignKey("EXCEED.InventoryStore", t => t.StoreCode, cascadeDelete: true)
                .Index(t => t.StoreCode);
            
            CreateTable(
                "EXCEED.UoM",
                c => new
                    {
                        UoMName = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.UoMName);
            
            CreateTable(
                "EXCEED.APBusinessType",
                c => new
                    {
                        APBusinessTypeID = c.Int(nullable: false, identity: true),
                        TypeOfBusiness = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.APBusinessTypeID);
            
            CreateTable(
                "EXCEED.ApMiscellaneousPaymentDistribution",
                c => new
                    {
                        ApMiscellaneousPaymentDistributionId = c.Int(nullable: false, identity: true),
                        ApMiscellaneousPaymentId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApMiscellaneousPaymentDistributionId)
                .ForeignKey("EXCEED.ApMiscellaneousPayment", t => t.ApMiscellaneousPaymentId, cascadeDelete: true)
                .Index(t => t.ApMiscellaneousPaymentId);
            
            CreateTable(
                "EXCEED.ApMiscellaneousPayment",
                c => new
                    {
                        ApMiscellaneousPaymentId = c.Int(nullable: false, identity: true),
                        Purpose = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountCode = c.String(),
                        TaxType = c.String(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdingType = c.String(nullable: false),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdTax = c.Boolean(nullable: false),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPayable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(nullable: false),
                        PaymentReference = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        OnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApMiscellaneousPaymentId);
            
            CreateTable(
                "EXCEED.ApMiscellaneousPaymentValidation",
                c => new
                    {
                        ApMiscellaneousPaymentValidationId = c.Int(nullable: false, identity: true),
                        ApMiscellaneousPaymentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApMiscellaneousPaymentValidationId)
                .ForeignKey("EXCEED.ApMiscellaneousPayment", t => t.ApMiscellaneousPaymentId, cascadeDelete: true)
                .Index(t => t.ApMiscellaneousPaymentId);
            
            CreateTable(
                "EXCEED.ApPaybleInvoiceDistribution",
                c => new
                    {
                        ApPaybleInvoiceDistributionId = c.Int(nullable: false, identity: true),
                        ApPaybleInvoicesId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaybleInvoiceDistributionId)
                .ForeignKey("EXCEED.ApPaybleInvoices", t => t.ApPaybleInvoicesId, cascadeDelete: true)
                .Index(t => t.ApPaybleInvoicesId);
            
            CreateTable(
                "EXCEED.ApPaybleInvoices",
                c => new
                    {
                        ApPaybleInvoicesId = c.Int(nullable: false, identity: true),
                        SupplierName = c.String(nullable: false),
                        SupplierId = c.String(),
                        Type = c.Int(nullable: false),
                        PoNum = c.String(),
                        PurposeOfPayment = c.String(),
                        InvoiceDate = c.DateTime(nullable: false),
                        InvoiceNumber = c.String(),
                        PaymentTerms = c.Int(nullable: false),
                        InvoiceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvanceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPayable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceStatus = c.Int(nullable: false),
                        IsSendForApproval = c.Boolean(nullable: false),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        OnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        APSupplierRegistration_APSupplierRegistrationID = c.Int(),
                    })
                .PrimaryKey(t => t.ApPaybleInvoicesId)
                .ForeignKey("EXCEED.APSupplierRegistration", t => t.APSupplierRegistration_APSupplierRegistrationID)
                .Index(t => t.APSupplierRegistration_APSupplierRegistrationID);
            
            CreateTable(
                "EXCEED.ApPaybleInvoicesLines",
                c => new
                    {
                        ApPaybleInvoicesLinesId = c.Int(nullable: false, identity: true),
                        ApPaybleInvoicesId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        InvoiceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvanceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPayable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceiptNo = c.String(),
                        TaxType = c.String(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdingType = c.String(nullable: false),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdTax = c.Boolean(nullable: false),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PoNo = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaybleInvoicesLinesId)
                .ForeignKey("EXCEED.ApPaybleInvoices", t => t.ApPaybleInvoicesId, cascadeDelete: true)
                .Index(t => t.ApPaybleInvoicesId);
            
            CreateTable(
                "EXCEED.ApPaybleInvoiceValidation",
                c => new
                    {
                        ApPaybleInvoiceValidationId = c.Int(nullable: false, identity: true),
                        ApPaybleInvoicesId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaybleInvoiceValidationId)
                .ForeignKey("EXCEED.ApPaybleInvoices", t => t.ApPaybleInvoicesId, cascadeDelete: true)
                .Index(t => t.ApPaybleInvoicesId);
            
            CreateTable(
                "EXCEED.APPaymentsViewModel",
                c => new
                    {
                        APPaymentsViewModelID = c.Int(nullable: false, identity: true),
                        SupplierName = c.String(),
                        SupplierID = c.String(),
                        InvoiceNumber = c.String(),
                        InvoiceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(nullable: false),
                        PaymentReference = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        Status = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ApPaybleInvoices_ApPaybleInvoicesId = c.Int(),
                    })
                .PrimaryKey(t => t.APPaymentsViewModelID)
                .ForeignKey("EXCEED.ApPaybleInvoices", t => t.ApPaybleInvoices_ApPaybleInvoicesId)
                .Index(t => t.ApPaybleInvoices_ApPaybleInvoicesId);
            
            CreateTable(
                "EXCEED.ApPaymentDistribution",
                c => new
                    {
                        ApPaymentDistributionId = c.Int(nullable: false, identity: true),
                        ApPaymentId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaymentDistributionId)
                .ForeignKey("EXCEED.ApPayment", t => t.ApPaymentId, cascadeDelete: true)
                .Index(t => t.ApPaymentId);
            
            CreateTable(
                "EXCEED.ApPayment",
                c => new
                    {
                        ApPaymentId = c.Int(nullable: false, identity: true),
                        IsPrepayment = c.Boolean(nullable: false),
                        SupplierName = c.String(nullable: false),
                        SupplierId = c.String(),
                        InvoiceNumber = c.String(),
                        InvoiceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(nullable: false),
                        PaymentReference = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        OnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        NetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HasPrepayment = c.Boolean(nullable: false),
                        PrepaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaymentId);
            
            CreateTable(
                "EXCEED.ApPaymentValidation",
                c => new
                    {
                        ApPaymentValidationId = c.Int(nullable: false, identity: true),
                        ApPaymentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaymentValidationId)
                .ForeignKey("EXCEED.ApPayment", t => t.ApPaymentId, cascadeDelete: true)
                .Index(t => t.ApPaymentId);
            
            CreateTable(
                "HRM.ApplicantExamResult",
                c => new
                    {
                        ApplicantResultId = c.Int(nullable: false, identity: true),
                        ApplicantRegistrationId = c.Int(nullable: false),
                        Interview = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TheoryExam = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PracticalExam = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReferenceType = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ApplicantResultId);
            
            CreateTable(
                "HRM.ApplicantExperience",
                c => new
                    {
                        ApplicantExpId = c.Int(nullable: false, identity: true),
                        ApplicantRegistrationId = c.Int(nullable: false),
                        EduLevel = c.Int(nullable: false),
                        EducField = c.Int(nullable: false),
                        OrganizationName = c.String(nullable: false),
                        Postion = c.String(nullable: false),
                        Experience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ApplicantExpId)
                .ForeignKey("HRM.ApplicantRegistration", t => t.ApplicantRegistrationId, cascadeDelete: true)
                .Index(t => t.ApplicantRegistrationId);
            
            CreateTable(
                "HRM.ApplicantRegistration",
                c => new
                    {
                        ApplicantRegistrationId = c.Int(nullable: false, identity: true),
                        VaccancyCode = c.String(nullable: false, maxLength: 128),
                        ApplicantName = c.String(nullable: false),
                        Age = c.Int(nullable: false),
                        Sex = c.Int(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                        RegistrationPalce = c.String(nullable: false),
                        RegisterBy = c.String(nullable: false),
                        TotalResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ApplicantRegistrationId)
                .ForeignKey("HRM.Vacancy", t => t.VaccancyCode, cascadeDelete: true)
                .Index(t => t.VaccancyCode);
            
            CreateTable(
                "HRM.Vacancy",
                c => new
                    {
                        VaccancyCode = c.String(nullable: false, maxLength: 128),
                        RecruitmentPlanId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        LookupId = c.Int(nullable: false),
                        RecruSource = c.Int(nullable: false),
                        Location = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.VaccancyCode)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.RecruitmentPlan", t => t.RecruitmentPlanId, cascadeDelete: true)
                .Index(t => t.RecruitmentPlanId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "HRM.RecruitmentPlan",
                c => new
                    {
                        RecruitmentPlanId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        NoOfVacantPost = c.Int(nullable: false),
                        EduLevel = c.Int(nullable: false),
                        RelevantExp = c.String(),
                        Experience = c.String(),
                        SpecialSkill = c.String(),
                        PlannedSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RecruiTime = c.DateTime(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        RecruitmentType = c.Int(nullable: false),
                        RankId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentPlanId)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .ForeignKey("HRM.Rank", t => t.RankId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.RankId);
            
            CreateTable(
                "HRM.MainCompanyStructure",
                c => new
                    {
                        OrgStructureId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StructureId = c.Int(nullable: false),
                        Parent = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.OrgStructureId)
                .ForeignKey("HRM.Structure", t => t.StructureId, cascadeDelete: true)
                .Index(t => t.StructureId);
            
            CreateTable(
                "HRM.Structure",
                c => new
                    {
                        StructureId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.StructureId);
            
            CreateTable(
                "HRM.Rank",
                c => new
                    {
                        RankId = c.Int(nullable: false, identity: true),
                        RankName = c.String(nullable: false),
                        StartingSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaximumSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RankId);
            
            CreateTable(
                "HRM.ApplicantionForm",
                c => new
                    {
                        ApplicantionFormId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        ApplicationType = c.String(),
                        VaccancyCode = c.String(nullable: false, maxLength: 128),
                        Remark = c.String(),
                        TotalResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ApplicantionFormId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("HRM.Vacancy", t => t.VaccancyCode, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.VaccancyCode);
            
            CreateTable(
                "HRM.ApplicantScreeningResult",
                c => new
                    {
                        ScreeningResultId = c.Int(nullable: false, identity: true),
                        ApplicantRegistrationId = c.Int(nullable: false),
                        EduLevel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EduField = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RelevantExperience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RelatedExperience = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Gender = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Disability = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Profile = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Training = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discipline = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReferenceType = c.String(),
                        Remark = c.String(),
                        TotalResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ScreeningResultId);
            
            CreateTable(
                "HRM.ApplicantTraining",
                c => new
                    {
                        ApplicantTrainingId = c.Int(nullable: false, identity: true),
                        ApplicantRegistrationId = c.Int(nullable: false),
                        TrainingName = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ApplicantTrainingId)
                .ForeignKey("HRM.ApplicantRegistration", t => t.ApplicantRegistrationId, cascadeDelete: true)
                .Index(t => t.ApplicantRegistrationId);
            
            CreateTable(
                "EXCEED.Appreciation",
                c => new
                    {
                        AppreciationID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RegistrationId = c.Int(nullable: false),
                        AppreciationDate = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AppreciationID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.RegistrationId, cascadeDelete: true)
                .Index(t => t.RegistrationId);
            
            CreateTable(
                "EXCEED.AssetRegistration",
                c => new
                    {
                        RegistrationID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Tag = c.String(),
                        SIV = c.String(),
                        PurchaseDate = c.DateTime(),
                        PurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GRN = c.String(),
                        GRNDate = c.DateTime(),
                        OriginalValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        IsMachinery = c.Boolean(nullable: false),
                        Supplier = c.String(),
                        Manufacturer = c.String(),
                        SerialNo = c.String(),
                        PlateNo = c.String(),
                        PlateType = c.String(),
                        ChasisNo = c.String(),
                        MotorNo = c.String(),
                        ModelNo = c.String(),
                        StoreCode = c.String(),
                        LoadCapacity = c.String(),
                        FuelType = c.String(),
                        ManufacturDate = c.Int(nullable: false),
                        NetWeight = c.String(),
                        TotalWeight = c.String(),
                        Battery = c.String(),
                        Tyre = c.String(),
                        IsBuilding = c.Boolean(nullable: false),
                        Location = c.String(),
                        CostCenter = c.String(),
                        Contractor = c.String(),
                        ConstructionStartDate = c.DateTime(),
                        DepretiationRuleId = c.Int(nullable: false),
                        ConstructionEndDate = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RegistrationID)
                .ForeignKey("EXCEED.DepreciationRule", t => t.DepretiationRuleId, cascadeDelete: true)
                .Index(t => t.DepretiationRuleId);
            
            CreateTable(
                "EXCEED.AnnualDepreciation",
                c => new
                    {
                        DepreciationID = c.Int(nullable: false, identity: true),
                        RegistrationID = c.Int(nullable: false),
                        DepreciationTime = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OriginalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Accomodate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DepreciationID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.RegistrationID, cascadeDelete: true)
                .Index(t => t.RegistrationID);
            
            CreateTable(
                "EXCEED.DepreciationRule",
                c => new
                    {
                        DepretiationRuleId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(maxLength: 128),
                        SubCategoryCode = c.String(),
                        AccountCode = c.String(),
                        DepretiationCode = c.String(),
                        ClearingAccount = c.String(),
                        FixedAssetGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepretiationRuleId)
                .ForeignKey("EXCEED.FixedAssetGroup", t => t.FixedAssetGroupID, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.FixedAssetGroupID);
            
            CreateTable(
                "EXCEED.FixedAssetGroup",
                c => new
                    {
                        FixedAssetGroupID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DepreciationMethod = c.Int(nullable: false),
                        YearlyDepreciationPercentage = c.Single(nullable: false),
                        SalvageValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetGroupID);
            
            CreateTable(
                "EXCEED.Disposal",
                c => new
                    {
                        DisposalID = c.Int(nullable: false, identity: true),
                        RegistrationID = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DisposalID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.RegistrationID, cascadeDelete: true)
                .Index(t => t.RegistrationID);
            
            CreateTable(
                "EXCEED.Location",
                c => new
                    {
                        LocationID = c.Int(nullable: false, identity: true),
                        RegistrationID = c.Int(nullable: false),
                        LocationUser = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LocationID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.RegistrationID, cascadeDelete: true)
                .Index(t => t.RegistrationID);
            
            //CreateTable(
            //    "EXCEED.Revaluation",
            //    c => new
            //        {
            //            RevaluationID = c.Int(nullable: false, identity: true),
            //            RegistrationID = c.Int(nullable: false),
            //            AddedValue = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            Description = c.String(),
            //            RevaluationDate = c.DateTime(nullable: false),
            //            OrgBranchName = c.String(),
            //            DateCreated = c.DateTime(),
            //            LastModified = c.DateTime(),
            //            CreatedBy = c.String(),
            //            ModifiedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.RevaluationID)
            //    .ForeignKey("EXCEED.AssetRegistration", t => t.RegistrationID, cascadeDelete: true)
            //    .Index(t => t.RegistrationID);
            
            CreateTable(
                "EXCEED.Sales",
                c => new
                    {
                        SalesID = c.Int(nullable: false, identity: true),
                        RegistrationID = c.Int(nullable: false),
                        Amount = c.String(nullable: false),
                        TaxAmount = c.String(),
                        TotalAmount = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.RegistrationID, cascadeDelete: true)
                .Index(t => t.RegistrationID);
            
            CreateTable(
                "EXCEED.ApSetupItem",
                c => new
                    {
                        ApSetupItemId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        CreationSign = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApSetupItemId);
            
            CreateTable(
                "EXCEED.ApSetupReceiptBalanceAccount",
                c => new
                    {
                        ApSetupReceiptBalanceAccountId = c.Int(nullable: false, identity: true),
                        PrepaymentAccountCode = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ApSetupReceiptBalanceAccountId);
            
            CreateTable(
                "EXCEED.APsupplierBlacklist",
                c => new
                    {
                        APsupplierBlacklistID = c.Int(nullable: false, identity: true),
                        APSupplierRegistrationID = c.Int(nullable: false),
                        Opertion = c.String(),
                        Date = c.DateTime(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.APsupplierBlacklistID)
                .ForeignKey("EXCEED.APSupplierRegistration", t => t.APSupplierRegistrationID, cascadeDelete: true)
                .Index(t => t.APSupplierRegistrationID);
            
            CreateTable(
                "EXCEED.APSupplierRegistration",
                c => new
                    {
                        APSupplierRegistrationID = c.Int(nullable: false, identity: true),
                        SupplierName = c.String(nullable: false),
                        Account = c.String(nullable: false),
                        PhoneNo = c.String(),
                        TINNo = c.String(nullable: false, maxLength: 10),
                        Address1 = c.String(nullable: false),
                        Address2 = c.String(),
                        ContactName = c.String(nullable: false),
                        Country = c.String(),
                        SupplierType = c.String(),
                        BusinessType = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.APSupplierRegistrationID);
            
            CreateTable(
                "EXCEED.APSupplierType",
                c => new
                    {
                        APSupplierTypeID = c.Int(nullable: false, identity: true),
                        Supplier = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.APSupplierTypeID);
            
            CreateTable(
                "EXCEED.ArCustomerProfile",
                c => new
                    {
                        ArCustomerProfileId = c.Int(nullable: false, identity: true),
                        CustomerId = c.String(nullable: false),
                        CompanyName = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        TinNo = c.String(maxLength: 10),
                        CustomerType = c.String(nullable: false),
                        ContactName = c.String(nullable: false),
                        PhoneNumber = c.String(),
                        Address = c.String(nullable: false),
                        Country = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArCustomerProfileId);
            
            CreateTable(
                "EXCEED.ArInvoiceViewModel",
                c => new
                    {
                        ArInvoiceViewModelId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        CustomerId = c.String(),
                        TransactionClass = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PaymentTerm = c.String(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        LineAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceStatus = c.String(),
                        ArCustomerProfile_ArCustomerProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.ArInvoiceViewModelId)
                .ForeignKey("EXCEED.ArCustomerProfile", t => t.ArCustomerProfile_ArCustomerProfileId)
                .Index(t => t.ArCustomerProfile_ArCustomerProfileId);
            
            CreateTable(
                "EXCEED.ArCustomerType",
                c => new
                    {
                        ArCustomerTypeId = c.Int(nullable: false, identity: true),
                        CustomerTypeName = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArCustomerTypeId);
            
            CreateTable(
                "EXCEED.ArInvoiceDistribution",
                c => new
                    {
                        ArInvoiceDistributionId = c.Int(nullable: false, identity: true),
                        ArInvoiceId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArInvoiceDistributionId)
                .ForeignKey("EXCEED.ArInvoice", t => t.ArInvoiceId, cascadeDelete: true)
                .Index(t => t.ArInvoiceId);
            
            CreateTable(
                "EXCEED.ArInvoice",
                c => new
                    {
                        ArInvoiceId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(nullable: false),
                        CustomerId = c.String(),
                        TransactionClass = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PaymentTerm = c.String(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        LineAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceStatus = c.Int(nullable: false),
                        IsSendForApproval = c.Boolean(nullable: false),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        OnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArInvoiceId);
            
            CreateTable(
                "EXCEED.ArInvoiceLines",
                c => new
                    {
                        ArInvoiceLinesId = c.Int(nullable: false, identity: true),
                        ArInvoiceId = c.Int(nullable: false),
                        Items = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxType = c.String(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArInvoiceLinesId)
                .ForeignKey("EXCEED.ArInvoice", t => t.ArInvoiceId, cascadeDelete: true)
                .Index(t => t.ArInvoiceId);
            
            CreateTable(
                "EXCEED.ArInvoiceValidation",
                c => new
                    {
                        ArInvoiceValidationId = c.Int(nullable: false, identity: true),
                        ArInvoiceId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArInvoiceValidationId)
                .ForeignKey("EXCEED.ArInvoice", t => t.ArInvoiceId, cascadeDelete: true)
                .Index(t => t.ArInvoiceId);
            
            CreateTable(
                "EXCEED.ArStandardCollectionViewModel",
                c => new
                    {
                        ArStandardCollectionViewModelId = c.Int(nullable: false, identity: true),
                        InvoiceNumber = c.String(),
                        InstallementNo = c.String(),
                        ApplyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceDue = c.String(),
                        ApplyDate = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ArInvoice_ArInvoiceId = c.Int(),
                    })
                .PrimaryKey(t => t.ArStandardCollectionViewModelId)
                .ForeignKey("EXCEED.ArInvoice", t => t.ArInvoice_ArInvoiceId)
                .Index(t => t.ArInvoice_ArInvoiceId);
            
            CreateTable(
                "EXCEED.ARMiscelaneousCollectionDistribution",
                c => new
                    {
                        ARMiscelaneousCollectionDistributionID = c.Int(nullable: false, identity: true),
                        ARMiscelaneousCollectionID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ARMiscelaneousCollectionDistributionID)
                .ForeignKey("EXCEED.ARMiscelaneousCollection", t => t.ARMiscelaneousCollectionID, cascadeDelete: true)
                .Index(t => t.ARMiscelaneousCollectionID);
            
            CreateTable(
                "EXCEED.ARMiscelaneousCollection",
                c => new
                    {
                        ARMiscelaneousCollectionID = c.Int(nullable: false, identity: true),
                        Activity = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RevenueAccount = c.String(nullable: false),
                        RevenueAccountDesc = c.String(),
                        TaxType = c.String(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdingType = c.String(nullable: false),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdTax = c.Boolean(nullable: false),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceiptNumber = c.String(nullable: false),
                        ReceiptDate = c.DateTime(nullable: false),
                        ReceiptMethod = c.String(nullable: false),
                        ReceiptAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineTransferred = c.Boolean(nullable: false),
                        OnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ARMiscelaneousCollectionID);
            
            CreateTable(
                "EXCEED.ARMiscelaneousCollectionValidation",
                c => new
                    {
                        ARMiscelaneousCollectionValidationID = c.Int(nullable: false, identity: true),
                        ARMiscelaneousCollectionID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ARMiscelaneousCollectionValidationID)
                .ForeignKey("EXCEED.ARMiscelaneousCollection", t => t.ARMiscelaneousCollectionID, cascadeDelete: true)
                .Index(t => t.ARMiscelaneousCollectionID);
            
            CreateTable(
                "EXCEED.ARRemitAddress",
                c => new
                    {
                        ARRemitAddressID = c.Int(nullable: false, identity: true),
                        Company = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        PostalCode = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ARRemitAddressID);
            
            CreateTable(
                "EXCEED.ARSetupProductServiceAccount",
                c => new
                    {
                        ARSetupProductServiceAccountID = c.Int(nullable: false, identity: true),
                        Product = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ARSetupProductServiceAccountID);
            
            CreateTable(
                "EXCEED.ARSetupReceiptBalanceAccount",
                c => new
                    {
                        ARSetupReceiptBalanceAccountID = c.Int(nullable: false, identity: true),
                        UnidentifiedAccountCode = c.String(),
                        OnAccountAccountCode = c.String(),
                        UnappliedAccountCode = c.String(),
                        PrepaymentAccountCode = c.String(nullable: false),
                        Branch = c.String(),
                    })
                .PrimaryKey(t => t.ARSetupReceiptBalanceAccountID);
            
            CreateTable(
                "EXCEED.ARSetupTaxRate",
                c => new
                    {
                        ARSetupTaxRateID = c.Int(nullable: false, identity: true),
                        Tax = c.String(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ARSetupTaxRateID);
            
            CreateTable(
                "EXCEED.ARSetupTransactionClass",
                c => new
                    {
                        ARSetupTransactionClassID = c.Int(nullable: false, identity: true),
                        ClassName = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ARSetupTransactionClassID);
            
            CreateTable(
                "EXCEED.ARSetupTransactionType",
                c => new
                    {
                        ARSetupTransactionTypeID = c.Int(nullable: false, identity: true),
                        TransactionName = c.String(nullable: false),
                        TransactionClass = c.String(nullable: false),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        CreationSign = c.String(nullable: false),
                        OpenReceivable = c.Boolean(nullable: false),
                        PosttoGL = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ReceivableAccount = c.String(nullable: false),
                        RevenueAccount = c.String(nullable: false),
                        TaxAccount = c.String(nullable: false),
                        FreightAccount = c.String(),
                        ClearingAccount = c.String(),
                        UnearnedAccount = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ARSetupTransactionTypeID);
            
            CreateTable(
                "EXCEED.ArStandardCollectionDistribution",
                c => new
                    {
                        ArStandardCollectionDistributionId = c.Int(nullable: false, identity: true),
                        ArStandardCollectionId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArStandardCollectionDistributionId)
                .ForeignKey("EXCEED.ArStandardCollection", t => t.ArStandardCollectionId, cascadeDelete: true)
                .Index(t => t.ArStandardCollectionId);
            
            CreateTable(
                "EXCEED.ArStandardCollection",
                c => new
                    {
                        ArStandardCollectionId = c.Int(nullable: false, identity: true),
                        ReceiptNumber = c.String(nullable: false),
                        ReceiptAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceiptDate = c.DateTime(nullable: false),
                        ReceiptMethod = c.String(nullable: false),
                        CustomerName = c.String(),
                        CustomerId = c.String(),
                        UnidentifiedBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AppliedBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OnAccountBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnappliedBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrepaymentBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineTransferred = c.Boolean(nullable: false),
                        OnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArStandardCollectionId);
            
            CreateTable(
                "EXCEED.ArStandardCollectionLines",
                c => new
                    {
                        ArStandardCollectionLinesId = c.Int(nullable: false, identity: true),
                        ArStandardCollectionId = c.Int(nullable: false),
                        IsPrepayment = c.Boolean(nullable: false),
                        InvoiceNumber = c.String(),
                        InstallementNo = c.String(),
                        ApplyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApplyDate = c.DateTime(nullable: false),
                        TaxType = c.String(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdingType = c.String(nullable: false),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdTax = c.Boolean(nullable: false),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HasPrepayment = c.Boolean(nullable: false),
                        PrepaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArStandardCollectionLinesId)
                .ForeignKey("EXCEED.ArStandardCollection", t => t.ArStandardCollectionId, cascadeDelete: true)
                .Index(t => t.ArStandardCollectionId);
            
            CreateTable(
                "EXCEED.ArStandardCollectionValidation",
                c => new
                    {
                        ArStandardCollectionValidationId = c.Int(nullable: false, identity: true),
                        ArStandardCollectionId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArStandardCollectionValidationId)
                .ForeignKey("EXCEED.ArStandardCollection", t => t.ArStandardCollectionId, cascadeDelete: true)
                .Index(t => t.ArStandardCollectionId);
            
            CreateTable(
                "Payroll.Association",
                c => new
                    {
                        AssociationId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsMemeber = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AssociationId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "common.Attachment",
                c => new
                    {
                        AttachmentId = c.Int(nullable: false, identity: true),
                        ReferenceType = c.Int(nullable: false),
                        Reference = c.String(),
                        Description = c.String(),
                        URL = c.String(),
                        BinaryFile = c.Binary(),
                        FileType = c.String(),
                        Index = c.Double(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AttachmentId);
            
            CreateTable(
                "HRM.Attendance",
                c => new
                    {
                        AttendanceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        AttendanceType = c.Int(nullable: false),
                        FromDateTime = c.DateTime(nullable: false),
                        ToDateTime = c.DateTime(nullable: false),
                        DelayTime = c.Int(nullable: false),
                        NoOfAbcent = c.Int(nullable: false),
                        MeasureTaken = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AttendanceId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EXCEED.BankAccount",
                c => new
                    {
                        BankAccountId = c.Int(nullable: false, identity: true),
                        AccountName = c.String(nullable: false),
                        AccountNumber = c.String(),
                        BankName = c.String(),
                        BankBranch = c.String(),
                        BankAdress = c.String(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AccountControl = c.String(),
                        AccountUse = c.String(),
                        Status = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BankAccountId);
            
            CreateTable(
                "EXCEED.BankGuarantee",
                c => new
                    {
                        BankGuaranteeID = c.Int(nullable: false, identity: true),
                        GuaranteeType = c.Int(nullable: false),
                        GuaranteeRefNo = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(),
                        Date = c.DateTime(nullable: false),
                        BankBranch = c.String(),
                        BankAddress = c.String(),
                        ValidityPeriod = c.Int(nullable: false),
                        ValidityStatus = c.Int(nullable: false),
                        GuaranteePresenterName = c.String(),
                        GuaranteePresenterAddress = c.String(),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BankGuaranteeID);
            
            CreateTable(
                "Common.BankReference",
                c => new
                    {
                        BankReferenceId = c.Int(nullable: false, identity: true),
                        BankId = c.Int(nullable: false),
                        ReferenceType = c.String(),
                        Reference = c.String(),
                        Type = c.Int(nullable: false),
                        AccountNumber = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.BankReferenceId)
                .ForeignKey("Common.Bank", t => t.BankId, cascadeDelete: true)
                .Index(t => t.BankId);
            
            CreateTable(
                "Common.Bank",
                c => new
                    {
                        BankID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Category = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        ParentId = c.Int(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.BankID);
            
            CreateTable(
                "EXCEED.BidAwardedSupplier",
                c => new
                    {
                        BidAwardedSupplierID = c.Int(nullable: false, identity: true),
                        LotID = c.Int(nullable: false),
                        Supplier = c.String(),
                        AwardDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BidAwardedSupplierID)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            CreateTable(
                "EXCEED.Lot",
                c => new
                    {
                        LotID = c.Int(nullable: false, identity: true),
                        TenderID = c.Int(nullable: false),
                        LotNumber = c.Int(nullable: false),
                        Description = c.String(),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LotID)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .ForeignKey("EXCEED.Tender", t => t.TenderID, cascadeDelete: true)
                .Index(t => t.TenderID)
                .Index(t => t.PurchaseRequisitionID);
            
            CreateTable(
                "EXCEED.BidPParticipatingSuppliers",
                c => new
                    {
                        BidPParticipatingSuppliersID = c.Int(nullable: false, identity: true),
                        LotID = c.Int(nullable: false),
                        Supplier = c.String(),
                        Representative = c.String(),
                        Address = c.String(),
                        Telephone = c.String(),
                        BidSubmitionDate = c.DateTime(nullable: false),
                        BidSubmitionTime = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BidPParticipatingSuppliersID)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            CreateTable(
                "EXCEED.BidPurchasingSuppliers",
                c => new
                    {
                        BidPurchasingSuppliersID = c.Int(nullable: false, identity: true),
                        LotID = c.Int(nullable: false),
                        Supplier = c.String(),
                        Representative = c.String(),
                        PurchaseDate = c.DateTime(nullable: false),
                        Receipt = c.String(),
                        Telephone = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BidPurchasingSuppliersID)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            CreateTable(
                "EXCEED.TenderEvaluationCriteria",
                c => new
                    {
                        TenderEvaluationCriteriaID = c.Int(nullable: false, identity: true),
                        LotID = c.Int(nullable: false),
                        CriteriaName = c.String(),
                        Weight = c.Single(nullable: false),
                        Description = c.String(),
                        Type = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TenderEvaluationCriteriaID)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            CreateTable(
                "EXCEED.TenderEvaluationResult",
                c => new
                    {
                        TenderEvaluationResultID = c.Int(nullable: false, identity: true),
                        LotID = c.Int(nullable: false),
                        SupplierName = c.String(nullable: false),
                        CriteriaName = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Score = c.Single(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TenderEvaluationResultID)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            CreateTable(
                "EXCEED.LotFinancialOffer",
                c => new
                    {
                        LotFinancialOfferID = c.Int(nullable: false, identity: true),
                        LotID = c.Int(nullable: false),
                        Supplier = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LotFinancialOfferID)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            CreateTable(
                "EXCEED.LotItems",
                c => new
                    {
                        LotItemsID = c.Int(nullable: false, identity: true),
                        LotID = c.Int(nullable: false),
                        InventoryItem = c.String(nullable: false),
                        UOM = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LotItemsID)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            CreateTable(
                "EXCEED.PurchaseRequisition",
                c => new
                    {
                        PurchaseRequisitionID = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        Department = c.String(),
                        StoreKeeperName = c.String(),
                        ReferenceNo = c.String(),
                        ProcurementOfficer = c.String(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionID)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionID, cascadeDelete: true)
                .Index(t => t.StoreRequisitionID);
            
            CreateTable(
                "EXCEED.PurchaseRequisitionDistribution",
                c => new
                    {
                        PurchaseRequisitionDistributionID = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionDistributionID)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID);
            
            CreateTable(
                "EXCEED.PurchaseRequisitionItem",
                c => new
                    {
                        PurchaseRequisitionItemID = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        PartNo = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionItemID)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID);
            
            CreateTable(
                "EXCEED.PurchaseRequisitionPurchaseDepartmentValidation",
                c => new
                    {
                        PurchaseRequisitionPurchaseDepartmentValidationID = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionPurchaseDepartmentValidationID)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID);
            
            CreateTable(
                "EXCEED.PurchaseRequisitionUserDepartmentValidation",
                c => new
                    {
                        PurchaseRequisitionUserDepartmentValidationID = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionUserDepartmentValidationID)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID);
            
            CreateTable(
                "EXCEED.StoreRequisition",
                c => new
                    {
                        StoreRequisitionID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        Department = c.String(nullable: false),
                        IsFuel = c.Boolean(nullable: false),
                        PersonID = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                        StoreCode = c.String(nullable: false, maxLength: 128),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionID)
                .ForeignKey("EXCEED.InventoryStore", t => t.StoreCode, cascadeDelete: true)
                .Index(t => t.StoreCode);
            
            CreateTable(
                "EXCEED.StoreRequisitionDistribution",
                c => new
                    {
                        StoreRequisitionDistributionID = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.StoreRequisitionDistributionID)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionID, cascadeDelete: true)
                .Index(t => t.StoreRequisitionID);
            
            CreateTable(
                "EXCEED.StoreRequisitionItem",
                c => new
                    {
                        StoreRequisitionItemID = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        PartNo = c.String(),
                        RequestedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionItemID)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionID, cascadeDelete: true)
                .Index(t => t.StoreRequisitionID)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "EXCEED.StoreRequisitionPropertyValidation",
                c => new
                    {
                        StoreRequisitionPropertyValidationID = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionPropertyValidationID)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionID, cascadeDelete: true)
                .Index(t => t.StoreRequisitionID);
            
            CreateTable(
                "EXCEED.StoreRequisitionValidation",
                c => new
                    {
                        StoreRequisitionValidationID = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionValidationID)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionID, cascadeDelete: true)
                .Index(t => t.StoreRequisitionID);
            
            CreateTable(
                "EXCEED.Tender",
                c => new
                    {
                        TenderID = c.Int(nullable: false, identity: true),
                        TenderReference = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        FloatDate = c.DateTime(nullable: false),
                        ClosingDate = c.DateTime(nullable: false),
                        OpenigDate = c.DateTime(nullable: false),
                        IsICB = c.Boolean(nullable: false),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TenderID);
            
            CreateTable(
                "EXCEED.TenderMinuteUploadDBModel",
                c => new
                    {
                        TenderMinuteUploadDBModelId = c.Int(nullable: false, identity: true),
                        TenderID = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.TenderMinuteUploadDBModelId)
                .ForeignKey("EXCEED.Tender", t => t.TenderID, cascadeDelete: true)
                .Index(t => t.TenderID);
            
            CreateTable(
                "EXCEED.TenderValidation",
                c => new
                    {
                        TenderValidationID = c.Int(nullable: false, identity: true),
                        TenderID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TenderValidationID)
                .ForeignKey("EXCEED.Tender", t => t.TenderID, cascadeDelete: true)
                .Index(t => t.TenderID);
            
            CreateTable(
                "HRM.Branch",
                c => new
                    {
                        BranchId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.BranchId);
            
            CreateTable(
                "EXCEED.BudgetPlan",
                c => new
                    {
                        BudgetPlanID = c.Int(nullable: false, identity: true),
                        BudgetYear = c.String(),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        Unit = c.String(nullable: false),
                        TotalYearlyBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetPlanID);
            
            CreateTable(
                "EXCEED.BudgetPlanMonthly",
                c => new
                    {
                        BudgetPlanMonthlyID = c.Int(nullable: false, identity: true),
                        BudgetDefineID = c.Int(nullable: false),
                        BudgetMonth = c.String(),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        Unit = c.String(),
                        MonthlyBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        BudgetPlan_BudgetPlanID = c.Int(),
                        BudgetTransfer_BudgetTransferID = c.Int(),
                    })
                .PrimaryKey(t => t.BudgetPlanMonthlyID)
                .ForeignKey("EXCEED.BudgetPlan", t => t.BudgetPlan_BudgetPlanID)
                .ForeignKey("EXCEED.BudgetTransfer", t => t.BudgetTransfer_BudgetTransferID)
                .Index(t => t.BudgetPlan_BudgetPlanID)
                .Index(t => t.BudgetTransfer_BudgetTransferID);
            
            CreateTable(
                "EXCEED.BudgetTransfer",
                c => new
                    {
                        BudgetTransferID = c.Int(nullable: false, identity: true),
                        FromBudgetYear = c.String(nullable: false),
                        ToBudgetYear = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        Unit = c.String(nullable: false),
                        Transferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetTransferID);
            
            CreateTable(
                "EXCEED.BudgetUsage",
                c => new
                    {
                        BudgetUsageID = c.Int(nullable: false, identity: true),
                        BudgetYear = c.String(nullable: false),
                        BudgetMonth = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        Unit = c.String(nullable: false),
                        MonthlyActuals = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetUsageID);
            
            CreateTable(
                "HRM.ClearanceProvider",
                c => new
                    {
                        ClearanceProviderId = c.Int(nullable: false, identity: true),
                        WorkUnit = c.String(),
                        Name = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ClearanceProviderId);
            
            CreateTable(
                "HRM.Clearance",
                c => new
                    {
                        ClearanceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        ReasonOfTermination = c.String(nullable: false),
                        TerminatedSince = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ClearanceId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.Commitment",
                c => new
                    {
                        CommitmentId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Reason = c.String(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Value = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CommitmentId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "HRM.Committee",
                c => new
                    {
                        CommitteeId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Role = c.String(nullable: false),
                        CommitteeType = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CommitteeId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.MainCompany",
                c => new
                    {
                        MainCompanyId = c.Int(nullable: false, identity: true),
                        TradeName = c.String(nullable: false),
                        BrandName = c.String(),
                        BusinessType = c.Int(nullable: false),
                        Category = c.String(),
                        AmharicName = c.String(),
                        Abbreviation = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MainCompanyId);
            
            CreateTable(
                "HRM.Competency",
                c => new
                    {
                        CompetencyId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Level = c.String(),
                        Status = c.String(),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CompetencyId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "common.Configuration",
                c => new
                    {
                        ConfigurationId = c.Int(nullable: false, identity: true),
                        Category = c.String(),
                        Reference = c.String(nullable: false),
                        Attribute = c.String(nullable: false),
                        CurrentValue = c.String(nullable: false),
                        PreviousValue = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ConfigurationId);
            
            CreateTable(
                "HRM.Contract",
                c => new
                    {
                        ContractId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        MonthlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ContractId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EXCEED.ContractManagementItem",
                c => new
                    {
                        ContractManagementItemID = c.Int(nullable: false, identity: true),
                        ContractManagementID = c.Int(nullable: false),
                        ItemCategory = c.String(),
                        InventoryItem = c.String(nullable: false),
                        UoM = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractManagementItemID)
                .ForeignKey("EXCEED.ContractManagement", t => t.ContractManagementID, cascadeDelete: true)
                .Index(t => t.ContractManagementID);
            
            CreateTable(
                "EXCEED.ContractManagement",
                c => new
                    {
                        ContractManagementID = c.Int(nullable: false, identity: true),
                        ContractRefNo = c.String(),
                        SigningDate = c.DateTime(nullable: false),
                        Supplier = c.String(),
                        PaymentTerm = c.Int(nullable: false),
                        DeliveryTerm = c.Int(nullable: false),
                        DeliveryDate = c.DateTime(nullable: false),
                        ContractAmount = c.String(),
                        ContractPeriod = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractManagementID);
            
            CreateTable(
                "HRM.Cosign",
                c => new
                    {
                        CosignId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        AgreementDate = c.DateTime(nullable: false),
                        FullName = c.String(nullable: false),
                        BirthPlace = c.String(nullable: false),
                        Occupation = c.String(),
                        Workorganization = c.String(),
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CosignId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Common.Country",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PolticalName = c.String(),
                        Continent = c.String(),
                        TelephoneCode = c.String(),
                        TimeZone = c.String(),
                        Nationality = c.String(),
                        CountryCode = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "HRM.CourseTracking",
                c => new
                    {
                        CourseTrackingId = c.Int(nullable: false, identity: true),
                        PeriodId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrainingType = c.Int(nullable: false),
                        MainCompanyId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CourseTrackingId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompany", t => t.MainCompanyId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.PeriodId)
                .Index(t => t.LookupId)
                .Index(t => t.MainCompanyId);
            
            CreateTable(
                "EXCEED.CPOManagement",
                c => new
                    {
                        CPOManagementID = c.Int(nullable: false, identity: true),
                        Branch = c.String(nullable: false),
                        ItemDesc = c.String(nullable: false),
                        SupplierName = c.String(nullable: false),
                        AdvanceAmount = c.Decimal(precision: 18, scale: 2),
                        AdvanceDeadline = c.DateTime(),
                        PerformanceAmount = c.Decimal(precision: 18, scale: 2),
                        PerformanceDeadline = c.DateTime(),
                        Bank = c.String(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CPOManagementID);
            
            CreateTable(
                "EXCEED.CustomerTicketView",
                c => new
                    {
                        CustomerTicketID = c.Int(nullable: false, identity: true),
                        CustomerID = c.String(),
                        Description = c.String(),
                        Priority = c.Int(nullable: false),
                        TicketGroup = c.String(),
                        EscalationLevel = c.String(),
                        Source = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CustomerTicketID);
            
            CreateTable(
                "EXCEED.DepartmentCostCenter",
                c => new
                    {
                        DepartmentCostCenterID = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false),
                        LocationCode = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentCostCenterID);
            
            CreateTable(
                "HRM.Disability",
                c => new
                    {
                        DisabilityId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Type = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DisabilityId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.DisciplinaryCase",
                c => new
                    {
                        DisciplinaryCaseId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        TransgressionDate = c.DateTime(nullable: false),
                        NatureofTransgression = c.String(nullable: false),
                        Evidence = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DisciplinaryCaseId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.DisciplineMeasures",
                c => new
                    {
                        MeasuresId = c.Int(nullable: false, identity: true),
                        MeasuresType = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.MeasuresId);
            
            CreateTable(
                "HRM.DisciplineMeasuresDetail",
                c => new
                    {
                        DetailMeasuresId = c.Int(nullable: false, identity: true),
                        MeasuresId = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DetailMeasuresId)
                .ForeignKey("HRM.DisciplineMeasures", t => t.MeasuresId, cascadeDelete: true)
                .Index(t => t.MeasuresId);
            
            CreateTable(
                "HRM.DisciplineMeasuresType",
                c => new
                    {
                        DisciplineTypeId = c.Int(nullable: false, identity: true),
                        MeasuresId = c.Int(nullable: false),
                        ExpireYear = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PercentageEffectOnPromotion = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DisciplineTypeId)
                .ForeignKey("HRM.DisciplineMeasures", t => t.MeasuresId, cascadeDelete: true)
                .Index(t => t.MeasuresId);
            
            CreateTable(
                "HRM.DisciplineRecord",
                c => new
                    {
                        RecordId = c.Int(nullable: false, identity: true),
                        MeasuresId = c.Int(nullable: false),
                        DetailMeasuresId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RecordId)
                .ForeignKey("HRM.DisciplineMeasuresDetail", t => t.DetailMeasuresId, cascadeDelete: true)
                .Index(t => t.DetailMeasuresId);
            
            CreateTable(
                "HRM.Education",
                c => new
                    {
                        EmpEduId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        OrganizationName = c.String(nullable: false),
                        OrgAddress = c.String(nullable: false),
                        From = c.DateTime(),
                        To = c.DateTime(nullable: false),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EduLevel = c.Int(nullable: false),
                        EduField = c.Int(nullable: false),
                        TotalCost = c.Double(nullable: false),
                        PercentCoverdByOrg = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmpEduId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.Emergency",
                c => new
                    {
                        EmergencyId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Name = c.String(),
                        LookupId = c.Int(nullable: false),
                        HomePhoneNo = c.String(),
                        MobilePhoneNo = c.String(),
                        WorkPhoneno = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmergencyId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "HRM.EmpDisciplinaryHearing",
                c => new
                    {
                        DisciplinaryHearingId = c.Int(nullable: false, identity: true),
                        DisciplinaryCaseId = c.Int(nullable: false),
                        HearingDate = c.DateTime(nullable: false),
                        HearingOutcome = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DisciplinaryHearingId)
                .ForeignKey("HRM.DisciplinaryCase", t => t.DisciplinaryCaseId, cascadeDelete: true)
                .Index(t => t.DisciplinaryCaseId);
            
            CreateTable(
                "HRM.EmpDisciplineMeasure",
                c => new
                    {
                        EmpDisciplineMeasureId = c.Int(nullable: false, identity: true),
                        DisciplinaryCaseId = c.Int(nullable: false),
                        MeasuresId = c.Int(nullable: false),
                        DetailMeasuresId = c.Int(nullable: false),
                        DecisionDate = c.DateTime(nullable: false),
                        IsExpired = c.Boolean(nullable: false),
                        IsReversedByAppeal = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmpDisciplineMeasureId)
                .ForeignKey("HRM.DisciplinaryCase", t => t.DisciplinaryCaseId, cascadeDelete: true)
                .ForeignKey("HRM.DisciplineMeasuresDetail", t => t.DetailMeasuresId, cascadeDelete: true)
                .Index(t => t.DisciplinaryCaseId)
                .Index(t => t.DetailMeasuresId);
            
            CreateTable(
                "HRM.EmployeeAssociation",
                c => new
                    {
                        AssociationId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Name = c.String(),
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AssociationId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Performance.EmployeeMeasurement",
                c => new
                    {
                        EmpMeasurementId = c.Int(nullable: false, identity: true),
                        MeasurementDefinitionId = c.Int(nullable: false),
                        MeasurementId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        EmpMesPercentageofWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PeriodId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmpMeasurementId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Performance.Measurement", t => t.MeasurementId, cascadeDelete: true)
                .ForeignKey("Performance.MeasurementDefinition", t => t.MeasurementDefinitionId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.MeasurementDefinitionId)
                .Index(t => t.MeasurementId)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "Performance.Measurement",
                c => new
                    {
                        MeasurementId = c.Int(nullable: false, identity: true),
                        MeasurementDefinitionId = c.Int(nullable: false),
                        MeasurementName = c.String(nullable: false),
                        MesPercentageofWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.MeasurementId);
            
            CreateTable(
                "Performance.MeasurementDefinition",
                c => new
                    {
                        MeasurementDefinitionId = c.Int(nullable: false, identity: true),
                        EvaluationCriteria = c.String(nullable: false),
                        LookupId = c.Int(nullable: false),
                        DefnPercentageofWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EmployeeId = c.Int(nullable: false),
                        Role = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.MeasurementDefinitionId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Performance.EmployeeMeasurementDefinition",
                c => new
                    {
                        EmpMeasurementDefinitionId = c.Int(nullable: false, identity: true),
                        MeasurementDefinitionId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        EmpPercentageofWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PeriodId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmpMeasurementDefinitionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Performance.MeasurementDefinition", t => t.MeasurementDefinitionId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.MeasurementDefinitionId)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "Performance.EmployeePerformanceAction",
                c => new
                    {
                        EmpActionId = c.Int(nullable: false, identity: true),
                        MeasurementDefinitionId = c.Int(nullable: false),
                        MeasurementId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        EmpPerfPercentageofWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PeriodId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmpActionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Performance.PerformanceAction", t => t.ActionId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.ActionId)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "Performance.PerformanceAction",
                c => new
                    {
                        ActionId = c.Int(nullable: false, identity: true),
                        MeasurementDefinitionId = c.Int(nullable: false),
                        MeasurementId = c.Int(nullable: false),
                        ActionName = c.String(nullable: false),
                        AcPercentageofWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ActionId);
            
            CreateTable(
                "Performance.EmployeePerformance",
                c => new
                    {
                        EmployeePerformanceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeePerformanceId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Payroll.EmpPayrollDeduction",
                c => new
                    {
                        EmpPayrollDeductionId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        EmpFullName = c.String(nullable: false),
                        PeriodName = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeductionType = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmpPayrollDeductionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Payroll.EmpPayrollLongTermDeduction",
                c => new
                    {
                        EmpPayrollLongTermDeductionId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        EmpFullName = c.String(nullable: false),
                        DeductionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Months = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeductionType = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmpPayrollLongTermDeductionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Payroll.EmpPayrollNoDaysWorked",
                c => new
                    {
                        EmpPayrollNoDaysWorkedId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        Days = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmpPayrollNoDaysWorkedId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "Payroll.EmpPayrollServiceChargeViewModel",
                c => new
                    {
                        EmpPayrollServiceChargeViewModelId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        EmpFullName = c.String(),
                        PeriodName = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmpPayrollServiceChargeViewModelId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.Training",
                c => new
                    {
                        EmpEduTrainId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        OrganizationName = c.String(nullable: false),
                        OrgAddress = c.String(nullable: false),
                        IsTraining = c.Boolean(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Double(nullable: false),
                        PercentCoverdByOrg = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrainingName = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmpEduTrainId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EXCEED.EvaluatedPrice",
                c => new
                    {
                        EvaluatedPriceId = c.Int(nullable: false, identity: true),
                        ItemCode = c.String(),
                        SettedPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreviousPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.EvaluatedPriceId);
            
            CreateTable(
                "HRM.ExitInterview",
                c => new
                    {
                        ExitIntervId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        ToKeep = c.String(),
                        ToRemove = c.String(),
                        Reason = c.String(),
                        DueToWork = c.Boolean(nullable: false),
                        Sicknees = c.String(),
                        MainReason = c.String(),
                        OtherReason = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExitIntervId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.Experience",
                c => new
                    {
                        ExperienceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Organozation = c.String(nullable: false),
                        OrgFullAddress = c.String(nullable: false),
                        EmploymentType = c.String(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        TotalDuration = c.Int(nullable: false),
                        Responsibility = c.String(),
                        ClearanceDateTime = c.DateTime(nullable: false),
                        CleranceReason = c.String(),
                        IncomeTaxPaymentRefNo = c.String(),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExperienceId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.ExprianceRequest",
                c => new
                    {
                        ExprianceRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        ApplicationDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExprianceRequestId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EXCEED.FinanceDashboardRange",
                c => new
                    {
                        FinanceDashboardRangeId = c.Int(nullable: false, identity: true),
                        FinanceDashboardId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.FinanceDashboardRangeId)
                .ForeignKey("EXCEED.FinanceDashboard", t => t.FinanceDashboardId, cascadeDelete: true)
                .Index(t => t.FinanceDashboardId);
            
            CreateTable(
                "EXCEED.FinanceDashboard",
                c => new
                    {
                        FinanceDashboardId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.FinanceDashboardId);
            
            CreateTable(
                "EXCEED.FinInStatementRecord",
                c => new
                    {
                        FinInStatementRecordID = c.Int(nullable: false, identity: true),
                        FYear = c.String(),
                        Status = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinInStatementRecordID);
            
            CreateTable(
                "EXCEED.FinSetupCurrency",
                c => new
                    {
                        FinSetupCurrencyID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Territory = c.String(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinSetupCurrencyID);
            
            CreateTable(
                "EXCEED.FinSetupCurrencyExchange",
                c => new
                    {
                        FinSetupCurrencyExchangeID = c.Int(nullable: false, identity: true),
                        FromCurrency = c.String(nullable: false),
                        ToCurrency = c.String(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        RateOne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RateTwo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinSetupCurrencyExchangeID);
            
            CreateTable(
                "EXCEED.FixedAssetCategory",
                c => new
                    {
                        FixedAssetCategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false),
                        CategoryCode = c.String(nullable: false),
                        AssetGroup = c.String(nullable: false),
                        DepreciationCategory = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetCategoryID);
            
            CreateTable(
                "EXCEED.FixedAssetClearingAccountSetup",
                c => new
                    {
                        FixedAssetClearingAccountSetupID = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetClearingAccountSetupID);
            
            CreateTable(
                "EXCEED.FixedAssetDepreciationSetup",
                c => new
                    {
                        FixedAssetDepreciationSetupID = c.Int(nullable: false, identity: true),
                        FixedAssetAccountCode = c.String(nullable: false),
                        FixedAssetAccountCodeDescription = c.String(nullable: false),
                        DepreciationAccountCode = c.String(nullable: false),
                        AccumulatedDepreciationAccountCode = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetDepreciationSetupID);
            
            CreateTable(
                "EXCEED.FixedAssetSubCategory",
                c => new
                    {
                        FixedAssetSubCategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false),
                        SubCategoryName = c.String(nullable: false),
                        SubCategoryCode = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetSubCategoryID);
            
            CreateTable(
                "Payroll.FixedContribution",
                c => new
                    {
                        FixedContributionId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Months = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.FixedContributionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "EXCEED.FleetAccidentRegistration",
                c => new
                    {
                        FleetAccidentRegistrationID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        OccurrenceDate = c.DateTime(nullable: false),
                        CauseOfAccident = c.String(nullable: false),
                        TypeOfDamage = c.String(),
                        Level = c.Int(nullable: false),
                        Body = c.String(nullable: false),
                        Location = c.String(nullable: false),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        Police = c.String(),
                        Select = c.String(),
                        ByInsurance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ByCompany = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ByOperator = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ByOthers = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoteOnAccident = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetAccidentRegistrationID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.FleetEquipmentRegistration",
                c => new
                    {
                        AssetNumber = c.String(nullable: false, maxLength: 128),
                        FleetSetupEquipmentCategoryID = c.Int(nullable: false),
                        FleetSetupEquipmentTypeID = c.Int(nullable: false),
                        FleetSetupEquipmentNameID = c.Int(nullable: false),
                        PlateNumber = c.String(nullable: false),
                        Make = c.String(nullable: false),
                        Year = c.Int(nullable: false),
                        Model = c.String(nullable: false),
                        Color = c.String(nullable: false),
                        Engine = c.String(nullable: false),
                        Chassis = c.String(),
                        Capacity = c.String(),
                        Size = c.String(),
                        Status = c.Int(nullable: false),
                        FuelStandard = c.String(nullable: false),
                        RentalRate = c.String(),
                        WayOfSupply = c.Int(nullable: false),
                        Supplier = c.String(),
                        PurchasingDate = c.DateTime(),
                        PurchasingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DepreciationCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Serial = c.String(),
                        Liberie = c.String(),
                        FuelType = c.Int(nullable: false),
                        Odometer = c.String(),
                        OdometerState = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetSetupEquipmentCategory", t => t.FleetSetupEquipmentCategoryID, cascadeDelete: true)
                .ForeignKey("EXCEED.FleetSetupEquipmentName", t => t.FleetSetupEquipmentNameID, cascadeDelete: true)
                .ForeignKey("EXCEED.FleetSetupEquipmentType", t => t.FleetSetupEquipmentTypeID, cascadeDelete: true)
                .Index(t => t.FleetSetupEquipmentCategoryID)
                .Index(t => t.FleetSetupEquipmentTypeID)
                .Index(t => t.FleetSetupEquipmentNameID);
            
            CreateTable(
                "EXCEED.FleetSetupEquipmentCategory",
                c => new
                    {
                        FleetSetupEquipmentCategoryID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupEquipmentCategoryID);
            
            CreateTable(
                "EXCEED.FleetAttachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Extension = c.String(),
                        AssetNumber = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetSetupEquipmentName",
                c => new
                    {
                        FleetSetupEquipmentNameID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupEquipmentNameID);
            
            CreateTable(
                "EXCEED.FleetSetupEquipmentType",
                c => new
                    {
                        FleetSetupEquipmentTypeID = c.Int(nullable: false, identity: true),
                        CategoryID = c.Int(nullable: false),
                        Type = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupEquipmentTypeID);
            
            CreateTable(
                "EXCEED.FleetOperatorRegistration",
                c => new
                    {
                        FleetOperatorRegistrationID = c.Int(nullable: false, identity: true),
                        FName = c.String(nullable: false),
                        MName = c.String(nullable: false),
                        LName = c.String(nullable: false),
                        Gender = c.Int(nullable: false),
                        MobileNumber = c.String(),
                        HousePhoneNumber = c.String(),
                        City = c.String(),
                        EmailAddress = c.String(),
                        FullName = c.String(),
                        FullAddress = c.String(),
                        GMobile = c.String(),
                        HireDate = c.DateTime(nullable: false),
                        TerminationDate = c.DateTime(),
                        FleetSetupOperatorPositionID = c.Int(nullable: false),
                        MonthlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPremium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DesertAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OtherIncentive = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FleetSetupEducationLevelID = c.Int(nullable: false),
                        License = c.Int(nullable: false),
                        LicenseType = c.Int(nullable: false),
                        LicenseLevel = c.Int(nullable: false),
                        TrainingCenter = c.String(),
                        Location = c.String(),
                        COCAttached = c.Boolean(nullable: false),
                        IssuedDate = c.DateTime(),
                        RenewalDate = c.DateTime(),
                        ExpiredDate = c.DateTime(),
                        AssetNumber = c.String(maxLength: 128),
                        PlateNumber = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetOperatorRegistrationID)
                .ForeignKey("EXCEED.FleetSetupEducationLevel", t => t.FleetSetupEducationLevelID, cascadeDelete: true)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetSetupOperatorPosition", t => t.FleetSetupOperatorPositionID, cascadeDelete: true)
                .Index(t => t.FleetSetupOperatorPositionID)
                .Index(t => t.FleetSetupEducationLevelID)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetSetupEducationLevel",
                c => new
                    {
                        FleetSetupEducationLevelID = c.Int(nullable: false, identity: true),
                        Level = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupEducationLevelID);
            
            CreateTable(
                "EXCEED.OperatorAttachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Extension = c.String(),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.FleetSetupOperatorPosition",
                c => new
                    {
                        FleetSetupOperatorPositionID = c.Int(nullable: false, identity: true),
                        Position = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupOperatorPositionID);
            
            CreateTable(
                "EXCEED.FleetEquipmentInspection",
                c => new
                    {
                        FleetEquipmentInspectionID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        InspectionDate = c.DateTime(nullable: false),
                        InspectionResult = c.String(),
                        Status = c.Int(nullable: false),
                        Note = c.String(),
                        InspectedBy = c.String(),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetEquipmentInspectionID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.FleetEquipmentPlanandEvaluation",
                c => new
                    {
                        FleetEquipmentPlanandEvaluationID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        PlanWorking = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanDown = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanIdle = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanTyre = c.String(),
                        Planpetty = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetEquipmentPlanandEvaluationID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetEquipmentRentForInternalProjects",
                c => new
                    {
                        FleetEquipmentRentForInternalProjectsID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        EquipmentOwner = c.Int(nullable: false),
                        OperatorID = c.Int(nullable: false),
                        AssistantOperatorID = c.Int(nullable: false),
                        RequestQuantity = c.Int(nullable: false),
                        AssignedQuantity = c.Int(nullable: false),
                        AssignedDate = c.DateTime(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        TotalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssignedBy = c.String(),
                        ApprovedBy = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetEquipmentRentForInternalProjectsID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetEquipmentRentFromExternal",
                c => new
                    {
                        FleetEquipmentRentFromExternalID = c.Int(nullable: false, identity: true),
                        FleetSetupEquipmentNameID = c.Int(nullable: false),
                        ChassisNumber = c.Int(nullable: false),
                        PlateNumber = c.Int(nullable: false),
                        MotorNumber = c.Int(nullable: false),
                        Capacity = c.String(),
                        TotalRentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelStandard = c.String(),
                        HorsePower = c.Int(nullable: false),
                        ManufacturedYear = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        TotalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(),
                        FuelCoveredBy = c.String(),
                        ProjectLocation = c.String(),
                        UnitOFWorkMeasurement = c.String(),
                        SupplyName = c.String(),
                        SupplyAddress = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetEquipmentRentFromExternalID)
                .ForeignKey("EXCEED.FleetSetupEquipmentName", t => t.FleetSetupEquipmentNameID, cascadeDelete: true)
                .Index(t => t.FleetSetupEquipmentNameID);
            
            CreateTable(
                "EXCEED.FleetEquipmentRentRequisition",
                c => new
                    {
                        FleetEquipmentRentRequisitionID = c.Int(nullable: false, identity: true),
                        FleetSetupEquipmentNameID = c.Int(nullable: false),
                        Capacity = c.String(),
                        Status = c.Int(nullable: false),
                        RequestQuantity = c.Int(nullable: false),
                        ProjectSite = c.String(),
                        ProjectLocation = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        TotalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RequestedBy = c.String(),
                        ApprovedBy = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetEquipmentRentRequisitionID)
                .ForeignKey("EXCEED.FleetSetupEquipmentName", t => t.FleetSetupEquipmentNameID, cascadeDelete: true)
                .Index(t => t.FleetSetupEquipmentNameID);
            
            CreateTable(
                "EXCEED.FleetEquipmentRentToExternal",
                c => new
                    {
                        FleetEquipmentRentToExternalID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        Capacity = c.String(),
                        RequestedQuantity = c.Int(nullable: false),
                        RentedQuantity = c.Int(nullable: false),
                        HorsePower = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        TotalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(),
                        FuelCoveredBy = c.String(),
                        ProjectLocation = c.String(),
                        CustomerName = c.String(),
                        MobileNumber = c.String(),
                        City = c.String(),
                        EmailAddress = c.String(),
                        Website = c.String(),
                        ApprovedBy = c.String(),
                        FleetEquipmentRentFromExternal = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetEquipmentRentToExternalID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetEquipmentReplacement",
                c => new
                    {
                        FleetEquipmentReplacementID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        ReplacementYear = c.Int(nullable: false),
                        ReplacementRecommendationDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        RecommendedBy = c.String(),
                        ApprovedBy = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetEquipmentReplacementID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetEquipmentTimeSheet",
                c => new
                    {
                        FleetEquipmentTimeSheetID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        TypeOfWork = c.String(),
                        TotalRentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Location = c.String(),
                        KMReading = c.String(),
                        FuelUsed = c.Int(nullable: false),
                        WorkingHoursMorningFrom = c.String(nullable: false),
                        WorkingHoursMorningTo = c.String(nullable: false),
                        WorkingHoursAfternoonFrom = c.String(nullable: false),
                        WorkingHoursAfternoonTo = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IdelTime = c.String(),
                        DownTime = c.String(),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        CustomerName = c.String(),
                        AdminName = c.String(),
                        ApprovedBy = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetEquipmentTimeSheetID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.FleetFuelManagement",
                c => new
                    {
                        FleetFuelManagementID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        ProjectName = c.String(),
                        FuelType = c.String(nullable: false),
                        FuelStandard = c.String(),
                        InitialKmHr = c.String(),
                        FinalKmHr = c.String(),
                        PlanKmHr = c.String(),
                        FuelSource = c.Int(nullable: false),
                        FuelFill = c.String(),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Int(nullable: false),
                        TotalFuelCost = c.Int(nullable: false),
                        ReceiptNo = c.String(),
                        Date = c.DateTime(nullable: false),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        ApprovedBy = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetFuelManagementID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.FleetInsuranceRegistration",
                c => new
                    {
                        FleetInsuranceRegistrationID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        PlateNumber = c.String(),
                        Premium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DueDate = c.DateTime(nullable: false),
                        ExpireDate = c.DateTime(),
                        Company = c.String(nullable: false),
                        InsuranceType = c.Int(nullable: false),
                        MobileNumber = c.String(),
                        City = c.String(),
                        EmailAddress = c.String(),
                        Website = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetInsuranceRegistrationID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.InsuranceAttachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Extension = c.String(),
                        FleetInsuranceRegistrationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EXCEED.FleetInsuranceRegistration", t => t.FleetInsuranceRegistrationID, cascadeDelete: true)
                .Index(t => t.FleetInsuranceRegistrationID);
            
            CreateTable(
                "EXCEED.FleetMaintenanceAccessory",
                c => new
                    {
                        FleetMaintenanceAccessoryID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        AccessoryName = c.String(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        NoteOnAccessories = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetMaintenanceAccessoryID)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.FleetMaintenanceCannibalization",
                c => new
                    {
                        FleetMaintenanceCannibalizationID = c.Int(nullable: false, identity: true),
                        PartTakenFromAssetNumber = c.String(),
                        PartTakenFromPlateNumber = c.String(nullable: false),
                        PartTakenFromEquipmentName = c.String(),
                        PartNeededAssetNumber = c.String(),
                        PartNeededPlateNumber = c.String(nullable: false),
                        PartNeededByEquipmentName = c.String(),
                        Model = c.String(),
                        PartName = c.String(),
                        UOM = c.String(),
                        Quantity = c.Int(nullable: false),
                        PreparedBy = c.String(),
                        TakenBy = c.String(),
                        NoteOnCannibalization = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetMaintenanceCannibalizationID);
            
            CreateTable(
                "EXCEED.FleetMaintenanceForExternal",
                c => new
                    {
                        FleetMaintenanceForExternalID = c.Int(nullable: false, identity: true),
                        EquipmentType = c.String(nullable: false),
                        PlateNumber = c.Int(nullable: false),
                        MaintenanceCenter = c.Int(nullable: false),
                        RepairType = c.String(nullable: false),
                        StartedDate = c.DateTime(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        TotalTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AuthorizedBy = c.String(),
                        SparePart = c.String(),
                        PartNo = c.String(),
                        SparePartQuantity = c.Int(nullable: false),
                        SparePartUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SparePartTotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilGrease = c.String(),
                        UOM = c.String(),
                        LubricantQuantity = c.Int(nullable: false),
                        LubricantsUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubricantsTotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CustomerName = c.String(),
                        PhoneNumber = c.String(),
                        City = c.String(),
                        EmailAddress = c.String(),
                        Website = c.String(),
                        OperatorName = c.String(),
                        PaidAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BankSlipNo = c.String(),
                        BankBranchName = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetMaintenanceForExternalID);
            
            CreateTable(
                "EXCEED.FleetMaintenanceInspection",
                c => new
                    {
                        FleetMaintenanceInspectionID = c.Int(nullable: false, identity: true),
                        PlateNumber = c.String(),
                        EquipmentName = c.String(),
                        AssetNumber = c.String(),
                        FleetSetupMaintenanceCenterID = c.Int(nullable: false),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        FleetMaintenanceJobOrderID = c.Int(nullable: false),
                        MaintenanceCategory = c.Int(nullable: false),
                        Reading = c.String(),
                        OpeningDate = c.DateTime(nullable: false),
                        OrderdBy = c.String(nullable: false),
                        JobOrderApproval = c.Int(nullable: false),
                        InspectedDate = c.DateTime(nullable: false),
                        InspectedBy = c.String(nullable: false),
                        NoteOnMaintenanceInspection = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetMaintenanceInspectionID)
                .ForeignKey("EXCEED.FleetMaintenanceJobOrder", t => t.FleetMaintenanceJobOrderID, cascadeDelete: true)
                .Index(t => t.FleetMaintenanceJobOrderID);
            
            CreateTable(
                "EXCEED.FleetMaintenanceJobOrder",
                c => new
                    {
                        FleetMaintenanceJobOrderID = c.Int(nullable: false, identity: true),
                        PlateNumber = c.String(),
                        EquipmentName = c.String(),
                        AssetNumber = c.String(maxLength: 128),
                        FleetSetupMaintenanceCenterID = c.Int(nullable: false),
                        Location = c.String(nullable: false),
                        FleetSetupRepairTypeID = c.Int(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        OrderdBy = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        OrderdDate = c.DateTime(nullable: false),
                        NoteOnMaintenanceJobOrder = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetMaintenanceJobOrderID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .ForeignKey("EXCEED.FleetSetupMaintenanceCenter", t => t.FleetSetupMaintenanceCenterID, cascadeDelete: true)
                .ForeignKey("EXCEED.FleetSetupRepairType", t => t.FleetSetupRepairTypeID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetSetupMaintenanceCenterID)
                .Index(t => t.FleetSetupRepairTypeID)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.FleetSetupMaintenanceCenter",
                c => new
                    {
                        FleetSetupMaintenanceCenterID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Note = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        MaintainanceCenterAddress_FleetAddressRegistrationID = c.Int(),
                    })
                .PrimaryKey(t => t.FleetSetupMaintenanceCenterID)
                .ForeignKey("EXCEED.FleetAddressRegistration", t => t.MaintainanceCenterAddress_FleetAddressRegistrationID)
                .Index(t => t.MaintainanceCenterAddress_FleetAddressRegistrationID);
            
            CreateTable(
                "EXCEED.FleetAddressRegistration",
                c => new
                    {
                        FleetAddressRegistrationID = c.Int(nullable: false, identity: true),
                        MobileNumber = c.String(),
                        HousePhoneNumber = c.String(),
                        City = c.String(),
                        EmailAddress = c.String(),
                        Website = c.String(),
                    })
                .PrimaryKey(t => t.FleetAddressRegistrationID);
            
            CreateTable(
                "EXCEED.FleetSetupRepairType",
                c => new
                    {
                        FleetSetupRepairTypeID = c.Int(nullable: false, identity: true),
                        RepairType = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupRepairTypeID);
            
            CreateTable(
                "EXCEED.FleetMaintenanceMechanic",
                c => new
                    {
                        FleetMaintenanceMechanicID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        FleetSetupMaintenanceCenterID = c.Int(nullable: false),
                        Location = c.String(),
                        MaintenanceCategory = c.Int(nullable: false),
                        FleetSetupRepairTypeID = c.Int(nullable: false),
                        EnteredDate = c.DateTime(nullable: false),
                        StartedDate = c.DateTime(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        ExecutedBy = c.String(nullable: false),
                        SupervisedBy = c.String(),
                        SparePart = c.String(),
                        PartNo = c.String(),
                        SparePartQuantity = c.Int(nullable: false),
                        SparePartUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SparePartTotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SparePartReceiptNo = c.String(),
                        OilGrease = c.String(),
                        UOM = c.String(),
                        LubricantsQuantity = c.Int(nullable: false),
                        LubricantsUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubricantsTotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubricantsReceiptNo = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetMaintenanceMechanicID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetSetupMaintenanceCenter", t => t.FleetSetupMaintenanceCenterID, cascadeDelete: true)
                .ForeignKey("EXCEED.FleetSetupRepairType", t => t.FleetSetupRepairTypeID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetSetupMaintenanceCenterID)
                .Index(t => t.FleetSetupRepairTypeID);
            
            CreateTable(
                "EXCEED.FleetMaintenanceOutsourced",
                c => new
                    {
                        FleetMaintenanceOutsourcedID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        FleetSetupRepairTypeID = c.Int(nullable: false),
                        StartedDate = c.DateTime(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        ReasonOfRepair = c.String(),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AuthorizedBy = c.String(nullable: false),
                        NoteOnMaintenanceOutsourced = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetMaintenanceOutsourcedID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetSetupRepairType", t => t.FleetSetupRepairTypeID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetSetupRepairTypeID);
            
            CreateTable(
                "EXCEED.FleetMaintenancePlanner",
                c => new
                    {
                        FleetMaintenancePlannerID = c.Int(nullable: false, identity: true),
                        PlateNumber = c.String(),
                        EquipmentName = c.String(),
                        AssetNumber = c.String(),
                        FleetSetupMaintenanceCenterID = c.Int(nullable: false),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        FleetMaintenanceJobOrderID = c.Int(nullable: false),
                        FleetMaintenanceInspectionID = c.Int(nullable: false),
                        MaintenanceWorkStatus = c.Int(nullable: false),
                        ClosingDate = c.DateTime(nullable: false),
                        MaintainedBy = c.String(nullable: false),
                        NoteOnMaintenancePlanner = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetMaintenancePlannerID)
                .ForeignKey("EXCEED.FleetMaintenanceInspection", t => t.FleetMaintenanceInspectionID, cascadeDelete: true)
                .Index(t => t.FleetMaintenanceInspectionID);
            
            CreateTable(
                "EXCEED.FleetMaintenanceTool",
                c => new
                    {
                        FleetMaintenanceToolID = c.Int(nullable: false, identity: true),
                        ToolName = c.String(nullable: false),
                        UOM = c.String(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        OwnedBy = c.String(),
                        Location = c.String(),
                        NoteOnMaintenanceTools = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetMaintenanceToolID);
            
            CreateTable(
                "EXCEED.FleetResourcesDistribution",
                c => new
                    {
                        FleetResourcesDistributionID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        RequestQuantity = c.Int(nullable: false),
                        AssignedQuantity = c.Int(nullable: false),
                        EquipmentOwner = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        TotalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssignedBy = c.String(nullable: false),
                        OperatorID = c.Int(nullable: false),
                        AssistantOperatorID = c.Int(nullable: false),
                        AssignedDate = c.DateTime(nullable: false),
                        NoteOnResourcesDistribution = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetResourcesDistributionID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetResourcesRequisitions",
                c => new
                    {
                        FleetResourcesRequisitionsID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        FleetEquipmentRegistrationID = c.Int(nullable: false),
                        RequestedBy = c.String(nullable: false),
                        RequestedDate = c.DateTime(nullable: false),
                        Capacity = c.String(),
                        RequestQuantity = c.Int(nullable: false),
                        ProjectName = c.String(nullable: false),
                        ProjectLocation = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        TotalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedBy = c.String(),
                        NoteOnResourcesRequisitions = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetResourcesRequisitionsID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetResourcesReturn",
                c => new
                    {
                        FleetResourcesReturnID = c.Int(nullable: false, identity: true),
                        ReturnedBy = c.String(nullable: false),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        FleetEquipmentRegistrationID = c.Int(nullable: false),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        ReturnTime = c.String(),
                        MajorReasonForReturn = c.Int(nullable: false),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetResourcesReturnID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.FleetResourcesTransferBetweenDrivers",
                c => new
                    {
                        FleetResourcesTransferBetweenDriversID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        FleetEquipmentRegistrationID = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        TotalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransferBy = c.String(nullable: false),
                        FromOperatorID = c.Int(nullable: false),
                        ToOperatorID = c.Int(nullable: false),
                        TransferredDate = c.DateTime(nullable: false),
                        NoteOnResourcesTransferBetweenDrivers = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetResourcesTransferBetweenDriversID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetResourcesTransferBetweenSites",
                c => new
                    {
                        FleetResourcesTransferBetweenSitesID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        FleetEquipmentRegistrationID = c.Int(nullable: false),
                        TransferredBY = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        TotalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FromLocation = c.String(nullable: false),
                        ToLocation = c.String(nullable: false),
                        NoteOnResourcesTransferSite = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetResourcesTransferBetweenSitesID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetSalvageStoreGRN",
                c => new
                    {
                        FleetSalvageStoreGRNID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        ReceivedDate = c.DateTime(nullable: false),
                        VoucherNo = c.Int(nullable: false),
                        PartNo = c.Int(nullable: false),
                        ItemNo = c.Int(nullable: false),
                        ItemDescription = c.String(),
                        UOM = c.String(),
                        Quantity = c.Int(nullable: false),
                        Note = c.String(),
                        ReceivedBy = c.String(),
                        ReturnBy = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSalvageStoreGRNID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetSalvageStoreIssue",
                c => new
                    {
                        FleetSalvageStoreIssueID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        FleetEquipmentRegistrationID = c.Int(nullable: false),
                        ReceivedDate = c.DateTime(nullable: false),
                        VoucherNo = c.Int(nullable: false),
                        PartNo = c.Int(nullable: false),
                        ItemNo = c.Int(nullable: false),
                        ItemDescription = c.String(),
                        UOM = c.String(),
                        Quantity = c.Int(nullable: false),
                        Description = c.String(),
                        ReceivedBy = c.String(),
                        IssuedBy = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSalvageStoreIssueID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .Index(t => t.AssetNumber);
            
            CreateTable(
                "EXCEED.FleetSetupEmploymentType",
                c => new
                    {
                        FleetSetupEmploymentTypeID = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupEmploymentTypeID);
            
            CreateTable(
                "EXCEED.FleetSetupEquipmentFuelStandard",
                c => new
                    {
                        FleetSetupEquipmentFuelStandardID = c.Int(nullable: false, identity: true),
                        FuelStandard = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupEquipmentFuelStandardID);
            
            CreateTable(
                "EXCEED.FleetSetupEquipmentManufacturer",
                c => new
                    {
                        FleetSetupEquipmentManufacturerID = c.Int(nullable: false, identity: true),
                        Manufacturer = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupEquipmentManufacturerID);
            
            CreateTable(
                "EXCEED.FleetSetupRentalRate",
                c => new
                    {
                        FleetSetupRentalRateID = c.Int(nullable: false, identity: true),
                        RentRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetSetupRentalRateID);
            
            CreateTable(
                "EXCEED.FleetTyreManagementIssue",
                c => new
                    {
                        FleetTyreManagementIssueID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        FleetEquipmentRegistrationID = c.Int(nullable: false),
                        TireType = c.Int(nullable: false),
                        Brand = c.String(),
                        SerialNo = c.String(),
                        TyreTube = c.Int(nullable: false),
                        KMReading = c.String(),
                        Reason = c.String(),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Int(nullable: false),
                        InstalledDate = c.DateTime(nullable: false),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        ApprovedBy = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetTyreManagementIssueID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.FleetTyreManagementReturn",
                c => new
                    {
                        FleetTyreManagementReturnID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        EquipmentName = c.String(),
                        PlateNumber = c.String(),
                        FleetEquipmentRegistrationID = c.Int(nullable: false),
                        TireType = c.Int(nullable: false),
                        Brand = c.String(),
                        SerialNo = c.String(),
                        TyreTube = c.Int(nullable: false),
                        KMReading = c.String(),
                        Reason = c.String(),
                        Quantity = c.Int(nullable: false),
                        FleetOperatorRegistrationID = c.Int(nullable: false),
                        ApprovedBy = c.String(),
                        Note = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetTyreManagementReturnID)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.FleetOperatorRegistration", t => t.FleetOperatorRegistrationID, cascadeDelete: true)
                .Index(t => t.AssetNumber)
                .Index(t => t.FleetOperatorRegistrationID);
            
            CreateTable(
                "EXCEED.ForeignPurchase",
                c => new
                    {
                        ForeignPurchaseId = c.Int(nullable: false, identity: true),
                        ItemCode = c.String(),
                        Unit = c.String(),
                        Quantity = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LcReference = c.String(),
                        InsurancePolicy = c.String(),
                        TransitOperation = c.String(),
                        Status = c.String(),
                        CNo = c.String(),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrnNo = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseId);
            
            CreateTable(
                "EXCEED.GapBalanceSheetRange",
                c => new
                    {
                        GapBalanceSheetRangeId = c.Int(nullable: false, identity: true),
                        GapBalanceSheetId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapBalanceSheetRangeId)
                .ForeignKey("EXCEED.GapBalanceSheet", t => t.GapBalanceSheetId, cascadeDelete: true)
                .Index(t => t.GapBalanceSheetId);
            
            CreateTable(
                "EXCEED.GapBalanceSheet",
                c => new
                    {
                        GapBalanceSheetId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Parent = c.String(),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapBalanceSheetId);
            
            CreateTable(
                "EXCEED.GapCashFlowRange",
                c => new
                    {
                        GapCashFlowRangeId = c.Int(nullable: false, identity: true),
                        GapCashFlowId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapCashFlowRangeId)
                .ForeignKey("EXCEED.GapCashFlow", t => t.GapCashFlowId, cascadeDelete: true)
                .Index(t => t.GapCashFlowId);
            
            CreateTable(
                "EXCEED.GapCashFlow",
                c => new
                    {
                        GapCashFlowId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapCashFlowId);
            
            CreateTable(
                "EXCEED.GapIncomeStatementByProjectRange",
                c => new
                    {
                        GapIncomeStatementByProjectRangeId = c.Int(nullable: false, identity: true),
                        GapIncomeStatementByProjectId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapIncomeStatementByProjectRangeId)
                .ForeignKey("EXCEED.GapIncomeStatementByProject", t => t.GapIncomeStatementByProjectId, cascadeDelete: true)
                .Index(t => t.GapIncomeStatementByProjectId);
            
            CreateTable(
                "EXCEED.GapIncomeStatementByProject",
                c => new
                    {
                        GapIncomeStatementByProjectId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Parent = c.String(),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapIncomeStatementByProjectId);
            
            CreateTable(
                "EXCEED.GapIncomeStatementRange",
                c => new
                    {
                        GapIncomeStatementRangeId = c.Int(nullable: false, identity: true),
                        GapIncomeStatementId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapIncomeStatementRangeId)
                .ForeignKey("EXCEED.GapIncomeStatement", t => t.GapIncomeStatementId, cascadeDelete: true)
                .Index(t => t.GapIncomeStatementId);
            
            CreateTable(
                "EXCEED.GapIncomeStatement",
                c => new
                    {
                        GapIncomeStatementId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Parent = c.String(),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapIncomeStatementId);
            
            CreateTable(
                "EXCEED.GLChartOfAccountAccount",
                c => new
                    {
                        Values = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                        HasParent = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Parent = c.Boolean(nullable: false),
                        AllowBudgeting = c.Boolean(nullable: false),
                        AllowPosting = c.Boolean(nullable: false),
                        AccountType = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Values);
            
            CreateTable(
                "EXCEED.GLChartOfAccountCostCenter",
                c => new
                    {
                        Values = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        HasParent = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Parent = c.Boolean(nullable: false),
                        AllowBudgeting = c.Boolean(nullable: false),
                        AllowPosting = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Values);
            
            CreateTable(
                "EXCEED.GLChartOfAccountLocation",
                c => new
                    {
                        Values = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        HasParent = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Parent = c.Boolean(nullable: false),
                        AllowBudgeting = c.Boolean(nullable: false),
                        AllowPosting = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Values);
            
            CreateTable(
                "EXCEED.GLChartOfAccountSubAccount",
                c => new
                    {
                        Values = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        HasParent = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Parent = c.Boolean(nullable: false),
                        AllowBudgeting = c.Boolean(nullable: false),
                        AllowPosting = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Values);
            
            CreateTable(
                "EXCEED.GLCountryType",
                c => new
                    {
                        GLCountryTypeID = c.Int(nullable: false, identity: true),
                        CountryName = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLCountryTypeID);
            
            CreateTable(
                "EXCEED.GLJournalCategoriers",
                c => new
                    {
                        GLJournalCategoriersID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLJournalCategoriersID);
            
            CreateTable(
                "EXCEED.GLJournalSources",
                c => new
                    {
                        GLJournalSourcesID = c.Int(nullable: false, identity: true),
                        Sources = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        JournalRefernces = c.Boolean(nullable: false),
                        JournalApproval = c.Boolean(nullable: false),
                        FreezJournal = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLJournalSourcesID);
            
            CreateTable(
                "EXCEED.GLLedgerPostingLines",
                c => new
                    {
                        GLLedgerPostingLinesID = c.Int(nullable: false, identity: true),
                        GLLedgerPostingID = c.Int(nullable: false),
                        JournalNumber = c.String(),
                        EffectiveDate = c.DateTime(nullable: false),
                        Period = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLLedgerPostingLinesID)
                .ForeignKey("EXCEED.GLLedgerPosting", t => t.GLLedgerPostingID, cascadeDelete: true)
                .Index(t => t.GLLedgerPostingID);
            
            CreateTable(
                "EXCEED.GLLedgerPosting",
                c => new
                    {
                        GLLedgerPostingID = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AccountType = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLLedgerPostingID);
            
            CreateTable(
                "EXCEED.GlobalBranchsetup",
                c => new
                    {
                        GlobalBranchsetupID = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        OrgBranchName = c.String(nullable: false),
                        OrgBranchAbbreviation = c.String(nullable: false),
                        BranchAccountCode = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.GlobalBranchsetupID);
            
            CreateTable(
                "EXCEED.GlobalOrgInformation",
                c => new
                    {
                        GlobalOrgInformationID = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Name = c.String(nullable: false),
                        Abbreviation = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Email = c.String(),
                        Fax = c.String(),
                        Address = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.GlobalOrgInformationID);
            
            CreateTable(
                "EXCEED.GlobalUserAccessLog",
                c => new
                    {
                        GlobalUserAccessLogID = c.Int(nullable: false, identity: true),
                        IPAddress = c.String(),
                        UserName = c.String(),
                        TimeAccessed = c.DateTime(nullable: false),
                        Type = c.String(),
                        Status = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.GlobalUserAccessLogID);
            
            CreateTable(
                "EXCEED.GLRecordJournalDetail",
                c => new
                    {
                        GlRecordJournalDetailId = c.Int(nullable: false, identity: true),
                        GlRecordJournalId = c.Int(nullable: false),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalDescription = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlRecordJournalDetailId)
                .ForeignKey("EXCEED.GlRecordJournal", t => t.GlRecordJournalId, cascadeDelete: true)
                .Index(t => t.GlRecordJournalId);
            
            CreateTable(
                "EXCEED.GlRecordJournal",
                c => new
                    {
                        GlRecordJournalId = c.Int(nullable: false, identity: true),
                        JournalName = c.String(nullable: false),
                        Description = c.String(),
                        FiscalYear = c.String(),
                        PeriodName = c.String(nullable: false),
                        CategoryName = c.String(nullable: false),
                        SourcesName = c.String(),
                        JournalNumber = c.String(),
                        EffectiveDate = c.DateTime(nullable: false),
                        Posting = c.String(),
                        isReversed = c.Boolean(nullable: false),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isPosted = c.Boolean(nullable: false),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlRecordJournalId);
            
            CreateTable(
                "EXCEED.GLRecordJournalValidation",
                c => new
                    {
                        GlRecordJournalValidationId = c.Int(nullable: false, identity: true),
                        GlRecordJournalId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlRecordJournalValidationId)
                .ForeignKey("EXCEED.GlRecordJournal", t => t.GlRecordJournalId, cascadeDelete: true)
                .Index(t => t.GlRecordJournalId);
            
            CreateTable(
                "EXCEED.GlSegmentLength",
                c => new
                    {
                        GlSegmentLengthId = c.Int(nullable: false, identity: true),
                        AccountLength = c.Int(nullable: false),
                        LocationLength = c.Int(nullable: false),
                        CostCenterLength = c.Int(nullable: false),
                        SubAccountLength = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlSegmentLengthId);
            
            CreateTable(
                "EXCEED.GlSegmentSequence",
                c => new
                    {
                        GlSegmentSequenceId = c.Int(nullable: false, identity: true),
                        FirstSegment = c.Int(nullable: false),
                        SecondSegment = c.Int(nullable: false),
                        ThirdSegment = c.Int(nullable: false),
                        FourthSegment = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlSegmentSequenceId);
            
            CreateTable(
                "EXCEED.GLTaxRate",
                c => new
                    {
                        GLTaxRateID = c.Int(nullable: false, identity: true),
                        TaxName = c.String(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApplyToAr = c.Boolean(nullable: false),
                        ApplyToAp = c.Boolean(nullable: false),
                        ApplyToWithholding = c.Boolean(nullable: false),
                        ArAccountCode = c.String(),
                        ApAccountCode = c.String(),
                        WithholdingAccountCode = c.String(),
                        Enable = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLTaxRateID);
            
            CreateTable(
                "EXCEED.GLVatWithholding",
                c => new
                    {
                        GLVatWithholdingID = c.Int(nullable: false, identity: true),
                        WithholdingAccountCode = c.String(),
                        Branch = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLVatWithholdingID);
            
            CreateTable(
                "EXCEED.GoodReceiveDistribution",
                c => new
                    {
                        GoodReceiveDistributionID = c.Int(nullable: false, identity: true),
                        GoodReceiveId = c.Int(nullable: false),
                        GoodReceiveItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.GoodReceiveDistributionID)
                .ForeignKey("EXCEED.GoodReceive", t => t.GoodReceiveId, cascadeDelete: true)
                .Index(t => t.GoodReceiveId);
            
            CreateTable(
                "EXCEED.GoodReceive",
                c => new
                    {
                        GoodReceiveId = c.Int(nullable: false, identity: true),
                        PurchaseOrderID = c.Int(nullable: false),
                        StoreCode = c.String(nullable: false),
                        SupplierName = c.String(),
                        SupplierInvoiceNo = c.String(),
                        FiscalYear = c.DateTime(nullable: false),
                        Period = c.String(),
                        Reference = c.String(),
                        ReceiveDate = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodReceiveId)
                .ForeignKey("EXCEED.PurchaseOrder", t => t.PurchaseOrderID, cascadeDelete: true)
                .Index(t => t.PurchaseOrderID);
            
            CreateTable(
                "EXCEED.GoodReceiveItem",
                c => new
                    {
                        GoodReceiveItemID = c.Int(nullable: false, identity: true),
                        GoodReceiveId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxType = c.String(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsExtra = c.Boolean(nullable: false),
                        ExtraQuantity = c.Decimal(precision: 18, scale: 2),
                        ExtraAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WarrantyInformation = c.String(),
                        Specification = c.String(),
                        Expire = c.Boolean(nullable: false),
                        ExpireDate = c.DateTime(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsMachinery = c.Boolean(nullable: false),
                        BatchNo = c.String(),
                        Model = c.String(),
                        SerialNo = c.String(),
                        Condition = c.String(),
                        PlateNo = c.String(),
                        ChassisNo = c.String(),
                        EngineNo = c.String(),
                        EngineModelNo = c.String(),
                        EngineElectricalNo = c.String(),
                        EngineElectricalModelNo = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodReceiveItemID)
                .ForeignKey("EXCEED.GoodReceive", t => t.GoodReceiveId, cascadeDelete: true)
                .Index(t => t.GoodReceiveId);
            
            CreateTable(
                "EXCEED.GoodReceiveValidation",
                c => new
                    {
                        GoodReceiveValidationID = c.Int(nullable: false, identity: true),
                        GoodReceiveId = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodReceiveValidationID)
                .ForeignKey("EXCEED.GoodReceive", t => t.GoodReceiveId, cascadeDelete: true)
                .Index(t => t.GoodReceiveId);
            
            CreateTable(
                "EXCEED.ItemInspection",
                c => new
                    {
                        ItemInspectionId = c.Int(nullable: false, identity: true),
                        InspectionDate = c.DateTime(nullable: false),
                        StoreCode = c.String(nullable: false),
                        CommitteeMember1 = c.String(),
                        CommitteeMember2 = c.String(),
                        CommitteeMember3 = c.String(),
                        CommitteeDecision = c.String(nullable: false),
                        GoodReceiveId = c.Int(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        GoodReceiveDonation_GoodReceiveDonationId = c.Int(),
                    })
                .PrimaryKey(t => t.ItemInspectionId)
                .ForeignKey("EXCEED.GoodReceive", t => t.GoodReceiveId, cascadeDelete: true)
                .ForeignKey("EXCEED.GoodReceiveDonation", t => t.GoodReceiveDonation_GoodReceiveDonationId)
                .Index(t => t.GoodReceiveId)
                .Index(t => t.GoodReceiveDonation_GoodReceiveDonationId);
            
            CreateTable(
                "EXCEED.PurchaseOrder",
                c => new
                    {
                        PurchaseOrderID = c.Int(nullable: false, identity: true),
                        Supplier = c.String(nullable: false),
                        FiscalYear = c.String(),
                        Period = c.String(),
                        POType = c.Int(nullable: false),
                        ProcurementMethod = c.Int(nullable: false),
                        PRNo = c.Int(nullable: false),
                        PONumber = c.String(),
                        OrderDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        CostCenter = c.String(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseOrderID);
            
            CreateTable(
                "EXCEED.PurchaseOrderDistribution",
                c => new
                    {
                        PurchaseOrderDistributionID = c.Int(nullable: false, identity: true),
                        PurchaseOrderID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseOrderDistributionID)
                .ForeignKey("EXCEED.PurchaseOrder", t => t.PurchaseOrderID, cascadeDelete: true)
                .Index(t => t.PurchaseOrderID);
            
            CreateTable(
                "EXCEED.PurchaseOrderItem",
                c => new
                    {
                        PurchaseOrderItemID = c.Int(nullable: false, identity: true),
                        PurchaseOrderID = c.Int(nullable: false),
                        ItemCategory = c.String(),
                        InventoryItem = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxType = c.String(),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseOrderItemID)
                .ForeignKey("EXCEED.PurchaseOrder", t => t.PurchaseOrderID, cascadeDelete: true)
                .Index(t => t.PurchaseOrderID);
            
            CreateTable(
                "EXCEED.PurchaseOrderValidation",
                c => new
                    {
                        PurchaseOrderValidationID = c.Int(nullable: false, identity: true),
                        PurchaseOrderID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseOrderValidationID)
                .ForeignKey("EXCEED.PurchaseOrder", t => t.PurchaseOrderID, cascadeDelete: true)
                .Index(t => t.PurchaseOrderID);
            
            CreateTable(
                "EXCEED.GoodReceiveDonationItem",
                c => new
                    {
                        GoodReceiveDonationItemID = c.Int(nullable: false, identity: true),
                        GoodReceiveDonationId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxType = c.String(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WarrantyInformation = c.String(),
                        Specification = c.String(),
                        Expire = c.Boolean(nullable: false),
                        ExpireDate = c.DateTime(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsMachinery = c.Boolean(nullable: false),
                        BatchNo = c.String(),
                        Model = c.String(),
                        SerialNo = c.String(),
                        Condition = c.String(),
                        PlateNo = c.String(),
                        ChassisNo = c.String(),
                        EngineNo = c.String(),
                        EngineModelNo = c.String(),
                        EngineElectricalNo = c.String(),
                        EngineElectricalModelNo = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodReceiveDonationItemID)
                .ForeignKey("EXCEED.GoodReceiveDonation", t => t.GoodReceiveDonationId, cascadeDelete: true)
                .Index(t => t.GoodReceiveDonationId);
            
            CreateTable(
                "EXCEED.GoodReceiveDonation",
                c => new
                    {
                        GoodReceiveDonationId = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(nullable: false),
                        SupplierName = c.String(),
                        SupplierInvoiceNo = c.String(),
                        FiscalYear = c.DateTime(nullable: false),
                        Period = c.String(),
                        Reference = c.String(),
                        ReceiveDate = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodReceiveDonationId);
            
            CreateTable(
                "EXCEED.GoodReciveDonationValidation",
                c => new
                    {
                        GoodReciveDonationValidationID = c.Int(nullable: false, identity: true),
                        GoodReceiveDonationId = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodReciveDonationValidationID)
                .ForeignKey("EXCEED.GoodReceiveDonation", t => t.GoodReceiveDonationId, cascadeDelete: true)
                .Index(t => t.GoodReceiveDonationId);
            
            CreateTable(
                "EXCEED.GoodReciveDonationDistribution",
                c => new
                    {
                        GoodReciveDonationDistributionID = c.Int(nullable: false, identity: true),
                        GoodReceiveDonationId = c.Int(nullable: false),
                        GoodReceiveItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodReciveDonationDistributionID)
                .ForeignKey("EXCEED.GoodReceiveDonation", t => t.GoodReceiveDonationId, cascadeDelete: true)
                .Index(t => t.GoodReceiveDonationId);
            
            CreateTable(
                "EXCEED.GoodSellingAndLendingDistribution",
                c => new
                    {
                        GoodSellingAndLendingDistributionID = c.Int(nullable: false, identity: true),
                        GoodSellingAndLendingID = c.Int(nullable: false),
                        GoodSellingAndLendingItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.GoodSellingAndLendingDistributionID)
                .ForeignKey("EXCEED.GoodSellingAndLending", t => t.GoodSellingAndLendingID, cascadeDelete: true)
                .Index(t => t.GoodSellingAndLendingID);
            
            CreateTable(
                "EXCEED.GoodSellingAndLending",
                c => new
                    {
                        GoodSellingAndLendingID = c.Int(nullable: false, identity: true),
                        SellOrLend = c.String(nullable: false),
                        StoreCode = c.String(nullable: false),
                        Buyer = c.String(nullable: false),
                        BuyerAddress = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodSellingAndLendingID);
            
            CreateTable(
                "EXCEED.GoodSellingAndLendingItem",
                c => new
                    {
                        GoodSellingAndLendingItemID = c.Int(nullable: false, identity: true),
                        GoodSellingAndLendingID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        QuantitySold = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPurchasingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitSellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalSellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLAccountCode = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodSellingAndLendingItemID)
                .ForeignKey("EXCEED.GoodSellingAndLending", t => t.GoodSellingAndLendingID, cascadeDelete: true)
                .Index(t => t.GoodSellingAndLendingID);
            
            CreateTable(
                "EXCEED.GoodSellingAndLendingValidation",
                c => new
                    {
                        GoodSellingAndLendingValidationID = c.Int(nullable: false, identity: true),
                        GoodSellingAndLendingID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodSellingAndLendingValidationID)
                .ForeignKey("EXCEED.GoodSellingAndLending", t => t.GoodSellingAndLendingID, cascadeDelete: true)
                .Index(t => t.GoodSellingAndLendingID);
            
            CreateTable(
                "common.Identification",
                c => new
                    {
                        IdentificationId = c.Int(nullable: false, identity: true),
                        ReferenceType = c.String(),
                        Reference = c.String(),
                        Description = c.String(),
                        IdNumber = c.String(),
                        Type = c.Int(nullable: false),
                        IssueDate = c.DateTime(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false),
                        Issuer = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.IdentificationId);
            
            CreateTable(
                "EXCEED.IfrsScoiContinuingOperationRange",
                c => new
                    {
                        IfrsScoiContinuingOperationRangeId = c.Int(nullable: false, identity: true),
                        IfrsScoiContinuingOperationId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsScoiContinuingOperationRangeId)
                .ForeignKey("EXCEED.IfrsScoiContinuingOperation", t => t.IfrsScoiContinuingOperationId, cascadeDelete: true)
                .Index(t => t.IfrsScoiContinuingOperationId);
            
            CreateTable(
                "EXCEED.IfrsScoiContinuingOperation",
                c => new
                    {
                        IfrsScoiContinuingOperationId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfComprehensiveIncomeId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsScoiContinuingOperationId)
                .ForeignKey("EXCEED.IfrsStatementOfComprehensiveIncome", t => t.IfrsStatementOfComprehensiveIncomeId, cascadeDelete: true)
                .Index(t => t.IfrsStatementOfComprehensiveIncomeId);
            
            CreateTable(
                "EXCEED.IfrsStatementOfComprehensiveIncome",
                c => new
                    {
                        IfrsStatementOfComprehensiveIncomeId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.IfrsStatementOfComprehensiveIncomeId);
            
            CreateTable(
                "EXCEED.IfrsScoiDiscontinuedOperation",
                c => new
                    {
                        IfrsScoiDiscontinuedOperationId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfComprehensiveIncomeId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsScoiDiscontinuedOperationId)
                .ForeignKey("EXCEED.IfrsStatementOfComprehensiveIncome", t => t.IfrsStatementOfComprehensiveIncomeId, cascadeDelete: true)
                .Index(t => t.IfrsStatementOfComprehensiveIncomeId);
            
            CreateTable(
                "EXCEED.IfrsScoiDiscontinuedOperationRange",
                c => new
                    {
                        IfrsScoiDiscontinuedOperationRangeId = c.Int(nullable: false, identity: true),
                        IfrsScoiDiscontinuedOperationId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsScoiDiscontinuedOperationRangeId)
                .ForeignKey("EXCEED.IfrsScoiDiscontinuedOperation", t => t.IfrsScoiDiscontinuedOperationId, cascadeDelete: true)
                .Index(t => t.IfrsScoiDiscontinuedOperationId);
            
            CreateTable(
                "EXCEED.IfrsScoiOtherComprehensiveIncome",
                c => new
                    {
                        IfrsScoiOtherComprehensiveIncomeId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfComprehensiveIncomeId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsScoiOtherComprehensiveIncomeId)
                .ForeignKey("EXCEED.IfrsStatementOfComprehensiveIncome", t => t.IfrsStatementOfComprehensiveIncomeId, cascadeDelete: true)
                .Index(t => t.IfrsStatementOfComprehensiveIncomeId);
            
            CreateTable(
                "EXCEED.IfrsScoiOtherComprehensiveIncomeRange",
                c => new
                    {
                        IfrsScoiOtherComprehensiveIncomeRangeId = c.Int(nullable: false, identity: true),
                        IfrsScoiOtherComprehensiveIncomeId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsScoiOtherComprehensiveIncomeRangeId)
                .ForeignKey("EXCEED.IfrsScoiOtherComprehensiveIncome", t => t.IfrsScoiOtherComprehensiveIncomeId, cascadeDelete: true)
                .Index(t => t.IfrsScoiOtherComprehensiveIncomeId);
            
            CreateTable(
                "EXCEED.IfrsSocfFinancingActivity",
                c => new
                    {
                        IfrsSocfFinancingActivityId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfCashFlowId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsSocfFinancingActivityId)
                .ForeignKey("EXCEED.IfrsStatementOfCashFlow", t => t.IfrsStatementOfCashFlowId, cascadeDelete: true)
                .Index(t => t.IfrsStatementOfCashFlowId);
            
            CreateTable(
                "EXCEED.IfrsSocfFinancingActivityRange",
                c => new
                    {
                        IfrsSocfFinancingActivityRangeId = c.Int(nullable: false, identity: true),
                        IfrsSocfFinancingActivityId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsSocfFinancingActivityRangeId)
                .ForeignKey("EXCEED.IfrsSocfFinancingActivity", t => t.IfrsSocfFinancingActivityId, cascadeDelete: true)
                .Index(t => t.IfrsSocfFinancingActivityId);
            
            CreateTable(
                "EXCEED.IfrsStatementOfCashFlow",
                c => new
                    {
                        IfrsStatementOfCashFlowId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.IfrsStatementOfCashFlowId);
            
            CreateTable(
                "EXCEED.IfrsSocfInvestingActivity",
                c => new
                    {
                        IfrsSocfInvestingActivityId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfCashFlowId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsSocfInvestingActivityId)
                .ForeignKey("EXCEED.IfrsStatementOfCashFlow", t => t.IfrsStatementOfCashFlowId, cascadeDelete: true)
                .Index(t => t.IfrsStatementOfCashFlowId);
            
            CreateTable(
                "EXCEED.IfrsSocfInvestingActivityRange",
                c => new
                    {
                        IfrsSocfInvestingActivityRangeId = c.Int(nullable: false, identity: true),
                        IfrsSocfInvestingActivityId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsSocfInvestingActivityRangeId)
                .ForeignKey("EXCEED.IfrsSocfInvestingActivity", t => t.IfrsSocfInvestingActivityId, cascadeDelete: true)
                .Index(t => t.IfrsSocfInvestingActivityId);
            
            CreateTable(
                "EXCEED.IfrsSocfOperatingActivity",
                c => new
                    {
                        IfrsSocfOperatingActivityId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfCashFlowId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsSocfOperatingActivityId)
                .ForeignKey("EXCEED.IfrsStatementOfCashFlow", t => t.IfrsStatementOfCashFlowId, cascadeDelete: true)
                .Index(t => t.IfrsStatementOfCashFlowId);
            
            CreateTable(
                "EXCEED.IfrsSocfOperatingActivityRange",
                c => new
                    {
                        IfrsSocfOperatingActivityRangeId = c.Int(nullable: false, identity: true),
                        IfrsSocfOperatingActivityId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsSocfOperatingActivityRangeId)
                .ForeignKey("EXCEED.IfrsSocfOperatingActivity", t => t.IfrsSocfOperatingActivityId, cascadeDelete: true)
                .Index(t => t.IfrsSocfOperatingActivityId);
            
            CreateTable(
                "EXCEED.IfrsSofpAssetRange",
                c => new
                    {
                        IfrsSofpAssetRangeId = c.Int(nullable: false, identity: true),
                        IfrsSofpAssetId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsSofpAssetRangeId)
                .ForeignKey("EXCEED.IfrsSofpAsset", t => t.IfrsSofpAssetId, cascadeDelete: true)
                .Index(t => t.IfrsSofpAssetId);
            
            CreateTable(
                "EXCEED.IfrsSofpAsset",
                c => new
                    {
                        IfrsSofpAssetId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfFinancialPositionId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsSofpAssetId)
                .ForeignKey("EXCEED.IfrsStatementOfFinancialPosition", t => t.IfrsStatementOfFinancialPositionId, cascadeDelete: true)
                .Index(t => t.IfrsStatementOfFinancialPositionId);
            
            CreateTable(
                "EXCEED.IfrsStatementOfFinancialPosition",
                c => new
                    {
                        IfrsStatementOfFinancialPositionId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.IfrsStatementOfFinancialPositionId);
            
            CreateTable(
                "EXCEED.IfrsSofpEquityAndLiablity",
                c => new
                    {
                        IfrsSofpEquityAndLiablityId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfFinancialPositionId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        IfrsStatementOfChangesInEquity_IfrsStatementOfChangesInEquityId = c.Int(),
                    })
                .PrimaryKey(t => t.IfrsSofpEquityAndLiablityId)
                .ForeignKey("EXCEED.IfrsStatementOfFinancialPosition", t => t.IfrsStatementOfFinancialPositionId, cascadeDelete: true)
                .ForeignKey("EXCEED.IfrsStatementOfChangesInEquity", t => t.IfrsStatementOfChangesInEquity_IfrsStatementOfChangesInEquityId)
                .Index(t => t.IfrsStatementOfFinancialPositionId)
                .Index(t => t.IfrsStatementOfChangesInEquity_IfrsStatementOfChangesInEquityId);
            
            CreateTable(
                "EXCEED.IfrsSofpEquityAndLiablityRange",
                c => new
                    {
                        IfrsSofpEquityAndLiablityRangeId = c.Int(nullable: false, identity: true),
                        IfrsSofpEquityAndLiablityId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsSofpEquityAndLiablityRangeId)
                .ForeignKey("EXCEED.IfrsSofpEquityAndLiablity", t => t.IfrsSofpEquityAndLiablityId, cascadeDelete: true)
                .Index(t => t.IfrsSofpEquityAndLiablityId);
            
            CreateTable(
                "EXCEED.IfrsStatementOfChangesInEquity",
                c => new
                    {
                        IfrsStatementOfChangesInEquityId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsStatementOfChangesInEquityId);
            
            CreateTable(
                "EXCEED.IfrsStatementOfChangesInEquityRange",
                c => new
                    {
                        IfrsStatementOfChangesInEquityRangeId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfChangesInEquityId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IfrsStatementOfChangesInEquityRangeId)
                .ForeignKey("EXCEED.IfrsStatementOfChangesInEquity", t => t.IfrsStatementOfChangesInEquityId, cascadeDelete: true)
                .Index(t => t.IfrsStatementOfChangesInEquityId);
            
            CreateTable(
                "HRM.Incomming",
                c => new
                    {
                        IncommingId = c.Int(nullable: false, identity: true),
                        LettersRefNo = c.String(nullable: false),
                        Owner = c.String(nullable: false),
                        LetterDate = c.DateTime(nullable: false),
                        RecivedDate = c.DateTime(nullable: false),
                        Subject = c.String(nullable: false),
                        Folder = c.String(),
                        Cluster = c.String(),
                        NoOfAttachment = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.IncommingId);
            
            CreateTable(
                "HRM.IncommingStatus",
                c => new
                    {
                        IncommingStatusId = c.Int(nullable: false, identity: true),
                        IncommingId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Decsion = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.IncommingStatusId)
                .ForeignKey("HRM.Incomming", t => t.IncommingId, cascadeDelete: true)
                .Index(t => t.IncommingId);
            
            CreateTable(
                "EXCEED.IndexingCount",
                c => new
                    {
                        IndexCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        CurrentValue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IndexCode)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "common.IndustryInvolved",
                c => new
                    {
                        IndustryInvolvedID = c.Int(nullable: false, identity: true),
                        Index = c.Double(nullable: false),
                        Reference = c.String(),
                        ReferenceType = c.String(),
                        Industry = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.IndustryInvolvedID);
            
            CreateTable(
                "Archive.InstitutionArchiveNo",
                c => new
                    {
                        InstiArchiveNoId = c.Int(nullable: false, identity: true),
                        FileNo = c.String(nullable: false),
                        InstitutionName = c.String(nullable: false),
                        InstitutionTypeId = c.Int(nullable: false),
                        ZoneId = c.Int(nullable: false),
                        Shelve = c.String(nullable: false),
                        Row = c.String(nullable: false),
                        Folder = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InstiArchiveNoId)
                .ForeignKey("Archive.InstitutionType", t => t.InstitutionTypeId, cascadeDelete: true)
                .ForeignKey("Archive.Zone", t => t.ZoneId, cascadeDelete: true)
                .Index(t => t.InstitutionTypeId)
                .Index(t => t.ZoneId);
            
            CreateTable(
                "Archive.InstitutionType",
                c => new
                    {
                        InstitutionTypeId = c.Int(nullable: false, identity: true),
                        InstitutionName = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InstitutionTypeId);
            
            CreateTable(
                "Archive.Zone",
                c => new
                    {
                        ZoneId = c.Int(nullable: false, identity: true),
                        ZoneName = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ZoneId);
            
            CreateTable(
                "Insurance.Insurance",
                c => new
                    {
                        InsuranceId = c.Int(nullable: false, identity: true),
                        InsuranceGroupId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        LookupId = c.Int(nullable: false),
                        InsuranceCorporation = c.String(nullable: false),
                        Number = c.Int(nullable: false),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SumofInsured = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Premium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InsuranceId)
                .ForeignKey("Insurance.InsuranceGroup", t => t.InsuranceGroupId, cascadeDelete: true)
                .Index(t => t.InsuranceGroupId);
            
            CreateTable(
                "Insurance.InsuranceGroup",
                c => new
                    {
                        InsuranceGroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InsuranceGroupId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "EXCEED.InventoryPeriod",
                c => new
                    {
                        InventoryPeriodId = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 45),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryPeriodId)
                .Index(t => t.Description, unique: true);
            
            CreateTable(
                "EXCEED.InventoryPlan",
                c => new
                    {
                        InventoryPlanId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        GLPeriodId = c.Int(nullable: false),
                        Remark = c.String(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InventoryPlanId)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .Index(t => t.GLPeriodId);
            
            CreateTable(
                "EXCEED.PlanItem",
                c => new
                    {
                        PlanItemID = c.Int(nullable: false, identity: true),
                        InventoryPlanID = c.Int(nullable: false),
                        ItemCategory = c.String(),
                        InventoryItem = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FundSource = c.Int(nullable: false),
                        PurchaseCategory = c.String(),
                        BudgetType = c.Int(nullable: false),
                        ProcurementMethod = c.Int(nullable: false),
                        TenderFloatingTime = c.DateTime(nullable: false),
                        PurchaseDeliveryTime = c.DateTime(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PlanItemID)
                .ForeignKey("EXCEED.InventoryPlan", t => t.InventoryPlanID, cascadeDelete: true)
                .Index(t => t.InventoryPlanID);
            
            CreateTable(
                "EXCEED.PlanValidation",
                c => new
                    {
                        PlanValidationID = c.Int(nullable: false, identity: true),
                        InventoryPlanID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PlanValidationID)
                .ForeignKey("EXCEED.InventoryPlan", t => t.InventoryPlanID, cascadeDelete: true)
                .Index(t => t.InventoryPlanID);
            
            CreateTable(
                "EXCEED.IssueDistribution",
                c => new
                    {
                        IssueDistributionID = c.Int(nullable: false, identity: true),
                        IssueID = c.Int(nullable: false),
                        IssueItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IssueDistributionID)
                .ForeignKey("EXCEED.Issue", t => t.IssueID, cascadeDelete: true)
                .Index(t => t.IssueID);
            
            CreateTable(
                "EXCEED.Issue",
                c => new
                    {
                        IssueID = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(nullable: false),
                        StoreRequisitionID = c.Int(nullable: false),
                        FiscalYear = c.String(),
                        Period = c.String(),
                        Reference = c.String(),
                        IssueDate = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IssueID)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionID, cascadeDelete: true)
                .Index(t => t.StoreRequisitionID);
            
            CreateTable(
                "EXCEED.IssueItem",
                c => new
                    {
                        IssueItemID = c.Int(nullable: false, identity: true),
                        IssueID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLAccountCode = c.String(nullable: false),
                        IssueGoodReceiveItemID = c.String(),
                        IssueGoodReceiveItemPrice = c.String(),
                        FixedAssetSelector = c.String(),
                        IsFixedAsset = c.Boolean(nullable: false),
                        FromStoreReturn = c.Boolean(nullable: false),
                        IsMachinery = c.Boolean(nullable: false),
                        BatchNo = c.String(),
                        Model = c.String(),
                        SerialNo = c.String(),
                        Condition = c.String(),
                        PlateNo = c.String(),
                        ChassisNo = c.String(),
                        EngineNo = c.String(),
                        EngineModelNo = c.String(),
                        EngineElectricalNo = c.String(),
                        EngineElectricalModelNo = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IssueItemID)
                .ForeignKey("EXCEED.Issue", t => t.IssueID, cascadeDelete: true)
                .Index(t => t.IssueID);
            
            CreateTable(
                "EXCEED.IssueValidation",
                c => new
                    {
                        IssueValidationID = c.Int(nullable: false, identity: true),
                        IssueID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IssueValidationID)
                .ForeignKey("EXCEED.Issue", t => t.IssueID, cascadeDelete: true)
                .Index(t => t.IssueID);
            
            CreateTable(
                "EXCEED.IssueFuelDistribution",
                c => new
                    {
                        IssueFuelDistributionID = c.Int(nullable: false, identity: true),
                        IssueFuelID = c.Int(nullable: false),
                        IssueItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IssueFuelDistributionID)
                .ForeignKey("EXCEED.IssueFuel", t => t.IssueFuelID, cascadeDelete: true)
                .Index(t => t.IssueFuelID);
            
            CreateTable(
                "EXCEED.IssueFuel",
                c => new
                    {
                        IssueFuelID = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        StoreCode = c.String(nullable: false),
                        RequestingDepartment = c.String(),
                        Project = c.String(),
                        ReadingKM = c.String(),
                        FiscalYear = c.String(),
                        Period = c.String(),
                        Reference = c.String(),
                        IssueDate = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IssueFuelID)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionID, cascadeDelete: true)
                .Index(t => t.StoreRequisitionID);
            
            CreateTable(
                "EXCEED.IssueFuelItem",
                c => new
                    {
                        IssueFuelItemID = c.Int(nullable: false, identity: true),
                        IssueFuelID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLAccountCode = c.String(nullable: false),
                        IssueGoodReceiveItemID = c.String(),
                        IssueGoodReceiveItemPrice = c.String(),
                        EmployeeOfFuelCompany = c.String(),
                        EmployeeWhoGotItRefuelled = c.String(),
                        FixedAssetSelector = c.String(),
                        Name = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IssueFuelItemID)
                .ForeignKey("EXCEED.IssueFuel", t => t.IssueFuelID, cascadeDelete: true)
                .Index(t => t.IssueFuelID);
            
            CreateTable(
                "EXCEED.IssueFuelValidation",
                c => new
                    {
                        IssueFuelValidationID = c.Int(nullable: false, identity: true),
                        IssueFuelID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IssueFuelValidationID)
                .ForeignKey("EXCEED.IssueFuel", t => t.IssueFuelID, cascadeDelete: true)
                .Index(t => t.IssueFuelID);
            
            CreateTable(
                "EXCEED.ItemPerStoreBalance",
                c => new
                    {
                        ItemPerStoreBalanceID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        Type = c.String(),
                        UoMName = c.String(maxLength: 128),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvgPrice = c.DateTime(nullable: false),
                        Store = c.String(),
                    })
                .PrimaryKey(t => t.ItemPerStoreBalanceID)
                .ForeignKey("EXCEED.UoM", t => t.UoMName)
                .Index(t => t.UoMName);
            
            CreateTable(
                "EXCEED.ItemPriceIndex",
                c => new
                    {
                        ItemPriceIndexID = c.Int(nullable: false, identity: true),
                        ItemCategory = c.String(nullable: false),
                        InventoryItem = c.String(nullable: false),
                        PriceBeforeVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SourceOfData = c.String(),
                        Remark = c.String(),
                        DateOfInformation = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ItemPriceIndexID);
            
            CreateTable(
                "EXCEED.ItemTransferDistribution",
                c => new
                    {
                        ItemTransferDistributionID = c.Int(nullable: false, identity: true),
                        ItemTransferID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ItemTransferDistributionID)
                .ForeignKey("EXCEED.ItemTransfer", t => t.ItemTransferID, cascadeDelete: true)
                .Index(t => t.ItemTransferID);
            
            CreateTable(
                "EXCEED.ItemTransfer",
                c => new
                    {
                        ItemTransferID = c.Int(nullable: false, identity: true),
                        RequisitionNo = c.String(),
                        DocumentRef = c.String(),
                        TransferDate = c.DateTime(nullable: false),
                        FromStore = c.String(nullable: false),
                        ToStore = c.String(nullable: false),
                        DeliveredBy = c.String(),
                        VehiclePlate = c.String(),
                        DriverTye = c.String(),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ItemTransferID);
            
            CreateTable(
                "EXCEED.ItemTransferItem",
                c => new
                    {
                        ItemTransferItemID = c.Int(nullable: false, identity: true),
                        ItemTransferID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        PartOrCodeNumber = c.String(nullable: false),
                        RequestedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShippedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ItemTransferItemID)
                .ForeignKey("EXCEED.ItemTransfer", t => t.ItemTransferID, cascadeDelete: true)
                .Index(t => t.ItemTransferID);
            
            CreateTable(
                "EXCEED.ItemTransferValidation",
                c => new
                    {
                        ItemTransferValidationID = c.Int(nullable: false, identity: true),
                        ItemTransferID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ItemTransferValidationID)
                .ForeignKey("EXCEED.ItemTransfer", t => t.ItemTransferID, cascadeDelete: true)
                .Index(t => t.ItemTransferID);
            
            CreateTable(
                "HRM.JobTitleCode",
                c => new
                    {
                        JobTitleCodeId = c.Int(nullable: false, identity: true),
                        TitleCode = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.JobTitleCodeId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.JobTitleMapping",
                c => new
                    {
                        JobTitleMappingId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        MainFunction = c.String(),
                        Quntity = c.Int(nullable: false),
                        PlanType = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.JobTitleMappingId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "common.LanguageProficiency",
                c => new
                    {
                        LanguageProficiencyId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        Reading = c.String(),
                        Listening = c.String(),
                        Writing = c.String(),
                        Speaking = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LanguageProficiencyId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Language", t => t.LanguageId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "common.Language",
                c => new
                    {
                        LanguageId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        LanguageType = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LanguageId)
                .ForeignKey("Common.Country", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
            CreateTable(
                "HRM.LateComming",
                c => new
                    {
                        LateCommingId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Time = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LateCommingId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EXCEED.LCManagement",
                c => new
                    {
                        LCManagementID = c.Int(nullable: false, identity: true),
                        RequestDate = c.DateTime(nullable: false),
                        RequestAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RequestCurrency = c.String(),
                        ApprovedDate = c.DateTime(),
                        ApprovedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedCurrency = c.String(),
                        SwiftCode = c.String(),
                        LCNo = c.String(),
                        LCOpeningDate = c.DateTime(),
                        LCBeneficiary = c.String(),
                        OpeningChargeAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OpeningChargeCurrency = c.String(),
                        LCPeriod = c.String(),
                        LCAmendementAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LCAmendementCurrency = c.String(),
                        LCAdditionalPeriodRequested = c.String(),
                        CustomTaxCharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LogisticsCharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GoodDeliveryDate = c.DateTime(),
                        ContractRefNo = c.String(),
                        InsuranceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FreightCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InlandHaulageRevenue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LCManagementID);
            
            CreateTable(
                "HRM.LeavePaid",
                c => new
                    {
                        LeavePaidId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LeavePaidId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "HRM.LeavePlan",
                c => new
                    {
                        LeavePlanId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        LeaveAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LeavePlanId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "HRM.LeaveRequest",
                c => new
                    {
                        LeaveRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        LeaveDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                        Reason = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LeaveRequestId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "HRM.Leave",
                c => new
                    {
                        TakenLeaveId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LeaveType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PeriodId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        OrgName = c.String(),
                        Before = c.String(),
                        After = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.TakenLeaveId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "HRM.LeaveEntitle",
                c => new
                    {
                        LeaveEntitleId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LeaveEntitleId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "HRM.LeaveTransfer",
                c => new
                    {
                        LeaveTransferId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        From = c.Int(nullable: false),
                        To = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LeaveTransferId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Archive.LetterSender",
                c => new
                    {
                        LetterSenderId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Receiver = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.LetterSenderId);
            
            CreateTable(
                "Archive.LetterStatus",
                c => new
                    {
                        LetterStatusId = c.Int(nullable: false, identity: true),
                        LetterSenderId = c.Int(nullable: false),
                        Decision = c.String(),
                        Location = c.String(),
                    })
                .PrimaryKey(t => t.LetterStatusId)
                .ForeignKey("Archive.LetterSender", t => t.LetterSenderId, cascadeDelete: true)
                .Index(t => t.LetterSenderId);
            
            CreateTable(
                "Payroll.Loan",
                c => new
                    {
                        LoanId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        LoanAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Months = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        JumpHoliday = c.Boolean(nullable: false),
                        Description = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LoanId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "EXCEED.MailModel",
                c => new
                    {
                        MailModelID = c.Int(nullable: false, identity: true),
                        From = c.String(),
                        To = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                    })
                .PrimaryKey(t => t.MailModelID);
            
            CreateTable(
                "Health.MedicalRequest",
                c => new
                    {
                        MedicalRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.MedicalRequestId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.OrgStructureId);
            
            CreateTable(
                "HRM.NeedAssessment",
                c => new
                    {
                        AssesmentId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        NoOfStaff = c.Int(nullable: false),
                        RequiredTraining = c.String(),
                        LookupId = c.Int(nullable: false),
                        Description = c.String(),
                        Type = c.String(),
                        MainCompanyId = c.Int(nullable: false),
                        DevelopmentRequired = c.String(),
                        DevelopmentBy = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AssesmentId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompany", t => t.MainCompanyId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.LookupId)
                .Index(t => t.MainCompanyId);
            
            CreateTable(
                "Archive.IncommingLetter",
                c => new
                    {
                        BookNo = c.String(nullable: false, maxLength: 128),
                        LetterDate = c.DateTime(nullable: false),
                        LetterReferenceNo = c.String(nullable: false),
                        RecivedDate = c.DateTime(nullable: false),
                        OwnerName = c.String(nullable: false),
                        InstitutionTypeId = c.Int(nullable: false),
                        Subject = c.String(nullable: false),
                        Shelve = c.String(nullable: false),
                        Row = c.String(nullable: false),
                        FolderNo = c.String(nullable: false),
                        FileNo = c.String(nullable: false),
                        NoOfAttachment = c.Int(nullable: false),
                        SerialNo = c.String(),
                        RefernceType = c.String(),
                        Remark = c.String(),
                        OutgoingLetterDate = c.DateTime(nullable: false),
                        OutgoingLetterReferenceNo = c.String(),
                        OutgoingOwnerName = c.String(),
                        OutgoingInstitutionTypeId = c.Int(nullable: false),
                        OutgoingSubject = c.String(),
                        OutgoingShelve = c.String(),
                        OutgoingRow = c.String(),
                        OutgoingFolderNo = c.String(),
                        OutgoingFileNo = c.String(),
                        OutgoingNoOfAttachment = c.Int(nullable: false),
                        OutgoingSerialNo = c.String(),
                        OutgoingRefernceType = c.String(),
                        OutgoingRemark = c.String(),
                    })
                .PrimaryKey(t => t.BookNo)
                .ForeignKey("Archive.InstitutionType", t => t.InstitutionTypeId, cascadeDelete: true)
                .Index(t => t.InstitutionTypeId);
            
            CreateTable(
                "Archive.OutgoingLetter",
                c => new
                    {
                        NewOutgoingLetterLetterId = c.Int(nullable: false, identity: true),
                        BookNo = c.String(nullable: false),
                        LetterReferenceNo = c.String(nullable: false),
                        OwnerName = c.String(nullable: false),
                        InstitutionTypeId = c.Int(nullable: false),
                        Subject = c.String(nullable: false),
                        Shelve = c.String(nullable: false),
                        Row = c.String(nullable: false),
                        FolderNo = c.String(nullable: false),
                        FileNo = c.String(nullable: false),
                        NoOfAttachment = c.Int(nullable: false),
                        SerialNo = c.String(),
                        RefernceType = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.NewOutgoingLetterLetterId)
                .ForeignKey("Archive.InstitutionType", t => t.InstitutionTypeId, cascadeDelete: true)
                .Index(t => t.InstitutionTypeId);
            
            CreateTable(
                "EXCEED.OrgBranchUserMapping",
                c => new
                    {
                        OrgBranchUserMappingID = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Branch = c.String(nullable: false),
                        User = c.String(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.OrgBranchUserMappingID);
            
            CreateTable(
                "HRM.OtherBenefit",
                c => new
                    {
                        OtherBenefitId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        BenefitName = c.Int(nullable: false),
                        UnitofMeasurement = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        TimeInterval = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.OtherBenefitId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            CreateTable(
                "HRM.Outgoing",
                c => new
                    {
                        OutgoingId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        FolderNo = c.String(),
                        CardSerialNo = c.String(),
                        Owner = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.OutgoingId);
            
            CreateTable(
                "HRM.OutgoingStatus",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        OutgoingId = c.Int(nullable: false),
                        NoOfAttachment = c.Int(nullable: false),
                        Subject = c.String(),
                        Destination = c.String(),
                    })
                .PrimaryKey(t => t.StatusId)
                .ForeignKey("HRM.Outgoing", t => t.OutgoingId, cascadeDelete: true)
                .Index(t => t.OutgoingId);
            
            CreateTable(
                "EXCEED.OverTimeJobResult",
                c => new
                    {
                        OverTimeJobResultId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        JobResult = c.String(),
                        Cost = c.String(),
                        Benefit = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.OverTimeJobResultId);
            
            CreateTable(
                "Payroll.OverTimePerDay",
                c => new
                    {
                        TicketNumber = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        NormalHours = c.Boolean(nullable: false),
                        NightHours = c.Boolean(nullable: false),
                        HolidayHours = c.Boolean(nullable: false),
                        RestdayHours = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        TotalHr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.TicketNumber)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "HRM.OverTimeRequest",
                c => new
                    {
                        OverTimeRequestId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        FromTime = c.DateTime(nullable: false),
                        ToTime = c.DateTime(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.OverTimeRequestId)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.OrgStructureId);
            
            CreateTable(
                "HRM.OverTimeRequestEmployee",
                c => new
                    {
                        OverTimeRequestEmployeeId = c.Int(nullable: false, identity: true),
                        OverTimeRequestId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.OverTimeRequestEmployeeId)
                .ForeignKey("HRM.OverTimeRequest", t => t.OverTimeRequestId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.OverTimeRequestId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Payroll.OverTime",
                c => new
                    {
                        OverTimeId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        NormalHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NightHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HolidayHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RestdayHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.OverTimeId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "Payroll.OverTimeTransfer",
                c => new
                    {
                        OverTimeTransferId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OverTimeTransferId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Payroll.OverTimeTrasferDetail",
                c => new
                    {
                        OverTimeTrasferDetailId = c.Int(nullable: false, identity: true),
                        OverTimeTransferId = c.Int(nullable: false),
                        NormalHours = c.Boolean(nullable: false),
                        NightHours = c.Boolean(nullable: false),
                        HolidayHours = c.Boolean(nullable: false),
                        RestdayHours = c.Boolean(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.OverTimeTrasferDetailId)
                .ForeignKey("Payroll.OverTimeTransfer", t => t.OverTimeTransferId, cascadeDelete: true)
                .Index(t => t.OverTimeTransferId);
            
            CreateTable(
                "EXCEED.ParticipatingSupplier",
                c => new
                    {
                        ParticipatingSupplierID = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        InventoryItem = c.String(nullable: false),
                        UPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Supplier = c.String(nullable: false),
                        Representative = c.String(),
                        Address = c.String(),
                        Telephone = c.String(),
                        ProformaSubmitionDate = c.DateTime(nullable: false),
                        ProformaSubmitionTime = c.String(),
                        Remark = c.String(),
                        GLTaxRateID = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ParticipatingSupplierID)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .ForeignKey("EXCEED.GLTaxRate", t => t.GLTaxRateID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID)
                .Index(t => t.GLTaxRateID);
            
            CreateTable(
                "EXCEED.ProformaPurchase",
                c => new
                    {
                        ProformaPurchaseID = c.Int(nullable: false, identity: true),
                        ProformaReference = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        FloatDate = c.DateTime(nullable: false),
                        ClosingDate = c.DateTime(nullable: false),
                        EvaluationDate = c.DateTime(nullable: false),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformaPurchaseID)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID);
            
            CreateTable(
                "EXCEED.ProformaEvaluationCriteria",
                c => new
                    {
                        ProformaEvaluationCriteriaID = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        CriteriaName = c.String(),
                        Weight = c.Single(nullable: false),
                        Description = c.String(),
                        Type = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformaEvaluationCriteriaID)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            CreateTable(
                "EXCEED.ProformaEvaluationResult",
                c => new
                    {
                        ProformaEvaluationResultID = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        SupplierName = c.String(nullable: false),
                        CriteriaName = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Score = c.Single(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformaEvaluationResultID)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            CreateTable(
                "EXCEED.ProformaItem",
                c => new
                    {
                        ProformaItemID = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        InventoryItem = c.String(nullable: false),
                        UOM = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformaItemID)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            CreateTable(
                "EXCEED.RequestedSupplier",
                c => new
                    {
                        RequestedSupplierID = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        InventoryItem = c.String(),
                        Supplier = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RequestedSupplierID)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            CreateTable(
                "EXCEED.WinningSupplier",
                c => new
                    {
                        WinningSupplierID = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        InventoryItem = c.String(nullable: false),
                        Quanity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Supplier = c.String(nullable: false),
                        DeliveryDate = c.DateTime(),
                        GRNNo = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WinningSupplierID)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            CreateTable(
                "EXCEED.PurchaseValidation",
                c => new
                    {
                        PurchaseValidationID = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseValidationID)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            CreateTable(
                "Health.PatientCard",
                c => new
                    {
                        PatientCardId = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Age = c.Int(nullable: false),
                        Sex = c.String(),
                    })
                .PrimaryKey(t => t.PatientCardId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Health.PatientCardDetail",
                c => new
                    {
                        PatientCardDetailId = c.Int(nullable: false, identity: true),
                        PatientCardId = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        DiagnosisTratment = c.String(),
                    })
                .PrimaryKey(t => t.PatientCardDetailId)
                .ForeignKey("Health.PatientCard", t => t.PatientCardId, cascadeDelete: true)
                .Index(t => t.PatientCardId);
            
            CreateTable(
                "Health.PatientsLeave",
                c => new
                    {
                        PatientsLeaveId = c.Int(nullable: false, identity: true),
                        PatientCardId = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        TentativeDiagnosis = c.String(),
                        RecommendedRest = c.String(),
                        Days = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TreatedBy = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PatientsLeaveId)
                .ForeignKey("Health.PatientCard", t => t.PatientCardId, cascadeDelete: true)
                .Index(t => t.PatientCardId);
            
            CreateTable(
                "EXCEED.PaymentMethod",
                c => new
                    {
                        PaymentMethodID = c.Int(nullable: false, identity: true),
                        Method = c.String(nullable: false),
                        Branch = c.String(nullable: false),
                        AccountCode = c.String(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PaymentMethodID);
            
            CreateTable(
                "EXCEED.PaymentTerms",
                c => new
                    {
                        PaymentTermsID = c.Int(nullable: false, identity: true),
                        PaymentTermsDays = c.Int(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PaymentTermsID);
            
            CreateTable(
                "Payroll.PayrollAccounts",
                c => new
                    {
                        PayrollAccountsId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        IsPerEmployee = c.Boolean(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Account = c.String(),
                        IsPerCostCenter = c.Boolean(nullable: false),
                        ReferenceType = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PayrollAccountsId);
            
            CreateTable(
                "Payroll.PayrollDistribution",
                c => new
                    {
                        PayrollDistributionId = c.Int(nullable: false, identity: true),
                        PeriodId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        CostCenterCode = c.String(),
                    })
                .PrimaryKey(t => t.PayrollDistributionId)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "Payroll.PayrollEmployeeAccount",
                c => new
                    {
                        PayrollEmployeeAccountId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        CostCenterAccountCode = c.String(nullable: false),
                        LocationAccCode = c.String(),
                        Remark = c.String(),
                        AccountCostCenter_Values = c.String(maxLength: 128),
                        AccountLocation_Values = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PayrollEmployeeAccountId)
                .ForeignKey("EXCEED.GLChartOfAccountCostCenter", t => t.AccountCostCenter_Values)
                .ForeignKey("EXCEED.GLChartOfAccountLocation", t => t.AccountLocation_Values)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.AccountCostCenter_Values)
                .Index(t => t.AccountLocation_Values);
            
            CreateTable(
                "Payroll.PayrollEmployeeCostCenter",
                c => new
                    {
                        PayrollEmployeeCostCenterId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        EmpFullName = c.String(nullable: false),
                        CostCenterAccountCode = c.String(nullable: false),
                        CostCenterAccountDesc = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayrollEmployeeCostCenterId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Payroll.PayrollProcessAdditions",
                c => new
                    {
                        PayrollProcessAdditionsId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        PayrollProcessViewModelId = c.Int(nullable: false),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostCenterAccountCode = c.String(),
                        CostCenterAccountDesc = c.String(),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayrollProcessAdditionsId)
                .ForeignKey("Payroll.PayrollProcessViewModel", t => t.PayrollProcessViewModelId, cascadeDelete: true)
                .Index(t => t.PayrollProcessViewModelId);
            
            CreateTable(
                "Payroll.PayrollProcessViewModel",
                c => new
                    {
                        PayrollProcessViewModelId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        FullName = c.String(),
                        LocationCode = c.String(),
                        PensionNumber = c.String(),
                        PeriodId = c.Int(nullable: false),
                        PeriodName = c.String(),
                        DaysAbsent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AbsentDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxablePositionAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonTaxablePositionAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPositionAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxableTopUpAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonTaxableTopUpAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalTopUpAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxableProfessionalAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonTaxableProfessionalAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalProfessionalAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxableTransportAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonTaxableTransportAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalTransportAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxableHouseAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonTaxableHouseAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalHouseAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxableTelephoneAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonTaxableTelephoneAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalTelephoneAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxableAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonTaxableAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalOvertime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAdditions = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDeductions = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalLongTermDeductions = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalLoan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAdvance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRegularContribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalFixedContribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LabourUnionContribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssocationContribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrossEarning = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrossTaxableEarning = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrossNonTaxableEarning = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrossDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ServiceCharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetEarning = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IncomeTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PensionFromCompany = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PensionFromEmployee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPension = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                        CostCenterAccountCode = c.String(),
                        CostCenterAccountDesc = c.String(),
                        OrgBranchAccountCode = c.String(),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        BankAccount = c.String(),
                    })
                .PrimaryKey(t => t.PayrollProcessViewModelId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "Payroll.PayrollProcessAdvances",
                c => new
                    {
                        PayrollProcessAdvancesId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        AdvanceId = c.Int(nullable: false),
                        PayrollProcessViewModelId = c.Int(nullable: false),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Outstanding = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostCenterAccountCode = c.String(),
                        CostCenterAccountDesc = c.String(),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayrollProcessAdvancesId)
                .ForeignKey("Payroll.PayrollProcessViewModel", t => t.PayrollProcessViewModelId, cascadeDelete: true)
                .Index(t => t.PayrollProcessViewModelId);
            
            CreateTable(
                "Payroll.PayrollProcessAllowances",
                c => new
                    {
                        PayrollProcessAllowancesId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        PayrollProcessViewModelId = c.Int(nullable: false),
                        Name = c.String(),
                        AllowanceType = c.Int(nullable: false),
                        Taxable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonTaxable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostCenterAccountCode = c.String(),
                        CostCenterAccountDesc = c.String(),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayrollProcessAllowancesId)
                .ForeignKey("Payroll.PayrollProcessViewModel", t => t.PayrollProcessViewModelId, cascadeDelete: true)
                .Index(t => t.PayrollProcessViewModelId);
            
            CreateTable(
                "Payroll.PayrollProcessDeductions",
                c => new
                    {
                        PayrollProcessDeductionsId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        PayrollProcessViewModelId = c.Int(nullable: false),
                        Name = c.String(),
                        OutstandingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostCenterAccountCode = c.String(),
                        CostCenterAccountDesc = c.String(),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        Index = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PayrollProcessDeductionsId)
                .ForeignKey("Payroll.PayrollProcessViewModel", t => t.PayrollProcessViewModelId, cascadeDelete: true)
                .Index(t => t.PayrollProcessViewModelId);
            
            CreateTable(
                "Payroll.PayrollProcessFixedContribution",
                c => new
                    {
                        PayrollProcessFixedContributionID = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        FixedContributionId = c.Int(nullable: false),
                        PayrollProcessViewModelId = c.Int(nullable: false),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Outstanding = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostCenterAccountCode = c.String(),
                        CostCenterAccountDesc = c.String(),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayrollProcessFixedContributionID)
                .ForeignKey("Payroll.PayrollProcessViewModel", t => t.PayrollProcessViewModelId, cascadeDelete: true)
                .Index(t => t.PayrollProcessViewModelId);
            
            CreateTable(
                "Payroll.PayrollProcessLoans",
                c => new
                    {
                        PayrollProcessLoansId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LoanId = c.Int(nullable: false),
                        PayrollProcessViewModelId = c.Int(nullable: false),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Outstanding = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostCenterAccountCode = c.String(),
                        CostCenterAccountDesc = c.String(),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayrollProcessLoansId)
                .ForeignKey("Payroll.PayrollProcessViewModel", t => t.PayrollProcessViewModelId, cascadeDelete: true)
                .Index(t => t.PayrollProcessViewModelId);
            
            CreateTable(
                "Payroll.PayrollProcessLongTermDeductions",
                c => new
                    {
                        EmpPayrollLongTermDeductionId = c.Int(nullable: false, identity: true),
                        PayrollProcessLongTermDeductionsId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        PayrollProcessViewModelId = c.Int(nullable: false),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Outstanding = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostCenterAccountCode = c.String(),
                        CostCenterAccountDesc = c.String(),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmpPayrollLongTermDeductionId)
                .ForeignKey("Payroll.PayrollProcessViewModel", t => t.PayrollProcessViewModelId, cascadeDelete: true)
                .Index(t => t.PayrollProcessViewModelId);
            
            CreateTable(
                "Payroll.PayrollProcessRegularContribution",
                c => new
                    {
                        PayrollProcessRegularContributionID = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        RegContributionId = c.Int(nullable: false),
                        PayrollProcessViewModelId = c.Int(nullable: false),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostCenterAccountCode = c.String(),
                        CostCenterAccountDesc = c.String(),
                        OrgBranchName = c.String(),
                        Status = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayrollProcessRegularContributionID)
                .ForeignKey("Payroll.PayrollProcessViewModel", t => t.PayrollProcessViewModelId, cascadeDelete: true)
                .Index(t => t.PayrollProcessViewModelId);
            
            CreateTable(
                "Payroll.PeriodStatus",
                c => new
                    {
                        PeriodStatusId = c.Int(nullable: false, identity: true),
                        PeriodId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PeriodStatusId)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "Archive.PersonalArchiveNo",
                c => new
                    {
                        PersonalArchiveNoId = c.Int(nullable: false, identity: true),
                        Folder = c.String(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        ZoneId = c.Int(nullable: false),
                        Shelve = c.String(nullable: false),
                        Row = c.String(nullable: false),
                        FileNo = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PersonalArchiveNoId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("Archive.Zone", t => t.ZoneId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.ZoneId);
            
            CreateTable(
                "HRM.PlacementHistory",
                c => new
                    {
                        PlacementHistoryId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        JobTitle = c.String(nullable: false),
                        Rank = c.String(nullable: false),
                        Step = c.String(nullable: false),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PlacementHistoryId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.Placement",
                c => new
                    {
                        PlacementId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        AssignmentDate = c.DateTime(nullable: false),
                        LookupId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        JobTitleName = c.String(nullable: false),
                        MappingId = c.Int(nullable: false),
                        RankId = c.Int(nullable: false),
                        StepId = c.Int(nullable: false),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssignmentType = c.Int(nullable: false),
                        ResponsibilityLevel = c.Int(nullable: false),
                        ReportsTo = c.Int(nullable: false),
                        ReportsToMappingId = c.Int(nullable: false),
                        Department = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PlacementId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .ForeignKey("HRM.Rank", t => t.RankId, cascadeDelete: true)
                .ForeignKey("HRM.Step", t => t.StepId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LookupId)
                .Index(t => t.OrgStructureId)
                .Index(t => t.RankId)
                .Index(t => t.StepId)
                .Index(t => t.BranchId);
            
            CreateTable(
                "HRM.Step",
                c => new
                    {
                        StepId = c.Int(nullable: false, identity: true),
                        StepName = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.StepId);
            
            CreateTable(
                "Health.Prescription",
                c => new
                    {
                        PrescriptionId = c.Int(nullable: false, identity: true),
                        PatientCardId = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Rate = c.String(),
                        TreatedBy = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PrescriptionId)
                .ForeignKey("Health.PatientCard", t => t.PatientCardId, cascadeDelete: true)
                .Index(t => t.PatientCardId);
            
            CreateTable(
                "EXCEED.ProcurementPOType",
                c => new
                    {
                        ProcurementPOTypeID = c.Int(nullable: false, identity: true),
                        POType = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProcurementPOTypeID);
            
            CreateTable(
                "EXCEED.ProcurementSupplier",
                c => new
                    {
                        ProcurementSupplierID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        SupplierId = c.String(),
                        TradeLicense = c.String(nullable: false),
                        Address = c.String(),
                        FormerContractPerformance = c.String(),
                        IsCredit = c.Boolean(nullable: false),
                        Account = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AccountCode = c.String(),
                        Remark = c.String(),
                        Rank = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProcurementSupplierID);
            
            CreateTable(
                "HRM.Prohibition",
                c => new
                    {
                        ProhibitionId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        LookupId = c.Int(nullable: false),
                        EvaluationResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ProhibitionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "EXCEED.ProjectBudget",
                c => new
                    {
                        ProjectBudgetId = c.Int(nullable: false, identity: true),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectBudgetId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectInformation",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 128),
                        ProjectInformationId = c.Int(nullable: false),
                        Name = c.String(),
                        Category = c.Int(nullable: false),
                        Location = c.String(),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectScope = c.String(),
                        ProjectManager = c.String(),
                        Client = c.String(),
                        Consultant = c.String(),
                        AwardDate = c.DateTime(nullable: false),
                        ContractPeriod = c.String(),
                        ContractSigningDate = c.DateTime(nullable: false),
                        SiteHandOverDate = c.DateTime(nullable: false),
                        EstimatedStartDate = c.DateTime(nullable: false),
                        EstimatedEndDate = c.DateTime(nullable: false),
                        CompletionDate = c.DateTime(nullable: false),
                        LiabilityPeriod = c.String(),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "EXCEED.ProjectCalendar",
                c => new
                    {
                        ProjectCalendarId = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        FromHour = c.String(),
                        ToHour = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectCalendarId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectConstraintsAndRisk",
                c => new
                    {
                        ProjectConstraintsAndRiskId = c.Int(nullable: false, identity: true),
                        Assumptions = c.String(),
                        Constraints = c.String(),
                        RiskDescription = c.String(),
                        RiskStatus = c.Int(nullable: false),
                        RiskImpact = c.Int(nullable: false),
                        RiskProbablity = c.Int(nullable: false),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectConstraintsAndRiskId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectdDocument",
                c => new
                    {
                        ProjectdDocumentId = c.Int(nullable: false, identity: true),
                        DocumentType = c.String(),
                        File = c.Binary(),
                        Description = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectdDocumentId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectDefineBudget",
                c => new
                    {
                        ProjectDefineBudgetId = c.Int(nullable: false, identity: true),
                        BudgetCode = c.String(nullable: false),
                        TotalBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectDefineBudgetId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectEquipmentUtilization",
                c => new
                    {
                        ProjectEquipmentUtilizationId = c.Int(nullable: false, identity: true),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectEquipmentUtilizationId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectEquipmentFuelconsumption",
                c => new
                    {
                        ProjectEquipmentFuelconsumptionId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentId = c.Int(nullable: false),
                        PreviousDelivered = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDateDelivered = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodDelivered = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPreviousConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCurrentConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalToDateConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDateBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectEquipmentUtilization_ProjectEquipmentUtilizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentFuelconsumptionId)
                .ForeignKey("EXCEED.ProjectEquipmentUtilization", t => t.ProjectEquipmentUtilization_ProjectEquipmentUtilizationId)
                .Index(t => t.ProjectEquipmentUtilization_ProjectEquipmentUtilizationId);
            
            CreateTable(
                "EXCEED.ProjectEquipmentHour",
                c => new
                    {
                        ProjectEquipmentHourId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentId = c.Int(nullable: false),
                        PlannedHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvailableHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizedHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvailableEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizedEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverallEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        ProjectEquipmentUtilization_ProjectEquipmentUtilizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentHourId)
                .ForeignKey("EXCEED.ProjectEquipmentUtilization", t => t.ProjectEquipmentUtilization_ProjectEquipmentUtilizationId)
                .Index(t => t.ProjectEquipmentUtilization_ProjectEquipmentUtilizationId);
            
            CreateTable(
                "EXCEED.ProjectEquipmentMobilization",
                c => new
                    {
                        ProjectEquipmentMobilizationId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentId = c.Int(nullable: false),
                        EquipmentType = c.String(nullable: false),
                        From = c.String(nullable: false),
                        To = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Time = c.String(nullable: false),
                        ProjectEquipmentUtilization_ProjectEquipmentUtilizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentMobilizationId)
                .ForeignKey("EXCEED.ProjectEquipmentUtilization", t => t.ProjectEquipmentUtilization_ProjectEquipmentUtilizationId)
                .Index(t => t.ProjectEquipmentUtilization_ProjectEquipmentUtilizationId);
            
            CreateTable(
                "EXCEED.ProjectEquipmentPerformance",
                c => new
                    {
                        ProjectEquipmentPerformanceId = c.Int(nullable: false, identity: true),
                        ProjectEquipmentId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        EquipmentType = c.String(nullable: false),
                        PreviousPerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodPerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDatePerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectEquipmentUtilization_ProjectEquipmentUtilizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEquipmentPerformanceId)
                .ForeignKey("EXCEED.ProjectEquipmentUtilization", t => t.ProjectEquipmentUtilization_ProjectEquipmentUtilizationId)
                .Index(t => t.ProjectEquipmentUtilization_ProjectEquipmentUtilizationId);
            
            CreateTable(
                "EXCEED.ProjectHumanResourceUtilization",
                c => new
                    {
                        ProjectHumanResourceUtilizationId = c.Int(nullable: false, identity: true),
                        CrewCode = c.String(nullable: false),
                        CrewName = c.String(nullable: false),
                        CrewLeader = c.String(nullable: false),
                        CombinationOfCrew = c.String(nullable: false),
                        TypeOfWork = c.String(nullable: false),
                        CrewOutput = c.String(nullable: false),
                        CrewDirectCost = c.String(nullable: false),
                        CrewIndirectCost = c.String(nullable: false),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectHumanResourceUtilizationId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectMaterialDelivery",
                c => new
                    {
                        ProjectMaterialDeliveryId = c.Int(nullable: false, identity: true),
                        DeliveryDate = c.DateTime(nullable: false),
                        ItemCode = c.String(nullable: false),
                        DeliveredQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectMaterialDeliveryId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectMaterialIssued",
                c => new
                    {
                        ProjectMaterialIssuedId = c.Int(nullable: false, identity: true),
                        IssueDate = c.DateTime(nullable: false),
                        ItemCode = c.String(nullable: false),
                        IssuedQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectMaterialIssuedId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectMaterialRequest",
                c => new
                    {
                        ProjectMaterialRequestId = c.Int(nullable: false, identity: true),
                        RequestDate = c.DateTime(nullable: false),
                        ItemCode = c.String(nullable: false),
                        RequestedQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Decription = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectMaterialRequestId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectWorkBreakDown",
                c => new
                    {
                        ProjectWorkBreakDownId = c.Int(nullable: false, identity: true),
                        TaskCode = c.String(),
                        Description = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectWorkBreakDownId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectBudgetRequest",
                c => new
                    {
                        ProjectBudgetRequestId = c.Int(nullable: false, identity: true),
                        ProjectBudgetId = c.Int(nullable: false),
                        RequestedFor = c.String(nullable: false),
                        Requested = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Approved = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Difference = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectBudgetRequestId)
                .ForeignKey("EXCEED.ProjectBudget", t => t.ProjectBudgetId, cascadeDelete: true)
                .Index(t => t.ProjectBudgetId);
            
            CreateTable(
                "EXCEED.ProjectBudgetTransfer",
                c => new
                    {
                        ProjectBudgetTransferId = c.Int(nullable: false, identity: true),
                        ProjectBudgetId = c.Int(nullable: false),
                        PreviousTransferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPeriodTransferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectBudgetTransferId)
                .ForeignKey("EXCEED.ProjectBudget", t => t.ProjectBudgetId, cascadeDelete: true)
                .Index(t => t.ProjectBudgetId);
            
            CreateTable(
                "EXCEED.ProjectBudgetUtilization",
                c => new
                    {
                        ProjectBudgetUtilizationId = c.Int(nullable: false, identity: true),
                        ProjectBudgetId = c.Int(nullable: false),
                        RequestedFor = c.String(nullable: false),
                        Transferred = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Utilized = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Difference = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectBudgetUtilizationId)
                .ForeignKey("EXCEED.ProjectBudget", t => t.ProjectBudgetId, cascadeDelete: true)
                .Index(t => t.ProjectBudgetId);
            
            CreateTable(
                "EXCEED.ProjectContract",
                c => new
                    {
                        ProjectContractId = c.Int(nullable: false, identity: true),
                        VariationOrder = c.String(),
                        SupplimentaryAggrement = c.String(),
                        ProvisionalSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuspensionOrder = c.String(),
                        ClaimRequest = c.String(),
                        RevisedContractTime = c.DateTime(nullable: false),
                        RevisedContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CertifiedPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectContractId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectEstimationBoq",
                c => new
                    {
                        ProjectEstimationBoqid = c.Int(nullable: false, identity: true),
                        ProjectEstimationNameId = c.Int(nullable: false),
                        TypeOfBoq = c.String(nullable: false),
                        BillNumber = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqid)
                .ForeignKey("EXCEED.ProjectEstimationName", t => t.ProjectEstimationNameId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationNameId);
            
            CreateTable(
                "EXCEED.ProjectEstimationBoqBreakdown",
                c => new
                    {
                        ProjectEstimationBoqBreakdownId = c.Int(nullable: false, identity: true),
                        ProjectEstimationBoqid = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        BillNumber = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectEstimationBoq_ProjectEstimationNameId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEstimationBoqBreakdownId)
                .ForeignKey("EXCEED.ProjectEstimationName", t => t.ProjectEstimationBoq_ProjectEstimationNameId)
                .ForeignKey("EXCEED.ProjectEstimationBoq", t => t.ProjectEstimationBoqid, cascadeDelete: true)
                .Index(t => t.ProjectEstimationBoqid)
                .Index(t => t.ProjectEstimationBoq_ProjectEstimationNameId);
            
            CreateTable(
                "EXCEED.ProjectEstimationName",
                c => new
                    {
                        ProjectEstimationNameId = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false),
                        ProjectLocation = c.String(nullable: false),
                        ClientName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectEstimationNameId);
            
            CreateTable(
                "EXCEED.ProjectEstimationCoefficients",
                c => new
                    {
                        ProjectEstimationCoefficientsId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        ItemName = c.String(nullable: false),
                        Ratio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Kg = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shrinkage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Wastage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationCoefficientsId);
            
            CreateTable(
                "EXCEED.ProjectEstimationDirectLabour",
                c => new
                    {
                        ProjectEstimationDirectLabourId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        LabourName = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationDirectLabourId)
                .ForeignKey("EXCEED.ProjectEstimationWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "EXCEED.ProjectEstimationWorkItem",
                c => new
                    {
                        ProjectEstimationWorkItemId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkCategoryId = c.Int(nullable: false),
                        Category = c.String(nullable: false),
                        ItemName = c.String(nullable: false),
                        DirectLabour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IndirectLabour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentOutput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectEstimationUnitRate_ProjectEstimationUnitRateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectEstimationWorkItemId)
                .ForeignKey("EXCEED.ProjectEstimationUnitRate", t => t.ProjectEstimationUnitRate_ProjectEstimationUnitRateId)
                .ForeignKey("EXCEED.ProjectEstimationWorkCategory", t => t.ProjectEstimationWorkCategoryId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkCategoryId)
                .Index(t => t.ProjectEstimationUnitRate_ProjectEstimationUnitRateId);
            
            CreateTable(
                "EXCEED.ProjectEstimationEquipment",
                c => new
                    {
                        ProjectEstimationEquipmentId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        EquipmentName = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationEquipmentId)
                .ForeignKey("EXCEED.ProjectEstimationWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "EXCEED.ProjectEstimationIndirectLabour",
                c => new
                    {
                        ProjectEstimationIndirectLabourId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        LabourName = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationIndirectLabourId)
                .ForeignKey("EXCEED.ProjectEstimationWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "EXCEED.ProjectEstimationMaterial",
                c => new
                    {
                        ProjectEstimationMaterialId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemNo = c.String(nullable: false),
                        TypeOfMaterial = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationMaterialId)
                .ForeignKey("EXCEED.ProjectEstimationWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "EXCEED.ProjectEstimationUnitRate",
                c => new
                    {
                        ProjectEstimationUnitRateId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        ItemName = c.String(nullable: false),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Overheard = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Profit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationUnitRateId);
            
            CreateTable(
                "EXCEED.ProjectEstimationWorkCategory",
                c => new
                    {
                        ProjectEstimationWorkCategoryId = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectEstimationWorkCategoryId);
            
            CreateTable(
                "EXCEED.ProjectEstimationIndexLabour",
                c => new
                    {
                        ProjectEstimationIndexLabourId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        LabourByTrade = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        IndexFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BirrMonth = c.String(nullable: false),
                        IdealBirrDay = c.String(nullable: false),
                        ActualBirrDay = c.String(nullable: false),
                        IdealBirrDays = c.String(nullable: false),
                        ActualBirrDays = c.String(nullable: false),
                        DesertBirrDays = c.String(nullable: false),
                        NetBirrDays = c.String(nullable: false),
                        NetBirrHour = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "EXCEED.ProjectEstimationMachineryRent",
                c => new
                    {
                        ProjectEstimationMachineryRentId = c.Int(nullable: false, identity: true),
                        FuelPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelSupply = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelDistribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemNo = c.String(nullable: false),
                        TypeEquipment = c.String(nullable: false),
                        Model = c.String(nullable: false),
                        YearManufacture = c.String(nullable: false),
                        PowerOutput = c.String(nullable: false),
                        WithoutFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationMachineryRentId);
            
            CreateTable(
                "EXCEED.ProjectEstimationMobilization",
                c => new
                    {
                        ProjectEstimationMobilizationId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        TypeOfMachinery = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Trip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Distance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Speed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffName = c.String(nullable: false),
                        StaffQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perdiem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffTotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationMobilizationId);
            
            CreateTable(
                "EXCEED.ProjectEstimationRawData",
                c => new
                    {
                        ProjectEstimationRawDataId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationRawDataId);
            
            CreateTable(
                "EXCEED.ProjectEstimationTransportCost",
                c => new
                    {
                        ProjectEstimationTransportCostId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationTransportCostId);
            
            CreateTable(
                "EXCEED.ProjectEstimationUnitFactory",
                c => new
                    {
                        ProjectEstimationUnitFactoryId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false),
                        FactoryName = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationUnitFactoryId);
            
            CreateTable(
                "EXCEED.ProjectPaymenAdvance",
                c => new
                    {
                        ProjectPaymenAdvanceId = c.Int(nullable: false, identity: true),
                        AdvanceTakenBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvanceTakenAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAdvanceTakenBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAdvanceTakenAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OutstandingBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OutstandingAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenAdvanceId)
                .ForeignKey("EXCEED.ProjectPaymentCertificate", t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId)
                .Index(t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId);
            
            CreateTable(
                "EXCEED.ProjectPaymentCertificate",
                c => new
                    {
                        ProjectPaymentCertificateId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectPaymentCertificateId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectPaymenCertificatetLine",
                c => new
                    {
                        ProjectPaymenCertificatetLineId = c.Int(nullable: false, identity: true),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplimentaryContract = c.String(),
                        VariationOrder = c.String(),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.String(),
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenCertificatetLineId)
                .ForeignKey("EXCEED.ProjectPaymentCertificate", t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId)
                .Index(t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId);
            
            CreateTable(
                "EXCEED.ProjectPaymenRetention",
                c => new
                    {
                        ProjectPaymenRetentionId = c.Int(nullable: false, identity: true),
                        RetentionBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RetentionAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetentionBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalRetentionAfterVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectPaymentCertificate_ProjectPaymentCertificateId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectPaymenRetentionId)
                .ForeignKey("EXCEED.ProjectPaymentCertificate", t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId)
                .Index(t => t.ProjectPaymentCertificate_ProjectPaymentCertificateId);
            
            CreateTable(
                "EXCEED.ProjectSubContract",
                c => new
                    {
                        ProjectSubContractId = c.Int(nullable: false, identity: true),
                        WorkType = c.String(),
                        SubContractor = c.String(),
                        ContractSigningDate = c.DateTime(nullable: false),
                        CommencementDate = c.DateTime(nullable: false),
                        ContractPeriod = c.String(),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplimentaryAggrement = c.String(),
                        ProvisionalSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SuspensionOrder = c.String(),
                        ClaimRequest = c.String(),
                        RevisedContractTime = c.DateTime(nullable: false),
                        RevisedContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CertifiedPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectSubContractId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectTakeOff",
                c => new
                    {
                        ProjectTakeOffId = c.Int(nullable: false, identity: true),
                        TakeOffNo = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectTakeOffId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "EXCEED.ProjectTakeOffLine",
                c => new
                    {
                        ProjectTakeOffLineId = c.Int(nullable: false, identity: true),
                        T = c.String(),
                        D = c.String(),
                        S = c.String(),
                        Description = c.String(),
                        ProjectTakeOff_ProjectTakeOffId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectTakeOffLineId)
                .ForeignKey("EXCEED.ProjectTakeOff", t => t.ProjectTakeOff_ProjectTakeOffId)
                .Index(t => t.ProjectTakeOff_ProjectTakeOffId);
            
            CreateTable(
                "EXCEED.ProjectTaskSchedule",
                c => new
                    {
                        ProjectTaskScheduleId = c.Int(nullable: false, identity: true),
                        TaskName = c.String(),
                        Duration = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CompletePercentage = c.String(),
                        Project_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectTaskScheduleId)
                .ForeignKey("EXCEED.ProjectInformation", t => t.Project_Code)
                .Index(t => t.Project_Code);
            
            CreateTable(
                "HRM.PromotionRequest",
                c => new
                    {
                        PromotionRequestId = c.Int(nullable: false, identity: true),
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
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .ForeignKey("HRM.Rank", t => t.RankId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.LookupId)
                .Index(t => t.RankId);
            
            CreateTable(
                "EXCEED.PropertyCountItem",
                c => new
                    {
                        PropertyCountItemId = c.Int(nullable: false, identity: true),
                        PropertyCountId = c.Int(nullable: false),
                        ItemCode = c.String(maxLength: 128),
                        PreviousBalance = c.Int(nullable: false),
                        Count = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                        Updated = c.Boolean(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemCategoryCode = c.String(),
                        ShortageQty = c.Decimal(precision: 18, scale: 2),
                        OverageQty = c.Decimal(precision: 18, scale: 2),
                        ShortagePrice = c.Decimal(precision: 18, scale: 2),
                        OveragePrice = c.Decimal(precision: 18, scale: 2),
                        Amouont = c.Decimal(precision: 18, scale: 2),
                        TotalAmount = c.Decimal(precision: 18, scale: 2),
                        PhysicalAmount = c.Decimal(precision: 18, scale: 2),
                        Uom = c.String(),
                        ItemDescription = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.PropertyCountItemId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.PropertyCount", t => t.PropertyCountId, cascadeDelete: true)
                .Index(t => t.PropertyCountId)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "EXCEED.PropertyCount",
                c => new
                    {
                        PropertyCountId = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PropertyCountId)
                .ForeignKey("EXCEED.InventoryStore", t => t.StoreCode)
                .Index(t => t.StoreCode);
            
            CreateTable(
                "EXCEED.PurchaseFollowUp",
                c => new
                    {
                        PurchaseFollowUpID = c.Int(nullable: false, identity: true),
                        RequestDate = c.DateTime(nullable: false),
                        Division = c.String(),
                        Department = c.String(),
                        Section = c.String(),
                        ItemCategory = c.String(),
                        InventoryItem = c.String(),
                        PRQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PRNo = c.Int(nullable: false),
                        ProcurementOfficer = c.String(),
                        ReceiveDate = c.DateTime(nullable: false),
                        PurchaseDate = c.DateTime(),
                        GRNQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProcurementMethod = c.Int(nullable: false),
                        RemainingQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(),
                        Remark = c.String(),
                        isOnlineApprovedUser = c.Boolean(nullable: false),
                        OnlineApprovedByUser = c.String(),
                        OnlineApprovedTimeUser = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseFollowUpID);
            
            CreateTable(
                "EXCEED.PurchaseReturnDistribution",
                c => new
                    {
                        PurchaseReturnDistributionID = c.Int(nullable: false, identity: true),
                        PurchaseReturnID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PurchaseReturnDistributionID)
                .ForeignKey("EXCEED.PurchaseReturn", t => t.PurchaseReturnID, cascadeDelete: true)
                .Index(t => t.PurchaseReturnID);
            
            CreateTable(
                "EXCEED.PurchaseReturn",
                c => new
                    {
                        PurchaseReturnID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        StoreCode = c.String(nullable: false),
                        Reason = c.String(nullable: false),
                        GoodReceiveId = c.Int(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseReturnID)
                .ForeignKey("EXCEED.GoodReceive", t => t.GoodReceiveId, cascadeDelete: true)
                .Index(t => t.GoodReceiveId);
            
            CreateTable(
                "EXCEED.PurchaseReturnItems",
                c => new
                    {
                        PurchaseReturnItemsID = c.Int(nullable: false, identity: true),
                        PurchaseReturnID = c.Int(nullable: false),
                        InventoryItem = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxType = c.String(),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseReturnItemsID)
                .ForeignKey("EXCEED.PurchaseReturn", t => t.PurchaseReturnID, cascadeDelete: true)
                .Index(t => t.PurchaseReturnID);
            
            CreateTable(
                "EXCEED.PurchaseReturnValidation",
                c => new
                    {
                        PurchaseReturnValidationID = c.Int(nullable: false, identity: true),
                        PurchaseReturnID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseReturnValidationID)
                .ForeignKey("EXCEED.PurchaseReturn", t => t.PurchaseReturnID, cascadeDelete: true)
                .Index(t => t.PurchaseReturnID);
            
            CreateTable(
                "EXCEED.ReconciliationBankAccount",
                c => new
                    {
                        ReconciliationBankAccountId = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false),
                        Reference = c.String(nullable: false),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        Reconciled = c.Boolean(nullable: false),
                        Check = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ReconciliationBankAccountId);
            
            CreateTable(
                "EXCEED.ReconciliationBookAccount",
                c => new
                    {
                        ReconciliationBookAccountId = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(nullable: false),
                        Type = c.String(nullable: false),
                        Reference = c.String(nullable: false),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        Reconciled = c.Boolean(nullable: false),
                        Check = c.Boolean(nullable: false),
                        Source = c.String(),
                        SourceID = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ReconciliationBookAccountId);
            
            CreateTable(
                "EXCEED.ReconciliationSchedule",
                c => new
                    {
                        ReconciliationScheduleId = c.Int(nullable: false, identity: true),
                        BankAccount = c.String(nullable: false),
                        StatementNum = c.String(),
                        Date = c.DateTime(nullable: false),
                        BankEndingBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ReconciliationScheduleId);
            
            CreateTable(
                "HRM.RecruitmentRequest",
                c => new
                    {
                        RecruitmentRequestId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        EmploymentType = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        NoOfVaccantPostion = c.Int(nullable: false),
                        NoOfCurrentStaff = c.Int(nullable: false),
                        NoOfShouldBeStaff = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PlanType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RecruitmentRequestId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "HRM.Recruitment",
                c => new
                    {
                        RecruitmentId = c.Int(nullable: false, identity: true),
                        IsForContrat = c.Boolean(nullable: false),
                        LookupId = c.Int(nullable: false),
                        NoOfVaccantPostion = c.Int(nullable: false),
                        NoOfCurrentStaff = c.Int(nullable: false),
                        NoOfShouldBeStaff = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RecruitmentId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            //CreateTable(
            //    "EXCEED.RevaluationItem",
            //    c => new
            //        {
            //            ReEvaluationItemId = c.Int(nullable: false, identity: true),
            //            ReEvaluationId = c.Int(nullable: false),
            //            ItemCode = c.String(maxLength: 128),
            //            PreviousPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            NewPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            Status = c.Boolean(nullable: false),
            //            ItemCategoryCode = c.String(),
            //        })
            //    .PrimaryKey(t => t.ReEvaluationItemId)
            //    .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
            //    .ForeignKey("EXCEED.Revaluation", t => t.ReEvaluationId, cascadeDelete: true)
            //    .Index(t => t.ReEvaluationId)
            //    .Index(t => t.ItemCode);
            
            //CreateTable(
            //    "EXCEED.Revaluation",
            //    c => new
            //        {
            //            ReEvaluationId = c.Int(nullable: false, identity: true),
            //            StoreCode = c.String(maxLength: 128),
            //            ItemCategoryCode = c.String(maxLength: 128),
            //            Status = c.String(),
            //            Remark = c.String(),
            //            IsVoid = c.Boolean(nullable: false),
            //            VoidBy = c.String(),
            //            VoidTime = c.DateTime(),
            //            IsOnlineApproved = c.Boolean(nullable: false),
            //            OnlineApprovedBy = c.String(),
            //            OnlineApprovedTime = c.DateTime(),
            //            IsOnlineTransferred = c.Boolean(nullable: false),
            //            IsOnlineTransferredBy = c.String(),
            //            OnlineTransferredTime = c.DateTime(),
            //            DateCreated = c.DateTime(),
            //            LastModified = c.DateTime(),
            //            CreatedBy = c.String(),
            //            ModifiedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.ReEvaluationId)
            //    .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
            //    .ForeignKey("EXCEED.InventoryStore", t => t.StoreCode)
            //    .Index(t => t.StoreCode)
            //    .Index(t => t.ItemCategoryCode);
            
            //CreateTable(
            //    "EXCEED.ReEvaluationValidation",
            //    c => new
            //        {
            //            ReEvaluationValidationId = c.Int(nullable: false, identity: true),
            //            Status = c.String(),
            //            Remark = c.String(),
            //            ReEvaluationId = c.Int(nullable: false),
            //            OrgBranchName = c.String(),
            //            DateCreated = c.DateTime(),
            //            LastModified = c.DateTime(),
            //            CreatedBy = c.String(),
            //            ModifiedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.ReEvaluationValidationId)
            //    .ForeignKey("EXCEED.Revaluation", t => t.ReEvaluationId, cascadeDelete: true)
            //    .Index(t => t.ReEvaluationId);
            
            CreateTable(
                "Health.ReferralForm",
                c => new
                    {
                        ReferralFormId = c.Int(nullable: false, identity: true),
                        MedicalRequestId = c.Int(nullable: false),
                        ReferredDate = c.DateTime(nullable: false),
                        ReferredTime = c.DateTime(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ReferralFormId)
                .ForeignKey("Health.MedicalRequest", t => t.MedicalRequestId, cascadeDelete: true)
                .Index(t => t.MedicalRequestId);
            
            CreateTable(
                "Health.ReferralSheet",
                c => new
                    {
                        ReferralSheetId = c.Int(nullable: false, identity: true),
                        PatientCardId = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        ReferredFrom = c.String(),
                        ReferredTo = c.String(),
                        BriefHistory = c.String(),
                        PhiscalFindings = c.String(),
                        LabXrayFindings = c.String(),
                        Diagnosis = c.String(),
                        TreatmentGiven = c.String(),
                        Recommendation = c.String(),
                        RecommendedBy = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ReferralSheetId)
                .ForeignKey("Health.PatientCard", t => t.PatientCardId, cascadeDelete: true)
                .Index(t => t.PatientCardId);
            
            CreateTable(
                "Payroll.RegularContribution",
                c => new
                    {
                        RegContributionId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RegContributionId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PeriodId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "Common.Relation",
                c => new
                    {
                        RelationId = c.Int(nullable: false, identity: true),
                        Refereing = c.String(nullable: false),
                        Reference = c.String(nullable: false),
                        Remark = c.String(),
                        RelationType_LookupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RelationId)
                .ForeignKey("common.Lookup", t => t.RelationType_LookupId, cascadeDelete: true)
                .Index(t => t.RelationType_LookupId);
            
            CreateTable(
                "EXCEED.ReportTemplate",
                c => new
                    {
                        ReportTemplateId = c.Int(nullable: false, identity: true),
                        Temp1 = c.String(),
                        Temp2 = c.String(),
                        Temp3 = c.String(),
                        Temp4 = c.String(),
                        Temp5 = c.String(),
                        Type = c.String(),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ReportTemplateId);
            
            CreateTable(
                "HRM.RequestesStatus",
                c => new
                    {
                        RequestStatusId = c.Int(nullable: false, identity: true),
                        RequestId = c.Int(nullable: false),
                        ReferenceType = c.String(),
                        Status = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ChangedBy = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RequestStatusId);
            
            CreateTable(
                "Insurance.Restitution",
                c => new
                    {
                        RestitutionId = c.Int(nullable: false, identity: true),
                        AccidentRecordId = c.Int(nullable: false),
                        RestitutionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RestitutionId)
                .ForeignKey("Insurance.AccidentRecord", t => t.AccidentRecordId, cascadeDelete: true)
                .Index(t => t.AccidentRecordId);
            
            CreateTable(
                "HRM.Salary",
                c => new
                    {
                        SalaryStructureId = c.Int(nullable: false, identity: true),
                        RankId = c.Int(nullable: false),
                        StepId = c.Int(nullable: false),
                        SalaryAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.SalaryStructureId)
                .ForeignKey("HRM.Rank", t => t.RankId, cascadeDelete: true)
                .ForeignKey("HRM.Step", t => t.StepId, cascadeDelete: true)
                .Index(t => t.RankId)
                .Index(t => t.StepId);
            
            CreateTable(
                "HRM.SalaryJobTitle",
                c => new
                    {
                        SalJobTiId = c.Int(nullable: false, identity: true),
                        RankId = c.Int(nullable: false),
                        StepId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        JobTitle = c.String(),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.SalJobTiId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.Rank", t => t.RankId, cascadeDelete: true)
                .ForeignKey("HRM.Step", t => t.StepId, cascadeDelete: true)
                .Index(t => t.RankId)
                .Index(t => t.StepId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "EXCEED.SalesQuoteVoucherView",
                c => new
                    {
                        SalesQuoteVoucherID = c.Int(nullable: false, identity: true),
                        ConsigneeType = c.String(),
                        Consignee = c.String(),
                        IssuedDate = c.DateTime(nullable: false),
                        IsIssued = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        IsVoid = c.Boolean(nullable: false),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period = c.String(),
                        LastObjectState = c.String(),
                        PaymentMethod = c.Int(nullable: false),
                        Remark = c.String(),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalCharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.SalesQuoteVoucherID);
            
            CreateTable(
                "EXCEED.StockAdjustmentDistribution",
                c => new
                    {
                        StockAdjustmentDistributionID = c.Int(nullable: false, identity: true),
                        StockAdjustmentID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.StockAdjustmentDistributionID)
                .ForeignKey("EXCEED.StockAdjustment", t => t.StockAdjustmentID, cascadeDelete: true)
                .Index(t => t.StockAdjustmentID);
            
            CreateTable(
                "EXCEED.StockAdjustment",
                c => new
                    {
                        StockAdjustmentID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        StoreCode = c.String(nullable: false),
                        AdjustmentDate = c.DateTime(nullable: false),
                        Reason = c.String(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StockAdjustmentID);
            
            CreateTable(
                "EXCEED.StockAdjustmentItems",
                c => new
                    {
                        StockAdjustmentItemsID = c.Int(nullable: false, identity: true),
                        StockAdjustmentID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        BinCardBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdjustedPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockAdjustmentType = c.String(),
                        PriceAdjustmentType = c.String(),
                        AdjustedQuantity = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StockAdjustmentItemsID)
                .ForeignKey("EXCEED.StockAdjustment", t => t.StockAdjustmentID, cascadeDelete: true)
                .Index(t => t.StockAdjustmentID);
            
            CreateTable(
                "EXCEED.StockAdjustmentValidation",
                c => new
                    {
                        StockAdjustmentValidationID = c.Int(nullable: false, identity: true),
                        StockAdjustmentID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StockAdjustmentValidationID)
                .ForeignKey("EXCEED.StockAdjustment", t => t.StockAdjustmentID, cascadeDelete: true)
                .Index(t => t.StockAdjustmentID);
            
            CreateTable(
                "EXCEED.StoreReturnDistribution",
                c => new
                    {
                        StoreReturnDistributionID = c.Int(nullable: false, identity: true),
                        StoreReturnID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.StoreReturnDistributionID)
                .ForeignKey("EXCEED.StoreReturn", t => t.StoreReturnID, cascadeDelete: true)
                .Index(t => t.StoreReturnID);
            
            CreateTable(
                "EXCEED.StoreReturn",
                c => new
                    {
                        StoreReturnID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        StoreCode = c.String(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        Reason = c.String(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreReturnID);
            
            CreateTable(
                "EXCEED.StoreReturnItems",
                c => new
                    {
                        StoreReturnItemsID = c.Int(nullable: false, identity: true),
                        StoreReturnID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreReturnItemsID)
                .ForeignKey("EXCEED.StoreReturn", t => t.StoreReturnID, cascadeDelete: true)
                .Index(t => t.StoreReturnID);
            
            CreateTable(
                "EXCEED.StoreReturnValidation",
                c => new
                    {
                        StoreReturnValidationID = c.Int(nullable: false, identity: true),
                        StoreReturnID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreReturnValidationID)
                .ForeignKey("EXCEED.StoreReturn", t => t.StoreReturnID, cascadeDelete: true)
                .Index(t => t.StoreReturnID);
            
            CreateTable(
                "EXCEED.SupplierCategory",
                c => new
                    {
                        SupplierCategoryID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupplierCategoryID);
            
            CreateTable(
                "EXCEED.SupplierRank",
                c => new
                    {
                        SupplierRankId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 45),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupplierRankId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "EXCEED.TagSetting",
                c => new
                    {
                        TagSettingId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ItemCodeTrim = c.Int(nullable: false),
                        IndexCount = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TagSettingId);
            
            CreateTable(
                "EXCEED.TempGoodReceiveItem",
                c => new
                    {
                        TempGoodReceiveItemID = c.Int(nullable: false, identity: true),
                        TempGoodReceiveID = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TempGoodReceiveItemID)
                .ForeignKey("EXCEED.TempGoodReceive", t => t.TempGoodReceiveID, cascadeDelete: true)
                .Index(t => t.TempGoodReceiveID);
            
            CreateTable(
                "EXCEED.TempGoodReceive",
                c => new
                    {
                        TempGoodReceiveID = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(nullable: false),
                        DeliveredBy = c.String(),
                        ReceivedBy = c.String(),
                        ReceiveDate = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TempGoodReceiveID);
            
            CreateTable(
                "EXCEED.TempGoodReceiveValidation",
                c => new
                    {
                        TempGoodReceiveValidationID = c.Int(nullable: false, identity: true),
                        TempGoodReceiveID = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TempGoodReceiveValidationID)
                .ForeignKey("EXCEED.TempGoodReceive", t => t.TempGoodReceiveID, cascadeDelete: true)
                .Index(t => t.TempGoodReceiveID);
            
            CreateTable(
                "EXCEED.TenderBidDocument",
                c => new
                    {
                        TenderBidDocumentId = c.Int(nullable: false, identity: true),
                        TenderID = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.TenderBidDocumentId)
                .ForeignKey("EXCEED.Tender", t => t.TenderID, cascadeDelete: true)
                .Index(t => t.TenderID);
            
            CreateTable(
                "EXCEED.TenderTemplateFiles",
                c => new
                    {
                        TenderTemplateFilesId = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Description = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.TenderTemplateFilesId);
            
            CreateTable(
                "HRM.Termination",
                c => new
                    {
                        TerminationId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Reason = c.String(),
                        ApplicationDate = c.DateTime(nullable: false),
                        TerminationDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.TerminationId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "HRM.Trainee",
                c => new
                    {
                        TraineeId = c.Int(nullable: false, identity: true),
                        CourseTrackingId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        IsCompleteTraining = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.TraineeId)
                .ForeignKey("HRM.CourseTracking", t => t.CourseTrackingId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.CourseTrackingId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.TrainingApplicant",
                c => new
                    {
                        TrainingApplicantId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        NoofTrainee = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.TrainingApplicantId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "HRM.TrainingPlan",
                c => new
                    {
                        TrainingPlanId = c.Int(nullable: false, identity: true),
                        PeriodId = c.Int(nullable: false),
                        WorkUnit = c.Int(nullable: false),
                        Course = c.Int(nullable: false),
                        NoofTrainee = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        MainCompanyId = c.Int(nullable: false),
                        CostPerPerson = c.Double(nullable: false),
                        TotalCost = c.Double(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.TrainingPlanId)
                .ForeignKey("HRM.MainCompany", t => t.MainCompanyId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.PeriodId)
                .Index(t => t.LookupId)
                .Index(t => t.MainCompanyId);
            
            CreateTable(
                "HRM.TrainingStatus",
                c => new
                    {
                        TrainingStatusId = c.Int(nullable: false, identity: true),
                        TraineeId = c.Int(nullable: false),
                        TrainingOutCome = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.TrainingStatusId)
                .ForeignKey("HRM.Trainee", t => t.TraineeId, cascadeDelete: true)
                .Index(t => t.TraineeId);
            
            CreateTable(
                "EXCEED.TrainingType",
                c => new
                    {
                        TrainingTypeID = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TrainingTypeID);
            
            CreateTable(
                "Health.Treatment",
                c => new
                    {
                        TreatmentId = c.Int(nullable: false, identity: true),
                        PrescriptionId = c.Int(nullable: false),
                        Measure = c.String(),
                        Quntity = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TreatmentId)
                .ForeignKey("Health.Prescription", t => t.PrescriptionId, cascadeDelete: true)
                .Index(t => t.PrescriptionId);
            
            CreateTable(
                "EXCEED.StoreItemAssignmentInventoryItem",
                c => new
                    {
                        StoreItemAssignment_StoreItemAssignmentID = c.Int(nullable: false),
                        InventoryItem_ItemCode = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.StoreItemAssignment_StoreItemAssignmentID, t.InventoryItem_ItemCode })
                .ForeignKey("EXCEED.StoreItemAssignment", t => t.StoreItemAssignment_StoreItemAssignmentID, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryItem", t => t.InventoryItem_ItemCode, cascadeDelete: true)
                .Index(t => t.StoreItemAssignment_StoreItemAssignmentID)
                .Index(t => t.InventoryItem_ItemCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Health.Treatment", "PrescriptionId", "Health.Prescription");
            DropForeignKey("HRM.TrainingStatus", "TraineeId", "HRM.Trainee");
            DropForeignKey("HRM.TrainingPlan", "PeriodId", "common.Period");
            DropForeignKey("HRM.TrainingPlan", "LookupId", "common.Lookup");
            DropForeignKey("HRM.TrainingPlan", "MainCompanyId", "HRM.MainCompany");
            DropForeignKey("HRM.TrainingApplicant", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.TrainingApplicant", "LookupId", "common.Lookup");
            DropForeignKey("HRM.Trainee", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Trainee", "CourseTrackingId", "HRM.CourseTracking");
            DropForeignKey("HRM.Termination", "LookupId", "common.Lookup");
            DropForeignKey("HRM.Termination", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.TenderBidDocument", "TenderID", "EXCEED.Tender");
            DropForeignKey("EXCEED.TempGoodReceiveValidation", "TempGoodReceiveID", "EXCEED.TempGoodReceive");
            DropForeignKey("EXCEED.TempGoodReceiveItem", "TempGoodReceiveID", "EXCEED.TempGoodReceive");
            DropForeignKey("EXCEED.StoreReturnValidation", "StoreReturnID", "EXCEED.StoreReturn");
            DropForeignKey("EXCEED.StoreReturnItems", "StoreReturnID", "EXCEED.StoreReturn");
            DropForeignKey("EXCEED.StoreReturnDistribution", "StoreReturnID", "EXCEED.StoreReturn");
            DropForeignKey("EXCEED.StockAdjustmentValidation", "StockAdjustmentID", "EXCEED.StockAdjustment");
            DropForeignKey("EXCEED.StockAdjustmentItems", "StockAdjustmentID", "EXCEED.StockAdjustment");
            DropForeignKey("EXCEED.StockAdjustmentDistribution", "StockAdjustmentID", "EXCEED.StockAdjustment");
            DropForeignKey("HRM.SalaryJobTitle", "StepId", "HRM.Step");
            DropForeignKey("HRM.SalaryJobTitle", "RankId", "HRM.Rank");
            DropForeignKey("HRM.SalaryJobTitle", "LookupId", "common.Lookup");
            DropForeignKey("HRM.Salary", "StepId", "HRM.Step");
            DropForeignKey("HRM.Salary", "RankId", "HRM.Rank");
            DropForeignKey("Insurance.Restitution", "AccidentRecordId", "Insurance.AccidentRecord");
            DropForeignKey("Common.Relation", "RelationType_LookupId", "common.Lookup");
            DropForeignKey("Payroll.RegularContribution", "PeriodId", "common.Period");
            DropForeignKey("Payroll.RegularContribution", "LookupId", "common.Lookup");
            DropForeignKey("Payroll.RegularContribution", "EmployeeId", "HRM.Employee");
            DropForeignKey("Health.ReferralSheet", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.ReferralForm", "MedicalRequestId", "Health.MedicalRequest");
            DropForeignKey("EXCEED.ReEvaluationValidation", "ReEvaluationId", "EXCEED.Revaluation");
            DropForeignKey("EXCEED.Revaluation", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.RevaluationItem", "ReEvaluationId", "EXCEED.Revaluation");
            DropForeignKey("EXCEED.Revaluation", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.RevaluationItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("HRM.Recruitment", "LookupId", "common.Lookup");
            DropForeignKey("HRM.RecruitmentRequest", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.RecruitmentRequest", "LookupId", "common.Lookup");
            DropForeignKey("EXCEED.PurchaseReturnValidation", "PurchaseReturnID", "EXCEED.PurchaseReturn");
            DropForeignKey("EXCEED.PurchaseReturnItems", "PurchaseReturnID", "EXCEED.PurchaseReturn");
            DropForeignKey("EXCEED.PurchaseReturnDistribution", "PurchaseReturnID", "EXCEED.PurchaseReturn");
            DropForeignKey("EXCEED.PurchaseReturn", "GoodReceiveId", "EXCEED.GoodReceive");
            DropForeignKey("EXCEED.PropertyCount", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.PropertyCountItem", "PropertyCountId", "EXCEED.PropertyCount");
            DropForeignKey("EXCEED.PropertyCountItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("HRM.PromotionRequest", "RankId", "HRM.Rank");
            DropForeignKey("HRM.PromotionRequest", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.PromotionRequest", "LookupId", "common.Lookup");
            DropForeignKey("EXCEED.ProjectTaskSchedule", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectTakeOffLine", "ProjectTakeOff_ProjectTakeOffId", "EXCEED.ProjectTakeOff");
            DropForeignKey("EXCEED.ProjectTakeOff", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectSubContract", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectPaymenRetention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "EXCEED.ProjectPaymentCertificate");
            DropForeignKey("EXCEED.ProjectPaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "EXCEED.ProjectPaymentCertificate");
            DropForeignKey("EXCEED.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "EXCEED.ProjectPaymentCertificate");
            DropForeignKey("EXCEED.ProjectPaymentCertificate", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectEstimationWorkItem", "ProjectEstimationWorkCategoryId", "EXCEED.ProjectEstimationWorkCategory");
            DropForeignKey("EXCEED.ProjectEstimationWorkItem", "ProjectEstimationUnitRate_ProjectEstimationUnitRateId", "EXCEED.ProjectEstimationUnitRate");
            DropForeignKey("EXCEED.ProjectEstimationMaterial", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem");
            DropForeignKey("EXCEED.ProjectEstimationIndirectLabour", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem");
            DropForeignKey("EXCEED.ProjectEstimationEquipment", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem");
            DropForeignKey("EXCEED.ProjectEstimationDirectLabour", "ProjectEstimationWorkItemId", "EXCEED.ProjectEstimationWorkItem");
            DropForeignKey("EXCEED.ProjectEstimationBoqBreakdown", "ProjectEstimationBoqid", "EXCEED.ProjectEstimationBoq");
            DropForeignKey("EXCEED.ProjectEstimationBoqBreakdown", "ProjectEstimationBoq_ProjectEstimationNameId", "EXCEED.ProjectEstimationName");
            DropForeignKey("EXCEED.ProjectEstimationBoq", "ProjectEstimationNameId", "EXCEED.ProjectEstimationName");
            DropForeignKey("EXCEED.ProjectContract", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectBudgetUtilization", "ProjectBudgetId", "EXCEED.ProjectBudget");
            DropForeignKey("EXCEED.ProjectBudgetTransfer", "ProjectBudgetId", "EXCEED.ProjectBudget");
            DropForeignKey("EXCEED.ProjectBudgetRequest", "ProjectBudgetId", "EXCEED.ProjectBudget");
            DropForeignKey("EXCEED.ProjectWorkBreakDown", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectMaterialRequest", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectMaterialIssued", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectMaterialDelivery", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectHumanResourceUtilization", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectEquipmentPerformance", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization");
            DropForeignKey("EXCEED.ProjectEquipmentMobilization", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization");
            DropForeignKey("EXCEED.ProjectEquipmentHour", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization");
            DropForeignKey("EXCEED.ProjectEquipmentFuelconsumption", "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId", "EXCEED.ProjectEquipmentUtilization");
            DropForeignKey("EXCEED.ProjectEquipmentUtilization", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectDefineBudget", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectdDocument", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectConstraintsAndRisk", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectCalendar", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("EXCEED.ProjectBudget", "Project_Code", "EXCEED.ProjectInformation");
            DropForeignKey("HRM.Prohibition", "LookupId", "common.Lookup");
            DropForeignKey("HRM.Prohibition", "EmployeeId", "HRM.Employee");
            DropForeignKey("Health.Prescription", "PatientCardId", "Health.PatientCard");
            DropForeignKey("HRM.Placement", "StepId", "HRM.Step");
            DropForeignKey("HRM.Placement", "RankId", "HRM.Rank");
            DropForeignKey("HRM.Placement", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.Placement", "LookupId", "common.Lookup");
            DropForeignKey("HRM.Placement", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Placement", "BranchId", "HRM.Branch");
            DropForeignKey("HRM.PlacementHistory", "EmployeeId", "HRM.Employee");
            DropForeignKey("Archive.PersonalArchiveNo", "ZoneId", "Archive.Zone");
            DropForeignKey("Archive.PersonalArchiveNo", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.PeriodStatus", "PeriodId", "common.Period");
            DropForeignKey("Payroll.PayrollProcessRegularContribution", "PayrollProcessViewModelId", "Payroll.PayrollProcessViewModel");
            DropForeignKey("Payroll.PayrollProcessLongTermDeductions", "PayrollProcessViewModelId", "Payroll.PayrollProcessViewModel");
            DropForeignKey("Payroll.PayrollProcessLoans", "PayrollProcessViewModelId", "Payroll.PayrollProcessViewModel");
            DropForeignKey("Payroll.PayrollProcessFixedContribution", "PayrollProcessViewModelId", "Payroll.PayrollProcessViewModel");
            DropForeignKey("Payroll.PayrollProcessDeductions", "PayrollProcessViewModelId", "Payroll.PayrollProcessViewModel");
            DropForeignKey("Payroll.PayrollProcessAllowances", "PayrollProcessViewModelId", "Payroll.PayrollProcessViewModel");
            DropForeignKey("Payroll.PayrollProcessAdvances", "PayrollProcessViewModelId", "Payroll.PayrollProcessViewModel");
            DropForeignKey("Payroll.PayrollProcessAdditions", "PayrollProcessViewModelId", "Payroll.PayrollProcessViewModel");
            DropForeignKey("Payroll.PayrollProcessViewModel", "PeriodId", "common.Period");
            DropForeignKey("Payroll.PayrollProcessViewModel", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.PayrollEmployeeCostCenter", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.PayrollEmployeeAccount", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.PayrollEmployeeAccount", "AccountLocation_Values", "EXCEED.GLChartOfAccountLocation");
            DropForeignKey("Payroll.PayrollEmployeeAccount", "AccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter");
            DropForeignKey("Payroll.PayrollDistribution", "PeriodId", "common.Period");
            DropForeignKey("Health.PatientsLeave", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.PatientCardDetail", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.PatientCard", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.ParticipatingSupplier", "GLTaxRateID", "EXCEED.GLTaxRate");
            DropForeignKey("EXCEED.PurchaseValidation", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropForeignKey("EXCEED.ProformaPurchase", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropForeignKey("EXCEED.WinningSupplier", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropForeignKey("EXCEED.RequestedSupplier", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropForeignKey("EXCEED.ParticipatingSupplier", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropForeignKey("EXCEED.ProformaItem", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropForeignKey("EXCEED.ProformaEvaluationResult", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropForeignKey("EXCEED.ProformaEvaluationCriteria", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropForeignKey("Payroll.OverTimeTrasferDetail", "OverTimeTransferId", "Payroll.OverTimeTransfer");
            DropForeignKey("Payroll.OverTimeTransfer", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.OverTime", "PeriodId", "common.Period");
            DropForeignKey("Payroll.OverTime", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.OverTimeRequestEmployee", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.OverTimeRequestEmployee", "OverTimeRequestId", "HRM.OverTimeRequest");
            DropForeignKey("HRM.OverTimeRequest", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("Payroll.OverTimePerDay", "PeriodId", "common.Period");
            DropForeignKey("Payroll.OverTimePerDay", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.OutgoingStatus", "OutgoingId", "HRM.Outgoing");
            DropForeignKey("HRM.OtherBenefit", "LookupId", "common.Lookup");
            DropForeignKey("Archive.OutgoingLetter", "InstitutionTypeId", "Archive.InstitutionType");
            DropForeignKey("Archive.IncommingLetter", "InstitutionTypeId", "Archive.InstitutionType");
            DropForeignKey("HRM.NeedAssessment", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.NeedAssessment", "MainCompanyId", "HRM.MainCompany");
            DropForeignKey("HRM.NeedAssessment", "LookupId", "common.Lookup");
            DropForeignKey("Health.MedicalRequest", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("Health.MedicalRequest", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Loan", "PeriodId", "common.Period");
            DropForeignKey("Payroll.Loan", "EmployeeId", "HRM.Employee");
            DropForeignKey("Archive.LetterStatus", "LetterSenderId", "Archive.LetterSender");
            DropForeignKey("HRM.LeaveTransfer", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.LeaveEntitle", "PeriodId", "common.Period");
            DropForeignKey("HRM.LeaveEntitle", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Leave", "PeriodId", "common.Period");
            DropForeignKey("HRM.Leave", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.LeaveRequest", "PeriodId", "common.Period");
            DropForeignKey("HRM.LeaveRequest", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.LeavePlan", "PeriodId", "common.Period");
            DropForeignKey("HRM.LeavePlan", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.LeavePaid", "PeriodId", "common.Period");
            DropForeignKey("HRM.LeavePaid", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.LateComming", "EmployeeId", "HRM.Employee");
            DropForeignKey("common.LanguageProficiency", "LanguageId", "common.Language");
            DropForeignKey("common.Language", "CountryId", "Common.Country");
            DropForeignKey("common.LanguageProficiency", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.JobTitleMapping", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.JobTitleMapping", "LookupId", "common.Lookup");
            DropForeignKey("HRM.JobTitleCode", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.ItemTransferValidation", "ItemTransferID", "EXCEED.ItemTransfer");
            DropForeignKey("EXCEED.ItemTransferItem", "ItemTransferID", "EXCEED.ItemTransfer");
            DropForeignKey("EXCEED.ItemTransferDistribution", "ItemTransferID", "EXCEED.ItemTransfer");
            DropForeignKey("EXCEED.ItemPerStoreBalance", "UoMName", "EXCEED.UoM");
            DropForeignKey("EXCEED.IssueFuel", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropForeignKey("EXCEED.IssueFuelValidation", "IssueFuelID", "EXCEED.IssueFuel");
            DropForeignKey("EXCEED.IssueFuelItem", "IssueFuelID", "EXCEED.IssueFuel");
            DropForeignKey("EXCEED.IssueFuelDistribution", "IssueFuelID", "EXCEED.IssueFuel");
            DropForeignKey("EXCEED.Issue", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropForeignKey("EXCEED.IssueValidation", "IssueID", "EXCEED.Issue");
            DropForeignKey("EXCEED.IssueItem", "IssueID", "EXCEED.Issue");
            DropForeignKey("EXCEED.IssueDistribution", "IssueID", "EXCEED.Issue");
            DropForeignKey("EXCEED.PlanValidation", "InventoryPlanID", "EXCEED.InventoryPlan");
            DropForeignKey("EXCEED.PlanItem", "InventoryPlanID", "EXCEED.InventoryPlan");
            DropForeignKey("EXCEED.InventoryPlan", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Insurance.Insurance", "InsuranceGroupId", "Insurance.InsuranceGroup");
            DropForeignKey("Insurance.InsuranceGroup", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("Insurance.InsuranceGroup", "LookupId", "common.Lookup");
            DropForeignKey("Archive.InstitutionArchiveNo", "ZoneId", "Archive.Zone");
            DropForeignKey("Archive.InstitutionArchiveNo", "InstitutionTypeId", "Archive.InstitutionType");
            DropForeignKey("EXCEED.IndexingCount", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("HRM.IncommingStatus", "IncommingId", "HRM.Incomming");
            DropForeignKey("EXCEED.IfrsStatementOfChangesInEquityRange", "IfrsStatementOfChangesInEquityId", "EXCEED.IfrsStatementOfChangesInEquity");
            DropForeignKey("EXCEED.IfrsSofpEquityAndLiablity", "IfrsStatementOfChangesInEquity_IfrsStatementOfChangesInEquityId", "EXCEED.IfrsStatementOfChangesInEquity");
            DropForeignKey("EXCEED.IfrsSofpEquityAndLiablity", "IfrsStatementOfFinancialPositionId", "EXCEED.IfrsStatementOfFinancialPosition");
            DropForeignKey("EXCEED.IfrsSofpEquityAndLiablityRange", "IfrsSofpEquityAndLiablityId", "EXCEED.IfrsSofpEquityAndLiablity");
            DropForeignKey("EXCEED.IfrsSofpAsset", "IfrsStatementOfFinancialPositionId", "EXCEED.IfrsStatementOfFinancialPosition");
            DropForeignKey("EXCEED.IfrsSofpAssetRange", "IfrsSofpAssetId", "EXCEED.IfrsSofpAsset");
            DropForeignKey("EXCEED.IfrsSocfOperatingActivity", "IfrsStatementOfCashFlowId", "EXCEED.IfrsStatementOfCashFlow");
            DropForeignKey("EXCEED.IfrsSocfOperatingActivityRange", "IfrsSocfOperatingActivityId", "EXCEED.IfrsSocfOperatingActivity");
            DropForeignKey("EXCEED.IfrsSocfInvestingActivity", "IfrsStatementOfCashFlowId", "EXCEED.IfrsStatementOfCashFlow");
            DropForeignKey("EXCEED.IfrsSocfInvestingActivityRange", "IfrsSocfInvestingActivityId", "EXCEED.IfrsSocfInvestingActivity");
            DropForeignKey("EXCEED.IfrsSocfFinancingActivity", "IfrsStatementOfCashFlowId", "EXCEED.IfrsStatementOfCashFlow");
            DropForeignKey("EXCEED.IfrsSocfFinancingActivityRange", "IfrsSocfFinancingActivityId", "EXCEED.IfrsSocfFinancingActivity");
            DropForeignKey("EXCEED.IfrsScoiOtherComprehensiveIncome", "IfrsStatementOfComprehensiveIncomeId", "EXCEED.IfrsStatementOfComprehensiveIncome");
            DropForeignKey("EXCEED.IfrsScoiOtherComprehensiveIncomeRange", "IfrsScoiOtherComprehensiveIncomeId", "EXCEED.IfrsScoiOtherComprehensiveIncome");
            DropForeignKey("EXCEED.IfrsScoiDiscontinuedOperation", "IfrsStatementOfComprehensiveIncomeId", "EXCEED.IfrsStatementOfComprehensiveIncome");
            DropForeignKey("EXCEED.IfrsScoiDiscontinuedOperationRange", "IfrsScoiDiscontinuedOperationId", "EXCEED.IfrsScoiDiscontinuedOperation");
            DropForeignKey("EXCEED.IfrsScoiContinuingOperation", "IfrsStatementOfComprehensiveIncomeId", "EXCEED.IfrsStatementOfComprehensiveIncome");
            DropForeignKey("EXCEED.IfrsScoiContinuingOperationRange", "IfrsScoiContinuingOperationId", "EXCEED.IfrsScoiContinuingOperation");
            DropForeignKey("EXCEED.GoodSellingAndLendingValidation", "GoodSellingAndLendingID", "EXCEED.GoodSellingAndLending");
            DropForeignKey("EXCEED.GoodSellingAndLendingItem", "GoodSellingAndLendingID", "EXCEED.GoodSellingAndLending");
            DropForeignKey("EXCEED.GoodSellingAndLendingDistribution", "GoodSellingAndLendingID", "EXCEED.GoodSellingAndLending");
            DropForeignKey("EXCEED.ItemInspection", "GoodReceiveDonation_GoodReceiveDonationId", "EXCEED.GoodReceiveDonation");
            DropForeignKey("EXCEED.GoodReciveDonationDistribution", "GoodReceiveDonationId", "EXCEED.GoodReceiveDonation");
            DropForeignKey("EXCEED.GoodReceiveDonationItem", "GoodReceiveDonationId", "EXCEED.GoodReceiveDonation");
            DropForeignKey("EXCEED.GoodReciveDonationValidation", "GoodReceiveDonationId", "EXCEED.GoodReceiveDonation");
            DropForeignKey("EXCEED.GoodReceive", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropForeignKey("EXCEED.PurchaseOrderValidation", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropForeignKey("EXCEED.PurchaseOrderItem", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropForeignKey("EXCEED.PurchaseOrderDistribution", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropForeignKey("EXCEED.ItemInspection", "GoodReceiveId", "EXCEED.GoodReceive");
            DropForeignKey("EXCEED.GoodReceiveValidation", "GoodReceiveId", "EXCEED.GoodReceive");
            DropForeignKey("EXCEED.GoodReceiveItem", "GoodReceiveId", "EXCEED.GoodReceive");
            DropForeignKey("EXCEED.GoodReceiveDistribution", "GoodReceiveId", "EXCEED.GoodReceive");
            DropForeignKey("EXCEED.GLRecordJournalValidation", "GlRecordJournalId", "EXCEED.GlRecordJournal");
            DropForeignKey("EXCEED.GLRecordJournalDetail", "GlRecordJournalId", "EXCEED.GlRecordJournal");
            DropForeignKey("EXCEED.GLLedgerPostingLines", "GLLedgerPostingID", "EXCEED.GLLedgerPosting");
            DropForeignKey("EXCEED.GapIncomeStatementRange", "GapIncomeStatementId", "EXCEED.GapIncomeStatement");
            DropForeignKey("EXCEED.GapIncomeStatementByProjectRange", "GapIncomeStatementByProjectId", "EXCEED.GapIncomeStatementByProject");
            DropForeignKey("EXCEED.GapCashFlowRange", "GapCashFlowId", "EXCEED.GapCashFlow");
            DropForeignKey("EXCEED.GapBalanceSheetRange", "GapBalanceSheetId", "EXCEED.GapBalanceSheet");
            DropForeignKey("EXCEED.FleetTyreManagementReturn", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.FleetTyreManagementReturn", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetTyreManagementIssue", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.FleetTyreManagementIssue", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetSalvageStoreIssue", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetSalvageStoreGRN", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetResourcesTransferBetweenSites", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetResourcesTransferBetweenDrivers", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetResourcesReturn", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.FleetResourcesReturn", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetResourcesRequisitions", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetResourcesDistribution", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetMaintenancePlanner", "FleetMaintenanceInspectionID", "EXCEED.FleetMaintenanceInspection");
            DropForeignKey("EXCEED.FleetMaintenanceOutsourced", "FleetSetupRepairTypeID", "EXCEED.FleetSetupRepairType");
            DropForeignKey("EXCEED.FleetMaintenanceOutsourced", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetMaintenanceMechanic", "FleetSetupRepairTypeID", "EXCEED.FleetSetupRepairType");
            DropForeignKey("EXCEED.FleetMaintenanceMechanic", "FleetSetupMaintenanceCenterID", "EXCEED.FleetSetupMaintenanceCenter");
            DropForeignKey("EXCEED.FleetMaintenanceMechanic", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetMaintenanceInspection", "FleetMaintenanceJobOrderID", "EXCEED.FleetMaintenanceJobOrder");
            DropForeignKey("EXCEED.FleetMaintenanceJobOrder", "FleetSetupRepairTypeID", "EXCEED.FleetSetupRepairType");
            DropForeignKey("EXCEED.FleetMaintenanceJobOrder", "FleetSetupMaintenanceCenterID", "EXCEED.FleetSetupMaintenanceCenter");
            DropForeignKey("EXCEED.FleetSetupMaintenanceCenter", "MaintainanceCenterAddress_FleetAddressRegistrationID", "EXCEED.FleetAddressRegistration");
            DropForeignKey("EXCEED.FleetMaintenanceJobOrder", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.FleetMaintenanceJobOrder", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetMaintenanceAccessory", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetMaintenanceAccessory", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.InsuranceAttachment", "FleetInsuranceRegistrationID", "EXCEED.FleetInsuranceRegistration");
            DropForeignKey("EXCEED.FleetInsuranceRegistration", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetFuelManagement", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.FleetFuelManagement", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetEquipmentTimeSheet", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.FleetEquipmentTimeSheet", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetEquipmentReplacement", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetEquipmentRentToExternal", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetEquipmentRentRequisition", "FleetSetupEquipmentNameID", "EXCEED.FleetSetupEquipmentName");
            DropForeignKey("EXCEED.FleetEquipmentRentFromExternal", "FleetSetupEquipmentNameID", "EXCEED.FleetSetupEquipmentName");
            DropForeignKey("EXCEED.FleetEquipmentRentForInternalProjects", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetEquipmentPlanandEvaluation", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetEquipmentInspection", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.FleetEquipmentInspection", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetAccidentRegistration", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.FleetOperatorRegistration", "FleetSetupOperatorPositionID", "EXCEED.FleetSetupOperatorPosition");
            DropForeignKey("EXCEED.OperatorAttachment", "FleetOperatorRegistrationID", "EXCEED.FleetOperatorRegistration");
            DropForeignKey("EXCEED.FleetOperatorRegistration", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetOperatorRegistration", "FleetSetupEducationLevelID", "EXCEED.FleetSetupEducationLevel");
            DropForeignKey("EXCEED.FleetAccidentRegistration", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetEquipmentRegistration", "FleetSetupEquipmentTypeID", "EXCEED.FleetSetupEquipmentType");
            DropForeignKey("EXCEED.FleetEquipmentRegistration", "FleetSetupEquipmentNameID", "EXCEED.FleetSetupEquipmentName");
            DropForeignKey("EXCEED.FleetAttachment", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.FleetEquipmentRegistration", "FleetSetupEquipmentCategoryID", "EXCEED.FleetSetupEquipmentCategory");
            DropForeignKey("Payroll.FixedContribution", "PeriodId", "common.Period");
            DropForeignKey("Payroll.FixedContribution", "LookupId", "common.Lookup");
            DropForeignKey("Payroll.FixedContribution", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.FinanceDashboardRange", "FinanceDashboardId", "EXCEED.FinanceDashboard");
            DropForeignKey("HRM.ExprianceRequest", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Experience", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.ExitInterview", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Training", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.EmpPayrollServiceChargeViewModel", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.EmpPayrollNoDaysWorked", "PeriodId", "common.Period");
            DropForeignKey("Payroll.EmpPayrollNoDaysWorked", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.EmpPayrollLongTermDeduction", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.EmpPayrollDeduction", "EmployeeId", "HRM.Employee");
            DropForeignKey("Performance.EmployeePerformance", "LookupId", "common.Lookup");
            DropForeignKey("Performance.EmployeePerformance", "EmployeeId", "HRM.Employee");
            DropForeignKey("Performance.EmployeePerformanceAction", "PeriodId", "common.Period");
            DropForeignKey("Performance.EmployeePerformanceAction", "ActionId", "Performance.PerformanceAction");
            DropForeignKey("Performance.EmployeePerformanceAction", "EmployeeId", "HRM.Employee");
            DropForeignKey("Performance.EmployeeMeasurementDefinition", "PeriodId", "common.Period");
            DropForeignKey("Performance.EmployeeMeasurementDefinition", "MeasurementDefinitionId", "Performance.MeasurementDefinition");
            DropForeignKey("Performance.EmployeeMeasurementDefinition", "EmployeeId", "HRM.Employee");
            DropForeignKey("Performance.EmployeeMeasurement", "PeriodId", "common.Period");
            DropForeignKey("Performance.EmployeeMeasurement", "MeasurementDefinitionId", "Performance.MeasurementDefinition");
            DropForeignKey("Performance.MeasurementDefinition", "LookupId", "common.Lookup");
            DropForeignKey("Performance.EmployeeMeasurement", "MeasurementId", "Performance.Measurement");
            DropForeignKey("Performance.EmployeeMeasurement", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.EmployeeAssociation", "LookupId", "common.Lookup");
            DropForeignKey("HRM.EmployeeAssociation", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.EmpDisciplineMeasure", "DetailMeasuresId", "HRM.DisciplineMeasuresDetail");
            DropForeignKey("HRM.EmpDisciplineMeasure", "DisciplinaryCaseId", "HRM.DisciplinaryCase");
            DropForeignKey("HRM.EmpDisciplinaryHearing", "DisciplinaryCaseId", "HRM.DisciplinaryCase");
            DropForeignKey("HRM.Emergency", "LookupId", "common.Lookup");
            DropForeignKey("HRM.Emergency", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Education", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.DisciplineRecord", "DetailMeasuresId", "HRM.DisciplineMeasuresDetail");
            DropForeignKey("HRM.DisciplineMeasuresType", "MeasuresId", "HRM.DisciplineMeasures");
            DropForeignKey("HRM.DisciplineMeasuresDetail", "MeasuresId", "HRM.DisciplineMeasures");
            DropForeignKey("HRM.DisciplinaryCase", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Disability", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.CourseTracking", "PeriodId", "common.Period");
            DropForeignKey("HRM.CourseTracking", "MainCompanyId", "HRM.MainCompany");
            DropForeignKey("HRM.CourseTracking", "LookupId", "common.Lookup");
            DropForeignKey("HRM.Cosign", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Cosign", "LookupId", "common.Lookup");
            DropForeignKey("EXCEED.ContractManagementItem", "ContractManagementID", "EXCEED.ContractManagement");
            DropForeignKey("HRM.Contract", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Competency", "LookupId", "common.Lookup");
            DropForeignKey("HRM.Competency", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Committee", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Commitment", "LookupId", "common.Lookup");
            DropForeignKey("HRM.Commitment", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Clearance", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.BudgetPlanMonthly", "BudgetTransfer_BudgetTransferID", "EXCEED.BudgetTransfer");
            DropForeignKey("EXCEED.BudgetPlanMonthly", "BudgetPlan_BudgetPlanID", "EXCEED.BudgetPlan");
            DropForeignKey("EXCEED.TenderValidation", "TenderID", "EXCEED.Tender");
            DropForeignKey("EXCEED.TenderMinuteUploadDBModel", "TenderID", "EXCEED.Tender");
            DropForeignKey("EXCEED.Lot", "TenderID", "EXCEED.Tender");
            DropForeignKey("EXCEED.Lot", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropForeignKey("EXCEED.PurchaseRequisition", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropForeignKey("EXCEED.StoreRequisitionValidation", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropForeignKey("EXCEED.StoreRequisitionPropertyValidation", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropForeignKey("EXCEED.StoreRequisitionItem", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropForeignKey("EXCEED.StoreRequisitionItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.StoreRequisitionDistribution", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropForeignKey("EXCEED.StoreRequisition", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.PurchaseRequisitionUserDepartmentValidation", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropForeignKey("EXCEED.PurchaseRequisitionPurchaseDepartmentValidation", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropForeignKey("EXCEED.PurchaseRequisitionItem", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropForeignKey("EXCEED.PurchaseRequisitionDistribution", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropForeignKey("EXCEED.LotItems", "LotID", "EXCEED.Lot");
            DropForeignKey("EXCEED.LotFinancialOffer", "LotID", "EXCEED.Lot");
            DropForeignKey("EXCEED.TenderEvaluationResult", "LotID", "EXCEED.Lot");
            DropForeignKey("EXCEED.TenderEvaluationCriteria", "LotID", "EXCEED.Lot");
            DropForeignKey("EXCEED.BidPurchasingSuppliers", "LotID", "EXCEED.Lot");
            DropForeignKey("EXCEED.BidPParticipatingSuppliers", "LotID", "EXCEED.Lot");
            DropForeignKey("EXCEED.BidAwardedSupplier", "LotID", "EXCEED.Lot");
            DropForeignKey("Common.BankReference", "BankId", "Common.Bank");
            DropForeignKey("HRM.Attendance", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Association", "PeriodId", "common.Period");
            DropForeignKey("Payroll.Association", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.ArStandardCollectionValidation", "ArStandardCollectionId", "EXCEED.ArStandardCollection");
            DropForeignKey("EXCEED.ArStandardCollectionLines", "ArStandardCollectionId", "EXCEED.ArStandardCollection");
            DropForeignKey("EXCEED.ArStandardCollectionDistribution", "ArStandardCollectionId", "EXCEED.ArStandardCollection");
            DropForeignKey("EXCEED.ARMiscelaneousCollectionValidation", "ARMiscelaneousCollectionID", "EXCEED.ARMiscelaneousCollection");
            DropForeignKey("EXCEED.ARMiscelaneousCollectionDistribution", "ARMiscelaneousCollectionID", "EXCEED.ARMiscelaneousCollection");
            DropForeignKey("EXCEED.ArStandardCollectionViewModel", "ArInvoice_ArInvoiceId", "EXCEED.ArInvoice");
            DropForeignKey("EXCEED.ArInvoiceValidation", "ArInvoiceId", "EXCEED.ArInvoice");
            DropForeignKey("EXCEED.ArInvoiceLines", "ArInvoiceId", "EXCEED.ArInvoice");
            DropForeignKey("EXCEED.ArInvoiceDistribution", "ArInvoiceId", "EXCEED.ArInvoice");
            DropForeignKey("EXCEED.ArInvoiceViewModel", "ArCustomerProfile_ArCustomerProfileId", "EXCEED.ArCustomerProfile");
            DropForeignKey("EXCEED.APsupplierBlacklist", "APSupplierRegistrationID", "EXCEED.APSupplierRegistration");
            DropForeignKey("EXCEED.ApPaybleInvoices", "APSupplierRegistration_APSupplierRegistrationID", "EXCEED.APSupplierRegistration");
            DropForeignKey("EXCEED.Sales", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Revaluation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Location", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Disposal", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.AssetRegistration", "DepretiationRuleId", "EXCEED.DepreciationRule");
            DropForeignKey("EXCEED.DepreciationRule", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.DepreciationRule", "FixedAssetGroupID", "EXCEED.FixedAssetGroup");
            DropForeignKey("EXCEED.AnnualDepreciation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Appreciation", "RegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("HRM.ApplicantTraining", "ApplicantRegistrationId", "HRM.ApplicantRegistration");
            DropForeignKey("HRM.ApplicantionForm", "VaccancyCode", "HRM.Vacancy");
            DropForeignKey("HRM.ApplicantionForm", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.ApplicantExperience", "ApplicantRegistrationId", "HRM.ApplicantRegistration");
            DropForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy");
            DropForeignKey("HRM.Vacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan");
            DropForeignKey("HRM.RecruitmentPlan", "RankId", "HRM.Rank");
            DropForeignKey("HRM.RecruitmentPlan", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.MainCompanyStructure", "StructureId", "HRM.Structure");
            DropForeignKey("HRM.Vacancy", "LookupId", "common.Lookup");
            DropForeignKey("EXCEED.ApPaymentValidation", "ApPaymentId", "EXCEED.ApPayment");
            DropForeignKey("EXCEED.ApPaymentDistribution", "ApPaymentId", "EXCEED.ApPayment");
            DropForeignKey("EXCEED.APPaymentsViewModel", "ApPaybleInvoices_ApPaybleInvoicesId", "EXCEED.ApPaybleInvoices");
            DropForeignKey("EXCEED.ApPaybleInvoiceValidation", "ApPaybleInvoicesId", "EXCEED.ApPaybleInvoices");
            DropForeignKey("EXCEED.ApPaybleInvoicesLines", "ApPaybleInvoicesId", "EXCEED.ApPaybleInvoices");
            DropForeignKey("EXCEED.ApPaybleInvoiceDistribution", "ApPaybleInvoicesId", "EXCEED.ApPaybleInvoices");
            DropForeignKey("EXCEED.ApMiscellaneousPaymentValidation", "ApMiscellaneousPaymentId", "EXCEED.ApMiscellaneousPayment");
            DropForeignKey("EXCEED.ApMiscellaneousPaymentDistribution", "ApMiscellaneousPaymentId", "EXCEED.ApMiscellaneousPayment");
            DropForeignKey("EXCEED.AnnualNeedAssesmentItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.InventoryItem", "UoMName", "EXCEED.UoM");
            DropForeignKey("EXCEED.StoreKeeperAssignment", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.StoreItemAssignment", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.StoreItemAssignmentInventoryItem", "InventoryItem_ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.StoreItemAssignmentInventoryItem", "StoreItemAssignment_StoreItemAssignmentID", "EXCEED.StoreItemAssignment");
            DropForeignKey("EXCEED.BinCardLine", "StoreItemAssignmentID", "EXCEED.StoreItemAssignment");
            DropForeignKey("EXCEED.InventoryItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.AnnualNeedAssesment", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("EXCEED.GLPeriod", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("EXCEED.AnnualNeedAssesmentValidation", "AnnualNeedAssesmentID", "EXCEED.AnnualNeedAssesment");
            DropForeignKey("EXCEED.AnnualNeedAssesmentItem", "AnnualNeedAssesmentID", "EXCEED.AnnualNeedAssesment");
            DropForeignKey("HRM.AllowancePerEmployee", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.AllowancePerEmployee", "AllowanceDefinationId", "HRM.AllowanceDefination");
            DropForeignKey("Payroll.Advance", "PeriodId", "common.Period");
            DropForeignKey("Payroll.Advance", "EmployeeId", "HRM.Employee");
            DropForeignKey("Payroll.Addition", "PeriodId", "common.Period");
            DropForeignKey("Payroll.Addition", "LookupId", "common.Lookup");
            DropForeignKey("Payroll.Addition", "EmployeeId", "HRM.Employee");
            DropForeignKey("Insurance.AccidentMedicalRecord", "AccidentRecordId", "Insurance.AccidentRecord");
            DropForeignKey("HRM.Absence", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.UserCardItems", "UserCard_EmployeeId", "HRM.Employee");
            DropIndex("EXCEED.StoreItemAssignmentInventoryItem", new[] { "InventoryItem_ItemCode" });
            DropIndex("EXCEED.StoreItemAssignmentInventoryItem", new[] { "StoreItemAssignment_StoreItemAssignmentID" });
            DropIndex("Health.Treatment", new[] { "PrescriptionId" });
            DropIndex("HRM.TrainingStatus", new[] { "TraineeId" });
            DropIndex("HRM.TrainingPlan", new[] { "MainCompanyId" });
            DropIndex("HRM.TrainingPlan", new[] { "LookupId" });
            DropIndex("HRM.TrainingPlan", new[] { "PeriodId" });
            DropIndex("HRM.TrainingApplicant", new[] { "LookupId" });
            DropIndex("HRM.TrainingApplicant", new[] { "OrgStructureId" });
            DropIndex("HRM.Trainee", new[] { "EmployeeId" });
            DropIndex("HRM.Trainee", new[] { "CourseTrackingId" });
            DropIndex("HRM.Termination", new[] { "LookupId" });
            DropIndex("HRM.Termination", new[] { "EmployeeId" });
            DropIndex("EXCEED.TenderBidDocument", new[] { "TenderID" });
            DropIndex("EXCEED.TempGoodReceiveValidation", new[] { "TempGoodReceiveID" });
            DropIndex("EXCEED.TempGoodReceiveItem", new[] { "TempGoodReceiveID" });
            DropIndex("EXCEED.SupplierRank", new[] { "Name" });
            DropIndex("EXCEED.StoreReturnValidation", new[] { "StoreReturnID" });
            DropIndex("EXCEED.StoreReturnItems", new[] { "StoreReturnID" });
            DropIndex("EXCEED.StoreReturnDistribution", new[] { "StoreReturnID" });
            DropIndex("EXCEED.StockAdjustmentValidation", new[] { "StockAdjustmentID" });
            DropIndex("EXCEED.StockAdjustmentItems", new[] { "StockAdjustmentID" });
            DropIndex("EXCEED.StockAdjustmentDistribution", new[] { "StockAdjustmentID" });
            DropIndex("HRM.SalaryJobTitle", new[] { "LookupId" });
            DropIndex("HRM.SalaryJobTitle", new[] { "StepId" });
            DropIndex("HRM.SalaryJobTitle", new[] { "RankId" });
            DropIndex("HRM.Salary", new[] { "StepId" });
            DropIndex("HRM.Salary", new[] { "RankId" });
            DropIndex("Insurance.Restitution", new[] { "AccidentRecordId" });
            DropIndex("Common.Relation", new[] { "RelationType_LookupId" });
            DropIndex("Payroll.RegularContribution", new[] { "LookupId" });
            DropIndex("Payroll.RegularContribution", new[] { "PeriodId" });
            DropIndex("Payroll.RegularContribution", new[] { "EmployeeId" });
            DropIndex("Health.ReferralSheet", new[] { "PatientCardId" });
            DropIndex("Health.ReferralForm", new[] { "MedicalRequestId" });
            DropIndex("EXCEED.ReEvaluationValidation", new[] { "ReEvaluationId" });
            DropIndex("EXCEED.Revaluation", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.Revaluation", new[] { "StoreCode" });
            DropIndex("EXCEED.RevaluationItem", new[] { "ItemCode" });
            DropIndex("EXCEED.RevaluationItem", new[] { "ReEvaluationId" });
            DropIndex("HRM.Recruitment", new[] { "LookupId" });
            DropIndex("HRM.RecruitmentRequest", new[] { "LookupId" });
            DropIndex("HRM.RecruitmentRequest", new[] { "OrgStructureId" });
            DropIndex("EXCEED.PurchaseReturnValidation", new[] { "PurchaseReturnID" });
            DropIndex("EXCEED.PurchaseReturnItems", new[] { "PurchaseReturnID" });
            DropIndex("EXCEED.PurchaseReturn", new[] { "GoodReceiveId" });
            DropIndex("EXCEED.PurchaseReturnDistribution", new[] { "PurchaseReturnID" });
            DropIndex("EXCEED.PropertyCount", new[] { "StoreCode" });
            DropIndex("EXCEED.PropertyCountItem", new[] { "ItemCode" });
            DropIndex("EXCEED.PropertyCountItem", new[] { "PropertyCountId" });
            DropIndex("HRM.PromotionRequest", new[] { "RankId" });
            DropIndex("HRM.PromotionRequest", new[] { "LookupId" });
            DropIndex("HRM.PromotionRequest", new[] { "OrgStructureId" });
            DropIndex("EXCEED.ProjectTaskSchedule", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectTakeOffLine", new[] { "ProjectTakeOff_ProjectTakeOffId" });
            DropIndex("EXCEED.ProjectTakeOff", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectSubContract", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectPaymenRetention", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("EXCEED.ProjectPaymenCertificatetLine", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("EXCEED.ProjectPaymentCertificate", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectPaymenAdvance", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("EXCEED.ProjectEstimationMaterial", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("EXCEED.ProjectEstimationIndirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("EXCEED.ProjectEstimationEquipment", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("EXCEED.ProjectEstimationWorkItem", new[] { "ProjectEstimationUnitRate_ProjectEstimationUnitRateId" });
            DropIndex("EXCEED.ProjectEstimationWorkItem", new[] { "ProjectEstimationWorkCategoryId" });
            DropIndex("EXCEED.ProjectEstimationDirectLabour", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("EXCEED.ProjectEstimationBoqBreakdown", new[] { "ProjectEstimationBoq_ProjectEstimationNameId" });
            DropIndex("EXCEED.ProjectEstimationBoqBreakdown", new[] { "ProjectEstimationBoqid" });
            DropIndex("EXCEED.ProjectEstimationBoq", new[] { "ProjectEstimationNameId" });
            DropIndex("EXCEED.ProjectContract", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectBudgetUtilization", new[] { "ProjectBudgetId" });
            DropIndex("EXCEED.ProjectBudgetTransfer", new[] { "ProjectBudgetId" });
            DropIndex("EXCEED.ProjectBudgetRequest", new[] { "ProjectBudgetId" });
            DropIndex("EXCEED.ProjectWorkBreakDown", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectMaterialRequest", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectMaterialIssued", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectMaterialDelivery", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectHumanResourceUtilization", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectEquipmentPerformance", new[] { "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId" });
            DropIndex("EXCEED.ProjectEquipmentMobilization", new[] { "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId" });
            DropIndex("EXCEED.ProjectEquipmentHour", new[] { "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId" });
            DropIndex("EXCEED.ProjectEquipmentFuelconsumption", new[] { "ProjectEquipmentUtilization_ProjectEquipmentUtilizationId" });
            DropIndex("EXCEED.ProjectEquipmentUtilization", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectDefineBudget", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectdDocument", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectConstraintsAndRisk", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectCalendar", new[] { "Project_Code" });
            DropIndex("EXCEED.ProjectBudget", new[] { "Project_Code" });
            DropIndex("HRM.Prohibition", new[] { "LookupId" });
            DropIndex("HRM.Prohibition", new[] { "EmployeeId" });
            DropIndex("Health.Prescription", new[] { "PatientCardId" });
            DropIndex("HRM.Placement", new[] { "BranchId" });
            DropIndex("HRM.Placement", new[] { "StepId" });
            DropIndex("HRM.Placement", new[] { "RankId" });
            DropIndex("HRM.Placement", new[] { "OrgStructureId" });
            DropIndex("HRM.Placement", new[] { "LookupId" });
            DropIndex("HRM.Placement", new[] { "EmployeeId" });
            DropIndex("HRM.PlacementHistory", new[] { "EmployeeId" });
            DropIndex("Archive.PersonalArchiveNo", new[] { "ZoneId" });
            DropIndex("Archive.PersonalArchiveNo", new[] { "EmployeeId" });
            DropIndex("Payroll.PeriodStatus", new[] { "PeriodId" });
            DropIndex("Payroll.PayrollProcessRegularContribution", new[] { "PayrollProcessViewModelId" });
            DropIndex("Payroll.PayrollProcessLongTermDeductions", new[] { "PayrollProcessViewModelId" });
            DropIndex("Payroll.PayrollProcessLoans", new[] { "PayrollProcessViewModelId" });
            DropIndex("Payroll.PayrollProcessFixedContribution", new[] { "PayrollProcessViewModelId" });
            DropIndex("Payroll.PayrollProcessDeductions", new[] { "PayrollProcessViewModelId" });
            DropIndex("Payroll.PayrollProcessAllowances", new[] { "PayrollProcessViewModelId" });
            DropIndex("Payroll.PayrollProcessAdvances", new[] { "PayrollProcessViewModelId" });
            DropIndex("Payroll.PayrollProcessViewModel", new[] { "PeriodId" });
            DropIndex("Payroll.PayrollProcessViewModel", new[] { "EmployeeId" });
            DropIndex("Payroll.PayrollProcessAdditions", new[] { "PayrollProcessViewModelId" });
            DropIndex("Payroll.PayrollEmployeeCostCenter", new[] { "EmployeeId" });
            DropIndex("Payroll.PayrollEmployeeAccount", new[] { "AccountLocation_Values" });
            DropIndex("Payroll.PayrollEmployeeAccount", new[] { "AccountCostCenter_Values" });
            DropIndex("Payroll.PayrollEmployeeAccount", new[] { "EmployeeId" });
            DropIndex("Payroll.PayrollDistribution", new[] { "PeriodId" });
            DropIndex("Health.PatientsLeave", new[] { "PatientCardId" });
            DropIndex("Health.PatientCardDetail", new[] { "PatientCardId" });
            DropIndex("Health.PatientCard", new[] { "EmployeeId" });
            DropIndex("EXCEED.PurchaseValidation", new[] { "ProformaPurchaseID" });
            DropIndex("EXCEED.WinningSupplier", new[] { "ProformaPurchaseID" });
            DropIndex("EXCEED.RequestedSupplier", new[] { "ProformaPurchaseID" });
            DropIndex("EXCEED.ProformaItem", new[] { "ProformaPurchaseID" });
            DropIndex("EXCEED.ProformaEvaluationResult", new[] { "ProformaPurchaseID" });
            DropIndex("EXCEED.ProformaEvaluationCriteria", new[] { "ProformaPurchaseID" });
            DropIndex("EXCEED.ProformaPurchase", new[] { "PurchaseRequisitionID" });
            DropIndex("EXCEED.ParticipatingSupplier", new[] { "GLTaxRateID" });
            DropIndex("EXCEED.ParticipatingSupplier", new[] { "ProformaPurchaseID" });
            DropIndex("Payroll.OverTimeTrasferDetail", new[] { "OverTimeTransferId" });
            DropIndex("Payroll.OverTimeTransfer", new[] { "EmployeeId" });
            DropIndex("Payroll.OverTime", new[] { "PeriodId" });
            DropIndex("Payroll.OverTime", new[] { "EmployeeId" });
            DropIndex("HRM.OverTimeRequestEmployee", new[] { "EmployeeId" });
            DropIndex("HRM.OverTimeRequestEmployee", new[] { "OverTimeRequestId" });
            DropIndex("HRM.OverTimeRequest", new[] { "OrgStructureId" });
            DropIndex("Payroll.OverTimePerDay", new[] { "PeriodId" });
            DropIndex("Payroll.OverTimePerDay", new[] { "EmployeeId" });
            DropIndex("HRM.OutgoingStatus", new[] { "OutgoingId" });
            DropIndex("HRM.OtherBenefit", new[] { "LookupId" });
            DropIndex("Archive.OutgoingLetter", new[] { "InstitutionTypeId" });
            DropIndex("Archive.IncommingLetter", new[] { "InstitutionTypeId" });
            DropIndex("HRM.NeedAssessment", new[] { "MainCompanyId" });
            DropIndex("HRM.NeedAssessment", new[] { "LookupId" });
            DropIndex("HRM.NeedAssessment", new[] { "OrgStructureId" });
            DropIndex("Health.MedicalRequest", new[] { "OrgStructureId" });
            DropIndex("Health.MedicalRequest", new[] { "EmployeeId" });
            DropIndex("Payroll.Loan", new[] { "PeriodId" });
            DropIndex("Payroll.Loan", new[] { "EmployeeId" });
            DropIndex("Archive.LetterStatus", new[] { "LetterSenderId" });
            DropIndex("HRM.LeaveTransfer", new[] { "EmployeeId" });
            DropIndex("HRM.LeaveEntitle", new[] { "PeriodId" });
            DropIndex("HRM.LeaveEntitle", new[] { "EmployeeId" });
            DropIndex("HRM.Leave", new[] { "PeriodId" });
            DropIndex("HRM.Leave", new[] { "EmployeeId" });
            DropIndex("HRM.LeaveRequest", new[] { "PeriodId" });
            DropIndex("HRM.LeaveRequest", new[] { "EmployeeId" });
            DropIndex("HRM.LeavePlan", new[] { "PeriodId" });
            DropIndex("HRM.LeavePlan", new[] { "EmployeeId" });
            DropIndex("HRM.LeavePaid", new[] { "PeriodId" });
            DropIndex("HRM.LeavePaid", new[] { "EmployeeId" });
            DropIndex("HRM.LateComming", new[] { "EmployeeId" });
            DropIndex("common.Language", new[] { "CountryId" });
            DropIndex("common.LanguageProficiency", new[] { "LanguageId" });
            DropIndex("common.LanguageProficiency", new[] { "EmployeeId" });
            DropIndex("HRM.JobTitleMapping", new[] { "LookupId" });
            DropIndex("HRM.JobTitleMapping", new[] { "OrgStructureId" });
            DropIndex("HRM.JobTitleCode", new[] { "EmployeeId" });
            DropIndex("EXCEED.ItemTransferValidation", new[] { "ItemTransferID" });
            DropIndex("EXCEED.ItemTransferItem", new[] { "ItemTransferID" });
            DropIndex("EXCEED.ItemTransferDistribution", new[] { "ItemTransferID" });
            DropIndex("EXCEED.ItemPerStoreBalance", new[] { "UoMName" });
            DropIndex("EXCEED.IssueFuelValidation", new[] { "IssueFuelID" });
            DropIndex("EXCEED.IssueFuelItem", new[] { "IssueFuelID" });
            DropIndex("EXCEED.IssueFuel", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.IssueFuelDistribution", new[] { "IssueFuelID" });
            DropIndex("EXCEED.IssueValidation", new[] { "IssueID" });
            DropIndex("EXCEED.IssueItem", new[] { "IssueID" });
            DropIndex("EXCEED.Issue", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.IssueDistribution", new[] { "IssueID" });
            DropIndex("EXCEED.PlanValidation", new[] { "InventoryPlanID" });
            DropIndex("EXCEED.PlanItem", new[] { "InventoryPlanID" });
            DropIndex("EXCEED.InventoryPlan", new[] { "GLPeriodId" });
            DropIndex("EXCEED.InventoryPeriod", new[] { "Description" });
            DropIndex("Insurance.InsuranceGroup", new[] { "LookupId" });
            DropIndex("Insurance.InsuranceGroup", new[] { "OrgStructureId" });
            DropIndex("Insurance.Insurance", new[] { "InsuranceGroupId" });
            DropIndex("Archive.InstitutionArchiveNo", new[] { "ZoneId" });
            DropIndex("Archive.InstitutionArchiveNo", new[] { "InstitutionTypeId" });
            DropIndex("EXCEED.IndexingCount", new[] { "ItemCode" });
            DropIndex("HRM.IncommingStatus", new[] { "IncommingId" });
            DropIndex("EXCEED.IfrsStatementOfChangesInEquityRange", new[] { "IfrsStatementOfChangesInEquityId" });
            DropIndex("EXCEED.IfrsSofpEquityAndLiablityRange", new[] { "IfrsSofpEquityAndLiablityId" });
            DropIndex("EXCEED.IfrsSofpEquityAndLiablity", new[] { "IfrsStatementOfChangesInEquity_IfrsStatementOfChangesInEquityId" });
            DropIndex("EXCEED.IfrsSofpEquityAndLiablity", new[] { "IfrsStatementOfFinancialPositionId" });
            DropIndex("EXCEED.IfrsSofpAsset", new[] { "IfrsStatementOfFinancialPositionId" });
            DropIndex("EXCEED.IfrsSofpAssetRange", new[] { "IfrsSofpAssetId" });
            DropIndex("EXCEED.IfrsSocfOperatingActivityRange", new[] { "IfrsSocfOperatingActivityId" });
            DropIndex("EXCEED.IfrsSocfOperatingActivity", new[] { "IfrsStatementOfCashFlowId" });
            DropIndex("EXCEED.IfrsSocfInvestingActivityRange", new[] { "IfrsSocfInvestingActivityId" });
            DropIndex("EXCEED.IfrsSocfInvestingActivity", new[] { "IfrsStatementOfCashFlowId" });
            DropIndex("EXCEED.IfrsSocfFinancingActivityRange", new[] { "IfrsSocfFinancingActivityId" });
            DropIndex("EXCEED.IfrsSocfFinancingActivity", new[] { "IfrsStatementOfCashFlowId" });
            DropIndex("EXCEED.IfrsScoiOtherComprehensiveIncomeRange", new[] { "IfrsScoiOtherComprehensiveIncomeId" });
            DropIndex("EXCEED.IfrsScoiOtherComprehensiveIncome", new[] { "IfrsStatementOfComprehensiveIncomeId" });
            DropIndex("EXCEED.IfrsScoiDiscontinuedOperationRange", new[] { "IfrsScoiDiscontinuedOperationId" });
            DropIndex("EXCEED.IfrsScoiDiscontinuedOperation", new[] { "IfrsStatementOfComprehensiveIncomeId" });
            DropIndex("EXCEED.IfrsScoiContinuingOperation", new[] { "IfrsStatementOfComprehensiveIncomeId" });
            DropIndex("EXCEED.IfrsScoiContinuingOperationRange", new[] { "IfrsScoiContinuingOperationId" });
            DropIndex("EXCEED.GoodSellingAndLendingValidation", new[] { "GoodSellingAndLendingID" });
            DropIndex("EXCEED.GoodSellingAndLendingItem", new[] { "GoodSellingAndLendingID" });
            DropIndex("EXCEED.GoodSellingAndLendingDistribution", new[] { "GoodSellingAndLendingID" });
            DropIndex("EXCEED.GoodReciveDonationDistribution", new[] { "GoodReceiveDonationId" });
            DropIndex("EXCEED.GoodReciveDonationValidation", new[] { "GoodReceiveDonationId" });
            DropIndex("EXCEED.GoodReceiveDonationItem", new[] { "GoodReceiveDonationId" });
            DropIndex("EXCEED.PurchaseOrderValidation", new[] { "PurchaseOrderID" });
            DropIndex("EXCEED.PurchaseOrderItem", new[] { "PurchaseOrderID" });
            DropIndex("EXCEED.PurchaseOrderDistribution", new[] { "PurchaseOrderID" });
            DropIndex("EXCEED.ItemInspection", new[] { "GoodReceiveDonation_GoodReceiveDonationId" });
            DropIndex("EXCEED.ItemInspection", new[] { "GoodReceiveId" });
            DropIndex("EXCEED.GoodReceiveValidation", new[] { "GoodReceiveId" });
            DropIndex("EXCEED.GoodReceiveItem", new[] { "GoodReceiveId" });
            DropIndex("EXCEED.GoodReceive", new[] { "PurchaseOrderID" });
            DropIndex("EXCEED.GoodReceiveDistribution", new[] { "GoodReceiveId" });
            DropIndex("EXCEED.GLRecordJournalValidation", new[] { "GlRecordJournalId" });
            DropIndex("EXCEED.GLRecordJournalDetail", new[] { "GlRecordJournalId" });
            DropIndex("EXCEED.GLLedgerPostingLines", new[] { "GLLedgerPostingID" });
            DropIndex("EXCEED.GapIncomeStatementRange", new[] { "GapIncomeStatementId" });
            DropIndex("EXCEED.GapIncomeStatementByProjectRange", new[] { "GapIncomeStatementByProjectId" });
            DropIndex("EXCEED.GapCashFlowRange", new[] { "GapCashFlowId" });
            DropIndex("EXCEED.GapBalanceSheetRange", new[] { "GapBalanceSheetId" });
            DropIndex("EXCEED.FleetTyreManagementReturn", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetTyreManagementReturn", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetTyreManagementIssue", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetTyreManagementIssue", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetSalvageStoreIssue", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetSalvageStoreGRN", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetResourcesTransferBetweenSites", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetResourcesTransferBetweenDrivers", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetResourcesReturn", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetResourcesReturn", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetResourcesRequisitions", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetResourcesDistribution", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetMaintenancePlanner", new[] { "FleetMaintenanceInspectionID" });
            DropIndex("EXCEED.FleetMaintenanceOutsourced", new[] { "FleetSetupRepairTypeID" });
            DropIndex("EXCEED.FleetMaintenanceOutsourced", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetMaintenanceMechanic", new[] { "FleetSetupRepairTypeID" });
            DropIndex("EXCEED.FleetMaintenanceMechanic", new[] { "FleetSetupMaintenanceCenterID" });
            DropIndex("EXCEED.FleetMaintenanceMechanic", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetSetupMaintenanceCenter", new[] { "MaintainanceCenterAddress_FleetAddressRegistrationID" });
            DropIndex("EXCEED.FleetMaintenanceJobOrder", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetMaintenanceJobOrder", new[] { "FleetSetupRepairTypeID" });
            DropIndex("EXCEED.FleetMaintenanceJobOrder", new[] { "FleetSetupMaintenanceCenterID" });
            DropIndex("EXCEED.FleetMaintenanceJobOrder", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetMaintenanceInspection", new[] { "FleetMaintenanceJobOrderID" });
            DropIndex("EXCEED.FleetMaintenanceAccessory", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetMaintenanceAccessory", new[] { "AssetNumber" });
            DropIndex("EXCEED.InsuranceAttachment", new[] { "FleetInsuranceRegistrationID" });
            DropIndex("EXCEED.FleetInsuranceRegistration", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetFuelManagement", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetFuelManagement", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetEquipmentTimeSheet", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetEquipmentTimeSheet", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetEquipmentReplacement", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetEquipmentRentToExternal", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetEquipmentRentRequisition", new[] { "FleetSetupEquipmentNameID" });
            DropIndex("EXCEED.FleetEquipmentRentFromExternal", new[] { "FleetSetupEquipmentNameID" });
            DropIndex("EXCEED.FleetEquipmentRentForInternalProjects", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetEquipmentPlanandEvaluation", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetEquipmentInspection", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetEquipmentInspection", new[] { "AssetNumber" });
            DropIndex("EXCEED.OperatorAttachment", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetOperatorRegistration", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetOperatorRegistration", new[] { "FleetSetupEducationLevelID" });
            DropIndex("EXCEED.FleetOperatorRegistration", new[] { "FleetSetupOperatorPositionID" });
            DropIndex("EXCEED.FleetAttachment", new[] { "AssetNumber" });
            DropIndex("EXCEED.FleetEquipmentRegistration", new[] { "FleetSetupEquipmentNameID" });
            DropIndex("EXCEED.FleetEquipmentRegistration", new[] { "FleetSetupEquipmentTypeID" });
            DropIndex("EXCEED.FleetEquipmentRegistration", new[] { "FleetSetupEquipmentCategoryID" });
            DropIndex("EXCEED.FleetAccidentRegistration", new[] { "FleetOperatorRegistrationID" });
            DropIndex("EXCEED.FleetAccidentRegistration", new[] { "AssetNumber" });
            DropIndex("Payroll.FixedContribution", new[] { "LookupId" });
            DropIndex("Payroll.FixedContribution", new[] { "PeriodId" });
            DropIndex("Payroll.FixedContribution", new[] { "EmployeeId" });
            DropIndex("EXCEED.FinanceDashboardRange", new[] { "FinanceDashboardId" });
            DropIndex("HRM.ExprianceRequest", new[] { "EmployeeId" });
            DropIndex("HRM.Experience", new[] { "EmployeeId" });
            DropIndex("HRM.ExitInterview", new[] { "EmployeeId" });
            DropIndex("HRM.Training", new[] { "EmployeeId" });
            DropIndex("Payroll.EmpPayrollServiceChargeViewModel", new[] { "EmployeeId" });
            DropIndex("Payroll.EmpPayrollNoDaysWorked", new[] { "PeriodId" });
            DropIndex("Payroll.EmpPayrollNoDaysWorked", new[] { "EmployeeId" });
            DropIndex("Payroll.EmpPayrollLongTermDeduction", new[] { "EmployeeId" });
            DropIndex("Payroll.EmpPayrollDeduction", new[] { "EmployeeId" });
            DropIndex("Performance.EmployeePerformance", new[] { "LookupId" });
            DropIndex("Performance.EmployeePerformance", new[] { "EmployeeId" });
            DropIndex("Performance.EmployeePerformanceAction", new[] { "PeriodId" });
            DropIndex("Performance.EmployeePerformanceAction", new[] { "EmployeeId" });
            DropIndex("Performance.EmployeePerformanceAction", new[] { "ActionId" });
            DropIndex("Performance.EmployeeMeasurementDefinition", new[] { "PeriodId" });
            DropIndex("Performance.EmployeeMeasurementDefinition", new[] { "EmployeeId" });
            DropIndex("Performance.EmployeeMeasurementDefinition", new[] { "MeasurementDefinitionId" });
            DropIndex("Performance.MeasurementDefinition", new[] { "LookupId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "PeriodId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "EmployeeId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "MeasurementId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "MeasurementDefinitionId" });
            DropIndex("HRM.EmployeeAssociation", new[] { "LookupId" });
            DropIndex("HRM.EmployeeAssociation", new[] { "EmployeeId" });
            DropIndex("HRM.EmpDisciplineMeasure", new[] { "DetailMeasuresId" });
            DropIndex("HRM.EmpDisciplineMeasure", new[] { "DisciplinaryCaseId" });
            DropIndex("HRM.EmpDisciplinaryHearing", new[] { "DisciplinaryCaseId" });
            DropIndex("HRM.Emergency", new[] { "LookupId" });
            DropIndex("HRM.Emergency", new[] { "EmployeeId" });
            DropIndex("HRM.Education", new[] { "EmployeeId" });
            DropIndex("HRM.DisciplineRecord", new[] { "DetailMeasuresId" });
            DropIndex("HRM.DisciplineMeasuresType", new[] { "MeasuresId" });
            DropIndex("HRM.DisciplineMeasuresDetail", new[] { "MeasuresId" });
            DropIndex("HRM.DisciplinaryCase", new[] { "EmployeeId" });
            DropIndex("HRM.Disability", new[] { "EmployeeId" });
            DropIndex("HRM.CourseTracking", new[] { "MainCompanyId" });
            DropIndex("HRM.CourseTracking", new[] { "LookupId" });
            DropIndex("HRM.CourseTracking", new[] { "PeriodId" });
            DropIndex("HRM.Cosign", new[] { "LookupId" });
            DropIndex("HRM.Cosign", new[] { "EmployeeId" });
            DropIndex("EXCEED.ContractManagementItem", new[] { "ContractManagementID" });
            DropIndex("HRM.Contract", new[] { "EmployeeId" });
            DropIndex("HRM.Competency", new[] { "LookupId" });
            DropIndex("HRM.Competency", new[] { "EmployeeId" });
            DropIndex("HRM.Committee", new[] { "EmployeeId" });
            DropIndex("HRM.Commitment", new[] { "LookupId" });
            DropIndex("HRM.Commitment", new[] { "EmployeeId" });
            DropIndex("HRM.Clearance", new[] { "EmployeeId" });
            DropIndex("EXCEED.BudgetPlanMonthly", new[] { "BudgetTransfer_BudgetTransferID" });
            DropIndex("EXCEED.BudgetPlanMonthly", new[] { "BudgetPlan_BudgetPlanID" });
            DropIndex("EXCEED.TenderValidation", new[] { "TenderID" });
            DropIndex("EXCEED.TenderMinuteUploadDBModel", new[] { "TenderID" });
            DropIndex("EXCEED.StoreRequisitionValidation", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.StoreRequisitionPropertyValidation", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.StoreRequisitionItem", new[] { "ItemCode" });
            DropIndex("EXCEED.StoreRequisitionItem", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.StoreRequisitionDistribution", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.StoreRequisition", new[] { "StoreCode" });
            DropIndex("EXCEED.PurchaseRequisitionUserDepartmentValidation", new[] { "PurchaseRequisitionID" });
            DropIndex("EXCEED.PurchaseRequisitionPurchaseDepartmentValidation", new[] { "PurchaseRequisitionID" });
            DropIndex("EXCEED.PurchaseRequisitionItem", new[] { "PurchaseRequisitionID" });
            DropIndex("EXCEED.PurchaseRequisitionDistribution", new[] { "PurchaseRequisitionID" });
            DropIndex("EXCEED.PurchaseRequisition", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.LotItems", new[] { "LotID" });
            DropIndex("EXCEED.LotFinancialOffer", new[] { "LotID" });
            DropIndex("EXCEED.TenderEvaluationResult", new[] { "LotID" });
            DropIndex("EXCEED.TenderEvaluationCriteria", new[] { "LotID" });
            DropIndex("EXCEED.BidPurchasingSuppliers", new[] { "LotID" });
            DropIndex("EXCEED.BidPParticipatingSuppliers", new[] { "LotID" });
            DropIndex("EXCEED.Lot", new[] { "PurchaseRequisitionID" });
            DropIndex("EXCEED.Lot", new[] { "TenderID" });
            DropIndex("EXCEED.BidAwardedSupplier", new[] { "LotID" });
            DropIndex("Common.BankReference", new[] { "BankId" });
            DropIndex("HRM.Attendance", new[] { "EmployeeId" });
            DropIndex("Payroll.Association", new[] { "PeriodId" });
            DropIndex("Payroll.Association", new[] { "EmployeeId" });
            DropIndex("EXCEED.ArStandardCollectionValidation", new[] { "ArStandardCollectionId" });
            DropIndex("EXCEED.ArStandardCollectionLines", new[] { "ArStandardCollectionId" });
            DropIndex("EXCEED.ArStandardCollectionDistribution", new[] { "ArStandardCollectionId" });
            DropIndex("EXCEED.ARMiscelaneousCollectionValidation", new[] { "ARMiscelaneousCollectionID" });
            DropIndex("EXCEED.ARMiscelaneousCollectionDistribution", new[] { "ARMiscelaneousCollectionID" });
            DropIndex("EXCEED.ArStandardCollectionViewModel", new[] { "ArInvoice_ArInvoiceId" });
            DropIndex("EXCEED.ArInvoiceValidation", new[] { "ArInvoiceId" });
            DropIndex("EXCEED.ArInvoiceLines", new[] { "ArInvoiceId" });
            DropIndex("EXCEED.ArInvoiceDistribution", new[] { "ArInvoiceId" });
            DropIndex("EXCEED.ArInvoiceViewModel", new[] { "ArCustomerProfile_ArCustomerProfileId" });
            DropIndex("EXCEED.APsupplierBlacklist", new[] { "APSupplierRegistrationID" });
            DropIndex("EXCEED.Sales", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Revaluation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Location", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Disposal", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.DepreciationRule", new[] { "FixedAssetGroupID" });
            DropIndex("EXCEED.DepreciationRule", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.AnnualDepreciation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.AssetRegistration", new[] { "DepretiationRuleId" });
            DropIndex("EXCEED.Appreciation", new[] { "RegistrationId" });
            DropIndex("HRM.ApplicantTraining", new[] { "ApplicantRegistrationId" });
            DropIndex("HRM.ApplicantionForm", new[] { "VaccancyCode" });
            DropIndex("HRM.ApplicantionForm", new[] { "EmployeeId" });
            DropIndex("HRM.MainCompanyStructure", new[] { "StructureId" });
            DropIndex("HRM.RecruitmentPlan", new[] { "RankId" });
            DropIndex("HRM.RecruitmentPlan", new[] { "OrgStructureId" });
            DropIndex("HRM.Vacancy", new[] { "LookupId" });
            DropIndex("HRM.Vacancy", new[] { "RecruitmentPlanId" });
            DropIndex("HRM.ApplicantRegistration", new[] { "VaccancyCode" });
            DropIndex("HRM.ApplicantExperience", new[] { "ApplicantRegistrationId" });
            DropIndex("EXCEED.ApPaymentValidation", new[] { "ApPaymentId" });
            DropIndex("EXCEED.ApPaymentDistribution", new[] { "ApPaymentId" });
            DropIndex("EXCEED.APPaymentsViewModel", new[] { "ApPaybleInvoices_ApPaybleInvoicesId" });
            DropIndex("EXCEED.ApPaybleInvoiceValidation", new[] { "ApPaybleInvoicesId" });
            DropIndex("EXCEED.ApPaybleInvoicesLines", new[] { "ApPaybleInvoicesId" });
            DropIndex("EXCEED.ApPaybleInvoices", new[] { "APSupplierRegistration_APSupplierRegistrationID" });
            DropIndex("EXCEED.ApPaybleInvoiceDistribution", new[] { "ApPaybleInvoicesId" });
            DropIndex("EXCEED.ApMiscellaneousPaymentValidation", new[] { "ApMiscellaneousPaymentId" });
            DropIndex("EXCEED.ApMiscellaneousPaymentDistribution", new[] { "ApMiscellaneousPaymentId" });
            DropIndex("EXCEED.StoreKeeperAssignment", new[] { "StoreCode" });
            DropIndex("EXCEED.BinCardLine", new[] { "StoreItemAssignmentID" });
            DropIndex("EXCEED.StoreItemAssignment", new[] { "StoreCode" });
            DropIndex("EXCEED.InventoryItem", new[] { "UoMName" });
            DropIndex("EXCEED.InventoryItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.GLPeriod", new[] { "GlFiscalYearId" });
            DropIndex("EXCEED.AnnualNeedAssesmentValidation", new[] { "AnnualNeedAssesmentID" });
            DropIndex("EXCEED.AnnualNeedAssesment", new[] { "GLPeriodId" });
            DropIndex("EXCEED.AnnualNeedAssesmentItem", new[] { "ItemCode" });
            DropIndex("EXCEED.AnnualNeedAssesmentItem", new[] { "AnnualNeedAssesmentID" });
            DropIndex("HRM.AllowancePerEmployee", new[] { "EmployeeId" });
            DropIndex("HRM.AllowancePerEmployee", new[] { "AllowanceDefinationId" });
            DropIndex("Payroll.Advance", new[] { "PeriodId" });
            DropIndex("Payroll.Advance", new[] { "EmployeeId" });
            DropIndex("Payroll.Addition", new[] { "LookupId" });
            DropIndex("Payroll.Addition", new[] { "PeriodId" });
            DropIndex("Payroll.Addition", new[] { "EmployeeId" });
            DropIndex("Insurance.AccidentMedicalRecord", new[] { "AccidentRecordId" });
            DropIndex("EXCEED.UserCardItems", new[] { "UserCard_EmployeeId" });
            DropIndex("HRM.Absence", new[] { "EmployeeId" });
            DropTable("EXCEED.StoreItemAssignmentInventoryItem");
            DropTable("Health.Treatment");
            DropTable("EXCEED.TrainingType");
            DropTable("HRM.TrainingStatus");
            DropTable("HRM.TrainingPlan");
            DropTable("HRM.TrainingApplicant");
            DropTable("HRM.Trainee");
            DropTable("HRM.Termination");
            DropTable("EXCEED.TenderTemplateFiles");
            DropTable("EXCEED.TenderBidDocument");
            DropTable("EXCEED.TempGoodReceiveValidation");
            DropTable("EXCEED.TempGoodReceive");
            DropTable("EXCEED.TempGoodReceiveItem");
            DropTable("EXCEED.TagSetting");
            DropTable("EXCEED.SupplierRank");
            DropTable("EXCEED.SupplierCategory");
            DropTable("EXCEED.StoreReturnValidation");
            DropTable("EXCEED.StoreReturnItems");
            DropTable("EXCEED.StoreReturn");
            DropTable("EXCEED.StoreReturnDistribution");
            DropTable("EXCEED.StockAdjustmentValidation");
            DropTable("EXCEED.StockAdjustmentItems");
            DropTable("EXCEED.StockAdjustment");
            DropTable("EXCEED.StockAdjustmentDistribution");
            DropTable("EXCEED.SalesQuoteVoucherView");
            DropTable("HRM.SalaryJobTitle");
            DropTable("HRM.Salary");
            DropTable("Insurance.Restitution");
            DropTable("HRM.RequestesStatus");
            DropTable("EXCEED.ReportTemplate");
            DropTable("Common.Relation");
            DropTable("Payroll.RegularContribution");
            DropTable("Health.ReferralSheet");
            DropTable("Health.ReferralForm");
            DropTable("EXCEED.ReEvaluationValidation");
            DropTable("EXCEED.Revaluation");
            DropTable("EXCEED.RevaluationItem");
            DropTable("HRM.Recruitment");
            DropTable("HRM.RecruitmentRequest");
            DropTable("EXCEED.ReconciliationSchedule");
            DropTable("EXCEED.ReconciliationBookAccount");
            DropTable("EXCEED.ReconciliationBankAccount");
            DropTable("EXCEED.PurchaseReturnValidation");
            DropTable("EXCEED.PurchaseReturnItems");
            DropTable("EXCEED.PurchaseReturn");
            DropTable("EXCEED.PurchaseReturnDistribution");
            DropTable("EXCEED.PurchaseFollowUp");
            DropTable("EXCEED.PropertyCount");
            DropTable("EXCEED.PropertyCountItem");
            DropTable("HRM.PromotionRequest");
            DropTable("EXCEED.ProjectTaskSchedule");
            DropTable("EXCEED.ProjectTakeOffLine");
            DropTable("EXCEED.ProjectTakeOff");
            DropTable("EXCEED.ProjectSubContract");
            DropTable("EXCEED.ProjectPaymenRetention");
            DropTable("EXCEED.ProjectPaymenCertificatetLine");
            DropTable("EXCEED.ProjectPaymentCertificate");
            DropTable("EXCEED.ProjectPaymenAdvance");
            DropTable("EXCEED.ProjectEstimationUnitFactory");
            DropTable("EXCEED.ProjectEstimationTransportCost");
            DropTable("EXCEED.ProjectEstimationRawData");
            DropTable("EXCEED.ProjectEstimationMobilization");
            DropTable("EXCEED.ProjectEstimationMachineryRent");
            DropTable("EXCEED.ProjectEstimationIndexLabour");
            DropTable("EXCEED.ProjectEstimationWorkCategory");
            DropTable("EXCEED.ProjectEstimationUnitRate");
            DropTable("EXCEED.ProjectEstimationMaterial");
            DropTable("EXCEED.ProjectEstimationIndirectLabour");
            DropTable("EXCEED.ProjectEstimationEquipment");
            DropTable("EXCEED.ProjectEstimationWorkItem");
            DropTable("EXCEED.ProjectEstimationDirectLabour");
            DropTable("EXCEED.ProjectEstimationCoefficients");
            DropTable("EXCEED.ProjectEstimationName");
            DropTable("EXCEED.ProjectEstimationBoqBreakdown");
            DropTable("EXCEED.ProjectEstimationBoq");
            DropTable("EXCEED.ProjectContract");
            DropTable("EXCEED.ProjectBudgetUtilization");
            DropTable("EXCEED.ProjectBudgetTransfer");
            DropTable("EXCEED.ProjectBudgetRequest");
            DropTable("EXCEED.ProjectWorkBreakDown");
            DropTable("EXCEED.ProjectMaterialRequest");
            DropTable("EXCEED.ProjectMaterialIssued");
            DropTable("EXCEED.ProjectMaterialDelivery");
            DropTable("EXCEED.ProjectHumanResourceUtilization");
            DropTable("EXCEED.ProjectEquipmentPerformance");
            DropTable("EXCEED.ProjectEquipmentMobilization");
            DropTable("EXCEED.ProjectEquipmentHour");
            DropTable("EXCEED.ProjectEquipmentFuelconsumption");
            DropTable("EXCEED.ProjectEquipmentUtilization");
            DropTable("EXCEED.ProjectDefineBudget");
            DropTable("EXCEED.ProjectdDocument");
            DropTable("EXCEED.ProjectConstraintsAndRisk");
            DropTable("EXCEED.ProjectCalendar");
            DropTable("EXCEED.ProjectInformation");
            DropTable("EXCEED.ProjectBudget");
            DropTable("HRM.Prohibition");
            DropTable("EXCEED.ProcurementSupplier");
            DropTable("EXCEED.ProcurementPOType");
            DropTable("Health.Prescription");
            DropTable("HRM.Step");
            DropTable("HRM.Placement");
            DropTable("HRM.PlacementHistory");
            DropTable("Archive.PersonalArchiveNo");
            DropTable("Payroll.PeriodStatus");
            DropTable("Payroll.PayrollProcessRegularContribution");
            DropTable("Payroll.PayrollProcessLongTermDeductions");
            DropTable("Payroll.PayrollProcessLoans");
            DropTable("Payroll.PayrollProcessFixedContribution");
            DropTable("Payroll.PayrollProcessDeductions");
            DropTable("Payroll.PayrollProcessAllowances");
            DropTable("Payroll.PayrollProcessAdvances");
            DropTable("Payroll.PayrollProcessViewModel");
            DropTable("Payroll.PayrollProcessAdditions");
            DropTable("Payroll.PayrollEmployeeCostCenter");
            DropTable("Payroll.PayrollEmployeeAccount");
            DropTable("Payroll.PayrollDistribution");
            DropTable("Payroll.PayrollAccounts");
            DropTable("EXCEED.PaymentTerms");
            DropTable("EXCEED.PaymentMethod");
            DropTable("Health.PatientsLeave");
            DropTable("Health.PatientCardDetail");
            DropTable("Health.PatientCard");
            DropTable("EXCEED.PurchaseValidation");
            DropTable("EXCEED.WinningSupplier");
            DropTable("EXCEED.RequestedSupplier");
            DropTable("EXCEED.ProformaItem");
            DropTable("EXCEED.ProformaEvaluationResult");
            DropTable("EXCEED.ProformaEvaluationCriteria");
            DropTable("EXCEED.ProformaPurchase");
            DropTable("EXCEED.ParticipatingSupplier");
            DropTable("Payroll.OverTimeTrasferDetail");
            DropTable("Payroll.OverTimeTransfer");
            DropTable("Payroll.OverTime");
            DropTable("HRM.OverTimeRequestEmployee");
            DropTable("HRM.OverTimeRequest");
            DropTable("Payroll.OverTimePerDay");
            DropTable("EXCEED.OverTimeJobResult");
            DropTable("HRM.OutgoingStatus");
            DropTable("HRM.Outgoing");
            DropTable("HRM.OtherBenefit");
            DropTable("EXCEED.OrgBranchUserMapping");
            DropTable("Archive.OutgoingLetter");
            DropTable("Archive.IncommingLetter");
            DropTable("HRM.NeedAssessment");
            DropTable("Health.MedicalRequest");
            DropTable("EXCEED.MailModel");
            DropTable("Payroll.Loan");
            DropTable("Archive.LetterStatus");
            DropTable("Archive.LetterSender");
            DropTable("HRM.LeaveTransfer");
            DropTable("HRM.LeaveEntitle");
            DropTable("HRM.Leave");
            DropTable("HRM.LeaveRequest");
            DropTable("HRM.LeavePlan");
            DropTable("HRM.LeavePaid");
            DropTable("EXCEED.LCManagement");
            DropTable("HRM.LateComming");
            DropTable("common.Language");
            DropTable("common.LanguageProficiency");
            DropTable("HRM.JobTitleMapping");
            DropTable("HRM.JobTitleCode");
            DropTable("EXCEED.ItemTransferValidation");
            DropTable("EXCEED.ItemTransferItem");
            DropTable("EXCEED.ItemTransfer");
            DropTable("EXCEED.ItemTransferDistribution");
            DropTable("EXCEED.ItemPriceIndex");
            DropTable("EXCEED.ItemPerStoreBalance");
            DropTable("EXCEED.IssueFuelValidation");
            DropTable("EXCEED.IssueFuelItem");
            DropTable("EXCEED.IssueFuel");
            DropTable("EXCEED.IssueFuelDistribution");
            DropTable("EXCEED.IssueValidation");
            DropTable("EXCEED.IssueItem");
            DropTable("EXCEED.Issue");
            DropTable("EXCEED.IssueDistribution");
            DropTable("EXCEED.PlanValidation");
            DropTable("EXCEED.PlanItem");
            DropTable("EXCEED.InventoryPlan");
            DropTable("EXCEED.InventoryPeriod");
            DropTable("Insurance.InsuranceGroup");
            DropTable("Insurance.Insurance");
            DropTable("Archive.Zone");
            DropTable("Archive.InstitutionType");
            DropTable("Archive.InstitutionArchiveNo");
            DropTable("common.IndustryInvolved");
            DropTable("EXCEED.IndexingCount");
            DropTable("HRM.IncommingStatus");
            DropTable("HRM.Incomming");
            DropTable("EXCEED.IfrsStatementOfChangesInEquityRange");
            DropTable("EXCEED.IfrsStatementOfChangesInEquity");
            DropTable("EXCEED.IfrsSofpEquityAndLiablityRange");
            DropTable("EXCEED.IfrsSofpEquityAndLiablity");
            DropTable("EXCEED.IfrsStatementOfFinancialPosition");
            DropTable("EXCEED.IfrsSofpAsset");
            DropTable("EXCEED.IfrsSofpAssetRange");
            DropTable("EXCEED.IfrsSocfOperatingActivityRange");
            DropTable("EXCEED.IfrsSocfOperatingActivity");
            DropTable("EXCEED.IfrsSocfInvestingActivityRange");
            DropTable("EXCEED.IfrsSocfInvestingActivity");
            DropTable("EXCEED.IfrsStatementOfCashFlow");
            DropTable("EXCEED.IfrsSocfFinancingActivityRange");
            DropTable("EXCEED.IfrsSocfFinancingActivity");
            DropTable("EXCEED.IfrsScoiOtherComprehensiveIncomeRange");
            DropTable("EXCEED.IfrsScoiOtherComprehensiveIncome");
            DropTable("EXCEED.IfrsScoiDiscontinuedOperationRange");
            DropTable("EXCEED.IfrsScoiDiscontinuedOperation");
            DropTable("EXCEED.IfrsStatementOfComprehensiveIncome");
            DropTable("EXCEED.IfrsScoiContinuingOperation");
            DropTable("EXCEED.IfrsScoiContinuingOperationRange");
            DropTable("common.Identification");
            DropTable("EXCEED.GoodSellingAndLendingValidation");
            DropTable("EXCEED.GoodSellingAndLendingItem");
            DropTable("EXCEED.GoodSellingAndLending");
            DropTable("EXCEED.GoodSellingAndLendingDistribution");
            DropTable("EXCEED.GoodReciveDonationDistribution");
            DropTable("EXCEED.GoodReciveDonationValidation");
            DropTable("EXCEED.GoodReceiveDonation");
            DropTable("EXCEED.GoodReceiveDonationItem");
            DropTable("EXCEED.PurchaseOrderValidation");
            DropTable("EXCEED.PurchaseOrderItem");
            DropTable("EXCEED.PurchaseOrderDistribution");
            DropTable("EXCEED.PurchaseOrder");
            DropTable("EXCEED.ItemInspection");
            DropTable("EXCEED.GoodReceiveValidation");
            DropTable("EXCEED.GoodReceiveItem");
            DropTable("EXCEED.GoodReceive");
            DropTable("EXCEED.GoodReceiveDistribution");
            DropTable("EXCEED.GLVatWithholding");
            DropTable("EXCEED.GLTaxRate");
            DropTable("EXCEED.GlSegmentSequence");
            DropTable("EXCEED.GlSegmentLength");
            DropTable("EXCEED.GLRecordJournalValidation");
            DropTable("EXCEED.GlRecordJournal");
            DropTable("EXCEED.GLRecordJournalDetail");
            DropTable("EXCEED.GlobalUserAccessLog");
            DropTable("EXCEED.GlobalOrgInformation");
            DropTable("EXCEED.GlobalBranchsetup");
            DropTable("EXCEED.GLLedgerPosting");
            DropTable("EXCEED.GLLedgerPostingLines");
            DropTable("EXCEED.GLJournalSources");
            DropTable("EXCEED.GLJournalCategoriers");
            DropTable("EXCEED.GLCountryType");
            DropTable("EXCEED.GLChartOfAccountSubAccount");
            DropTable("EXCEED.GLChartOfAccountLocation");
            DropTable("EXCEED.GLChartOfAccountCostCenter");
            DropTable("EXCEED.GLChartOfAccountAccount");
            DropTable("EXCEED.GapIncomeStatement");
            DropTable("EXCEED.GapIncomeStatementRange");
            DropTable("EXCEED.GapIncomeStatementByProject");
            DropTable("EXCEED.GapIncomeStatementByProjectRange");
            DropTable("EXCEED.GapCashFlow");
            DropTable("EXCEED.GapCashFlowRange");
            DropTable("EXCEED.GapBalanceSheet");
            DropTable("EXCEED.GapBalanceSheetRange");
            DropTable("EXCEED.ForeignPurchase");
            DropTable("EXCEED.FleetTyreManagementReturn");
            DropTable("EXCEED.FleetTyreManagementIssue");
            DropTable("EXCEED.FleetSetupRentalRate");
            DropTable("EXCEED.FleetSetupEquipmentManufacturer");
            DropTable("EXCEED.FleetSetupEquipmentFuelStandard");
            DropTable("EXCEED.FleetSetupEmploymentType");
            DropTable("EXCEED.FleetSalvageStoreIssue");
            DropTable("EXCEED.FleetSalvageStoreGRN");
            DropTable("EXCEED.FleetResourcesTransferBetweenSites");
            DropTable("EXCEED.FleetResourcesTransferBetweenDrivers");
            DropTable("EXCEED.FleetResourcesReturn");
            DropTable("EXCEED.FleetResourcesRequisitions");
            DropTable("EXCEED.FleetResourcesDistribution");
            DropTable("EXCEED.FleetMaintenanceTool");
            DropTable("EXCEED.FleetMaintenancePlanner");
            DropTable("EXCEED.FleetMaintenanceOutsourced");
            DropTable("EXCEED.FleetMaintenanceMechanic");
            DropTable("EXCEED.FleetSetupRepairType");
            DropTable("EXCEED.FleetAddressRegistration");
            DropTable("EXCEED.FleetSetupMaintenanceCenter");
            DropTable("EXCEED.FleetMaintenanceJobOrder");
            DropTable("EXCEED.FleetMaintenanceInspection");
            DropTable("EXCEED.FleetMaintenanceForExternal");
            DropTable("EXCEED.FleetMaintenanceCannibalization");
            DropTable("EXCEED.FleetMaintenanceAccessory");
            DropTable("EXCEED.InsuranceAttachment");
            DropTable("EXCEED.FleetInsuranceRegistration");
            DropTable("EXCEED.FleetFuelManagement");
            DropTable("EXCEED.FleetEquipmentTimeSheet");
            DropTable("EXCEED.FleetEquipmentReplacement");
            DropTable("EXCEED.FleetEquipmentRentToExternal");
            DropTable("EXCEED.FleetEquipmentRentRequisition");
            DropTable("EXCEED.FleetEquipmentRentFromExternal");
            DropTable("EXCEED.FleetEquipmentRentForInternalProjects");
            DropTable("EXCEED.FleetEquipmentPlanandEvaluation");
            DropTable("EXCEED.FleetEquipmentInspection");
            DropTable("EXCEED.FleetSetupOperatorPosition");
            DropTable("EXCEED.OperatorAttachment");
            DropTable("EXCEED.FleetSetupEducationLevel");
            DropTable("EXCEED.FleetOperatorRegistration");
            DropTable("EXCEED.FleetSetupEquipmentType");
            DropTable("EXCEED.FleetSetupEquipmentName");
            DropTable("EXCEED.FleetAttachment");
            DropTable("EXCEED.FleetSetupEquipmentCategory");
            DropTable("EXCEED.FleetEquipmentRegistration");
            DropTable("EXCEED.FleetAccidentRegistration");
            DropTable("Payroll.FixedContribution");
            DropTable("EXCEED.FixedAssetSubCategory");
            DropTable("EXCEED.FixedAssetDepreciationSetup");
            DropTable("EXCEED.FixedAssetClearingAccountSetup");
            DropTable("EXCEED.FixedAssetCategory");
            DropTable("EXCEED.FinSetupCurrencyExchange");
            DropTable("EXCEED.FinSetupCurrency");
            DropTable("EXCEED.FinInStatementRecord");
            DropTable("EXCEED.FinanceDashboard");
            DropTable("EXCEED.FinanceDashboardRange");
            DropTable("HRM.ExprianceRequest");
            DropTable("HRM.Experience");
            DropTable("HRM.ExitInterview");
            DropTable("EXCEED.EvaluatedPrice");
            DropTable("HRM.Training");
            DropTable("Payroll.EmpPayrollServiceChargeViewModel");
            DropTable("Payroll.EmpPayrollNoDaysWorked");
            DropTable("Payroll.EmpPayrollLongTermDeduction");
            DropTable("Payroll.EmpPayrollDeduction");
            DropTable("Performance.EmployeePerformance");
            DropTable("Performance.PerformanceAction");
            DropTable("Performance.EmployeePerformanceAction");
            DropTable("Performance.EmployeeMeasurementDefinition");
            DropTable("Performance.MeasurementDefinition");
            DropTable("Performance.Measurement");
            DropTable("Performance.EmployeeMeasurement");
            DropTable("HRM.EmployeeAssociation");
            DropTable("HRM.EmpDisciplineMeasure");
            DropTable("HRM.EmpDisciplinaryHearing");
            DropTable("HRM.Emergency");
            DropTable("HRM.Education");
            DropTable("HRM.DisciplineRecord");
            DropTable("HRM.DisciplineMeasuresType");
            DropTable("HRM.DisciplineMeasuresDetail");
            DropTable("HRM.DisciplineMeasures");
            DropTable("HRM.DisciplinaryCase");
            DropTable("HRM.Disability");
            DropTable("EXCEED.DepartmentCostCenter");
            DropTable("EXCEED.CustomerTicketView");
            DropTable("EXCEED.CPOManagement");
            DropTable("HRM.CourseTracking");
            DropTable("Common.Country");
            DropTable("HRM.Cosign");
            DropTable("EXCEED.ContractManagement");
            DropTable("EXCEED.ContractManagementItem");
            DropTable("HRM.Contract");
            DropTable("common.Configuration");
            DropTable("HRM.Competency");
            DropTable("HRM.MainCompany");
            DropTable("HRM.Committee");
            DropTable("HRM.Commitment");
            DropTable("HRM.Clearance");
            DropTable("HRM.ClearanceProvider");
            DropTable("EXCEED.BudgetUsage");
            DropTable("EXCEED.BudgetTransfer");
            DropTable("EXCEED.BudgetPlanMonthly");
            DropTable("EXCEED.BudgetPlan");
            DropTable("HRM.Branch");
            DropTable("EXCEED.TenderValidation");
            DropTable("EXCEED.TenderMinuteUploadDBModel");
            DropTable("EXCEED.Tender");
            DropTable("EXCEED.StoreRequisitionValidation");
            DropTable("EXCEED.StoreRequisitionPropertyValidation");
            DropTable("EXCEED.StoreRequisitionItem");
            DropTable("EXCEED.StoreRequisitionDistribution");
            DropTable("EXCEED.StoreRequisition");
            DropTable("EXCEED.PurchaseRequisitionUserDepartmentValidation");
            DropTable("EXCEED.PurchaseRequisitionPurchaseDepartmentValidation");
            DropTable("EXCEED.PurchaseRequisitionItem");
            DropTable("EXCEED.PurchaseRequisitionDistribution");
            DropTable("EXCEED.PurchaseRequisition");
            DropTable("EXCEED.LotItems");
            DropTable("EXCEED.LotFinancialOffer");
            DropTable("EXCEED.TenderEvaluationResult");
            DropTable("EXCEED.TenderEvaluationCriteria");
            DropTable("EXCEED.BidPurchasingSuppliers");
            DropTable("EXCEED.BidPParticipatingSuppliers");
            DropTable("EXCEED.Lot");
            DropTable("EXCEED.BidAwardedSupplier");
            DropTable("Common.Bank");
            DropTable("Common.BankReference");
            DropTable("EXCEED.BankGuarantee");
            DropTable("EXCEED.BankAccount");
            DropTable("HRM.Attendance");
            DropTable("common.Attachment");
            DropTable("Payroll.Association");
            DropTable("EXCEED.ArStandardCollectionValidation");
            DropTable("EXCEED.ArStandardCollectionLines");
            DropTable("EXCEED.ArStandardCollection");
            DropTable("EXCEED.ArStandardCollectionDistribution");
            DropTable("EXCEED.ARSetupTransactionType");
            DropTable("EXCEED.ARSetupTransactionClass");
            DropTable("EXCEED.ARSetupTaxRate");
            DropTable("EXCEED.ARSetupReceiptBalanceAccount");
            DropTable("EXCEED.ARSetupProductServiceAccount");
            DropTable("EXCEED.ARRemitAddress");
            DropTable("EXCEED.ARMiscelaneousCollectionValidation");
            DropTable("EXCEED.ARMiscelaneousCollection");
            DropTable("EXCEED.ARMiscelaneousCollectionDistribution");
            DropTable("EXCEED.ArStandardCollectionViewModel");
            DropTable("EXCEED.ArInvoiceValidation");
            DropTable("EXCEED.ArInvoiceLines");
            DropTable("EXCEED.ArInvoice");
            DropTable("EXCEED.ArInvoiceDistribution");
            DropTable("EXCEED.ArCustomerType");
            DropTable("EXCEED.ArInvoiceViewModel");
            DropTable("EXCEED.ArCustomerProfile");
            DropTable("EXCEED.APSupplierType");
            DropTable("EXCEED.APSupplierRegistration");
            DropTable("EXCEED.APsupplierBlacklist");
            DropTable("EXCEED.ApSetupReceiptBalanceAccount");
            DropTable("EXCEED.ApSetupItem");
            DropTable("EXCEED.Sales");
            DropTable("EXCEED.Revaluation");
            DropTable("EXCEED.Location");
            DropTable("EXCEED.Disposal");
            DropTable("EXCEED.FixedAssetGroup");
            DropTable("EXCEED.DepreciationRule");
            DropTable("EXCEED.AnnualDepreciation");
            DropTable("EXCEED.AssetRegistration");
            DropTable("EXCEED.Appreciation");
            DropTable("HRM.ApplicantTraining");
            DropTable("HRM.ApplicantScreeningResult");
            DropTable("HRM.ApplicantionForm");
            DropTable("HRM.Rank");
            DropTable("HRM.Structure");
            DropTable("HRM.MainCompanyStructure");
            DropTable("HRM.RecruitmentPlan");
            DropTable("HRM.Vacancy");
            DropTable("HRM.ApplicantRegistration");
            DropTable("HRM.ApplicantExperience");
            DropTable("HRM.ApplicantExamResult");
            DropTable("EXCEED.ApPaymentValidation");
            DropTable("EXCEED.ApPayment");
            DropTable("EXCEED.ApPaymentDistribution");
            DropTable("EXCEED.APPaymentsViewModel");
            DropTable("EXCEED.ApPaybleInvoiceValidation");
            DropTable("EXCEED.ApPaybleInvoicesLines");
            DropTable("EXCEED.ApPaybleInvoices");
            DropTable("EXCEED.ApPaybleInvoiceDistribution");
            DropTable("EXCEED.ApMiscellaneousPaymentValidation");
            DropTable("EXCEED.ApMiscellaneousPayment");
            DropTable("EXCEED.ApMiscellaneousPaymentDistribution");
            DropTable("EXCEED.APBusinessType");
            DropTable("EXCEED.UoM");
            DropTable("EXCEED.StoreKeeperAssignment");
            DropTable("EXCEED.InventoryStore");
            DropTable("EXCEED.BinCardLine");
            DropTable("EXCEED.StoreItemAssignment");
            DropTable("EXCEED.ItemCategory");
            DropTable("EXCEED.InventoryItem");
            DropTable("EXCEED.GlFiscalYear");
            DropTable("EXCEED.GLPeriod");
            DropTable("EXCEED.AnnualNeedAssesmentValidation");
            DropTable("EXCEED.AnnualNeedAssesment");
            DropTable("EXCEED.AnnualNeedAssesmentItem");
            DropTable("HRM.AllowanceTypeRule");
            DropTable("HRM.AllowancePerEmployee");
            DropTable("HRM.AllowanceDefination");
            DropTable("Payroll.Advance");
            DropTable("common.Address");
            DropTable("common.Period");
            DropTable("common.Lookup");
            DropTable("Payroll.Addition");
            DropTable("Insurance.AccidentRecord");
            DropTable("Insurance.AccidentMedicalRecord");
            DropTable("EXCEED.UserCardItems");
            DropTable("HRM.Employee");
            DropTable("HRM.Absence");
        }
    }
}
