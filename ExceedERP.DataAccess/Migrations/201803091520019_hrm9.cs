namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Vacancy", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "BranchId", c => c.Int(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "IsForOther", c => c.Boolean(nullable: false));
            CreateIndex("HRM.Vacancy", "OrgStructureId");
            CreateIndex("HRM.RecruitmentRequest", "BranchId");
            AddForeignKey("HRM.Vacancy", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
            AddForeignKey("HRM.RecruitmentRequest", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.RecruitmentRequest", "BranchId", "HRM.Branch");
            DropForeignKey("HRM.Vacancy", "OrgStructureId", "HRM.MainCompanyStructure");
            DropIndex("HRM.RecruitmentRequest", new[] { "BranchId" });
            DropIndex("HRM.Vacancy", new[] { "OrgStructureId" });
            DropColumn("HRM.RecruitmentRequest", "IsForOther");
            DropColumn("HRM.RecruitmentRequest", "BranchId");
            DropColumn("HRM.Vacancy", "OrgStructureId");
        }
    }
}
