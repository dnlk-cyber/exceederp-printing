namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class costcentertype : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLChartOfAccountCostCenter", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GLChartOfAccountCostCenter", "Type");
        }
    }
}
