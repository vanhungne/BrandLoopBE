using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class hungdb3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CampaignReports_CampaignId",
                table: "CampaignReports");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "CampaignInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignReports_CampaignId",
                table: "CampaignReports",
                column: "CampaignId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CampaignReports_CampaignId",
                table: "CampaignReports");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "CampaignInvitations");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignReports_CampaignId",
                table: "CampaignReports",
                column: "CampaignId");
        }
    }
}
