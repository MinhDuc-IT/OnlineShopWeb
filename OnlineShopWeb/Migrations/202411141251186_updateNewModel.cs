namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateNewModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "gender", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "birthDay", c => c.String());
            AddColumn("dbo.Users", "avatar", c => c.String());
            AddColumn("dbo.Users", "userName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "userName");
            DropColumn("dbo.Users", "avatar");
            DropColumn("dbo.Users", "birthDay");
            DropColumn("dbo.Users", "gender");
        }
    }
}
