namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fi : DbMigration
    {
        public override void Up()
        {
            //AddColumn("EXCEED.AnnualDepreciation", "DepreciationAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AddColumn("EXCEED.AnnualDepreciation", "GlFiscalYearId", c => c.Int(nullable: false));
            //DropColumn("EXCEED.AnnualDepreciation", "DepreciationTime");
            //DropColumn("EXCEED.AnnualDepreciation", "Amount");
            //DropColumn("EXCEED.AnnualDepreciation", "OriginalPrice");
            //DropColumn("EXCEED.AnnualDepreciation", "Accomodate");
            //DropColumn("EXCEED.AnnualDepreciation", "GlPeriod");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.AnnualDepreciation", "GlPeriod", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "Accomodate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AnnualDepreciation", "OriginalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AnnualDepreciation", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AnnualDepreciation", "DepreciationTime", c => c.DateTime(nullable: false));
            DropColumn("EXCEED.AnnualDepreciation", "GlFiscalYearId");
            DropColumn("EXCEED.AnnualDepreciation", "DepreciationAmount");
        }
    }
}
