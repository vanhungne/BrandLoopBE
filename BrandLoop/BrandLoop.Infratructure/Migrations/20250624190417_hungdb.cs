using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class hungdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bỏ khóa ngoại và chỉ số cũ
            migrationBuilder.DropForeignKey(
                name: "FK_KolsJoinCampaigns_InfluencerReports_InfluencerReportId",
                table: "KolsJoinCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_KolsJoinCampaigns_InfluencerReportId",
                table: "KolsJoinCampaigns");

            // Xoá cột khóa ngoại cũ từ KolsJoinCampaigns
            migrationBuilder.DropColumn(
                name: "InfluencerReportId",
                table: "KolsJoinCampaigns");

            // Xoá cột EditedAt từ Feedbacks
            migrationBuilder.DropColumn(
                name: "EditedAt",
                table: "Feedbacks");

            // Xoá bảng InfluencerReports để tạo lại (vì không thể sửa IDENTITY)
            migrationBuilder.DropTable(
                name: "InfluencerReports");

            // Tạo lại bảng InfluencerReports với InfluencerReportId không có IDENTITY
            migrationBuilder.CreateTable(
                name: "InfluencerReports",
                columns: table => new
                {
                    InfluencerReportId = table.Column<int>(nullable: false), // PK & FK
                    TotalContent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalReach = table.Column<int>(nullable: false),
                    TotalImpressions = table.Column<int>(nullable: false),
                    TotalEngagement = table.Column<int>(nullable: false),
                    AvgEngagementRate = table.Column<double>(nullable: false),
                    TotalClicks = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfluencerReports", x => x.InfluencerReportId);
                    table.ForeignKey(
                        name: "FK_InfluencerReports_KolsJoinCampaigns_InfluencerReportId",
                        column: x => x.InfluencerReportId,
                        principalTable: "KolsJoinCampaigns",
                        principalColumn: "KolsJoinCampaignId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Thêm cột FeedbackFrom mới vào Feedbacks
            migrationBuilder.AddColumn<int>(
                name: "FeedbackFrom",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop lại bảng InfluencerReports
            migrationBuilder.DropTable(
                name: "InfluencerReports");

            // Xóa cột FeedbackFrom
            migrationBuilder.DropColumn(
                name: "FeedbackFrom",
                table: "Feedbacks");

            // Thêm lại cột EditedAt
            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAt",
                table: "Feedbacks",
                type: "datetime2",
                nullable: true);

            // Thêm lại cột InfluencerReportId vào bảng KolsJoinCampaigns
            migrationBuilder.AddColumn<int>(
                name: "InfluencerReportId",
                table: "KolsJoinCampaigns",
                type: "int",
                nullable: true);

            // Tạo lại bảng InfluencerReports cũ có IDENTITY
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

            // Tạo lại chỉ số và khóa ngoại
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
    }
}
