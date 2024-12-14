namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastViewedToProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "LastViewed", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "LastViewed");
        }
    }
}
