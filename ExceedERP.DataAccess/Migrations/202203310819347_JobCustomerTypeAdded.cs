namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobCustomerTypeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.Job", "ProductionCustomertypeId", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.Job", "Status", c => c.Int());
            DropColumn("ProductFollowUp.Job", "OrderType");
        }
        
        public override void Down()
        {
            AddColumn("ProductFollowUp.Job", "OrderType", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.Job", "Status", c => c.String(maxLength: 25));
            DropColumn("ProductFollowUp.Job", "ProductionCustomertypeId");
        }
    }
}
