namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class branchinvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArInvoice", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ArSalesOrder", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArSalesOrder", "BranchId");
            DropColumn("EXCEED.ArInvoice", "BranchId");
        }
    }
}
