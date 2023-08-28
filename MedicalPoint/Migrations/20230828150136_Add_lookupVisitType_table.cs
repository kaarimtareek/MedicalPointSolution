using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class Add_lookupVisitType_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "VisitRests");

            migrationBuilder.AddColumn<int>(
                name: "RestTypeId",
                table: "VisitRests",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LookupVisitRestTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupVisitRestTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitRests_RestTypeId",
                table: "VisitRests",
                column: "RestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRests_LookupVisitRestTypes_RestTypeId",
                table: "VisitRests",
                column: "RestTypeId",
                principalTable: "LookupVisitRestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitRests_LookupVisitRestTypes_RestTypeId",
                table: "VisitRests");

            migrationBuilder.DropTable(
                name: "LookupVisitRestTypes");

            migrationBuilder.DropIndex(
                name: "IX_VisitRests_RestTypeId",
                table: "VisitRests");

            migrationBuilder.DropColumn(
                name: "RestTypeId",
                table: "VisitRests");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "VisitRests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
