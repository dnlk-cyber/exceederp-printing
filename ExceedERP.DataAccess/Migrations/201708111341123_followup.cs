namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class followup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PurchaseFollowUpStatus",
                c => new
                    {
                        PurchaseFollowUpStatusId = c.Int(nullable: false, identity: true),
                        PurchaseFollowUpID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseFollowUpStatusId)
                .ForeignKey("EXCEED.PurchaseFollowUp", t => t.PurchaseFollowUpID, cascadeDelete: true)
                .Index(t => t.PurchaseFollowUpID);
            
            CreateTable(
                "EXCEED.PurchaseFollowUpSystemStatus",
                c => new
                    {
                        PurchaseFollowUpSystemStatusId = c.Int(nullable: false, identity: true),
                        PurchaseFollowUpID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        User = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseFollowUpSystemStatusId)
                .ForeignKey("EXCEED.PurchaseFollowUp", t => t.PurchaseFollowUpID, cascadeDelete: true)
                .Index(t => t.PurchaseFollowUpID);
            
            AddColumn("EXCEED.PurchaseFollowUp", "PurchaseRequisitionID", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUp", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUp", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUp", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUp", "ReferenceNo", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUp", "PurchaseCategory", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUp", "PurchaseStatus", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Lot", "PurchaseRequisitionID", c => c.Int(nullable: false));
            AlterColumn("EXCEED.TenderValidation", "Status", c => c.Int(nullable: false));
            AlterColumn("EXCEED.PurchaseFollowUp", "ProcurementOfficer", c => c.Int(nullable: false));
            CreateIndex("EXCEED.PurchaseFollowUp", "PurchaseRequisitionID");
            AddForeignKey("EXCEED.PurchaseFollowUp", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition", "PurchaseRequisitionID", cascadeDelete: true);
            DropColumn("EXCEED.PurchaseFollowUp", "Division");
            DropColumn("EXCEED.PurchaseFollowUp", "Department");
            DropColumn("EXCEED.PurchaseFollowUp", "Section");
            DropColumn("EXCEED.PurchaseFollowUp", "ItemCategory");
            DropColumn("EXCEED.PurchaseFollowUp", "InventoryItem");
            DropColumn("EXCEED.PurchaseFollowUp", "PRQuantity");
            DropColumn("EXCEED.PurchaseFollowUp", "PRNo");
            DropColumn("EXCEED.PurchaseFollowUp", "ReceiveDate");
            DropColumn("EXCEED.PurchaseFollowUp", "PurchaseDate");
            DropColumn("EXCEED.PurchaseFollowUp", "GRNQuantity");
            DropColumn("EXCEED.PurchaseFollowUp", "RemainingQuantity");
            DropColumn("EXCEED.PurchaseFollowUp", "Status");
            DropColumn("EXCEED.PurchaseFollowUp", "Remark");
            DropColumn("EXCEED.PurchaseFollowUp", "isOnlineApprovedUser");
            DropColumn("EXCEED.PurchaseFollowUp", "OnlineApprovedByUser");
            DropColumn("EXCEED.PurchaseFollowUp", "OnlineApprovedTimeUser");
            DropColumn("EXCEED.PurchaseFollowUp", "isVoid");
            DropColumn("EXCEED.PurchaseFollowUp", "VoidBy");
            DropColumn("EXCEED.PurchaseFollowUp", "VoidTime");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PurchaseFollowUp", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseFollowUp", "VoidBy", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUp", "isVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUp", "OnlineApprovedTimeUser", c => c.DateTime());
            AddColumn("EXCEED.PurchaseFollowUp", "OnlineApprovedByUser", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUp", "isOnlineApprovedUser", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUp", "Remark", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUp", "Status", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUp", "RemainingQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PurchaseFollowUp", "GRNQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PurchaseFollowUp", "PurchaseDate", c => c.DateTime());
            AddColumn("EXCEED.PurchaseFollowUp", "ReceiveDate", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUp", "PRNo", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUp", "PRQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PurchaseFollowUp", "InventoryItem", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUp", "ItemCategory", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUp", "Section", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUp", "Department", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUp", "Division", c => c.String());
            DropForeignKey("EXCEED.PurchaseFollowUp", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropForeignKey("EXCEED.PurchaseFollowUpSystemStatus", "PurchaseFollowUpID", "EXCEED.PurchaseFollowUp");
            DropForeignKey("EXCEED.PurchaseFollowUpStatus", "PurchaseFollowUpID", "EXCEED.PurchaseFollowUp");
            DropIndex("EXCEED.PurchaseFollowUpSystemStatus", new[] { "PurchaseFollowUpID" });
            DropIndex("EXCEED.PurchaseFollowUpStatus", new[] { "PurchaseFollowUpID" });
            DropIndex("EXCEED.PurchaseFollowUp", new[] { "PurchaseRequisitionID" });
            AlterColumn("EXCEED.PurchaseFollowUp", "ProcurementOfficer", c => c.String());
            AlterColumn("EXCEED.TenderValidation", "Status", c => c.String());
            AlterColumn("EXCEED.Lot", "PurchaseRequisitionID", c => c.String());
            DropColumn("EXCEED.PurchaseFollowUp", "PurchaseStatus");
            DropColumn("EXCEED.PurchaseFollowUp", "PurchaseCategory");
            DropColumn("EXCEED.PurchaseFollowUp", "ReferenceNo");
            DropColumn("EXCEED.PurchaseFollowUp", "EmployeeId");
            DropColumn("EXCEED.PurchaseFollowUp", "OrgStructureId");
            DropColumn("EXCEED.PurchaseFollowUp", "BranchId");
            DropColumn("EXCEED.PurchaseFollowUp", "PurchaseRequisitionID");
            DropTable("EXCEED.PurchaseFollowUpSystemStatus");
            DropTable("EXCEED.PurchaseFollowUpStatus");
        }
    }
}
