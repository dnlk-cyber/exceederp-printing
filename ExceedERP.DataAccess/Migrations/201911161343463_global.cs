namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class global : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.UserBusinessUnitMap",
                c => new
                    {
                        UserBusinessUnitMapId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        BusinessUnitId = c.String(),
                    })
                .PrimaryKey(t => t.UserBusinessUnitMapId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.UserBusinessUnitMap");
        }
    }
}
