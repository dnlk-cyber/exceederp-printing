namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archive3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Archive.EmployeeLetterSender", "TransferDescription", c => c.String());
            AddColumn("Archive.LetterSender", "TransferDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Archive.LetterSender", "TransferDescription");
            DropColumn("Archive.EmployeeLetterSender", "TransferDescription");
        }
    }
}
