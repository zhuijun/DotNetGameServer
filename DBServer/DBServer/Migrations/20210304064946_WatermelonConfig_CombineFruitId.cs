using Microsoft.EntityFrameworkCore.Migrations;

namespace DBServer.Migrations
{
    public partial class WatermelonConfig_CombineFruitId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CombineFruitId",
                table: "FruitConfig",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "合成后的水果Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CombineFruitId",
                table: "FruitConfig");
        }
    }
}
