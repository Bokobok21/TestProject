using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequestDriverChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "RequestDrivers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "RequestDrivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
