namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction18 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ProjectPaymentTerm",
                c => new
                    {
                        ProjectPaymentTermId = c.Int(nullable: false, identity: true),
                        ProjectContractId = c.Int(nullable: false),
                        AttributeName = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectPaymentTermId)
                .ForeignKey("Construction.ProjectContract", t => t.ProjectContractId, cascadeDelete: true)
                .Index(t => t.ProjectContractId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.ProjectPaymentTerm", "ProjectContractId", "Construction.ProjectContract");
            DropIndex("Construction.ProjectPaymentTerm", new[] { "ProjectContractId" });
            DropTable("Construction.ProjectPaymentTerm");
        }
    }
}
