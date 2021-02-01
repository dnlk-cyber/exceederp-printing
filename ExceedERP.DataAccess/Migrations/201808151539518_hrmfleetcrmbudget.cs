namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmfleetcrmbudget : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.EmployeeAccident",
                c => new
                    {
                        EmployeeAccidentId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        IsAtWorkPlace = c.Boolean(nullable: false),
                        IsOnDuety = c.Boolean(nullable: false),
                        Witness = c.String(),
                        AccidentDate = c.DateTime(),
                        AccidentTime = c.DateTime(),
                        LevelOfInjuired = c.String(),
                        AccidentDateAmh = c.String(),
                        CreatedBy = c.String(),
                        LastModifiedBy = c.String(),
                        IsSendForApproval = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeAccidentId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "EquipmentName", c => c.String());
            AddColumn("Budget.PlanCategory", "GrandTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.BudgetTransfer", "FromBranch", c => c.String());
            AddColumn("Budget.BudgetTransfer", "ToBranch", c => c.String());
            AddColumn("FleetManagement.RentOtherSuppliers", "WithDriver", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentOtherSuppliers", "GoodCondition", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentOtherSuppliers", "NormalCondition", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentOtherSuppliers", "BadCondition", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentOtherSuppliers", "Address", c => c.String());
            AddColumn("FleetManagement.RentOtherSuppliers", "EquipmentName", c => c.String(nullable: false));
            AddColumn("FleetManagement.RentOtherSuppliers", "Condition", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.EmployeeAccident", "EmployeeId", "HRM.Employee");
            DropIndex("EXCEED.EmployeeAccident", new[] { "EmployeeId" });
            DropColumn("FleetManagement.RentOtherSuppliers", "Condition");
            DropColumn("FleetManagement.RentOtherSuppliers", "EquipmentName");
            DropColumn("FleetManagement.RentOtherSuppliers", "Address");
            DropColumn("FleetManagement.RentOtherSuppliers", "BadCondition");
            DropColumn("FleetManagement.RentOtherSuppliers", "NormalCondition");
            DropColumn("FleetManagement.RentOtherSuppliers", "GoodCondition");
            DropColumn("FleetManagement.RentOtherSuppliers", "WithDriver");
            DropColumn("Budget.BudgetTransfer", "ToBranch");
            DropColumn("Budget.BudgetTransfer", "FromBranch");
            DropColumn("Budget.PlanCategory", "GrandTotal");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "EquipmentName");
            DropTable("EXCEED.EmployeeAccident");
        }
    }
}
