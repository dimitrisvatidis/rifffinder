using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rifffinder.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstrumentWanted",
                table: "Postings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstrumentWanted",
                table: "Postings");
        }
    }
}
