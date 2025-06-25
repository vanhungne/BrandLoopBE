using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandLoop.Infratructure.Migrations
{
    /// <inheritdoc />
    public partial class hungdb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_UID",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "UID",
                table: "Feedbacks",
                newName: "ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_UID",
                table: "Feedbacks",
                newName: "IX_Feedbacks_ToUserId");

            migrationBuilder.AddColumn<string>(
                name: "FromUserId",
                table: "Feedbacks",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserUID",
                table: "Feedbacks",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_FromUserId",
                table: "Feedbacks",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserUID",
                table: "Feedbacks",
                column: "UserUID");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_FromUserId",
                table: "Feedbacks",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "UID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_ToUserId",
                table: "Feedbacks",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "UID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_UserUID",
                table: "Feedbacks",
                column: "UserUID",
                principalTable: "Users",
                principalColumn: "UID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_FromUserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_ToUserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_UserUID",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_FromUserId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_UserUID",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "UserUID",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "Feedbacks",
                newName: "UID");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_ToUserId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_UID");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_UID",
                table: "Feedbacks",
                column: "UID",
                principalTable: "Users",
                principalColumn: "UID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
