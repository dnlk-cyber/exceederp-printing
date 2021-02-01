namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class health : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.HealthRankAndValueSetting", "CurrentValue", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.HealthRankAndValueSetting", "CurrentValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
