namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProcurementPlanItem", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProcurementPlanItem", "Name");
        }
    }
}
