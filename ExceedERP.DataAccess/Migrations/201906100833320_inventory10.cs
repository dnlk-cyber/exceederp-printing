namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreItemAssignment", "SafetyPoint", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreItemAssignment", "DangerPoint", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.StoreItemAssignment", "ReOrderPoint", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.StoreItemAssignment", "MinPoint", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.StoreItemAssignment", "MaxPoint", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.StoreItemAssignment", "MaxPoint", c => c.Single(nullable: false));
            AlterColumn("EXCEED.StoreItemAssignment", "MinPoint", c => c.Single(nullable: false));
            AlterColumn("EXCEED.StoreItemAssignment", "ReOrderPoint", c => c.Single(nullable: false));
            DropColumn("EXCEED.StoreItemAssignment", "DangerPoint");
            DropColumn("EXCEED.StoreItemAssignment", "SafetyPoint");
        }
    }
}
