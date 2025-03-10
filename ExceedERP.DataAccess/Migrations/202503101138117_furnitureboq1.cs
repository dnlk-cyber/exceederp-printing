namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnitureboq1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Manufacturing.FurnitureBoqDocumentComment",
                c => new
                    {
                        FurnitureBoqDocumentCommentId = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        FurnitureBoqRequestId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureBoqDocumentCommentId);
            
            AddColumn("Manufacturing.FurnitureBoqRequest", "IsSent", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureBoqRequest", "IsEstApproved", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureBoqRequest", "IsEstRejected", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureBoqRequest", "IsEstSent", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureBoqRequest", "SentBy", c => c.String());
            AddColumn("Manufacturing.FurnitureBoqRequest", "SentDate", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureBoqRequest", "EstApprovedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureBoqRequest", "EstApprovedDate", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureBoqRequest", "EstRejectedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureBoqRequest", "EstRejectedDate", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureBoqRequest", "EstSentBy", c => c.String());
            AddColumn("Manufacturing.FurnitureBoqRequest", "EstSentDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("Manufacturing.FurnitureBoqRequest", "EstSentDate");
            DropColumn("Manufacturing.FurnitureBoqRequest", "EstSentBy");
            DropColumn("Manufacturing.FurnitureBoqRequest", "EstRejectedDate");
            DropColumn("Manufacturing.FurnitureBoqRequest", "EstRejectedBy");
            DropColumn("Manufacturing.FurnitureBoqRequest", "EstApprovedDate");
            DropColumn("Manufacturing.FurnitureBoqRequest", "EstApprovedBy");
            DropColumn("Manufacturing.FurnitureBoqRequest", "SentDate");
            DropColumn("Manufacturing.FurnitureBoqRequest", "SentBy");
            DropColumn("Manufacturing.FurnitureBoqRequest", "IsEstSent");
            DropColumn("Manufacturing.FurnitureBoqRequest", "IsEstRejected");
            DropColumn("Manufacturing.FurnitureBoqRequest", "IsEstApproved");
            DropColumn("Manufacturing.FurnitureBoqRequest", "IsSent");
            DropTable("Manufacturing.FurnitureBoqDocumentComment");
        }
    }
}
