namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PensionAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "JobCosting.JcPensions",
                c => new
                    {
                        JcPensionId = c.Int(nullable: false, identity: true),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.JcPensionId);
            
        }
        
        public override void Down()
        {
            DropTable("JobCosting.JcPensions");
        }
    }
}
