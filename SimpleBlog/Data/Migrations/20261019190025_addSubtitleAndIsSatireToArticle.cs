using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class addSubtitleAndIsSatireToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSatire",
                table: "Articles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "Articles",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSatire",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "Articles");
        }
    }
}
