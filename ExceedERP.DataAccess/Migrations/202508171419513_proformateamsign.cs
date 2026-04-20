namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proformateamsign : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformaTeamMember", "Signed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformaTeamMember", "Signed");
        }
    }
}
