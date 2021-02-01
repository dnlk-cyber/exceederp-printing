namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class leaveafternoon : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Leave", "MorningOrAfternoon", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Leave", "MorningOrAfternoon");
        }
    }
}
