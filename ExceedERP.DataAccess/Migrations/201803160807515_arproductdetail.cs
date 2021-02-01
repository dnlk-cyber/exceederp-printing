namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arproductdetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ArProductService",
                c => new
                    {
                        ArProductServiceId = c.Int(nullable: false, identity: true),
                        ARSetupProductServiceAccountID = c.Int(nullable: false),
                        ItemName = c.String(nullable: false),
                        Description = c.String(),
                        UoM = c.String(nullable: false),
                        SalesPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArProductServiceId)
                .ForeignKey("EXCEED.ARSetupProductServiceAccount", t => t.ARSetupProductServiceAccountID, cascadeDelete: true)
                .Index(t => t.ARSetupProductServiceAccountID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ArProductService", "ARSetupProductServiceAccountID", "EXCEED.ARSetupProductServiceAccount");
            DropIndex("EXCEED.ArProductService", new[] { "ARSetupProductServiceAccountID" });
            DropTable("EXCEED.ArProductService");
        }
    }
}
