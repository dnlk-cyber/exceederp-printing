namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spelling : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemCategory", "Abbreviation", c => c.String());
            AddColumn("EXCEED.ServiceCategory", "Abbreviation", c => c.String());
            DropColumn("EXCEED.ItemCategory", "Abbrevarion");
            DropColumn("EXCEED.ServiceCategory", "Abbrevarion");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ServiceCategory", "Abbrevarion", c => c.String());
            AddColumn("EXCEED.ItemCategory", "Abbrevarion", c => c.String());
            DropColumn("EXCEED.ServiceCategory", "Abbreviation");
            DropColumn("EXCEED.ItemCategory", "Abbreviation");
        }
    }
}
