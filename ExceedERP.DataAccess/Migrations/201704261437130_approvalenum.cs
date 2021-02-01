namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approvalenum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.AnnualNeedAssesmentValidation", "Status", c => c.Int(nullable: false));
            AlterColumn("EXCEED.GuaranteeValidation", "Status", c => c.Int(nullable: false));
            AlterColumn("EXCEED.PlanValidation", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PlanValidation", "Status", c => c.String());
            AlterColumn("EXCEED.GuaranteeValidation", "Status", c => c.String());
            AlterColumn("EXCEED.AnnualNeedAssesmentValidation", "Status", c => c.String());
        }
    }
}
