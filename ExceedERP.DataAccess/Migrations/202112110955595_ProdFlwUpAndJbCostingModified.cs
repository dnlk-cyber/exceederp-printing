namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProdFlwUpAndJbCostingModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ChangeOrder", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.ChangeOrder", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.ChangeOrder", "CreatedBy", c => c.String());
            AddColumn("EXCEED.ChangeOrder", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.IssueItem", "JobNo", c => c.String());
            AddColumn("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId", c => c.Int());
            AddColumn("JobCosting.JobMaterialCosts", "ItemCategoryCode", c => c.String());
            AddColumn("JobCosting.JobMaterialCosts", "ItemCode", c => c.String());
            AddColumn("JobCosting.JobMaterialCosts", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("JobCosting.JobMaterialCosts", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("JobCosting.JobMaterialReturns", "ItemCategoryCode", c => c.String());
            AddColumn("JobCosting.JobMaterialReturns", "ItemCode", c => c.String());
            AddColumn("JobCosting.JobMaterialReturns", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("JobCosting.JobMaterialReturns", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Job", "MachineId", c => c.Int(nullable: false));
            AddColumn("EXCEED.StoreRequisitionItem", "JobNo", c => c.String());
            AddColumn("EXCEED.StoreReturnItems", "JobNo", c => c.String());
            CreateIndex("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId");
            AddForeignKey("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId", "ProductFollowUp.JobCategories", "JobCategoryId");
            DropColumn("JobCosting.JobMaterialCosts", "CostCenter");
            DropColumn("JobCosting.JobMaterialCosts", "GlFiscalYearId");
            DropColumn("JobCosting.JobMaterialCosts", "GLPeriodId");
            DropColumn("JobCosting.JobMaterialReturns", "CostCenter");
            DropColumn("JobCosting.JobMaterialReturns", "GlFiscalYearId");
            DropColumn("JobCosting.JobMaterialReturns", "GLPeriodId");
        }
        
        public override void Down()
        {
            AddColumn("JobCosting.JobMaterialReturns", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("JobCosting.JobMaterialReturns", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("JobCosting.JobMaterialReturns", "CostCenter", c => c.String());
            AddColumn("JobCosting.JobMaterialCosts", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("JobCosting.JobMaterialCosts", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("JobCosting.JobMaterialCosts", "CostCenter", c => c.String());
            DropForeignKey("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId", "ProductFollowUp.JobCategories");
            DropIndex("ProductFollowUp.JobCategories", new[] { "JobCategorys_JobCategoryId" });
            DropColumn("EXCEED.StoreReturnItems", "JobNo");
            DropColumn("EXCEED.StoreRequisitionItem", "JobNo");
            DropColumn("ProductFollowUp.Job", "MachineId");
            DropColumn("JobCosting.JobMaterialReturns", "TaxAmount");
            DropColumn("JobCosting.JobMaterialReturns", "SubTotal");
            DropColumn("JobCosting.JobMaterialReturns", "ItemCode");
            DropColumn("JobCosting.JobMaterialReturns", "ItemCategoryCode");
            DropColumn("JobCosting.JobMaterialCosts", "TaxAmount");
            DropColumn("JobCosting.JobMaterialCosts", "SubTotal");
            DropColumn("JobCosting.JobMaterialCosts", "ItemCode");
            DropColumn("JobCosting.JobMaterialCosts", "ItemCategoryCode");
            DropColumn("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId");
            DropColumn("EXCEED.IssueItem", "JobNo");
            DropColumn("EXCEED.ChangeOrder", "ModifiedBy");
            DropColumn("EXCEED.ChangeOrder", "CreatedBy");
            DropColumn("EXCEED.ChangeOrder", "LastModified");
            DropColumn("EXCEED.ChangeOrder", "DateCreated");
        }
    }
}
