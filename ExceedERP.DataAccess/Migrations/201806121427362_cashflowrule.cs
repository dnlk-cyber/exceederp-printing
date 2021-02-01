namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cashflowrule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.GaapIncomeStatementByProjectOperationExpense", "GaapIncomeStatementByProjectId", "EXCEED.GaapIncomeStatementByProject");
            DropForeignKey("EXCEED.GaapIncomeStatementByProjectOperationRevenue", "GaapIncomeStatementByProjectId", "EXCEED.GaapIncomeStatementByProject");
            DropForeignKey("EXCEED.GaapIncomeStatementByProjectOperationRevenueRange", "GaapIncomeStatementByProjectOperationRevenueId", "EXCEED.GaapIncomeStatementByProjectOperationRevenue");
            DropForeignKey("EXCEED.GaapIncomeStatementByProjectOperationExpenseRange", "GaapIncomeStatementByProjectOperationExpenseId", "EXCEED.GaapIncomeStatementByProjectOperationExpense");
            DropIndex("EXCEED.GaapIncomeStatementByProjectOperationExpenseRange", new[] { "GaapIncomeStatementByProjectOperationExpenseId" });
            DropIndex("EXCEED.GaapIncomeStatementByProjectOperationExpense", new[] { "GaapIncomeStatementByProjectId" });
            DropIndex("EXCEED.GaapIncomeStatementByProjectOperationRevenue", new[] { "GaapIncomeStatementByProjectId" });
            DropIndex("EXCEED.GaapIncomeStatementByProjectOperationRevenueRange", new[] { "GaapIncomeStatementByProjectOperationRevenueId" });
            CreateTable(
                "EXCEED.GaapCashFlowPayOutRange",
                c => new
                    {
                        GaapCashFlowPayOutRangeId = c.Int(nullable: false, identity: true),
                        GaapCashFlowPayOutId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GaapCashFlowPayOutRangeId)
                .ForeignKey("EXCEED.GaapCashFlowPayOut", t => t.GaapCashFlowPayOutId, cascadeDelete: true)
                .Index(t => t.GaapCashFlowPayOutId);
            
            CreateTable(
                "EXCEED.GaapCashFlowPayOut",
                c => new
                    {
                        GaapCashFlowPayOutId = c.Int(nullable: false, identity: true),
                        GaapCashFlowId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.GaapCashFlowPayOutId)
                .ForeignKey("EXCEED.GaapCashFlow", t => t.GaapCashFlowId, cascadeDelete: true)
                .Index(t => t.GaapCashFlowId);
            
            CreateTable(
                "EXCEED.GaapCashFlow",
                c => new
                    {
                        GaapCashFlowId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.GaapCashFlowId);
            
            CreateTable(
                "EXCEED.GaapCashFlowReceipt",
                c => new
                    {
                        GaapCashFlowReceiptId = c.Int(nullable: false, identity: true),
                        GaapCashFlowId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.GaapCashFlowReceiptId)
                .ForeignKey("EXCEED.GaapCashFlow", t => t.GaapCashFlowId, cascadeDelete: true)
                .Index(t => t.GaapCashFlowId);
            
            CreateTable(
                "EXCEED.GaapCashFlowReceiptRange",
                c => new
                    {
                        GaapCashFlowReceiptRangeId = c.Int(nullable: false, identity: true),
                        GaapCashFlowReceiptId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GaapCashFlowReceiptRangeId)
                .ForeignKey("EXCEED.GaapCashFlowReceipt", t => t.GaapCashFlowReceiptId, cascadeDelete: true)
                .Index(t => t.GaapCashFlowReceiptId);
            
            DropTable("EXCEED.GaapIncomeStatementByProjectOperationExpenseRange");
            DropTable("EXCEED.GaapIncomeStatementByProjectOperationExpense");
            DropTable("EXCEED.GaapIncomeStatementByProject");
            DropTable("EXCEED.GaapIncomeStatementByProjectOperationRevenue");
            DropTable("EXCEED.GaapIncomeStatementByProjectOperationRevenueRange");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.GaapIncomeStatementByProjectOperationRevenueRange",
                c => new
                    {
                        GaapIncomeStatementByProjectOperationRevenueRangeId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementByProjectOperationRevenueId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementByProjectOperationRevenueRangeId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatementByProjectOperationRevenue",
                c => new
                    {
                        GaapIncomeStatementByProjectOperationRevenueId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementByProjectId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementByProjectOperationRevenueId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatementByProject",
                c => new
                    {
                        GaapIncomeStatementByProjectId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementByProjectId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatementByProjectOperationExpense",
                c => new
                    {
                        GaapIncomeStatementByProjectOperationExpenseId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementByProjectId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementByProjectOperationExpenseId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatementByProjectOperationExpenseRange",
                c => new
                    {
                        GaapIncomeStatementByProjectOperationExpenseRangeId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementByProjectOperationExpenseId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementByProjectOperationExpenseRangeId);
            
            DropForeignKey("EXCEED.GaapCashFlowPayOutRange", "GaapCashFlowPayOutId", "EXCEED.GaapCashFlowPayOut");
            DropForeignKey("EXCEED.GaapCashFlowReceiptRange", "GaapCashFlowReceiptId", "EXCEED.GaapCashFlowReceipt");
            DropForeignKey("EXCEED.GaapCashFlowReceipt", "GaapCashFlowId", "EXCEED.GaapCashFlow");
            DropForeignKey("EXCEED.GaapCashFlowPayOut", "GaapCashFlowId", "EXCEED.GaapCashFlow");
            DropIndex("EXCEED.GaapCashFlowReceiptRange", new[] { "GaapCashFlowReceiptId" });
            DropIndex("EXCEED.GaapCashFlowReceipt", new[] { "GaapCashFlowId" });
            DropIndex("EXCEED.GaapCashFlowPayOut", new[] { "GaapCashFlowId" });
            DropIndex("EXCEED.GaapCashFlowPayOutRange", new[] { "GaapCashFlowPayOutId" });
            DropTable("EXCEED.GaapCashFlowReceiptRange");
            DropTable("EXCEED.GaapCashFlowReceipt");
            DropTable("EXCEED.GaapCashFlow");
            DropTable("EXCEED.GaapCashFlowPayOut");
            DropTable("EXCEED.GaapCashFlowPayOutRange");
            CreateIndex("EXCEED.GaapIncomeStatementByProjectOperationRevenueRange", "GaapIncomeStatementByProjectOperationRevenueId");
            CreateIndex("EXCEED.GaapIncomeStatementByProjectOperationRevenue", "GaapIncomeStatementByProjectId");
            CreateIndex("EXCEED.GaapIncomeStatementByProjectOperationExpense", "GaapIncomeStatementByProjectId");
            CreateIndex("EXCEED.GaapIncomeStatementByProjectOperationExpenseRange", "GaapIncomeStatementByProjectOperationExpenseId");
            AddForeignKey("EXCEED.GaapIncomeStatementByProjectOperationExpenseRange", "GaapIncomeStatementByProjectOperationExpenseId", "EXCEED.GaapIncomeStatementByProjectOperationExpense", "GaapIncomeStatementByProjectOperationExpenseId", cascadeDelete: true);
            AddForeignKey("EXCEED.GaapIncomeStatementByProjectOperationRevenueRange", "GaapIncomeStatementByProjectOperationRevenueId", "EXCEED.GaapIncomeStatementByProjectOperationRevenue", "GaapIncomeStatementByProjectOperationRevenueId", cascadeDelete: true);
            AddForeignKey("EXCEED.GaapIncomeStatementByProjectOperationRevenue", "GaapIncomeStatementByProjectId", "EXCEED.GaapIncomeStatementByProject", "GaapIncomeStatementByProjectId", cascadeDelete: true);
            AddForeignKey("EXCEED.GaapIncomeStatementByProjectOperationExpense", "GaapIncomeStatementByProjectId", "EXCEED.GaapIncomeStatementByProject", "GaapIncomeStatementByProjectId", cascadeDelete: true);
        }
    }
}
