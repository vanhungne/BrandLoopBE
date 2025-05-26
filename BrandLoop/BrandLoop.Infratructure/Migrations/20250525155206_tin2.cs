using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class tin2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "InfluenceProfiles");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DayOfBirth",
                table: "InfluenceProfiles",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfBirth",
                table: "InfluenceProfiles");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "InfluenceProfiles",
                type: "int",
                nullable: true);
        }
    }
}
