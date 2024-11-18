namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnituredesigns : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FurniturStandardJobDesign",
                c => new
                    {
                        FurniturStandardJobDesignId = c.Int(nullable: false, identity: true),
                        ReferenceId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.FurniturStandardJobDesignId);
            
            AddColumn("Manufacturing.FurnitureBoqRequest", "RequestDate", c => c.DateTime(nullable: false));
            AddColumn("Manufacturing.FurnitureBoqRequest", "Remark", c => c.String());
            DropColumn("Manufacturing.FurnitureBoqJobDesign", "JobReferenceId");
        }
        
        public override void Down()
        {
            AddColumn("Manufacturing.FurnitureBoqJobDesign", "JobReferenceId", c => c.Int(nullable: false));
            DropColumn("Manufacturing.FurnitureBoqRequest", "Remark");
            DropColumn("Manufacturing.FurnitureBoqRequest", "RequestDate");
            DropTable("EXCEED.FurniturStandardJobDesign");
        }
    }
}
