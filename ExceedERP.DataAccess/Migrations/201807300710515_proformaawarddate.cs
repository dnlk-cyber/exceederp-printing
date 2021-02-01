namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proformaawarddate : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.WinningSupplier", "AwardDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.WinningSupplier", "AwardDate");
        }
    }
}
