namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CreateUserTable : DbMigration
    {
        public override void Up()
        {
            // Tạo bảng Users
            CreateTable(
                "dbo.Users",
                c => new
                {
                    CustomerId = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 100),
                    Email = c.String(nullable: false),
                    Password = c.String(nullable: false),
                    Phone = c.String(nullable: false),
                    Address = c.String(maxLength: 200),
                    Gender = c.Int(),
                    BirthDay = c.DateTime(),
                    Avatar = c.Binary(),
                    Role = c.String(nullable: false),
                })
                .PrimaryKey(t => t.CustomerId);

            // Khôi phục lại các khóa ngoại sau khi tạo bảng Users
            AddForeignKey("dbo.UserProducts", "UserId", "dbo.Users", "CustomerId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "CustomerId", "dbo.Users", "CustomerId", cascadeDelete: true);
            AddForeignKey("dbo.Carts", "CustomerId", "dbo.Users", "CustomerId", cascadeDelete: true);
        }

        public override void Down()
        {
            // Xóa bảng Users và các khóa ngoại liên quan
            DropForeignKey("dbo.Carts", "CustomerId", "dbo.Users");
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Users");
            DropForeignKey("dbo.UserProducts", "UserId", "dbo.Users");

            // Xóa bảng Users
            DropTable("dbo.Users");
        }
    }
}
