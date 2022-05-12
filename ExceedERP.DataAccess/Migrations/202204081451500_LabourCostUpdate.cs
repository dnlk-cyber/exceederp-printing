namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LabourCostUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("JobCosting.JobLaborCosts", "PermanetLaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("JobCosting.JobLaborCosts", "PermanetOverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("JobCosting.JobLaborCosts", "ContractLaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("JobCosting.JobLaborCosts", "ContractOverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("JobCosting.JobLaborCosts", "ContractOverHeadCost");
            DropColumn("JobCosting.JobLaborCosts", "ContractLaborCost");
            DropColumn("JobCosting.JobLaborCosts", "PermanetOverHeadCost");
            DropColumn("JobCosting.JobLaborCosts", "PermanetLaborCost");
        }
    }
}
