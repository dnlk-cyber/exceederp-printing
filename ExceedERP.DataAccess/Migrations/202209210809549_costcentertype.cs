namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class costcentertype : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLChartOfAccountSegment", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GLChartOfAccountSegment", "Type");
        }
    }
}
