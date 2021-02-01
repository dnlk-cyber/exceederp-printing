namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset18 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.AssetTransfer", "CustodianBeforeTransfer", c => c.Int());
            AlterColumn("EXCEED.AssetTransfer", "TransferTo", c => c.Int());
            AlterColumn("EXCEED.AssetTransfer", "FromBranchId", c => c.Int());
            AlterColumn("EXCEED.AssetTransfer", "ToBranchId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.AssetTransfer", "ToBranchId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.AssetTransfer", "FromBranchId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.AssetTransfer", "TransferTo", c => c.Int(nullable: false));
            AlterColumn("EXCEED.AssetTransfer", "CustodianBeforeTransfer", c => c.Int(nullable: false));
        }
    }
}
