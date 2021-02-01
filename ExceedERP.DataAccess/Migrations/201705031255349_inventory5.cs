namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.PurchaseRequisitionUserDepartmentValidation", "Approver");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PurchaseRequisitionUserDepartmentValidation", "Approver", c => c.String());
        }
    }
}
