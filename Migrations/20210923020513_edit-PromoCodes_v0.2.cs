using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopIn_API.Migrations
{
    public partial class editPromoCodes_v02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserPromoCodes",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PromoCodeId = table.Column<int>(type: "int", nullable: false),
                    isUsedPromoCode = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserPromoCodes", x => new { x.PromoCodeId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserPromoCodes_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserPromoCodes_PromoCodes_PromoCodeId",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserPromoCodes_ApplicationUserId",
                table: "ApplicationUserPromoCodes",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserPromoCodes");
        }
    }
}
