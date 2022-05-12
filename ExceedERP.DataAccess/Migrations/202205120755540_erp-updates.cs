namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class erpupdates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Common.Identification", "Description", c => c.String());
            AlterColumn("Common.Identification", "IdNumber", c => c.String());
            AlterColumn("Common.Identification", "IssueDate", c => c.DateTime());
            AlterColumn("Common.Identification", "ExpiryDate", c => c.DateTime());
            AlterColumn("Common.Identification", "Issuer", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Common.Identification", "Issuer", c => c.String(nullable: false));
            AlterColumn("Common.Identification", "ExpiryDate", c => c.DateTime(nullable: false));
            AlterColumn("Common.Identification", "IssueDate", c => c.DateTime(nullable: false));
            AlterColumn("Common.Identification", "IdNumber", c => c.String(nullable: false));
            AlterColumn("Common.Identification", "Description", c => c.String(nullable: false));
        }
    }
}
