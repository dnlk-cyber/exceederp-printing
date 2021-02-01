namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction31 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.WorkItemDuration", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.Task", "WorkItemDurationId", "Construction.WorkItemDuration");
            DropForeignKey("Construction.ProjectTakeOffLine", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropForeignKey("Construction.Task", "ProjectWorkBreakDownId", "Construction.ProjectWorkBreakDown");
            DropIndex("Construction.Task", new[] { "WorkItemDurationId" });
            DropIndex("Construction.Task", new[] { "ProjectWorkBreakDownId" });
            DropIndex("Construction.WorkItemDuration", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ProjectTakeOffLine", new[] { "UnitOfMeasurementId" });
            RenameColumn(table: "Construction.Task", name: "ProjectWorkBreakDownId", newName: "ProjectWorkBreakDown_ProjectWorkBreakDownId");
            CreateTable(
                "Construction.BoQOfExecutedWork",
                c => new
                    {
                        BoQOfExecutedWorkId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        BillReference = c.String(),
                        BillNumber = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.BoQOfExecutedWorkId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.BoQOfExecutedWorkContent",
                c => new
                    {
                        BoQOfExecutedWorkContentId = c.Int(nullable: false, identity: true),
                        BoQOfExecutedWorkId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        Index = c.Int(nullable: false),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        Description = c.String(),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        ContractQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityTillPrevPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityThisPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountTillPrevPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountThisPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountToDatePayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkCategoryId = c.Int(nullable: false),
                        ConfigurationId = c.Int(nullable: false),
                        ParentBoQContentId = c.Int(nullable: false),
                        SubProjectTypeName = c.String(),
                        BlockName = c.String(),
                        StructureyType = c.String(),
                        WorkCategory = c.String(),
                    })
                .PrimaryKey(t => t.BoQOfExecutedWorkContentId)
                .ForeignKey("Construction.BoQOfExecutedWork", t => t.BoQOfExecutedWorkId, cascadeDelete: true)
                .Index(t => t.BoQOfExecutedWorkId);
            
            CreateTable(
                "Construction.BoQOfExecutedWorkContentDetail",
                c => new
                    {
                        BoQOfExecutedWorkContentDetailId = c.Int(nullable: false, identity: true),
                        BoQOfExecutedWorkContentId = c.Int(nullable: false),
                        Itemcode = c.String(),
                        Index = c.Int(nullable: false),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        Description = c.String(),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        ContractQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityTillPrevPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityThisPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountTillPrevPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountThisPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountToDatePayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.BoQOfExecutedWorkContentDetailId);
            
            CreateTable(
                "Construction.EstimationCustomWorkItemDirectLabor",
                c => new
                    {
                        EstimationCustomWorkItemDirectLaborId = c.Int(nullable: false, identity: true),
                        ProjectEstimationCustomWorkItemId = c.Int(nullable: false),
                        ProjectEstimationIndexLabourId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManpowerUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IndexedHourlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EstimationCustomWorkItemDirectLaborId)
                .ForeignKey("Construction.ProjectEstimationCustomWorkItem", t => t.ProjectEstimationCustomWorkItemId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectEstmnIndexLabour", t => t.ProjectEstimationIndexLabourId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationCustomWorkItemId)
                .Index(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "Construction.EstimationCustomWorkItemEquipment",
                c => new
                    {
                        EstimationCustomWorkItemEquipmentId = c.Int(nullable: false, identity: true),
                        ProjectEstimationCustomWorkItemId = c.Int(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentType = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EstimationCustomWorkItemEquipmentId)
                .ForeignKey("Construction.ProjectEstimationCustomWorkItem", t => t.ProjectEstimationCustomWorkItemId, cascadeDelete: true)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.ProjectEstimationCustomWorkItemId)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "Construction.EstimationCustomWorkItemMaterial",
                c => new
                    {
                        EstimationCustomWorkItemMaterialId = c.Int(nullable: false, identity: true),
                        ProjectEstimationCustomWorkItemId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(maxLength: 100),
                        ItemCode = c.String(maxLength: 100),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EstimationCustomWorkItemMaterialId)
                .ForeignKey("Construction.ProjectEstimationCustomWorkItem", t => t.ProjectEstimationCustomWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationCustomWorkItemId);
            
            CreateTable(
                "Construction.GenericAreaWorkItem",
                c => new
                    {
                        GenericAreaWorkItemId = c.Int(nullable: false, identity: true),
                        WorkCategoryId = c.Int(nullable: false),
                        WorkItemName = c.String(),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Area = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransportCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CylinderLock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        SubProjectTypeId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GenericAreaWorkItemId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.WorkCategoryId)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.GenericProjectCustomWorkItem",
                c => new
                    {
                        GenericProjectCustomWorkItemId = c.Int(nullable: false, identity: true),
                        WorkCategoryId = c.Int(nullable: false),
                        WorkItemName = c.String(),
                        ProjectInformationId = c.Int(nullable: false),
                        TotalUnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectType = c.Int(nullable: false),
                        SubProjectTypeId = c.Int(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        ItemCode = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GenericProjectCustomWorkItemId);
            
            CreateTable(
                "Construction.GenericProjectDirectLabour",
                c => new
                    {
                        GenericProjectDirectLabourId = c.Int(nullable: false, identity: true),
                        GenericWorkItemPerProjectId = c.Int(nullable: false),
                        ProjectEstimationIndexLabourId = c.Int(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IndexedHourlySalary = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DailyPay = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ManpowerUnitCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GenericProjectDirectLabourId)
                .ForeignKey("Construction.ProjectEstmnIndexLabour", t => t.ProjectEstimationIndexLabourId, cascadeDelete: true)
                .ForeignKey("Construction.GenericWorkItemPerProject", t => t.GenericWorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.GenericWorkItemPerProjectId)
                .Index(t => t.ProjectEstimationIndexLabourId);
            
            CreateTable(
                "Construction.GenericWorkItemPerProject",
                c => new
                    {
                        GenericWorkItemPerProjectId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        WorkCategoryId = c.Int(nullable: false),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ItemName = c.String(),
                        TotalQuantityOfWork = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LabourOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentOutput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalUnitCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectType = c.Int(nullable: false),
                        Remark = c.String(),
                        LabourHourlyOutPut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfMeasurementId = c.Int(),
                        OverheadRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ParentId = c.Int(nullable: false),
                        SubProjectTypeId = c.Int(),
                        ItemCode = c.String(),
                        IsSubCategory = c.Boolean(nullable: false),
                        CopiedFromId = c.Int(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GenericWorkItemPerProjectId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.WorkCategoryId);
            
            CreateTable(
                "Construction.GenericProjectEquipment",
                c => new
                    {
                        GenericProjectEquipmentId = c.Int(nullable: false, identity: true),
                        GenericWorkItemPerProjectId = c.Int(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitFactory = c.Decimal(nullable: false, precision: 18, scale: 4),
                        RentalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EquipmentUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EquipmentType = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GenericProjectEquipmentId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .ForeignKey("Construction.GenericWorkItemPerProject", t => t.GenericWorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.GenericWorkItemPerProjectId)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "Construction.GenericProjectMaterial",
                c => new
                    {
                        GenericProjectMaterialId = c.Int(nullable: false, identity: true),
                        GenericWorkItemPerProjectId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        ItemCategoryCode = c.String(),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MaterialUnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GenericProjectMaterialId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("Construction.GenericWorkItemPerProject", t => t.GenericWorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.GenericWorkItemPerProjectId)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Construction.GenericSubContractWorkItem",
                c => new
                    {
                        GenericSubContractWorkItemId = c.Int(nullable: false, identity: true),
                        WorkCategoryId = c.Int(nullable: false),
                        WorkItemName = c.String(),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        Quntity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractCostOne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractCostTwo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractCostThree = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SelectedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectInformationId = c.Int(nullable: false),
                        SubProjectTypeId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GenericSubContractWorkItemId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .ForeignKey("Construction.UnitOfMeasurement", t => t.UnitOfMeasurementId, cascadeDelete: true)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.WorkCategoryId)
                .Index(t => t.UnitOfMeasurementId)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ScheduleRevisionValidation",
                c => new
                    {
                        ScheduleRevisionValidationId = c.Int(nullable: false, identity: true),
                        ScheduleRevisionId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ScheduleRevisionValidationId)
                .ForeignKey("Construction.ScheduleRevision", t => t.ScheduleRevisionId, cascadeDelete: true)
                .Index(t => t.ScheduleRevisionId);
            
            CreateTable(
                "Construction.SubProjectStructureTypeMapping",
                c => new
                    {
                        SubProjectStructureTypeMappingId = c.Int(nullable: false, identity: true),
                        SubProjectTypeId = c.Int(nullable: false),
                        ConfigurationId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SubProjectStructureTypeMappingId)
                .ForeignKey("Common.Configuration", t => t.ConfigurationId, cascadeDelete: true)
                .ForeignKey("Construction.SubProjectType", t => t.SubProjectTypeId, cascadeDelete: true)
                .Index(t => t.SubProjectTypeId)
                .Index(t => t.ConfigurationId);
            
            CreateTable(
                "Construction.SubProjectType",
                c => new
                    {
                        SubProjectTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        NumberOfBlocks = c.Int(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SubProjectTypeId);
            
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "GenericWorkItemPerProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "WorkCategory", c => c.String());
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "StructureyType", c => c.String());
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "SubProjectTypeName", c => c.String());
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "SubProjectTypeId", c => c.Int(nullable: false));
            AddColumn("Construction.Task", "WorkItemPerProjectId", c => c.Int());
            AddColumn("Construction.Task", "WorkCategoryId", c => c.Int(nullable: false));
            AddColumn("Construction.Task", "ConfigurationId", c => c.Int(nullable: false));
            AddColumn("Construction.Task", "SubProjectTypeId", c => c.Int(nullable: false));
            AddColumn("Construction.Task", "SubProjectTypeName", c => c.String());
            AddColumn("Construction.Task", "BlockName", c => c.String());
            AddColumn("Construction.Task", "StructureyType", c => c.String());
            AddColumn("Construction.Task", "WorkCategory", c => c.String());
            AddColumn("Construction.Task", "ExecutedQuantityTillPrevVersion", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "ExecutedQuantityThisVersion", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "ExecutedQuantityToDate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "ExecutedAmountTillPrevVersion", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "ExecutedAmountThisVersion", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "ExecutedAmountToDate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Task", "IsSubActivityCreatedInSchedule", c => c.Boolean(nullable: false));
            AddColumn("Construction.WorkItemDuration", "GenericWorkItemPerProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "SubProjectTypeName", c => c.String());
            AddColumn("Construction.WorkItemDuration", "StructureyType", c => c.String());
            AddColumn("Construction.WorkItemDuration", "WorkCategory", c => c.String());
            AddColumn("Construction.WorkItemPerProject", "GenericWorkItemPerProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemPerProject", "ActualQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.AreaWorkItem", "GenericAreaWorkItemId", c => c.Int(nullable: false));
            AddColumn("Construction.Block", "SubProjectTypeId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "EquipmentCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "LaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "BillReference", c => c.String(nullable: false));
            AddColumn("Construction.PaymentCertificate", "Correction", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectCustomWorkItem", "GenericProjectCustomWorkItemId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTakeOff", "WorkItemPerProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTakeOff", "ProjectInformationId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTakeOffLine", "No", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectTakeOffLine", "Depth", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ProjectTakeOffLine", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectTakeOffLine", "IsDeductive", c => c.Boolean(nullable: false));
            AddColumn("Construction.ProjectTakeOffLine", "DateOfInformation", c => c.DateTime(nullable: false));
            AddColumn("Construction.ResourceUtilizationWorkItem", "SubProjectTypeName", c => c.String());
            AddColumn("Construction.ResourceUtilizationWorkItem", "BlockName", c => c.String());
            AddColumn("Construction.ResourceUtilizationWorkItem", "StructureyType", c => c.String());
            AddColumn("Construction.ResourceUtilizationWorkItem", "WorkCategory", c => c.String());
            AddColumn("Construction.ScheduleRevision", "ScheduleStatus", c => c.Int(nullable: false));
            AddColumn("Construction.ScheduleRevision", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Construction.ScheduleRevision", "Remark", c => c.String());
            AddColumn("Construction.ScheduleRevision", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Construction.ScheduleRevision", "VoidBy", c => c.String());
            AddColumn("Construction.ScheduleRevision", "VoidTime", c => c.DateTime());
            AddColumn("Construction.ScheduleRevision", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Construction.ScheduleRevision", "OnlineApprovedBy", c => c.String());
            AddColumn("Construction.ScheduleRevision", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Construction.ScheduleRevision", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Construction.ScheduleRevision", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Construction.ScheduleRevision", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Construction.ScheduleRevision", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Construction.ScheduleRevision", "SendForApprovalBy", c => c.String());
            AddColumn("Construction.ScheduleRevision", "SendForApprovalTime", c => c.DateTime());
            AddColumn("Construction.SubContractWorkItem", "GenericSubContractWorkItemId", c => c.Int(nullable: false));
            AlterColumn("Construction.Task", "ProjectWorkBreakDown_ProjectWorkBreakDownId", c => c.Int());
            AlterColumn("Construction.ProjectTakeOffLine", "Length", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ScheduleRevision", "Status", c => c.String());
            CreateIndex("Construction.Task", "WorkItemPerProjectId");
            CreateIndex("Construction.Task", "ProjectWorkBreakDown_ProjectWorkBreakDownId");
            CreateIndex("Construction.WorkItemDuration", "GenericWorkItemPerProjectId");
            AddForeignKey("Construction.Task", "WorkItemPerProjectId", "Construction.WorkItemPerProject", "WorkItemPerProjectId");
            AddForeignKey("Construction.WorkItemDuration", "GenericWorkItemPerProjectId", "Construction.GenericWorkItemPerProject", "GenericWorkItemPerProjectId", cascadeDelete: true);
            AddForeignKey("Construction.Task", "ProjectWorkBreakDown_ProjectWorkBreakDownId", "Construction.ProjectWorkBreakDown", "ProjectWorkBreakDownId");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "WorkItemPerProjectId");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "Category");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "Type");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "BlockId");
            //DropColumn("Construction.Task", "WorkItemDurationId");
            DropColumn("Construction.WorkItemDuration", "WorkItemPerProjectId");
            DropColumn("Construction.ProjectTakeOff", "ProjectEstimationWorkItemId");
            DropColumn("Construction.ProjectTakeOff", "ProjectId");
            DropColumn("Construction.ProjectTakeOffLine", "T");
            DropColumn("Construction.ProjectTakeOffLine", "D");
            DropColumn("Construction.ProjectTakeOffLine", "S");
            DropColumn("Construction.ProjectTakeOffLine", "Height");
            DropColumn("Construction.ResourceUtilizationWorkItem", "Category");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ResourceUtilizationWorkItem", "Category", c => c.String());
            AddColumn("Construction.ProjectTakeOffLine", "Height", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ProjectTakeOffLine", "S", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ProjectTakeOffLine", "D", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ProjectTakeOffLine", "T", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.ProjectTakeOff", "ProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTakeOff", "ProjectEstimationWorkItemId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "WorkItemPerProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.Task", "WorkItemDurationId", c => c.Int());
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "BlockId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "Type", c => c.String());
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "Category", c => c.String());
            AddColumn("Construction.ProjectEstimnBoqBreakdown", "WorkItemPerProjectId", c => c.Int(nullable: false));
            DropForeignKey("Construction.Task", "ProjectWorkBreakDown_ProjectWorkBreakDownId", "Construction.ProjectWorkBreakDown");
            DropForeignKey("Construction.WorkItemDuration", "GenericWorkItemPerProjectId", "Construction.GenericWorkItemPerProject");
            DropForeignKey("Construction.SubProjectStructureTypeMapping", "SubProjectTypeId", "Construction.SubProjectType");
            DropForeignKey("Construction.SubProjectStructureTypeMapping", "ConfigurationId", "Common.Configuration");
            DropForeignKey("Construction.ScheduleRevisionValidation", "ScheduleRevisionId", "Construction.ScheduleRevision");
            DropForeignKey("Construction.GenericSubContractWorkItem", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.GenericSubContractWorkItem", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropForeignKey("Construction.GenericSubContractWorkItem", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.GenericProjectMaterial", "GenericWorkItemPerProjectId", "Construction.GenericWorkItemPerProject");
            DropForeignKey("Construction.GenericProjectMaterial", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.GenericProjectEquipment", "GenericWorkItemPerProjectId", "Construction.GenericWorkItemPerProject");
            DropForeignKey("Construction.GenericProjectEquipment", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("Construction.GenericProjectDirectLabour", "GenericWorkItemPerProjectId", "Construction.GenericWorkItemPerProject");
            DropForeignKey("Construction.GenericWorkItemPerProject", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.GenericWorkItemPerProject", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.GenericProjectDirectLabour", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Construction.GenericAreaWorkItem", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.GenericAreaWorkItem", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.EstimationCustomWorkItemMaterial", "ProjectEstimationCustomWorkItemId", "Construction.ProjectEstimationCustomWorkItem");
            DropForeignKey("Construction.EstimationCustomWorkItemEquipment", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("Construction.EstimationCustomWorkItemEquipment", "ProjectEstimationCustomWorkItemId", "Construction.ProjectEstimationCustomWorkItem");
            DropForeignKey("Construction.EstimationCustomWorkItemDirectLabor", "ProjectEstimationIndexLabourId", "Construction.ProjectEstmnIndexLabour");
            DropForeignKey("Construction.EstimationCustomWorkItemDirectLabor", "ProjectEstimationCustomWorkItemId", "Construction.ProjectEstimationCustomWorkItem");
            DropForeignKey("Construction.BoQOfExecutedWorkContent", "BoQOfExecutedWorkId", "Construction.BoQOfExecutedWork");
            DropForeignKey("Construction.BoQOfExecutedWork", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.Task", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropIndex("Construction.WorkItemDuration", new[] { "GenericWorkItemPerProjectId" });
            DropIndex("Construction.SubProjectStructureTypeMapping", new[] { "ConfigurationId" });
            DropIndex("Construction.SubProjectStructureTypeMapping", new[] { "SubProjectTypeId" });
            DropIndex("Construction.ScheduleRevisionValidation", new[] { "ScheduleRevisionId" });
            DropIndex("Construction.GenericSubContractWorkItem", new[] { "ProjectInformationId" });
            DropIndex("Construction.GenericSubContractWorkItem", new[] { "UnitOfMeasurementId" });
            DropIndex("Construction.GenericSubContractWorkItem", new[] { "WorkCategoryId" });
            DropIndex("Construction.GenericProjectMaterial", new[] { "ItemCode" });
            DropIndex("Construction.GenericProjectMaterial", new[] { "GenericWorkItemPerProjectId" });
            DropIndex("Construction.GenericProjectEquipment", new[] { "FleetsID" });
            DropIndex("Construction.GenericProjectEquipment", new[] { "GenericWorkItemPerProjectId" });
            DropIndex("Construction.GenericWorkItemPerProject", new[] { "WorkCategoryId" });
            DropIndex("Construction.GenericWorkItemPerProject", new[] { "ProjectInformationId" });
            DropIndex("Construction.GenericProjectDirectLabour", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.GenericProjectDirectLabour", new[] { "GenericWorkItemPerProjectId" });
            DropIndex("Construction.GenericAreaWorkItem", new[] { "ProjectInformationId" });
            DropIndex("Construction.GenericAreaWorkItem", new[] { "WorkCategoryId" });
            DropIndex("Construction.EstimationCustomWorkItemMaterial", new[] { "ProjectEstimationCustomWorkItemId" });
            DropIndex("Construction.EstimationCustomWorkItemEquipment", new[] { "FleetsID" });
            DropIndex("Construction.EstimationCustomWorkItemEquipment", new[] { "ProjectEstimationCustomWorkItemId" });
            DropIndex("Construction.EstimationCustomWorkItemDirectLabor", new[] { "ProjectEstimationIndexLabourId" });
            DropIndex("Construction.EstimationCustomWorkItemDirectLabor", new[] { "ProjectEstimationCustomWorkItemId" });
            DropIndex("Construction.BoQOfExecutedWorkContent", new[] { "BoQOfExecutedWorkId" });
            DropIndex("Construction.BoQOfExecutedWork", new[] { "ProjectInformationId" });
            DropIndex("Construction.Task", new[] { "ProjectWorkBreakDown_ProjectWorkBreakDownId" });
            DropIndex("Construction.Task", new[] { "WorkItemPerProjectId" });
            AlterColumn("Construction.ScheduleRevision", "Status", c => c.Int(nullable: false));
            AlterColumn("Construction.ProjectTakeOffLine", "Length", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.Task", "ProjectWorkBreakDown_ProjectWorkBreakDownId", c => c.Int(nullable: false));
            DropColumn("Construction.SubContractWorkItem", "GenericSubContractWorkItemId");
            DropColumn("Construction.ScheduleRevision", "SendForApprovalTime");
            DropColumn("Construction.ScheduleRevision", "SendForApprovalBy");
            DropColumn("Construction.ScheduleRevision", "IsSendForApproval");
            DropColumn("Construction.ScheduleRevision", "OnlineTransferredTime");
            DropColumn("Construction.ScheduleRevision", "IsOnlineTransferredBy");
            DropColumn("Construction.ScheduleRevision", "IsOnlineTransferred");
            DropColumn("Construction.ScheduleRevision", "OnlineApprovedTime");
            DropColumn("Construction.ScheduleRevision", "OnlineApprovedBy");
            DropColumn("Construction.ScheduleRevision", "IsOnlineApproved");
            DropColumn("Construction.ScheduleRevision", "VoidTime");
            DropColumn("Construction.ScheduleRevision", "VoidBy");
            DropColumn("Construction.ScheduleRevision", "IsVoid");
            DropColumn("Construction.ScheduleRevision", "Remark");
            DropColumn("Construction.ScheduleRevision", "IsPostedToSecondLedger");
            DropColumn("Construction.ScheduleRevision", "ScheduleStatus");
            DropColumn("Construction.ResourceUtilizationWorkItem", "WorkCategory");
            DropColumn("Construction.ResourceUtilizationWorkItem", "StructureyType");
            DropColumn("Construction.ResourceUtilizationWorkItem", "BlockName");
            DropColumn("Construction.ResourceUtilizationWorkItem", "SubProjectTypeName");
            DropColumn("Construction.ProjectTakeOffLine", "DateOfInformation");
            DropColumn("Construction.ProjectTakeOffLine", "IsDeductive");
            DropColumn("Construction.ProjectTakeOffLine", "Quantity");
            DropColumn("Construction.ProjectTakeOffLine", "Depth");
            DropColumn("Construction.ProjectTakeOffLine", "No");
            DropColumn("Construction.ProjectTakeOff", "ProjectInformationId");
            DropColumn("Construction.ProjectTakeOff", "WorkItemPerProjectId");
            DropColumn("Construction.ProjectCustomWorkItem", "GenericProjectCustomWorkItemId");
            DropColumn("Construction.PaymentCertificate", "Correction");
            DropColumn("Construction.PaymentCertificate", "BillReference");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "LaborCost");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "EquipmentCost");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "MaterialCost");
            DropColumn("Construction.Block", "SubProjectTypeId");
            DropColumn("Construction.AreaWorkItem", "GenericAreaWorkItemId");
            DropColumn("Construction.WorkItemPerProject", "ActualQuantityOfWork");
            DropColumn("Construction.WorkItemPerProject", "GenericWorkItemPerProjectId");
            DropColumn("Construction.WorkItemDuration", "WorkCategory");
            DropColumn("Construction.WorkItemDuration", "StructureyType");
            DropColumn("Construction.WorkItemDuration", "SubProjectTypeName");
            DropColumn("Construction.WorkItemDuration", "GenericWorkItemPerProjectId");
            DropColumn("Construction.Task", "IsSubActivityCreatedInSchedule");
            DropColumn("Construction.Task", "ExecutedAmountToDate");
            DropColumn("Construction.Task", "ExecutedAmountThisVersion");
            DropColumn("Construction.Task", "ExecutedAmountTillPrevVersion");
            DropColumn("Construction.Task", "ExecutedQuantityToDate");
            DropColumn("Construction.Task", "ExecutedQuantityThisVersion");
            DropColumn("Construction.Task", "ExecutedQuantityTillPrevVersion");
            DropColumn("Construction.Task", "WorkCategory");
            DropColumn("Construction.Task", "StructureyType");
            DropColumn("Construction.Task", "BlockName");
            DropColumn("Construction.Task", "SubProjectTypeName");
            DropColumn("Construction.Task", "SubProjectTypeId");
            DropColumn("Construction.Task", "ConfigurationId");
            DropColumn("Construction.Task", "WorkCategoryId");
            DropColumn("Construction.Task", "WorkItemPerProjectId");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "SubProjectTypeId");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "SubProjectTypeName");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "StructureyType");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "WorkCategory");
            DropColumn("Construction.ProjectEstimnBoqBreakdown", "GenericWorkItemPerProjectId");
            DropTable("Construction.SubProjectType");
            DropTable("Construction.SubProjectStructureTypeMapping");
            DropTable("Construction.ScheduleRevisionValidation");
            DropTable("Construction.GenericSubContractWorkItem");
            DropTable("Construction.GenericProjectMaterial");
            DropTable("Construction.GenericProjectEquipment");
            DropTable("Construction.GenericWorkItemPerProject");
            DropTable("Construction.GenericProjectDirectLabour");
            DropTable("Construction.GenericProjectCustomWorkItem");
            DropTable("Construction.GenericAreaWorkItem");
            DropTable("Construction.EstimationCustomWorkItemMaterial");
            DropTable("Construction.EstimationCustomWorkItemEquipment");
            DropTable("Construction.EstimationCustomWorkItemDirectLabor");
            DropTable("Construction.BoQOfExecutedWorkContentDetail");
            DropTable("Construction.BoQOfExecutedWorkContent");
            DropTable("Construction.BoQOfExecutedWork");
            RenameColumn(table: "Construction.Task", name: "ProjectWorkBreakDown_ProjectWorkBreakDownId", newName: "ProjectWorkBreakDownId");
            CreateIndex("Construction.ProjectTakeOffLine", "UnitOfMeasurementId");
            CreateIndex("Construction.WorkItemDuration", "WorkItemPerProjectId");
            CreateIndex("Construction.Task", "ProjectWorkBreakDownId");
            CreateIndex("Construction.Task", "WorkItemDurationId");
            AddForeignKey("Construction.Task", "ProjectWorkBreakDownId", "Construction.ProjectWorkBreakDown", "ProjectWorkBreakDownId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectTakeOffLine", "UnitOfMeasurementId", "Construction.UnitOfMeasurement", "UnitOfMeasurementId", cascadeDelete: true);
            AddForeignKey("Construction.Task", "WorkItemDurationId", "Construction.WorkItemDuration", "WorkItemDurationId");
            AddForeignKey("Construction.WorkItemDuration", "WorkItemPerProjectId", "Construction.WorkItemPerProject", "WorkItemPerProjectId", cascadeDelete: true);
        }
    }
}
