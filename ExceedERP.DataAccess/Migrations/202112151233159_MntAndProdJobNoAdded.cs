namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MntAndProdJobNoAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.IssueItem", "ProductionJobNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "MaintenanceJobNo", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "ProductionJobNo", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "MaintenanceJobNo", c => c.String());
            AddColumn("EXCEED.StoreReturnItems", "ProductionJobNo", c => c.String());
            AddColumn("EXCEED.StoreReturnItems", "MaintenanceJobNo", c => c.String());
            DropColumn("EXCEED.IssueItem", "JobNo");
            DropColumn("EXCEED.StoreRequisitionItem", "JobNo");
            DropColumn("EXCEED.StoreReturnItems", "JobNo");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.StoreReturnItems", "JobNo", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "JobNo", c => c.String());
            AddColumn("EXCEED.IssueItem", "JobNo", c => c.String());
            DropColumn("EXCEED.StoreReturnItems", "MaintenanceJobNo");
            DropColumn("EXCEED.StoreReturnItems", "ProductionJobNo");
            DropColumn("EXCEED.StoreRequisitionItem", "MaintenanceJobNo");
            DropColumn("EXCEED.StoreRequisitionItem", "ProductionJobNo");
            DropColumn("EXCEED.IssueItem", "MaintenanceJobNo");
            DropColumn("EXCEED.IssueItem", "ProductionJobNo");
        }
    }
}
