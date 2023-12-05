namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetupgradebe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FixedAssetReturnDepartmentValidation",
                c => new
                    {
                        FixedAssetReturnDepartmentValidationId = c.Int(nullable: false, identity: true),
                        FixedAssetReturnId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetReturnDepartmentValidationId)
                .ForeignKey("EXCEED.FixedAssetReturn", t => t.FixedAssetReturnId, cascadeDelete: true)
                .Index(t => t.FixedAssetReturnId);
            
            AddColumn("EXCEED.AssetRegistration", "Disable", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetReturn", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetReturn", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetReturn", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetReturn", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetReturn", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetReturn", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.FixedAssetReturn", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetReturn", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetReturn", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.FixedAssetReturn", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetReturn", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.FixedAssetReturn", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.FixedAssetReturn", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetReturn", "CurrentApprovalStep", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.FixedAssetReturnDepartmentValidation", "FixedAssetReturnId", "EXCEED.FixedAssetReturn");
            DropIndex("EXCEED.FixedAssetReturnDepartmentValidation", new[] { "FixedAssetReturnId" });
            DropColumn("EXCEED.FixedAssetReturn", "CurrentApprovalStep");
            DropColumn("EXCEED.FixedAssetReturn", "IsFinalApproval");
            DropColumn("EXCEED.FixedAssetReturn", "ForwardedTo");
            DropColumn("EXCEED.FixedAssetReturn", "ForwardedFrom");
            DropColumn("EXCEED.FixedAssetReturn", "DepartmentSendForApprovalTime");
            DropColumn("EXCEED.FixedAssetReturn", "DepartmentSendForApprovalBy");
            DropColumn("EXCEED.FixedAssetReturn", "IsDepartmentSendForApproval");
            DropColumn("EXCEED.FixedAssetReturn", "DepartmentOnlineApprovedTime");
            DropColumn("EXCEED.FixedAssetReturn", "DepartmentOnlineApprovedBy");
            DropColumn("EXCEED.FixedAssetReturn", "IsDepartmentOnlineApproved");
            DropColumn("EXCEED.FixedAssetReturn", "CreatedAtOrgStructureId");
            DropColumn("EXCEED.FixedAssetReturn", "CreatedAtBusinessUnitId");
            DropColumn("EXCEED.FixedAssetReturn", "CreatedAtBranchId");
            DropColumn("EXCEED.FixedAssetReturn", "BusinessUnitId");
            DropColumn("EXCEED.AssetRegistration", "Disable");
            DropTable("EXCEED.FixedAssetReturnDepartmentValidation");
        }
    }
}
