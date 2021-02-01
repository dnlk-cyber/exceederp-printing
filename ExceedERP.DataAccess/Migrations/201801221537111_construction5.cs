namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate");
            DropForeignKey("Construction.PaymentCertificate", "Project_ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.ProjectPaymenAdvance", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.PaymenCertificatetLine", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            DropIndex("Construction.PaymentCertificate", new[] { "Project_ProjectInformationId" });
            DropIndex("Construction.Retention", new[] { "ProjectPaymentCertificate_ProjectPaymentCertificateId" });
            RenameColumn(table: "Construction.PaymentCertificate", name: "Project_ProjectInformationId", newName: "ProjectInformationId");
            CreateTable(
                "Construction.PerformanceEvaluationComponentWeight",
                c => new
                    {
                        PerformanceEvaluationComponentWeightId = c.Int(nullable: false, identity: true),
                        ComponentName = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceEvaluationComponentWeightId);
            
            CreateTable(
                "Construction.ProjectAdvancePayment",
                c => new
                    {
                        ProjectAdvancePaymentId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        TotalProjectAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvancePaymentRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedPaymentRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectAdvancePaymentId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ProjectPaymentAttributes",
                c => new
                    {
                        ProjectPaymentAttributeId = c.Int(nullable: false, identity: true),
                        AttributeName = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectPaymentAttributeId);
            
            AddColumn("Construction.PaymentCertificate", "PaymentNumber", c => c.Int(nullable: false));
            AddColumn("Construction.PaymentCertificate", "ValueOfWorkExecutedToDate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "Vat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "TotalWithVat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "PreviousPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "Retention", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "Penality", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "AdvanceRepayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "OtherDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "NetPayableAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "OrgBranchName", c => c.String());
            AddColumn("Construction.PaymentCertificate", "DateCreated", c => c.DateTime());
            AddColumn("Construction.PaymentCertificate", "LastModified", c => c.DateTime());
            AddColumn("Construction.PaymentCertificate", "CreatedBy", c => c.String());
            AddColumn("Construction.PaymentCertificate", "ModifiedBy", c => c.String());
            AlterColumn("Construction.PaymentCertificate", "ProjectInformationId", c => c.Int(nullable: false));
            CreateIndex("Construction.PaymentCertificate", "ProjectInformationId");
            AddForeignKey("Construction.PaymentCertificate", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            DropColumn("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            DropColumn("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            DropColumn("Construction.PaymentCertificate", "TakeOffNo");
            DropColumn("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", c => c.Int());
            AddColumn("Construction.PaymentCertificate", "TakeOffNo", c => c.String(maxLength: 100));
            AddColumn("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", c => c.Int());
            AddColumn("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", c => c.Int());
            DropForeignKey("Construction.PaymentCertificate", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectAdvancePayment", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.ProjectAdvancePayment", new[] { "ProjectInformationId" });
            DropIndex("Construction.PaymentCertificate", new[] { "ProjectInformationId" });
            AlterColumn("Construction.PaymentCertificate", "ProjectInformationId", c => c.Int());
            DropColumn("Construction.PaymentCertificate", "ModifiedBy");
            DropColumn("Construction.PaymentCertificate", "CreatedBy");
            DropColumn("Construction.PaymentCertificate", "LastModified");
            DropColumn("Construction.PaymentCertificate", "DateCreated");
            DropColumn("Construction.PaymentCertificate", "OrgBranchName");
            DropColumn("Construction.PaymentCertificate", "NetPayableAmount");
            DropColumn("Construction.PaymentCertificate", "OtherDeductions");
            DropColumn("Construction.PaymentCertificate", "AdvanceRepayment");
            DropColumn("Construction.PaymentCertificate", "Penality");
            DropColumn("Construction.PaymentCertificate", "Retention");
            DropColumn("Construction.PaymentCertificate", "PreviousPayment");
            DropColumn("Construction.PaymentCertificate", "TotalWithVat");
            DropColumn("Construction.PaymentCertificate", "Vat");
            DropColumn("Construction.PaymentCertificate", "ValueOfWorkExecutedToDate");
            DropColumn("Construction.PaymentCertificate", "PaymentNumber");
            DropTable("Construction.ProjectPaymentAttributes");
            DropTable("Construction.ProjectAdvancePayment");
            DropTable("Construction.PerformanceEvaluationComponentWeight");
            RenameColumn(table: "Construction.PaymentCertificate", name: "ProjectInformationId", newName: "Project_ProjectInformationId");
            CreateIndex("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            CreateIndex("Construction.PaymentCertificate", "Project_ProjectInformationId");
            CreateIndex("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            CreateIndex("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId");
            AddForeignKey("Construction.PaymentCertificate", "Project_ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId");
            AddForeignKey("Construction.Retention", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate", "ProjectPaymentCertificateId");
            AddForeignKey("Construction.PaymenCertificatetLine", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate", "ProjectPaymentCertificateId");
            AddForeignKey("Construction.ProjectPaymenAdvance", "ProjectPaymentCertificate_ProjectPaymentCertificateId", "Construction.PaymentCertificate", "ProjectPaymentCertificateId");
        }
    }
}
