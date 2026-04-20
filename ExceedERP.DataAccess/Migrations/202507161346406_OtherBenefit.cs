namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OtherBenefit : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.OtherBenefit", "IsForAll", c => c.Boolean(nullable: false));
            AddColumn("HRM.OtherBenefit", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.OtherBenefit", "Gender");
            DropColumn("HRM.OtherBenefit", "IsForAll");
        }
    }
}
