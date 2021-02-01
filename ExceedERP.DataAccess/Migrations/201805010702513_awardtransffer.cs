namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class awardtransffer : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BidAwardedSupplier", "IsTransferredToContract", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.BidAwardedSupplier", "IsTransferredToContract");
        }
    }
}
