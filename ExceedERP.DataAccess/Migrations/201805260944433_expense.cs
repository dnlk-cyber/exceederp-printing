namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class expense : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApTravelExpensePayment", "Reference", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseRefund", "Reference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApTravelExpenseRefund", "Reference");
            DropColumn("EXCEED.ApTravelExpensePayment", "Reference");
        }
    }
}
