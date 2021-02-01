namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adwavictory : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "Archive.LetterSender", newSchema: "EXCEED");
            DropForeignKey("Construction.ProjectBudget", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectBudgetRequest", "ProjectBudgetId", "Construction.ProjectBudget");
            DropForeignKey("Construction.ProjectBudgetTransfer", "ProjectBudgetId", "Construction.ProjectBudget");
            DropForeignKey("Construction.ProjectBudgetUtilization", "ProjectBudgetId", "Construction.ProjectBudget");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "LookupId", "common.Lookup");
            DropForeignKey("Construction.ProjectEstimnWorkItem", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectTakeOff", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectTakeOffLine", "ProjectTakeOff_ProjectTakeOffId", "Construction.ProjectTakeOff");
            DropIndex("Construction.ProjectBudget", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectBudgetRequest", new[] { "ProjectBudgetId" });
            DropIndex("Construction.ProjectBudgetTransfer", new[] { "ProjectBudgetId" });
            DropIndex("Construction.ProjectBudgetUtilization", new[] { "ProjectBudgetId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "LookupId" });
            DropIndex("Construction.ProjectEstimnWorkItem", new[] { "ProjectEstimationNameId" });
            DropIndex("Construction.ProjectTakeOff", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.ProjectTakeOffLine", new[] { "ProjectTakeOff_ProjectTakeOffId" });
            RenameColumn(table: "Construction.ProjectTakeOffLine", name: "ProjectTakeOff_ProjectTakeOffId", newName: "ProjectTakeOffId");
            CreateTable(
                "Construction.InternalMachinaryRent",
                c => new
                    {
                        InternalMachinaryRentId = c.Int(nullable: false, identity: true),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        PurchaseCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaximumLife = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DepCostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyRentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InternalMachinaryRentId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "EXCEED.GanttDependency",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PredecessorID = c.Int(nullable: false),
                        SuccessorID = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "EXCEED.GanttResourceAssignment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TaskID = c.Int(nullable: false),
                        ResourceID = c.Int(nullable: false),
                        Units = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "EXCEED.GanttResource",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Color = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "Construction.MachinaryRent",
                c => new
                    {
                        MachinaryRentId = c.Int(nullable: false, identity: true),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        Model = c.String(maxLength: 200),
                        YearManufacture = c.String(maxLength: 50),
                        Rental = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BirrPerLitter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCostPerHr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalWithFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.MachinaryRentId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Construction.MaterialUtilization",
                c => new
                    {
                        MaterialUtilizationId = c.Int(nullable: false, identity: true),
                        MaterialType = c.String(),
                        RequestedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveredQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalDemand = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialIssuedToSite = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizationOfItem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialWastage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        ProjectInformationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaterialUtilizationId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.WorkCategory",
                c => new
                    {
                        WorkCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        StructureType = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkCategoryId);
            
            CreateTable(
                "Construction.ProjectEstimationBoqDetail",
                c => new
                    {
                        ProjectEstmnBoqDetailId = c.Int(nullable: false, identity: true),
                        ProjectEstimationBoqBreakdownId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                        LookupId = c.Int(nullable: false),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quntity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstmnBoqDetailId)
                .ForeignKey("Construction.ProjectEstimnBoqBreakdown", t => t.ProjectEstimationBoqBreakdownId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationBoqBreakdownId);
            
            CreateTable(
                "Construction.ProjectEstmnBoqSummary",
                c => new
                    {
                        ProjectEstmnBoqSummaryId = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        Category = c.String(),
                        Unit = c.String(),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        ProjectEstimationNameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectEstmnBoqSummaryId)
                .ForeignKey("Construction.ProjectEstimationName", t => t.ProjectEstimationNameId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationNameId);
            
            CreateTable(
                "Construction.ProjectMachineryRentHeader",
                c => new
                    {
                        ProjectEstimationMachineryRentId = c.Int(nullable: false, identity: true),
                        FuelPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelSupply = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelDistribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        ProjectEstimationNameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectEstimationMachineryRentId)
                .ForeignKey("Construction.ProjectEstimationName", t => t.ProjectEstimationNameId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationNameId);
            
            CreateTable(
                "Construction.ProjectTakeOffLineForKg",
                c => new
                    {
                        ProjectTakeOffLineForKgId = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 200),
                        Type = c.String(),
                        Shape = c.String(),
                        UnitRateId = c.Int(nullable: false),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoOfBars = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoOfElement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalBar = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalLength = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectTakeOffId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectTakeOffLineForKgId)
                .ForeignKey("Construction.ProjectTakeOff", t => t.ProjectTakeOffId, cascadeDelete: true)
                .ForeignKey("Construction.UnitRate", t => t.UnitRateId, cascadeDelete: true)
                .Index(t => t.UnitRateId)
                .Index(t => t.ProjectTakeOffId);
            
            CreateTable(
                "Construction.UnitRate",
                c => new
                    {
                        UnitRateId = c.Int(nullable: false, identity: true),
                        Diameter = c.String(),
                        UnitWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.UnitRateId);
            
            CreateTable(
                "EXCEED.ProjectTaskSchedule",
                c => new
                    {
                        ProjectTaskScheduleId = c.Int(nullable: false, identity: true),
                        TaskName = c.String(maxLength: 100),
                        Duration = c.String(maxLength: 50),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CompletePercentage = c.String(maxLength: 50),
                        ProjectInformationId = c.Int(nullable: false),
                        RecurrenceId = c.Int(nullable: false),
                        StartTimezone = c.String(),
                        EndTimezone = c.String(),
                        Description = c.String(),
                        IsAllDay = c.Boolean(nullable: false),
                        RecurrenceException = c.String(),
                        RecurrenceRule = c.String(),
                    })
                .PrimaryKey(t => t.ProjectTaskScheduleId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Archive.RecivedLetters",
                c => new
                    {
                        RecivedLetterId = c.Int(nullable: false, identity: true),
                        RecivedDate = c.DateTime(nullable: false),
                        InstnPerson = c.String(),
                        Receiver = c.String(),
                        Sender = c.String(),
                        Description = c.String(),
                        FileName = c.String(),
                        LetterStatus = c.Int(nullable: false),
                        BookNo = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RecivedLetterId)
                .ForeignKey("Archive.IncommingLetter", t => t.BookNo)
                .Index(t => t.BookNo);
            
            CreateTable(
                "Schedule.Schedule",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ParentID = c.Int(),
                        OrderID = c.Int(nullable: false),
                        Title = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        PercentComplete = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Expanded = c.Boolean(nullable: false),
                        Summary = c.Boolean(nullable: false),
                        ProjectCode = c.String(),
                        ProjectInformationId = c.Int(nullable: false),
                        GanttTask1_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("Schedule.Schedule", t => t.GanttTask1_ID)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.GanttTask1_ID);
            
            AddColumn("Construction.ProjectPaymenAdvance", "ProjectInformationId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectBudgetRequest", "ProjectDefineBudgetId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectBudgetTransfer", "ProjectDefineBudgetId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectBudgetUtilization", "ProjectDefineBudgetId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectHumanResourceUtilization", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("Construction.PaymenCertificatetLine", "ProjectInformationId", c => c.Int(nullable: false));
            AddColumn("Construction.Retention", "ProjectInformationId", c => c.Int(nullable: false));
            AddColumn("EXCEED.LetterSender", "Sender", c => c.String());
            AddColumn("EXCEED.LetterSender", "BookNo", c => c.String(maxLength: 128));
            AddColumn("Payroll.OverTimeTransfer", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.PayrollDistribution", "BranchId", c => c.Int(nullable: false));
            AddColumn("Payroll.PayrollProcessViewModel", "BranchId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimationName", "LookupId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationWorkItemId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "Unit", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "StructureType", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "WorkCategoryId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "Category", c => c.String());
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "Type", c => c.String());
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "Index", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnWorkItem", "WorkCategoryId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnWorkItem", "Unit", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnWorkItem", "OverheadRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimnWorkItem", "ProfitMargin", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimnWorkItem", "ParentId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTakeOff", "ProjectEstimationWorkItemId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTakeOff", "ProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTakeOff", "Remark", c => c.String());
            AddColumn("Construction.ProjectTakeOffLine", "LookupId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTakeOffLine", "Length", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectTakeOffLine", "Width", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectTakeOffLine", "Height", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectTakeOffLine", "T", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectTakeOffLine", "D", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectTakeOffLine", "S", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectTakeOffLine", "Description", c => c.String(maxLength: 400));
            AlterColumn("Construction.ProjectTakeOffLine", "ProjectTakeOffId", c => c.Int(nullable: false));
            CreateIndex("Construction.ProjectPaymenAdvance", "ProjectInformationId");
            CreateIndex("Construction.ProjectBudgetRequest", "ProjectDefineBudgetId");
            CreateIndex("Construction.ProjectBudgetTransfer", "ProjectDefineBudgetId");
            CreateIndex("Construction.ProjectBudgetUtilization", "ProjectDefineBudgetId");
            CreateIndex("Construction.ProjectHumanResourceUtilization", "EmployeeId");
            CreateIndex("EXCEED.LetterSender", "BookNo");
            CreateIndex("Construction.PaymenCertificatetLine", "ProjectInformationId");
            CreateIndex("Payroll.OverTimeTransfer", "PeriodId");
            CreateIndex("Construction.Retention", "ProjectInformationId");
            CreateIndex("Payroll.PayrollDistribution", "BranchId");
            CreateIndex("Payroll.PayrollProcessViewModel", "BranchId");
            CreateIndex("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId");
            CreateIndex("Construction.ProjectEstimnBoqBreakdown", "WorkCategoryId");
            CreateIndex("Construction.ProjectEstimnWorkItem", "WorkCategoryId");
            CreateIndex("Construction.ProjectTakeOffLine", "ProjectTakeOffId");
            AddForeignKey("Construction.ProjectBudgetRequest", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget", "ProjectDefineBudgetId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectBudgetTransfer", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget", "ProjectDefineBudgetId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectBudgetUtilization", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget", "ProjectDefineBudgetId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectHumanResourceUtilization", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectPaymenAdvance", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("EXCEED.LetterSender", "BookNo", "Archive.IncommingLetter", "BookNo");
            AddForeignKey("Construction.PaymenCertificatetLine", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Payroll.OverTimeTransfer", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Construction.Retention", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Payroll.PayrollDistribution", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            AddForeignKey("Payroll.PayrollProcessViewModel", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId", "Construction.ProjectEstimationName", "ProjectEstimationNameId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimnBoqBreakdown", "WorkCategoryId", "Construction.WorkCategory", "WorkCategoryId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimnWorkItem", "WorkCategoryId", "Construction.WorkCategory", "WorkCategoryId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectTakeOffLine", "ProjectTakeOffId", "Construction.ProjectTakeOff", "ProjectTakeOffId", cascadeDelete: true);
            DropColumn("Construction.ProjectBudgetRequest", "ProjectBudgetId");
            DropColumn("Construction.ProjectBudgetTransfer", "ProjectBudgetId");
            DropColumn("Construction.ProjectBudgetUtilization", "ProjectBudgetId");
            DropColumn("Construction.ProjectHumanResourceUtilization", "CrewLeader");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationBoqid");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "ItemNo");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "LookupId");
            DropColumn("Construction.ProjectEstimnWorkItem", "ProjectType");
            DropColumn("Construction.ProjectEstimnWorkItem", "ProjectEstimationNameId");
            DropColumn("Construction.ProjectTakeOff", "Project_ProjectInformationId");
            DropTable("Construction.ProjectBudget");
            DropTable("Construction.ProjectEstmnMachineryRent");
            DropTable("Construction.ProjectEstmnUnitRate");
        }
        
        public override void Down()
        {
            CreateTable(
                "Construction.ProjectEstmnUnitRate",
                c => new
                    {
                        ProjectEstimationUnitRateId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        ItemName = c.String(nullable: false, maxLength: 200),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Overheard = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Profit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationUnitRateId);
            
            CreateTable(
                "Construction.ProjectEstmnMachineryRent",
                c => new
                    {
                        ProjectEstimationMachineryRentId = c.Int(nullable: false, identity: true),
                        FuelPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelSupply = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelDistribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemNo = c.String(nullable: false, maxLength: 100),
                        TypeEquipment = c.Int(nullable: false),
                        Model = c.String(nullable: false, maxLength: 200),
                        YearManufacture = c.String(nullable: false, maxLength: 50),
                        PowerOutput = c.String(nullable: false, maxLength: 200),
                        WithoutFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProjectEstimationMachineryRentId);
            
            CreateTable(
                "Construction.ProjectBudget",
                c => new
                    {
                        ProjectBudgetId = c.Int(nullable: false, identity: true),
                        Project_ProjectInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectBudgetId);
            
            AddColumn("Construction.ProjectTakeOff", "Project_ProjectInformationId", c => c.Int());
            AddColumn("Construction.ProjectEstimnWorkItem", "ProjectEstimationNameId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnWorkItem", "ProjectType", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "LookupId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "ItemNo", c => c.String(nullable: false, maxLength: 100));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationBoqid", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectHumanResourceUtilization", "CrewLeader", c => c.String(nullable: false, maxLength: 200));
            AddColumn("Construction.ProjectBudgetUtilization", "ProjectBudgetId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectBudgetTransfer", "ProjectBudgetId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectBudgetRequest", "ProjectBudgetId", c => c.Int(nullable: false));
            DropForeignKey("Construction.ProjectTakeOffLine", "ProjectTakeOffId", "Construction.ProjectTakeOff");
            DropForeignKey("Schedule.Schedule", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Schedule.Schedule", "GanttTask1_ID", "Schedule.Schedule");
            DropForeignKey("Archive.RecivedLetters", "BookNo", "Archive.IncommingLetter");
            DropForeignKey("EXCEED.ProjectTaskSchedule", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectTakeOffLineForKg", "UnitRateId", "Construction.UnitRate");
            DropForeignKey("Construction.ProjectTakeOffLineForKg", "ProjectTakeOffId", "Construction.ProjectTakeOff");
            DropForeignKey("Construction.ProjectMachineryRentHeader", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstmnBoqSummary", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Construction.ProjectEstimnWorkItem", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.ProjectEstimationBoqDetail", "ProjectEstimationBoqBreakdownId", "Construction.ProjectEstimnBoqBreakdown");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId", "Construction.ProjectEstimationName");
            DropForeignKey("Payroll.PayrollProcessViewModel", "BranchId", "HRM.Branch");
            DropForeignKey("Payroll.PayrollDistribution", "BranchId", "HRM.Branch");
            DropForeignKey("Construction.Retention", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Payroll.OverTimeTransfer", "PeriodId", "common.Period");
            DropForeignKey("Construction.MaterialUtilization", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.MachinaryRent", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.PaymenCertificatetLine", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("EXCEED.LetterSender", "BookNo", "Archive.IncommingLetter");
            DropForeignKey("Construction.InternalMachinaryRent", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectPaymenAdvance", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectHumanResourceUtilization", "EmployeeId", "HRM.Employee");
            DropForeignKey("Construction.ProjectBudgetUtilization", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget");
            DropForeignKey("Construction.ProjectBudgetTransfer", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget");
            DropForeignKey("Construction.ProjectBudgetRequest", "ProjectDefineBudgetId", "Construction.ProjectDefineBudget");
            DropIndex("Schedule.Schedule", new[] { "GanttTask1_ID" });
            DropIndex("Schedule.Schedule", new[] { "ProjectInformationId" });
            DropIndex("Archive.RecivedLetters", new[] { "BookNo" });
            DropIndex("EXCEED.ProjectTaskSchedule", new[] { "ProjectInformationId" });
            DropIndex("Construction.ProjectTakeOffLineForKg", new[] { "ProjectTakeOffId" });
            DropIndex("Construction.ProjectTakeOffLineForKg", new[] { "UnitRateId" });
            DropIndex("Construction.ProjectTakeOffLine", new[] { "ProjectTakeOffId" });
            DropIndex("Construction.ProjectMachineryRentHeader", new[] { "ProjectEstimationNameId" });
            DropIndex("Construction.ProjectEstmnBoqSummary", new[] { "ProjectEstimationNameId" });
            DropIndex("Construction.ProjectEstimnWorkItem", new[] { "WorkCategoryId" });
            DropIndex("Construction.ProjectEstimationBoqDetail", new[] { "ProjectEstimationBoqBreakdownId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "WorkCategoryId" });
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "ProjectEstimationNameId" });
            DropIndex("Payroll.PayrollProcessViewModel", new[] { "BranchId" });
            DropIndex("Payroll.PayrollDistribution", new[] { "BranchId" });
            DropIndex("Construction.Retention", new[] { "ProjectInformationId" });
            DropIndex("Payroll.OverTimeTransfer", new[] { "PeriodId" });
            DropIndex("Construction.MaterialUtilization", new[] { "ProjectInformationId" });
            DropIndex("Construction.MachinaryRent", new[] { "ItemCode" });
            DropIndex("Construction.PaymenCertificatetLine", new[] { "ProjectInformationId" });
            DropIndex("EXCEED.LetterSender", new[] { "BookNo" });
            DropIndex("Construction.InternalMachinaryRent", new[] { "ItemCode" });
            DropIndex("Construction.ProjectHumanResourceUtilization", new[] { "EmployeeId" });
            DropIndex("Construction.ProjectBudgetUtilization", new[] { "ProjectDefineBudgetId" });
            DropIndex("Construction.ProjectBudgetTransfer", new[] { "ProjectDefineBudgetId" });
            DropIndex("Construction.ProjectBudgetRequest", new[] { "ProjectDefineBudgetId" });
            DropIndex("Construction.ProjectPaymenAdvance", new[] { "ProjectInformationId" });
            AlterColumn("Construction.ProjectTakeOffLine", "ProjectTakeOffId", c => c.Int());
            AlterColumn("Construction.ProjectTakeOffLine", "Description", c => c.String(maxLength: 200));
            AlterColumn("Construction.ProjectTakeOffLine", "S", c => c.String(maxLength: 100));
            AlterColumn("Construction.ProjectTakeOffLine", "D", c => c.String(maxLength: 100));
            AlterColumn("Construction.ProjectTakeOffLine", "T", c => c.String(maxLength: 100));
            DropColumn("Construction.ProjectTakeOffLine", "Height");
            DropColumn("Construction.ProjectTakeOffLine", "Width");
            DropColumn("Construction.ProjectTakeOffLine", "Length");
            DropColumn("Construction.ProjectTakeOffLine", "LookupId");
            DropColumn("Construction.ProjectTakeOff", "Remark");
            DropColumn("Construction.ProjectTakeOff", "ProjectId");
            DropColumn("Construction.ProjectTakeOff", "ProjectEstimationWorkItemId");
            DropColumn("Construction.ProjectEstimnWorkItem", "ParentId");
            DropColumn("Construction.ProjectEstimnWorkItem", "ProfitMargin");
            DropColumn("Construction.ProjectEstimnWorkItem", "OverheadRate");
            DropColumn("Construction.ProjectEstimnWorkItem", "Unit");
            DropColumn("Construction.ProjectEstimnWorkItem", "WorkCategoryId");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "Index");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "Type");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "Category");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "WorkCategoryId");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "StructureType");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "Unit");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationWorkItemId");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "ProjectEstimationNameId");
            DropColumn("Construction.ProjectEstimationName", "LookupId");
            DropColumn("Payroll.PayrollProcessViewModel", "BranchId");
            DropColumn("Payroll.PayrollDistribution", "BranchId");
            DropColumn("Payroll.OverTimeTransfer", "PeriodId");
            DropColumn("EXCEED.LetterSender", "BookNo");
            DropColumn("EXCEED.LetterSender", "Sender");
            DropColumn("Construction.Retention", "ProjectInformationId");
            DropColumn("Construction.PaymenCertificatetLine", "ProjectInformationId");
            DropColumn("Construction.ProjectHumanResourceUtilization", "EmployeeId");
            DropColumn("Construction.ProjectBudgetUtilization", "ProjectDefineBudgetId");
            DropColumn("Construction.ProjectBudgetTransfer", "ProjectDefineBudgetId");
            DropColumn("Construction.ProjectBudgetRequest", "ProjectDefineBudgetId");
            DropColumn("Construction.ProjectPaymenAdvance", "ProjectInformationId");
            DropTable("Schedule.Schedule");
            DropTable("Archive.RecivedLetters");
            DropTable("EXCEED.ProjectTaskSchedule");
            DropTable("Construction.UnitRate");
            DropTable("Construction.ProjectTakeOffLineForKg");
            DropTable("Construction.ProjectMachineryRentHeader");
            DropTable("Construction.ProjectEstmnBoqSummary");
            DropTable("Construction.ProjectEstimationBoqDetail");
            DropTable("Construction.WorkCategory");
            DropTable("Construction.MaterialUtilization");
            DropTable("Construction.MachinaryRent");
            DropTable("EXCEED.GanttResource");
            DropTable("EXCEED.GanttResourceAssignment");
            DropTable("EXCEED.GanttDependency");
            DropTable("Construction.InternalMachinaryRent");
            RenameColumn(table: "Construction.ProjectTakeOffLine", name: "ProjectTakeOffId", newName: "ProjectTakeOff_ProjectTakeOffId");
            CreateIndex("Construction.ProjectTakeOffLine", "ProjectTakeOff_ProjectTakeOffId");
            CreateIndex("Construction.ProjectTakeOff", "Project_ProjectInformationId");
            CreateIndex("Construction.ProjectEstimnWorkItem", "ProjectEstimationNameId");
            CreateIndex("Construction.ProjectEstimnBoqBreakdown", "LookupId");
            CreateIndex("Construction.ProjectBudgetUtilization", "ProjectBudgetId");
            CreateIndex("Construction.ProjectBudgetTransfer", "ProjectBudgetId");
            CreateIndex("Construction.ProjectBudgetRequest", "ProjectBudgetId");
            CreateIndex("Construction.ProjectBudget", "Project_ProjectInformationId");
            AddForeignKey("Construction.ProjectTakeOffLine", "ProjectTakeOff_ProjectTakeOffId", "Construction.ProjectTakeOff", "ProjectTakeOffId");
            AddForeignKey("Construction.ProjectTakeOff", "Project_ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId");
            AddForeignKey("Construction.ProjectEstimnWorkItem", "ProjectEstimationNameId", "Construction.ProjectEstimationName", "ProjectEstimationNameId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimnBoqBreakdown", "LookupId", "common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectBudgetUtilization", "ProjectBudgetId", "Construction.ProjectBudget", "ProjectBudgetId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectBudgetTransfer", "ProjectBudgetId", "Construction.ProjectBudget", "ProjectBudgetId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectBudgetRequest", "ProjectBudgetId", "Construction.ProjectBudget", "ProjectBudgetId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectBudget", "Project_ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId");
            MoveTable(name: "EXCEED.LetterSender", newSchema: "Archive");
        }
    }
}
