namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrpayroll : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Payroll.PayrollPeriod",
                c => new
                    {
                        GLPeriodId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateFromAmharic = c.String(),
                        DateTo = c.DateTime(nullable: false),
                        DateToAmharic = c.String(),
                        Status = c.Int(nullable: false),
                        IsPermanentlyClosed = c.Boolean(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLPeriodId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            AddColumn("Payroll.Association", "ApplyToAll", c => c.Boolean(nullable: false));
            AddColumn("Health.FixedAssetAccidentRecord", "BranchId", c => c.Int(nullable: false));
            AddColumn("Payroll.PayrollAccounts", "IsPerBranch", c => c.Boolean(nullable: false));
            CreateIndex("Health.FixedAssetAccidentRecord", "BranchId");
            AddForeignKey("Health.FixedAssetAccidentRecord", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Health.FixedAssetAccidentRecord", "BranchId", "HRM.Branch");
            DropForeignKey("Payroll.PayrollPeriod", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("Health.FixedAssetAccidentRecord", new[] { "BranchId" });
            DropIndex("Payroll.PayrollPeriod", new[] { "GlFiscalYearId" });
            DropColumn("Payroll.PayrollAccounts", "IsPerBranch");
            DropColumn("Health.FixedAssetAccidentRecord", "BranchId");
            DropColumn("Payroll.Association", "ApplyToAll");
            DropTable("Payroll.PayrollPeriod");
        }
    }
}
