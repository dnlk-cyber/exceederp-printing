namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm25 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Insurance.Insurance", "InsuranceGroupId", "Insurance.InsuranceGroup");
            DropIndex("Insurance.Insurance", new[] { "InsuranceGroupId" });
            AddColumn("FleetManagement.FleetInsurance", "InsuranceTypeId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetInsurance", "InsuranceCategoryId", c => c.Int(nullable: false));
            AddColumn("Insurance.Insurance", "InsuranceTypeId", c => c.Int(nullable: false));
            AddColumn("Insurance.Insurance", "InsuranceCategoryId", c => c.Int(nullable: false));
            DropColumn("Insurance.Insurance", "InsuranceGroupId");
        }
        
        public override void Down()
        {
            AddColumn("Insurance.Insurance", "InsuranceGroupId", c => c.Int(nullable: false));
            DropColumn("Insurance.Insurance", "InsuranceCategoryId");
            DropColumn("Insurance.Insurance", "InsuranceTypeId");
            DropColumn("FleetManagement.FleetInsurance", "InsuranceCategoryId");
            DropColumn("FleetManagement.FleetInsurance", "InsuranceTypeId");
            CreateIndex("Insurance.Insurance", "InsuranceGroupId");
            AddForeignKey("Insurance.Insurance", "InsuranceGroupId", "Insurance.InsuranceGroup", "InsuranceGroupId", cascadeDelete: true);
        }
    }
}
