namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class incomestatementbyproject : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.GapCashFlowRange", "GapCashFlowId", "EXCEED.GapCashFlow");
            DropForeignKey("EXCEED.GapIncomeStatementByProjectRange", "GapIncomeStatementByProjectId", "EXCEED.GapIncomeStatementByProject");
            DropIndex("EXCEED.GapCashFlowRange", new[] { "GapCashFlowId" });
            DropIndex("EXCEED.GapIncomeStatementByProjectRange", new[] { "GapIncomeStatementByProjectId" });
            CreateTable(
                "EXCEED.GaapIncomeStatementByProjectOperationExpenseRange",
                c => new
                    {
                        GaapIncomeStatementByProjectOperationExpenseRangeId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementByProjectOperationExpenseId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementByProjectOperationExpenseRangeId)
                .ForeignKey("EXCEED.GaapIncomeStatementByProjectOperationExpense", t => t.GaapIncomeStatementByProjectOperationExpenseId, cascadeDelete: true)
                .Index(t => t.GaapIncomeStatementByProjectOperationExpenseId);
            
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
                .PrimaryKey(t => t.GaapIncomeStatementByProjectOperationExpenseId)
                .ForeignKey("EXCEED.GaapIncomeStatementByProject", t => t.GaapIncomeStatementByProjectId, cascadeDelete: true)
                .Index(t => t.GaapIncomeStatementByProjectId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatementByProject",
                c => new
                    {
                        GaapIncomeStatementByProjectId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementByProjectId);
            
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
                .PrimaryKey(t => t.GaapIncomeStatementByProjectOperationRevenueId)
                .ForeignKey("EXCEED.GaapIncomeStatementByProject", t => t.GaapIncomeStatementByProjectId, cascadeDelete: true)
                .Index(t => t.GaapIncomeStatementByProjectId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatementByProjectOperationRevenueRange",
                c => new
                    {
                        GaapIncomeStatementByProjectOperationRevenueRangeId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementByProjectOperationRevenueId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementByProjectOperationRevenueRangeId)
                .ForeignKey("EXCEED.GaapIncomeStatementByProjectOperationRevenue", t => t.GaapIncomeStatementByProjectOperationRevenueId, cascadeDelete: true)
                .Index(t => t.GaapIncomeStatementByProjectOperationRevenueId);
            
            DropTable("EXCEED.GapCashFlowRange");
            DropTable("EXCEED.GapCashFlow");
            DropTable("EXCEED.GapIncomeStatementByProjectRange");
            DropTable("EXCEED.GapIncomeStatementByProject");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.GapIncomeStatementByProject",
                c => new
                    {
                        GapIncomeStatementByProjectId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Parent = c.String(),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapIncomeStatementByProjectId);
            
            CreateTable(
                "EXCEED.GapIncomeStatementByProjectRange",
                c => new
                    {
                        GapIncomeStatementByProjectRangeId = c.Int(nullable: false, identity: true),
                        GapIncomeStatementByProjectId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapIncomeStatementByProjectRangeId);
            
            CreateTable(
                "EXCEED.GapCashFlow",
                c => new
                    {
                        GapCashFlowId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapCashFlowId);
            
            CreateTable(
                "EXCEED.GapCashFlowRange",
                c => new
                    {
                        GapCashFlowRangeId = c.Int(nullable: false, identity: true),
                        GapCashFlowId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapCashFlowRangeId);
            
            DropForeignKey("EXCEED.GaapIncomeStatementByProjectOperationExpenseRange", "GaapIncomeStatementByProjectOperationExpenseId", "EXCEED.GaapIncomeStatementByProjectOperationExpense");
            DropForeignKey("EXCEED.GaapIncomeStatementByProjectOperationRevenueRange", "GaapIncomeStatementByProjectOperationRevenueId", "EXCEED.GaapIncomeStatementByProjectOperationRevenue");
            DropForeignKey("EXCEED.GaapIncomeStatementByProjectOperationRevenue", "GaapIncomeStatementByProjectId", "EXCEED.GaapIncomeStatementByProject");
            DropForeignKey("EXCEED.GaapIncomeStatementByProjectOperationExpense", "GaapIncomeStatementByProjectId", "EXCEED.GaapIncomeStatementByProject");
            DropIndex("EXCEED.GaapIncomeStatementByProjectOperationRevenueRange", new[] { "GaapIncomeStatementByProjectOperationRevenueId" });
            DropIndex("EXCEED.GaapIncomeStatementByProjectOperationRevenue", new[] { "GaapIncomeStatementByProjectId" });
            DropIndex("EXCEED.GaapIncomeStatementByProjectOperationExpense", new[] { "GaapIncomeStatementByProjectId" });
            DropIndex("EXCEED.GaapIncomeStatementByProjectOperationExpenseRange", new[] { "GaapIncomeStatementByProjectOperationExpenseId" });
            DropTable("EXCEED.GaapIncomeStatementByProjectOperationRevenueRange");
            DropTable("EXCEED.GaapIncomeStatementByProjectOperationRevenue");
            DropTable("EXCEED.GaapIncomeStatementByProject");
            DropTable("EXCEED.GaapIncomeStatementByProjectOperationExpense");
            DropTable("EXCEED.GaapIncomeStatementByProjectOperationExpenseRange");
            CreateIndex("EXCEED.GapIncomeStatementByProjectRange", "GapIncomeStatementByProjectId");
            CreateIndex("EXCEED.GapCashFlowRange", "GapCashFlowId");
            AddForeignKey("EXCEED.GapIncomeStatementByProjectRange", "GapIncomeStatementByProjectId", "EXCEED.GapIncomeStatementByProject", "GapIncomeStatementByProjectId", cascadeDelete: true);
            AddForeignKey("EXCEED.GapCashFlowRange", "GapCashFlowId", "EXCEED.GapCashFlow", "GapCashFlowId", cascadeDelete: true);
        }
    }
}
