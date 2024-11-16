namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "gender", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "birthDay", c => c.String());
            AddColumn("dbo.Users", "avatar", c => c.String());
            AddColumn("dbo.Users", "userName", c => c.String());
            AddColumn("dbo.CartItems", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CartItems", "TotalPrice");
            DropColumn("dbo.Users", "userName");
            DropColumn("dbo.Users", "avatar");
            DropColumn("dbo.Users", "birthDay");
            DropColumn("dbo.Users", "gender");
        }
    }
}
