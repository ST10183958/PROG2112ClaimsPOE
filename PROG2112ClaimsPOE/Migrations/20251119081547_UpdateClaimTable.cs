using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG2112ClaimsPOE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClaimTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Payment",
                table: "ClaimTable",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payment",
                table: "ClaimTable");
        }
    }
}
