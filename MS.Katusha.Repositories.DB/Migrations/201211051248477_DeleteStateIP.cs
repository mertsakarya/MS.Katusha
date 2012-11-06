namespace MS.Katusha.Repositories.DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteStateIP : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.States", "IP");
        }
        
        public override void Down()
        {
            AddColumn("dbo.States", "IP", c => c.String());
        }
    }
}
