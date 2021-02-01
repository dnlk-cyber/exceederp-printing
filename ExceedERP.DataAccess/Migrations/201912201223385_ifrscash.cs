namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ifrscash : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.IfrsCashAndCashEquivalentRange",
                c => new
                    {
                        IfrsCashAndCashEquivalentRangeId = c.Int(nullable: false, identity: true),
                        IfrsStatementOfCashFlowId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IfrsCashAndCashEquivalentRangeId)
                .ForeignKey("EXCEED.IfrsStatementOfCashFlow", t => t.IfrsStatementOfCashFlowId, cascadeDelete: true)
                .Index(t => t.IfrsStatementOfCashFlowId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.IfrsCashAndCashEquivalentRange", "IfrsStatementOfCashFlowId", "EXCEED.IfrsStatementOfCashFlow");
            DropIndex("EXCEED.IfrsCashAndCashEquivalentRange", new[] { "IfrsStatementOfCashFlowId" });
            DropTable("EXCEED.IfrsCashAndCashEquivalentRange");
        }
    }
}
