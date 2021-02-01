namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pr : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ParticipatingSupplier", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ParticipatingSupplier", "Quantity");
        }
    }
}
