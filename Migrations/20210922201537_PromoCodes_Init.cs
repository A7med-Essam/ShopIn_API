using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopIn_API.Migrations
{
    public partial class PromoCodes_Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    promoCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserPromoCode",
                columns: table => new
                {
                    applicationUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    promoCodesid = table.Column<int>(type: "int", nullable: false)
                 },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserPromoCode", x => new { x.applicationUsersId, x.promoCodesid });
                    table.ForeignKey(
                        name: "FK_ApplicationUserPromoCode_AspNetUsers_applicationUsersId",
                        column: x => x.applicationUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserPromoCode_PromoCodes_promoCodesid",
                        column: x => x.promoCodesid,
                        principalTable: "PromoCodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserPromoCode_promoCodesid",
                table: "ApplicationUserPromoCode",
                column: "promoCodesid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserPromoCode");

            migrationBuilder.DropTable(
                name: "PromoCodes");
        }
    }
}
