using Microsoft.EntityFrameworkCore.Migrations;

namespace DBServer.Migrations
{
    public partial class WatermelonConfig_FruitId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FruitId",
                table: "WatermelonConfig",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "水果Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FruitId",
                table: "WatermelonConfig");
        }
    }
}
