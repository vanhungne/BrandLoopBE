using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class updateda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_KolsJoinCampaigns_KolsJoinCampaignId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "KolsJoinCampaignId",
                table: "Payments",
                newName: "CampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_KolsJoinCampaignId",
                table: "Payments",
                newName: "IX_Payments_CampaignId");

            // ➤ DROP PRIMARY KEY trước khi đổi kiểu
            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.AlterColumn<long>(
                name: "PaymentId",
                table: "Payments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            // ➤ ADD PRIMARY KEY lại sau khi đổi kiểu
            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentId");

            migrationBuilder.CreateTable(
                name: "CampainImages",
                columns: table => new
                {
                    CampaignImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampainImages", x => x.CampaignImageId);
                    table.ForeignKey(
                        name: "FK_CampainImages_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampainImages_CampaignId",
                table: "CampainImages",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Campaigns_CampaignId",
                table: "Payments",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "CampaignId",
                onDelete: ReferentialAction.Restrict);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Campaigns_CampaignId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "CampainImages");

            migrationBuilder.RenameColumn(
                name: "CampaignId",
                table: "Payments",
                newName: "KolsJoinCampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_CampaignId",
                table: "Payments",
                newName: "IX_Payments_KolsJoinCampaignId");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_KolsJoinCampaigns_KolsJoinCampaignId",
                table: "Payments",
                column: "KolsJoinCampaignId",
                principalTable: "KolsJoinCampaigns",
                principalColumn: "KolsJoinCampaignId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
