namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemsubandrevenuereport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.RevenueBySalesType",
                c => new
                    {
                        RevenueBySalesTypeId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.RevenueBySalesTypeId);
            
            CreateTable(
                "EXCEED.RevenueList",
                c => new
                    {
                        RevenueListId = c.Int(nullable: false, identity: true),
                        RevenueBySalesTypeId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.RevenueListId)
                .ForeignKey("EXCEED.RevenueBySalesType", t => t.RevenueBySalesTypeId, cascadeDelete: true)
                .Index(t => t.RevenueBySalesTypeId);
            
            CreateTable(
                "EXCEED.RevenueListRange",
                c => new
                    {
                        RevenueListRangeId = c.Int(nullable: false, identity: true),
                        RevenueListId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.RevenueListRangeId)
                .ForeignKey("EXCEED.RevenueList", t => t.RevenueListId, cascadeDelete: true)
                .Index(t => t.RevenueListId);
            
            AddColumn("EXCEED.ItemCategory", "SubCategoryCode", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.RevenueListRange", "RevenueListId", "EXCEED.RevenueList");
            DropForeignKey("EXCEED.RevenueList", "RevenueBySalesTypeId", "EXCEED.RevenueBySalesType");
            DropIndex("EXCEED.RevenueListRange", new[] { "RevenueListId" });
            DropIndex("EXCEED.RevenueList", new[] { "RevenueBySalesTypeId" });
            DropColumn("EXCEED.ItemCategory", "SubCategoryCode");
            DropTable("EXCEED.RevenueListRange");
            DropTable("EXCEED.RevenueList");
            DropTable("EXCEED.RevenueBySalesType");
        }
    }
}
