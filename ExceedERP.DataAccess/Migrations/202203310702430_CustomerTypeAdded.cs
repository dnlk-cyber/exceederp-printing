namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerTypeAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ProductFollowUp.ProductionCustomerTypes",
                c => new
                    {
                        ProductionCustomerTypeId = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        Location = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ProductionCustomerTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("ProductFollowUp.ProductionCustomerTypes");
        }
    }
}
