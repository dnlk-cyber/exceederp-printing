namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cont : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ContractRenewal",
                c => new
                    {
                        ContractRenewalId = c.Int(nullable: false, identity: true),
                        ContractManagementID = c.Int(nullable: false),
                        SigningDate = c.DateTime(nullable: false),
                        DaysAdded = c.Int(nullable: false),
                        AdditionalPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractRenewalId)
                .ForeignKey("EXCEED.ContractManagement", t => t.ContractManagementID, cascadeDelete: true)
                .Index(t => t.ContractManagementID);
            
            CreateTable(
                "EXCEED.ContractRenewalValidation",
                c => new
                    {
                        ContractRenewalValidationId = c.Int(nullable: false, identity: true),
                        ContractRenewalId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractRenewalValidationId)
                .ForeignKey("EXCEED.ContractRenewal", t => t.ContractRenewalId, cascadeDelete: true)
                .Index(t => t.ContractRenewalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ContractRenewalValidation", "ContractRenewalId", "EXCEED.ContractRenewal");
            DropForeignKey("EXCEED.ContractRenewal", "ContractManagementID", "EXCEED.ContractManagement");
            DropIndex("EXCEED.ContractRenewalValidation", new[] { "ContractRenewalId" });
            DropIndex("EXCEED.ContractRenewal", new[] { "ContractManagementID" });
            DropTable("EXCEED.ContractRenewalValidation");
            DropTable("EXCEED.ContractRenewal");
        }
    }
}
