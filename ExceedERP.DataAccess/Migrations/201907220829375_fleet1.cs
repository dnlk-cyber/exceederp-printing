namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleet1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("FleetManagement.LogisticPlan", "Type", c => c.String());
            AddColumn("FleetManagement.OtherCompanyMaintenance", "KmhrReading", c => c.String());
            AddColumn("FleetManagement.OtherCompanyMaintenance", "SerialNo", c => c.String());
            AddColumn("FleetManagement.OtherCompanyMaintenance", "PlateNo", c => c.String(nullable: false));
            AddColumn("FleetManagement.PMSchedule", "Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("FleetManagement.PMSchedule", "Type");
            DropColumn("FleetManagement.OtherCompanyMaintenance", "PlateNo");
            DropColumn("FleetManagement.OtherCompanyMaintenance", "SerialNo");
            DropColumn("FleetManagement.OtherCompanyMaintenance", "KmhrReading");
            DropColumn("FleetManagement.LogisticPlan", "Type");
        }
    }
}
