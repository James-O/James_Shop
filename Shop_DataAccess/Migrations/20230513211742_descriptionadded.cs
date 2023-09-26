using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_DataA.Migrations
{
    /// <inheritdoc />
    public partial class descriptionadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ApplicationTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ApplicationTypes");
        }
    }
}
