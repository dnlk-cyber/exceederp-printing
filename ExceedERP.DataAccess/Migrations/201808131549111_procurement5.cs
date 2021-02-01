namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ContractPaymentDistribution", "ContractPaymentId", "EXCEED.ContractPayment");
            DropIndex("EXCEED.ContractPaymentDistribution", new[] { "ContractPaymentId" });
            CreateTable(
                "EXCEED.ContractPaymentGrn",
                c => new
                    {
                        ContractPaymentGrnId = c.Int(nullable: false, identity: true),
                        ContractPaymentId = c.Int(nullable: false),
                        GrnNo = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractPaymentGrnId)
                .ForeignKey("EXCEED.ContractPayment", t => t.ContractPaymentId, cascadeDelete: true)
                .Index(t => t.ContractPaymentId);
            
            AddColumn("EXCEED.ApPaybleInvoicesLines", "GrnNo", c => c.String());
            DropTable("EXCEED.ContractPaymentDistribution");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.ContractPaymentDistribution",
                c => new
                    {
                        ContractPaymentDistributionId = c.Int(nullable: false, identity: true),
                        ContractPaymentId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractPaymentDistributionId);
            
            DropForeignKey("EXCEED.ContractPaymentGrn", "ContractPaymentId", "EXCEED.ContractPayment");
            DropIndex("EXCEED.ContractPaymentGrn", new[] { "ContractPaymentId" });
            DropColumn("EXCEED.ApPaybleInvoicesLines", "GrnNo");
            DropTable("EXCEED.ContractPaymentGrn");
            CreateIndex("EXCEED.ContractPaymentDistribution", "ContractPaymentId");
            AddForeignKey("EXCEED.ContractPaymentDistribution", "ContractPaymentId", "EXCEED.ContractPayment", "ContractPaymentId", cascadeDelete: true);
        }
    }
}
