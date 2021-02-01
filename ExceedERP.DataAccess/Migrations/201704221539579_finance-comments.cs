namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financecomments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FinanceSetting",
                c => new
                    {
                        FinanceSettingId = c.Int(nullable: false, identity: true),
                        ProfitTaxRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitTaxCalculatedRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinanceSettingId);
            
            AddColumn("EXCEED.GLPeriod", "IsPermanentlyClosed", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GlFiscalYear", "IsPermanentlyClosed", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GLTaxRate", "CalculatedRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GLTaxRate", "CalculatedRate");
            DropColumn("EXCEED.GlFiscalYear", "IsPermanentlyClosed");
            DropColumn("EXCEED.GLPeriod", "IsPermanentlyClosed");
            DropTable("EXCEED.FinanceSetting");
        }
    }
}
