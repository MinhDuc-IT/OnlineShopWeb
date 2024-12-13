namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastViewedToUserProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProducts", "LastViewed", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProducts", "LastViewed");
        }
    }
}
