namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixeissue : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.FixedAssetIssueItem", "TagNo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.FixedAssetIssueItem", "TagNo", c => c.String(nullable: false));
        }
    }
}
