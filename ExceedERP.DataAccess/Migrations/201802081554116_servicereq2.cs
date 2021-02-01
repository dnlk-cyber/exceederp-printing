namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicereq2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PurchaseRequisitionSecondValidation",
                c => new
                    {
                        PurchaseRequisitionSecondValidationId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseRequisitionSecondValidationId)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID);
            
            AddColumn("EXCEED.PurchaseRequisitionUserDepartmentValidation", "OrgBranchName", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PurchaseRequisitionSecondValidation", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropIndex("EXCEED.PurchaseRequisitionSecondValidation", new[] { "PurchaseRequisitionID" });
            DropColumn("EXCEED.PurchaseRequisitionUserDepartmentValidation", "OrgBranchName");
            DropTable("EXCEED.PurchaseRequisitionSecondValidation");
        }
    }
}
