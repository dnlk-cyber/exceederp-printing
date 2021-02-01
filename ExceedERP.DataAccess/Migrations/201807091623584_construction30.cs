namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction30 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.PEVCostSetting", "ProjectInformationId", c => c.Int(nullable: false));
            AlterColumn("Construction.Task", "ActualDuration", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("Construction.Task", "ActualDuration", c => c.Int(nullable: false));
            DropColumn("Construction.PEVCostSetting", "ProjectInformationId");
        }
    }
}
