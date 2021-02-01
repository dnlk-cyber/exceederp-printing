namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenderextension : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.TenderExtension",
                c => new
                    {
                        TenderExtensionId = c.Int(nullable: false, identity: true),
                        FloatDate = c.DateTime(nullable: false),
                        ClosingDate = c.DateTime(nullable: false),
                        OpenigDate = c.DateTime(nullable: false),
                        TechnicalOpenigDate = c.DateTime(),
                        FinancialOpenigDate = c.DateTime(),
                        Remark = c.String(),
                        TenderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TenderExtensionId)
                .ForeignKey("EXCEED.Tender", t => t.TenderId, cascadeDelete: true)
                .Index(t => t.TenderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.TenderExtension", "TenderId", "EXCEED.Tender");
            DropIndex("EXCEED.TenderExtension", new[] { "TenderId" });
            DropTable("EXCEED.TenderExtension");
        }
    }
}
