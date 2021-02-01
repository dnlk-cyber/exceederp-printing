namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmfleet4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Budget.BudgetRequestTransferDistribution",
                c => new
                    {
                        BudgetRequestTransferDistributionId = c.Int(nullable: false, identity: true),
                        BudgetRequestId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.BudgetRequestTransferDistributionId)
                .ForeignKey("Budget.BudgetRequest", t => t.BudgetRequestId, cascadeDelete: true)
                .Index(t => t.BudgetRequestId);
            
            CreateTable(
                "Budget.BudgetType",
                c => new
                    {
                        BudgetTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Budget.BudgetRequestTransferDistribution", "BudgetRequestId", "Budget.BudgetRequest");
            DropIndex("Budget.BudgetRequestTransferDistribution", new[] { "BudgetRequestId" });
            DropTable("Budget.BudgetType");
            DropTable("Budget.BudgetRequestTransferDistribution");
        }
    }
}
