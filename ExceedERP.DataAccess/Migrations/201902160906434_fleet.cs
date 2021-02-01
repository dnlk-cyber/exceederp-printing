namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleet : DbMigration
    {
        public override void Up()
        {
            AddColumn("FleetManagement.Accident", "PolicyIdNo", c => c.String());
            AddColumn("FleetManagement.Accident", "CurrentValue", c => c.String());
            AddColumn("FleetManagement.Accident", "RenewalDateAmh", c => c.String());
            AddColumn("EXCEED.EmployeeAccident", "Description", c => c.String());
            AlterColumn("FleetManagement.Accident", "RenewalDate", c => c.DateTime());
            DropColumn("FleetManagement.Accident", "Operator");
            DropColumn("FleetManagement.Accident", "PoliceIdNo");
        }
        
        public override void Down()
        {
            AddColumn("FleetManagement.Accident", "PoliceIdNo", c => c.String());
            AddColumn("FleetManagement.Accident", "Operator", c => c.String());
            AlterColumn("FleetManagement.Accident", "RenewalDate", c => c.DateTime(nullable: false));
            DropColumn("EXCEED.EmployeeAccident", "Description");
            DropColumn("FleetManagement.Accident", "RenewalDateAmh");
            DropColumn("FleetManagement.Accident", "CurrentValue");
            DropColumn("FleetManagement.Accident", "PolicyIdNo");
        }
    }
}
