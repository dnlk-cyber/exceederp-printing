namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArStandardCollectionLines", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArStandardCollectionLines", "BranchId");
        }
    }
}
