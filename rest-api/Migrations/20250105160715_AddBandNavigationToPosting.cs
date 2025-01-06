using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rifffinder.Migrations
{
    /// <inheritdoc />
    public partial class AddBandNavigationToPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Postings_BandId",
                table: "Postings",
                column: "BandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Postings_Bands_BandId",
                table: "Postings",
                column: "BandId",
                principalTable: "Bands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Postings_Bands_BandId",
                table: "Postings");

            migrationBuilder.DropIndex(
                name: "IX_Postings_BandId",
                table: "Postings");
        }
    }
}
