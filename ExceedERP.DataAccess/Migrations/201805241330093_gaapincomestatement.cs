namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gaapincomestatement : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.GapIncomeStatementRange", "GapIncomeStatementId", "EXCEED.GapIncomeStatement");
            DropIndex("EXCEED.GapIncomeStatementRange", new[] { "GapIncomeStatementId" });
            CreateTable(
                "EXCEED.GaapIncomeStatementOperationExpenseRange",
                c => new
                    {
                        GaapIncomeStatementOperationExpenseRangeId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementOperationExpenseId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementOperationExpenseRangeId)
                .ForeignKey("EXCEED.GaapIncomeStatementOperationExpense", t => t.GaapIncomeStatementOperationExpenseId, cascadeDelete: true)
                .Index(t => t.GaapIncomeStatementOperationExpenseId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatementOperationExpense",
                c => new
                    {
                        GaapIncomeStatementOperationExpenseId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementOperationExpenseId)
                .ForeignKey("EXCEED.GaapIncomeStatement", t => t.GaapIncomeStatementId, cascadeDelete: true)
                .Index(t => t.GaapIncomeStatementId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatement",
                c => new
                    {
                        GaapIncomeStatementId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatementOperationRevenue",
                c => new
                    {
                        GaapIncomeStatementOperationRevenueId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementOperationRevenueId)
                .ForeignKey("EXCEED.GaapIncomeStatement", t => t.GaapIncomeStatementId, cascadeDelete: true)
                .Index(t => t.GaapIncomeStatementId);
            
            CreateTable(
                "EXCEED.GaapIncomeStatementOperationRevenueRange",
                c => new
                    {
                        GaapIncomeStatementOperationRevenueRangeId = c.Int(nullable: false, identity: true),
                        GaapIncomeStatementOperationRevenueId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GaapIncomeStatementOperationRevenueRangeId)
                .ForeignKey("EXCEED.GaapIncomeStatementOperationRevenue", t => t.GaapIncomeStatementOperationRevenueId, cascadeDelete: true)
                .Index(t => t.GaapIncomeStatementOperationRevenueId);
            
            DropTable("EXCEED.GapIncomeStatementRange");
            DropTable("EXCEED.GapIncomeStatement");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.GapIncomeStatement",
                c => new
                    {
                        GapIncomeStatementId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Parent = c.String(),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapIncomeStatementId);
            
            CreateTable(
                "EXCEED.GapIncomeStatementRange",
                c => new
                    {
                        GapIncomeStatementRangeId = c.Int(nullable: false, identity: true),
                        GapIncomeStatementId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapIncomeStatementRangeId);
            
            DropForeignKey("EXCEED.GaapIncomeStatementOperationExpenseRange", "GaapIncomeStatementOperationExpenseId", "EXCEED.GaapIncomeStatementOperationExpense");
            DropForeignKey("EXCEED.GaapIncomeStatementOperationRevenueRange", "GaapIncomeStatementOperationRevenueId", "EXCEED.GaapIncomeStatementOperationRevenue");
            DropForeignKey("EXCEED.GaapIncomeStatementOperationRevenue", "GaapIncomeStatementId", "EXCEED.GaapIncomeStatement");
            DropForeignKey("EXCEED.GaapIncomeStatementOperationExpense", "GaapIncomeStatementId", "EXCEED.GaapIncomeStatement");
            DropIndex("EXCEED.GaapIncomeStatementOperationRevenueRange", new[] { "GaapIncomeStatementOperationRevenueId" });
            DropIndex("EXCEED.GaapIncomeStatementOperationRevenue", new[] { "GaapIncomeStatementId" });
            DropIndex("EXCEED.GaapIncomeStatementOperationExpense", new[] { "GaapIncomeStatementId" });
            DropIndex("EXCEED.GaapIncomeStatementOperationExpenseRange", new[] { "GaapIncomeStatementOperationExpenseId" });
            DropTable("EXCEED.GaapIncomeStatementOperationRevenueRange");
            DropTable("EXCEED.GaapIncomeStatementOperationRevenue");
            DropTable("EXCEED.GaapIncomeStatement");
            DropTable("EXCEED.GaapIncomeStatementOperationExpense");
            DropTable("EXCEED.GaapIncomeStatementOperationExpenseRange");
            CreateIndex("EXCEED.GapIncomeStatementRange", "GapIncomeStatementId");
            AddForeignKey("EXCEED.GapIncomeStatementRange", "GapIncomeStatementId", "EXCEED.GapIncomeStatement", "GapIncomeStatementId", cascadeDelete: true);
        }
    }
}
