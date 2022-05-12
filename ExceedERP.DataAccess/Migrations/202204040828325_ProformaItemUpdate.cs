namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProformaItemUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.PFProformaItems", "JT", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.PFProformaItems", "JT");
        }
    }
}
