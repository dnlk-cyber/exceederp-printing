namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guranteeupload : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GuaranteeFile",
                c => new
                    {
                        GuaranteeFileId = c.Int(nullable: false, identity: true),
                        GuaranteeId = c.Int(nullable: false),
                        FileName = c.String(),
                        Description = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.GuaranteeFileId)
                .ForeignKey("EXCEED.Guarantee", t => t.GuaranteeId, cascadeDelete: true)
                .Index(t => t.GuaranteeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GuaranteeFile", "GuaranteeId", "EXCEED.Guarantee");
            DropIndex("EXCEED.GuaranteeFile", new[] { "GuaranteeId" });
            DropTable("EXCEED.GuaranteeFile");
        }
    }
}
