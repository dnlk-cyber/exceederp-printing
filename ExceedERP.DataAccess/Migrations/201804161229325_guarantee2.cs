namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guarantee2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GuaranteeRenewalRequest",
                c => new
                    {
                        GuaranteeRenewalRequestId = c.Int(nullable: false, identity: true),
                        GuaranteeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GuaranteeRenewalRequestId)
                .ForeignKey("EXCEED.Guarantee", t => t.GuaranteeId, cascadeDelete: true)
                .Index(t => t.GuaranteeId);
            
            AddColumn("EXCEED.Guarantee", "RenewalStatus", c => c.Int(nullable: false));
            DropColumn("EXCEED.ApPayment", "NetPayable");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApPayment", "NetPayable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("EXCEED.GuaranteeRenewalRequest", "GuaranteeId", "EXCEED.Guarantee");
            DropIndex("EXCEED.GuaranteeRenewalRequest", new[] { "GuaranteeId" });
            DropColumn("EXCEED.Guarantee", "RenewalStatus");
            DropTable("EXCEED.GuaranteeRenewalRequest");
        }
    }
}
