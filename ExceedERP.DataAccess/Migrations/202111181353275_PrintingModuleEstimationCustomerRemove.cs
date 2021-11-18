namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintingModuleEstimationCustomerRemove : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("PrintingEstimation.EstimationForms", "CustomerId", "PrintingEstimation.Customers");
            DropForeignKey("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId", "HRM.EmployeeTypes");
            DropIndex("PrintingEstimation.EstimationForms", new[] { "CustomerId" });
            DropIndex("JobCosting.DailyLaborRates", new[] { "EmployeeType_EmployeeTypeId" });
            AddColumn("JobCosting.DailyLaborRates", "EmployeeType", c => c.Int(nullable: false));
            DropColumn("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId");
            DropTable("PrintingEstimation.Customers");
        }
        
        public override void Down()
        {
            CreateTable(
                "PrintingEstimation.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ContactName = c.String(),
                        Phone = c.String(),
                        City = c.String(),
                        SubCity = c.String(),
                        Woreda = c.String(),
                        Pobox = c.String(),
                        VAT = c.Decimal(precision: 18, scale: 2),
                        Margin = c.Decimal(precision: 18, scale: 2),
                        OverHead = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            AddColumn("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId", c => c.Int());
            DropColumn("JobCosting.DailyLaborRates", "EmployeeType");
            CreateIndex("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId");
            CreateIndex("PrintingEstimation.EstimationForms", "CustomerId");
            AddForeignKey("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId", "HRM.EmployeeTypes", "EmployeeTypeId");
            AddForeignKey("PrintingEstimation.EstimationForms", "CustomerId", "PrintingEstimation.Customers", "CustomerId", cascadeDelete: true);
        }
    }
}
