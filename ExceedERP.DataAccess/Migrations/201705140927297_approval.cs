namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approval : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PurchaseRequisitionPurchaseDepartmentValidation", "Status", c => c.Int(nullable: false));
            AlterColumn("EXCEED.PurchaseRequisitionUserDepartmentValidation", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseRequisitionUserDepartmentValidation", "Status", c => c.String());
            AlterColumn("EXCEED.PurchaseRequisitionPurchaseDepartmentValidation", "Status", c => c.String());
        }
    }
}
