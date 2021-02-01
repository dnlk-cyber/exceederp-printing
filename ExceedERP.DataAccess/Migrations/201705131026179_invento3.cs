namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invento3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.IssueFuelDistribution", "IssueFuelID", "EXCEED.IssueFuel");
            DropForeignKey("EXCEED.IssueFuelItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.IssueFuelItem", "IssueFuelID", "EXCEED.IssueFuel");
            DropForeignKey("EXCEED.IssueFuelItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.IssueFuelValidation", "IssueFuelID", "EXCEED.IssueFuel");
            DropForeignKey("EXCEED.IssueFuel", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropIndex("EXCEED.IssueFuelDistribution", new[] { "IssueFuelID" });
            DropIndex("EXCEED.IssueFuel", new[] { "StoreRequisitionID" });
            DropIndex("EXCEED.IssueFuelItem", new[] { "IssueFuelID" });
            DropIndex("EXCEED.IssueFuelItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.IssueFuelItem", new[] { "ItemCode" });
            DropIndex("EXCEED.IssueFuelValidation", new[] { "IssueFuelID" });
            AddColumn("EXCEED.Issue", "Owner", c => c.Int(nullable: false));
            AddColumn("EXCEED.Issue", "PlateNo", c => c.String());
            AddColumn("EXCEED.Issue", "SupplierAccountCode", c => c.String());
            AddColumn("EXCEED.Issue", "HasReading", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Issue", "ReadingKM", c => c.String());
            AddColumn("EXCEED.IssueItem", "AdditionalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.IssueItem", "EmployeeOfFuelCompany", c => c.String());
            AddColumn("EXCEED.IssueItem", "EmployeeWhoGotItRefuelled", c => c.String());
            AddColumn("EXCEED.IssueItem", "Name", c => c.String());
            DropColumn("EXCEED.IssueItem", "IsFixedAsset");
            DropColumn("EXCEED.IssueItem", "IsMachinery");
            DropColumn("EXCEED.IssueItem", "BatchNo");
            DropColumn("EXCEED.IssueItem", "Model");
            DropColumn("EXCEED.IssueItem", "SerialNo");
            DropColumn("EXCEED.IssueItem", "Condition");
            DropColumn("EXCEED.IssueItem", "PlateNo");
            DropColumn("EXCEED.IssueItem", "ChassisNo");
            DropColumn("EXCEED.IssueItem", "EngineNo");
            DropColumn("EXCEED.IssueItem", "EngineModelNo");
            DropColumn("EXCEED.IssueItem", "EngineElectricalNo");
            DropColumn("EXCEED.IssueItem", "EngineElectricalModelNo");
            DropTable("EXCEED.IssueFuelDistribution");
            DropTable("EXCEED.IssueFuel");
            DropTable("EXCEED.IssueFuelItem");
            DropTable("EXCEED.IssueFuelValidation");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.IssueFuelValidation",
                c => new
                    {
                        IssueFuelValidationID = c.Int(nullable: false, identity: true),
                        IssueFuelID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IssueFuelValidationID);
            
            CreateTable(
                "EXCEED.IssueFuelItem",
                c => new
                    {
                        IssueFuelItemID = c.Int(nullable: false, identity: true),
                        IssueFuelID = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        RequestedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLAccountCode = c.String(nullable: false),
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
                .PrimaryKey(t => t.IssueFuelItemID);
            
            CreateTable(
                "EXCEED.IssueFuel",
                c => new
                    {
                        IssueFuelID = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        StoreCode = c.String(nullable: false),
                        Owner = c.Int(nullable: false),
                        PlateNo = c.String(),
                        SupplierAccountCode = c.String(),
                        HasReading = c.Boolean(nullable: false),
                        ReadingKM = c.String(),
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
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IssueFuelID);
            
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
                .PrimaryKey(t => t.IssueFuelDistributionID);
            
            AddColumn("EXCEED.IssueItem", "EngineElectricalModelNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "EngineElectricalNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "EngineModelNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "EngineNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "ChassisNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "PlateNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "Condition", c => c.String());
            AddColumn("EXCEED.IssueItem", "SerialNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "Model", c => c.String());
            AddColumn("EXCEED.IssueItem", "BatchNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "IsMachinery", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.IssueItem", "IsFixedAsset", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.IssueItem", "Name");
            DropColumn("EXCEED.IssueItem", "EmployeeWhoGotItRefuelled");
            DropColumn("EXCEED.IssueItem", "EmployeeOfFuelCompany");
            DropColumn("EXCEED.IssueItem", "AdditionalCost");
            DropColumn("EXCEED.Issue", "ReadingKM");
            DropColumn("EXCEED.Issue", "HasReading");
            DropColumn("EXCEED.Issue", "SupplierAccountCode");
            DropColumn("EXCEED.Issue", "PlateNo");
            DropColumn("EXCEED.Issue", "Owner");
            CreateIndex("EXCEED.IssueFuelValidation", "IssueFuelID");
            CreateIndex("EXCEED.IssueFuelItem", "ItemCode");
            CreateIndex("EXCEED.IssueFuelItem", "ItemCategoryCode");
            CreateIndex("EXCEED.IssueFuelItem", "IssueFuelID");
            CreateIndex("EXCEED.IssueFuel", "StoreRequisitionID");
            CreateIndex("EXCEED.IssueFuelDistribution", "IssueFuelID");
            AddForeignKey("EXCEED.IssueFuel", "StoreRequisitionID", "EXCEED.StoreRequisition", "StoreRequisitionID", cascadeDelete: true);
            AddForeignKey("EXCEED.IssueFuelValidation", "IssueFuelID", "EXCEED.IssueFuel", "IssueFuelID", cascadeDelete: true);
            AddForeignKey("EXCEED.IssueFuelItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.IssueFuelItem", "IssueFuelID", "EXCEED.IssueFuel", "IssueFuelID", cascadeDelete: true);
            AddForeignKey("EXCEED.IssueFuelItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.IssueFuelDistribution", "IssueFuelID", "EXCEED.IssueFuel", "IssueFuelID", cascadeDelete: true);
        }
    }
}
