namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateBanner : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Banners",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    title = c.String(nullable: false, maxLength: 500),
                    description = c.String(nullable: false, maxLength: 500),
                    Image = c.Binary(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.Banners");
        }
    }
}
