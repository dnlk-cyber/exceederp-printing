namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetCRMmerge : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.Accident",
                c => new
                    {
                        AccidentID = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        DateOfOccurrence = c.DateTime(nullable: false),
                        CauseOfAccident = c.String(nullable: false),
                        LevelOfDamage = c.String(nullable: false),
                        Accountablebody = c.String(nullable: false),
                        AccidentLocation = c.String(nullable: false),
                        LicenseNo = c.String(),
                        Policereport = c.String(),
                        Selectreport = c.String(),
                        Description = c.String(),
                        CostcoveredbyInsurance = c.Int(nullable: false),
                        CostcoveredbyCompany = c.Int(nullable: false),
                        CostcoveredbyOperator = c.Int(nullable: false),
                        CostcoveredbyOtherBodies = c.Int(nullable: false),
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
                .PrimaryKey(t => t.AccidentID)
                .ForeignKey("EXCEED.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "common.AccountMap",
                c => new
                    {
                        AccountMapID = c.Int(nullable: false, identity: true),
                        ControlAccountID = c.String(nullable: false),
                        AccountID = c.String(nullable: false),
                        ReferenceType = c.String(),
                        Reference = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccountMapID);
            
            CreateTable(
                "common.AccountRequirement",
                c => new
                    {
                        AccountRequirementID = c.Int(nullable: false, identity: true),
                        ControlAccountID = c.String(),
                        ObjectType = c.String(),
                        IsMandatory = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccountRequirementID);
            
            CreateTable(
                "common.Account",
                c => new
                    {
                        AccountID = c.Int(nullable: false, identity: true),
                        ControlAccount = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Parent = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccountID);
            
            CreateTable(
                "common.ActivityDefinition",
                c => new
                    {
                        ActivityDefinitionID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        Description = c.String(),
                        Index = c.Int(nullable: false),
                        HasIssuingEffect = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ActivityDefinitionID);
            
            CreateTable(
                "common.Activity",
                c => new
                    {
                        ActivityID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        ActivityDefinition = c.String(),
                        User = c.String(),
                        StartTimeStamp = c.DateTime(nullable: false),
                        EndTimeStamp = c.DateTime(nullable: false),
                        OrganizationUnitDefinition = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ActivityID);
            
            CreateTable(
                "EXCEED.AdministrativeJobApproval",
                c => new
                    {
                        AdministrativeJobApprovalID = c.Int(nullable: false, identity: true),
                        AdministrativeJobId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdministrativeJobApprovalID);
            
            CreateTable(
                "EXCEED.AnnualInventoryInspectionApproval",
                c => new
                    {
                        AnnualInventoryInspectionApprovalID = c.Int(nullable: false, identity: true),
                        AnnualEquipmentInventoryInspectionId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualInventoryInspectionApprovalID)
                .ForeignKey("EXCEED.AnnualEquipmentInventoryInspection", t => t.AnnualEquipmentInventoryInspectionId, cascadeDelete: true)
                .Index(t => t.AnnualEquipmentInventoryInspectionId);
            
            CreateTable(
                "EXCEED.AssignedEquipment",
                c => new
                    {
                        AssignedEquipmentID = c.Guid(nullable: false),
                        RequestNumber = c.Guid(nullable: false),
                        EquipmentID = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        EquipmentName = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        AssignedDate = c.DateTime(),
                        RequestedDate = c.DateTime(),
                        Type = c.String(nullable: false),
                        IsAssigned = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.AssignedEquipmentID);
            
            CreateTable(
                "EXCEED.AssignEquipmentApproval",
                c => new
                    {
                        AssignEquipmentApprovalID = c.Int(nullable: false, identity: true),
                        AssignEquipmentId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        AssignedEquipment_AssignedEquipmentID = c.Guid(),
                    })
                .PrimaryKey(t => t.AssignEquipmentApprovalID)
                .ForeignKey("EXCEED.AssignedEquipment", t => t.AssignedEquipment_AssignedEquipmentID)
                .Index(t => t.AssignedEquipment_AssignedEquipmentID);
            
            CreateTable(
                "common.BankAccountDetail",
                c => new
                    {
                        BankAccountDetailID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        Description = c.String(),
                        BankAccountDetailType = c.Int(nullable: false),
                        Bank = c.String(),
                        AccountNumber = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BankAccountDetailID);
            
            CreateTable(
                "common.Beginning",
                c => new
                    {
                        BeginningID = c.Int(nullable: false, identity: true),
                        ReferenceType = c.String(),
                        Reference = c.String(),
                        Period = c.String(nullable: false),
                        Value = c.Double(nullable: false),
                        IsProvisional = c.Boolean(nullable: false),
                        Remark = c.String(),
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
                .PrimaryKey(t => t.BeginningID);
            
            CreateTable(
                "EXCEED.BinCardLineReserve",
                c => new
                    {
                        BinCardLineReserveID = c.Int(nullable: false, identity: true),
                        StockLevelID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        VoucherDefinitionID = c.String(),
                        VoucherID = c.String(),
                        BalanceQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingBalanceQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InActiveAfterDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsTemporal = c.Boolean(nullable: false),
                        ReserveStatus = c.String(),
                    })
                .PrimaryKey(t => t.BinCardLineReserveID);
            
            CreateTable(
                "EXCEED.CampaignProduct",
                c => new
                    {
                        CampaignProductID = c.Int(nullable: false, identity: true),
                        CampaignID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Feedback = c.String(),
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
                .PrimaryKey(t => t.CampaignProductID);
            
            CreateTable(
                "CRM.Campaign",
                c => new
                    {
                        CampaignID = c.Int(nullable: false, identity: true),
                        AssignedTo = c.String(),
                        TeamId = c.String(),
                        CartID = c.String(),
                        CompaignType = c.String(),
                        Budget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpectedRevenu = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Objective = c.String(),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CampaignID);
            
            CreateTable(
                "EXCEED.Cannibalization",
                c => new
                    {
                        CannibalizationID = c.Guid(nullable: false),
                        PartNeededFleetsID = c.Guid(nullable: false),
                        PartTakenFleetsID = c.Guid(nullable: false),
                        Model = c.String(nullable: false),
                        UnitofMeasurement = c.String(nullable: false),
                        PartName = c.String(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Status = c.String(),
                        TakenBy = c.String(),
                        Description = c.String(),
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
                .PrimaryKey(t => t.CannibalizationID);
            
            CreateTable(
                "EXCEED.CannibalizationApproval",
                c => new
                    {
                        CannibalizationApprovalID = c.Int(nullable: false, identity: true),
                        CannibalizationId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CannibalizationApprovalID)
                .ForeignKey("EXCEED.Cannibalization", t => t.CannibalizationId, cascadeDelete: true)
                .Index(t => t.CannibalizationId);
            
            CreateTable(
                "Common.Cart",
                c => new
                    {
                        CartID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Index = c.Int(nullable: false),
                        Parent = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CartID);
            
            CreateTable(
                "common.CartTransaction",
                c => new
                    {
                        CartTransactionID = c.Int(nullable: false, identity: true),
                        Cart = c.String(),
                        Voucher = c.String(),
                        Description = c.String(),
                        Value = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CartTransactionID);
            
            CreateTable(
                "Sales.CashSalesInvoice",
                c => new
                    {
                        CashSalesInvoiceID = c.Int(nullable: false, identity: true),
                        Consignee = c.String(nullable: false),
                        References = c.String(),
                        IssuedDate = c.DateTime(nullable: false),
                        IsIssued = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        IsVoid = c.Boolean(nullable: false),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period = c.String(),
                        LastObjectState = c.String(),
                        PaymentMethod = c.String(),
                        RecieveMethod = c.String(),
                        Remark = c.String(),
                        Status = c.String(),
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
                .PrimaryKey(t => t.CashSalesInvoiceID);
            
            CreateTable(
                "common.CommunicationSource",
                c => new
                    {
                        CommunicationSourceID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        CommunicationType = c.String(),
                        CommunicationCode = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CommunicationSourceID);
            
            CreateTable(
                "common.ControlAccount",
                c => new
                    {
                        ControlAccountId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        NormalBalance = c.String(),
                        AccountCategory = c.String(),
                        AccountSubCategory = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ControlAccountId);
            
            CreateTable(
                "Common.CreditSalesInvoice",
                c => new
                    {
                        CreditSalesInvoiceID = c.Int(nullable: false, identity: true),
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
                        Status = c.String(),
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
                .PrimaryKey(t => t.CreditSalesInvoiceID);
            
            CreateTable(
                "EXCEED.CreditSetting",
                c => new
                    {
                        CreditSettingID = c.Int(nullable: false, identity: true),
                        OrganizationID = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CreditSettingID);
            
            CreateTable(
                "EXCEED.CRMHoliday",
                c => new
                    {
                        CRMHolidayID = c.Int(nullable: false, identity: true),
                        HolidayDefinitionID = c.Int(nullable: false),
                        PeriodID = c.Int(nullable: false),
                        Remark = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CRMHolidayID)
                .ForeignKey("EXCEED.HolidayDefinition", t => t.HolidayDefinitionID, cascadeDelete: true)
                .ForeignKey("common.Period", t => t.PeriodID, cascadeDelete: true)
                .Index(t => t.HolidayDefinitionID)
                .Index(t => t.PeriodID);
            
            CreateTable(
                "EXCEED.HolidayDefinition",
                c => new
                    {
                        HolidayDefinitionID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        IsFixed = c.Boolean(nullable: false),
                        WillClose = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.HolidayDefinitionID);
            
            CreateTable(
                "common.Currency",
                c => new
                    {
                        CurrencyID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Country = c.String(),
                        Abbrevation = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CurrencyID);
            
            CreateTable(
                "common.CurrencyPreference",
                c => new
                    {
                        CurrencyPreferenceID = c.Int(nullable: false, identity: true),
                        Currency = c.String(),
                        Reference = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CurrencyPreferenceID);
            
            CreateTable(
                "EXCEED.CustomerHandlingTeam",
                c => new
                    {
                        CustomerHandlingTeamID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Category = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        ParentID = c.Int(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CustomerHandlingTeamID);
            
            CreateTable(
                "CRM.CustomerTicket",
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
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CustomerTicketID);
            
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
                "EXCEED.DefaultLedgerAccount",
                c => new
                    {
                        DefaultLedgerAccountID = c.Int(nullable: false, identity: true),
                        ControlAccount = c.String(),
                        LedgerAccount = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DefaultLedgerAccountID);
            
            CreateTable(
                "EXCEED.DriverVehicleTransfer",
                c => new
                    {
                        DriverVehicleTransferID = c.Guid(nullable: false),
                        FromOperatorId = c.Guid(nullable: false),
                        ToOperatorId = c.Guid(nullable: false),
                        StartingDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DriverVehicleTransferID);
            
            CreateTable(
                "EXCEED.DriverTransferApproval",
                c => new
                    {
                        DriverTransferApprovalID = c.Int(nullable: false, identity: true),
                        DriverVehicleTransferId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DriverTransferApprovalID)
                .ForeignKey("EXCEED.DriverVehicleTransfer", t => t.DriverVehicleTransferId, cascadeDelete: true)
                .Index(t => t.DriverVehicleTransferId);
            
            CreateTable(
                "EXCEED.Educationallevel",
                c => new
                    {
                        EducationallevelID = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EducationallevelID);
            
            CreateTable(
                "EXCEED.EquipmentName",
                c => new
                    {
                        EquipmentNameID = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsAssigned = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentNameID);
            
            CreateTable(
                "EXCEED.FleetHistory",
                c => new
                    {
                        FleetHistoryID = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        FuelStandard = c.String(),
                        RentalRate = c.Decimal(precision: 18, scale: 2),
                        AnnualDepreciationCost = c.Int(),
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
                .PrimaryKey(t => t.FleetHistoryID)
                .ForeignKey("EXCEED.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "EXCEED.FollowUp",
                c => new
                    {
                        FollowUpId = c.Guid(nullable: false),
                        JobOrderId = c.Guid(nullable: false),
                        Detail = c.Guid(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FollowUpId);
            
            CreateTable(
                "EXCEED.FuelIssueApproval",
                c => new
                    {
                        FuelIssueApprovalID = c.Int(nullable: false, identity: true),
                        FuelIssueId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        Fuel_FuelID = c.Guid(),
                    })
                .PrimaryKey(t => t.FuelIssueApprovalID)
                .ForeignKey("EXCEED.Fuel", t => t.Fuel_FuelID)
                .Index(t => t.Fuel_FuelID);
            
            CreateTable(
                "common.GlobalTypeList",
                c => new
                    {
                        GlobalTypeListID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Category = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.GlobalTypeListID);
            
            CreateTable(
                "common.GlobalTypeTax",
                c => new
                    {
                        GlobalTypeTaxID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        Tax = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.GlobalTypeTaxID);
            
            CreateTable(
                "EXCEED.GoodRecivingNoteApproval",
                c => new
                    {
                        GoodRecivingNoteApprovalID = c.Int(nullable: false, identity: true),
                        SalvageStoreGoodReceivingId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GoodRecivingNoteApprovalID)
                .ForeignKey("EXCEED.SalvageStoreGoodReceiving", t => t.SalvageStoreGoodReceivingId, cascadeDelete: true)
                .Index(t => t.SalvageStoreGoodReceivingId);
            
            CreateTable(
                "EXCEED.SalvageStoreGoodReceiving",
                c => new
                    {
                        SalvageStoreGoodReceivingID = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        SparePartID = c.Guid(nullable: false),
                        VoucherNo = c.Int(nullable: false),
                        Date = c.DateTime(),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalvageStoreGoodReceivingID)
                .ForeignKey("EXCEED.Fleets", t => t.FleetsID, cascadeDelete: true)
                .ForeignKey("EXCEED.SparePart", t => t.SparePartID, cascadeDelete: true)
                .Index(t => t.FleetsID)
                .Index(t => t.SparePartID);
            
            CreateTable(
                "EXCEED.SparePart",
                c => new
                    {
                        SparePartID = c.Guid(nullable: false),
                        SparePartName = c.String(nullable: false),
                        PartNo = c.String(nullable: false),
                        Quantity = c.Int(),
                        UnitPrice = c.Int(nullable: false),
                        ReceiptNo = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SparePartID);
            
            CreateTable(
                "EXCEED.JobOrder",
                c => new
                    {
                        JobOrderID = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        JobOrderNum = c.Int(),
                        Type = c.String(),
                        MaintenanceCenter = c.String(),
                        ProjectName = c.String(nullable: false),
                        PercentCompleted = c.String(),
                        Status = c.String(),
                        DateIssued = c.DateTime(),
                        DateCompleted = c.DateTime(),
                        InspectedBy = c.String(),
                        InspectorComments = c.String(nullable: false),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
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
                .PrimaryKey(t => t.JobOrderID)
                .ForeignKey("EXCEED.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "EXCEED.JobOrderDetail",
                c => new
                    {
                        JobOrderDetailID = c.Guid(nullable: false),
                        JobOrderID = c.Guid(nullable: false),
                        ServiceID = c.Guid(nullable: false),
                        ServiceName = c.String(),
                        Status = c.String(),
                        StartingDate = c.DateTime(),
                        ToDate = c.DateTime(),
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
                .PrimaryKey(t => t.JobOrderDetailID)
                .ForeignKey("EXCEED.JobOrder", t => t.JobOrderID, cascadeDelete: true)
                .ForeignKey("EXCEED.Service", t => t.ServiceID, cascadeDelete: true)
                .Index(t => t.JobOrderID)
                .Index(t => t.ServiceID);
            
            CreateTable(
                "EXCEED.Service",
                c => new
                    {
                        ServiceID = c.Guid(nullable: false),
                        PlannerID = c.Guid(nullable: false),
                        ServiceName = c.String(nullable: false),
                        Type = c.String(),
                        PartCost = c.Int(),
                        LabourCost = c.Int(),
                        Priority = c.String(),
                        DateIntervalType = c.String(nullable: false),
                        DateInterval = c.Int(nullable: false),
                        MileageInterval = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        IsPm = c.String(nullable: false),
                        ServiceType = c.String(nullable: false),
                        ApprovedBy = c.String(),
                        OrgBranchName = c.String(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceID)
                .ForeignKey("EXCEED.Planner", t => t.PlannerID, cascadeDelete: true)
                .Index(t => t.PlannerID);
            
            CreateTable(
                "EXCEED.Planner",
                c => new
                    {
                        PlannerID = c.Guid(nullable: false),
                        PlannerName = c.String(nullable: false),
                        TrackByDate = c.Boolean(nullable: false),
                        TrackByMileage = c.Boolean(nullable: false),
                        DateCrated = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PlannerID);
            
            CreateTable(
                "common.JournalDetail",
                c => new
                    {
                        JournalDetailID = c.Int(nullable: false, identity: true),
                        VoucherType = c.String(),
                        Voucher = c.String(),
                        Account = c.String(),
                        Debit = c.Double(nullable: false),
                        Credit = c.Double(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.JournalDetailID);
            
            CreateTable(
                "common.JournalTemplate",
                c => new
                    {
                        JournalTemplateID = c.Int(nullable: false, identity: true),
                        VoucherDefinition = c.String(),
                        ControlAccount = c.String(),
                        IsDebit = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.JournalTemplateID);
            
            CreateTable(
                "Common.LifeSpan",
                c => new
                    {
                        LifeSpanID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        ProductionDate = c.DateTime(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LifeSpanID);
            
            CreateTable(
                "common.LifeTime",
                c => new
                    {
                        LifeTimeID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Reference = c.String(),
                        Factor = c.String(),
                        Value = c.Double(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LifeTimeID);
            
            CreateTable(
                "EXCEED.LineItem",
                c => new
                    {
                        LineItemID = c.Int(nullable: false, identity: true),
                        References = c.String(),
                        VoucherDefinitionID = c.String(),
                        Voucher = c.String(),
                        StoreID = c.String(nullable: false),
                        Class = c.String(),
                        StockBalanceReserved = c.Decimal(precision: 18, scale: 2),
                        StockBalance = c.Decimal(precision: 18, scale: 2),
                        PriceTag = c.String(),
                        PriceTagDetail = c.String(),
                        ThingID = c.String(nullable: false),
                        Variety = c.String(nullable: false),
                        UnitAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.String(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CalculatedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.LineItemID);
            
            CreateTable(
                "EXCEED.MailModel",
                c => new
                    {
                        MailModelID = c.Int(nullable: false, identity: true),
                        From = c.String(),
                        To = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MailModelID);
            
            CreateTable(
                "EXCEED.MaintenanceCenter",
                c => new
                    {
                        MaintenanceCenterID = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Type = c.String(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceCenterID);
            
            CreateTable(
                "EXCEED.MaintenanceTool",
                c => new
                    {
                        MaintenanceToolID = c.Guid(nullable: false),
                        ToolsName = c.String(nullable: false),
                        UnitofMeasurement = c.String(),
                        Quantity = c.Int(nullable: false),
                        OwnedBy = c.String(),
                        Location = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceToolID);
            
            CreateTable(
                "EXCEED.ServiceTool",
                c => new
                    {
                        ServiceToolID = c.Guid(nullable: false),
                        ServiceID = c.Guid(nullable: false),
                        MaintenanceToolID = c.Guid(nullable: false),
                        ServiceName = c.String(),
                        Date = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceToolID)
                .ForeignKey("EXCEED.MaintenanceTool", t => t.MaintenanceToolID, cascadeDelete: true)
                .Index(t => t.MaintenanceToolID);
            
            CreateTable(
                "EXCEED.Manufacturer",
                c => new
                    {
                        ManufacturerID = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ManufacturerID);
            
            CreateTable(
                "EXCEED.MechanicCrew",
                c => new
                    {
                        MechanicCrewID = c.Guid(nullable: false),
                        FullName = c.String(),
                        SupervisorName = c.String(),
                        Postion = c.String(),
                        DateHired = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MechanicCrewID);
            
            CreateTable(
                "EXCEED.ServiceCrew",
                c => new
                    {
                        ServiceCrewID = c.Guid(nullable: false),
                        ServiceID = c.Guid(nullable: false),
                        MechanicCrewID = c.Guid(nullable: false),
                        ServiceName = c.String(),
                        Date = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceCrewID)
                .ForeignKey("EXCEED.MechanicCrew", t => t.MechanicCrewID, cascadeDelete: true)
                .Index(t => t.MechanicCrewID);
            
            CreateTable(
                "Common.NonCashTransaction",
                c => new
                    {
                        NonCashTransactionID = c.Int(nullable: false, identity: true),
                        Voucher = c.String(),
                        Consignee = c.String(),
                        IsIncoming = c.Boolean(nullable: false),
                        ReceiveMethod = c.String(),
                        ReceiveProcessor = c.String(),
                        Index = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssueDate = c.DateTime(nullable: false),
                        MaturityDate = c.DateTime(nullable: false),
                        Number = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Executed = c.Boolean(nullable: false),
                        CurrencyCode = c.String(),
                        Remark = c.String(),
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
                .PrimaryKey(t => t.NonCashTransactionID);
            
            CreateTable(
                "Common.ObjectAccountRequirement",
                c => new
                    {
                        ObjectAccountRequirementID = c.Int(nullable: false, identity: true),
                        ControlAccount = c.String(),
                        ObjectType = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ObjectAccountRequirementID);
            
            CreateTable(
                "Common.ObjectStateDefinition",
                c => new
                    {
                        ObjectStateDefinitionID = c.Int(nullable: false, identity: true),
                        GSLType = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Color = c.String(nullable: false),
                        Font = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ObjectStateDefinitionID);
            
            CreateTable(
                "common.ObjectState",
                c => new
                    {
                        ObjectStateID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        ObjectStateDefinitionID = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ObjectStateID);
            
            CreateTable(
                "EXCEED.OperatorPosition",
                c => new
                    {
                        OperatorPositionID = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.OperatorPositionID);
            
            CreateTable(
                "Common.OrganizationCategory",
                c => new
                    {
                        OrganizationCategoryID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Index = c.Int(nullable: false),
                        Parent = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.OrganizationCategoryID);
            
            CreateTable(
                "Common.OrganizationCustomer",
                c => new
                    {
                        OrganizationCustomerID = c.Int(nullable: false, identity: true),
                        TradeName = c.String(nullable: false),
                        BrandName = c.String(nullable: false),
                        BusinessType = c.String(nullable: false),
                        Category = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsCredit = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.OrganizationCustomerID);
            
            CreateTable(
                "EXCEED.OtherCompanyMaintenaceApproval",
                c => new
                    {
                        OtherCompanyMaintenaceApprovalID = c.Int(nullable: false, identity: true),
                        OtherCompanyMaintenaceId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        OtherCompanyMaintenace_OtherCompanyMaintenanceID = c.Guid(),
                    })
                .PrimaryKey(t => t.OtherCompanyMaintenaceApprovalID)
                .ForeignKey("EXCEED.OtherCompanyMaintenance", t => t.OtherCompanyMaintenace_OtherCompanyMaintenanceID)
                .Index(t => t.OtherCompanyMaintenace_OtherCompanyMaintenanceID);
            
            CreateTable(
                "EXCEED.OtherCompanyMaintenance",
                c => new
                    {
                        OtherCompanyMaintenanceID = c.Guid(nullable: false),
                        MaintenanceName = c.String(nullable: false),
                        NameofCustomer = c.String(nullable: false),
                        CustomerPhone = c.String(nullable: false),
                        CustomerEmail = c.String(),
                        OperatorName = c.String(),
                        BankSlipNo = c.String(nullable: false),
                        BankBranchName = c.String(nullable: false),
                        Description = c.String(),
                        ApprovedBy = c.String(),
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
                .PrimaryKey(t => t.OtherCompanyMaintenanceID);
            
            CreateTable(
                "EXCEED.OutsourceMaintenance",
                c => new
                    {
                        OutsourceMaintenanceID = c.Guid(nullable: false),
                        FleetID = c.Guid(nullable: false),
                        TypeOfRepair = c.String(nullable: false),
                        fromDate = c.DateTime(nullable: false),
                        toDate = c.DateTime(nullable: false),
                        ReasonForMaintenace = c.String(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        Fleet_FleetsID = c.Guid(),
                    })
                .PrimaryKey(t => t.OutsourceMaintenanceID)
                .ForeignKey("EXCEED.Fleets", t => t.Fleet_FleetsID)
                .Index(t => t.Fleet_FleetsID);
            
            CreateTable(
                "EXCEED.OutsourceMaintenanceApproval",
                c => new
                    {
                        OutsourceMaintenanceApprovalID = c.Int(nullable: false, identity: true),
                        OutsourceMaintenanceId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.OutsourceMaintenanceApprovalID)
                .ForeignKey("EXCEED.OutsourceMaintenance", t => t.OutsourceMaintenanceId, cascadeDelete: true)
                .Index(t => t.OutsourceMaintenanceId);
            
            CreateTable(
                "Common.PersonCategory",
                c => new
                    {
                        PersonCategoryID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Index = c.Int(nullable: false),
                        Parent = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PersonCategoryID);
            
            CreateTable(
                "EXCEED.PlanAndEvaluationApproval",
                c => new
                    {
                        PlanAndEvaluationApprovalID = c.Int(nullable: false, identity: true),
                        EquipmentPlanandEvaluationsId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PlanAndEvaluationApprovalID)
                .ForeignKey("EXCEED.EquipmentPlanandEvaluations", t => t.EquipmentPlanandEvaluationsId, cascadeDelete: true)
                .Index(t => t.EquipmentPlanandEvaluationsId);
            
            CreateTable(
                "EXCEED.PreCustomer",
                c => new
                    {
                        PreCustomerID = c.Int(nullable: false, identity: true),
                        TradeName = c.String(),
                        BrandName = c.String(),
                        BusinessType = c.String(),
                        Category = c.String(),
                        Status = c.String(),
                        IsActive = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.PreCustomerID);
            
            CreateTable(
                "EXCEED.PreCustomerSchedule",
                c => new
                    {
                        PreCustomerScheduleID = c.Int(nullable: false, identity: true),
                        PreCustomerID = c.String(),
                        ScheduledDate = c.DateTime(nullable: false),
                        ScheduleStatus = c.String(),
                        ScheduleOutcome = c.String(),
                        IsActive = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.PreCustomerScheduleID);
            
            CreateTable(
                "EXCEED.PreCustomerView",
                c => new
                    {
                        PreCustomerViewID = c.Int(nullable: false, identity: true),
                        TradeName = c.String(),
                        BrandName = c.String(),
                        BusinessType = c.String(),
                        Category = c.String(),
                        Status = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PreCustomerViewID);
            
            CreateTable(
                "common.Price",
                c => new
                    {
                        PriceID = c.Int(nullable: false, identity: true),
                        Thing = c.String(nullable: false, maxLength: 50),
                        ReferenceType = c.String(nullable: false),
                        Reference = c.String(),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsDefault = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                        Remark = c.String(),
                        Variety = c.String(nullable: false),
                        Class = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriceID)
                .Index(t => new { t.Thing, t.Priority }, unique: true, name: "IX_Priority");
            
            CreateTable(
                "Common.Product",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Type = c.String(),
                        CategoryID = c.String(nullable: false),
                        UOM = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
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
                .PrimaryKey(t => t.ProductID);
            
            CreateTable(
                "EXCEED.ReceiveMethod",
                c => new
                    {
                        ReceiveMethodID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ReceiveMethodID);
            
            CreateTable(
                "common.Religion",
                c => new
                    {
                        ReligionID = c.Int(nullable: false, identity: true),
                        Person = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ReligionID);
            
            CreateTable(
                "EXCEED.RentCustomerFeedBack",
                c => new
                    {
                        RentCustomerFeedBackID = c.Guid(nullable: false),
                        RequestID = c.Guid(nullable: false),
                        DetailFeedBack = c.String(nullable: false),
                        FeedBackDate = c.DateTime(),
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
                .PrimaryKey(t => t.RentCustomerFeedBackID)
                .ForeignKey("EXCEED.RentRequest", t => t.RequestID, cascadeDelete: true)
                .Index(t => t.RequestID);
            
            CreateTable(
                "EXCEED.RentRequest",
                c => new
                    {
                        RentRequestID = c.Guid(nullable: false),
                        RequestNumber = c.Int(),
                        EquipmentStatus = c.String(nullable: false),
                        RequestQuantity = c.Int(),
                        FromBranchName = c.String(nullable: false),
                        FromStructureName = c.String(nullable: false),
                        RequestingBranchName = c.String(),
                        RequestingStructureName = c.String(),
                        StartingDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        Description = c.String(),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
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
                .PrimaryKey(t => t.RentRequestID);
            
            CreateTable(
                "EXCEED.RentForInternalApproval",
                c => new
                    {
                        RentForInternalApprovalID = c.Int(nullable: false, identity: true),
                        RentInternalProjectId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentForInternalApprovalID)
                .ForeignKey("EXCEED.RentInternalProject", t => t.RentInternalProjectId, cascadeDelete: true)
                .Index(t => t.RentInternalProjectId);
            
            CreateTable(
                "EXCEED.RentInternalProject",
                c => new
                    {
                        RentInternalProjectID = c.Guid(nullable: false),
                        EquipmentType = c.String(nullable: false),
                        EquipmentOwner = c.String(nullable: false),
                        AssignedDate = c.DateTime(),
                        StartingDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        AssignedBy = c.String(),
                        ApprovedBy = c.String(),
                        Status = c.String(),
                        RequestQuantity = c.Int(nullable: false),
                        AssignedQuantity = c.Int(),
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
                .PrimaryKey(t => t.RentInternalProjectID);
            
            CreateTable(
                "EXCEED.RentFromExternal",
                c => new
                    {
                        RentFromExternalID = c.Guid(nullable: false),
                        EquipmentType = c.String(nullable: false),
                        CompanyName = c.String(nullable: false),
                        CompanyAddress = c.String(nullable: false),
                        CompanyPhno = c.String(nullable: false),
                        CompanyEmail = c.String(),
                        WorkingCapacity = c.String(nullable: false),
                        RentalRateIncludingVAT = c.Boolean(nullable: false),
                        Status = c.String(),
                        Quantity = c.Int(),
                        FuelCoveredBy = c.String(nullable: false),
                        ProjectLocation = c.String(nullable: false),
                        UnitOfWorkMeasurement = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        NameBankBranch = c.String(nullable: false),
                        BankFullAddress = c.String(nullable: false),
                        BankSlipNo = c.String(nullable: false),
                        AdvancePayment = c.Int(nullable: false),
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
                .PrimaryKey(t => t.RentFromExternalID);
            
            CreateTable(
                "EXCEED.RentFromExternalApproval",
                c => new
                    {
                        RentFromExternalApprovalID = c.Int(nullable: false, identity: true),
                        RentFromExternalId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentFromExternalApprovalID)
                .ForeignKey("EXCEED.RentFromExternal", t => t.RentFromExternalId, cascadeDelete: true)
                .Index(t => t.RentFromExternalId);
            
            CreateTable(
                "EXCEED.RentOtherSupplierApproval",
                c => new
                    {
                        RentOtherSupplierApprovalID = c.Int(nullable: false, identity: true),
                        RentOtherSupplierId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        RentOtherSupplier_RentOtherSuppliersID = c.Guid(),
                    })
                .PrimaryKey(t => t.RentOtherSupplierApprovalID)
                .ForeignKey("EXCEED.RentOtherSuppliers", t => t.RentOtherSupplier_RentOtherSuppliersID)
                .Index(t => t.RentOtherSupplier_RentOtherSuppliersID);
            
            CreateTable(
                "EXCEED.RentOtherSuppliers",
                c => new
                    {
                        RentOtherSuppliersID = c.Guid(nullable: false),
                        EquipmentType = c.String(nullable: false),
                        WorkingCapacity = c.String(nullable: false),
                        RentalRateIncludingVAT = c.Boolean(nullable: false),
                        Status = c.String(),
                        Quantity = c.Int(),
                        FuelCoveredBy = c.String(nullable: false),
                        ProjectLocation = c.String(nullable: false),
                        UnitOfWorkMeasurement = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        NameBankBranch = c.String(),
                        BankFullAddress = c.String(),
                        BankSlipNo = c.String(),
                        AdvancePayment = c.Int(nullable: false),
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
                .PrimaryKey(t => t.RentOtherSuppliersID);
            
            CreateTable(
                "EXCEED.RentRequestApproval",
                c => new
                    {
                        RentRequestApprovalID = c.Int(nullable: false, identity: true),
                        RentRequestId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentRequestApprovalID)
                .ForeignKey("EXCEED.RentRequest", t => t.RentRequestId, cascadeDelete: true)
                .Index(t => t.RentRequestId);
            
            CreateTable(
                "EXCEED.RepairType",
                c => new
                    {
                        RepairTypeID = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RepairTypeID);
            
            CreateTable(
                "Common.Role",
                c => new
                    {
                        RoleID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        ParentID = c.Int(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RoleID);
            
            CreateTable(
                "Common.SalesOrderVoucher",
                c => new
                    {
                        SalesOrderVoucherID = c.Int(nullable: false, identity: true),
                        References = c.String(),
                        Consignee = c.String(nullable: false),
                        IssuedDate = c.DateTime(nullable: false),
                        IsIssued = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        IsVoid = c.Boolean(nullable: false),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period = c.String(),
                        LastObjectState = c.String(),
                        PaymentMethod = c.String(),
                        Remark = c.String(),
                        Status = c.String(),
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
                .PrimaryKey(t => t.SalesOrderVoucherID);
            
            CreateTable(
                "common.SalesQuoteVoucher",
                c => new
                    {
                        SalesQuoteVoucherID = c.Int(nullable: false, identity: true),
                        Consignee = c.String(),
                        IssuedDate = c.DateTime(nullable: false),
                        IsIssued = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        IsVoid = c.Boolean(nullable: false),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period = c.String(),
                        LastObjectState = c.String(),
                        PaymentMethod = c.String(),
                        Remark = c.String(),
                        Status = c.String(),
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
                .PrimaryKey(t => t.SalesQuoteVoucherID);
            
            CreateTable(
                "EXCEED.SalesQuoteVoucherView",
                c => new
                    {
                        SalesQuoteVoucherID = c.Int(nullable: false, identity: true),
                        Consignee = c.String(nullable: false),
                        IssuedDate = c.DateTime(nullable: false),
                        IsIssued = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        IsVoid = c.Boolean(nullable: false),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period = c.String(),
                        LastObjectState = c.String(),
                        PaymentMethod = c.String(),
                        Remark = c.String(),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalCharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesQuoteVoucherID);
            
            CreateTable(
                "EXCEED.SalvageStore",
                c => new
                    {
                        SalvageStoreID = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        SparePartID = c.Guid(nullable: false),
                        VoucherNo = c.Int(nullable: false),
                        Date = c.DateTime(),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalvageStoreID)
                .ForeignKey("EXCEED.Fleets", t => t.FleetsID, cascadeDelete: true)
                .ForeignKey("EXCEED.SparePart", t => t.SparePartID, cascadeDelete: true)
                .Index(t => t.FleetsID)
                .Index(t => t.SparePartID);
            
            CreateTable(
                "EXCEED.SalvageStoreApproval",
                c => new
                    {
                        SalvageStoreApprovalID = c.Int(nullable: false, identity: true),
                        SalvageStoreId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalvageStoreApprovalID)
                .ForeignKey("EXCEED.SalvageStore", t => t.SalvageStoreId, cascadeDelete: true)
                .Index(t => t.SalvageStoreId);
            
            CreateTable(
                "common.SerialCode",
                c => new
                    {
                        SerialCodeID = c.Int(nullable: false, identity: true),
                        LineItem = c.String(),
                        SerialDefinition = c.String(),
                        Number = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SerialCodeID);
            
            CreateTable(
                "common.SerialDefinition",
                c => new
                    {
                        SerialDefinitionID = c.Int(nullable: false, identity: true),
                        Thing = c.String(),
                        Description = c.String(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SerialDefinitionID);
            
            CreateTable(
                "EXCEED.ServiceSpare",
                c => new
                    {
                        ServiceSpareID = c.Guid(nullable: false),
                        ServiceID = c.Guid(nullable: false),
                        SparePartID = c.Guid(nullable: false),
                        ServiceName = c.String(),
                        Date = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ServiceSpare_ServiceSpareID = c.Guid(),
                    })
                .PrimaryKey(t => t.ServiceSpareID)
                .ForeignKey("EXCEED.ServiceSpare", t => t.ServiceSpare_ServiceSpareID)
                .ForeignKey("EXCEED.SparePart", t => t.SparePartID, cascadeDelete: true)
                .Index(t => t.SparePartID)
                .Index(t => t.ServiceSpare_ServiceSpareID);
            
            CreateTable(
                "EXCEED.SitesVehicleTransfer",
                c => new
                    {
                        SitesVehicleTransferID = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        StartingDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        InitialSite = c.String(nullable: false),
                        DestinationSite = c.String(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SitesVehicleTransferID)
                .ForeignKey("EXCEED.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "EXCEED.SiteVehicleApproval",
                c => new
                    {
                        SiteVehicleApprovalID = c.Int(nullable: false, identity: true),
                        SitesVehicleTransferId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SiteVehicleApprovalID)
                .ForeignKey("EXCEED.SitesVehicleTransfer", t => t.SitesVehicleTransferId, cascadeDelete: true)
                .Index(t => t.SitesVehicleTransferId);
            
            CreateTable(
                "Common.Specification",
                c => new
                    {
                        SpecificationID = c.Int(nullable: false, identity: true),
                        ThingType = c.String(),
                        ThingID = c.String(nullable: false, maxLength: 50),
                        IsKey = c.Boolean(nullable: false),
                        Category = c.String(nullable: false),
                        Index = c.Int(nullable: false),
                        Attribute = c.String(nullable: false),
                        Value = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SpecificationID)
                .Index(t => new { t.ThingID, t.Index }, unique: true, name: "IX_SpecificationIndex");
            
            CreateTable(
                "common.StockBalance",
                c => new
                    {
                        StockBalanceID = c.Int(nullable: false, identity: true),
                        StockBalanceType = c.String(nullable: false),
                        ThingType = c.String(),
                        Variety = c.String(),
                        Thing = c.String(),
                        Class = c.String(),
                        StoreID = c.String(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period = c.String(nullable: false),
                        Remark = c.String(),
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
                .PrimaryKey(t => t.StockBalanceID);
            
            CreateTable(
                "common.StockLevel",
                c => new
                    {
                        StockLevelID = c.Int(nullable: false, identity: true),
                        ThingType = c.String(),
                        Thing = c.String(),
                        StoreID = c.String(nullable: false),
                        MinimumLevel = c.Double(nullable: false),
                        MaximumLevel = c.Double(nullable: false),
                        LeadTime = c.Double(nullable: false),
                        EconomicOrderQuantity = c.Double(nullable: false),
                        Remark = c.String(),
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
                .PrimaryKey(t => t.StockLevelID);
            
            CreateTable(
                "Common.Store",
                c => new
                    {
                        StoreID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        ParentId = c.String(),
                        Specialization = c.String(nullable: false),
                        Abbrivation = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreID);
            
            CreateTable(
                "common.StoreTransaction",
                c => new
                    {
                        StoreTransactionID = c.Int(nullable: false, identity: true),
                        Voucher = c.String(),
                        Source = c.String(),
                        TransactionType = c.String(),
                        Amount = c.String(),
                        Destination = c.String(),
                        HasEffect = c.Boolean(nullable: false),
                        beforeStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        afterStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
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
                .PrimaryKey(t => t.StoreTransactionID);
            
            CreateTable(
                "EXCEED.SubCountry",
                c => new
                    {
                        SubCountryID = c.Int(nullable: false, identity: true),
                        CountryID = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        AlternativeName = c.String(nullable: false),
                        Parent = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SubCountryID);
            
            CreateTable(
                "common.Tax",
                c => new
                    {
                        TaxID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaxID);
            
            CreateTable(
                "common.TaxTransaction",
                c => new
                    {
                        TaxTransactionID = c.Int(nullable: false, identity: true),
                        Voucher = c.String(),
                        TaxType = c.String(),
                        TaxableAmount = c.Double(nullable: false),
                        TaxAmount = c.Double(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaxTransactionID);
            
            CreateTable(
                "EXCEED.TechnicalJobApproval",
                c => new
                    {
                        TechnicalJobApprovalID = c.Int(nullable: false, identity: true),
                        TechnicalJobId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TechnicalJobApprovalID);
            
            CreateTable(
                "common.Term",
                c => new
                    {
                        TermID = c.Int(nullable: false, identity: true),
                        VoucherDefinitionID = c.Int(nullable: false),
                        LookupID = c.Int(nullable: false),
                        DefaultValue = c.String(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TermID)
                .ForeignKey("common.Lookup", t => t.LookupID, cascadeDelete: true)
                .ForeignKey("common.VoucherDefinition", t => t.VoucherDefinitionID, cascadeDelete: true)
                .Index(t => t.VoucherDefinitionID)
                .Index(t => t.LookupID);
            
            CreateTable(
                "common.VoucherDefinition",
                c => new
                    {
                        VoucherDefinitionID = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        Name = c.String(),
                        Abbriviation = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        JournalType = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.VoucherDefinitionID);
            
            CreateTable(
                "Common.ThingCategory",
                c => new
                    {
                        Index = c.Int(nullable: false),
                        ThingCategoryID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Parent = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ThingCategoryID)
                .Index(t => t.Index, unique: true, name: "IX_ThingCategory");
            
            CreateTable(
                "Common.ThingCategoryDefaultAccount",
                c => new
                    {
                        ThingCategoryDefaultAccountID = c.Int(nullable: false, identity: true),
                        ThingCategoryID = c.Int(nullable: false),
                        ControlAccountID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ThingCategoryDefaultAccountID);
            
            CreateTable(
                "CRM.TicketActivityDefinition",
                c => new
                    {
                        TicketActivityDefinitionID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Index = c.Int(nullable: false),
                        CanFinalize = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TicketActivityDefinitionID);
            
            CreateTable(
                "CRM.TicketAssignment",
                c => new
                    {
                        TicketAssignmentID = c.Int(nullable: false, identity: true),
                        TicketID = c.String(),
                        TicketActivityDefinitionID = c.String(),
                        AssignedBy = c.String(),
                        ProcessedBy = c.String(),
                        AssignmentDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TicketAssignmentID);
            
            CreateTable(
                "CRM.TicketRole",
                c => new
                    {
                        TicketRoleID = c.Int(nullable: false, identity: true),
                        TicketActivityDefinitionID = c.String(),
                        RoleID = c.String(),
                        TicketGroup = c.String(),
                        NeedsPassKey = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TicketRoleID);
            
            CreateTable(
                "EXCEED.TimeSheet",
                c => new
                    {
                        TimeSheetID = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        Date = c.DateTime(),
                        RentalRate = c.Int(),
                        TypeofWork = c.String(nullable: false),
                        Location = c.String(nullable: false),
                        KMHrReading = c.Int(nullable: false),
                        WorkingHoursMorningFrom = c.DateTime(nullable: false),
                        WorkingHoursMorningto = c.DateTime(nullable: false),
                        WorkingHoursAfternoonFrom = c.DateTime(nullable: false),
                        WorkingHoursAfternoonTo = c.DateTime(nullable: false),
                        IdleTime = c.Int(),
                        DownTime = c.Int(),
                        CustomerName = c.String(nullable: false),
                        EquipmentAdministratorName = c.String(nullable: false),
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
                .PrimaryKey(t => t.TimeSheetID)
                .ForeignKey("EXCEED.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "EXCEED.TireIssueApproval",
                c => new
                    {
                        TireIssueApprovalID = c.Int(nullable: false, identity: true),
                        TireIssueId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TireIssueApprovalID)
                .ForeignKey("EXCEED.TireIssue", t => t.TireIssueId, cascadeDelete: true)
                .Index(t => t.TireIssueId);
            
            CreateTable(
                "EXCEED.TireReturnApproval",
                c => new
                    {
                        TireReturnApprovalID = c.Int(nullable: false, identity: true),
                        TireReturnId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TireReturnApprovalID)
                .ForeignKey("EXCEED.TireReturn", t => t.TireReturnId, cascadeDelete: true)
                .Index(t => t.TireReturnId);
            
            CreateTable(
                "common.TransactionReference",
                c => new
                    {
                        TransactionReferenceID = c.Int(nullable: false, identity: true),
                        ReferenceType = c.String(),
                        Referring = c.String(),
                        Referenced = c.String(),
                        Value = c.Double(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TransactionReferenceID);
            
            CreateTable(
                "common.User",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Person = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "common.ValueFactorDefinition",
                c => new
                    {
                        ValueFactorDefinitionID = c.Int(nullable: false, identity: true),
                        ValueFactorDefinitionType = c.String(),
                        Description = c.String(),
                        IsPercent = c.Boolean(nullable: false),
                        Value = c.Double(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ValueFactorDefinitionID);
            
            CreateTable(
                "common.ValueFactor",
                c => new
                    {
                        ValueFactorID = c.Int(nullable: false, identity: true),
                        ReferenceType = c.String(),
                        Reference = c.String(),
                        ValueFactorDefinition = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ValueFactorID);
            
            CreateTable(
                "EXCEED.Variety",
                c => new
                    {
                        VarietyID = c.Int(nullable: false, identity: true),
                        VarietyName = c.String(nullable: false),
                        ProductName = c.String(nullable: false),
                        Class = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VarietyID);
            
            CreateTable(
                "EXCEED.VehicleDemand",
                c => new
                    {
                        VehicleDemandId = c.Guid(nullable: false),
                        RequestNumber = c.Int(),
                        RequestQuantity = c.Int(),
                        FromBranchName = c.String(nullable: false),
                        FromStructureName = c.String(nullable: false),
                        RequestingBranchName = c.String(),
                        RequestingStructureName = c.String(),
                        StartingDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        EquipmentStatus = c.String(),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
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
                .PrimaryKey(t => t.VehicleDemandId);
            
            CreateTable(
                "EXCEED.VehicleDemandApproval",
                c => new
                    {
                        VehicleDemandApprovalID = c.Int(nullable: false, identity: true),
                        VehicleDemandId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehicleDemandApprovalID)
                .ForeignKey("EXCEED.VehicleDemand", t => t.VehicleDemandId, cascadeDelete: true)
                .Index(t => t.VehicleDemandId);
            
            CreateTable(
                "EXCEED.VehicleDistribution",
                c => new
                    {
                        VehicleDistributionID = c.Guid(nullable: false),
                        RequestID = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        StartingDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        FromBranchName = c.String(nullable: false),
                        FromStructureName = c.String(nullable: false),
                        RequestingBranchName = c.String(),
                        RequestingStructureName = c.String(),
                        Referance = c.String(),
                        RequestNumber = c.Int(),
                        DistrubtionNumber = c.Int(),
                        AssignedQuantity = c.Int(),
                        EquipmentStatus = c.String(),
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
                        RentRequest_RentRequestID = c.Guid(),
                    })
                .PrimaryKey(t => t.VehicleDistributionID)
                .ForeignKey("EXCEED.RentRequest", t => t.RentRequest_RentRequestID)
                .Index(t => t.RentRequest_RentRequestID);
            
            CreateTable(
                "EXCEED.VehicleDistributionApproval",
                c => new
                    {
                        VehicleDistributionApprovalID = c.Int(nullable: false, identity: true),
                        VehicleDistributionId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehicleDistributionApprovalID)
                .ForeignKey("EXCEED.VehicleDistribution", t => t.VehicleDistributionId, cascadeDelete: true)
                .Index(t => t.VehicleDistributionId);
            
            CreateTable(
                "EXCEED.VehicleReturn",
                c => new
                    {
                        VehicleReturnID = c.Guid(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        TotalTimeElapsed = c.String(nullable: false),
                        ReturnedBy = c.String(nullable: false),
                        ReasonforReturn = c.String(nullable: false),
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
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehicleReturnID)
                .ForeignKey("EXCEED.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "EXCEED.VehicleReturnApproval",
                c => new
                    {
                        VehicleReturnApprovalID = c.Int(nullable: false, identity: true),
                        VehicleReturnId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehicleReturnApprovalID)
                .ForeignKey("EXCEED.VehicleReturn", t => t.VehicleReturnId, cascadeDelete: true)
                .Index(t => t.VehicleReturnId);
            
            CreateTable(
                "common.VoucherExtension",
                c => new
                    {
                        VoucherExtensionID = c.Int(nullable: false, identity: true),
                        VoucherDefinition = c.String(),
                        Description = c.String(),
                        Index = c.Double(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VoucherExtensionID);
            
            CreateTable(
                "common.VoucherExtensionTransaction",
                c => new
                    {
                        VoucherExtensionTransactionID = c.Int(nullable: false, identity: true),
                        Voucher = c.String(),
                        VoucherExtension = c.String(nullable: false),
                        Number = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VoucherExtensionTransactionID);
            
            CreateTable(
                "common.VoucherTerm",
                c => new
                    {
                        VoucherTermID = c.Int(nullable: false, identity: true),
                        VoucherDefinitionID = c.Int(nullable: false),
                        Voucher = c.String(),
                        TermID = c.Int(nullable: false),
                        Value = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VoucherTermID)
                .ForeignKey("common.Term", t => t.TermID, cascadeDelete: true)
                .Index(t => t.TermID);
            
            CreateTable(
                "common.VoucherValue",
                c => new
                    {
                        VoucherValueID = c.Int(nullable: false, identity: true),
                        VoucherType = c.String(),
                        Voucher = c.String(),
                        Discount = c.Double(nullable: false),
                        SubTotal = c.Double(nullable: false),
                        AdditionalCharge = c.Double(nullable: false),
                        Remark = c.Double(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VoucherValueID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("common.VoucherTerm", "TermID", "common.Term");
            DropForeignKey("EXCEED.VehicleReturnApproval", "VehicleReturnId", "EXCEED.VehicleReturn");
            DropForeignKey("EXCEED.VehicleReturn", "FleetsID", "EXCEED.Fleets");
            DropForeignKey("EXCEED.VehicleDistributionApproval", "VehicleDistributionId", "EXCEED.VehicleDistribution");
            DropForeignKey("EXCEED.VehicleDistribution", "RentRequest_RentRequestID", "EXCEED.RentRequest");
            DropForeignKey("EXCEED.VehicleDemandApproval", "VehicleDemandId", "EXCEED.VehicleDemand");
            DropForeignKey("EXCEED.TireReturnApproval", "TireReturnId", "EXCEED.TireReturn");
            DropForeignKey("EXCEED.TireIssueApproval", "TireIssueId", "EXCEED.TireIssue");
            DropForeignKey("EXCEED.TimeSheet", "FleetsID", "EXCEED.Fleets");
            DropForeignKey("common.Term", "VoucherDefinitionID", "common.VoucherDefinition");
            DropForeignKey("common.Term", "LookupID", "common.Lookup");
            DropForeignKey("EXCEED.SiteVehicleApproval", "SitesVehicleTransferId", "EXCEED.SitesVehicleTransfer");
            DropForeignKey("EXCEED.SitesVehicleTransfer", "FleetsID", "EXCEED.Fleets");
            DropForeignKey("EXCEED.ServiceSpare", "SparePartID", "EXCEED.SparePart");
            DropForeignKey("EXCEED.ServiceSpare", "ServiceSpare_ServiceSpareID", "EXCEED.ServiceSpare");
            DropForeignKey("EXCEED.SalvageStoreApproval", "SalvageStoreId", "EXCEED.SalvageStore");
            DropForeignKey("EXCEED.SalvageStore", "SparePartID", "EXCEED.SparePart");
            DropForeignKey("EXCEED.SalvageStore", "FleetsID", "EXCEED.Fleets");
            DropForeignKey("EXCEED.RentRequestApproval", "RentRequestId", "EXCEED.RentRequest");
            DropForeignKey("EXCEED.RentOtherSupplierApproval", "RentOtherSupplier_RentOtherSuppliersID", "EXCEED.RentOtherSuppliers");
            DropForeignKey("EXCEED.RentFromExternalApproval", "RentFromExternalId", "EXCEED.RentFromExternal");
            DropForeignKey("EXCEED.RentForInternalApproval", "RentInternalProjectId", "EXCEED.RentInternalProject");
            DropForeignKey("EXCEED.RentCustomerFeedBack", "RequestID", "EXCEED.RentRequest");
            DropForeignKey("EXCEED.PlanAndEvaluationApproval", "EquipmentPlanandEvaluationsId", "EXCEED.EquipmentPlanandEvaluations");
            DropForeignKey("EXCEED.OutsourceMaintenanceApproval", "OutsourceMaintenanceId", "EXCEED.OutsourceMaintenance");
            DropForeignKey("EXCEED.OutsourceMaintenance", "Fleet_FleetsID", "EXCEED.Fleets");
            DropForeignKey("EXCEED.OtherCompanyMaintenaceApproval", "OtherCompanyMaintenace_OtherCompanyMaintenanceID", "EXCEED.OtherCompanyMaintenance");
            DropForeignKey("EXCEED.ServiceCrew", "MechanicCrewID", "EXCEED.MechanicCrew");
            DropForeignKey("EXCEED.ServiceTool", "MaintenanceToolID", "EXCEED.MaintenanceTool");
            DropForeignKey("EXCEED.JobOrderDetail", "ServiceID", "EXCEED.Service");
            DropForeignKey("EXCEED.Service", "PlannerID", "EXCEED.Planner");
            DropForeignKey("EXCEED.JobOrderDetail", "JobOrderID", "EXCEED.JobOrder");
            DropForeignKey("EXCEED.JobOrder", "FleetsID", "EXCEED.Fleets");
            DropForeignKey("EXCEED.GoodRecivingNoteApproval", "SalvageStoreGoodReceivingId", "EXCEED.SalvageStoreGoodReceiving");
            DropForeignKey("EXCEED.SalvageStoreGoodReceiving", "SparePartID", "EXCEED.SparePart");
            DropForeignKey("EXCEED.SalvageStoreGoodReceiving", "FleetsID", "EXCEED.Fleets");
            DropForeignKey("EXCEED.FuelIssueApproval", "Fuel_FuelID", "EXCEED.Fuel");
            DropForeignKey("EXCEED.FleetHistory", "FleetsID", "EXCEED.Fleets");
            DropForeignKey("EXCEED.DriverTransferApproval", "DriverVehicleTransferId", "EXCEED.DriverVehicleTransfer");
            DropForeignKey("EXCEED.CRMHoliday", "PeriodID", "common.Period");
            DropForeignKey("EXCEED.CRMHoliday", "HolidayDefinitionID", "EXCEED.HolidayDefinition");
            DropForeignKey("EXCEED.CannibalizationApproval", "CannibalizationId", "EXCEED.Cannibalization");
            DropForeignKey("EXCEED.AssignEquipmentApproval", "AssignedEquipment_AssignedEquipmentID", "EXCEED.AssignedEquipment");
            DropForeignKey("EXCEED.AnnualInventoryInspectionApproval", "AnnualEquipmentInventoryInspectionId", "EXCEED.AnnualEquipmentInventoryInspection");
            DropForeignKey("EXCEED.Accident", "FleetsID", "EXCEED.Fleets");
            DropIndex("common.VoucherTerm", new[] { "TermID" });
            DropIndex("EXCEED.VehicleReturnApproval", new[] { "VehicleReturnId" });
            DropIndex("EXCEED.VehicleReturn", new[] { "FleetsID" });
            DropIndex("EXCEED.VehicleDistributionApproval", new[] { "VehicleDistributionId" });
            DropIndex("EXCEED.VehicleDistribution", new[] { "RentRequest_RentRequestID" });
            DropIndex("EXCEED.VehicleDemandApproval", new[] { "VehicleDemandId" });
            DropIndex("EXCEED.TireReturnApproval", new[] { "TireReturnId" });
            DropIndex("EXCEED.TireIssueApproval", new[] { "TireIssueId" });
            DropIndex("EXCEED.TimeSheet", new[] { "FleetsID" });
            DropIndex("Common.ThingCategory", "IX_ThingCategory");
            DropIndex("common.Term", new[] { "LookupID" });
            DropIndex("common.Term", new[] { "VoucherDefinitionID" });
            DropIndex("Common.Specification", "IX_SpecificationIndex");
            DropIndex("EXCEED.SiteVehicleApproval", new[] { "SitesVehicleTransferId" });
            DropIndex("EXCEED.SitesVehicleTransfer", new[] { "FleetsID" });
            DropIndex("EXCEED.ServiceSpare", new[] { "ServiceSpare_ServiceSpareID" });
            DropIndex("EXCEED.ServiceSpare", new[] { "SparePartID" });
            DropIndex("EXCEED.SalvageStoreApproval", new[] { "SalvageStoreId" });
            DropIndex("EXCEED.SalvageStore", new[] { "SparePartID" });
            DropIndex("EXCEED.SalvageStore", new[] { "FleetsID" });
            DropIndex("EXCEED.RentRequestApproval", new[] { "RentRequestId" });
            DropIndex("EXCEED.RentOtherSupplierApproval", new[] { "RentOtherSupplier_RentOtherSuppliersID" });
            DropIndex("EXCEED.RentFromExternalApproval", new[] { "RentFromExternalId" });
            DropIndex("EXCEED.RentForInternalApproval", new[] { "RentInternalProjectId" });
            DropIndex("EXCEED.RentCustomerFeedBack", new[] { "RequestID" });
            DropIndex("common.Price", "IX_Priority");
            DropIndex("EXCEED.PlanAndEvaluationApproval", new[] { "EquipmentPlanandEvaluationsId" });
            DropIndex("EXCEED.OutsourceMaintenanceApproval", new[] { "OutsourceMaintenanceId" });
            DropIndex("EXCEED.OutsourceMaintenance", new[] { "Fleet_FleetsID" });
            DropIndex("EXCEED.OtherCompanyMaintenaceApproval", new[] { "OtherCompanyMaintenace_OtherCompanyMaintenanceID" });
            DropIndex("EXCEED.ServiceCrew", new[] { "MechanicCrewID" });
            DropIndex("EXCEED.ServiceTool", new[] { "MaintenanceToolID" });
            DropIndex("EXCEED.Service", new[] { "PlannerID" });
            DropIndex("EXCEED.JobOrderDetail", new[] { "ServiceID" });
            DropIndex("EXCEED.JobOrderDetail", new[] { "JobOrderID" });
            DropIndex("EXCEED.JobOrder", new[] { "FleetsID" });
            DropIndex("EXCEED.SalvageStoreGoodReceiving", new[] { "SparePartID" });
            DropIndex("EXCEED.SalvageStoreGoodReceiving", new[] { "FleetsID" });
            DropIndex("EXCEED.GoodRecivingNoteApproval", new[] { "SalvageStoreGoodReceivingId" });
            DropIndex("EXCEED.FuelIssueApproval", new[] { "Fuel_FuelID" });
            DropIndex("EXCEED.FleetHistory", new[] { "FleetsID" });
            DropIndex("EXCEED.DriverTransferApproval", new[] { "DriverVehicleTransferId" });
            DropIndex("EXCEED.CRMHoliday", new[] { "PeriodID" });
            DropIndex("EXCEED.CRMHoliday", new[] { "HolidayDefinitionID" });
            DropIndex("EXCEED.CannibalizationApproval", new[] { "CannibalizationId" });
            DropIndex("EXCEED.AssignEquipmentApproval", new[] { "AssignedEquipment_AssignedEquipmentID" });
            DropIndex("EXCEED.AnnualInventoryInspectionApproval", new[] { "AnnualEquipmentInventoryInspectionId" });
            DropIndex("EXCEED.Accident", new[] { "FleetsID" });
            DropTable("common.VoucherValue");
            DropTable("common.VoucherTerm");
            DropTable("common.VoucherExtensionTransaction");
            DropTable("common.VoucherExtension");
            DropTable("EXCEED.VehicleReturnApproval");
            DropTable("EXCEED.VehicleReturn");
            DropTable("EXCEED.VehicleDistributionApproval");
            DropTable("EXCEED.VehicleDistribution");
            DropTable("EXCEED.VehicleDemandApproval");
            DropTable("EXCEED.VehicleDemand");
            DropTable("EXCEED.Variety");
            DropTable("common.ValueFactor");
            DropTable("common.ValueFactorDefinition");
            DropTable("common.User");
            DropTable("common.TransactionReference");
            DropTable("EXCEED.TireReturnApproval");
            DropTable("EXCEED.TireIssueApproval");
            DropTable("EXCEED.TimeSheet");
            DropTable("CRM.TicketRole");
            DropTable("CRM.TicketAssignment");
            DropTable("CRM.TicketActivityDefinition");
            DropTable("Common.ThingCategoryDefaultAccount");
            DropTable("Common.ThingCategory");
            DropTable("common.VoucherDefinition");
            DropTable("common.Term");
            DropTable("EXCEED.TechnicalJobApproval");
            DropTable("common.TaxTransaction");
            DropTable("common.Tax");
            DropTable("EXCEED.SubCountry");
            DropTable("common.StoreTransaction");
            DropTable("Common.Store");
            DropTable("common.StockLevel");
            DropTable("common.StockBalance");
            DropTable("Common.Specification");
            DropTable("EXCEED.SiteVehicleApproval");
            DropTable("EXCEED.SitesVehicleTransfer");
            DropTable("EXCEED.ServiceSpare");
            DropTable("common.SerialDefinition");
            DropTable("common.SerialCode");
            DropTable("EXCEED.SalvageStoreApproval");
            DropTable("EXCEED.SalvageStore");
            DropTable("EXCEED.SalesQuoteVoucherView");
            DropTable("common.SalesQuoteVoucher");
            DropTable("Common.SalesOrderVoucher");
            DropTable("Common.Role");
            DropTable("EXCEED.RepairType");
            DropTable("EXCEED.RentRequestApproval");
            DropTable("EXCEED.RentOtherSuppliers");
            DropTable("EXCEED.RentOtherSupplierApproval");
            DropTable("EXCEED.RentFromExternalApproval");
            DropTable("EXCEED.RentFromExternal");
            DropTable("EXCEED.RentInternalProject");
            DropTable("EXCEED.RentForInternalApproval");
            DropTable("EXCEED.RentRequest");
            DropTable("EXCEED.RentCustomerFeedBack");
            DropTable("common.Religion");
            DropTable("EXCEED.ReceiveMethod");
            DropTable("Common.Product");
            DropTable("common.Price");
            DropTable("EXCEED.PreCustomerView");
            DropTable("EXCEED.PreCustomerSchedule");
            DropTable("EXCEED.PreCustomer");
            DropTable("EXCEED.PlanAndEvaluationApproval");
            DropTable("Common.PersonCategory");
            DropTable("EXCEED.OutsourceMaintenanceApproval");
            DropTable("EXCEED.OutsourceMaintenance");
            DropTable("EXCEED.OtherCompanyMaintenance");
            DropTable("EXCEED.OtherCompanyMaintenaceApproval");
            DropTable("Common.OrganizationCustomer");
            DropTable("Common.OrganizationCategory");
            DropTable("EXCEED.OperatorPosition");
            DropTable("common.ObjectState");
            DropTable("Common.ObjectStateDefinition");
            DropTable("Common.ObjectAccountRequirement");
            DropTable("Common.NonCashTransaction");
            DropTable("EXCEED.ServiceCrew");
            DropTable("EXCEED.MechanicCrew");
            DropTable("EXCEED.Manufacturer");
            DropTable("EXCEED.ServiceTool");
            DropTable("EXCEED.MaintenanceTool");
            DropTable("EXCEED.MaintenanceCenter");
            DropTable("EXCEED.MailModel");
            DropTable("EXCEED.LineItem");
            DropTable("common.LifeTime");
            DropTable("Common.LifeSpan");
            DropTable("common.JournalTemplate");
            DropTable("common.JournalDetail");
            DropTable("EXCEED.Planner");
            DropTable("EXCEED.Service");
            DropTable("EXCEED.JobOrderDetail");
            DropTable("EXCEED.JobOrder");
            DropTable("EXCEED.SparePart");
            DropTable("EXCEED.SalvageStoreGoodReceiving");
            DropTable("EXCEED.GoodRecivingNoteApproval");
            DropTable("common.GlobalTypeTax");
            DropTable("common.GlobalTypeList");
            DropTable("EXCEED.FuelIssueApproval");
            DropTable("EXCEED.FollowUp");
            DropTable("EXCEED.FleetHistory");
            DropTable("EXCEED.EquipmentName");
            DropTable("EXCEED.Educationallevel");
            DropTable("EXCEED.DriverTransferApproval");
            DropTable("EXCEED.DriverVehicleTransfer");
            DropTable("EXCEED.DefaultLedgerAccount");
            DropTable("EXCEED.CustomerTicketView");
            DropTable("CRM.CustomerTicket");
            DropTable("EXCEED.CustomerHandlingTeam");
            DropTable("common.CurrencyPreference");
            DropTable("common.Currency");
            DropTable("EXCEED.HolidayDefinition");
            DropTable("EXCEED.CRMHoliday");
            DropTable("EXCEED.CreditSetting");
            DropTable("Common.CreditSalesInvoice");
            DropTable("common.ControlAccount");
            DropTable("common.CommunicationSource");
            DropTable("Sales.CashSalesInvoice");
            DropTable("common.CartTransaction");
            DropTable("Common.Cart");
            DropTable("EXCEED.CannibalizationApproval");
            DropTable("EXCEED.Cannibalization");
            DropTable("CRM.Campaign");
            DropTable("EXCEED.CampaignProduct");
            DropTable("EXCEED.BinCardLineReserve");
            DropTable("common.Beginning");
            DropTable("common.BankAccountDetail");
            DropTable("EXCEED.AssignEquipmentApproval");
            DropTable("EXCEED.AssignedEquipment");
            DropTable("EXCEED.AnnualInventoryInspectionApproval");
            DropTable("EXCEED.AdministrativeJobApproval");
            DropTable("common.Activity");
            DropTable("common.ActivityDefinition");
            DropTable("common.Account");
            DropTable("common.AccountRequirement");
            DropTable("common.AccountMap");
            DropTable("EXCEED.Accident");
        }
    }
}
