using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class EmailAndCreatedOnOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestampUtc",
                table: "Order",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Order",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTimestampUtc",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Order");
        }
    }
}
