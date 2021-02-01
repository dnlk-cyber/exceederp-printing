namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supprankremoved : DbMigration
    {
        public override void Up()
        {
            DropIndex("EXCEED.SupplierRank", new[] { "Name" });
            DropTable("EXCEED.SupplierRank");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.SupplierRank",
                c => new
                    {
                        SupplierRankId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 45),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupplierRankId);
            
            CreateIndex("EXCEED.SupplierRank", "Name", unique: true);
        }
    }
}
