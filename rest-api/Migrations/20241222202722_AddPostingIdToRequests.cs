using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rifffinder.Migrations
{
    /// <inheritdoc />
    public partial class AddPostingIdToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostingId",
                table: "Requests",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostingId",
                table: "Requests");
        }
    }
}
