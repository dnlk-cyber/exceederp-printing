namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OtherBenefit1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.RetirementSetting", "RetireTypeSetting", c => c.Int(nullable: false));
            //DropColumn("HRM.RetirementSetting", "RetireSetting");
        }
        
        public override void Down()
        {
            AddColumn("HRM.RetirementSetting", "RetireSetting", c => c.Int(nullable: false));
            //DropColumn("HRM.RetirementSetting", "RetireTypeSetting");
        }
    }
}
