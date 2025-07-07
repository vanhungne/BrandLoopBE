using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEvidenceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evidence_InfluencerReports_InfluencerReportId",
                table: "Evidence");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Evidence",
                table: "Evidence");

            migrationBuilder.RenameTable(
                name: "Evidence",
                newName: "Evidences");

            migrationBuilder.RenameIndex(
                name: "IX_Evidence_InfluencerReportId",
                table: "Evidences",
                newName: "IX_Evidences_InfluencerReportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Evidences",
                table: "Evidences",
                column: "EvidenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evidences_InfluencerReports_InfluencerReportId",
                table: "Evidences",
                column: "InfluencerReportId",
                principalTable: "InfluencerReports",
                principalColumn: "InfluencerReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evidences_InfluencerReports_InfluencerReportId",
                table: "Evidences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Evidences",
                table: "Evidences");

            migrationBuilder.RenameTable(
                name: "Evidences",
                newName: "Evidence");

            migrationBuilder.RenameIndex(
                name: "IX_Evidences_InfluencerReportId",
                table: "Evidence",
                newName: "IX_Evidence_InfluencerReportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Evidence",
                table: "Evidence",
                column: "EvidenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evidence_InfluencerReports_InfluencerReportId",
                table: "Evidence",
                column: "InfluencerReportId",
                principalTable: "InfluencerReports",
                principalColumn: "InfluencerReportId");
        }
    }
}
