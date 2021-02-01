namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement19 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.AnnualNeedAssesmentItem", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.AnnualNeedAssesmentItem", "Price", c => c.String());
        }
    }
}
