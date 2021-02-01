namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class po2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseOrder", "OrgStructureId", c => c.Int(nullable: false));
            DropColumn("EXCEED.PurchaseOrder", "CostCenter");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PurchaseOrder", "CostCenter", c => c.String());
            DropColumn("EXCEED.PurchaseOrder", "OrgStructureId");
        }
    }
}
