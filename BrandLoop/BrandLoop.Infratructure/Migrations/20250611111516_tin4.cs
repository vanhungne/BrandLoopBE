using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class tin4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InfluencerTypeId",
                table: "InfluenceProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InfluencerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MinFollower = table.Column<int>(type: "int", nullable: false),
                    MaxFollower = table.Column<int>(type: "int", nullable: false),
                    PlatformFee = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfluencerTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "InfluencerTypes",
                columns: new[] { "Id", "MaxFollower", "MinFollower", "Name", "PlatformFee" },
                values: new object[,]
                {
                    { 1, 10000, 0, "Norman", 10000 },
                    { 2, 50000, 10000, "Nano Influencers", 100000 },
                    { 3, 100000, 50000, "Micro Influencers", 200000 },
                    { 4, 500000, 100000, "Mid-Tier Influencers", 300000 },
                    { 5, 1000000, 500000, "Macro Influencers", 500000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfluenceProfiles_InfluencerTypeId",
                table: "InfluenceProfiles",
                column: "InfluencerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InfluenceProfiles_InfluencerTypes_InfluencerTypeId",
                table: "InfluenceProfiles",
                column: "InfluencerTypeId",
                principalTable: "InfluencerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InfluenceProfiles_InfluencerTypes_InfluencerTypeId",
                table: "InfluenceProfiles");

            migrationBuilder.DropTable(
                name: "InfluencerTypes");

            migrationBuilder.DropIndex(
                name: "IX_InfluenceProfiles_InfluencerTypeId",
                table: "InfluenceProfiles");

            migrationBuilder.DropColumn(
                name: "InfluencerTypeId",
                table: "InfluenceProfiles");
        }
    }
}
