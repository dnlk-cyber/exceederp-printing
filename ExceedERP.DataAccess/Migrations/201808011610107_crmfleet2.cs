namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmfleet2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter");
            DropIndex("Budget.BudgetPlan", new[] { "GLChartOfAccountCostCenter_Values" });
            CreateTable(
                "Budget.BudgetRequestLine",
                c => new
                    {
                        BudgetRequestLineId = c.Int(nullable: false, identity: true),
                        BudgetRequestId = c.Int(nullable: false),
                        CalculateAuto = c.Boolean(nullable: false),
                        BudgetRequestType = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        PurchasedRequestUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchasedRequestUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentTotalHr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoWorkingDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalaryPerMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ResponsibleAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DisturbanceAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProffiessionalAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(),
                        FromMonth = c.String(),
                        ToMonth = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reason = c.String(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetRequestLineId)
                .ForeignKey("Budget.BudgetRequest", t => t.BudgetRequestId, cascadeDelete: true)
                .Index(t => t.BudgetRequestId);
            
            AddColumn("FleetManagement.Accident", "Insured", c => c.String());
            AddColumn("FleetManagement.Accident", "FullName", c => c.String());
            AddColumn("FleetManagement.Accident", "Address", c => c.String());
            AddColumn("FleetManagement.Accident", "Occupation", c => c.String());
            AddColumn("FleetManagement.Accident", "PhoneNo", c => c.String());
            AddColumn("FleetManagement.Accident", "PolicyNo", c => c.String());
            AddColumn("FleetManagement.Accident", "RenewalDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.Accident", "DoYouHaveMoreThanOneInsurance", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Accident", "DoYouHaveOtherInsuranceCanEarnForTheAccident", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Accident", "DoThePoliceRecoredIt", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Accident", "WitnessInYourCar", c => c.String());
            AddColumn("FleetManagement.Accident", "WitnessOutOfYourCar", c => c.String());
            AddColumn("FleetManagement.Accident", "DescriptionForWhyNotTakeWitness", c => c.String());
            AddColumn("FleetManagement.Accident", "OpionionResponsible", c => c.String());
            AddColumn("FleetManagement.Accident", "PoliceIdNo", c => c.String());
            AddColumn("FleetManagement.Accident", "WittnessDescription", c => c.String());
            AddColumn("FleetManagement.Accident", "AssetDamage", c => c.String());
            AddColumn("FleetManagement.Accident", "ThirdPartyDamage", c => c.String());
            AddColumn("FleetManagement.Accident", "InjuredPersons", c => c.String());
            AddColumn("Budget.BudgetPlan", "BranchId", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetPlan", "WorkUnitName", c => c.String());
            AddColumn("Budget.BudgetRequest", "BranchId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetInsuranceTypeDetail", "InsuranceId", c => c.Guid(nullable: false));
            AlterColumn("Budget.BudgetRequest", "WorkUnit", c => c.String());
            CreateIndex("Budget.BudgetPlan", "BranchId");
            CreateIndex("Budget.BudgetRequest", "BranchId");
            AddForeignKey("Budget.BudgetPlan", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            AddForeignKey("Budget.BudgetRequest", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            DropColumn("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values");
            DropColumn("Budget.BudgetRequest", "AccountCode");
            DropColumn("Budget.BudgetRequest", "Branch");
        }
        
        public override void Down()
        {
            AddColumn("Budget.BudgetRequest", "Branch", c => c.Int(nullable: false));
            AddColumn("Budget.BudgetRequest", "AccountCode", c => c.String());
            AddColumn("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values", c => c.String(maxLength: 128));
            DropForeignKey("Budget.BudgetRequestLine", "BudgetRequestId", "Budget.BudgetRequest");
            DropForeignKey("Budget.BudgetRequest", "BranchId", "HRM.Branch");
            DropForeignKey("Budget.BudgetPlan", "BranchId", "HRM.Branch");
            DropIndex("Budget.BudgetRequestLine", new[] { "BudgetRequestId" });
            DropIndex("Budget.BudgetRequest", new[] { "BranchId" });
            DropIndex("Budget.BudgetPlan", new[] { "BranchId" });
            AlterColumn("Budget.BudgetRequest", "WorkUnit", c => c.Int(nullable: false));
            DropColumn("FleetManagement.FleetInsuranceTypeDetail", "InsuranceId");
            DropColumn("Budget.BudgetRequest", "BranchId");
            DropColumn("Budget.BudgetPlan", "WorkUnitName");
            DropColumn("Budget.BudgetPlan", "BranchId");
            DropColumn("FleetManagement.Accident", "InjuredPersons");
            DropColumn("FleetManagement.Accident", "ThirdPartyDamage");
            DropColumn("FleetManagement.Accident", "AssetDamage");
            DropColumn("FleetManagement.Accident", "WittnessDescription");
            DropColumn("FleetManagement.Accident", "PoliceIdNo");
            DropColumn("FleetManagement.Accident", "OpionionResponsible");
            DropColumn("FleetManagement.Accident", "DescriptionForWhyNotTakeWitness");
            DropColumn("FleetManagement.Accident", "WitnessOutOfYourCar");
            DropColumn("FleetManagement.Accident", "WitnessInYourCar");
            DropColumn("FleetManagement.Accident", "DoThePoliceRecoredIt");
            DropColumn("FleetManagement.Accident", "DoYouHaveOtherInsuranceCanEarnForTheAccident");
            DropColumn("FleetManagement.Accident", "DoYouHaveMoreThanOneInsurance");
            DropColumn("FleetManagement.Accident", "RenewalDate");
            DropColumn("FleetManagement.Accident", "PolicyNo");
            DropColumn("FleetManagement.Accident", "PhoneNo");
            DropColumn("FleetManagement.Accident", "Occupation");
            DropColumn("FleetManagement.Accident", "Address");
            DropColumn("FleetManagement.Accident", "FullName");
            DropColumn("FleetManagement.Accident", "Insured");
            DropTable("Budget.BudgetRequestLine");
            CreateIndex("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values");
            AddForeignKey("Budget.BudgetPlan", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter", "Values");
        }
    }
}
