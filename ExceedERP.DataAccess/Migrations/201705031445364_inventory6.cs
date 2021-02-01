namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory6 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.IssueValidation", "Approver");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.IssueValidation", "Approver", c => c.String());
        }
    }
}
