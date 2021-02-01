namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reassignment : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "TenderType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "TenderType");
        }
    }
}
