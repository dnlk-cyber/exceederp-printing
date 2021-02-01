namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tend2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisitionItem", "UoM", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisitionItem", "UoM");
        }
    }
}
