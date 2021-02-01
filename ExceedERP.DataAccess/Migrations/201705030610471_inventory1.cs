namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.StoreRequisition", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.StoreRequisition", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.StoreRequisition", "OrgStructureId", "HRM.MainCompanyStructure");
            DropIndex("EXCEED.StoreRequisition", new[] { "OrgStructureId" });
            DropIndex("EXCEED.StoreRequisition", new[] { "EmployeeId" });
            DropIndex("EXCEED.StoreRequisition", new[] { "StoreCode" });
            AddColumn("EXCEED.IssueItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.IssueItem", "RequestedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.IssueItem", "ApprovedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.IssueItem", "IssuedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisitionItem", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PurchaseRequisitionItem", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionItem", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.PurchaseValidationItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.IssueItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.StoreRequisition", "StoreCode", c => c.String(nullable: false));
            AlterColumn("EXCEED.PurchaseRequisitionItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.PurchaseValidationItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.IssueItem", "ItemCategoryCode");
            CreateIndex("EXCEED.IssueItem", "ItemCode");
            CreateIndex("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode");
            CreateIndex("EXCEED.PurchaseRequisitionItem", "ItemCode");
            CreateIndex("EXCEED.PurchaseValidationItem", "ItemCategoryCode");
            CreateIndex("EXCEED.PurchaseValidationItem", "ItemCode");
            AddForeignKey("EXCEED.IssueItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.IssueItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseRequisitionItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseValidationItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseValidationItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.Issue", "FiscalYear");
            DropColumn("EXCEED.Issue", "Period");
            DropColumn("EXCEED.IssueItem", "UOM");
            DropColumn("EXCEED.IssueItem", "Quantity");
            DropColumn("EXCEED.StoreRequisitionItem", "InventoryItemDesc");
            DropColumn("EXCEED.PurchaseRequisition", "Department");
            DropColumn("EXCEED.PurchaseRequisition", "StoreKeeperName");
            DropColumn("EXCEED.PurchaseRequisitionItem", "UOM");
            DropColumn("EXCEED.PurchaseRequisitionItem", "OrgBranchName");
            DropColumn("EXCEED.PurchaseValidationItem", "Unit");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PurchaseValidationItem", "Unit", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionItem", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionItem", "UOM", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "StoreKeeperName", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "Department", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.IssueItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.IssueItem", "UOM", c => c.String());
            AddColumn("EXCEED.Issue", "Period", c => c.String());
            AddColumn("EXCEED.Issue", "FiscalYear", c => c.String());
            DropForeignKey("EXCEED.PurchaseValidationItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.PurchaseValidationItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.PurchaseRequisitionItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.IssueItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.IssueItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.PurchaseValidationItem", new[] { "ItemCode" });
            DropIndex("EXCEED.PurchaseValidationItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.PurchaseRequisitionItem", new[] { "ItemCode" });
            DropIndex("EXCEED.PurchaseRequisitionItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.IssueItem", new[] { "ItemCode" });
            DropIndex("EXCEED.IssueItem", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.PurchaseValidationItem", "ItemCode", c => c.String());
            AlterColumn("EXCEED.PurchaseRequisitionItem", "ItemCode", c => c.String(nullable: false));
            AlterColumn("EXCEED.StoreRequisition", "StoreCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.IssueItem", "ItemCode", c => c.String(nullable: false));
            DropColumn("EXCEED.PurchaseValidationItem", "ItemCategoryCode");
            DropColumn("EXCEED.PurchaseRequisitionItem", "ItemSpecification");
            DropColumn("EXCEED.PurchaseRequisitionItem", "IsApproved");
            DropColumn("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode");
            DropColumn("EXCEED.PurchaseRequisition", "EmployeeId");
            DropColumn("EXCEED.PurchaseRequisition", "OrgStructureId");
            DropColumn("EXCEED.StoreRequisitionItem", "ItemSpecification");
            DropColumn("EXCEED.IssueItem", "IssuedQuantity");
            DropColumn("EXCEED.IssueItem", "ApprovedQuantity");
            DropColumn("EXCEED.IssueItem", "RequestedQuantity");
            DropColumn("EXCEED.IssueItem", "ItemCategoryCode");
            CreateIndex("EXCEED.StoreRequisition", "StoreCode");
            CreateIndex("EXCEED.StoreRequisition", "EmployeeId");
            CreateIndex("EXCEED.StoreRequisition", "OrgStructureId");
            AddForeignKey("EXCEED.StoreRequisition", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
            AddForeignKey("EXCEED.StoreRequisition", "StoreCode", "EXCEED.InventoryStore", "StoreCode", cascadeDelete: true);
            AddForeignKey("EXCEED.StoreRequisition", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
        }
    }
}
