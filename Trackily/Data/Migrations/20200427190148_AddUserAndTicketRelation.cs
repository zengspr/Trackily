using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trackily.Migrations
{
    public partial class AddUserAndTicketRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TrackilyUserId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    CreatorId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    RelatedFiles = table.Column<string>(nullable: true),
                    IsReviewed = table.Column<bool>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTickets",
                columns: table => new
                {
                    TrackilyUserId = table.Column<Guid>(nullable: false),
                    TicketId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTickets", x => new { x.TrackilyUserId, x.TicketId });
                    table.ForeignKey(
                        name: "FK_UserTickets_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTickets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatorId",
                table: "Tickets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTickets_TicketId",
                table: "UserTickets",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTickets_UserId",
                table: "UserTickets",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTickets");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropColumn(
                name: "TrackilyUserId",
                table: "AspNetUsers");
        }
    }
}
