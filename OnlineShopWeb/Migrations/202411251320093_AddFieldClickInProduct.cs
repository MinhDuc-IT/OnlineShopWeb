namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldClickInProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Click", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Click");
        }
    }
}
