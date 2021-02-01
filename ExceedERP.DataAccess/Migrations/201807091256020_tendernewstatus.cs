namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tendernewstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Tender", "IsTechnicalOpened", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Tender", "IsFinancialOpened", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Tender", "IsFinancialOpened");
            DropColumn("EXCEED.Tender", "IsTechnicalOpened");
        }
    }
}
