namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storereq : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisitionItem", "IsApproved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreRequisitionItem", "IsApproved");
        }
    }
}
