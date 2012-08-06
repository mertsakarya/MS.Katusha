namespace MS.Katusha.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddPaypalPayerId : DbMigration
    {
        public override void Up()
        {
            AddColumn("Users", "PaypalPayerId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Users", "PaypalPayerId");
        }
    }
}
