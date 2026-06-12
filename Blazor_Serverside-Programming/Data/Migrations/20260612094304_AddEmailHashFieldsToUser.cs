using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blazor_Serverside_Programming.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailHashFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailHash",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailHashAlgorithm",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailHashIterations",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmailSalt",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailHash",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailHashAlgorithm",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailHashIterations",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailSalt",
                table: "AspNetUsers");
        }
    }
}
