namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subaccounttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLChartOfAccountAccount", "AccountSubType", c => c.Int(nullable: false));
            AlterColumn("EXCEED.GLVatWithholding", "WithholdingAccountCode", c => c.String(nullable: false));
            AlterColumn("EXCEED.GLVatWithholding", "Branch", c => c.String());
            AlterColumn("EXCEED.PaymentMethod", "Branch", c => c.String());
            AlterColumn("EXCEED.PaymentMethod", "AccountCode", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PaymentMethod", "AccountCode", c => c.String());
            AlterColumn("EXCEED.PaymentMethod", "Branch", c => c.String(nullable: false));
            AlterColumn("EXCEED.GLVatWithholding", "Branch", c => c.String(nullable: false));
            AlterColumn("EXCEED.GLVatWithholding", "WithholdingAccountCode", c => c.String());
            DropColumn("EXCEED.GLChartOfAccountAccount", "AccountSubType");
        }
    }
}
