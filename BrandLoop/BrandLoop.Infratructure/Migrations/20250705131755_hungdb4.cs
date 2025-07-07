using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class hungdb4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InfluencerEarning",
                table: "KolsJoinCampaigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Evidence",
                columns: table => new
                {
                    EvidenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EvidenceOf = table.Column<int>(type: "int", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InfluencerReportId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidence", x => x.EvidenceId);
                    table.ForeignKey(
                        name: "FK_Evidence_InfluencerReports_InfluencerReportId",
                        column: x => x.InfluencerReportId,
                        principalTable: "InfluencerReports",
                        principalColumn: "InfluencerReportId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evidence_InfluencerReportId",
                table: "Evidence",
                column: "InfluencerReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evidence");

            migrationBuilder.DropColumn(
                name: "InfluencerEarning",
                table: "KolsJoinCampaigns");
        }
    }
}
