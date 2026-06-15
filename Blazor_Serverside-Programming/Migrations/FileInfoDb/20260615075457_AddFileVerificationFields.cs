using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blazor_Serverside_Programming.Migrations.FileInfoDb
{
    /// <inheritdoc />
    public partial class AddFileVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HashAlgorithm",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VerificationHash",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VerificationKey",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "HashAlgorithm",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "VerificationHash",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "VerificationKey",
                table: "Files");
        }
    }
}
