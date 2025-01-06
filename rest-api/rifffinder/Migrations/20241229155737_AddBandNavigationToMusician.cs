using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rifffinder.Migrations
{
    /// <inheritdoc />
    public partial class AddBandNavigationToMusician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Musicians_BandId",
                table: "Musicians",
                column: "BandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Musicians_Bands_BandId",
                table: "Musicians",
                column: "BandId",
                principalTable: "Bands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Musicians_Bands_BandId",
                table: "Musicians");

            migrationBuilder.DropIndex(
                name: "IX_Musicians_BandId",
                table: "Musicians");
        }
    }
}
