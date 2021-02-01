namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm43 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.RetirementSetting",
                c => new
                    {
                        RetirementSettingId = c.Int(nullable: false, identity: true),
                        RetireSetting = c.Int(nullable: false),
                        CurrentValue = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.RetirementSettingId);
            
            AddColumn("HRM.ApplicantionForm", "From", c => c.DateTime(nullable: false));
            AddColumn("HRM.ApplicantionForm", "To", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.ApplicantionForm", "To");
            DropColumn("HRM.ApplicantionForm", "From");
            DropTable("HRM.RetirementSetting");
        }
    }
}
