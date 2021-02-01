namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _return : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.SupplierRankId)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("EXCEED.SupplierRank", new[] { "Name" });
            DropTable("EXCEED.SupplierRank");
        }
    }
}
