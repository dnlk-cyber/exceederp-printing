namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ParticipatingSupplier", "ProformaItemID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaPurchase", "EvaluateTotalFinance", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "ProformaItemID", c => c.Int(nullable: false));
            AddColumn("EXCEED.RequestedSupplier", "ProformaItemID", c => c.Int(nullable: false));
            AddColumn("EXCEED.WinningSupplierItem", "ProformaItemID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformPassFail", "ProformaItemID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformTechnical", "ProformaItemID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformTechnical", "ProformaItemID");
            DropColumn("EXCEED.ProformPassFail", "ProformaItemID");
            DropColumn("EXCEED.WinningSupplierItem", "ProformaItemID");
            DropColumn("EXCEED.RequestedSupplier", "ProformaItemID");
            DropColumn("EXCEED.ProformaEvaluationResult", "ProformaItemID");
            DropColumn("EXCEED.ProformaPurchase", "EvaluateTotalFinance");
            DropColumn("EXCEED.ParticipatingSupplier", "ProformaItemID");
        }
    }
}
