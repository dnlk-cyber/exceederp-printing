namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.WaterMark",
                c => new
                    {
                        WaterMarkId = c.Int(nullable: false, identity: true),
                        IsNeened = c.Boolean(nullable: false),
                        Name = c.String(),
                        Abbreviation = c.String(),
                        WaterMarkFileName = c.String(),
                    })
                .PrimaryKey(t => t.WaterMarkId);
            
        }
        
        public override void Down()
        {
            DropTable("HRM.WaterMark");
        }
    }
}
