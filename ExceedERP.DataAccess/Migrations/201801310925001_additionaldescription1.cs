namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionaldescription1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemTransferReceiveDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ItemTransferReceiveDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.ItemTransferReceiveDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.ItemTransferReceiveDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.ItemTransferReceiveDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.ItemTransferReceiveDistribution", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.StockAdjustmentDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.StockAdjustmentDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.StockAdjustmentDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.StockAdjustmentDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.StockAdjustmentDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.StockAdjustmentDistribution", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StockAdjustmentDistribution", "ModifiedBy");
            DropColumn("EXCEED.StockAdjustmentDistribution", "CreatedBy");
            DropColumn("EXCEED.StockAdjustmentDistribution", "LastModified");
            DropColumn("EXCEED.StockAdjustmentDistribution", "DateCreated");
            DropColumn("EXCEED.StockAdjustmentDistribution", "OrgBranchName");
            DropColumn("EXCEED.StockAdjustmentDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ItemTransferReceiveDistribution", "ModifiedBy");
            DropColumn("EXCEED.ItemTransferReceiveDistribution", "CreatedBy");
            DropColumn("EXCEED.ItemTransferReceiveDistribution", "LastModified");
            DropColumn("EXCEED.ItemTransferReceiveDistribution", "DateCreated");
            DropColumn("EXCEED.ItemTransferReceiveDistribution", "OrgBranchName");
            DropColumn("EXCEED.ItemTransferReceiveDistribution", "AdditionalDescription");
        }
    }
}
