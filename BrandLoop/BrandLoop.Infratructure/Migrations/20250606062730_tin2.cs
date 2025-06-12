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
            // Bỏ constraint PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            // Xóa cột PaymentId
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Payments");

            // Thêm lại cột PaymentId không có Identity
            migrationBuilder.AddColumn<long>(
                name: "PaymentId",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            // Thêm lại Primary Key
            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentId");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Payments");

            migrationBuilder.AddColumn<long>(
                name: "PaymentId",
                table: "Payments",
                type: "bigint",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
