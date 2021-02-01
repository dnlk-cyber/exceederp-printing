namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction29 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.Task", "EstimatedDuration", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "ActualDuration", c => c.Int(nullable: false));
            AddColumn("Construction.Task", "BlockId", c => c.Int(nullable: false));
            AddColumn("Construction.Dependency", "LagTime", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Construction.Dependency", "LagTime");
            DropColumn("Construction.Task", "BlockId");
            DropColumn("Construction.Task", "ActualDuration");
            DropColumn("Construction.Task", "EstimatedDuration");
        }
    }
}
