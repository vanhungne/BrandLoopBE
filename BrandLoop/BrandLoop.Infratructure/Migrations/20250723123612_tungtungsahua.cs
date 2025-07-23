using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class tungtungsahua : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KolsJoinCampaignId",
                table: "Evidences",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "InfluencerTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Entry-Level Influence");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_KolsJoinCampaignId",
                table: "Evidences",
                column: "KolsJoinCampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evidences_KolsJoinCampaigns_KolsJoinCampaignId",
                table: "Evidences",
                column: "KolsJoinCampaignId",
                principalTable: "KolsJoinCampaigns",
                principalColumn: "KolsJoinCampaignId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evidences_KolsJoinCampaigns_KolsJoinCampaignId",
                table: "Evidences");

            migrationBuilder.DropIndex(
                name: "IX_Evidences_KolsJoinCampaignId",
                table: "Evidences");

            migrationBuilder.DropColumn(
                name: "KolsJoinCampaignId",
                table: "Evidences");

            migrationBuilder.UpdateData(
                table: "InfluencerTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Norman");
        }
    }
}
