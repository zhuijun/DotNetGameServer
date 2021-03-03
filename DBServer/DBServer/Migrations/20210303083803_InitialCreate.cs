using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FruitConfig",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "newid()"),
                    FruitId = table.Column<int>(type: "int", nullable: false, comment: "水果Id"),
                    Rate = table.Column<int>(type: "int", nullable: false, comment: "概率"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "名称"),
                    Image = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "图片"),
                    Score = table.Column<int>(type: "int", nullable: false, comment: "合成可得的积分")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FruitConfig", x => x.Id);
                },
                comment: "合成大西瓜水果配置表");

            migrationBuilder.CreateTable(
                name: "GameBox",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false, comment: "角色Id"),
                    CouponsId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "优惠券Id"),
                    Amount = table.Column<long>(type: "bigint", nullable: false, comment: "优惠金额"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameBox", x => x.Id);
                },
                comment: "游戏宝箱表");

            migrationBuilder.CreateTable(
                name: "GameRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "newid()"),
                    RoleId = table.Column<long>(type: "bigint", nullable: false, comment: "角色Id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false, comment: "用户Id"),
                    NickName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "昵称"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRole", x => x.Id);
                },
                comment: "游戏角色表");

            migrationBuilder.CreateTable(
                name: "GameScore",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false, comment: "角色Id"),
                    Score = table.Column<long>(type: "bigint", nullable: false, comment: "积分"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScore", x => x.Id);
                },
                comment: "游戏积分表");

            migrationBuilder.CreateTable(
                name: "GameTruntable",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false, comment: "角色Id"),
                    AwardId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "奖励Id"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTruntable", x => x.Id);
                },
                comment: "大转盘记录表");

            migrationBuilder.CreateTable(
                name: "TruntableConfig",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AwardDesc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "奖励描述"),
                    ImagePath = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "图片路径"),
                    Price = table.Column<long>(type: "bigint", nullable: false, comment: "价值（单位：分）"),
                    IsValid = table.Column<bool>(type: "bit", nullable: false, comment: "是否开启"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruntableConfig", x => x.Id);
                },
                comment: "合成大西瓜大转盘配置表");

            migrationBuilder.CreateIndex(
                name: "IX_FruitConfig_FruitId",
                table: "FruitConfig",
                column: "FruitId",
                unique: true,
                filter: "[FruitId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FruitConfig");

            migrationBuilder.DropTable(
                name: "GameBox");

            migrationBuilder.DropTable(
                name: "GameRole");

            migrationBuilder.DropTable(
                name: "GameScore");

            migrationBuilder.DropTable(
                name: "GameTruntable");

            migrationBuilder.DropTable(
                name: "TruntableConfig");
        }
    }
}
