namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CostingAndFollowUpMerged : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ChangeOrder",
                c => new
                    {
                        ChangeOrderId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Reason = c.String(),
                        Quantity = c.Int(nullable: false),
                        Size = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceNo = c.String(),
                        OrderNo = c.String(),
                        PreparedBy = c.String(),
                        TypeAndQuantityOfMaterialNeeded = c.String(),
                        Checked = c.String(),
                    })
                .PrimaryKey(t => t.ChangeOrderId);
            
            CreateTable(
                "EXCEED.CommertialItems",
                c => new
                    {
                        CommertialItemsId = c.Int(nullable: false, identity: true),
                        Job_Type = c.Int(nullable: false),
                        SubCategory = c.String(maxLength: 50),
                        Description = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.CommertialItemsId);
            
            CreateTable(
                "EXCEED.CostCenter",
                c => new
                    {
                        CostCenterId = c.Int(nullable: false, identity: true),
                        CostCenterName = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CostCenterId);
            
            CreateTable(
                "ProductFollowUp.FinishedGoodReceiving",
                c => new
                    {
                        FinishedGoodReceivingId = c.Int(nullable: false, identity: true),
                        VoucherNo = c.String(),
                        Store = c.String(),
                        JobId = c.Int(nullable: false),
                        DeliveredBy = c.String(),
                        ReceivedBy = c.String(),
                        Supervisor = c.String(),
                    })
                .PrimaryKey(t => t.FinishedGoodReceivingId);
            
            CreateTable(
                "ProductFollowUp.FinishedGoodItems",
                c => new
                    {
                        FinishedGoodsId = c.Int(nullable: false, identity: true),
                        FinishedGoodReceivingId = c.Int(nullable: false),
                        CodeNo = c.String(),
                        Description = c.String(),
                        Unit = c.String(),
                        Quantity = c.Double(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.FinishedGoodsId);
            
            CreateTable(
                "ProductFollowUp.JobOrderMaterial",
                c => new
                    {
                        JobOrderMaterialId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        Gram = c.Double(nullable: false),
                        Size = c.Double(nullable: false),
                        Quantity = c.Double(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                        SIVNo = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.JobOrderMaterialId);
            
            CreateTable(
                "JobCosting.JobOrderSummaryCostCenterDetail",
                c => new
                    {
                        JobOrderSummaryCostCenterDetailId = c.Int(nullable: false, identity: true),
                        JobOrderSummaryCostCenterId = c.Int(nullable: false),
                        CostCenter = c.String(),
                        Date = c.DateTime(nullable: false),
                        DirectLaborTotalHour = c.Double(nullable: false),
                        Rate = c.Double(nullable: false),
                        DirectLaborAmount = c.Double(nullable: false),
                        DirectMachineTotalHour = c.Double(nullable: false),
                        OverHeadRate = c.Double(nullable: false),
                        OverHeadAmount = c.Double(nullable: false),
                        TotalCostOfTheCenter = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.JobOrderSummaryCostCenterDetailId);
            
            CreateTable(
                "JobCosting.JobOrderSummaryCostCenter",
                c => new
                    {
                        JobOrderSummaryCostCenterId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobOrderSummaryCostCenterId);
            
            CreateTable(
                "JobCosting.JobOrderSummary",
                c => new
                    {
                        JobOrderSummaryId = c.Int(nullable: false, identity: true),
                        JobId = c.String(),
                        Product = c.String(),
                        Quantity = c.Int(nullable: false),
                        Page = c.Int(nullable: false),
                        SellingPrice = c.Double(nullable: false),
                        DirectMaterial = c.Double(nullable: false),
                        DirectLabor = c.Double(nullable: false),
                        ManufacturingOverHead = c.Double(nullable: false),
                        SellingAndDistribution = c.Double(nullable: false),
                        AdminAndGeneralExpense = c.Double(nullable: false),
                        TotalCost = c.Double(nullable: false),
                        ProfitOrLoss = c.Double(nullable: false),
                        Percentage = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobOrderSummaryId);
            
            CreateTable(
                "JobCosting.JobOrderTimeRegistration",
                c => new
                    {
                        JobOrderTimeRegistrationId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.JobOrderTimeRegistrationId);
            
            CreateTable(
                "JobCosting.JobOrderTimeSheet",
                c => new
                    {
                        JobOrderTimeSheetId = c.Int(nullable: false, identity: true),
                        JobOrderTimeRegistrationId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Work = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        ComplationTime = c.DateTime(nullable: false),
                        TotalTime = c.DateTime(nullable: false),
                        NoOfEmployee = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.JobOrderTimeSheetId);
            
            CreateTable(
                "ProductFollowUp.Job",
                c => new
                    {
                        JobId = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(nullable: false),
                        ProformaId = c.Int(),
                        JobNo = c.String(),
                        CustomerName = c.String(maxLength: 50),
                        Estimatedby = c.String(),
                        Code = c.String(),
                        Approvedby = c.String(),
                        ContactName = c.String(),
                        ContactPhone = c.String(),
                        City = c.String(),
                        SubCity = c.String(),
                        Wereda = c.String(),
                        Pobox = c.String(),
                        JobTypeId = c.Int(nullable: false),
                        JobTypeItemId = c.Int(),
                        Pages = c.Int(nullable: false),
                        Copies = c.Int(nullable: false),
                        Quant = c.Int(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                        BeforeTax = c.Double(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Contract = c.Boolean(nullable: false),
                        TotalImpression = c.Int(nullable: false),
                        DeliveryDate = c.DateTime(),
                        TextWeight = c.Double(nullable: false),
                        CoverWeight = c.Double(nullable: false),
                        ColorText = c.Int(nullable: false),
                        Colorcover = c.Int(nullable: false),
                        FinishingSize = c.String(maxLength: 15),
                        PrintingLine = c.String(maxLength: 15),
                        PrintingMachine = c.String(maxLength: 15),
                        Sewing = c.Boolean(nullable: false),
                        New = c.Boolean(nullable: false),
                        PerfectBinding = c.Boolean(nullable: false),
                        Repeat = c.Boolean(nullable: false),
                        Lamnating = c.Boolean(nullable: false),
                        Varnish = c.Boolean(nullable: false),
                        Perforate = c.Boolean(nullable: false),
                        Camsur = c.Boolean(nullable: false),
                        GraphicDesignExpectedCompletetion = c.DateTime(),
                        PrintingExpectedCompletetion = c.DateTime(),
                        FinishExpectedCompletetion = c.DateTime(),
                        Status = c.String(maxLength: 25),
                        FinishingWorkflow = c.String(maxLength: 50),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.JobId);
            
            CreateTable(
                "EXCEED.JobTypeItem",
                c => new
                    {
                        JobTypeItemId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.JobTypeItemId);
            
            CreateTable(
                "EXCEED.JobType",
                c => new
                    {
                        JobTypeId = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        GLTaxRateID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobTypeId);
            
            CreateTable(
                "EXCEED.Machine",
                c => new
                    {
                        MachineId = c.Int(nullable: false, identity: true),
                        MachineName = c.String(),
                        Category = c.Int(nullable: false),
                        SubCategory = c.Int(nullable: false),
                        Description = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.MachineId);
            
            CreateTable(
                "EXCEED.PFProformaItems",
                c => new
                    {
                        PFProformaItemsId = c.Int(nullable: false, identity: true),
                        ProformaId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        JobTypeItemId = c.Int(),
                        SubCategory = c.String(),
                        Quantity = c.Int(nullable: false),
                        Page = c.String(),
                        Size = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BeforeTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.PFProformaItemsId);
            
            CreateTable(
                "ProductFollowUp.ProductDelivery",
                c => new
                    {
                        ProductDeliveryId = c.Int(nullable: false, identity: true),
                        JobId = c.Int(nullable: false),
                        CustomerRefNo = c.String(),
                        CreditSaleNo = c.String(),
                        CashSaleNo = c.String(),
                        PreparedBy = c.String(),
                        ApprovedBy = c.String(),
                        IdentificationNo = c.String(),
                    })
                .PrimaryKey(t => t.ProductDeliveryId);
            
            CreateTable(
                "ProductFollowUp.ProductDeliveryItems",
                c => new
                    {
                        ProductDeliveryItemsId = c.Int(nullable: false, identity: true),
                        ProductDeliveryId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        Description = c.String(),
                        Unit = c.String(),
                        Quantity = c.Double(nullable: false),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductDeliveryItemsId);
            
            CreateTable(
                "ProductFollowUp.Proforma",
                c => new
                    {
                        ProformaId = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(),
                        CustomerName = c.String(),
                        ContactName = c.String(maxLength: 50),
                        ContactPhone = c.String(maxLength: 50),
                        ProformaNo = c.String(),
                    })
                .PrimaryKey(t => t.ProformaId);
            
            CreateTable(
                "ProductFollowUp.Graphic",
                c => new
                    {
                        GraphicID = c.Int(nullable: false, identity: true),
                        JobID = c.Int(nullable: false),
                        UserID = c.String(maxLength: 50),
                        Time = c.DateTime(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GraphicID);
            
            CreateTable(
                "ProductFollowUp.Settings",
                c => new
                    {
                        SettingsId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 150),
                        CompanyAddress = c.String(nullable: false, maxLength: 15),
                        CompanyPhone = c.String(nullable: false, maxLength: 15),
                        CompanyEmail = c.String(nullable: false, maxLength: 50),
                        CompanyMail = c.String(nullable: false, maxLength: 50),
                        CompanyVatNo = c.String(nullable: false, maxLength: 50),
                        CompanyTIN = c.String(nullable: false, maxLength: 50),
                        CompanyLogo = c.Binary(nullable: false),
                        CompanyScreen = c.Binary(nullable: false),
                        SMSSenderStatus = c.String(nullable: false, maxLength: 15),
                        EditGrace = c.Int(nullable: false),
                        FiscalYearStart = c.DateTime(nullable: false),
                        DashboardCalendar = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.SettingsId);
            
        }
        
        public override void Down()
        {
            DropTable("ProductFollowUp.Settings");
            DropTable("ProductFollowUp.Graphic");
            DropTable("ProductFollowUp.Proforma");
            DropTable("ProductFollowUp.ProductDeliveryItems");
            DropTable("ProductFollowUp.ProductDelivery");
            DropTable("EXCEED.PFProformaItems");
            DropTable("EXCEED.Machine");
            DropTable("EXCEED.JobType");
            DropTable("EXCEED.JobTypeItem");
            DropTable("ProductFollowUp.Job");
            DropTable("JobCosting.JobOrderTimeSheet");
            DropTable("JobCosting.JobOrderTimeRegistration");
            DropTable("JobCosting.JobOrderSummary");
            DropTable("JobCosting.JobOrderSummaryCostCenter");
            DropTable("JobCosting.JobOrderSummaryCostCenterDetail");
            DropTable("ProductFollowUp.JobOrderMaterial");
            DropTable("ProductFollowUp.FinishedGoodItems");
            DropTable("ProductFollowUp.FinishedGoodReceiving");
            DropTable("EXCEED.CostCenter");
            DropTable("EXCEED.CommertialItems");
            DropTable("EXCEED.ChangeOrder");
        }
    }
}
