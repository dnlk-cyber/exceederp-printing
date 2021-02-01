namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supplier2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProcurementSupplier", "IsForShortList", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.SupplierPerformance", "Difference", c => c.Int(nullable: false));
            AlterColumn("EXCEED.SupplierPerformance", "PlannedDeliveryPeriod", c => c.Int(nullable: false));
            AlterColumn("EXCEED.SupplierPerformance", "ActualDeliveryPeriod", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.SupplierPerformance", "ActualDeliveryPeriod", c => c.String(nullable: false));
            AlterColumn("EXCEED.SupplierPerformance", "PlannedDeliveryPeriod", c => c.String(nullable: false));
            DropColumn("EXCEED.SupplierPerformance", "Difference");
            DropColumn("EXCEED.ProcurementSupplier", "IsForShortList");
        }
    }
}
