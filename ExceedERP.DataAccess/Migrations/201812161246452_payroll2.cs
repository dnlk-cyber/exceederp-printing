namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payroll2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PayrollCostAllocationSetting",
                c => new
                    {
                        PayrollCostAllocationSettingId = c.Int(nullable: false, identity: true),
                        CostCenter = c.String(),
                        ProductAccount = c.String(),
                        Percentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PayrollCostAllocationSettingId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.PayrollCostAllocationSetting");
        }
    }
}
