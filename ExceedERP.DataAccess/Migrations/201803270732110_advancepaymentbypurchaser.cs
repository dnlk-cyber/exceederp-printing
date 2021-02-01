namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class advancepaymentbypurchaser : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPayment", "CollectedByPurchaser", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPayment", "EmployeeId", c => c.Int());
            AddColumn("EXCEED.ApPayment", "PoType", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPayment", "PurechaseReference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApPayment", "PurechaseReference");
            DropColumn("EXCEED.ApPayment", "PoType");
            DropColumn("EXCEED.ApPayment", "EmployeeId");
            DropColumn("EXCEED.ApPayment", "CollectedByPurchaser");
        }
    }
}
