using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBServer.Migrations
{
    public partial class WatermelonConfig_Id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "WatermelonConfig",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "WatermelonConfig",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newid()");
        }
    }
}
