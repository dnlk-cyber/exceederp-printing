namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class outofstockproforma : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ParticipatingSupplier", "IsOutOfStock", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ParticipatingSupplier", "IsOutOfStock");
        }
    }
}
