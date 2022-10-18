namespace ExceedERP.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationSignature : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "User.ApplicationUserSignatures",
                c => new
                    {
                        ApplicationUserSignatureId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(),
                        Sign = c.Binary(),
                    })
                .PrimaryKey(t => t.ApplicationUserSignatureId);
            
        }
        
        public override void Down()
        {
            DropTable("User.ApplicationUserSignatures");
        }
    }
}
