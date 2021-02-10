namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InfoDeskAndCMSMerged : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "CMS.CmsTraineeIdentifications",
                c => new
                    {
                        CmsTraineeIdentificationId = c.Int(nullable: false, identity: true),
                        CmsTraineeId = c.Int(nullable: false),
                        IdentificationType = c.Int(nullable: false),
                        IdNumber = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CmsTraineeIdentificationId)
                .ForeignKey("CMS.CmsTrainees", t => t.CmsTraineeId, cascadeDelete: true)
                .Index(t => t.CmsTraineeId);
            
            CreateTable(
                "EXCEED.MaterialItems",
                c => new
                    {
                        MaterialItemsId = c.Int(nullable: false, identity: true),
                        ItemName = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.MaterialItemsId);
            
            CreateTable(
                "EXCEED.MaterialsUsed",
                c => new
                    {
                        MaterialsUsedId = c.Int(nullable: false, identity: true),
                        MaterialItemsId = c.Int(nullable: false),
                        AssignedTicketId = c.Int(nullable: false),
                        CustomerTicketId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.MaterialsUsedId);
            
            CreateTable(
                "CMS.WelderPerformanceQualification",
                c => new
                    {
                        WelderPerformanceQualificationId = c.Int(nullable: false, identity: true),
                        CmsTraineeId = c.Int(nullable: false),
                        WelderSSNumber = c.String(),
                        Date = c.DateTime(nullable: false),
                        ATFNumber = c.String(),
                        TestNumber = c.String(),
                        ReferenceWPSNumber = c.String(),
                        CodeOrSpecificationUsed = c.String(),
                        RangeOfCodeOrSpecificationUsed = c.String(),
                        WeldingProcess = c.String(),
                        RangeOfWeldingProcess = c.String(),
                        WeldingType = c.String(),
                        RangeOfWeldingType = c.String(),
                        BackingMaterial = c.Int(nullable: false),
                        RangeOfBackingMaterial = c.String(),
                        SpecOfBaseMetal = c.String(),
                        RangeOfSpecOfBaseMetal = c.String(),
                        GrooveBaseMetalPlateOrPipe = c.String(),
                        ThicknessOfGrooveBaseMetalPlateOrPipe = c.String(),
                        RangeThicknessOfGrooveBaseMetalPlateOrPipe = c.String(),
                        FilletBaseMetalPlateOrPipe = c.String(),
                        ThicknessOfFilletBaseMetalPlateOrPipe = c.String(),
                        RangeOfThicknessOfFilletBaseMetalPlateOrPipe = c.String(),
                        GroovePipeOrTubularOutsideDiameter = c.String(),
                        RangeOfGroovePipeOrTubularOutsideDiameter = c.String(),
                        FilletPipeOrTubularOutsideDiameter = c.String(),
                        RangeOfFilletPipeOrTubularOutsideDiameter = c.String(),
                        FillerMetalSpecificationNumber = c.String(),
                        RangeFillerMetalSpecificationNumber = c.String(),
                        FillerMetalClassificationNumber = c.String(),
                        RangeOfFillerMetalClassificationNumber = c.String(),
                        FillerMetalFNumber = c.String(),
                        RangeOfFillerMetalFNumber = c.String(),
                        FillerMetalDiameter = c.String(),
                        RangeOfFillerMetalDiameter = c.String(),
                        ConsumableInsert = c.Int(nullable: false),
                        PenetrationEnhanceFlux = c.Int(nullable: false),
                        WeldingMetalThickness = c.String(),
                        RangeOfWeldingMetalThickness = c.String(),
                        CurrentPolarityType = c.String(),
                        RangeOfCurrentPolarityType = c.String(),
                        MetalTransferModel = c.String(),
                        RangeOfMetalTransferModel = c.String(),
                        TypeOfTorchShieldingGas = c.String(),
                        RangeOfTypeOfTorchShieldingGas = c.String(),
                        FlowOfTorchShieldingGas = c.String(),
                        RangeOfFlowOfTorchShieldingGas = c.String(),
                        TypeOfRootShieldingGas = c.String(),
                        RangeOfTypeOfRootShieldingGas = c.String(),
                        FlowOfRootShieldingGas = c.String(),
                        RangeOfFlowOfRootShieldingGas = c.String(),
                        TestPositions = c.String(),
                        QualifiedPositions = c.String(),
                        VerticalPropagation = c.Int(nullable: false),
                        RangeOfVerticalPropagation = c.String(),
                        IsVisualTestingPerformedAndAccepted = c.Boolean(nullable: false),
                        IsRadiographicTestingPerformedAndAccepted = c.Boolean(nullable: false),
                        IsFractureTestingPerformedAndAccepted = c.Boolean(nullable: false),
                        IsBendTestingPerformedAndAccepted = c.Boolean(nullable: false),
                        IsNotchTensileTestingPerformedAndAccepted = c.Boolean(nullable: false),
                        IsMacroscopicExaminationPerformedAndAccepted = c.Boolean(nullable: false),
                        IsAdditionalTestingPerformedAndAccepted = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WelderPerformanceQualificationId)
                .ForeignKey("CMS.CmsTrainees", t => t.CmsTraineeId, cascadeDelete: true)
                .Index(t => t.CmsTraineeId);
            
            AddColumn("InfoDesk.AssignTicket_HelpDesk", "MaterialItemsId", c => c.Int(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "IsConfirmedByTheRequester", c => c.Boolean(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "Rate", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("CMS.WelderPerformanceQualification", "CmsTraineeId", "CMS.CmsTrainees");
            DropForeignKey("CMS.CmsTraineeIdentifications", "CmsTraineeId", "CMS.CmsTrainees");
            DropIndex("CMS.WelderPerformanceQualification", new[] { "CmsTraineeId" });
            DropIndex("CMS.CmsTraineeIdentifications", new[] { "CmsTraineeId" });
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "Rate");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "IsConfirmedByTheRequester");
            DropColumn("InfoDesk.AssignTicket_HelpDesk", "MaterialItemsId");
            DropTable("CMS.WelderPerformanceQualification");
            DropTable("EXCEED.MaterialsUsed");
            DropTable("EXCEED.MaterialItems");
            DropTable("CMS.CmsTraineeIdentifications");
        }
    }
}
