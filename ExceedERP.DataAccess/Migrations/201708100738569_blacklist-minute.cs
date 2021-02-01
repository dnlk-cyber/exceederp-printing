namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blacklistminute : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.SupplierBlackFile",
                c => new
                    {
                        SupplierBlackFileId = c.Int(nullable: false, identity: true),
                        SupplierBlackListId = c.Int(nullable: false),
                        FileName = c.String(),
                        Description = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.SupplierBlackFileId)
                .ForeignKey("EXCEED.SupplierBlackList", t => t.SupplierBlackListId, cascadeDelete: true)
                .Index(t => t.SupplierBlackListId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.SupplierBlackFile", "SupplierBlackListId", "EXCEED.SupplierBlackList");
            DropIndex("EXCEED.SupplierBlackFile", new[] { "SupplierBlackListId" });
            DropTable("EXCEED.SupplierBlackFile");
        }
    }
}
