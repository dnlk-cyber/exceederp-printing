namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobOrderWithOrderType : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.Job", "OrderType", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.Job", "ContactPhone", c => c.String());
            AddColumn("ProductFollowUp.Job", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("ProductFollowUp.Job", "Pages", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("ProductFollowUp.Job", "Copies", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("ProductFollowUp.Job", "Quant");
        }
        
        public override void Down()
        {
            AddColumn("ProductFollowUp.Job", "Quant", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.Job", "Copies", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.Job", "Pages", c => c.Int(nullable: false));
            DropColumn("ProductFollowUp.Job", "Quantity");
            DropColumn("ProductFollowUp.Job", "ContactPhone");
            DropColumn("ProductFollowUp.Job", "OrderType");
        }
    }
}
