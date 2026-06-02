using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsterAptitude.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionTimeLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeLimitSeconds",
                table: "TestSections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeLimitSeconds",
                table: "TestSections");
        }
    }
}
