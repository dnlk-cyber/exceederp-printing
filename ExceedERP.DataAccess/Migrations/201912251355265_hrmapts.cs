namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmapts : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Commitment", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugRequests", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugRequests", "StoreCode", c => c.String());
            AddColumn("HRM.ManPowerPlanningRequest", "IsNewJobTitle", c => c.Boolean(nullable: false));
            AddColumn("HRM.ManPowerPlanningRequest", "JobTitle", c => c.String());
            AlterColumn("APTS.DrugReceivalModel19", "SendForApprovalTime", c => c.DateTime());
            AlterColumn("HRM.Delegation", "ToDateTime", c => c.DateTime());
            DropTable("EXCEED.InventorySetting");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.InventorySetting",
                c => new
                    {
                        InventorySettingId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Value = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InventorySettingId);
            
            AlterColumn("HRM.Delegation", "ToDateTime", c => c.DateTime(nullable: false));
            AlterColumn("APTS.DrugReceivalModel19", "SendForApprovalTime", c => c.DateTime(nullable: false));
            DropColumn("HRM.ManPowerPlanningRequest", "JobTitle");
            DropColumn("HRM.ManPowerPlanningRequest", "IsNewJobTitle");
            DropColumn("APTS.DrugRequests", "StoreCode");
            DropColumn("APTS.DrugRequests", "IsOnlineApproved");
            DropColumn("HRM.Commitment", "IsActive");
        }
    }
}
