namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nextapprover : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetRequisition", "NextApprover", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetRequisition", "NextApprover");
        }
    }
}
