using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trackily.Migrations
{
    public partial class ChangeKeyOfTrackilyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTickets",
                table: "UserTickets");

            migrationBuilder.DropColumn(
                name: "TrackilyUserId",
                table: "UserTickets");

            migrationBuilder.DropColumn(
                name: "TrackilyUserId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserTickets",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTickets",
                table: "UserTickets",
                columns: new[] { "Id", "TicketId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTickets",
                table: "UserTickets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserTickets");

            migrationBuilder.AddColumn<Guid>(
                name: "TrackilyUserId",
                table: "UserTickets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TrackilyUserId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTickets",
                table: "UserTickets",
                columns: new[] { "TrackilyUserId", "TicketId" });
        }
    }
}
