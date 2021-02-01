namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentrequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PaymentRequestLine",
                c => new
                    {
                        PaymentRequestLineId = c.Int(nullable: false, identity: true),
                        PaymentRequestId = c.Int(nullable: false),
                        GLTaxRateID = c.Int(nullable: false),
                        ItemName = c.String(),
                        Description = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PaymentRequestLineId)
                .ForeignKey("EXCEED.GLTaxRate", t => t.GLTaxRateID, cascadeDelete: true)
                .ForeignKey("EXCEED.PaymentRequest", t => t.PaymentRequestId, cascadeDelete: true)
                .Index(t => t.PaymentRequestId)
                .Index(t => t.GLTaxRateID);
            
            CreateTable(
                "EXCEED.PaymentRequest",
                c => new
                    {
                        PaymentRequestId = c.Int(nullable: false, identity: true),
                        Supplier = c.String(),
                        Purchaser = c.String(),
                        ReasonOfPayment = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                        RequestDateAm = c.String(),
                        InvoiceNo = c.String(),
                        PurchaseRequestNo = c.String(),
                        PONo = c.String(),
                        GRN = c.String(),
                        BranchId = c.Int(nullable: false),
                        OrgStuructureId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Remark = c.String(),
                        MainCompanySturucture_OrgStructureId = c.Int(),
                    })
                .PrimaryKey(t => t.PaymentRequestId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.MainCompanySturucture_OrgStructureId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.EmployeeId)
                .Index(t => t.MainCompanySturucture_OrgStructureId);
            
            AddColumn("HRM.WeeklyAttendance", "DayOneAN", c => c.Boolean(nullable: false));
            AddColumn("HRM.WeeklyAttendance", "DayTwoAN", c => c.Boolean(nullable: false));
            AddColumn("HRM.WeeklyAttendance", "DayThreeAN", c => c.Boolean(nullable: false));
            AddColumn("HRM.WeeklyAttendance", "DayFourAN", c => c.Boolean(nullable: false));
            AddColumn("HRM.WeeklyAttendance", "DayFiveAN", c => c.Boolean(nullable: false));
            AddColumn("HRM.WeeklyAttendance", "DaySixAN", c => c.Boolean(nullable: false));
            AddColumn("HRM.WeeklyAttendance", "DaySevenAN", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PaymentRequestLine", "PaymentRequestId", "EXCEED.PaymentRequest");
            DropForeignKey("EXCEED.PaymentRequest", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.PaymentRequest", "MainCompanySturucture_OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("EXCEED.PaymentRequest", "BranchId", "HRM.Branch");
            DropForeignKey("EXCEED.PaymentRequestLine", "GLTaxRateID", "EXCEED.GLTaxRate");
            DropIndex("EXCEED.PaymentRequest", new[] { "MainCompanySturucture_OrgStructureId" });
            DropIndex("EXCEED.PaymentRequest", new[] { "EmployeeId" });
            DropIndex("EXCEED.PaymentRequest", new[] { "BranchId" });
            DropIndex("EXCEED.PaymentRequestLine", new[] { "GLTaxRateID" });
            DropIndex("EXCEED.PaymentRequestLine", new[] { "PaymentRequestId" });
            DropColumn("HRM.WeeklyAttendance", "DaySevenAN");
            DropColumn("HRM.WeeklyAttendance", "DaySixAN");
            DropColumn("HRM.WeeklyAttendance", "DayFiveAN");
            DropColumn("HRM.WeeklyAttendance", "DayFourAN");
            DropColumn("HRM.WeeklyAttendance", "DayThreeAN");
            DropColumn("HRM.WeeklyAttendance", "DayTwoAN");
            DropColumn("HRM.WeeklyAttendance", "DayOneAN");
            DropTable("EXCEED.PaymentRequest");
            DropTable("EXCEED.PaymentRequestLine");
        }
    }
}
