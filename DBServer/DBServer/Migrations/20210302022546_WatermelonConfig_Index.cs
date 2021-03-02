using Microsoft.EntityFrameworkCore.Migrations;

namespace DBServer.Migrations
{
    public partial class WatermelonConfig_Index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WatermelonConfig_FruitId",
                table: "WatermelonConfig");

            migrationBuilder.CreateIndex(
                name: "IX_WatermelonConfig_FruitId",
                table: "WatermelonConfig",
                column: "FruitId",
                unique: true,
                filter: "[FruitId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WatermelonConfig_FruitId",
                table: "WatermelonConfig");

            migrationBuilder.CreateIndex(
                name: "IX_WatermelonConfig_FruitId",
                table: "WatermelonConfig",
                column: "FruitId",
                filter: "[FruitId] IS NOT NULL");
        }
    }
}
