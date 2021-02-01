namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.OverTimeRequest", "FromDateAm", c => c.String());
            AddColumn("HRM.OverTimeRequest", "ToDateAm", c => c.String());
            AddColumn("HRM.Placement", "EmployementType", c => c.Int(nullable: false));
            AddColumn("Health.ReferralForm", "ReturnedDate", c => c.DateTime());
            AddColumn("Health.ReferralForm", "ReturnedDateAmharic", c => c.String());
            AddColumn("Health.ReferralForm", "ReturnedTime", c => c.DateTime());
            AlterColumn("Health.ReferralForm", "ReferredDateAmharic", c => c.String(nullable: false));
            DropColumn("Health.ReferralForm", "Time");
        }
        
        public override void Down()
        {
            AddColumn("Health.ReferralForm", "Time", c => c.DateTime(nullable: false));
            AlterColumn("Health.ReferralForm", "ReferredDateAmharic", c => c.String());
            DropColumn("Health.ReferralForm", "ReturnedTime");
            DropColumn("Health.ReferralForm", "ReturnedDateAmharic");
            DropColumn("Health.ReferralForm", "ReturnedDate");
            DropColumn("HRM.Placement", "EmployementType");
            DropColumn("HRM.OverTimeRequest", "ToDateAm");
            DropColumn("HRM.OverTimeRequest", "FromDateAm");
        }
    }
}
