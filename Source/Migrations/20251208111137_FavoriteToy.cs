using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracking.Migrations
{
    /// <inheritdoc />
    public partial class FavoriteToy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FavoriteToy",
                table: "Coworkers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoriteToy",
                table: "Coworkers");
        }
    }
}
