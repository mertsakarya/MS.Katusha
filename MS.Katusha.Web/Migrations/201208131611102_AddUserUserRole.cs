using MS.Katusha.Enumerations;

namespace MS.Katusha.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserUserRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("Users", "UserRole", c => c.Long(nullable: false, defaultValue: (long) UserRole.Normal));
        }
        
        public override void Down()
        {
            DropColumn("Users", "UserRole");
        }
    }
}
