namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procfixed : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "Name", c => c.String());
            DropColumn("EXCEED.AssetTransferReceive", "Type");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.AssetTransferReceive", "Type", c => c.Int(nullable: false));
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "Name");
        }
    }
}
