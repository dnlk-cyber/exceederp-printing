namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class costcenterbranch : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.BranchCostCenter",
                c => new
                    {
                        BranchCostCenterId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        Values = c.String(maxLength: 128),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.BranchCostCenterId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.GLChartOfAccountCostCenter", t => t.Values)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.OrgStructureId)
                .Index(t => t.Values);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.BranchCostCenter", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("EXCEED.BranchCostCenter", "Values", "EXCEED.GLChartOfAccountCostCenter");
            DropForeignKey("EXCEED.BranchCostCenter", "BranchId", "HRM.Branch");
            DropIndex("EXCEED.BranchCostCenter", new[] { "Values" });
            DropIndex("EXCEED.BranchCostCenter", new[] { "OrgStructureId" });
            DropIndex("EXCEED.BranchCostCenter", new[] { "BranchId" });
            DropTable("EXCEED.BranchCostCenter");
        }
    }
}
