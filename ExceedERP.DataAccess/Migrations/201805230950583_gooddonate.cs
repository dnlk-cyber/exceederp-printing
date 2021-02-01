namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gooddonate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.GoodSellingAndLendingValidation", "Status", c => c.Int(nullable: false));
            DropColumn("EXCEED.GoodSellingAndLendingValidation", "Approver");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.GoodSellingAndLendingValidation", "Approver", c => c.String());
            AlterColumn("EXCEED.GoodSellingAndLendingValidation", "Status", c => c.String());
        }
    }
}
