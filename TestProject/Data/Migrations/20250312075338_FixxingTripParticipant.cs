using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixxingTripParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripParticipants_AspNetUsers_ApplicationUserId",
                table: "TripParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_TripParticipants_Trips_TripId1",
                table: "TripParticipants");

            migrationBuilder.DropIndex(
                name: "IX_TripParticipants_ApplicationUserId",
                table: "TripParticipants");

            migrationBuilder.DropIndex(
                name: "IX_TripParticipants_TripId1",
                table: "TripParticipants");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "TripParticipants");

            migrationBuilder.DropColumn(
                name: "TripId1",
                table: "TripParticipants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "TripParticipants",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TripId1",
                table: "TripParticipants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripParticipants_ApplicationUserId",
                table: "TripParticipants",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TripParticipants_TripId1",
                table: "TripParticipants",
                column: "TripId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TripParticipants_AspNetUsers_ApplicationUserId",
                table: "TripParticipants",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TripParticipants_Trips_TripId1",
                table: "TripParticipants",
                column: "TripId1",
                principalTable: "Trips",
                principalColumn: "Id");
        }
    }
}
