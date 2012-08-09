namespace MS.Katusha.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddPhotoStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("Photos", "Status", c => c.Byte(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("Photos", "Status");
        }
    }
}
