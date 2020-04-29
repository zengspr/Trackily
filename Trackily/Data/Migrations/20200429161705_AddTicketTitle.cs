using Microsoft.EntityFrameworkCore.Migrations;

namespace Trackily.Migrations
{
    public partial class AddTicketTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedFiles",
                table: "Tickets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelatedFiles",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
