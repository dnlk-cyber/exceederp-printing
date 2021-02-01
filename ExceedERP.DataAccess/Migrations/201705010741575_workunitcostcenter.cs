namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class workunitcostcenter : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "OrgStructureId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.StoreRequisition", "OrgStructureId");
            AddForeignKey("EXCEED.StoreRequisition", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
            DropColumn("EXCEED.StoreRequisition", "Department");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.StoreRequisition", "Department", c => c.String(nullable: false));
            DropForeignKey("EXCEED.StoreRequisition", "OrgStructureId", "HRM.MainCompanyStructure");
            DropIndex("EXCEED.StoreRequisition", new[] { "OrgStructureId" });
            DropColumn("EXCEED.StoreRequisition", "OrgStructureId");
        }
    }
}
