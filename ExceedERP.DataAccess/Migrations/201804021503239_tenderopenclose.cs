namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenderopenclose : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Tender", "TechnicalOpenigDate", c => c.DateTime());
            AddColumn("EXCEED.Tender", "FinancialOpenigDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Tender", "FinancialOpenigDate");
            DropColumn("EXCEED.Tender", "TechnicalOpenigDate");
        }
    }
}
