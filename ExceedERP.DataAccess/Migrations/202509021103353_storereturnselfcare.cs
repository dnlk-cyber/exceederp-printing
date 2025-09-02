namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storereturnselfcare : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.StoreReturnDepartmentValidation",
                c => new
                    {
                        StoreReturnDepartmentValidationID = c.Int(nullable: false, identity: true),
                        StoreReturnID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Approver = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreReturnDepartmentValidationID)
                .ForeignKey("EXCEED.StoreReturn", t => t.StoreReturnID, cascadeDelete: true)
                .Index(t => t.StoreReturnID);
            
            AddColumn("EXCEED.StoreReturn", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.StoreReturn", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreReturn", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.StoreReturn", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.StoreReturn", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreReturn", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.StoreReturn", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.StoreReturn", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.StoreReturn", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.StoreReturn", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreReturn", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.StoreReturnItems", "NoPrice", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.StoreReturnDepartmentValidation", "StoreReturnID", "EXCEED.StoreReturn");
            DropIndex("EXCEED.StoreReturnDepartmentValidation", new[] { "StoreReturnID" });
            DropColumn("EXCEED.StoreReturnItems", "NoPrice");
            DropColumn("EXCEED.StoreReturn", "CurrentApprovalStep");
            DropColumn("EXCEED.StoreReturn", "IsFinalApproval");
            DropColumn("EXCEED.StoreReturn", "ForwardedTo");
            DropColumn("EXCEED.StoreReturn", "ForwardedFrom");
            DropColumn("EXCEED.StoreReturn", "DepartmentSendForApprovalTime");
            DropColumn("EXCEED.StoreReturn", "DepartmentSendForApprovalBy");
            DropColumn("EXCEED.StoreReturn", "IsDepartmentSendForApproval");
            DropColumn("EXCEED.StoreReturn", "DepartmentOnlineApprovedTime");
            DropColumn("EXCEED.StoreReturn", "DepartmentOnlineApprovedBy");
            DropColumn("EXCEED.StoreReturn", "IsDepartmentOnlineApproved");
            DropColumn("EXCEED.StoreReturn", "CreatedAtOrgStructureId");
            DropTable("EXCEED.StoreReturnDepartmentValidation");
        }
    }
}
