using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rifffinder.Migrations
{
    /// <inheritdoc />
    public partial class MakeBandIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Musicians_Bands_BandId",
                table: "Musicians");

            migrationBuilder.AlterColumn<int>(
                name: "BandId",
                table: "Musicians",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Musicians_Bands_BandId",
                table: "Musicians",
                column: "BandId",
                principalTable: "Bands",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Musicians_Bands_BandId",
                table: "Musicians");

            migrationBuilder.AlterColumn<int>(
                name: "BandId",
                table: "Musicians",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Musicians_Bands_BandId",
                table: "Musicians",
                column: "BandId",
                principalTable: "Bands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
