namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class appayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPayment", "WithholdingType", c => c.String());
            AddColumn("EXCEED.ApPayment", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApPayment", "WithholdTax", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPayment", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApPayment", "NetPayable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.IssueValidation", "OrgBranchName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.IssueValidation", "OrgBranchName");
            DropColumn("EXCEED.ApPayment", "NetPayable");
            DropColumn("EXCEED.ApPayment", "WithholdTaxAmount");
            DropColumn("EXCEED.ApPayment", "WithholdTax");
            DropColumn("EXCEED.ApPayment", "WithholdingAmount");
            DropColumn("EXCEED.ApPayment", "WithholdingType");
        }
    }
}
