namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class budgetsource : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.AnnualNeedAssesmentItem", "BudgetSource", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.AnnualNeedAssesmentItem", "BudgetSource", c => c.String());
        }
    }
}
