namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm46 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.SickLeaveSetting",
                c => new
                    {
                        SickLeaveSettingId = c.Int(nullable: false, identity: true),
                        CurrentValue = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.SickLeaveSettingId);
            
        }
        
        public override void Down()
        {
            DropTable("HRM.SickLeaveSetting");
        }
    }
}
