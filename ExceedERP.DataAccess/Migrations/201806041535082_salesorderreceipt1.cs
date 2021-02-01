namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesorderreceipt1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ArSalesOrderPartialPrintDistribution",
                c => new
                    {
                        ArSalesOrderPartialPrintDistributionId = c.Int(nullable: false, identity: true),
                        ArSalesOrderPartialPrintId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderPartialPrintDistributionId)
                .ForeignKey("EXCEED.ArSalesOrderPartialPrint", t => t.ArSalesOrderPartialPrintId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderPartialPrintId);
            
            CreateTable(
                "EXCEED.ArSalesOrderPartialPrintValidation",
                c => new
                    {
                        ArSalesOrderPartialPrintValidationId = c.Int(nullable: false, identity: true),
                        ArSalesOrderPartialPrintId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderPartialPrintValidationId)
                .ForeignKey("EXCEED.ArSalesOrderPartialPrint", t => t.ArSalesOrderPartialPrintId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderPartialPrintId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ArSalesOrderPartialPrintValidation", "ArSalesOrderPartialPrintId", "EXCEED.ArSalesOrderPartialPrint");
            DropForeignKey("EXCEED.ArSalesOrderPartialPrintDistribution", "ArSalesOrderPartialPrintId", "EXCEED.ArSalesOrderPartialPrint");
            DropIndex("EXCEED.ArSalesOrderPartialPrintValidation", new[] { "ArSalesOrderPartialPrintId" });
            DropIndex("EXCEED.ArSalesOrderPartialPrintDistribution", new[] { "ArSalesOrderPartialPrintId" });
            DropTable("EXCEED.ArSalesOrderPartialPrintValidation");
            DropTable("EXCEED.ArSalesOrderPartialPrintDistribution");
        }
    }
}
