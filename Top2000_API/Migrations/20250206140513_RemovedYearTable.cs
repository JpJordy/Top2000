using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Top2000_API.Migrations
{
    /// <inheritdoc />
    public partial class RemovedYearTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lijsten_Top2000Jaren_Top2000JaarId",
                table: "Lijsten");

            migrationBuilder.DropTable(
                name: "Top2000Jaren");

            migrationBuilder.DropIndex(
                name: "IX_Lijsten_Top2000JaarId",
                table: "Lijsten");

            migrationBuilder.RenameColumn(
                name: "Top2000JaarId",
                table: "Lijsten",
                newName: "Jaar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Jaar",
                table: "Lijsten",
                newName: "Top2000JaarId");

            migrationBuilder.CreateTable(
                name: "Top2000Jaren",
                columns: table => new
                {
                    Top2000JaarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jaar = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Top2000Jaren", x => x.Top2000JaarId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lijsten_Top2000JaarId",
                table: "Lijsten",
                column: "Top2000JaarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lijsten_Top2000Jaren_Top2000JaarId",
                table: "Lijsten",
                column: "Top2000JaarId",
                principalTable: "Top2000Jaren",
                principalColumn: "Top2000JaarId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
