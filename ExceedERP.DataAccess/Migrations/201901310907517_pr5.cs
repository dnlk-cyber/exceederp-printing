namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pr5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PurchaseRequisition", "ForBranchId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseRequisition", "ForBranchId", c => c.Int(nullable: false));
        }
    }
}
