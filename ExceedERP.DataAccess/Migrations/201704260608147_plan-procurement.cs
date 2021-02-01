namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class planprocurement : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AnnualNeedAssesment", "IsTransfferedToPlan", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.InventoryPlan", "PeriodName", c => c.String());
            AddColumn("EXCEED.InventoryPlan", "Branch", c => c.String());
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "UOM");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "UOM", c => c.String());
            DropColumn("EXCEED.InventoryPlan", "Branch");
            DropColumn("EXCEED.InventoryPlan", "PeriodName");
            DropColumn("EXCEED.AnnualNeedAssesment", "IsTransfferedToPlan");
        }
    }
}
