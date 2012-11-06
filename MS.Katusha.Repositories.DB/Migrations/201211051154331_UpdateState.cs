namespace MS.Katusha.Repositories.DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.States", "ProfileGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.States", "Name", c => c.String());
            AddColumn("dbo.States", "PhotoGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.States", "IP", c => c.String());
            AddColumn("dbo.States", "TokBoxSessionId", c => c.String());
            AddColumn("dbo.States", "TokBoxTicketId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.States", "TokBoxTicketId");
            DropColumn("dbo.States", "TokBoxSessionId");
            DropColumn("dbo.States", "IP");
            DropColumn("dbo.States", "PhotoGuid");
            DropColumn("dbo.States", "Name");
            DropColumn("dbo.States", "ProfileGuid");
        }
    }
}
