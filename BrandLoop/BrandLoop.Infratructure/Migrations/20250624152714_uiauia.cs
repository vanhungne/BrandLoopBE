using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class uiauia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "CampaignReports");

            migrationBuilder.RenameColumn(
                name: "Revenue",
                table: "CampaignReports",
                newName: "ROAS");

            migrationBuilder.RenameColumn(
                name: "Reach",
                table: "CampaignReports",
                newName: "TotalReach");

            migrationBuilder.RenameColumn(
                name: "Engagement",
                table: "CampaignReports",
                newName: "TotalImpressions");

            migrationBuilder.RenameColumn(
                name: "CustomerAmount",
                table: "CampaignReports",
                newName: "TotalEngagement");

            migrationBuilder.AddColumn<string>(
                name: "PaymentLink",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InfluencerReportId",
                table: "KolsJoinCampaigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AvgEngagementRate",
                table: "CampaignReports",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPerEngagement",
                table: "CampaignReports",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalClicks",
                table: "CampaignReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRevenue",
                table: "CampaignReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSpend",
                table: "CampaignReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "InfluencerReports",
                columns: table => new
                {
                    InfluencerReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalReach = table.Column<int>(type: "int", nullable: false),
                    TotalImpressions = table.Column<int>(type: "int", nullable: false),
                    TotalEngagement = table.Column<int>(type: "int", nullable: false),
                    AvgEngagementRate = table.Column<double>(type: "float", nullable: false),
                    TotalClicks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfluencerReports", x => x.InfluencerReportId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KolsJoinCampaigns_InfluencerReportId",
                table: "KolsJoinCampaigns",
                column: "InfluencerReportId",
                unique: true,
                filter: "[InfluencerReportId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_KolsJoinCampaigns_InfluencerReports_InfluencerReportId",
                table: "KolsJoinCampaigns",
                column: "InfluencerReportId",
                principalTable: "InfluencerReports",
                principalColumn: "InfluencerReportId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KolsJoinCampaigns_InfluencerReports_InfluencerReportId",
                table: "KolsJoinCampaigns");

            migrationBuilder.DropTable(
                name: "InfluencerReports");

            migrationBuilder.DropIndex(
                name: "IX_KolsJoinCampaigns_InfluencerReportId",
                table: "KolsJoinCampaigns");

            migrationBuilder.DropColumn(
                name: "PaymentLink",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "InfluencerReportId",
                table: "KolsJoinCampaigns");

            migrationBuilder.DropColumn(
                name: "AvgEngagementRate",
                table: "CampaignReports");

            migrationBuilder.DropColumn(
                name: "CostPerEngagement",
                table: "CampaignReports");

            migrationBuilder.DropColumn(
                name: "TotalClicks",
                table: "CampaignReports");

            migrationBuilder.DropColumn(
                name: "TotalRevenue",
                table: "CampaignReports");

            migrationBuilder.DropColumn(
                name: "TotalSpend",
                table: "CampaignReports");

            migrationBuilder.RenameColumn(
                name: "TotalReach",
                table: "CampaignReports",
                newName: "Reach");

            migrationBuilder.RenameColumn(
                name: "TotalImpressions",
                table: "CampaignReports",
                newName: "Engagement");

            migrationBuilder.RenameColumn(
                name: "TotalEngagement",
                table: "CampaignReports",
                newName: "CustomerAmount");

            migrationBuilder.RenameColumn(
                name: "ROAS",
                table: "CampaignReports",
                newName: "Revenue");

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "CampaignReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
