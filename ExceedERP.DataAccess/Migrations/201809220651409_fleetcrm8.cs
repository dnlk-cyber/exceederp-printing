namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FleetManagement.LicenseLevel",
                c => new
                    {
                        LicenseLevelId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LicenseLevelId);
            
            AlterColumn("FleetManagement.Operator", "GuaranteeName", c => c.String());
            AlterColumn("FleetManagement.Operator", "GuaranteeAddress", c => c.String());
            AlterColumn("FleetManagement.Operator", "GuaranteePhoneNumber", c => c.String());
            AlterColumn("Budget.BudgetTransfer", "FromWorkUnit", c => c.String());
            AlterColumn("Budget.BudgetTransfer", "ToWorkUnit", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Budget.BudgetTransfer", "ToWorkUnit", c => c.String(nullable: false));
            AlterColumn("Budget.BudgetTransfer", "FromWorkUnit", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Operator", "GuaranteePhoneNumber", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Operator", "GuaranteeAddress", c => c.String(nullable: false));
            AlterColumn("FleetManagement.Operator", "GuaranteeName", c => c.String(nullable: false));
            DropTable("FleetManagement.LicenseLevel");
        }
    }
}
