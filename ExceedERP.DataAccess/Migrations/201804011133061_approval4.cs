namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approval4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Global.ApprovalGroup", "OpertatingUnit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Global.ApprovalGroup", "OpertatingUnit", c => c.String());
        }
    }
}
