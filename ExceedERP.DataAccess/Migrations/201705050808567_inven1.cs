namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inven1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisition", "BranchId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.IssueFuelValidation", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.IssueFuelValidation", "Status", c => c.String());
            DropColumn("EXCEED.PurchaseRequisition", "BranchId");
        }
    }
}
