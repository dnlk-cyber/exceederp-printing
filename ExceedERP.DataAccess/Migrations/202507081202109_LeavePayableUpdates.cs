namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeavePayableUpdates : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "EXCEED.LeavePayableDetail", newName: "LeavePayableDetails");
            RenameTable(name: "EXCEED.LeavePayable", newName: "LeavePayables");
            RenameTable(name: "EXCEED.LeavePayableSetting", newName: "LeavePayableSettings");
            RenameTable(name: "EXCEED.SeverancePayable", newName: "SeverancePayables");
            MoveTable(name: "EXCEED.LeavePayableDetails", newSchema: "HRM");
            MoveTable(name: "EXCEED.LeavePayables", newSchema: "HRM");
            MoveTable(name: "EXCEED.LeavePayableSettings", newSchema: "HRM");
            MoveTable(name: "EXCEED.SeverancePayableDetail", newSchema: "HRM");
            MoveTable(name: "EXCEED.SeverancePayables", newSchema: "HRM");
            CreateTable(
                "HRM.SeverancePayableSettings",
                c => new
                    {
                        SeverancePayableSettingId = c.Int(nullable: false, identity: true),
                        ResponsibilityLevel = c.Int(nullable: false),
                        SalaryValue = c.Decimal(nullable: false, precision: 18, scale: 6),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeverancePayableSettingId);
            
            CreateTable(
                "HRM.SeveranceWithProbabilities",
                c => new
                    {
                        SeveranceWithProbabilityId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        EmployeeCode = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        DateOfBirthAm = c.String(),
                        Gender = c.Int(nullable: false),
                        HiredDate = c.DateTime(nullable: false),
                        HiredDateAmharic = c.String(),
                        NormalRetirmentAge = c.DateTime(nullable: false),
                        NormalRetirmentAgeAm = c.String(),
                        Age = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 6),
                        YearWorked = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingNoOfYears = c.Decimal(nullable: false, precision: 18, scale: 6),
                        ProbabilityOfTurnover = c.Decimal(nullable: false, precision: 18, scale: 6),
                        SalaryUpOnRetirement = c.Decimal(nullable: false, precision: 18, scale: 6),
                        PVOfSalaryUponRetirement = c.Decimal(nullable: false, precision: 18, scale: 6),
                        MaxAllowedPeriod = c.Decimal(nullable: false, precision: 18, scale: 6),
                        MaxEntitelementAmount = c.Decimal(nullable: false, precision: 18, scale: 6),
                        EV1 = c.Decimal(nullable: false, precision: 18, scale: 6),
                        MortalityRate = c.Decimal(nullable: false, precision: 18, scale: 6),
                        EV2 = c.Decimal(nullable: false, precision: 18, scale: 6),
                        TotalEV = c.Decimal(nullable: false, precision: 18, scale: 6),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeveranceWithProbabilityId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.LeavePayableWithDetailFiscalYears",
                c => new
                    {
                        LeavePayableWithDetailFiscalYearId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        PayableDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LeavePayableDetailId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LeavePayableWithDetailFiscalYearId)
                .ForeignKey("HRM.LeavePayableDetails", t => t.LeavePayableDetailId, cascadeDelete: true)
                .Index(t => t.LeavePayableDetailId);
            
            AddColumn("HRM.LeavePayableDetails", "AnnualLeavePerMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableDetails", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableDetails", "AgregateTaxableAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableDetails", "AgregateIncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableDetails", "BasicSalaryIncomeTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableDetails", "TaxDifference", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableDetails", "NetPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableDetails", "Remark", c => c.String());
            AddColumn("HRM.SeverancePayableDetail", "ServicePeriod", c => c.String());
            AddColumn("HRM.SeverancePayableDetail", "TaxOnBasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "Years", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "Months", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "Days", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "TotalServiceMonths", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "GrossCompensationPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "CompensationPaymentT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "TaxOnMonthlySalaryAndCompensation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "TaxDifference", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "AccTaxDifference", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.SeverancePayableDetail", "Remark", c => c.String());
            AddColumn("HRM.SeverancePayableDetail", "IsSendForPayment", c => c.Boolean(nullable: false));
            AddColumn("HRM.SeverancePayableDetail", "SendBy", c => c.String());
            AddColumn("HRM.SeverancePayableDetail", "DateSent", c => c.DateTime());
            AddColumn("HRM.SeverancePayableDetail", "PaymentRequestId", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.LeavePayableWithDetailFiscalYears", "LeavePayableDetailId", "HRM.LeavePayableDetails");
            DropForeignKey("HRM.SeveranceWithProbabilities", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.SeveranceWithProbabilities", "EmployeeId", "HRM.Employee");
            DropIndex("HRM.LeavePayableWithDetailFiscalYears", new[] { "LeavePayableDetailId" });
            DropIndex("HRM.SeveranceWithProbabilities", new[] { "EmployeeId" });
            DropIndex("HRM.SeveranceWithProbabilities", new[] { "GlFiscalYearId" });
            DropColumn("HRM.SeverancePayableDetail", "PaymentRequestId");
            DropColumn("HRM.SeverancePayableDetail", "DateSent");
            DropColumn("HRM.SeverancePayableDetail", "SendBy");
            DropColumn("HRM.SeverancePayableDetail", "IsSendForPayment");
            DropColumn("HRM.SeverancePayableDetail", "Remark");
            DropColumn("HRM.SeverancePayableDetail", "AccTaxDifference");
            DropColumn("HRM.SeverancePayableDetail", "TaxDifference");
            DropColumn("HRM.SeverancePayableDetail", "TaxOnMonthlySalaryAndCompensation");
            DropColumn("HRM.SeverancePayableDetail", "CompensationPaymentT");
            DropColumn("HRM.SeverancePayableDetail", "GrossCompensationPayment");
            DropColumn("HRM.SeverancePayableDetail", "TotalServiceMonths");
            DropColumn("HRM.SeverancePayableDetail", "Days");
            DropColumn("HRM.SeverancePayableDetail", "Months");
            DropColumn("HRM.SeverancePayableDetail", "Years");
            DropColumn("HRM.SeverancePayableDetail", "TaxOnBasicSalary");
            DropColumn("HRM.SeverancePayableDetail", "ServicePeriod");
            DropColumn("HRM.LeavePayableDetails", "Remark");
            DropColumn("HRM.LeavePayableDetails", "NetPayment");
            DropColumn("HRM.LeavePayableDetails", "TaxDifference");
            DropColumn("HRM.LeavePayableDetails", "BasicSalaryIncomeTax");
            DropColumn("HRM.LeavePayableDetails", "AgregateIncomeTax");
            DropColumn("HRM.LeavePayableDetails", "AgregateTaxableAmount");
            DropColumn("HRM.LeavePayableDetails", "BasicSalary");
            DropColumn("HRM.LeavePayableDetails", "AnnualLeavePerMonth");
            DropTable("HRM.LeavePayableWithDetailFiscalYears");
            DropTable("HRM.SeveranceWithProbabilities");
            DropTable("HRM.SeverancePayableSettings");
            MoveTable(name: "HRM.SeverancePayables", newSchema: "EXCEED");
            MoveTable(name: "HRM.SeverancePayableDetail", newSchema: "EXCEED");
            MoveTable(name: "HRM.LeavePayableSettings", newSchema: "EXCEED");
            MoveTable(name: "HRM.LeavePayables", newSchema: "EXCEED");
            MoveTable(name: "HRM.LeavePayableDetails", newSchema: "EXCEED");
            RenameTable(name: "EXCEED.SeverancePayables", newName: "SeverancePayable");
            RenameTable(name: "EXCEED.LeavePayableSettings", newName: "LeavePayableSetting");
            RenameTable(name: "EXCEED.LeavePayables", newName: "LeavePayable");
            RenameTable(name: "EXCEED.LeavePayableDetails", newName: "LeavePayableDetail");
        }
    }
}
